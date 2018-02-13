using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;

namespace SQLReg
{

    public static class RegHelper
    {

        //
        /*

                Use the following table to determine the ROOT registry value:
                -1
                For Per-User Installs, the registry value is written under HKCU; Per-Machine is HKLM
                0
                HKEY_CLASSES_ROOT
                1
                HKEY_CURRENT_USER
                2
                HKEY_LOCAL_MACHINE
                3
                HKEY_USERS
                */
        
        public static Dictionary<int, string> rootMap = new Dictionary<int, string>()
                        {
                            {0,"HKEY_CLASSES_ROOT" },
                            {1,"HKEY_CURRENT_USER" },
                            {2,"HKEY_LOCAL_MACHINE" },
                            {3,"HKEY_USERS" }

                        };
        public static bool IsKeyExist(RegistryKey root, string key)
        {
            RegistryKey rk = root.OpenSubKey(key);


            if (rk != null) { rk.Close(); return true; }
            else return false;

        }

        public static bool IsKeyExist(string fullPath)
        {

            int idx = fullPath.IndexOf("\\");
            string HK = fullPath.Substring(0, idx);
            string mykey = fullPath.Substring(idx + 1);

            return IsKeyExist(HKMap[HK], mykey);


        }
        public enum MyRegValueKind
        {
            None = -1,
            Unknown = 0,
            REG_SZ = 1,// String = 1,
            REG_EXPAND_SZ = 2,//  ExpandString = 2,
            REG_BINARY = 3,//Binary = 3,
            REG_DWORD = 4,// DWord = 4,
            REG_MULTI_SZ = 7,//MultiString = 7,
            REG_QWORD = 11// QWord = 11
        }
        public static Dictionary<String, RegistryKey> HKMap = new Dictionary<string, RegistryKey>
        {
            {"HKEY_CLASSES_ROOT",Registry.ClassesRoot },
            {"HKEY_CURRENT_USER",Registry.CurrentUser },
            {"HKEY_LOCAL_MACHINE",Registry.LocalMachine },
            {"HKEY_USERS",Registry.Users },
            {"HKEY_CURRENT_CONFIG",Registry.CurrentConfig },

        };

        public static HashSet<string> GetSubKeys(string fullPath)
        {
            HashSet<string> hs = new HashSet<string>();
            int idx = fullPath.IndexOf("\\");
            string HK = fullPath.Substring(0, idx);
            string mykey = fullPath.Substring(idx + 1);

            RegistryKey rk = HKMap[HK].OpenSubKey(mykey);

            if (rk == null) return hs;

            string[] subkeys = rk.GetSubKeyNames();

            foreach (string key in subkeys)
            {
                hs.Add(key);

            }

            return hs;
        }

        public static string GetValueForProperty(string fullPath, string propertyName)
        {

            int idx = fullPath.IndexOf("\\");
            string HK = fullPath.Substring(0, idx);
            string mykey = fullPath.Substring(idx + 1);

            RegistryKey rk = HKMap[HK].OpenSubKey(mykey);

            if (rk == null) return "";

            string result = "";
            string[] names = rk.GetValueNames();
            foreach (string nm in names)
            {
                if (propertyName == nm)
                {
                    object data = rk.GetValue(nm);
                    string rvk = ((MyRegValueKind)rk.GetValueKind(nm)).ToString();
                    string name = nm;
                    if (name == "") name = "(Default)";
                    if (data != null)
                    {
                        string stringData = "";
                        if (rvk == MyRegValueKind.REG_BINARY.ToString())
                        {
                            stringData = BitConverter.ToString((byte[])data);

                        }
                        else if (rvk == MyRegValueKind.REG_MULTI_SZ.ToString())
                        {
                            stringData = String.Join("\n ", ((string[])data));
                        }
                        else
                        {
                            stringData = data.ToString();
                        }


                        result = stringData;
                    }
                    break;
                }
            }
            rk.Close();
            return result;
        }
        public static List<RegProperty> GetValuesAndData(string fullPath)
        {
            int idx = fullPath.IndexOf("\\");
            string HK = fullPath.Substring(0, idx);
            string key = fullPath.Substring(idx + 1);

            return GetValuesAndData(HKMap[HK], key);
        }

        public static Dictionary<string, string[]> GetValuesAndDataToDict(string fullPath)
        {
            int idx = fullPath.IndexOf("\\");
            string HK = fullPath.Substring(0, idx);
            string key = fullPath.Substring(idx + 1);

            return GetValuesAndDataToDict(HKMap[HK], key);
        }



        public static List<RegProperty> GetValuesAndData(RegistryKey root, string key)
        {
            List<RegProperty> result = new List<RegProperty>();


            RegistryKey rk = root.OpenSubKey(key);
            if (rk == null) return null;

            string[] names = rk.GetValueNames();
            foreach (string nm in names)
            {
                object data = rk.GetValue(nm);
                string rvk = ((MyRegValueKind)rk.GetValueKind(nm)).ToString();
                string name = nm;
                if (name == "") name = "(Default)";
                if (data != null)
                {
                    string stringData = "";
                    if (rvk == MyRegValueKind.REG_BINARY.ToString())
                    {


                        stringData = BitConverter.ToString((byte[])data);

                    }
                    else if (rvk == MyRegValueKind.REG_MULTI_SZ.ToString())
                    {
                        stringData = String.Join("\n ", ((string[])data));
                    }
                    else
                    {
                        stringData = data.ToString();
                    }


                    result.Add(new RegProperty(name, rvk, stringData));
                }
                else result.Add(new RegProperty(name, rvk, null));
            }
            rk.Close();
            return result;
        }


        public static Dictionary<string, string[]> GetValuesAndDataToDict(RegistryKey root, string key)
        {
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();


            RegistryKey rk = root.OpenSubKey(key);
            if (rk == null) return null;

            string[] names = rk.GetValueNames();
            foreach (string nm in names)
            {
                object data = rk.GetValue(nm);
                string rvk = ((MyRegValueKind)rk.GetValueKind(nm)).ToString();
                string name = nm;
                if (name == "") name = "(Default)";
                if (data != null)
                {
                    string stringData = "";
                    if (rvk == MyRegValueKind.REG_BINARY.ToString())
                    {


                        stringData = BitConverter.ToString((byte[])data);

                    }
                    else if (rvk == MyRegValueKind.REG_MULTI_SZ.ToString())
                    {
                        stringData = String.Join("\n ", ((string[])data));
                    }
                    else
                    {
                        stringData = data.ToString();
                    }


                    result.Add(name, new string[] { rvk, stringData });
                }
                else result.Add(name, new string[] { rvk, "" });
            }
            rk.Close();
            return result;
        }

        /// <summary>
        /// given a registry key fullPath, try to find whether subkey=target exists
        /// if exists,  return it.
        /// level:how deep the check will go
        /// </summary>
        /// <param name="key"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// 
        public class levelkey
        {
            public string fullpath;
            public int level;
            public levelkey(string n,int l) { fullpath = n; level = l; }
        };
        public static string FindSubKey(string fullPath, string target,int level=1)
        {
           
          
            Queue<levelkey> lvlkeys = new Queue<RegHelper.levelkey>();
            

            int idx = fullPath.IndexOf("\\");
            string HK = fullPath.Substring(0, idx);
            string mykey = fullPath.Substring(idx + 1);

            RegistryKey rk = HKMap[HK].OpenSubKey(mykey);

            if (rk == null) return "";

            string[] subkeys = rk.GetSubKeyNames();

            foreach (string key in subkeys)
            {
                lvlkeys.Enqueue(new levelkey(fullPath+"\\"+ key, 1));

            }

            while(lvlkeys.Count>0)
            {
                levelkey key = lvlkeys.Dequeue();
                if (key.level > level) break;

                RegKeyLite subrk = new RegKeyLite(key.fullpath);
                if(subrk.subKeys.Contains(target))
                {
                    //we find it. so return  
                    return key.fullpath + "\\"+target;
                }

                foreach(string s in subrk.subKeys)
                {
                    lvlkeys.Enqueue(new levelkey(key.fullpath + "\\" + s, key.level+1));
                }

            }

            return "";


        }



    }
}
