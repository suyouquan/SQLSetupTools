using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReg
{
    public class subKey
    {
        public string subPath; // like SourceList\xxx
        public string property;
        public subKey(string sub, string prop)
        {
            subPath = sub;
            property = prop;
        }
    }


    public class LoadRegHive
    {
        public string key;
        public subKey sk;
        static readonly object _object = new object();
        public LoadRegHive(string s, subKey subk)
        {
            key = s;
            sk = subk;
        }
        public void Load()
        {


            Controller.UpdateProgress("Loading:"+key, true);
            RegHive reghive = new RegHive(key, sk);
            AddHives(key, reghive);

            Controller.UpdateProgress("Completed:" + key+" Total "+reghive.regKeys.Count+" subkeys", true);

        }

        public static void AddHives(string key, RegHive rh)
        {
            lock (_object)
            {
                Controller.regHives.Add(key, rh);
            }

        }
    }
    public class RegHive
    {
        public string hivePath;//Registry key path, like HKEY_CLASSES_ROOT\Installer\Dependencies

        //propertyMap is to save the packagename for installer related registry keys.
        //they have  file name like xxx.msi or xxx.msp etc. 
        //this is to help filter rows
        public Dictionary<string, List<string>> propertyMap = new Dictionary<string, List<string>>();//
        public HashSet<string> subKeys = new HashSet<string>();
        public subKey sbkey = null;

        public List<RegKey> regKeys = new List<RegKey>();

        public RegHive(string regPath, subKey sk = null)
        {
            hivePath = regPath;
            if (null != sk) sbkey = sk;
            subKeys = RegHelper.GetSubKeys(regPath);
            int cnt = 0;
            foreach (string sub in subKeys)
            {

                string fullpath = hivePath + "\\" + sub;

                if (cnt % 500 == 0)
                {
                    Controller.UpdateProgress("Reading (" + cnt + ") " + fullpath, false);
                    //If closing
                    if (Controller.shouldAbort) return;
                }
                cnt++;

                RegKey rk = new RegKey(fullpath, "");
                regKeys.Add(rk);

                if (sk != null)
                {
                    string fullpath2 = fullpath + "\\" + sk.subPath;
                    string result = RegHelper.GetValueForProperty(fullpath2, sk.property);
                    if (!string.IsNullOrEmpty(result))
                    {
                        if (propertyMap.ContainsKey(result))
                        {
                            propertyMap[result].Add(fullpath);
                        }
                        else propertyMap.Add(result, new List<string> { fullpath });
                    }

                } //if sk not null

            }
        } //ctor








    }
}
