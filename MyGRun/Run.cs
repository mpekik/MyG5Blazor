using MyG5Blazor.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MyGRun
{
    public partial class Run : Form
    {
        public Run()
        {
            InitializeComponent();
        }
        HTTPHandler httpHandler = new HTTPHandler();
        Config config = new Config();
        string fileName = string.Empty;
        string _myURL = string.Empty;
        string terminalId = string.Empty;
        string tokenId = string.Empty;
        string version = "";
        string versionName = "";
        string directory = Directory.GetCurrentDirectory();
        private async void Form1_Load(object sender, EventArgs e)
        {
            Run frm1 = new Run();
            label1.Left = (this.Size.Width - label1.Size.Width) / 2;
            label1.Top = (this.Size.Height - label1.Size.Height) / 2;
            await Task.Delay(1000);
            await CekUpdate();
            
            await RunApps();
        }

        private void CopyConfig()
        {
            //URL
            config._url.defaultURL= config.Read("URL", Config.PARAM_DEFAULT_URL);
            config._url.signURL = config.Read("URL", Config.PARAM_SIGN_URL);
            config._url.monitoringURL = config.Read("URL", Config.PARAM_MONITORING_URL);

            //Terminal
            config._terminal.systemMachine = config.Read("Terminal", Config.PARAM_MACHINE);
            config._terminal.regStatus = config.Read("Terminal", Config.PARAM_MYGRAPRI_REG_STATUS);
            config._terminal.token = config.Read("Terminal", Config.PARAM_TOKEN);

            //MyGraPARI
            config._mygrapari.uiVers = config.Read("MyGraPARI", Config.PARAM_MYGRAPARI_VER);
            config._mygrapari.monitoringVers = config.Read("MyGraPARI", Config.PARAM_DEVICE_MONITORING_VER);
            config._mygrapari.updatePatchInterval = config.Read("MyGraPARI", Config.PARAM_UPDATE_PATCH_INTERVAL);
            config._mygrapari.identification = config.Read("MyGraPARI", Config.PARAM_ADMIN_IDENTIFICATION);
            config._mygrapari.release = config.Read("MyGraPARI", Config.PARAM_ADMIN_RELEASE);
            config._mygrapari.monitoringInterval = config.Read("MyGraPARI", Config.PARAM_MONITORING_INTERVAL);
            config._mygrapari.loadDispenser = config.Read("MyGraPARI", Config.PARAM_MAX_RETRY_LOAD_DISPENSER);
            config._mygrapari.histMonth = config.Read("MyGraPARI", Config.PARAM_SCAN_HIST_MONTH);
            config._mygrapari.usbUp = config.Read("MyGraPARI", Config.PARAM_DELAY_USB_UP);
            config._mygrapari.usbDown = config.Read("MyGraPARI", Config.PARAM_DELAY_USB_DOWN);
            config._mygrapari.fingerPrintTry = config.Read("MyGraPARI", Config.PARAM_DEVICE_FINGERPRINT_TRY);
            config._mygrapari.readerObject = config.Read("MyGraPARI", Config.PARAM_DELAY_CREATE_READER_OBJECT);

            //Dispenser
            config._dispenser.dispenser = config.Read("Dispenser", Config.PARAM_DEVICE_DISPENSER);
            config._dispenser.cardReader = config.Read("Dispenser", Config.PARAM_DEVICE_CARD_READER);
            config._dispenser.cardReaderUSB = config.Read("Dispenser", Config.PARAM_DEVICE_CARD_READER_USB);
            config._dispenser.dispenserStock = config.Read("Dispenser", Config.PARAM_STOCK_DISPENSER_1);
            config._dispenser.persoExe = config.Read("Dispenser", Config.PARAM_PERSO_EXE);

            //IP Camera
            config._ipcamera.cameraURL = config.Read("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_URL);
            config._ipcamera.cameraUser = config.Read("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_USER);
            config._ipcamera.cameraPassword = config.Read("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_PASSWORD);
            config._ipcamera.cameraType = config.Read("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_TYPE);

            //Relay
            config._relay.relayPort = config.Read("IP_Camera", Config.PARAM_RELAY_PORT);
            config._relay.relayInterval = config.Read("IP_Camera", Config.PARAM_RELAY_ON_INTERVAL);

            //Printer
            config._printer.paperLength = config.Read("Printer", Config.PARAM_PAPER_LENGTH);
            config._printer.printerPort = config.Read("Printer", Config.PARAM_PRINTER_PORT);

            //EDC
            config._edc.edcPort = config.Read("EDC", Config.PARAM_EDC_PORT);

            //BILL
            config._bill.billPort = config.Read("BILL", Config.PARAM_BILL_PORT);
            config._bill.billBankStock = config.Read("BILL", Config.PARAM_BILL_BANK_STOK);
            config._bill.billAmount = config.Read("BILL", Config.PARAM_BILL_AMOUNT);

            //EKTP
            config._ektp.ektpPCID = config.Read("EKTP", Config.PARAM_EKTP_PCID);
            config._ektp.ektpCONF = config.Read("EKTP", Config.PARAM_EKTP_CONF);
            config._ektp.ektpUSB = config.Read("EKTP", Config.PARAM_EKTP_USB);
        }
        private async Task CekUpdate()
        {
            label1.Text = "Check for Update...";
            label1.Left = (this.Size.Width - label1.Size.Width) / 2;
            label1.Top = (this.Size.Height - label1.Size.Height) / 2;
            OurUtility.Write_Log("== Check for Update", "step-action");
            CopyConfig();
            await Task.Delay(1000);
            _myURL = config.ReadIP("URL", Config.PARAM_UPDATE_URL);
            terminalId = config.Read("Terminal", Config.PARAM_MACHINE);
            version = config.Read("MyGraPARI", Config.PARAM_MYGRAPARI_VER);
            tokenId = config.Read("Terminal", Config.PARAM_TOKEN);

            string loginURL = "update/v1/check-update";
            string myJson = "{ \"terminalId\": \"" + terminalId + "\"," +
                "\"version\": \"" + version + "\"" +
                    "}";
            string myURL = _myURL + loginURL;
            
            OurUtility.Write_Log("== Login Non-Trilogi", "step-action");
            OurUtility.Write_Log("== Request API : " + myJson, "step-action");
            string strResult = await httpHandler.PostCallAPI(myURL, myJson,terminalId,tokenId);
            //label3.Text = strResult;
            OurUtility.Write_Log("== Response API : " + strResult, "step-action");
            //if(true)
            if (strResult != null)
            {
                try
                {
                    JObject jobjResult = JObject.Parse(strResult);
                    //if (true)
                    if ((string)jobjResult["transaction"]["statusCode"] == "00")
                    {
                        OurUtility.Write_Log("== Status Code : " + (string)jobjResult["transaction"]["statusCode"] + " {Success}", "step-action");
                        OurUtility.Write_Log("== Login Berhasil", "step-action");
                        //if (true)
                        if ((string)jobjResult["isUpdateAvailable"] == "Y")
                        {
                            
                            OurUtility.Write_Log("== Update Available", "step-action");
                            fileName = (string)jobjResult["aplikasi"]["urlUpdate"];
                            version = (string)jobjResult["aplikasi"]["version"];
                            versionName= (string)jobjResult["aplikasi"]["versionName"];
                            if ((string)jobjResult["aplikasi"]["propertiesUpdate"] == "Y")
                            { }
                            await DownloadUpdate();
                        }else
                            OurUtility.Write_Log("== Program is Up to Date", "step-action");
                    }
                    else
                    {
                        //if (true)
                        string statusDescSub = "";
                        if ((string)jobjResult["transaction"]["errorCode"] != null)
                        {
                            OurUtility.Write_Log("== Status Code : " + (string)jobjResult["transaction"]["statusDesc"] + " {Error}", "step-action");                            
                            statusDescSub = (string)jobjResult["transaction"]["statusDesc"];
                        }
                        else
                        {
                           string _statusDescSub = ((string)jobjResult["transaction"]["statusDesc"]);
                            statusDescSub = _statusDescSub.Substring(0, _statusDescSub.IndexOf(":"));
                            OurUtility.Write_Log("== Status Code : " + statusDescSub + " {Error}", "step-action");                            
                        }
                        label1.Text = "Checking Failed...";
                        label1.Left = (this.Size.Width - label1.Size.Width) / 2;
                        label1.Top = (this.Size.Height - label1.Size.Height) / 2;
                        OurUtility.Write_Log("== Checking Failed - " + statusDescSub, "step-action");
                    }
                }
                catch (Exception ex)
                {
                    OurUtility.Write_Log("== Login Error :" + ex.Message, "step-action");
                    label1.Text = "Update Failed...";
                    //label2.Text = ex.Message.ToString();
                    label1.Left = (this.Size.Width - label1.Size.Width) / 2;
                    label1.Top = (this.Size.Height - label1.Size.Height) / 2;
                    OurUtility.Write_Log("== Update Failed - " + ex.Message.ToString(), "step-action");
                }
            }
            else
            {
                OurUtility.Write_Log("== Login Error : Hit WebService {Error}", "step-action");
                label4.Visible = false;
                label1.Text = "Checking Failed...";
                label1.Left = (this.Size.Width - label1.Size.Width) / 2;
                label1.Top = (this.Size.Height - label1.Size.Height) / 2;
                OurUtility.Write_Log("== Checking Failed - Hit WebServiceError", "step-action");
            }
            //await DownloadUpdate();
        }
        private async Task DownloadUpdate()
        {
            label1.Text = "Downloading Update...";
            label1.Left = (this.Size.Width - label1.Size.Width) / 2;
            label1.Top = (this.Size.Height - label1.Size.Height) / 2;
            OurUtility.Write_Log("== Downloading Update", "step-action");
            await Task.Delay(1000);
            string myURL = _myURL + "update/v1/download/"+fileName;
            string response = await httpHandler.GetCallAPI(myURL,"",terminalId,tokenId,fileName,directory);
            if (response=="Y")
            {
                label1.Text = "Download Completed";
                label1.Left = (this.Size.Width - label1.Size.Width) / 2;
                label1.Top = (this.Size.Height - label1.Size.Height) / 2;
                OurUtility.Write_Log("== Download Completed", "step-action");
                await DeleteOlder();
            }else
            {
                version = config._mygrapari.uiVers;
                label1.Text = "Download Failed";
                label1.Left = (this.Size.Width - label1.Size.Width) / 2;
                label1.Top = (this.Size.Height - label1.Size.Height) / 2;
                OurUtility.Write_Log("== Download Failed", "step-action");
            }
        }
        
        private async Task DeleteOlder()
        {
            //label1.Font = new Font("Microsoft Sans Serif", 48, FontStyle.Bold);
            label1.Text = "Updating to version" ;
            label1.Left = (this.Size.Width - label1.Size.Width) / 2;
            label1.Top = (this.Size.Height - label1.Size.Height) / 2;
            label4.Text = versionName;
            label4.Left = (this.Size.Width - label1.Size.Width) / 2;
            label4.Top = label1.Top+label1.Size.Height;
            OurUtility.Write_Log("== Updating", "step-action");
            await Task.Delay(1000);
            if (Directory.Exists(directory + @"\Old"))
            {
                EmptyFolder(new DirectoryInfo(directory+@"\Old"));
            }
            await MoveOld();
        }
        private void EmptyFolder(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                EmptyFolder(subfolder);
               }
            Directory.Delete(directoryInfo.FullName);
        }
        private void MoveFile(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Name != "IPTest-config.properties" && file.Name != "MyGRun.pdb" && file.Name != "MyGRun.exe" && file.Name != "MyGApps.zip" && file.Name != "MyGRun.exe.config")
                    File.Move(file.FullName, directory+@"\Old\" + file.Name);
            }
        }
        private async Task MoveOld()
        {
            await Task.Delay(1000);
            if (!Directory.Exists(directory+@"\Old"))
            {
                Directory.CreateDirectory(directory+@"\Old");
            }
            if (Directory.Exists(directory+@"\MyGApps"))
            {
                Directory.Move(directory+@"\MyGApps", directory+@"\Old\MyGApps");
            }
            if (Directory.Exists(directory+@"\wwwroot"))
            {
                Directory.Move(directory+@"\wwwroot", directory+@"\Old\wwwroot");
            }
            if (Directory.Exists(directory + @"\x86"))
            {
                Directory.Move(directory + @"\wwwroot", directory + @"\Old\wwwroot");
            }
            MoveFile(new DirectoryInfo(directory));
            await ExtractUpdate();
        }
        private async Task ExtractUpdate()
        {
            await Task.Delay(1000);
            string zipPath = directory+@"\MyGApps.zip";
            string extractPath = directory+@"\";
            if (File.Exists(directory+@"\MyGApps.zip"))
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
            label4.Visible = false;
            label1.Font = new Font("Microsoft Sans Serif", 72, FontStyle.Bold);
            label1.Text = "Update Complete";
            label1.Left = (this.Size.Width - label1.Size.Width) / 2;
            label1.Top = (this.Size.Height - label1.Size.Height) / 2;

            //await RunApps();
        }

        private void WriteConfig()
        {
            //version = "1234567890";
            //URL
            config.Write("URL", Config.PARAM_DEFAULT_URL, config._url.defaultURL);
            config.Write("URL", Config.PARAM_SIGN_URL, config._url.signURL);
            config.Write("URL", Config.PARAM_MONITORING_URL, config._url.monitoringURL);

            //Terminal
            config.Write("Terminal", Config.PARAM_MACHINE, config._terminal.systemMachine);
            config.Write("Terminal", Config.PARAM_MYGRAPRI_REG_STATUS, config._terminal.regStatus);
            config.Write("Terminal", Config.PARAM_TOKEN, config._terminal.token);

            //MyGraPARI
            config.Write("MyGraPARI", Config.PARAM_MYGRAPARI_VER, version);
            config.Write("MyGraPARI", Config.PARAM_DEVICE_MONITORING_VER, config._mygrapari.monitoringVers);
            config.Write("MyGraPARI", Config.PARAM_UPDATE_PATCH_INTERVAL, config._mygrapari.updatePatchInterval);
            config.Write("MyGraPARI", Config.PARAM_ADMIN_IDENTIFICATION, config._mygrapari.identification);
            config.Write("MyGraPARI", Config.PARAM_ADMIN_RELEASE, config._mygrapari.release);
            config.Write("MyGraPARI", Config.PARAM_MONITORING_INTERVAL, config._mygrapari.monitoringInterval);
            config.Write("MyGraPARI", Config.PARAM_MAX_RETRY_LOAD_DISPENSER, config._mygrapari.loadDispenser);
            config.Write("MyGraPARI", Config.PARAM_SCAN_HIST_MONTH, config._mygrapari.histMonth);
            config.Write("MyGraPARI", Config.PARAM_DELAY_USB_UP, config._mygrapari.usbUp);
            config.Write("MyGraPARI", Config.PARAM_DELAY_USB_DOWN, config._mygrapari.usbDown);
            config.Write("MyGraPARI", Config.PARAM_DEVICE_FINGERPRINT_TRY, config._mygrapari.fingerPrintTry);
            config.Write("MyGraPARI", Config.PARAM_DELAY_CREATE_READER_OBJECT, config._mygrapari.readerObject);

            //Dispenser
            config.Write("Dispenser", Config.PARAM_DEVICE_DISPENSER, config._dispenser.dispenser);
            config.Write("Dispenser", Config.PARAM_DEVICE_CARD_READER, config._dispenser.cardReader);
            config.Write("Dispenser", Config.PARAM_DEVICE_CARD_READER_USB, config._dispenser.cardReaderUSB);
            config.Write("Dispenser", Config.PARAM_STOCK_DISPENSER_1, config._dispenser.dispenserStock);
            config.Write("Dispenser", Config.PARAM_PERSO_EXE, config._dispenser.persoExe);

            //IP Camera
            config.Write("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_URL, config._ipcamera.cameraURL);
            config.Write("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_USER, config._ipcamera.cameraUser);
            config.Write("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_PASSWORD, config._ipcamera.cameraPassword);
            config.Write("IP_Camera", Config.PARAM_DEVICE_CAMERA_IP_TYPE, config._ipcamera.cameraType);

            //Relay
            config.Write("Relay", Config.PARAM_RELAY_PORT, config._relay.relayPort);
            config.Write("Relay", Config.PARAM_RELAY_ON_INTERVAL, config._relay.relayInterval);

            //Printer
            config.Write("Printer", Config.PARAM_PAPER_LENGTH, config._printer.paperLength);
            config.Write("Printer", Config.PARAM_PRINTER_PORT, config._printer.printerPort);

            //EDC
            config.Write("EDC", Config.PARAM_EDC_PORT, config._edc.edcPort);

            //BILL
            config.Write("BILL", Config.PARAM_BILL_PORT, config._bill.billPort);
            config.Write("BILL", Config.PARAM_BILL_BANK_STOK, config._bill.billBankStock);
            config.Write("BILL", Config.PARAM_BILL_AMOUNT, config._bill.billAmount);

            //EKTP
            config.Write("EKTP", Config.PARAM_EKTP_PCID, config._ektp.ektpPCID);
            config.Write("EKTP", Config.PARAM_EKTP_CONF, config._ektp.ektpCONF);
            config.Write("EKTP", Config.PARAM_EKTP_USB, config._ektp.ektpUSB);

        }
        private async Task RunApps()
        {
            WriteConfig();
            await Task.Delay(1000);
            Process.Start(directory+@"\MyGApps\Run.vbs");
            this.Close();
        }
    }
}
