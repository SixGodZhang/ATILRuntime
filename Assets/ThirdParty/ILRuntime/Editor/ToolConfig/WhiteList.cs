//-----------------------------------------------------------------------
// <filename>WhiteList</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/20 星期四 11:34:57# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace GameFramework.Taurus
{
    public class UnityEditorTool : Editor
    {
        /// <summary>
        /// Test
        /// </summary>
        [MenuItem("ILRuntime/WriteWhiteListToJson")]
        public static void WriteWhiteListToJson()
        {
            List<AdapterTreeNode> nodesList = new List<AdapterTreeNode>();

            nodesList.Add(
                    new AdapterTreeNode
                    {
                        id = 1,
                        depth = 1,
                        displayName = "mscorlib.dll",
                        children = new AdapterTreeNode[]
                        {
                            new AdapterTreeNode
                            {
                                id = 2,
                                depth = 2,
                                displayName = "System.IDisposable"
                            }
                        }

                    }
                );
            nodesList.Add(
                    new AdapterTreeNode
                    {
                        id = 3,
                        depth = 1,
                        displayName = "MyTestLibrary.dll",
                        children = new AdapterTreeNode[]
                        {
                            new AdapterTreeNode
                            {
                                id = 4,
                                depth = 2,
                                displayName = "MyTestLibrary.ITest",
                            },
                            new AdapterTreeNode
                            {
                                id = 5,
                                depth = 2,
                                displayName = "MyTestLibrary.TestBaset",
                            },
                        }
                    }
                );
            ReadJsonTreeNode.Instance.ConvertTreeNodesToPersistantData(nodesList);

            AssetDatabase.Refresh();
        }
    }


    public class WhiteTree
    {
        public static WhiteNode[] GetDefaultAssemblys()
        {
            string json = File.ReadAllText(PathConfig.GetPath(ILPath.WhiteListJsonPath));
            var nodes = ReadJsonTreeNode.Instance.ConvertPersistantDataToWhiteNode<AdapterTreeNode[]>(json);

            List<WhiteNode> returnNodes = ConvertAdapterTreeNodeToWhiteNode(nodes);

            return returnNodes.ToArray();
        }

        /// <summary>
        /// 从持久化数据中获取白名单节点信息
        /// </summary>
        public static List<WhiteNode> ConvertAdapterTreeNodeToWhiteNode<T>(T nodes)
        {
            if (typeof(T).FullName != "GameFramework.Taurus.AdapterTreeNode[]")
            {
#if UNITY_EDITOR
                Debug.Log("!!! type error.");
#endif
                return null;
            }

            var treeNodes = nodes as GameFramework.Taurus.AdapterTreeNode[];

            List<WhiteNode> whiteNodes = new List<WhiteNode>();
            foreach (var item in treeNodes)
            {
                whiteNodes.Add(ConvertNode(item));
            }

            return whiteNodes;
        }

        /// <summary>
        /// 转换节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static WhiteNode ConvertNode(AdapterTreeNode node)
        {
            if (node.children.Length <= 0)
                return null;

            WhiteNode whiteNode = new WhiteNode();
            whiteNode.Current = new TreeViewItem(node.id, node.depth, node.displayName);
            whiteNode.Children = GetTreeViewItemsByRecursive(node).ToArray();

            return whiteNode;
        }

        /// <summary>
        /// 通过递归获取子节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static List<TreeViewItem> GetTreeViewItemsByRecursive(AdapterTreeNode node)
        {
            if (node.children == null)
                return null;

            List<TreeViewItem> items = new List<TreeViewItem>();
            for (int i = 0; i < node.children.Length; i++)
            {
                TreeViewItem item = new TreeViewItem(node.children[i].id, node.children[i].depth, node.children[i].displayName);

                if (node.children!=null)
                {
                    item.children = GetTreeViewItemsByRecursive(node.children[i]);
                }

                items.Add(item);
            }

            return items;
        }
    }

    /// <summary>
    /// 白名单节点
    /// </summary>
    public class WhiteNode
    {
        public TreeViewItem Current;
        public TreeViewItem[] Children;
    }

    /// <summary>
    /// 适配器树节点
    /// </summary>
    [DataContract]
    public class AdapterTreeNode
    {
        [DataMember]
        public int id;
        [DataMember]
        public int depth;
        [DataMember]
        public string displayName;
        [DataMember]
        public AdapterTreeNode[] children;
    }

    public interface IReadOrWriteTreeNode
    {
        /// <summary>
        /// 将AdapterTreeNode持久化
        /// </summary>
        void ConvertTreeNodesToPersistantData<T>(T nodes) where T : class;
        /// <summary>
        /// 转化持久化数据到WhiteNode
        /// </summary>
        T ConvertPersistantDataToWhiteNode<T>(string json) where T : class;
    }

    public class ReadJsonTreeNode : IReadOrWriteTreeNode
    {
        #region 单例
        public static ReadJsonTreeNode _instance;
        public static ReadJsonTreeNode Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ReadJsonTreeNode();
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Deserialize a JSON stream to a object.  
        /// </summary>
        /// <returns></returns>
        public static T ReadToObject<T>(string json) where T:class
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            T nodes = ser.ReadObject(ms) as T;
            ms.Close();
            return nodes;
        }

        /// <summary>
        /// Serialize a object to json
        /// </summary>
        /// <returns></returns>
        public static string WriteFromObject<T>(T data) where T:class
        {
            MemoryStream ms = null;
            byte[] json = null;
            try
            {
                //Create a stream to serialize the object to.  
                ms = new MemoryStream();

                // Serializer the object to the stream.  
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(ms, data);
                json = ms.ToArray();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ms.Close();
            }

            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        /// <summary>
        /// 把持久化数据转化为WhiteNode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T ConvertPersistantDataToWhiteNode<T>(string json) where T:class
        {
            T objsArray = ReadToObject<T>(json);
            return objsArray;
        }

        /// <summary>
        /// 把白名单树节点转为持久化数据
        /// </summary>
        public void ConvertTreeNodesToPersistantData<T>(T nodes) where T : class
        {
            StringBuilder sbNodes = new StringBuilder();

            string temp = WriteFromObject(nodes);

            File.WriteAllText(PathConfig.GetPath(ILPath.WhiteListJsonPath), temp);
        }
    }
}
