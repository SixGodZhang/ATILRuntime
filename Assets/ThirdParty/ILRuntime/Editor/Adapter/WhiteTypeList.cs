//-----------------------------------------------------------------------
// <filename>GenAdapterConfig</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/11 星期二 17:27:06# </time>
//-----------------------------------------------------------------------

using GameFramework.Taurus;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeGenerationTools.Generator
{
    public class WhiteTypeList
    {
        /// <summary>
        /// 系统类型
        /// </summary>
        private static List<Type> systemWhiteTypeList = new List<Type>()
        {
            typeof(System.IDisposable),
        };

        //系统类型程序集
        //public static string mscorlib_path = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\mscorlib.dll";

        private static List<Tuple<string, string>> GetAssemblys()
        {
            List<Tuple<string, string>> _assemblyList = new List<Tuple<string, string>>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    Tuple<string, string> tuple = new Tuple<string, string>(assemblies[i].GetName().Name, assemblies[i].Location);
                    //UnityEngine.Debug.Log("!!! " + assemblies[i].GetName().Name);
                    _assemblyList.Add(tuple);
                }
                catch (NotSupportedException ex)
                {
                    //no catch ,because assembly can be loaded from memory and can't found it's location. 
                    //UnityEngine.Debug.LogError("Message: " + ex.Message + "\n Trace: " + ex.StackTrace);
                }
            }

            return _assemblyList;
        }

        /// <summary>
        /// 获取所有白名单
        /// </summary>
        /// <returns></returns>
        public static List<TypeDefinition> GetThirdPartyWhiteList()
        {
            //todo
            List<Tuple<string, List<string>>> exportedList = new List<Tuple<string, List<string>>>();
            WhiteNode[] nodes = WhiteTree.GetDefaultAssemblys();
            List<Tuple<string, string>> allAssemblys = GetAssemblys();
            for (int i = 0; i < nodes.Length; i++)
            {
                foreach (var assembly in allAssemblys)
                {
                    if (assembly.Item1.ToLower().Equals(nodes[i].Current.displayName.Replace(".dll","").ToLower()))
                    {
                        List<string> typeNames = new List<string>();
                        foreach (var type in nodes[i].Children)
                        {
                            typeNames.Add(type.displayName);
                        }
                        exportedList.Add(new Tuple<string, List<string>>(assembly.Item2, typeNames));
                        break;
                    }

                }
            }

            List<TypeDefinition> types = new List<TypeDefinition>();

            foreach (var assemblyunit in exportedList)
            {
                var module = ModuleDefinition.ReadModule(assemblyunit.Item1);
                var all_types = module.GetTypes();


                //find all typedefines in system whiteList
                foreach (var need in assemblyunit.Item2)
                {
                    foreach (var at in all_types)
                    {
                        if (at.FullName.Equals(need))
                            types.Add(at);
                    }
                }

            }
            
            return types;
        }
    }
}
