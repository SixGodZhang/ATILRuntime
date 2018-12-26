using System;

namespace Hotfix
{
    public class HotFixMode : SubMonoBehavior
    {
        public override void OnApplicationQuit()
        {
            UnityEngine.Debug.LogError("-----------OnApplicationQuit---------");
        }

        public override void OnDestroy()
        {
            
            UnityEngine.Debug.LogError("-----------OnDestroy---------");
        }

        public override void OnFixedUpdate()
        {
            UnityEngine.Debug.LogError("-----------OnFixedUpdate---------");
        }

        public override void Start()
        {
            UnityEngine.Debug.LogError("-----------Start---------");
            //测试委托注册
            DelegateTest.delegateTest01 += DelegateTestMethod01;
            DelegateTest.actionTest02 += ActionTest02;
            DelegateTest.funcTest03 += FuncTest03;
        }

        private string FuncTest03(string arg)
        {
            throw new NotImplementedException();
        }

        private void DelegateTestMethod01(string arg1, int arg2)
        {
            throw new NotImplementedException();
        }

        private void ActionTest02(string obj)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            UnityEngine.Debug.LogError("-----------Update---------");
        }
    }
}
