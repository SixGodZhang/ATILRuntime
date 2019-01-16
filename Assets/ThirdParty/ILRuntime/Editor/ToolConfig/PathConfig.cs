using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
    public enum ILPath
    {
        HotfixFolder = 0,
        AdapterOutputPath,
        BindingOutputPath,
        MainAssemblyPath,
        ILAssemblyPath,
        TemplatePath,
        HotBugfixNameSpace,
        WhiteListJsonPath
    }

    public class PathConfig
    {
        private static PathConfigAsset asset;
        private static readonly string _assetPath = "Assets/pathconfig.asset";

        static PathConfig()
        {
            InitializeConfigAsset();
        }

        /// <summary>
        /// 初始化资源文件
        /// </summary>
        public static void InitializeConfigAsset()
        {
            if (!File.Exists(_assetPath))
            {
                asset = ScriptableObject.CreateInstance<PathConfigAsset>();
                
                Debug.Log(_assetPath);
                try
                {
                    AssetDatabase.CreateAsset(asset, _assetPath);
                }
                catch (Exception ex)
                {
                    Debug.LogError("error: \n" + ex.Message);
                }
               
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                asset = AssetDatabase.LoadAssetAtPath<PathConfigAsset>(_assetPath);
            }

            //设置一些默认路径
            if (asset != null)
            {
                SetPath(ILPath.WhiteListJsonPath, Application.dataPath + "/ThirdParty/ILRuntime/Editor/ConfigFiles/white_list.json");
            }
        }

        /// <summary>
        /// 获取路径
        /// </summary>
        public static string GetPath(ILPath pathType)
        {
            if (asset == null)
                InitializeConfigAsset();

            string path = string.Empty;
            switch (pathType)
            {
                case ILPath.HotfixFolder:
                    path = asset.hotfixFolder;
                    break;
                case ILPath.AdapterOutputPath:
                    path = asset.adapter_output_path;
                    break;
                case ILPath.BindingOutputPath:
                    path = asset.binding_output_path;
                    break;
                case ILPath.MainAssemblyPath:
                    path = asset.main_assembly_path;
                    break;
                case ILPath.ILAssemblyPath:
                    path = asset.il_assembly_path;
                    break;
                case ILPath.TemplatePath:
                    path = asset.template_path;
                    break;
                case ILPath.HotBugfixNameSpace:
                    path = asset.template_path;
                    break;
                case ILPath.WhiteListJsonPath:
                    path = asset.white_list_json_path;
                    break;
            }

            return path;
        }

        /// <summary>
        /// 设置路径
        /// </summary>
        public static void SetPath(ILPath pathType, string newPath)
        {
            if (asset == null)
                InitializeConfigAsset();

            switch (pathType)
            {
                case ILPath.HotfixFolder:
                    asset.hotfixFolder = newPath;
                    break;
                case ILPath.AdapterOutputPath:
                    asset.adapter_output_path = newPath;
                    break;
                case ILPath.BindingOutputPath:
                    asset.binding_output_path = newPath;
                    break;
                case ILPath.MainAssemblyPath:
                    asset.main_assembly_path = newPath;
                    break;
                case ILPath.ILAssemblyPath:
                    asset.il_assembly_path = newPath;
                    break;
                case ILPath.TemplatePath:
                    asset.template_path = newPath;
                    break;
                case ILPath.HotBugfixNameSpace:
                    asset.template_path = newPath;
                    break;
                case ILPath.WhiteListJsonPath:
                    asset.white_list_json_path = newPath;
                    break;
            }

            EditorUtility.SetDirty(asset);
        }
    }
}
