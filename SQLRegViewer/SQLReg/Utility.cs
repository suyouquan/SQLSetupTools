using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Runtime.CompilerServices;

namespace SQLReg
{
    public static class Utility
    {


        public static string ReverseStringDirect(string s)
        {
            char[] array = new char[s.Length];
            int forward = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                array[forward++] = s[i];
            }
            return new string(array);
        }

        /// <summary>
        /// swap string characters between odd and even chars
        /// say, "1234"==>"2143"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SwapOddEven(string stringToSwap)
        {

            char[] array = stringToSwap.ToCharArray();

            // even size strings iterate the whole array
            // odd size strings stop one short 
            int offset = (stringToSwap.Length % 2);
            for (int i = 0; i < array.Length - offset; i += 2)
            {
                char temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
            }
            return new String(array);


        }

        /// <summary>
        /// convert normal GUID like product code, UpgradeCode to compressed registry key style
        /// 93 CE EF F6   AA 3D  4E 99   84 E1 8F FB F7 2C 43 0D  <--- Source
        //  6F FE EC 39   D3 AA  99 E4   48 1E F8 BF 7F C2 34 D0  <--- Target
        //  -----------   -----  -----   -----------------------
        //  01 23 45 67   89 01  23 45   67 89 01 23 45 67 89 01
        //  0                1                 2              3

        // examples:
        // {93CEEFF6-AA3D-4E99-84E1-8FFBF72C430D} -> 6FFEEC39D3AA99E4481EF8BF7FC234D0
        // {0E837AF0-4C92-4077-83F0-D022073F17C0} -> 0FA738E029C47704380F0D2270F3710C
        // {4AE61EA4-9F6F-4616-9035-0CF343EA462D} -> 4AE16EA4F6F961640953C03F34AE64D2
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        /// 
        public static string ConvertGUID2CompressedRegistryStyle(string guid, bool debug = false)
        {
            if (guid.Contains("{") && guid.Contains("}"))
            {
                if (guid.Length != 38)
                {
                    if (debug) Logger.LogError("Guid length!=38 " + guid);
                    return null;
                }
                string[] parts = guid.Replace("{", "").Replace("}", "").Split('-');
                if (parts.Length != 5)
                {
                    if (debug) Logger.LogError("Guid doesn't have 4 characters of '-'  " + guid);
                    return null;
                }

                //Now i got a valid guid

                string part0 = ReverseStringDirect(parts[0]);
                string part1 = ReverseStringDirect(parts[1]);
                string part2 = ReverseStringDirect(parts[2]);
                string part3 = SwapOddEven(parts[3]);
                string part4 = SwapOddEven(parts[4]);
                return part0 + part1 + part2 + part3 + part4;

            }
            else
            {
                if (debug) Logger.LogError("Guid doesn't have { and } " + guid);
                return null;
            };


        }

        public static string ConvertCompressedCodeToNormal(string ccode)
        {
            if (ccode.Length != 32) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            // {93CEEFF6-AA3D-4E99-84E1-8FFBF72C430D} -> 6FFEEC39D3AA99E4481EF8BF7FC234D0
            //6FFEEC39 D3AA99E4481EF8BF7FC234D0

            //Now i got a valid guid
            string[] parts = new string[5];

            parts[0] = ccode.Substring(0, 8);
            parts[1] = ccode.Substring(8, 4);
            parts[2] = ccode.Substring(12, 4);
            parts[3] = ccode.Substring(16, 4);
            parts[4] = ccode.Substring(20, 12);

            string part0 = ReverseStringDirect(parts[0]);
            string part1 = ReverseStringDirect(parts[1]);
            string part2 = ReverseStringDirect(parts[2]);
            string part3 = SwapOddEven(parts[3]);
            string part4 = SwapOddEven(parts[4]);


            return part0 + "-"+ part1 +"-"+ part2 +"-"+ part3 +"-"+ part4;
            




        }

        public static string CleanFileName(string filename)
        {
            var builder = new StringBuilder();
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            foreach (var cur in filename)
            {
                if (!invalid.Contains(cur))
                {
                    builder.Append(cur);
                }
                else builder.Append("_");
            }
            return builder.ToString();
        }

        public static List<string> GetFilesFromFolder(string sourceDirectory, string[] fileTypes)
        {
            List<string> resultFiles = new List<string>();

            try
            {
                //var mdmpFiles = Directory.EnumerateFiles(sourceDirectory, "*.mdmp", SearchOption.AllDirectories);

                DirectoryInfo di = new DirectoryInfo(sourceDirectory);
                var i = 0;

                var directory = new DirectoryInfo(sourceDirectory);
                //var fileTypes = new[] { "*.mdmp", "*.dmp" };
                var files = fileTypes.SelectMany(p => directory.EnumerateFiles(p, SearchOption.AllDirectories));

                //foreach (var dumpfile in di.EnumerateFiles("*.mdmp", SearchOption.AllDirectories))
                foreach (var f in files)
                {
                    i++;
                    // if (i > 6) break;
                    resultFiles.Add(f.FullName);


                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);


            }


            return resultFiles;
        }








    }
}
