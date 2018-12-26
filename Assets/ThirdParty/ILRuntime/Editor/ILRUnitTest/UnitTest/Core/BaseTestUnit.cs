//-----------------------------------------------------------------------
// <filename>BaseTestUnit</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/17 星期一 14:20:45# </time>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace GameFramework.Taurus.UnityEditor
{
    public abstract class BaseTestUnit : ITestable
    {
        protected AppDomain App;
        protected string AssemblyName;
        protected string TypeName;
        protected string MethodName;
        public bool Pass;
        protected StringBuilder Message = null;

        public string TestName { get { return TypeName + "." + MethodName; } }

        #region 接口方法

        public bool Init(string fileName)
        {
            AssemblyName = fileName;
            if (!File.Exists(AssemblyName))
                return false;
            using (var fs = new System.IO.FileStream(AssemblyName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                App = new ILRuntime.Runtime.Enviorment.AppDomain();
                var path = System.IO.Path.GetDirectoryName(AssemblyName);
                var name = System.IO.Path.GetFileNameWithoutExtension(AssemblyName);
                using (var fs2 = new System.IO.FileStream(string.Format("{0}\\{1}.pdb", path, name), System.IO.FileMode.Open))
                    App.LoadAssembly(fs, fs2, new Mono.Cecil.Pdb.PdbReaderProvider());
            }

            return true;
        }

        public bool Init(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            if (app == null)
                return false;

            App = app;
            return true;
        }

        public bool Init(ILRuntime.Runtime.Enviorment.AppDomain app, string type, string method)
        {
            if (app == null)
                return false;

            TypeName = type;
            MethodName = method;

            App = app;
            return true;
        }

        //需要子类去实现
        public abstract void Run();

        public abstract bool Check();

        public abstract TestResultInfo CheckResult();

        #endregion

        #region 常用工具方法
        /// <summary>
        /// call Method with no params and no return value;
        /// </summary>
        /// <param name="Instance">Instacne, if it is null means static method,else means instance method</param>
        /// <param name="type">TypeName ,eg "Namespace.ClassType"</param>
        /// <param name="method">MethodName</param>
        public void Invoke(Object Instance, string type, string method)
        {
            Message = new StringBuilder();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            App.Invoke(type, method, null); //InstanceTest
            sw.Stop();
            Message.AppendLine("Elappsed Time:" + sw.ElapsedMilliseconds + "ms\n");
        }

        public void Invoke(string type, string method)
        {
            Message = new StringBuilder();
            try
            {
                var sw = new System.Diagnostics.Stopwatch();
                //UnityEngine.Debug.Log(string.Format("Invoking {0}.{1}", type, method));
                //Console.WriteLine("Invoking {0}.{1}", type, method);
                sw.Start();
                var res = App.Invoke(type, method, null); //InstanceTest
                sw.Stop();
                if (res != null)
                    Message.AppendLine("Return:" + res);
                Message.AppendLine("Elappsed Time:" + sw.ElapsedMilliseconds + "ms\n");
                Pass = true;
            }
            catch (ILRuntime.Runtime.Intepreter.ILRuntimeException e)
            {
                Message.AppendLine(e.Message);
                if (!string.IsNullOrEmpty(e.ThisInfo))
                {
                    Message.AppendLine("this:");
                    Message.AppendLine(e.ThisInfo);
                }
                Message.AppendLine("Local Variables:");
                Message.AppendLine(e.LocalInfo);
                Message.AppendLine(e.StackTrace);
                if (e.InnerException != null)
                    Message.AppendLine(e.InnerException.ToString());
                Pass = false;
            }
            catch (Exception ex)
            {
                Message.AppendLine(ex.ToString());
                Pass = false;
            }
        }
        #endregion
    }
}
