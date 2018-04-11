using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.Windows.Forms;
using System.Threading;

using System.Diagnostics;
using System.Reflection;
using System.IO;
using Microsoft.Win32;

namespace ProductBrowser
{
    public class FindResult
    {
        public TreeNode node;
        public bool isNodeText;
 

    }


    public static class myFind
    {

        static int Index = 0;

        //to stastify Find function
        public static List<TreeNode> nodesList = new List<TreeNode>();
        public static int nodeIndex = 0;
        private static int total = 0;
        public static string lastFindText = "";
        public static TreeNode treeViewSelectedNode = null;
        public static int gridSelectedRowIdx, gridSelectedColumnIdx;
        public static FindResult findResult = new FindResult();
        public static void InitNodesList(TreeNode node)
        {
                node.Tag = nodeIndex;
                nodesList.Add(node);
                nodeIndex++;
           
            foreach (TreeNode nd in node.Nodes)
            {
                InitNodesList(nd);
            }


        }


        public static bool Find(string what)
        {
            if (Product.root.Nodes.Count <= 1) return false; //if to avoid the failure becase we start with nodeIndex=1
            
            if (nodesList.Count == 0) { InitNodesList(Product.root); total = nodeIndex; }
            TreeNode current = null;

            if (treeViewSelectedNode != null && treeViewSelectedNode.Tag != null)
            {


                TreeNode node = treeViewSelectedNode;
                nodeIndex = (int)node.Tag;

                //Don't search current node. just advance one
                nodeIndex++;

                if (nodeIndex == total)
                {
                    nodeIndex = 1;
                }

            }
            else nodeIndex = 1;//first node i 0 is the root. dont'  check it so set it to 1
           



            int cnt = 1;
            bool isFound = false;
            int saveIdx = nodeIndex;

            while (cnt < total)
            {
                current = nodesList[nodeIndex];

                if (IsNodeMatch(current, what.ToLower()))
                {

                    //need to advance  for next Find call.
                    nodeIndex++;

                    isFound = true;
                    
                    if (nodeIndex == total)
                    {
                        nodeIndex = 1;
                    }

                    break;
                }


                nodeIndex++;
                if (nodeIndex == total)
                {
                    nodeIndex = 1;
                }


                cnt++;


            }//while

            return isFound;

        }


        public static bool IsNodeMatch(TreeNode node, string what)
        {
            if (node.Text.ToLower().Contains(what))
            {
                findResult.node = node;
                findResult.isNodeText = true;
              
                return true;
            }

           
            return false;
        }

    }
}
