using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyG5Blazor.Data
{
    public class Transaction
    {
        public string transID { get; set; }
        public string buID { get; set; }
        public string termID { get; set; }
        public string errorCode { get; set; }

        public void SetTransaction(string strTransID, string strBUID, string strTermID)
        {
            transID = strTransID;
            buID = strBUID;
            termID = strTermID;
        }

        public void ClearTransaction()
        {
            transID = string.Empty;
            buID = string.Empty;
            termID = string.Empty;
        }
    }
}
