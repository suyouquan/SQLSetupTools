using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.IO;

namespace SQLReg.Analysis
{
  public static  class Uninstall
    {

        public static void Add()
        {
            var sumSQLProduct = Controller.sumSQLProduct;


            bool found = false;


            /****************************************************************************************************/
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall  
            /****************************************************************************************************/

            string[] hives = new string[] { "", "\\Wow6432Node" };
            Controller.UpdateProgress(@"Analyzing HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",true);
            int cnt = 0;

            foreach (string hiveRoot in hives)
            {
                if (!RegHelper.IsKeyExist(@"HKEY_LOCAL_MACHINE\SOFTWARE" + hiveRoot + @"\Microsoft\Windows\CurrentVersion\Uninstall"))
                    continue;

                RegHive reghive =new RegHive(@"HKEY_LOCAL_MACHINE\SOFTWARE" + hiveRoot + @"\Microsoft\Windows\CurrentVersion\Uninstall");

                //first, check product code
                //using regKeys instead subkeys

                foreach (RegKey rk in reghive.regKeys)
                {
                    found = false;
                    string reg = rk.Path;
                    string leaf = rk.leaf.ToLower();
                    string code = Utility.ConvertGUID2CompressedRegistryStyle(leaf);

                    
                        if (!String.IsNullOrEmpty(code))
                        {

                            if (Installer.foundCompressedProductCodes.Contains(code)
                                || sumSQLProduct.compressedProductCodes.Contains(code)
                                )
                            {
                                if (!Controller.sqlRegKeys.ContainsKey(reg))
                                {
                                  

                                cnt++;
                                Controller.sqlRegKeys.Add(reg,
                                new Reason(SourceType.Uninstall, code, "Uninstall:Product Code:"+code,true));

                            }

                                if (!Installer.foundCompressedProductCodes.Contains(code))
                                    Installer.foundCompressedProductCodes.Add(code);

                                

                            }
                        } //if it is GUID
                        else //If it is KB article?
                        {
                            if (sumSQLProduct.kbArticles.Contains(leaf))
                            {
                            if (!Controller.sqlRegKeys.ContainsKey(reg))
                            {
                                cnt++;
                                Controller.sqlRegKeys.Add(reg,
                                new Reason(SourceType.FromMSI_KBArticle, leaf, "Uninstall:Product Code:" + leaf,true));
                            }
                             
                            }
                            else //sometimes the meta is outdated so check more:

                            {
                                //If the key has SQLProductPatchFamilyCode && ProductId="SQLxxx", or ProductId="SQL2008", it is SQL patch key

                                if (rk.Properties.ContainsKey("SQLProductPatchFamilyCode")
                                    && rk.Properties.ContainsKey("ProductId")
                                    && rk.Properties["ProductId"][1].StartsWith("SQL")
                                    )
                                {
                                    //it is SQL patch key
                                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                                    {
                                    
                                    cnt++;
                                    Controller.sqlRegKeys.Add(reg,
                                    new Reason(SourceType.Uninstall, "SQLProductPatchFamilyCode", "Uninstall:SQLProductPatchFamilyCode",true));

                                }

                            }


                                else if (
                                    rk.Properties.ContainsKey("ProductId")
                                   && rk.Properties["ProductId"][1].StartsWith("SQL2008")
                                   )
                                {
                                    //it is SQL patch key
                                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                                    {
                                       

                                    cnt++;
                                    Controller.sqlRegKeys.Add(reg,
                                    new Reason(SourceType.Uninstall, "ProductId", "ProductId=SQL2008",true));



                                }

                            }
                                else if (rk.leaf.StartsWith("Microsoft SQL Server"))
                                {
                                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                                    {
                                      

                                    cnt++;
                                    Controller.sqlRegKeys.Add(reg,
                                    new Reason(SourceType.Uninstall, "Microsoft SQL Server", "It starts with \"Microsoft SQL Server\".",true));


                                }

                            }
                            }



                   

                    }
                }
            }//hiveRoot



            Controller.UpdateProgress("Uninstall Add keys:" + cnt, true);


        }


    }
}
