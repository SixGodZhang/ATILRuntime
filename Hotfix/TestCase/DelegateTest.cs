using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.TestCase
{
    class DelegateTest
    {
        //static TestDelegate testDele;
        static Action<int> pInt_rVoid_Action;

        static Action<string> pString_rVoid_Action;

        static Func<int, string> pInt_rString_Func;

        delegate int TestDelegate(int b);

        public static void DelegateTest01()
        {
            //该委托类型必须被注册
            //类似于Action,无返回值
            ILRuntimeTest.TestFramework.DelegateTest.IntDelegateTest += IntTest;

            //类似于Func,有返回值
            ILRuntimeTest.TestFramework.DelegateTest.IntDelegateTest2 += IntTest3;

            //热更代码中自用Action类型
            pInt_rVoid_Action += IntTest;
            pString_rVoid_Action += StringTest;

            //热更代码中自用Func类型
            pInt_rString_Func += StringFunc;

            ILRuntimeTest.TestFramework.DelegateTest.IntDelegateTest += IntTest2;

            DelegateTestCls cls = new DelegateTestCls(1000);
            ILRuntimeTest.TestFramework.DelegateTest.IntDelegateTest += cls.IntTest;
            ILRuntimeTest.TestFramework.DelegateTest.IntDelegateTest += cls.IntTest2;

            ILRuntimeTest.TestFramework.DelegateTest.IntDelegateTest(123);
        }

        class DelegateTestCls : DelegateTestClsBase
        {
            public DelegateTestCls(int b)
            {
                this.b = b;
            }
            public override void IntTest(int a)
            {
                base.IntTest(a);
                Console.WriteLine("dele3 a=" + (a + b));
            }


            public int IntTest3(int a)
            {
                Console.WriteLine("dele5 a=" + a);
                return a + 200;
            }
        }

        class DelegateTestClsBase
        {
            protected int b;
            public virtual void IntTest(int a)
            {
                Console.WriteLine("dele3base a=" + (a + b));
            }
            public virtual void IntTest2(int a)
            {
                Console.WriteLine("dele4 a=" + (a + b));
            }
        }

        private static string StringFunc(int arg)
        {
            throw new NotImplementedException();
        }

        private static void StringTest(string obj)
        {
            throw new NotImplementedException();
        }

        private static void IntTestA(int arg1, string arg2)
        {
            throw new NotImplementedException();
        }

        static void IntTest(int a)
        {
            Console.WriteLine("dele a=" + a);
        }

        static void IntTest2(int a)
        {
            Console.WriteLine("dele2 a=" + a);
        }

        static int IntTest3(int a)
        {
            Console.WriteLine("dele3 a=" + a);
            return a + 100;
        }
    }
}
