using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace SQLReg
{
    public class RegKey
    {
        public int Index { get; set; }
        public String Path { get; set; }
        public string leaf;
        public String Comment { get; set; }
        public bool Done;//if getsubkeys has been called, it is true.
        public List<RegProperty> RegProperties;
        public Dictionary<string, string[]> Properties = new Dictionary<string, string[]>();
        public HashSet<string> subKeys = new HashSet<string>();
        //whether SQL setup owns this key, can delete it, not share with others, 
        //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\MSSQLSERVER, MSSQLSERVER is the exclusive SQL Key, SQL owns this root exclusivly
        public bool IsSQLOwned=false;
        public bool IsSQLRoot= false;
        
        //to satisfy which node is current selected
        public int nodeIndex = 0;

        private static int _index = 0;
        public RegKey(string key, string cmt, List<RegProperty> lst)
        {
            Index = _index++;
            Path = key;
            AddSubKeys(key);
            leaf = GetLeaf(key);
            Comment = cmt;
            RegProperties = lst;
            Properties = RegHelper.GetValuesAndDataToDict(key);

        }

        public RegKey(string key, string cmt)
        {
            Index = _index++;
            Path = key;
            Comment = cmt;
            AddSubKeys(key);
            leaf = GetLeaf(key);
            RegProperties = RegHelper.GetValuesAndData(key);
            Properties = RegHelper.GetValuesAndDataToDict(key);
        }

        public void AddSubKeys(string path)
        {
            subKeys = RegHelper.GetSubKeys(path);
            foreach (string sub in subKeys)
            {
                subKeys.Add(sub);
            }


        }

        public string GetLeaf(string path)
        {
            int start = path.LastIndexOf("\\");
            string lastNode = path.Substring(start + 1);
            return lastNode;
        }




    }






    public class RegKeyLite
    {
        public int Index { get; set; }
        public String Path { get; set; }
        public string leaf;

        public bool Done;//if getsubkeys has been called, it is true.

        public HashSet<string> subKeys = new HashSet<string>();

        private static int _index = 0;



        public RegKeyLite(string key)
        {
            Index = _index++;
            Path = key;

            AddSubKeys(key);
            leaf = GetLeaf(key);

        }

        public void AddSubKeys(string path)
        {
            subKeys = RegHelper.GetSubKeys(path);
            foreach (string sub in subKeys)
            {
                subKeys.Add(sub);
            }


        }

        public string GetLeaf(string path)
        {
            int start = path.LastIndexOf("\\");
            string lastNode = path.Substring(start + 1);
            return lastNode;
        }
    }
}