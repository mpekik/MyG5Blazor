using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyG5Blazor.Data
{
    public class PrintContent
    {
        public string GK(Transaction trans,Costumer cst,Config config)
        {
            DateTime datetimenow = DateTime.Now;

            string contents = "----------------------------------------\n"+
                "TRANSACTION ID     : "+trans.transID+"\n"+
                "TERMINAL ID        : "+trans.termID+"\n"+
                "TANGGAL WAKTU      : "+ datetimenow.ToString("dd-MM-yyyy") + " " + datetimenow.ToString("HH:mm:ss") + "\n" +
                "NOMOR PONSEL       : "+cst.PhoneNumber+"\n"+
                "\n"+
                "ANDA TELAH BERHASIL GANTI KARTU HILANG/RUSAK\n"+
                "----------------------------------------\n";

            return contents;
        }
        public string BTH(Transaction trans, Costumer cst, Config config)
        {
            DateTime datetimenow = DateTime.Now;

            string contents = "BAYAR TAGIHAN HALO\n" +
                "DATE : " + datetimenow.ToString("dd-MM-yyyy") + "\tTIME : " + datetimenow.ToString("HH:mm:ss") + "\n" +
                "MACHINE ID : " + trans.termID + "\n" +
                "TRANSACTION ID : " + trans.transID + "\n" +
                "MSISDN : " + cst.PhoneNumber + "\n" +
                "NAME : " + cst.Name + "\n" +
                "\n";
            if (cst.intMPembayaran != 2)
                contents = contents + "TID : " + trans.edcTid + "\tMID : " + trans.edcMid + "\n" +
                "BATCH : " + trans.edcBatch + "\tTRACE No : " + trans.edcTrace + "\n" +
                "EDC REF NO : " + trans.edcRefNo + "\tAPPR CODE : " + trans.edcApproval + "\n" +
                "\nRINCIAN TAGIHAN : \n";
            foreach (dynamic t in cst.GetlistTagihan)
            {
                contents = contents +
                    "REFERENCE CODE : " + t._referenceNo + "\n" +
                    "AMOUNT : " + t._intTagihan.ToString("N0") + "\n";
            }

            contents = contents +
                "\n" +
                "TOTAL TAGIHAN : Rp " + cst.intTagihan.ToString("N0") + "\n" +
                "METODE PEMBAYARAN : " + cst.strMPembayaran + "\n" +
                "TOTAL PEMBAYARAN : Rp " + cst.intTagihanTerbayar.ToString("N0");

            return contents;
        }
        public string FOOTER(Transaction trans, Costumer cst, Config config)
        {
            string contents = "";
            if(cst.intMPembayaran!=2)
            {
                if(trans.edcStatus=="SUCCESS")
                {
                    contents = contents + "*** PIN VERIFICATION SUCCESS ***\n" +
                        "I AGREE TO PAY ABOVE TOTAL AMOUNT\n" +
                        "ACCORDING TO CARD ISSUER AGREEMENT\n" +
                        "\n***PEMBAYARAN BERHASIL DILAKUKAN***\n" +
                        "TERIMA KASIH TELAH MENGGUNAKAN\nLAYANAN JASA MYGRAPARI TELKOMSEL\n";
                }else if(trans.edcStatus=="FAILED")
                {
                    contents = contents + "***PEMBAYARAN GAGAL DILAKUKAN***\n" +
                        "PENGEMBALIAN DANA MELALUI MESIN EDC\nJIKA ANDA BELUM MELAKUKANNYA\n"+
                        "SILAHKAN MENGHUBUNGI COSTUMER SERVICE\nDENGAN MEMBAWA BUKTI STRUK INI\n"+
                        "TERIMA KASIH TELAH MENGGUNAKAN\nLAYANAN JASA MYGRAPARI TELKOMSEL\n";
                }
            }else
            {
                if (trans.edcStatus == "SUCCESS")
                {
                    contents = contents + "***PEMBAYARAN BERHASIL DILAKUKAN***\n" +
                        "UANG KEMBALI AKAN DIMASUKKAN KEDALAM\n"+
                        "DEPOSIT PEMBAYARAN UNTUK TAGIHAN BERIKUTNYA\n" +
                        "TERIMA KASIH TELAH MENGGUNAKAN\nLAYANAN JASA MYGRAPARI TELKOMSEL\n";
                }
                else if (trans.edcStatus == "FAILED")
                {
                    contents = contents + "***PEMBAYARAN GAGAL DILAKUKAN***\n" +
                        "SILAHKAN MENGHUBUNGI COSTUMER SERVICE\nDENGAN MEMBAWA BUKTI STRUK INI\n" +
                        "UNTUK MELAKUKAN PENGEMBALIAN DANA\nYANG SUDAH DIMASUKKAN\n" +
                        "TERIMA KASIH TELAH MENGGUNAKAN\nLAYANAN JASA MYGRAPARI TELKOMSEL\n";
                }
            }

            contents = contents + "\nEMAIL : cs@telkomsel.co.id\n" +
                "CONTACT CENTER : call 188\n" +
                "FACEBOOK : facebook.com/Telkomsel\n" +
                "TWITTER : @telkomsel";

            return contents;
        }
        public string Header(Transaction trans, Costumer cst, Config config)
        {
            string contents = config.terminalDesc + "\n" +
                config.terminalLocation;

            return contents;
        }
        public string HeaderGK(Transaction trans, Costumer cst, Config config)
        {
            string contents = "GANTI KARTU HILANG/RUSAK";
                
            return contents;
        }
        public string FooterGK(Transaction trans, Costumer cst, Config config)
        {
            string contents = "TERIMA KASIH TELAH MENGGUNAKAN\nLAYANAN JASA MYGRAPARI TELKOMSEL";
            
            return contents;
        }
    }
}
