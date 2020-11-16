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
        public const string PARAM_DEFAULT_URL = "param.system.default.url";
        public const string PARAM_SIGN_URL = "param.system.kiosk.sign.url";
        public const string PARAM_MONITORING_URL = "param.MyGrapari.monitoring.url";

        public const string PARAM_MACHINE = "param.system.machine";
        // Menu
        public const string PARAM_TOKEN = "param.system.token";

        public const string PARAM_MYGRAPARI_VER = "param.system.MyGrapariUI.vers";
        public const string PARAM_DEVICE_MONITORING_VER = "param.system.MyGrapariDeviceMonitoring.vers";
        public const string PARAM_UPDATE_PATCH_INTERVAL = "param.config.updatepatch.interval";
        public const string PARAM_MYGRAPRI_REG_STATUS = "param.MyGrapari.reg.status";

        public const string PARAM_ADMIN_IDENTIFICATION = "param.admin.indetification";
        public const string PARAM_ADMIN_RELEASE = "param.admin.release";

        public const string PARAM_MONITORING_INTERVAL = "param.monitoring.interval";
        public const string PARAM_MAX_RETRY_LOAD_DISPENSER = "param.maks.retry.load.dispenser";

        public const string PARAM_LANGUAGE = "system.param.language";
        public const string PARAM_ERR_MSG1_ID = "system.param.err.msg1.id";
        public const string PARAM_ERR_MSG1_EN = "system.param.err.msg1.en";
        public const string PARAM_ERR_MSG2_ID = "system.param.err.msg2.id";
        public const string PARAM_ERR_MSG2_EN = "system.param.err.msg2.en";

        public const string FILE_NAME_MyGraPARI = "MyGraPARI-config.properties";
        public const string FILE_NAME_UPDATER = "updatePatch_setting.properties";

        public const string PARAM_DEVICE_DISPENSER = "param.device.dispenser";
        public const string PARAM_DEVICE_CARD_READER = "param.device.card_reader";
        public const string PARAM_DEVICE_CARD_READER_USB = "param.device.card_reader.usb";
        public const string PARAM_DEVICE_CAMERA_IP_URL = "param.device.ip_camera.url";
        public const string PARAM_DEVICE_CAMERA_IP_TYPE = "param.device.ip_camera.type";
        public const string PARAM_DEVICE_CAMERA_IP_USER = "param.device.ip_camera.user";
        public const string PARAM_DEVICE_CAMERA_IP_PASSWORD = "param.device.ip_camera.password";

        public const string PARAM_STOCK_DISPENSER_1 = "param.tr.card.disp1.stock";

        public const string PARAM_PERSO_EXE = "param.perso.exe";

        public const string PARAM_PAPER_LENGTH = "param.paper.length";
        public const string PARAM_PRINTER_PORT = "param.printer.port";

        public const string PARAM_RELAY_PORT = "param.relay.port";
        public const string PARAM_RELAY_ON_INTERVAL = "param.relay.on_interval";

        public const string PARAM_DELAY_USB_UP = "param.delay.usb.up";
        public const string PARAM_DELAY_USB_DOWN = "param.delay.usb.down";

        public const string PARAM_DEVICE_FINGERPRINT_TRY = "param.device.fingerprint.try";

        public const string PARAM_DELAY_CREATE_READER_OBJECT = "param.delay.create.reader.object";
        public const string PARAM_URL_DELETE_KAFKA = "param.url.delete.kafka";

        public const string PARAM_EDC_PORT = "param.edc.port";

        public const string PARAM_UPDATE_TERMINAL_ID_KAFKA = "update.terminal_kafka";
        public const string PARAM_UPDATE_TERMINAL_ID_LOCAL = "update.terminal_local";
        public const string PARAM_UPDATE_TOKEN = "update.token";
        public const string PARAM_UPDATE_VERSION = "update.version";
        public const string PARAM_UPDATE_LAST_VERSION = "update.last_version";
        public const string PARAM_UPDATE_SERVICE_CHECK = "update.check_service";
        public const string PARAM_UPDATE_DOWNLOAD = "update.download";
        public const string PARAM_UPDATE_DESTINATION = "update.destination";
        // bank
        public const string PARAM_BILL_PORT = "param.bill.port";
        public const string PARAM_BILL_BANK_STOK = "param.bill.bank.stok";
        public const string PARAM_BILL_AMOUNT = "param.bill.amount";
        // nv200
        public const string PARAM_NV200_PORT = "param.nv200.port";
        public const string PARAM_NV200_BANK_STOK = "param.nv200.bank.stok";
        public const string PARAM_NV200_AMOUNT = "param.nv200.amount";
        // EKTP
        public const string PARAM_EKTP_PCID = "param.ektp.pcid";
        public const string PARAM_EKTP_CONF = "param.ektp.conf";
        public const string PARAM_EKTP_USB = "param.ektp.usb";

        //Util
        public const string PARAM_CASH_COLLECTED = "cash.collected";

        private FileIniDataParser iniFile = new FileIniDataParser();
        private FileIniDataParser iniFileIP = new FileIniDataParser();
        private FileIniDataParser iniFileUtil = new FileIniDataParser();
        private IniData iniData = null;
        private IniData iniDataIP = null;
        private IniData iniDataUtil = null;

        private string fileName = "MyGraPARI-config.properties";
        private string fileNameIP = "IPTest-config.properties";
        private string fileNameUtil = "Util-config.properties";

        public string vers = string.Empty;
        public Config()
        {
            fileName= Directory.GetCurrentDirectory() + @"\MyGApps\" + "MyGraPARI-config.properties";
            fileNameUtil = Directory.GetCurrentDirectory() + @"\MyGApps\" + "Util.properties";
            fileNameIP = Directory.GetCurrentDirectory() + @"\" + "IPTest-config.properties";
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
        public void InitIP()
        {
            try
            {
                //if (iniData == null)
                {
                    iniDataIP = iniFileIP.ReadFile(fileNameIP);
                }
            }
            catch { }
        }
        public void InitUtil()
        {
            try
            {
                //if (iniData == null)
                {
                    iniDataUtil = iniFileUtil.ReadFile(fileNameUtil);
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
            catch {
                result = null;
            }

            return result;
        }
        public string ReadIP(string p_section, string p_name)
        {
            string result = string.Empty;

            InitIP();

            try
            {
                result = iniDataIP[p_section][p_name];
            }
            catch { }

            return result;
        }
        public string ReadUtil(string p_section, string p_name)
        {
            string result = string.Empty;

            InitUtil();

            try
            {
                result = iniDataUtil[p_section][p_name];
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
                //Console.WriteLine(ex.Message);
            }
        }
        public void WriteUtil(string p_section, string p_name, string p_value)
        {
            InitUtil();

            try
            {
                try
                {
                    iniDataUtil.Sections.AddSection(p_section);
                }
                catch { }

                iniDataUtil[p_section][p_name] = p_value;

                iniDataUtil.Configuration.NewLineStr = "\r\n";
                iniFileUtil.WriteFile(fileNameUtil, iniDataUtil);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }
        public string terminalId = string.Empty;
        public string terminalDesc = string.Empty;
        public string terminalLocation = string.Empty;
        public string areaCode = string.Empty;
        public string regionalCode = string.Empty;
        public string unitId = string.Empty;
        public string unitName = string.Empty;

        public string menuConfigId = string.Empty;
        public string maxRetryPerso = string.Empty;
        public string tidEdc = string.Empty;
        public string midEdc = string.Empty;
        public string snEdc = string.Empty;
        public string apiBuid = string.Empty;
        public string apiUid = string.Empty;
        public string cardDispenser = string.Empty;
        public string bahasa = string.Empty;
        public string edc = string.Empty;
        public string cash = string.Empty;

        public string href = string.Empty;
        public string asssetPNG = string.Empty;
        public string eventType = string.Empty;
        public string wPNG = string.Empty;
        public string hPNG = string.Empty;
        public bool active = false;

        public string usbEKTP = string.Empty;
        public string usbCardReader = string.Empty;
        public string ektpType = string.Empty;
        public void Terminal(string _termID, string _termDesc, string _termLoc, string _areaCode, string _regionCode, string _unitId, string _unitName)
        {
            terminalId = _termID;
            terminalDesc = _termDesc;
            terminalLocation = _termLoc;
            areaCode = _areaCode;
            regionalCode = _regionCode;
            unitId = _unitId;
            unitName = _unitName;
        }

        public void ConfigIn(string _menuConfId, string _maxRetry, string _tidEdc, string _midEdc, string _snEdc, string _apiBuid, string _apiUid, string _cardDispenser, string _bahasa, string _edc, string _cash)
        {
            menuConfigId = _menuConfId;
            maxRetryPerso = _maxRetry;
            tidEdc = _tidEdc;
            midEdc = _midEdc;
            snEdc = _snEdc;
            apiBuid = _apiBuid;
            apiUid = _apiUid;
            cardDispenser = _cardDispenser;
            bahasa = _bahasa;
            edc = _edc;
            cash = _cash;
        }
    }
}
