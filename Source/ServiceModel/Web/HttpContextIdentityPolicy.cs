using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IdentityModel.Policy;
using System.Security.Principal;
using System.IdentityModel.Claims;

namespace System.ServiceModel.Web
{
	/// <summary>
	/// Propagates the <see cref="HttpContext.Current.User"/> as a claim 
	/// in WCF.
	/// </summary>
	/// <remarks>
	/// This policy must be added to the service behavior, such as via 
	/// configuration as explained in <see cref="http://msdn.microsoft.com/en-us/library/ms751416.aspx">Authorization Policy</see> 
	/// and <see cref="http://msdn.microsoft.com/en-us/library/system.identitymodel.policy.iauthorizationpolicy.aspx">IAuthorizationPolicy Interface</see>.
	/// </remarks>
	/// <example>
	/// The following example shows the <c>authorizationPolicies</c> configuration element:
	/// <code>
	/// &lt;authorizationPolicies&gt;
	/// &lt;add policyType="System.ServiceModel.Web.HttpContextIdentityPolicy, NetFx" /&gt;
	/// &lt;/authorizationPolicies&gt;
	/// </code>
	/// Replace NetFx with your assembly name if you're linking to this file instead.
	/// </example>
	public class HttpContextIdentityPolicy : IAuthorizationPolicy
	{
		public HttpContextIdentityPolicy()
		{
		}

		public bool Evaluate(EvaluationContext evaluationContext, ref object state)
		{
			IPrincipal principal = null;
			if (HttpContext.Current != null)
				principal = HttpContext.Current.User;

			if (principal != null)
			{
				// set the identity (for PrimaryIdentity)
				evaluationContext.Properties["Identities"] =
					new List<IIdentity>() { principal.Identity };

				evaluationContext.Properties["Principal"] = principal;

				var nameClaim = Claim.CreateNameClaim(principal.Identity.Name);
				ClaimSet set;

				if (HttpContext.Current != null)
				{
					set = new DefaultClaimSet(
						nameClaim, 
						new Claim(
							ClaimTypes.Authentication,
							HttpContext.Current.User.Identity,
							Rights.Identity)
					);
				}
				else
				{
					set = new DefaultClaimSet(nameClaim);
				}

				evaluationContext.AddClaimSet(this, set);
			}

			return true;
		}

		public System.IdentityModel.Claims.ClaimSet Issuer
		{
			get { return ClaimSet.System; }
		}

		public string Id
		{
			get { return "HttpContextIdentityPolicy"; }
		}
	}
}
