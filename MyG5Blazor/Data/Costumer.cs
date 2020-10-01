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
        public string nameMasked { get; set; }
        public string PhoneNumber { get; set; }
        public string phoneNumberMasked => PhoneNumber.Substring(0, PhoneNumber.Length - 4).PadRight(PhoneNumber.Length, '*');
        public string EKTPNumber { get; set; }
        public string IDType { get;set; }
        
        private string[] arrayMPembayaran = { "", "Kartu Debit (Ber Pin)", "Tunai (Uang Pas)", "Kartu Kredit (Ber Pin)" };
        public string strMPembayaran => arrayMPembayaran[intMPembayaran];
        public string strTagihan => intTagihan.ToString();
        public int intMPembayaran = 0;
        public int intTagihan = 0;
        public int intTagihanTerbayar = 0;
        public int intUangCount = 0;
        public string accountId { get; set; }
        public string tagihanTerbayar => intTagihan.ToString();
        public class Tagihan
        {
            public string _tglTagihan { get; set; }
            public string _strTagihan => _intTagihan.ToString();
            public int _intTagihan { get; set; }
            public string _referenceNo { get; set; }
            public string _custName { get; set; }
        }

        public void MaskingName()
        {
            string[] strSplit = Name.Split(" ");
            foreach(string ss in strSplit)
            {
                if (ss != "")
                {
                    if (ss.Length > 2)
                    {
                        string sm = ss.Substring(0, 2).PadRight(ss.Length, '*');
                        nameMasked = nameMasked + sm + " ";
                    }
                    else if (ss.Length == 1)
                    {
                        nameMasked = nameMasked + ss + " ";
                    }
                }
            }
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
            nameMasked = string.Empty;
            PhoneNumber = string.Empty;
            EKTPNumber = string.Empty;
            IDType = string.Empty;
            intMPembayaran = 0;
            intTagihan = 0;
            intTagihanTerbayar = 0;
            intUangCount = 0;
            accountId = string.Empty;
            _listTagihan.Clear();
            Type = string.Empty;
            intMPembayaran = 0;
            intTagihan = 0;
            intTagihanTerbayar = 0;
            intUangCount = 0;
        }
        public void CostumerProfile(string _type, string _name, string _phoneNumber, string _idType, string _ektpNumber)
        {
            Type = _type;
            Name = _name;
            PhoneNumber = _phoneNumber;
            IDType = _idType;
            EKTPNumber = _ektpNumber;
            MaskingName();
        }
        public void CostumerProfile(string _type, string _name, string _phoneNumber, string _idType, string _ektpNumber,string _accId)
        {
            Type = _type;
            Name = _name;
            PhoneNumber = _phoneNumber;
            IDType = _idType;
            EKTPNumber = _ektpNumber;
            accountId = _accId;
            MaskingName();
        }
        public void ClearTagihan()
        {
            _listTagihan.Clear();
        }
    }
}
