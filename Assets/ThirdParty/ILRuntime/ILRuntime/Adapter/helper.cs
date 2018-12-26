
using System;

namespace ILRuntime
{
    public class ILRuntimeHelper
    {
        public static void Init(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            if (app == null)
            {
                // should log error
                return;
            }

			// adaptor register 
                        
			app.RegisterCrossBindingAdaptor(new SubMonoBehaviorAdaptor());   

			// interface adaptor register
			            
			app.RegisterCrossBindingAdaptor(new IDisposableAdaptor());

			// delegate register 
						
			app.DelegateManager.RegisterMethodDelegate<System.String,System.Int32>();
			
			app.DelegateManager.RegisterMethodDelegate<System.String>();
			
			app.DelegateManager.RegisterFunctionDelegate<System.String,System.String>();


			// delegate convertor
            
        }
    }
}