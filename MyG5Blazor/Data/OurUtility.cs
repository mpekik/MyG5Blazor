using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace MyG5Blazor.Data
{
    public class OurUtility
    {
        public static void Write_Log(string p_str, string p_prefix_fileName)
        {
            string msg = string.Empty;

            Write_Log(p_str, p_prefix_fileName, ref msg);
        }
        public static string StringToDTString(string input)
        {
            string result = string.Empty;
            string Year = input.Substring(0, 2);
            string month = input.Substring(2, 2);
            string Day = input.Substring(4);

            result = Day + "-" + month + "-20" + Year;
            return result;
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
        public static string strIns(string strData, string strInsert)
        {
            int i = 0;
            string strRet = strData;
            try
            {
                for (i = strRet.Length - 2; i > 0; i -= 2)
                {
                    strRet = strRet.Insert(i, strInsert);
                }
            }
            catch (Exception ex)
            {
            }
            return strRet;
        }
        public static string Directory_Logs()
        {
            string dir = Directory.GetCurrentDirectory() + @"\Logs";

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
        public static int ToInt32(string p_str)
        {
            int result = 0;

            try
            {
                result = Convert.ToInt32(p_str);
            }
            catch { }

            return result;
        }

        public static string ReadAllText(string p_fileName)
        {
            string result = string.Empty;

            try
            {
                result = System.IO.File.ReadAllText(p_fileName);
            }
            catch { }

            return result;
        }

        public static byte[] StringToByteArray(string p_str)
        {
            byte[] result = null;

            try
            {
                p_str = p_str.Replace(Environment.NewLine, "");
                p_str = p_str.Replace("0x", "");
                p_str = p_str.Replace(" ", "");

                string[] x_bytes = p_str.Split(',');

                result = new byte[x_bytes.Length];
                int i = 0;
                foreach (string x_byte in x_bytes)
                {
                    result[i] = byte.Parse(x_byte, System.Globalization.NumberStyles.HexNumber);

                    i++;
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        private static string p_delay_UP = "3000";
        private static string p_delay_DOWN = "500";

        private static string Response_Perso_Success = "\r\n0\r\n";
        private static string Response_Perso_Failed = "\r\n1\r\n";
        public static bool MyPerso(string p_perso_exe, string p_rpsData, ref string p_message)
        {
            bool result = false;

            p_message = string.Empty;

            Write_Log("Call function MyPerso()", "step-action");

            // pastikan bahwa Close jalur ke eKTP
            Switcher.DOWN_eKTP(p_delay_DOWN);

                Switcher.UP_Reader_1(p_delay_UP);

            try
            {
                string workingDirectory = Path.GetDirectoryName(p_perso_exe);
                Process process = new Process
                {
                    StartInfo = {
                        FileName = p_perso_exe
                        , Arguments = p_rpsData
                        , UseShellExecute = false
                        , RedirectStandardOutput = true
                        , RedirectStandardError = true
                        , CreateNoWindow = true
                        , WindowStyle = ProcessWindowStyle.Minimized
                        , WorkingDirectory = workingDirectory
                    }
                };

                process.Start();
                string str = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                process.Close();
                process.Dispose();
                process = null;
                if (str == Response_Perso_Success)
                {
                    result = true;

                    Write_Log("MyPerso() Success [return 0]", "step-action");
                }

                if (!result)
                {
                    p_message = "Perso Failed [return 1]";

                    Write_Log("MyPerso() Failed [return 1]", "step-action");
                }
            }
            catch (Exception exception)
            {
                p_message = "[file : " + p_perso_exe + "] " + exception.Message;
            }
                Switcher.DOWN_Reader_1(p_delay_DOWN);

            return result;
        }
    }
}
