﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyG5Blazor.Data
{
    public class Costumer
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EKTPNumber { get; set; }
        private string[] arrayMPembayaran = { "", "Kartu Debit (Ber Pin)", "Tunai (Uang Pas)", "Kartu Kredit (Ber Pin)" };
        public string strMPembayaran => arrayMPembayaran[intMPembayaran];
        public string strTagihan => (intTagihan / 1000).ToString() + ".000";
        public int intMPembayaran = 0;
        public int intTagihan = 0;
        public class Tagihan
        {
            public string _tglTagihan { get; set; }
            public string _strTagihan => (_intTagihan / 1000).ToString() + ".000";
            public int _intTagihan { get; set; }
        }

            public string transactionID { get; set; }
            public string buID { get; set; }
            public string terminalID { get; set; }

        public void SetTransaction(string strTransID, string strBUID, string strTermID)
        {
            transactionID = strTransID;
            buID = strBUID;
            terminalID = strTermID;
        }

        private List<Tagihan> _listTagihan = new List<Tagihan>();

        public void AddlistTagihan(string strDate, int intVal)
        {
            Tagihan tagihan = new Tagihan();
            tagihan._tglTagihan = strDate;
            tagihan._intTagihan = intVal;
            _listTagihan.Add(tagihan);
        }

        public IReadOnlyList<Tagihan> GetlistTagihan
        {
            get
                {
                return _listTagihan.AsReadOnly();
            }
        }
        
        public void Clear()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            EKTPNumber = string.Empty;
            intMPembayaran = 0;
            intTagihan = 0;
            _listTagihan.Clear();
        }

        public void ClearTagihan()
        {
            _listTagihan.Clear();
        }
    }
}