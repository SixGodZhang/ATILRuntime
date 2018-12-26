using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.IMGUI.Controls;

namespace GameFramework.Taurus
{
    class WhiteTreeView : TreeView
    {
        public WhiteTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            WhiteNode[] nodes = WhiteTree.GetDefaultAssemblys();
            var allItems = new List<TreeViewItem>();
            for (int i = 0; i < nodes.Length; i++)
            {
                allItems.Add(nodes[i].Current);
                allItems.AddRange(nodes[i].Children);
            }

            // Utility method that initializes the TreeViewItem.children and -parent for all items.
            SetupParentsAndChildrenFromDepths(root, allItems);

            // Return root of the tree
            return root;
        }
    }
}
