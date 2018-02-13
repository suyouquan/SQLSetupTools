using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLReg;
using System.IO;

namespace SQLMsiMsp
{
    class Program
    {
        public static void Output(string msg)
        {
            Console.WriteLine(msg);
        }
        
        public static void MsiMspScan(string path,string outputFileName)
        {
            Logger.SetupLog();
            Controller.Init(Output, null,null);

            Output ("Scanning MSI/MSP packages from:" + path);
            //   SQLProduct sql2016 = new SQLProduct("SQL2016", SQLSetupSource);
            //  sqlProducts.Add(sql2016);
          
           SQLProduct  sql = new SQLProduct(outputFileName, path);
            

            try
            {
                //string content = OutputProcessor.SerializeToXML<SQLProduct>(sql);
                //File.WriteAllText(outputFileName, content);

                string name = "SQLMsiMsp_Scan_" + System.DateTime.Now.ToString("_yyyy-MM-dd_HH_mm_ss");
                SQLProductSum sumFromSetupSrc = new SQLProductSum(name, path, "By SQLMsiMspScan.");
                sumFromSetupSrc.InitOrAddHashSet(sql);
                string file = Path.GetFileNameWithoutExtension(outputFileName).Replace(".sum","") + ".sum.xml";
                string content = OutputProcessor.DataContractSerializeToXML<SQLProductSum>(sumFromSetupSrc);
                File.WriteAllText(file, content);

            }
            catch (Exception ex)
            {
                Output(ex.Message);
                Logger.LogError(ex.Message);
            }
            
        }

        static void Main(string[] args)
        {
           
            if(args.Length<2)
            {
                Console.WriteLine("Usage:SQLMsiMsp.exe srcFolder outputFileName\nExample:SQLMsiMsp \"C:\\SQLSetup\\SQL2016\" c:\\temp\\SQL2016RTM.xml");
                return;
            }
            else
            {
                
                string path = args[0];
                string filename = args[1];
                Output("Path:" + args[0]);
                Output("Output:" + args[1]);
                MsiMspScan(path, filename);

                Output("Finsihed. MsiMsp meta data saved to:"+filename +"");
            }

        }
    }
}
