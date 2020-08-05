using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.IO;

namespace MyG5Blazor.Data
{
    public class Config
    {
        public class URL
        {
            public string defaultURL { get; set; }
            public string signURL { get; set; }
            public string monitoringURL { get; set; }
        }
        public URL _url = new URL();

        public class Terminal
        {
            public string systemMachine { get; set; }
            public string regStatus { get; set; }
            public string token { get; set; }
        }
        public Terminal _terminal = new Terminal();

        public class MyGraPARI
        {
            public string uiVers { get; set; }
            public string monitoringVers { get; set; }
            public string updatePatchInterval { get; set; }
            public string identification { get; set; }
            public string release { get; set; }
            public string monitoringInterval { get; set; }
            public string loadDispenser { get; set; }
            public string histMonth { get; set; }
            public string usbUp { get; set; }
            public string usbDown { get; set; }
            public string fingerPrintTry { get; set; }
            public string readerObject { get; set; }
        }
        public MyGraPARI _mygrapari = new MyGraPARI();
        public class Dispenser
        {
            public string dispenser { get; set; }
            public string cardReader { get; set; }
            public string cardReaderUSB { get; set; }
            public string dispenserStock { get; set; }
            public string persoExe { get; set; }
        }
        public Dispenser _dispenser = new Dispenser();
        public class IPCamera
        {
            public string cameraURL { get; set; }
            public string cameraUser { get; set; }
            public string cameraPassword { get; set; }
            public string cameraType { get; set; }
        }
        public IPCamera _ipcamera = new IPCamera();
        public class Relay
        {
            public string relayPort { get; set; }
            public string relayInterval { get; set; }
        }
        public Relay _relay = new Relay();
        public class Printer
        {
            public string paperLength { get; set; }
            public string printerPort { get; set; }
        }
        public Printer _printer = new Printer();
        public class EDC
        {
            public string edcPort { get; set; }
        }
        public EDC _edc = new EDC();
        public class BILL
        {
            public string billPort { get; set; }
            public string billBankStock { get; set; }
            public string billAmount { get; set; }
        }
        public BILL _bill = new BILL();

        public class EKTP
        {
            public string ektpPCID { get; set; }
            public string ektpCONF { get; set; }
            public string ektpUSB { get; set; }
        }
        public EKTP _ektp = new EKTP();
        //URL
        public const string PARAM_DEFAULT_URL = "param.system.default.url";
        public const string PARAM_SIGN_URL = "param.system.kiosk.sign.url";
        public const string PARAM_MONITORING_URL = "param.MyGrapari.monitoring.url";

        //Terminal
        public const string PARAM_MACHINE = "param.system.machine";
        public const string PARAM_TOKEN = "param.system.token";
        public const string PARAM_MYGRAPRI_REG_STATUS = "param.MyGrapari.reg.status";

        //MyGraPARI
        public const string PARAM_MYGRAPARI_VER = "param.system.MyGrapariUI.vers";
        public const string PARAM_DEVICE_MONITORING_VER = "param.system.MyGrapariDeviceMonitoring.vers";
        public const string PARAM_UPDATE_PATCH_INTERVAL = "param.config.updatepatch.interval";
        public const string PARAM_ADMIN_IDENTIFICATION = "param.admin.identification";
        public const string PARAM_ADMIN_RELEASE = "param.admin.release";
        public const string PARAM_MONITORING_INTERVAL = "param.monitoring.interval";
        public const string PARAM_MAX_RETRY_LOAD_DISPENSER = "param.maks.retry.load.dispenser";
        public const string PARAM_SCAN_HIST_MONTH = "param.sys.scan.hist.month";
        public const string PARAM_DELAY_USB_UP = "param.delay.usb.up";
        public const string PARAM_DELAY_USB_DOWN = "param.delay.usb.down";
        public const string PARAM_DEVICE_FINGERPRINT_TRY = "param.device.fingerprint.try";
        public const string PARAM_DELAY_CREATE_READER_OBJECT = "param.delay.create.reader.object";

        //Dispenser
        public const string PARAM_DEVICE_DISPENSER = "param.device.dispenser";
        public const string PARAM_DEVICE_CARD_READER = "param.device.card_reader";
        public const string PARAM_DEVICE_CARD_READER_USB = "param.device.card_reader.usb";
        public const string PARAM_STOCK_DISPENSER_1 = "param.tr.card.disp1.stock";
        public const string PARAM_PERSO_EXE = "param.perso.exe";

        //IP Cam
        public const string PARAM_DEVICE_CAMERA_IP_URL = "param.device.ip_camera.url";
        public const string PARAM_DEVICE_CAMERA_IP_TYPE = "param.device.ip_camera.type";
        public const string PARAM_DEVICE_CAMERA_IP_USER = "param.device.ip_camera.user";
        public const string PARAM_DEVICE_CAMERA_IP_PASSWORD = "param.device.ip_camera.password";
        
        //Relay
        public const string PARAM_RELAY_PORT = "param.relay.port";
        public const string PARAM_RELAY_ON_INTERVAL = "param.relay.on_interval";

        //Printer
        public const string PARAM_PAPER_LENGTH = "param.paper.length";
        public const string PARAM_PRINTER_PORT = "param.printer.port";

        //EDC
        public const string PARAM_EDC_PORT = "param.edc.port";

        //BILL
        public const string PARAM_BILL_PORT = "param.bill.port";
        public const string PARAM_BILL_BANK_STOK = "param.bill.bank.stok";
        public const string PARAM_BILL_AMOUNT = "param.bill.amount";
        
        // EKTP
        public const string PARAM_EKTP_PCID = "param.ektp.pcid";
        public const string PARAM_EKTP_CONF = "param.ektp.conf";
        public const string PARAM_EKTP_USB = "param.ektp.usb";
        
        private FileIniDataParser iniFile = new FileIniDataParser();
        private IniData iniData = null;

        private string fileName = "MyGraPARI-config.properties";
        public const string FILE_NAME_MyGraPARI = "MyGraPARI-config.properties";
        public const string FILE_NAME_UPDATER = "updatePatch_setting.properties";
        public Config()
        {
            fileName= Directory.GetCurrentDirectory() + @"\MyGApps\" + "MyGraPARI-config.properties";
        }

        public Config(string p_fileName)
        {
            fileName = p_fileName;
        }

        public void Init()
        {
            try
            {
                //if (iniData == null)
                {
                    iniData = iniFile.ReadFile(fileName);
                }
            }
            catch { }
        }

        public string Read(string p_section, string p_name)
        {
            string result = string.Empty;

            Init();

            try
            {
                result = iniData[p_section][p_name];
            }
            catch { }

            return result;
        }

        public void Write(string p_section, string p_name, string p_value)
        {
            Init();

            try
            {
                try
                {
                    iniData.Sections.AddSection(p_section);
                }
                catch { }

                iniData[p_section][p_name] = p_value;

                iniData.Configuration.NewLineStr = "\r\n";
                iniFile.WriteFile(fileName, iniData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
    }
}
