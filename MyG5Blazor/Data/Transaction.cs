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
        public string mypersoResponse { get; set; }
        public string persoID { get; set; }
        public string paymentMethod => _payment[intPayment];
        public int intPayment { get; set; }
        public long startTime { get; set; }
        public long endTime { get; set; }
       
        private string[] _payment = { "", "DEBIT", "CASH", "CC" };
        public string bit = string.Empty;
        public string fee = string.Empty;
        public string ecr = string.Empty;
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
        public string status = string.Empty;
        public int jumlah_kartu = 0;
        public void SetTransaction(string strTransID, string strBUID, string strTermID,string strJenisTrans)
        {
            transID = strTransID;
            buID = strBUID;
            termID = strTermID;
            jenisTrans = strJenisTrans;
        }
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
