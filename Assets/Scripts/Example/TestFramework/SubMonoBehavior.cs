//-----------------------------------------------------------------------
// <filename>SubMonoBehavior</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #热更类使用的周期函数# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/6 星期四 14:57:57# </time>
//-----------------------------------------------------------------------

using ILRuntime.Other;

[NeedAdaptor]
public abstract class SubMonoBehavior
{
    /// <summary>
    /// 开始函数
    /// </summary>
    public abstract void Start();
    /// <summary>
    /// 渲染帧函数
    /// </summary>
    public abstract void Update();
    /// <summary>
    /// 固定帧函数
    /// </summary>
    public abstract void OnFixedUpdate();
    /// <summary>
    /// 销毁函数
    /// </summary>
    public abstract void OnDestroy();
    /// <summary>
    /// 退出函数
    /// </summary>
    public abstract void OnApplicationQuit();
}
