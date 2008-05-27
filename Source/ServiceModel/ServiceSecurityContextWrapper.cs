using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ServiceModel
{
	public class ServiceSecurityContextWrapper : IServiceSecurityContext
	{
		ServiceSecurityContext context;

		public ServiceSecurityContextWrapper(ServiceSecurityContext context)
		{
			this.context = context;
		}

		#region IServiceSecurityContext Members

		public System.IdentityModel.Policy.AuthorizationContext AuthorizationContext
		{
			get
			{
				return this.context.AuthorizationContext;
			}
		}

		public System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> AuthorizationPolicies
		{
			get
			{
				return this.context.AuthorizationPolicies;
			}
		}

		public bool IsAnonymous
		{
			get
			{
				return this.context.IsAnonymous;
			}
		}

		public System.Security.Principal.IIdentity PrimaryIdentity
		{
			get
			{
				return this.context.PrimaryIdentity;
			}
		}

		public System.Security.Principal.WindowsIdentity WindowsIdentity
		{
			get
			{
				return this.context.WindowsIdentity;
			}
		}

		#endregion
	}
}
