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

			var identity = new GenericPrincipal(new GenericIdentity("foo"), null);

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy(identity);

			policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(properties.ContainsKey("Identities"));
			Assert.IsTrue(properties["Identities"] is List<IIdentity>);
			Assert.AreEqual(identity.Identity, ((List<IIdentity>)properties["Identities"])[0]);
		}

		[Test]
		public void ShouldSetPrincipalProperty()
		{
			var evaluationContextMock = new Mock<EvaluationContext>();

			var properties = new Dictionary<string, object>();

			evaluationContextMock
				.ExpectGet(ec => ec.Properties)
				.Returns(properties);

			var identity = new GenericPrincipal(new GenericIdentity("foo"), null);

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy(identity);

			policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(properties.ContainsKey("Principal"));
			Assert.IsTrue(properties["Principal"] is IPrincipal);
			Assert.AreEqual(identity, (IPrincipal)properties["Principal"]);
		}

		[Test]
		public void ShouldAddClaimSet()
		{
			var identity = new GenericPrincipal(new GenericIdentity("foo"), null);

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy(identity);

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
		public void ShouldNotFailForNullIdentity()
		{
			var evaluationContextMock = new Mock<EvaluationContext>();

			object state = null;
			HttpContextIdentityPolicy policy = new HttpContextIdentityPolicy(null);

			bool result = policy.Evaluate(evaluationContextMock.Object, ref state);

			Assert.IsTrue(result);
		}
	}
}
