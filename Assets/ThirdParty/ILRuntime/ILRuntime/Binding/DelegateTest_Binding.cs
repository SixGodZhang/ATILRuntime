using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class DelegateTest_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::DelegateTest);

            field = type.GetField("delegateTest01", flag);
            app.RegisterCLRFieldGetter(field, get_delegateTest01_0);
            app.RegisterCLRFieldSetter(field, set_delegateTest01_0);
            field = type.GetField("actionTest02", flag);
            app.RegisterCLRFieldGetter(field, get_actionTest02_1);
            app.RegisterCLRFieldSetter(field, set_actionTest02_1);
            field = type.GetField("funcTest03", flag);
            app.RegisterCLRFieldGetter(field, get_funcTest03_2);
            app.RegisterCLRFieldSetter(field, set_funcTest03_2);


        }



        static object get_delegateTest01_0(ref object o)
        {
            return global::DelegateTest.delegateTest01;
        }
        static void set_delegateTest01_0(ref object o, object v)
        {
            global::DelegateTest.delegateTest01 = (global::DelegateTest01)v;
        }
        static object get_actionTest02_1(ref object o)
        {
            return global::DelegateTest.actionTest02;
        }
        static void set_actionTest02_1(ref object o, object v)
        {
            global::DelegateTest.actionTest02 = (System.Action<System.String>)v;
        }
        static object get_funcTest03_2(ref object o)
        {
            return global::DelegateTest.funcTest03;
        }
        static void set_funcTest03_2(ref object o, object v)
        {
            global::DelegateTest.funcTest03 = (System.Func<System.String, System.String>)v;
        }


    }
}
