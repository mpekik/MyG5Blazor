using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGRun
{
    class HTTPHandler
    {
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
        public async Task<string> GetCallAPI(string url, string jsonString, string terminalId, string tokenId, string fileName)
        {
            string secret = RandomString(10);
            long unix_timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string unixtimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(0, 10);
            string md5Input = terminalId + secret + tokenId + unixtimestamp;
            string signature = CreateMD5(md5Input);
            //url = "https://homepages.cae.wisc.edu/~ece533/images/airplane.png";
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
                        //await Task.Delay(2000);
                        
                        var response = await client.GetAsync(url);
                        //Console.WriteLine(response.StatusCode.ToString());
                        if (response != null)
                        {

                            if (response.IsSuccessStatusCode) 
                            {
                                string OutputDirectory = @"C:\MyGrapari\";
                                var httpStream = await response.Content.ReadAsStreamAsync();
                                var filePath = Path.Combine(OutputDirectory, "MyGApps.zip");
                                using (var fileStream = File.Create(filePath))
                                    using(var reader = new StreamReader(httpStream))
                                {
                                    httpStream.CopyTo(fileStream);
                                    fileStream.Flush();            
                                }
                                return "Y";
                            }
                            else
                            {
                                return "N";
                            }
                        }
                        else
                        {
                            return "N";
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return "N";
            }
        }
        public async Task<string> PostCallAPI(string url, string jsonString, string terminalId, string tokenId)
        {
            string secret = RandomString(10);
            long unix_timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string unixtimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(0, 10);
            string md5Input = terminalId + secret + tokenId + unixtimestamp;
            string signature = CreateMD5(md5Input);

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
    }
}
