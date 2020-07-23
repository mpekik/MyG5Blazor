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
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MyG5Blazor.Data
{
    public class OurUtility
    {
        private static Config config = new Config();
        public static string RandomString(int stringLength)
        {
            Random rd = new Random();
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            char[] chars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        
        public static void Write_Log(string p_str, string p_prefix_fileName)
        {
            string msg = string.Empty;

            Write_Log(p_str, p_prefix_fileName, ref msg);
        }
        private static byte[] ReadFully(Stream input)
        {
            try
            {
                int bytesBuffer = 1024;
                byte[] buffer = new byte[bytesBuffer];
                using (MemoryStream ms = new MemoryStream())
                {
                    int readBytes;
                    while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, readBytes);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Exception handling here:  Response.Write("Ex.: " + ex.Message);
                Console.WriteLine("Exception " + ex.Message);
            }

            return null;
        }
        public static byte[] CaptureImage()
        {
            Bitmap bm;
            string msg = string.Empty;
            byte[] result = null;
            string ipCamURL = config.Read("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_URL);
            string ipCamUser = config.Read("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_USER);
            string ipCamPass = config.Read("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_PASSWORD);
            try
            {
                Uri myUri = new Uri(ipCamURL);
                WebRequest myWebRequest = HttpWebRequest.Create(myUri);

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;

                NetworkCredential myNetworkCredential = new NetworkCredential(ipCamUser, ipCamPass);

                CredentialCache myCredentialCache = new CredentialCache();
                myCredentialCache.Add(myUri, "Digest", myNetworkCredential);

                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Credentials = myCredentialCache;

                var response = (HttpWebResponse)myHttpWebRequest.GetResponse();
                Console.WriteLine(response.StatusCode);
                result = ReadFully(response.GetResponseStream());
                MemoryStream mStream = new MemoryStream();
                byte[] pData = result;
                mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
                bm = new Bitmap(mStream, false);
                Write_Image(bm, ref msg);
                var imageBytesStr = Convert.ToBase64String(pData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return result;
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
        public static void Write_Image(Bitmap bmp, ref string p_message)
        {
            p_message = string.Empty;

            try
            {
                string dir = Directory_Images() + @"\" + DateTime.Now.ToString("yyyyMM");
                CreateDirectory(dir);

                string msg = string.Empty;
                bmp.Save(DateTime.Now.ToString("yyyyMMdd HH:mm:ss-fff") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }
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
        public static string Directory_Images()
        {
            string dir = Directory.GetCurrentDirectory() + @"\Images";

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

        public static async Task AuditTrail(Transaction trans, Menu menu, Costumer cst)
        {
            string _myURL = config.Read("URL", Config.PARAM_DEFAULT_URL);
            string saveURL = "mygrapari/log/v1/save";
            
            trans.endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            string auditTrail = string.Empty;
            int stepTrail = 1;
            auditTrail = auditTrail + "[ ";
            foreach (dynamic at in trans._auditTrail)
            {
                auditTrail = auditTrail +
                    "{ \"step\" : \"" + stepTrail + "\"," +
                    "\"action\" : \"" + at._action + "\"," +
                    "\"data\" : \"" + at._data + "\"," +
                    "\"result\" : \"" + at._result + "\"" +
                    "},";
                stepTrail += 1;
            }
            auditTrail = auditTrail.Remove(auditTrail.Length - 1);

            string myJson2 = "{ \"transaction\" : " +
                "{ \"transactionId\" : \"" + trans.transID + "\"," +
                "\"terminalId\" : \"" + trans.termID + "\"," +
                "\"transactionType\" : \"" + trans.jenisTrans + "\"," +
                    "\"noHp\" : \"" + cst.PhoneNumber + "\"," +
                    "\"startTime\" : \"" + trans.startTime.ToString() + "\"," +
                    "\"endTime\" : \"" + trans.endTime.ToString() + "\"," +
                    "\"status\" : \"" + trans.status + "\"," +
                    "\"description\" : \"" + trans.errorCode + "\"" +
                    "\"lembar_uang\" : \"" + cst.intUangCount + "\"," +
                    "\"jumlah_kartu\" : \"" + trans.jumlah_kartu + "\"," +
                    "}, \"auditTrail\" : " + auditTrail + "]}";
            string myURL2 = _myURL + saveURL;

            string strResult2 = await OurUtility.PostCallAPI(myURL2, myJson2,menu);
        }
        public static async Task<string> PostCallAPI(string url, string jsonString, Menu menu)
        {
            string secret = OurUtility.RandomString(10);
            long unix_timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string unixtimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(0, 10);
            string md5Input = menu.terminalId + secret + menu.tokenId + unixtimestamp;
            string signature = OurUtility.CreateMD5(md5Input);

            string ret = string.Empty;
            signature = signature.ToLower();
            //Task.Delay(200);
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    using (HttpClient client = new HttpClient(handler))
                    {
                        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        //                    client.DefaultRequestHeaders.Add("signature-key", signature);
                        //                  client.DefaultRequestHeaders.Add("secret-key", secret);
                        content.Headers.Add("signature-key", signature);
                        content.Headers.Add("secret-key", secret);
                        Console.WriteLine(signature);
                        Console.WriteLine(secret);
                        await Task.Delay(2000);
                        var response = await client.PostAsync(url, content);
                        //Console.WriteLine(response.StatusCode.ToString());
                        if (response != null)
                        {

                            if (response.IsSuccessStatusCode || response.StatusCode.ToString() == "BadRequest" || response.StatusCode.ToString() == "InternalServerError")
                            {
                                var jsonStringResult = await response.Content.ReadAsStringAsync();
                                return jsonStringResult;
                            }
                            else
                            {
                                return response.StatusCode.ToString();
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            return ret;
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