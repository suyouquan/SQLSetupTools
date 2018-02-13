using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;

namespace SQLReg
{
    public class FindResult
    {
        public TreeNode node;
        public bool isNodeText;
       
        public int hitRow;
        public int hitCol;
        
        
    }
   public static class RegNode
    {

        static int Index = 0;

        //to stastify Find function
        public static List<TreeNode> nodesList = new List<TreeNode>();
        public static int  nodeIndex=0;
        private static int total=0;
        public static string lastFindText = "";
        public static TreeNode treeViewSelectedNode = null;
        public static int gridSelectedRowIdx, gridSelectedColumnIdx;
        public static FindResult findResult =new FindResult();
        public static void InitNodesList(TreeNode node)
        {

            if (node.Tag != null)
            {
                nodesList.Add(node);
                RegKey rk = (RegKey)node.Tag;
                rk.nodeIndex = nodeIndex;
                nodeIndex++;
            }
            else { }
            foreach (TreeNode nd in node.Nodes)
            {
                InitNodesList(nd);
            }
            

        }
        /// <summary>
        /// Check whether this node has been expanded (all subkeys has been enumurated)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// 
        private static bool isDone(TreeNode node)
        {
            if (node.Tag == null) return false;
            RegKey rk = (RegKey)node.Tag;
            if (rk.Done == true) return true;
            return false;
        }

        public static void SetSelectedInfo(int row,int col)
        {
            gridSelectedRowIdx = row;
            gridSelectedColumnIdx = col;
        }
        public static void GetAllSubKeys_Recursive(TreeNode node)
        {

            if (isDone(node)) return;

            string fullPath = node.Name;

            int idx = fullPath.IndexOf("\\");
            string HK = fullPath.Substring(0, idx);
            // string nodeKey = fullPath.Substring(idx + 1);


            Queue<TreeNode> queue = new Queue<TreeNode>();

            queue.Enqueue(node);
         
            while (queue.Count > 0)
            {
                
                TreeNode currentNode = queue.Dequeue();

                if (Controller.shouldAbort) return;

                if (Index % 500 == 0)
                {
                    Controller.UpdateProgress("Adding Node (" + Index + ") " + currentNode.Name, false);
                    //If closing
                

                }
                Index++;


                string myKey = currentNode.Name.Substring(idx + 1);
                RegistryKey rk = RegHelper.HKMap[HK].OpenSubKey(myKey);

                if (rk == null) return;

                string[] subkeys = rk.GetSubKeyNames().OrderBy(p=>p).ToArray();
                string nodeName = "";
                foreach (string key in subkeys)
                {
                    nodeName = currentNode.Name + "\\" + key;

                    //Check to see if middle nodes exists or not
                    if (!currentNode.Nodes.ContainsKey(nodeName))
                    {
                        TreeNode nd = new TreeNode(key);
                        nd.Name = nodeName;
                        //Need to get its  values
                        RegKey regK = new RegKey(nodeName, "", RegHelper.GetValuesAndData(nodeName));
                        nd.Tag = regK;
                        
                        if (nd != node) nd.ForeColor = node.ForeColor;

                        currentNode.Nodes.Add(nd);
                    }
                    if (!isDone(currentNode.Nodes[nodeName]))
                    {
                        queue.Enqueue(currentNode.Nodes[nodeName]);
                    }

                }
                //mark this node done already
                ((RegKey)currentNode.Tag).Done = true;

            } //while queue.count>0

        }

        public static TreeNode AddKey(TreeNode rootNode,string fullPath)
        {
            int idx = fullPath.IndexOf("\\");
            string HK = fullPath.Substring(0, idx);
            string mykey = fullPath.Substring(idx + 1);

            RegistryKey HKXX = RegHelper.HKMap[HK];
            

           return  AddKey(rootNode, HKXX, mykey);

        }


        public static TreeNode  AddKey(TreeNode rootNode, RegistryKey HKXX, string key)
        {
            //Logger.LogMsg("AddKey:" + key);

            RegistryKey rk = HKXX.OpenSubKey(key);
            // RegistryKey rk = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine,
            // RegistryView.Registry64).OpenSubKey(key);


            if (rk == null)
            {
                Logger.LogMsg(HKXX.Name + "\\" + key + " doesn't exist.");
         
                return null;

            }
            rk.Close();

            //Check to see if HKXX exists, if not create it
            TreeNode HKNode = null;
            if (!rootNode.Nodes.ContainsKey(HKXX.Name))
            {
                TreeNode hk = new TreeNode(HKXX.Name);
                hk.Name = HKXX.Name;

                //hk.Expand(); //HKCR has many nodes, so don't expand
                rootNode.Nodes.Add(hk);
                HKNode = hk;
            }
            else
            {
                HKNode = rootNode.Nodes[HKXX.Name];
            }

            //Now add other nodes in the middle of the path
            string[] middleNames = key.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            string nodeName = "";
            TreeNode parent = HKNode;
            TreeNode current = null;
            for (int i = 0; i < middleNames.Length; i++)
            {

                nodeName = parent.Name + "\\" + middleNames[i];
                //Check to see if middle nodes exists or not
                if (!parent.Nodes.ContainsKey(nodeName))
                {
                    TreeNode nd = new TreeNode(middleNames[i]);
                    nd.Name = nodeName;

                    //Need to get its  values
                    RegKey regK = new RegKey(nodeName, "", RegHelper.GetValuesAndData(HKXX, nodeName.Replace(HKNode.Name + "\\", "")));
                    nd.Tag = regK;


                    parent.Nodes.Add(nd);
                }

                parent = parent.Nodes[nodeName];
                current = parent;

            }

            //now current is the last key, we need to get its subkeys recurivsly
            if (Controller.sqlRegKeys.ContainsKey(current.Name))
            {
                Reason r = Controller.sqlRegKeys[current.Name];
                if (r.cleanable == true)
                {
                    ((RegKey)current.Tag).IsSQLOwned = true;
                    ((RegKey)current.Tag).IsSQLRoot = true;
                    current.ForeColor = Color.DarkBlue;
                }
                else
                {
                    ((RegKey)current.Tag).IsSQLOwned = false;
                    ((RegKey)current.Tag).IsSQLRoot = true;
                    current.ForeColor = Color.DarkBlue;//same color anyway

                }
            }
            else
            {
                /*
                2018 - 02 - 06 21:36:48[ERROR][RegNode.cs:231:AddKey]sqlRegKeys doesn't have this node name! HKEY_CLASSES_ROOT\TypeLib\{3F98D457-551B-48C5-BDE8-7FDECCD5AFA5} vs key:TYPELIB\{3F98D457-551B-48C5-BDE8-7FDECCD5AFA5}
                2018 - 02 - 06 21:36:49[ERROR][RegNode.cs:231:AddKey]sqlRegKeys doesn't have this node name! HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MMC\Snapins\{d52e5f54-75d9-4a93-91b7-2215ea5cbed2} vs key:Software\Microsoft\MMC\Snapins\{d52e5f54-75d9-4a93-91b7-2215ea5cbed2}
                2018 - 02 - 06 21:36:49[ERROR][RegNode.cs:231:AddKey]sqlRegKeys doesn't have this node name! HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MMC\Snapins\{f66ae3a2-97c7-4e45-9c70-4ecea8b3bfa0} vs key:Software\Microsoft\MMC\Snapins\{f66ae3a2-97c7-4e45-9c70-4ecea8b3bfa0}
                
                I got above error in some cases, but it doesn't matther, because GetAllSubKeys_Recursive() will still be called on it.
                just IsSQLRoot is not able to set.
                 */

                Logger.LogError("sqlRegKeys doesn't have this node name! " + current.Name + " vs key:" + key);
            }
            
            GetAllSubKeys_Recursive(current);

            return current;
          //  Logger.LogMsg("AddKey " + key);
        }
        //Check whether a node is SQL ownered key, or its parent owns this key.
        //This check is for export function. we cannot cleanup/delete those keys that is not SQL owned exclusivley
        public static bool IsSQLExclusivelyOwned(TreeNode node)
        {
            TreeNode current=node;
            while(current != null)
            {
                  
                if (current.Tag != null)
                {
                    RegKey rk = (RegKey)current.Tag;
                    if (rk.IsSQLOwned) return true;
                }

                current = current.Parent;

            }
            return false;
        }
        public static bool IsMeOrParentSQLRoot(TreeNode node)
        {
            TreeNode current = node;
            while (current != null)
            {

                if (current.Tag != null)
                {
                    RegKey rk = (RegKey)current.Tag;
                    if (rk.IsSQLRoot) return true;
                }

                current = current.Parent;

            }
            return false;

        }

       public static void UpdateNodeTextWithNodeCount(TreeNode root)
        {
            int cnt = root.GetNodeCount(true);
            int sub = root.Nodes.Count;
            if(cnt>0)root.Text = root.Text + "  ("+sub+"-"+ cnt + ")";
           
            foreach(TreeNode n in root.Nodes)
            {
                UpdateNodeTextWithNodeCount(n);
            }


        }
        //Find node and its kids for specified name
        public static TreeNode FindFromTree(TreeNode node, string name)
        {

            if (node.Name == name) return node;

            //not match
            foreach (TreeNode nd in node.Nodes)
            {
                TreeNode result = FindFromTree(nd, name);
                if (result != null) { return result; }
            }
            return null;
        }

        //Given a node, find the lowest exists child from another tree
        public static TreeNode FindLowestParent(TreeNode root, TreeNode node)
        {
            if (node == null) return null;

            TreeNode result = FindFromTree(root, node.Name);
            if (result != null) return result;

            
            return FindLowestParent(root, node.Parent);
            //doesn't match, need to check node's parent

 

        }
        public static void UpdateParentNodeText(TreeNode parent)
        {
            if (parent == null) return;
            int cnt = parent.GetNodeCount(true);
            int sub = parent.Nodes.Count;
            int idx= parent.Name.LastIndexOf("\\");
            string txt = parent.Name;
            if (idx > 0)
            {
               txt = parent.Name.Substring(idx+1);

            }
            if (cnt > 0) parent.Text = txt + "  (" + sub + "-" + cnt + ")";
            
            UpdateParentNodeText(parent.Parent);
 
        }

        public static bool IsNodeMatch(TreeNode node,string what)
        {
            if (node.Text.ToLower().Contains(what))
            {
                findResult.node = node;
                findResult.isNodeText = true;
         
                findResult.hitRow = findResult.hitCol = -1;
                return true;
            }

            if (node.Tag == null) return false;
            RegKey rk = (RegKey)node.Tag;
            int row = 0;
            foreach(RegProperty rp in rk.RegProperties)
            {
                
                if (rp.Name.ToLower().Contains(what))
                {
                    findResult.node = node;
                    findResult.isNodeText = false;
                 
                    findResult.hitRow = row;
                    findResult.hitCol = 0;
                    return true;
                }
                if (rp.Data.ToLower().Contains(what))
                {
                    findResult.node = node;
                    findResult.isNodeText = false;
                   
                    findResult.hitRow = row;
                    findResult.hitCol = 2;

                    return true;
                }

                row++;
            }
            return false;
        }

        public static bool Find( string what)
        {
            if (Controller.rootNode.Nodes.Count <= 0) return false;
            if (nodesList.Count == 0) { InitNodesList(Controller.rootNode); total = nodeIndex; }
            TreeNode current=null;
            
            if (treeViewSelectedNode != null && treeViewSelectedNode.Tag!=null)
            {
                
                RegKey rk=(RegKey)treeViewSelectedNode.Tag;
                TreeNode node = treeViewSelectedNode;
                nodeIndex = rk.nodeIndex;

                //do something specialfor current selected node
                //Same as regedit.exe, for current select node
                /*
                if (if Grid doesn't have selected cells, search from first row)       
                if (if Grid is selected cells, move to next row to match) 
                won't check match from node text (same behavior as regedit.exe)
             */
                //Now find out latest row and col index in reg property
                //the grid max col number is 2 (total 3 columns)
                int nextRow = 0, nextCol = 0;
                if (gridSelectedRowIdx >= 0) nextRow = gridSelectedRowIdx;
                if (gridSelectedColumnIdx == -1) nextRow = 0;//if no selection
                else if (gridSelectedColumnIdx <= 1) nextCol = 2; //color 0,1
                else  if (gridSelectedColumnIdx > 1) // 2
                {
                    nextRow++;
                    nextCol = 0;
                }
                

                if (nextRow < rk.RegProperties.Count)
                {
                    for(int k=nextRow;k<rk.RegProperties.Count;k++)
                    {
                        RegProperty rp = rk.RegProperties[k];
                        if (nextCol == 0)
                        {
                            if (rp.Name.ToLower().Contains(what.ToLower()))
                            {
                                findResult.node = node;
                                findResult.isNodeText = false;
                    
                                findResult.hitRow = k;
                                findResult.hitCol = 0;
                                return true;
                            }
                        }
                     
                      
                            if (rp.Data.ToLower().Contains(what.ToLower()))
                            {
                                findResult.node = node;
                                findResult.isNodeText = false;
                              
                                findResult.hitRow = k;
                                findResult.hitCol = 2;

                                return true;
                            }
                      

                        nextCol = 0;
                    }//for



                }//if

                //the selected node doesn't match
                //Go to next node
               
                    nodeIndex++;
                    if (nodeIndex == total)
                    {
                        nodeIndex = 0;
                    }


            }
            else nodeIndex = 0;

        

            int cnt = 0;
            bool isFound = false;
            int saveIdx = nodeIndex;

            while (cnt<total)
            { 
                current = nodesList[nodeIndex];
               
                if (IsNodeMatch(current,what.ToLower()))
                {

                    //need to advance to next
                    isFound = true;
                    SetSelectedInfo(-1, -1);
                    nodeIndex++;
                    if (nodeIndex == total)
                    {
                        nodeIndex = 0;
                    }

                    break;
                }
 

                nodeIndex++;
                if (nodeIndex == total)
                {
                    nodeIndex = 0;
                }

           
                cnt++;
             

            }//while
            
            return isFound;

        }


        public static void ExpandParent(TreeNode node)
        {
            while(node.Parent!=null)
            {
                node.Parent.Expand();
                node = node.Parent;
            }
        }


    }
}
