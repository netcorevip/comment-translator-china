using System;
using System.IO;
using System.Text;

namespace CommentTranslator.Util
{

    public static class LogHelper
    {
        public static void LogFile(this object data, string name = "", string fliename = "Log")
        {
            string fileName = "/" + fliename + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string serverPath = "logs/";
            //  string wlPath = AppDomain.CurrentDomain.BaseDirectory + serverPath;   //当前运行环境目录地址
            string wlPath = @"D:\LogVisx\" + serverPath;
            if (!Directory.Exists(wlPath))
                Directory.CreateDirectory(wlPath); //如果没有该目录，则创建
            StreamWriter sw = new StreamWriter(wlPath + fileName, true, Encoding.UTF8);
            sw.WriteLine("记录时间：" + DateTime.Now.ToString() + "，标题：" + name);
            sw.WriteLine(data.ToStr() + "\r\n");
            sw.Close();
        }



        public static void LogFileJson(this object data, bool createfile = false, string fliename = "Log")
        {
            string fileName = "/" + fliename + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string serverPath = "logs/";
            string wlPath = AppDomain.CurrentDomain.BaseDirectory + serverPath;   //当前运行环境目录地址

            if (!Directory.Exists(wlPath))
            {
                Directory.CreateDirectory(wlPath); //如果没有该目录，则创建
            }
            else
            {
                if (createfile)
                {
                    fileName = "/" + fliename + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                }
            }
            StreamWriter sw = new StreamWriter(wlPath + fileName, true, Encoding.UTF8);
            sw.WriteLine(data.ToStr());
            sw.Close();
        }

        private static string ToStr(this object data)
        {
            //string、int类型不需要json转换
            var type = data?.GetType();
            string write = "";
            if (typeof(string) == type || typeof(int) == type || typeof(decimal) == type || typeof(double) == type || typeof(Guid) == type)
            {
                write = data.ToString();
            }
            else
            {
                write = data.ToString();
            }
            return write;
        }
    }
}
