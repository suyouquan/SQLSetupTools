using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.IO;

namespace SQLReg.Analysis
{
    //add keys fro MSI files->Registry keys table
    public class FromMSI
    {
        public static void Add()
        {
            var sumSQLProduct = Controller.sumSQLProduct;

            int cnt = 0;

            foreach (KeyValuePair<string, string> kv in sumSQLProduct.MsiRegistries)
            {
                //the value could be *, or could be not *(not ownerd by SQL)
                if (!RegHelper.IsKeyExist(kv.Key)) continue;

               if (!Controller.sqlRegKeys.ContainsKey(kv.Key))
                {

                    if (kv.Value == "*")
                    {
                        //remove kids
                        List<string> kids = new List<string>();
                        /*
                        foreach (string s in Controller.sqlRegKeys_Shared)
                        {
                            if (s == kv.Key || s.StartsWith(kv.Key)) kids.Add(s);
                        }
                        foreach (string s in kids) Controller.sqlRegKeys_Shared.Remove(s);
                        */

                        kids.Clear();
                        foreach (string s in Controller.sqlRegKeys.Keys)
                        {
                            if (s.StartsWith(kv.Key)) kids.Add(s);
                        }
                        foreach (string s in kids) Controller.sqlRegKeys.Remove(s);

                        //Now add it in.default cleanable=false.I don't want to remove it.
                          Controller.sqlRegKeys.Add(kv.Key,new Reason(SourceType.FromMSI,"", "Registry in MSI packages"));
                        cnt++;
                    }

                    else
                    {
                        //Don't add it, it could list HKLM\SOFTWARE as share key, too many nodes then.
                        /*
                        List<string> kids = new List<string>();

                        //check whether any start parent exists
                        bool hasStartParent = false;
                        foreach(string s in Controller.sqlRegKeys)
                        {
                            if(kv.Key.StartsWith(s))
                            {
                                if(!Controller.sqlRegKeys_Shared.Contains(s)) { hasStartParent = true; break; }
                            }
                        }

                        //only add it when there is no start parent
                        if (!hasStartParent)
                        {
                            //Now add it in
                            if (Controller.sqlRegKeys_Shared.Contains(kv.Key)) Controller.sqlRegKeys_Shared.Add(kv.Key);
                            Controller.sqlRegKeys.Add(kv.Key);
                            if(!Controller.keyReason.ContainsKey(kv.Key))Controller.keyReason.Add(kv.Key, "Registry in MSI packages.");
                            cnt++;
                        }
                        */

                    }




                }




            } //foreach



            Controller.UpdateProgress("FromMSI Add keys:" + cnt, true);

        }

         


    }
}
