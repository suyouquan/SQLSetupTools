using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;

namespace FixSQLMSI
{
   public class SQLProduct
    {
        public static Dictionary<String, String> VersionMap
            = new Dictionary<string, string>()
            {
                {"10.00","SQL2008" },
                {"10.50","SQL2008R2" },
                {"11","SQL2012" },
                {"12","SQL2014" },
                {"13","SQL2016" },
                {"14","SQL2017" }


            };

        /// <summary>
        /// Return list of SQL server  products
        /// </summary>
        /// <param name="products"></param>
     public static List<String> GetInstalledSQLProduct(IEnumerable<ProductInstallation> allProducts)
        {
            List<String> products = new List<string>();

            foreach(ProductInstallation p in allProducts)
            {
                try
                {
                    if (p.ProductName.ToUpper().Contains("SQL"))
                    {
                        foreach (string v in VersionMap.Keys)
                        {
                            if ((p.ProductVersion.Major.ToString() +"."+ p.ProductVersion.MajorRevision.ToString()).StartsWith(v))
                            {
                                products.Add(p.ProductName + ":" + p.ProductVersion);
                            }
                        }

                    }
                }
                catch(Exception e)
                { }
 
            } //foreach


            return products;
        } //end function













    }
}
