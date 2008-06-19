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
	/// &lt;add policyType="Instedd.GeoChat.HttpContextIdentityPolicy, Instedd.GeoChat.Server" /&gt;
	/// &lt;/authorizationPolicies&gt;
	/// </code>
	/// </example>
	public class HttpContextIdentityPolicy : IAuthorizationPolicy
	{
		IPrincipal principal;

		public HttpContextIdentityPolicy()
		{
			if (HttpContext.Current != null)
				this.principal = HttpContext.Current.User;
		}

		public HttpContextIdentityPolicy(IPrincipal principal)
		{
			this.principal = principal;
		}

		public bool Evaluate(EvaluationContext evaluationContext, ref object state)
		{
			if (this.principal != null)
			{
				// set the identity (for PrimaryIdentity)
				evaluationContext.Properties["Identities"] =
					new List<IIdentity>() { this.principal.Identity };

				evaluationContext.Properties["Principal"] = principal;

				// add a claim set containing the client name
				ClaimSet set = new DefaultClaimSet(
						Claim.CreateNameClaim(this.principal.Identity.Name), 
						new Claim(
							ClaimTypes.Authentication,
							HttpContext.Current.User.Identity,
							Rights.Identity)
				);					
				
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
