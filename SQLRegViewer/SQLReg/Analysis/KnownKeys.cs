using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace SQLReg.Analysis
{
    public class KnownKeys
    {
        public static void Add()
        {

            int cnt = 0;
            //add known SQL keys
            //key, can cleanable.
            Dictionary<string,bool> allKnownKeys = new Dictionary<string, bool>()
            {
                {@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server",true },
                {@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server",true },
                {@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSSQLServer",false },
                {@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\MSSQLServer",false }

            };

            
            foreach (KeyValuePair<string,bool> kv in allKnownKeys)
            {

                if (RegHelper.IsKeyExist(kv.Key))
                {
                    if (!Controller.sqlRegKeys.ContainsKey(kv.Key))
                    {
                        cnt++;
                        Controller.sqlRegKeys.Add(kv.Key,
                        new Reason(SourceType.KnownKey, kv.Key, "Known SQL Key",kv.Value));
                    }
                }

            }

             //some other app may need this key, so cannot delete it
          //  Controller.sqlRegKeys_Shared.Add(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSSQLServer");

            //got those SQL Server related distribute keys
            RegKeyLite rkl = new RegKeyLite(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft");
            foreach(string sub in rkl.subKeys)
            {
                string s = rkl.Path + "\\" + sub;
                if(sub.StartsWith("Microsoft SQL Server"))
                {
                    if (!Controller.sqlRegKeys.ContainsKey(s))
                    {
                        cnt++;
                        Controller.sqlRegKeys.Add(s,
                        new Reason(SourceType.KnownKey, s, "Known SQL Key",true));// 
                    }


                  //  if (!Controller.sqlRegKeys_Shared.Contains(s)) Controller.sqlRegKeys_Shared.Add(s);
                }

            }

            RegKeyLite rkl2 = new RegKeyLite(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft");
            foreach (string sub in rkl2.subKeys)
            {
                string s = rkl2.Path + "\\" + sub;
                if (sub.StartsWith("Microsoft SQL Server"))
                {
                      // if (Controller.sqlRegKeys_Shared.Contains(s)) Controller.sqlRegKeys_Shared.Add(s);
                    if (!Controller.sqlRegKeys.ContainsKey(s))
                    {
                        cnt++;
                        Controller.sqlRegKeys.Add(s,
                        new Reason(SourceType.KnownKey, s, "Known SQL Key",true));
                    }


                }

            }

            Controller.UpdateProgress("Known Keys:Add key:" + cnt);

        }

    }
}
