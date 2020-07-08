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

        public string pp = string.Empty;
        public string th = string.Empty;
        public string gk = string.Empty;
        public string u4g = string.Empty;
        public string psb = string.Empty;

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
