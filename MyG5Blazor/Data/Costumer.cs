using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyG5Blazor.Data
{
    public class Costumer
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EKTPNumber { get; set; }
        public string IDType { get;set; }
        
        private string[] arrayMPembayaran = { "", "Kartu Debit (Ber Pin)", "Tunai (Uang Pas)", "Kartu Kredit (Ber Pin)" };
        public string strMPembayaran => arrayMPembayaran[intMPembayaran];
        public string strTagihan => intTagihan.ToString();
        public int intMPembayaran = 0;
        public int intTagihan = 0;
        public int intTagihanTerbayar = 0;
        public int intUangCount = 0;
        public string tagihanTerbayar => intTagihan.ToString();
        public class Tagihan
        {
            public string _tglTagihan { get; set; }
            public string _strTagihan => _intTagihan.ToString();
            public int _intTagihan { get; set; }
            public string _referenceNo { get; set; }
            public string _custName { get; set; }
        }

            

        private List<Tagihan> _listTagihan = new List<Tagihan>();

        public void AddlistTagihan(string strDate, int intVal, string strCustName)
        {
            Tagihan tagihan = new Tagihan();
            tagihan._tglTagihan = strDate;
            tagihan._intTagihan = intVal;
            //tagihan._referenceNo = strRefNo;
            tagihan._custName = strCustName;
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
            intTagihanTerbayar = 0;
            intUangCount = 0;
            _listTagihan.Clear();
        }
        public void CostumerProfile(string strType, string strName, string strPhoneNumber, string strIDType, string strEKTPNumber)
        {
            Type = strType;
            Name = strName;
            PhoneNumber = strPhoneNumber;
            IDType = strIDType;
            EKTPNumber = strEKTPNumber;
        }
        public void ClearTagihan()
        {
            _listTagihan.Clear();
        }
    }
}
