using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGRun
{
    class OurUtility
    {
        public static void Write_Log(string p_str, string p_prefix_fileName)
        {
            string msg = string.Empty;

            Write_Log(p_str, p_prefix_fileName, ref msg);
        }
        public static void Write_Log(string p_str, string p_prefix_fileName, ref string p_message)
        {
            p_message = string.Empty;

            try
            {
                string dir = Directory_Logs() + @"\" + DateTime.Now.ToString("yyyyMM");
                CreateDirectory(dir);

                string fileName = dir + @"\" + p_prefix_fileName + DateTime.Now.ToString("yyyyMMdd-HH") + ".log";

                string msg = string.Empty;
                Write_to_File(DateTime.Now.ToString("yyyyMMdd HH:mm:ss-fff") + " " + p_str, fileName, ref msg);
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }
        }
        public static bool Write_to_File(string p_data, string p_fileName, ref string p_message)
        {
            bool result = false;

            p_message = string.Empty;

            try
            {
                using (StreamWriter writer = new StreamWriter(p_fileName, true))
                {
                    writer.WriteLine(p_data);
                }

                result = true;
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }
        public static string Directory_Logs()
        {
            string dir = Directory.GetCurrentDirectory() + @"\Logs\Update";

            CreateDirectory(dir);

            return dir;
        }
        public static string CreateDirectory(string p_directory)
        {
            try
            {
                Directory.CreateDirectory(p_directory);
            }
            catch { }

            return string.Empty;
        }
    }
}
