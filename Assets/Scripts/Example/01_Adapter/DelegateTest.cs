using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateTest01(string arg1, int arg2);

public class DelegateTest
{
    public static DelegateTest01 delegateTest01;
    public static Action<string> actionTest02;
    public static Func<string, string> funcTest03;
}
