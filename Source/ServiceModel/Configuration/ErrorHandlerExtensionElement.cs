using System.ServiceModel.Description;

namespace System.ServiceModel.Configuration
{
	public class ErrorHandlerExtensionElement : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof(ErrorHanderBehavior); }
		}

		protected override object CreateBehavior()
		{
			return new ErrorHanderBehavior(new ErrorHandler());
		}
	}
}
