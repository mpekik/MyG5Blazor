using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyG5Blazor.Data
{
    public class Transaction
    {
        public string jenisTrans { get; set; }
        public string transID { get; set; }
        public string buID { get; set; }
        public string termID { get; set; }
        public string errorCode { get; set; }
        public string serviceErrorCode { get; set; }
        public string mypersoResponse { get; set; }
        public string persoID { get; set; }
        public string paymentMethod => _payment[intPayment];
        public int intPayment { get; set; }
        public long startTime { get; set; }
        public long endTime { get; set; }
       public string resultPayment { get; set; }
        public class PSBDetail
        {
            public string alamatEmail { get; set; }
            public string otp { get; set; }
            public string nik { get; set; }
            public string nama { get; set; }
            public string tglLahir { get; set; }
            public string nokk { get; set; }
            public string selectedPaket { get; set; }
            public Menu.MenuPSB PaketPSB = new Menu.MenuPSB();
            public string patternNumber { get; set; }
            public string selectedNumber { get; set; }
            public string batasPemakaian { get; set; }
            public bool isTC { get; set; } = false;
            public bool isEbill { get; set; } = false;
        }

        public PSBDetail psbDetail = new PSBDetail();

        private string[] _payment = { "", "debit", "CASH", "cc" };
        public string bit = string.Empty;
        public string fee = string.Empty;
        public string ecr = string.Empty;
        public string ecrVoid = string.Empty;
        public string edcTrace = string.Empty;
        public string edcApproval = string.Empty;
        public string edcresp = string.Empty;
        public string edcTid = string.Empty;
        public string edcMid = string.Empty;
        public string edcBatch = string.Empty;
        public string edcIssuer = string.Empty;
        public string edcRefNo = string.Empty;
        public string edcCardNo = string.Empty;
        public string edcCardName = string.Empty;
        public string edcDate = string.Empty;
        public string edcTime = string.Empty;
        public string edcStatus = "FAILED";
        public string edcEntryCode = string.Empty;
        public string edcAmount = string.Empty;
        public string edcTotalAmount = string.Empty;
        public string edcRespVoid = string.Empty;
        public string edcRefNumber = string.Empty;
        public string edcBillingNumber = string.Empty;
        public string edcBalance = string.Empty;
        public string edcTopUp = string.Empty;
        public string edcExp = string.Empty;
        public string edcInvoiceLA = string.Empty;
        public string edcBankFiller = string.Empty;
        public string status = string.Empty;
        public string kip = "-";
        public int jumlah_kartu = 0;
        public void SetTransaction(string strTransID)
        {
            transID = strTransID;
        }

        public void SetTransaction(string strBUID, string strTermID, string strJenisTrans)
        {
            buID = strBUID;
            termID = strTermID;
            jenisTrans = strJenisTrans;
        }

        public void ClearTransaction()
        {
            transID = string.Empty;
            buID = string.Empty;
            termID = string.Empty;
            jenisTrans = string.Empty;
            mypersoResponse = string.Empty;
            errorCode = string.Empty;
            mypersoResponse = string.Empty;
            persoID = string.Empty;
            bit = string.Empty;
            fee = string.Empty;
            ecr = string.Empty;
            edcTrace = string.Empty;
            edcApproval = string.Empty;
            edcresp = string.Empty;
            edcTid = string.Empty;
            edcMid = string.Empty;
            edcBatch = string.Empty;
            edcIssuer = string.Empty;
            edcRefNo = string.Empty;
            edcCardNo = string.Empty;
            edcCardName = string.Empty;
            edcDate = string.Empty;
            edcTime = string.Empty;
            edcStatus = "FAILED";
            status = string.Empty;
            jumlah_kartu = 0;
            kip = "-";
            _auditTrail.Clear();
        }

        public class AuditTrail
        {
            public string _action { get; set; }
            public string _data { get; set; }
            public string _result { get; set; }
        }

        public List<AuditTrail> _auditTrail = new List<AuditTrail>();

        public void AddTrail(string strAction, string strData, string strResult)
        {
            AuditTrail at = new AuditTrail();
            at._action = strAction;
            at._data = strData;
            at._result = strResult;
            _auditTrail.Add(at);
        }
    }
}
