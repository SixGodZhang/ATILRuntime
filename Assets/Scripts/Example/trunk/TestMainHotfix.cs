//-----------------------------------------------------------------------
// <filename>TestMainHotfix</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/20 星期四 20:48:19# </time>
//-----------------------------------------------------------------------

using ILRuntime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
    [AllowHotfix]
    public class TestMainHotfix
    {
        private void DoPrivateAction()
        {
            Debug.Log("private action....");
        }

        public void DoAction()
        {
            Debug.Log("do action in MainProject ......");
        }

        public void DoActionWithParams(string args,int arr)
        {
            Debug.Log("do action in MainProject ......");
        }

        //public static void DoAction(int arg)
        //{
        //    Debug.Log("do action ......");
        //}

        //public static int DoAction(string arg)
        //{
        //    return 0;
        //}

        //public static void DoPlay()
        //{
        //    Debug.Log("do play ....");
        //}

        //public static void DoPlay(ref string arg)
        //{

        //}

        //public static void DoPlay(out int arg)
        //{
        //    arg = 22;
        //}

    }
}
