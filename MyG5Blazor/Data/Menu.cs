using MyG5Blazor.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyG5Blazor.Data
{
    public class Menu
    {
        public string terminalId = string.Empty;
        public string terminalDesc = string.Empty;
        public string terminalLocation = string.Empty;
        public string areaCode = string.Empty;
        public string regionalCode = string.Empty;
        public string unitId = string.Empty;
        public string unitName = string.Empty;
        public string userName = string.Empty;
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
        public string tokenId = string.Empty;
        public string href = string.Empty;
        public string asssetPNG = string.Empty;
        public string eventType = string.Empty;
        public string wPNG = string.Empty;
        public string hPNG = string.Empty;
        public bool active = false;

        public string[] AssetsPNG = { "Menu_Bayar_Tagihan.png", "Menu_Upgrade4G.png", "Menu_Beli_Pulsa.png", "Menu_Ganti_kartu.png" };
        public string[] apiURL = { "bayar-halo/v1/init", "upgrade-4g/v1/init", "isi-pulsa/v1/init", "ganti-kartu/v1/init" };
        public string[] stringMenu = { "1", "1", "1", "1", "1" };

        public class MenuPulsa
        {
            public string _nominal { get; set; }
            public string _labelNominal { get; set; }
            public string _icon { get; set; }
            public string _addHTML { get; set; }
        }

        public List<MenuPulsa> _menuPulsa = new List<MenuPulsa>();

        public void AddMenuPulsa (string nominal,string labelNominal, string icon, string addHTML)
        {
            MenuPulsa menuPulsa = new MenuPulsa();
            menuPulsa._nominal = nominal;
            menuPulsa._labelNominal = labelNominal;
            menuPulsa._icon = icon;
            menuPulsa._addHTML = addHTML;
            _menuPulsa.Add(menuPulsa);
        }
        public class MainMenu
        {
            public string _namaMenu { get; set; }
            public string _url { get; set; }
            public string _icon { get; set; }
            public string _code { get; set; }
            public string _href { get; set; }
            public string _style { get; set; }
        }

        public List<MainMenu> _mainMenu = new List<MainMenu>();

        public void AddMainMenu (string namamenu, string url, string icon,string code,string style)
        {
            MainMenu mainmenu = new MainMenu();
            mainmenu._namaMenu = namamenu;
            mainmenu._url = url;
            mainmenu._icon = icon;
            mainmenu._code = code;
            mainmenu._href = code+"/1";
            mainmenu._style = style;
            _mainMenu.Add(mainmenu);
        }

        public void Terminal(string _termID, string _termDesc, string _termLoc, string _areaCode, string _regionCode,string _unitId, string _unitName)
        {
            terminalId = _termID;
            terminalDesc = _termDesc;
            terminalLocation = _termLoc;
            areaCode = _areaCode;
            regionalCode = _regionCode;
            unitId = _unitId;
            unitName = _unitName;
        }

        public void Config(string _menuConfId, string _maxRetry, string _tidEdc, string _midEdc, string _snEdc, string _apiBuid, string _apiUid,string _cardDispenser,string _bahasa,string _edc, string _cash)
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
