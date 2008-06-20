using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using System.IdentityModel.Policy;
using System.Security.Principal;
using System.IdentityModel.Claims;
using System.ServiceModel.Web;
using System.Web;

namespace NetFx.UnitTests.Web
{
	[TestFixture]
	public class HttpContextIdentityPolicyFixture
	{
		[Test]
		public void ShouldSetIdentityProperty()
		{
			var evaluationContextMock = new Mock<EvaluationContext>();

			var properties = new Dictionary<string, object>();

			evaluationContextMock
				.ExpectGet(ec => ec.Properties)
				.Returns(properties);

			var principal = new GenericPrincipal(new GenericIdentity("foo"), null);
			var httpContext = new HttpContext(
				new HttpRequest("foo", "http://foo", ""),
				new HttpResponse(Console.Out));
			HttpContext.Current = httpContext;
			httpContext.User = principal;

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy();

			policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(properties.ContainsKey("Identities"));
			Assert.IsTrue(properties["Identities"] is List<IIdentity>);
			Assert.AreSame(principal.Identity, ((List<IIdentity>)properties["Identities"])[0]);
		}

		[Test]
		public void ShouldSetPrincipalProperty()
		{
			var evaluationContextMock = new Mock<EvaluationContext>();

			var properties = new Dictionary<string, object>();

			evaluationContextMock
				.ExpectGet(ec => ec.Properties)
				.Returns(properties);

			var principal = new GenericPrincipal(new GenericIdentity("foo"), null);
			var httpContext = new HttpContext(
				new HttpRequest("foo", "http://foo", ""),
				new HttpResponse(Console.Out));
			HttpContext.Current = httpContext;
			httpContext.User = principal;

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy();

			policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(properties.ContainsKey("Principal"));
			Assert.IsTrue(properties["Principal"] is IPrincipal);
			Assert.AreSame(principal, (IPrincipal)properties["Principal"]);
		}

		[Test]
		public void ShouldAddClaimSet()
		{
			var principal = new GenericPrincipal(new GenericIdentity("foo"), null);
			var httpContext = new HttpContext(
				new HttpRequest("foo", "http://foo", ""),
				new HttpResponse(Console.Out));
			HttpContext.Current = httpContext;
			httpContext.User = principal;

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy();

			var evaluationContextMock = new Mock<EvaluationContext>();

			var properties = new Dictionary<string, object>();

			evaluationContextMock
				.ExpectGet(ec => ec.Properties)
				.Returns(properties);

			evaluationContextMock.Expect(ec => ec.AddClaimSet(policy, It.IsAny<ClaimSet>())).Verifiable();
			
			policy.Evaluate(evaluationContextMock.Object, ref state);

			evaluationContextMock.Verify();
		}

		[Test]
		public void ShouldNotFailForNullHttpContext()
		{
			var evaluationContextMock = new Mock<EvaluationContext>();
			HttpContext.Current = null;

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy();

			bool result = policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(result);
		}

		[Test]
		public void ShouldNotFailForNullHttpContextUser()
		{
			var evaluationContextMock = new Mock<EvaluationContext>();
			var httpContext = new HttpContext(
				new HttpRequest("foo", "http://foo", ""),
				new HttpResponse(Console.Out));
			HttpContext.Current = httpContext;

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy();

			bool result = policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(result);
		}

		[Test]
		public void ShouldSetClaimForCurrentUserWithoutCachingIt()
		{
			var evaluationContextMock = new Mock<EvaluationContext>();
			var principal = new GenericPrincipal(new GenericIdentity("foo"), null);
			var httpContext = new HttpContext(
				new HttpRequest("foo", "http://foo", ""), 
				new HttpResponse(Console.Out));
			HttpContext.Current = httpContext;
			httpContext.User = principal;

			var properties = new Dictionary<string, object>();

			evaluationContextMock
				.ExpectGet(ec => ec.Properties)
				.Returns(properties);

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy();

			policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(properties.ContainsKey("Principal"));
			Assert.IsTrue(properties["Principal"] is IPrincipal);
			Assert.AreSame(principal, (IPrincipal)properties["Principal"]);

			// Change the current principal
			principal = new GenericPrincipal(new GenericIdentity("bar"), null);
			httpContext.User = principal;

			policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(properties.ContainsKey("Principal"));
			Assert.IsTrue(properties["Principal"] is IPrincipal);
			Assert.AreSame(principal, (IPrincipal)properties["Principal"]);
		}
	}
}
