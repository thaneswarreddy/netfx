using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Policy;
using System.Collections.ObjectModel;
using System.Security.Principal;

namespace System.ServiceModel
{
	/// <summary>
	/// Provides an implementation of <see cref="IServiceSecurityContext"/> that 
	/// wraps the underlying runtime <see cref="ServiceSecurityContext"/>.
	/// </summary>
	public class ServiceSecurityContextWrapper : IServiceSecurityContext
	{
		ServiceSecurityContext context;

		public ServiceSecurityContextWrapper(ServiceSecurityContext context)
		{
			this.context = context;
		}

		AuthorizationContext IServiceSecurityContext.AuthorizationContext
		{
			get
			{
				return this.context.AuthorizationContext;
			}
		}

		 ReadOnlyCollection<IAuthorizationPolicy> IServiceSecurityContext.AuthorizationPolicies
		{
			get
			{
				return this.context.AuthorizationPolicies;
			}
		}

		 bool IServiceSecurityContext.IsAnonymous
		{
			get
			{
				return this.context.IsAnonymous;
			}
		}

		 IIdentity IServiceSecurityContext.PrimaryIdentity
		{
			get
			{
				return this.context.PrimaryIdentity;
			}
		}

		 WindowsIdentity IServiceSecurityContext.WindowsIdentity
		{
			get
			{
				return this.context.WindowsIdentity;
			}
		}
	}
}
