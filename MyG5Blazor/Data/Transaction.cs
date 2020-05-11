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
        private string[] _payment = { "", "CC", "Cash", "CC" };
        public string bit = string.Empty;
        public string fee = string.Empty;
        public string ecr = string.Empty;
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
            mypersoResponse = string.Empty;
        }
    }
}
