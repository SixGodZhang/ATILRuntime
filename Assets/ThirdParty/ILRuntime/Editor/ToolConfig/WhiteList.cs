//-----------------------------------------------------------------------
// <filename>WhiteList</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/20 星期四 11:34:57# </time>
//-----------------------------------------------------------------------

using UnityEditor.IMGUI.Controls;

namespace GameFramework.Taurus
{
    public class WhiteTree
    {
        public static WhiteNode[] GetDefaultAssemblys()
        {
            WhiteNode[] nodes = new[]
            {
                new WhiteNode
                {
                    //id depth nodeName
                    Current = new TreeViewItem(1,1,"mscorlib.dll"),
                    Children = new TreeViewItem[]
                    {
                        new TreeViewItem(2,2,"System.IDisposable"),
                    }
                },
                new WhiteNode
                {
                    Current = new TreeViewItem(3,1,"MyTestLibrary.dll"),
                    Children = new TreeViewItem[]
                    {
                        new TreeViewItem(4,2,"MyTestLibrary.ITest"),
                        new TreeViewItem(5,2,"MyTestLibrary.TestBase"),
                    }
                }
            };

            return nodes;
        }
    }

    public class WhiteNode
    {
        public TreeViewItem Current;
        public TreeViewItem[] Children;
    }

    
}
