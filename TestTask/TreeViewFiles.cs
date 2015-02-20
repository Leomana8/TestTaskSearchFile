using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestTask
{
    static class TreeViewFiles
    {
        static private int _lengthRootName = 0;

        public static void NewTree(this TreeView tree, DirectoryInfo root)
        {
            tree.Nodes.Clear();
            var rootNode = new TreeNode(root.Name);
            tree.Nodes.Add(rootNode);
            _lengthRootName = root.FullName.Length;
        }

        public static void AddFile(this TreeView tree, FileInfo file)
        {
            TreeNode directoryNode = tree.AddDirectory(file.Directory);
            directoryNode.Nodes.Add(file.Name);
        }

        public static TreeNode AddDirectory(this TreeView tree, DirectoryInfo directory)
        {
            TreeNode lastNode = null;
            string newPath = directory.FullName;
            newPath = newPath.Substring(_lengthRootName);      
            string subPathAgg = string.Empty;
            foreach (string subPath in newPath.Split('\\'))
            {
                subPathAgg += subPath;
                TreeNode[] nodes = tree.Nodes.Find(subPathAgg, true);

                if (nodes.Length == 0)
                    if (lastNode == null)
                        lastNode = tree.Nodes.Add(subPathAgg, subPath, 0);

                    else
                        lastNode = lastNode.Nodes.Add(subPathAgg, subPath, 0);
                else
                    lastNode = nodes[0];
            }
            return lastNode;
        }

    }
}
