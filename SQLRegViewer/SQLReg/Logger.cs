using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SQLReg
{
    public static class Logger
    {
        public static string logFileName = "";
        public static int logNum = 0;
        public static StreamWriter logsw = null;
        public static void SetupLog(string fileName = "")
        {
            string computer = System.Environment.MachineName;
            var exe = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            if (String.IsNullOrEmpty(fileName)) fileName = exe+"_" + computer + "_";

            if (logsw != null) { logsw.Close(); logsw = null; }

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (!Directory.Exists(Path.Combine(path, "LOG")))
            {
                Directory.CreateDirectory(Path.Combine(path, "LOG"));
            }
            path = Path.Combine(path, "LOG");
            string name = fileName + logNum.ToString() + System.DateTime.Now.ToString("_yyyy-MM-dd_HH_mm_ss_") + ".log";

            logFileName = Path.Combine(path, name);

            logsw = new StreamWriter(logFileName, false);

            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LogMsg("SQLRegistryViewer Version " + ver );

            LogMsg(logFileName);

            ++logNum;
        }
        public static void LogMsg(string msg)
        {
            if (logsw != null)
            {
                logsw.WriteLine(
                 System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + msg);
                logsw.Flush();
            }
            else
            {
                SetupLog();
                if (logsw != null)
                {
                    logsw.WriteLine(
                     System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + msg);
                    logsw.Flush();
                }


            }

        }

        public static void LogWarning(string msg)
        {
            LogMsg("[WARNING]" + msg);
        }
        /*
        public static void LogError(string msg, [CallerMemberName]string memberName = "", [CallerFilePath]string file = "",
            [CallerLineNumber]int lineNum = 0)
        {

          

            //if (msg.Contains("[ERROR]"))
            //    Console.ForegroundColor = ConsoleColor.Red;
            //else if (msg.Contains("[WARNING]")) Console.ForegroundColor = ConsoleColor.Yellow;
            //else Console.ForegroundColor = ConsoleColor.Gray;
          
            string txt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "[ERROR]["   +Path.GetFileName(file) + ":" + lineNum + ":"+ memberName + "]"
                +
                "" + msg;
            if (logsw != null) { logsw.WriteLine(txt); logsw.Flush(); }
            else
            {
                SetupLog();
                if (logsw != null)
                {
                    logsw.WriteLine(txt); logsw.Flush();
                }

            }
 

        }


*/


        public static void LogError(string msg)
        {
            /*
            if (msg.Contains("[ERROR]"))
                Console.ForegroundColor = ConsoleColor.Red;
            else if (msg.Contains("[WARNING]")) Console.ForegroundColor = ConsoleColor.Yellow;
            else Console.ForegroundColor = ConsoleColor.Gray;
            */
            string txt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "[ERROR]" + msg;
            if (logsw != null) { logsw.WriteLine(txt); logsw.Flush(); }
            else
            {
                SetupLog();
                if (logsw != null)
                {
                    logsw.WriteLine(txt); logsw.Flush();
                }

            }



        }




    }
}
