using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;


    public class SubMonoBehaviorAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(SubMonoBehavior);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adaptor);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

		internal class Adaptor : SubMonoBehavior, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adaptor()
            {

            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            
            IMethod mStart;
            public override void Start()
            {
                if(mStart == null)
                {
                    mStart = instance.Type.GetMethod("Start", 0);
                }
                if (mStart != null)
                    appdomain.Invoke(mStart, instance );
            }

            IMethod mUpdate;
            public override void Update()
            {
                if(mUpdate == null)
                {
                    mUpdate = instance.Type.GetMethod("Update", 0);
                }
                if (mUpdate != null)
                    appdomain.Invoke(mUpdate, instance );
            }

            IMethod mOnFixedUpdate;
            public override void OnFixedUpdate()
            {
                if(mOnFixedUpdate == null)
                {
                    mOnFixedUpdate = instance.Type.GetMethod("OnFixedUpdate", 0);
                }
                if (mOnFixedUpdate != null)
                    appdomain.Invoke(mOnFixedUpdate, instance );
            }

            IMethod mOnDestroy;
            public override void OnDestroy()
            {
                if(mOnDestroy == null)
                {
                    mOnDestroy = instance.Type.GetMethod("OnDestroy", 0);
                }
                if (mOnDestroy != null)
                    appdomain.Invoke(mOnDestroy, instance );
            }

            IMethod mOnApplicationQuit;
            public override void OnApplicationQuit()
            {
                if(mOnApplicationQuit == null)
                {
                    mOnApplicationQuit = instance.Type.GetMethod("OnApplicationQuit", 0);
                }
                if (mOnApplicationQuit != null)
                    appdomain.Invoke(mOnApplicationQuit, instance );
            }

            
            
            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }

	
