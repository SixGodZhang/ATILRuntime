//-----------------------------------------------------------------------
// <filename>PathConfigAsset</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/29 星期六 10:31:04# </time>
//-----------------------------------------------------------------------

using UnityEngine;

namespace GameFramework.Taurus
{
    [CreateAssetMenu(fileName = "PathConfigAsset", menuName = "Create PathConfig")]
    public class PathConfigAsset : ScriptableObject
    {
        /// <summary>
        /// 热更DLL文件夹
        /// </summary>
        [Header("热更DLL文件夹" )]
        public string hotfixFolder = "";

        /// <summary>
        /// 适配器输出目录
        /// </summary>
        [Header("适配器代码输出目录")]
        public string adapter_output_path = "";

        /// <summary>
        /// 绑定输出目录
        /// </summary>
        [Header("绑定代码输出目录")]
        public string binding_output_path = "";

        /// <summary>
        /// Assembly-CSharp.dll path
        /// </summary>
        [Header("Assembly-CSharp.dll 路径")]
        public string main_assembly_path = "";

        /// <summary>
        /// hotfix dll path
        /// </summary>
        [Header("热更Dll 路径")]
        public string il_assembly_path = "";

        [Header("模板文件 路径")]
        public string template_path = "";

        [Header("热更修复的命名空间 路径")]
        public string hot_bugfix_namespace = "";

        [Header("存储白名单的Json文件")]
        public string white_list_json_path = "";

    }
}
