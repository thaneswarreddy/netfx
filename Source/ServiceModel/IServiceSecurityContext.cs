using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.IdentityModel.Policy;

namespace System.ServiceModel
{
	public interface IServiceSecurityContext
	{
		AuthorizationContext AuthorizationContext { get; }
		ReadOnlyCollection<IAuthorizationPolicy> AuthorizationPolicies { get; }
		bool IsAnonymous { get; }
		IIdentity PrimaryIdentity { get; }
		WindowsIdentity WindowsIdentity { get; }
	}
}
