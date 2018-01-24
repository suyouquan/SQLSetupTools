using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

using System.Reflection;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace FixMissingMSI
{
   public static class Output
    {


        /// <summary>
        /// output list to readable text file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string FormatListTXT<T>(IEnumerable<T> collection) where T : class
        {
            Logger.LogMsg("FormatListTXT start.");
            if ((collection == null) || collection.Count<T>() == 0)

            {
                //return empty table
                return "";

            }

            List<int> columnMaxWidths = GetColumnMaxWidth(collection);

            //get the names of fields
            StringBuilder result = new StringBuilder();
            StringBuilder result_forEasyReading = new StringBuilder();
           
      
            // FieldInfo[] fieldNames = typeof(T).GetFields();// collection.First<T>().GetType().GetFields();
            var fieldNames = GetAllFields(typeof(T)).ToArray();

            //foreach (FieldInfo f in fieldNames)
            for (int i = 0; i < fieldNames.Length; i++)
            {
                FieldInfo f = fieldNames[i];                 
                if (f.Name.StartsWith("_")) continue;
                else
                {
                    string fname = f.Name.Replace("<", "").Replace(">k__BackingField", "");
                    //add some interesting padding so it look good during debugging. every column should at least x charaters, otherwise padding with space
                    int len = fname.Length ;  
               

                    int COLUMNLEN = columnMaxWidths[i];
                    string paddingSpace = "";
                    if (len < COLUMNLEN) paddingSpace = new string(' ', COLUMNLEN - len);
                    //if name length is greather than that, need to adjus the list so that the row can match with column width
                    else columnMaxWidths[i] = len;//two quotes,one comma,and one space

                    if (i < fieldNames.Length - 1)
                        result_forEasyReading.Append("" + fname + ",  " + paddingSpace);
                    else result_forEasyReading.Append("" + fname + "  " + paddingSpace);

                }


            }
           
            result.Append(result_forEasyReading.ToString());
            result.Append("\n");

            foreach (T element in collection)
            {
                StringBuilder rowStr = new StringBuilder();
               
                FieldInfo[] fields = fieldNames;// element.GetType().GetFields();

               
                for (int i = 0; i < fields.Length; i++)
                //foreach (var f in fields)
                {
                    var f = fields[i];
                    if (f.Name.StartsWith("_")) continue;

                    string columDataStr = "";
                    try
                    {
 
                            try
                            {

                            if (f.FieldType == typeof(System.DateTime))
                            {
                                var t = (System.DateTime)f.GetValue(element);
                                columDataStr = t.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }
                            else
                            {
                                var val = f.GetValue(element);
                                if (val != null) columDataStr = f.GetValue(element).ToString();


                            }
                                
                            }
                            catch (Exception ex) { columDataStr = ""; }
                        
                        // Console.Write(f.Name + ":" + f.GetValue(element) + " ");
                    }
                    catch (Exception ex)
                    {
                        columDataStr = "";
                        Logger.LogMsg("OutputProcessor:Process:" + ex.Message);
                    }

                    //don't replace " with '
                    //columDataStr = columDataStr.Replace("\"", "'");

                    int len = columDataStr.Length;
                    int COLUMNLEN = columnMaxWidths[i];
                    string paddingSpace = "";
                    if (len < COLUMNLEN) paddingSpace = new string(' ', COLUMNLEN - len);
                    if (i < fields.Length - 1) rowStr.Append("" + (columDataStr) + ",  " + paddingSpace);
                    else  rowStr.Append("" + (columDataStr) + "  " + paddingSpace);
                } //for each field

                
                rowStr.Append("\n");

                result.Append(rowStr);

                //Console.WriteLine("");

            } //foreach 
            result.Append("\n");
            Logger.LogMsg("FormatListTXT end.");
            return result.ToString();

        }

        /// <summary>
        /// output list to csv text file,without padding space
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string FormatListCSV<T>(IEnumerable<T> collection) where T : class
        {
            Logger.LogMsg("FormatListCSV start.");
            if ((collection == null) || collection.Count<T>() == 0)

            {
                //return empty table
                return "";

            }

             
            //get the names of fields
            StringBuilder result = new StringBuilder();
            StringBuilder result_forEasyReading = new StringBuilder();


            // FieldInfo[] fieldNames = typeof(T).GetFields();// collection.First<T>().GetType().GetFields();
            var fieldNames = GetAllFields(typeof(T)).ToArray();

            //foreach (FieldInfo f in fieldNames)
            for (int i = 0; i < fieldNames.Length; i++)
            {
                FieldInfo f = fieldNames[i];
                if (f.Name.StartsWith("_")) continue;
                else
                {
                    string fname = f.Name.Replace("<", "").Replace(">k__BackingField", "");
                    //add some interesting padding so it look good during debugging. every column should at least x charaters, otherwise padding with space
                    int len = fname.Length; //+2 because the js table will have sort sign


                  
                    string paddingSpace = "";
                    
                    if (i < fieldNames.Length - 1)
                        result_forEasyReading.Append("\"" + fname + "\"," + paddingSpace);
                    else result_forEasyReading.Append("\"" + fname + "\"" + paddingSpace);

                }


            }

            result.Append(result_forEasyReading.ToString());
            result.Append("\n");

            foreach (T element in collection)
            {
                StringBuilder rowStr = new StringBuilder();

                FieldInfo[] fields = fieldNames;// element.GetType().GetFields();


                for (int i = 0; i < fields.Length; i++)
                //foreach (var f in fields)
                {
                    var f = fields[i];
                    if (f.Name.StartsWith("_")) continue;

                    string columDataStr = "";
                    try
                    {

                        try
                        {

                            if (f.FieldType == typeof(System.DateTime))
                            {
                                var t = (System.DateTime)f.GetValue(element);
                                columDataStr = t.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }
                            else
                            {
                                var val = f.GetValue(element);
                               if(val!=null) columDataStr = f.GetValue(element).ToString();
                            }

                        }
                        catch (Exception ex) { columDataStr = ""; }

            
                    }
                    catch (Exception ex)
                    {
                        columDataStr = "";
                        Logger.LogMsg("OutputProcessor:Process:" + ex.Message);
                    }

                    //replace " with '
                    columDataStr = columDataStr.Replace("\"", "'");
                    int len = columDataStr.Length;
                     string paddingSpace = "";
                     if (i < fields.Length - 1) rowStr.Append("\"" + (columDataStr) + "\"," + paddingSpace);
                    else rowStr.Append("\"" + (columDataStr) + "\"" + paddingSpace);
                } //for each field


                rowStr.Append("\n");

                result.Append(rowStr);

                //Console.WriteLine("");

            } //foreach 
            result.Append("\n");
            Logger.LogMsg("FormatListCSV end.");
            return result.ToString();

        }

        public static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }

        /// <summary>
        /// caculate column max width, so that the JSON output file is more reabable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<int> GetColumnMaxWidth<T>(IEnumerable<T> collection) where T : class
        {
            Logger.LogMsg("GetColumnMaxWidth start.");

            //the caller knows the collection is not empty. 
            var ele = collection.First();
            var fields = GetAllFields(typeof(T)).ToArray();
            
            int colNums = fields.Count();// ele.GetType().GetFields().Length;

            

            List<int> columnMaxWidths = new List<int>(new int[colNums]);

            foreach (T element in collection)
            {
                //FieldInfo[] fields = element.GetType().GetFields();

                for (var i = 0; i < fields.Length; i++)
                {
                    try
                    {
                        int len = 0;
                        if (fields[i].FieldType == typeof(System.DateTime))
                        {
                            len = 23; //ToString("yyyy-MM-dd HH:mm:ss.fff");
                        }
                        else
                        {
                            var val = fields[i].GetValue(element);
                            if(val!=null) len = fields[i].GetValue(element).ToString().Length;

                        }
                        //max width, but not longer than 100 chars
                        columnMaxWidths[i] = Math.Min(2000, Math.Max(columnMaxWidths[i], len));
                    }
                    catch (Exception e)
                    {
                        
                        //eat the exception. column width is not critcal thing
                    }
                }

            }
            Logger.LogMsg("GetColumnMaxWidth end.");
            return columnMaxWidths;


        }
    }
}
