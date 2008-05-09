using System.Globalization;
using System.Net;
using System.ServiceModel.Web;
using NUnit.Framework;

namespace System.ServiceModel.UnitTests
{
	[TestFixture]
	public class HttpClientCacheBehaviorFixture
	{
		[Test]
		public void OperationLevelExpiration()
		{
			TestService<TestServiceImpl>(x => x.Foo(), 500);
		}

		[Test]
		public void ServiceLevelExpiration()
		{
			TestService<TestServiceImpl2>(x => x.Foo(), 500);
		}

		[Test]
		public void OverrideServiceLevelExpiration()
		{
			TestService<TestServiceImpl2>(x => x.Bar(), 100);
		}

		[Test]
		public void ImmediateExpiration()
		{
			TestService<TestServiceImpl>(x => x.Bar(), 0);
		}

		private void TestService<T>(Action<ITestService> call, double expectedAge)
		{
			WebServiceHost host = new WebServiceHost(typeof(T), new Uri("http://localhost:7357"));
			host.Open();
			try
			{
				var factory = new WebChannelFactory<ITestService>(new Uri("http://localhost:7357"));
				var svc = factory.CreateChannel();
				using (new OperationContextScope(new OperationContext((IContextChannel)svc)))
				{
					call(svc);
					var headers = WebOperationContext.Current.IncomingResponse.Headers;

					Assert.IsNotNull(headers[HttpResponseHeader.Expires]);
					string format = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";
					var expires = DateTime.ParseExact(headers[HttpResponseHeader.Expires], format, CultureInfo.InvariantCulture);
					if (expectedAge > 0)
					{
						var diff = expires - DateTime.Now.ToUniversalTime();
						Assert.Less(Math.Abs(diff.TotalSeconds - expectedAge), 5);
						Assert.AreEqual("max-age=" + expectedAge, headers[HttpResponseHeader.CacheControl]);
					}
					else
					{
						Assert.AreEqual(DateTime.MinValue, expires);
						Assert.AreEqual("no-cache", headers[HttpResponseHeader.CacheControl]);
					}
				}
			}
			finally
			{
				host.Close();
			}
		}

		[ServiceContract]
		interface ITestService
		{
			[OperationContract]
			void Foo();

			[OperationContract]
			void Bar();
		}

		class TestServiceImpl : ITestService
		{
			[HttpClientCache(500)]
			public void Foo()
			{
			}

			[HttpClientCache(0)]
			public void Bar()
			{
			}
		}

		[HttpClientCache(500)]
		class TestServiceImpl2 : ITestService
		{
			public void Foo()
			{
			}

			[HttpClientCache(100)]
			public void Bar()
			{
			}
		}
	}
}
