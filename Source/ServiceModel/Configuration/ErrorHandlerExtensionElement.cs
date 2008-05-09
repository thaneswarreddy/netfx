using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace System.ServiceModel.Configuration
{
    public class ErrorHandlerExtensionElement: BehaviorExtensionElement
    {
        public override Type BehaviorType 
        {
            get { return typeof(ErrorHanderBehavior);}
        }

        protected override object CreateBehavior()
        {
            return new ErrorHanderBehavior(new ErrorHandler());
        }
    }
}
