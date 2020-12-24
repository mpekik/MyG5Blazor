using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using Aratek.TrustFinger;

namespace MyG5Blazor.Data
{
    public class CRT_Interface
    {
        private class EKTPDLLHandler
        {
            [DllImport("CRT_603_CZ1.dll")]
            public static extern int InitializeContext(ref UInt16 ReaderCount);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int GetSCardReaderName(UInt16 ReaderSort, ref UInt16 RxDataLen, byte[] RxData);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int GetStatusChange(UInt16 ReaderSort);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int ConnectSCardReader(UInt16 ReaderSort);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int GetSCardReaderStatus(UInt16 ReaderSort, byte[] ReaderName, ref UInt16 ReaderNameLen, ref byte CardState, ref byte CardProtocol, byte[] ATR_Data, ref UInt16 ATR_DataLen);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int APDU_Transmit(UInt16 ReaderSort, UInt16 TxDataLen, byte[] TxData, ref UInt16 RxDataLen, byte[] RxData);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int GetSCardNuber(UInt16 ReaderSort, ref UInt16 RxDataLen, byte[] RxData);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int Extended_Transmit(UInt16 ReaderSort, UInt16 TxDataLen, byte[] TxData, ref UInt16 RxDataLen, byte[] RxData);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int DisconnectSCardReader(UInt16 ReaderSort);

            [DllImport("CRT_603_CZ1.dll")]
            public static extern int ReleaseContext();
        }
        public class EKTPModel
        {
            public string strBio { get; set; } = "";
            public byte[] bytePhoto { get; set; } = new byte[65536];
            public int photolen { get; set; } = 0;
            public byte[] byteBio { get; set; } = new byte[65536];
            public int biolen { get; set; } = 0;
            public byte[] byteSignature { get; set; } = new byte[65536];
            public int signlen { get; set; } = 0;
            public byte[] byteMinu1 { get; set; } = new byte[65536];
            public int minu1len { get; set; } = 0;
            public byte[] byteMinu2 { get; set; } = new byte[65536];
            public int minu2len { get; set; } = 0;
            public List<byte[]> lbPhoto = new List<byte[]>();
            public byte[] UID { get; set; } = new byte[8];
            public byte[] CC { get; set; } = { };
            public UInt16 CC_LEN { get; set; } = 0;
            public byte[] GETCHAL { get; set; } = { };
            public UInt16 GETCHAL_LEN { get; set; } = 0;
            public byte[] CALCHAL { get; set; } = { };
            public UInt16 CALCHAL_LEN { get; set; } = 0;
            public byte[] EXTAUTH { get; set; } = { };
            public UInt16 EXTAUTH_LEN { get; set; } = 0;
            public byte[] INTAUTH { get; set; } = { };
            public UInt16 INTAUTH_LEN { get; set; } = 0;
            public byte[] RETDS { get; set; } = { };
            public UInt16 RETDS_LEN { get; set; } = 0;
            public byte[] FPR { get; set; } = { };
            //public Bitmap bmpPhoto

            public String[] SplitBio(string strBio)
            {
                String[] strSplit;
                String[] strSeparator = new String[] { "\",\"" };
                strSplit = strBio.Split(strSeparator, StringSplitOptions.None);

                strSplit[0] = strSplit[0].Remove(0, 1);
                strSplit[20] = strSplit[20].Remove(strSplit[20].Length - 1, 1);
                return strSplit;
            }
            public Bitmap Signature
            {
                get
                {
                    try
                    {
                        int height = 44;
                        int width = 168;
                        byte[] signatureData = byteSignature;

                        Bitmap tmpBMP = new Bitmap(width, height, PixelFormat.Format1bppIndexed);

                        BitmapData bmpDat1 = tmpBMP.LockBits(new Rectangle(0, 0, tmpBMP.Width, tmpBMP.Height),
                                                             ImageLockMode.ReadWrite, tmpBMP.PixelFormat);

                        for (int i = 0; i < tmpBMP.Height; i++)
                            Marshal.Copy(signatureData, i * tmpBMP.Width / 8,
                                        (IntPtr)((long)bmpDat1.Scan0 + bmpDat1.Stride * i),
                                        tmpBMP.Width / 8);

                        tmpBMP.UnlockBits(bmpDat1);
                        return tmpBMP;
                    }
                    catch (Exception)
                    {
                    }

                    return null;
                }
            }

            public void Clear()
            {
                Array.Clear(bytePhoto, 0, bytePhoto.Length);
                Array.Clear(byteBio, 0, byteBio.Length);
                Array.Clear(byteSignature, 0, byteSignature.Length);
                Array.Clear(byteMinu1, 0, byteMinu1.Length);
                Array.Clear(byteMinu2, 0, byteMinu2.Length);
                lbPhoto.Clear();
                Array.Clear(UID, 0, UID.Length);
                photolen = 0;
                biolen = 0;
                signlen = 0;
                minu1len = 0;
                minu2len = 0;
                CC = null;
                CC_LEN = 0;
                GETCHAL = null;
                GETCHAL_LEN = 0;
                CALCHAL = null;
                CALCHAL_LEN = 0;
                EXTAUTH = null;
                EXTAUTH_LEN = 0;
                INTAUTH = null;
                INTAUTH_LEN = 0;
                RETDS = null;
                RETDS_LEN = 0;
            }
        }
        public EKTPModel ektpData = new EKTPModel();
        private TrustFingerDevice dev = null;
        public class DeviceItem
        {
            public string Name { get; set; }
            public int DeviceIndex { get; set; }
        }
        private class ConfigCommand
        {
            public const int param_READER_RF = 0;
            public const int param_READER_SAM = 1;
            public const int param_DATA_LOOP = 248;
            public const int param_DATA_LOOP2 = 208;
            public static int param_CALC_LEN = 0;
            public static int param_EXT_AUTH_LEN = 0;
            public static int param_INT_AUTH_LEN = 0;
            public static int param_DECRYPT_LEN = 0;
            public static int param_RETRIEVE_DS_LEN = 0;

            public static string param_EXT_AUTH = string.Empty;
            public static string param_CALC_CHALLENGE = string.Empty;
            public static string param_UID = string.Empty;
            public static string param_INT_AUTH = string.Empty;
            public static string param_RETRIEVE_DS = string.Empty;

            public static readonly byte[] OPENSAM = { 0x00, 0xF0, 0x00, 0x00, 0x20 };
            public static readonly byte[] UIDA = { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
            public static readonly byte[] DFEKTP = { 0x00, 0xA4, 0x00, 0x00, 0x02, 0x7F, 0x0A };
            public static readonly byte[] EFPHOTO = { 0x00, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0xF2 };
            public static readonly byte[] READRF = { 0x00, 0xB0 };
            public static readonly byte[] UIDB = { 0x00, 0xDD, 0x00, 0x00, 0x08 };
            public static readonly byte[] EFCC = { 0x00, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0xF0 };
            public static readonly byte[] CC = { 0x00, 0xB0, 0x00, 0x00, 0x35 };
            public static readonly byte[] SAMRESET = { 0x00, 0xFF, 0x00, 0x00, 0x00 };
            public static readonly byte[] GETCHAL = { 0x00, 0x84, 0x00, 0x00, 0x08 };
            public static readonly byte[] CALCHAL33 = { 0x00, 0xF1, 0x00, 0x01 };
            public static readonly byte[] CALCHAL1D = { 0x00, 0xF1, 0x00, 0x00 };
            public static readonly byte[] EXTAUTH = { 0x00, 0x82, 0x00, 0x00, 0x28 };
            public static readonly byte[] INTAUTH = { 0x00, 0xF2, 0x00, 0x00 };
            public static readonly byte[] ENCRYPT = { 0x00, 0xF3, 0x00, 0x00 };
            public static readonly byte[] EFDS = { 0x00, 0xF3, 0x00, 0x00, 0x07, 0x00, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0xF6 };
            public static readonly byte[] DECRYPT = { 0x00, 0xF4, 0x00, 0x00 };
            public static readonly byte[] READDS = { 0x00, 0xF3, 0x00, 0x00, 0x05, 0x00, 0xB0, 0x00, 0x00, 0x50 };
            public static readonly byte[] AUTOVERIF = { 0x00, 0xFA, 0x00, 0x00, 0x00 };
            public static readonly byte[] RETDS = { 0x00, 0xFA, 0x06, 0x00 };
            public static readonly byte[] EFBIO = { 0x00, 0xF3, 0x00, 0x00, 0x07, 0x00, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0xF1 };
            public static readonly byte[] SAMREAD = { 0x05, 0x00, 0xB0 };
            public static readonly byte[] RETPHOTO = { 0x00, 0xFA, 0x03, 0x00 };
            public static readonly byte[] AUTODECIP = { 0x00, 0xFA, 0x05, 0x00, 0x00 };
            public static readonly byte[] SIGNATURE = { 0x00, 0xF3, 0x00, 0x00, 0x07, 0x00, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0xF3 };
            public static readonly byte[] MINUTIAE1 = { 0x00, 0xF3, 0x00, 0x00, 0x07, 0x00, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0xF4 };
            public static readonly byte[] MINUTIAE2 = { 0x00, 0xF3, 0x00, 0x00, 0x07, 0x00, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0xF5 };
            public static readonly byte[] STOPAUTOVERIF = { 0x00, 0xFA, 0x02, 0x00, 0x00 };
            public static readonly byte[] VERIFYDF = { 0x00, 0xFA, 0x04, 0x00, 0x00 };
            public static readonly byte[] ENCCC = { 0x00, 0xF3, 0x00, 0x00, 0x07, 0x00, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0xF0 };
            public static readonly byte[] ACTIVATION = { 0x00, 0xF8, 0x00, 0x00, 0x00 };
            public static readonly byte[] UIDSAM = { 0x00, 0xF7, 0x00, 0x00, 0x00 };
            public static readonly byte[] LOGACTIVATION = { 0x00, 0xFD, 0x00, 0x00, 0x20, 0x7B, 0xDA, 0x03, 0xC5, 0x78, 0x99, 0xD0, 0x26, 0x7A, 0xAE, 0x80, 0xDB, 0x96, 0x90, 0x00, 0xB3, 0x02, 0xB6, 0x7D, 0x2C, 0xA0, 0x14, 0x47, 0x8C, 0xED, 0x6E, 0xF3, 0x3C, 0x49, 0xD3, 0x14, 0x29 };
        }
        private string _pcidSAM = "";
        private string _confSAM = "";
        private string pGuid;
        private string samSlot1 = "01";
        private string samSlot2 = "02";

        private int devCount = 0;
        private UInt16 _readerRF = 0;
        private UInt16 _readerSAM = 1;

        private bool isOpen = false;
        public bool isCapture = false;
        private bool capturing = true;
        private bool LFDFlag = false;
        public bool isCaptured = false;
        private static string strIns(string strData, string strInsert)
        {
            int i = 0;
            string strRet = strData;
            try
            {
                for (i = strRet.Length - 2; i > 0; i -= 2)
                {
                    strRet = strRet.Insert(i, strInsert);
                }
            }
            catch (Exception ex)
            {
                strRet = ex.ToString();
            }
            return strRet;
        }
        private static byte[] NEW_XOR(string strpcid, string strconf)
        {
            byte[] bytepcid = new byte[16];
            bytepcid = StringToByteArray(strpcid);
            byte[] byteconf = new byte[32];
            byteconf = StringToByteArray(strconf);
            byte[] apdu = new byte[32];

            for (int i = 0; i < 16; i++)
            {
                apdu[i] = (byte)(bytepcid[i] ^ byteconf[i]);
            }

            for (int i = 0; i < 16; i++)
            {
                apdu[16 + i] = (byte)(bytepcid[i] ^ byteconf[16 + i]);
            }

            return apdu;
        }
        private static byte[] StringToByteArray(string p_str)
        {
            byte[] result = null;

            try
            {
                p_str = p_str.Replace(Environment.NewLine, "");
                p_str = p_str.Replace("0x", "");
                p_str = p_str.Replace(" ", "");

                string[] x_bytes = p_str.Split(',');

                result = new byte[x_bytes.Length];
                int i = 0;
                foreach (string x_byte in x_bytes)
                {
                    result[i] = byte.Parse(x_byte, System.Globalization.NumberStyles.HexNumber);

                    i++;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }
        private static string ByteArrayToString(byte[] p_bytes, int pbytes)
        {
            string result = string.Empty;

            try
            {
                for (int i = 0; i < pbytes; i++)
                {
                    result = result + p_bytes[i].ToString("X02");
                }
            }
            catch (Exception ex)
            {
                result = "Error : " + ex.ToString();
            }

            return result;
        }
        public void SetSAMParameter(string pcid, string conf)
        {
            _pcidSAM = pcid;
            _confSAM = conf;
        }
        public void SetReaderParameter(int rf, int sam)
        {
            _readerRF = (UInt16)rf;
            _readerSAM = (UInt16)sam;
        }
        public bool InitializeContext()
        {
            try
            {
                int rc = -1;
                UInt16 n = 0;

                rc = EKTPDLLHandler.InitializeContext(ref n);

                if (rc == 0 && n > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void ListReaders()
        {
            int indx, rc = -1;
            string rName = "";
            try
            {
                for (int n = 0; n < 5; n++)
                {
                    rName = "";
                    byte[] ReadersName = new byte[255];
                    UInt16 ReadersNameLen = 0;
                    rc = EKTPDLLHandler.GetSCardReaderName((UInt16)n, ref ReadersNameLen, ReadersName);
                    if (rc == 0)
                    {
                        rName = "";
                        indx = 0;
                        while (ReadersName[indx] != 0)
                        {
                            rName = rName + (char)ReadersName[indx];
                            indx = indx + 1;
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public bool SAMReaderSlot(string strData5)
        {
            int rc;
            bool SAMStatus = false;
            UInt16 TxDataLen, RxDataLen;
            byte[] TxData = new byte[1024];
            byte[] RxData = new byte[1024];

            RxDataLen = 0;

            TxData[0] = 0x68;
            TxData[1] = 0x92;
            TxData[2] = 0x01;
            TxData[3] = 0x00;
            TxData[4] = 0x03;
            TxData[5] = byte.Parse(strData5, System.Globalization.NumberStyles.HexNumber); ;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            TxDataLen = 8;

            try
            {
                rc = EKTPDLLHandler.Extended_Transmit(_readerSAM, TxDataLen, TxData, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    SAMStatus = true;
                }
                else
                {
                    SAMStatus = false;
                }
            }
            catch (Exception ex)
            {
                SAMStatus = false;
            }
            return SAMStatus;
        }
        private bool _ConnectRFReader()
        {
            int rc;
            bool RFStatus = false;
            try
            {
                rc = EKTPDLLHandler.ConnectSCardReader(0);
                if (rc == 0)
                {
                    RFStatus = true;
                }
                else
                {
                    RFStatus = false;
                }
            }
            catch (Exception ex)
            {
                RFStatus = false;
            }
            return RFStatus;
        }
        public bool ConnectRFReader()
        {
            bool RFStatus = true;
            try
            {
                RFStatus = _ConnectRFReader();
                return RFStatus;
            }
            catch
            {
                RFStatus = false;
                return RFStatus;
            }
        }
        public bool ConnectSAM()
        {
            byte[] byteSAM = NEW_XOR(strIns(_pcidSAM, ","), strIns(_confSAM, ","));
            UInt16 RxDataLen = 0;
            byte[] RxData = new byte[1024];

            byte[] byteCOM = new byte[ConfigCommand.OPENSAM.Length + byteSAM.Length];
            Buffer.BlockCopy(ConfigCommand.OPENSAM, 0, byteCOM, 0, 5);
            Buffer.BlockCopy(byteSAM, 0, byteCOM, 5, 32);
            try
            {
                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);
                if (RxData[0] == 0x90 && RxData[1] == 0x00)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public bool ReadUIDA()
        {
            try
            {
                byte[] byteCOM = ConfigCommand.UIDA;
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];

                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);
                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    ektpData.UID[0] = 0x80;
                    Buffer.BlockCopy(RxData, 0, ektpData.UID, 1, RxDataLen - 2);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool SelectDF()
        {
            UInt16 RxDataLen = 0;
            byte[] RxData = new byte[1024];
            byte[] byteCOM = ConfigCommand.DFEKTP;
            try
            {
                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool SelectEFPhoto()
        {
            UInt16 RxDataLen = 0;
            byte[] RxData = new byte[1024];
            byte[] byteCOM = ConfigCommand.EFPHOTO;

            try
            {
                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);
                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool ReadPhoto()
        {
            bool resp = false;
            try
            {
                resp = SelectDF();
                if (!resp)
                {
                    return false;
                }
                resp = SelectEFPhoto();
                if (!resp)
                {
                    return false;
                }
                UInt16 len_total;
                UInt16 k = 8;
                int l = 2;

                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                //APDU Command P1, P2 And Length
                byte[] byteCOM = new byte[5] { 0x00, 0x00, 0x00, 0x00, 0x08 };


                Buffer.BlockCopy(ConfigCommand.READRF, 0, byteCOM, 0, ConfigCommand.READRF.Length);
                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);
                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }
                byte[] bytePhoto = new byte[RxDataLen - 1];
                bytePhoto[0] = 0x08;
                Buffer.BlockCopy(RxData, 0, bytePhoto, 1, RxDataLen - 2);
                ektpData.lbPhoto.Add(bytePhoto);

                Array.Copy(RxData, 2, ektpData.bytePhoto, 0, RxDataLen - 4);
                ektpData.photolen = RxDataLen - 4;
                len_total = (UInt16)((RxData[0] * 256) + RxData[1] + 2);
                while (k < len_total)
                {
                    UInt16 a = 0;
                    byteCOM[2] = (byte)(k / 256);
                    byteCOM[3] = (byte)(k % 256);
                    if ((len_total - k) / ConfigCommand.param_DATA_LOOP <= 0)
                        a = (UInt16)(len_total - k);
                    else
                        a = ConfigCommand.param_DATA_LOOP;
                    bytePhoto = new byte[a + 1];
                    byteCOM[4] = (byte)a;
                    rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);
                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }
                    bytePhoto[0] = (byte)a;
                    Buffer.BlockCopy(RxData, 0, bytePhoto, 1, RxDataLen - 2);
                    ektpData.lbPhoto.Add(bytePhoto);
                    Array.Copy(RxData, 0, ektpData.bytePhoto, ektpData.photolen, RxDataLen - 2);
                    ektpData.photolen = ektpData.photolen + RxDataLen - 2;

                    k = (UInt16)(k + a);

                    l += 1;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ReadUIDB()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.UIDB;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    Buffer.BlockCopy(RxData, 0, ektpData.UID, 0, RxDataLen - 2);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool SelectEFCC()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.EFCC;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool ReadCC()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.CC;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);
                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    ektpData.CC_LEN = (UInt16)((RxData[0] * 256) + RxData[1]);
                    ektpData.CC = new byte[ektpData.CC_LEN];
                    Buffer.BlockCopy(RxData, 2, ektpData.CC, 0, ektpData.CC_LEN);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool ResetSAM()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.SAMRESET;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool GetChallenge()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.GETCHAL;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    ektpData.GETCHAL_LEN = (UInt16)(RxDataLen - 2);
                    ektpData.GETCHAL = new byte[ektpData.GETCHAL_LEN];
                    Buffer.BlockCopy(RxData, 0, ektpData.GETCHAL, 0, ektpData.GETCHAL_LEN);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool CalcullateChallenge()
        {
            try
            {
                ektpData.CALCHAL_LEN = (UInt16)(ektpData.CC_LEN + ektpData.UID.Length + ektpData.GETCHAL_LEN);
                ektpData.CALCHAL = new byte[ektpData.CALCHAL_LEN];
                Buffer.BlockCopy(ektpData.CC, 0, ektpData.CALCHAL, 0, ektpData.CC_LEN);
                Buffer.BlockCopy(ektpData.UID, 0, ektpData.CALCHAL, ektpData.CC_LEN, ektpData.UID.Length);
                Buffer.BlockCopy(ektpData.GETCHAL, 0, ektpData.CALCHAL, ektpData.CC_LEN + ektpData.UID.Length, ektpData.GETCHAL_LEN);
                byte[] byteCOM = new byte[ConfigCommand.CALCHAL1D.Length + 1 + ektpData.CALCHAL_LEN];
                if (ektpData.CC_LEN == 51)
                    Buffer.BlockCopy(ConfigCommand.CALCHAL33, 0, byteCOM, 0, ConfigCommand.CALCHAL33.Length);
                else
                    Buffer.BlockCopy(ConfigCommand.CALCHAL1D, 0, byteCOM, 0, ConfigCommand.CALCHAL1D.Length);
                byteCOM[ConfigCommand.CALCHAL1D.Length] = (byte)ektpData.CALCHAL_LEN;
                Buffer.BlockCopy(ektpData.CALCHAL, 0, byteCOM, ConfigCommand.CALCHAL1D.Length + 1, ektpData.CALCHAL_LEN);
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    ConfigCommand.param_EXT_AUTH = ByteArrayToString(RxData, RxDataLen - 2);
                    if (ConfigCommand.param_EXT_AUTH.Substring(0, 5) == "Error")
                    {
                        return false;
                    }
                    ConfigCommand.param_EXT_AUTH_LEN = RxDataLen - 2;

                    ektpData.EXTAUTH_LEN = (UInt16)(RxDataLen - 2);
                    ektpData.EXTAUTH = new byte[ektpData.EXTAUTH_LEN];
                    Buffer.BlockCopy(RxData, 0, ektpData.EXTAUTH, 0, ektpData.EXTAUTH_LEN);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool ExternalAuthentication()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = new byte[ConfigCommand.EXTAUTH.Length + ektpData.EXTAUTH_LEN];
                Buffer.BlockCopy(ConfigCommand.EXTAUTH, 0, byteCOM, 0, ConfigCommand.EXTAUTH.Length);
                Buffer.BlockCopy(ektpData.EXTAUTH, 0, byteCOM, ConfigCommand.EXTAUTH.Length, ektpData.EXTAUTH_LEN);

                int rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    ConfigCommand.param_INT_AUTH = ByteArrayToString(RxData, RxDataLen - 2);
                    if (ConfigCommand.param_INT_AUTH.Substring(0, 5) == "Error")
                    {
                        return false;
                    }
                    ConfigCommand.param_INT_AUTH_LEN = RxDataLen - 2;
                    ektpData.INTAUTH_LEN = (UInt16)(RxDataLen - 2);
                    ektpData.INTAUTH = new byte[ektpData.INTAUTH_LEN];
                    Buffer.BlockCopy(RxData, 0, ektpData.INTAUTH, 0, ektpData.INTAUTH_LEN);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool InternalAuthentication()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = new byte[ConfigCommand.INTAUTH.Length + 1 + ektpData.INTAUTH_LEN];
                Buffer.BlockCopy(ConfigCommand.INTAUTH, 0, byteCOM, 0, ConfigCommand.INTAUTH.Length);
                byteCOM[ConfigCommand.INTAUTH.Length] = (byte)ektpData.INTAUTH_LEN;
                Buffer.BlockCopy(ektpData.INTAUTH, 0, byteCOM, ConfigCommand.INTAUTH.Length + 1, ektpData.INTAUTH_LEN);

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);
                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool Authentication()
        {
            try
            {
                if (!SelectEFCC())
                {
                    return false;
                }
                else
                {
                    if (!ReadCC())
                    { return false; }
                    else
                    {
                        if (!ResetSAM())
                        { return false; }
                        else
                        {
                            if (!GetChallenge())
                            { return false; }
                            else
                            {
                                if (!CalcullateChallenge())
                                { return false; }
                                else
                                {
                                    if (!ExternalAuthentication())
                                    { return false; }
                                    else
                                    {
                                        if (!InternalAuthentication())
                                        { return false; }
                                        else
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        private bool SelectEFDS()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.EFDS;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }
                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool ReadDigitalSignature()
        {
            try
            {
                if (!SelectEFDS())
                {
                    return false;
                }
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.READDS;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }
                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }
                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                ektpData.RETDS_LEN = (UInt16)(RxDataLen - 4);
                ektpData.RETDS = new byte[ektpData.RETDS_LEN];
                Buffer.BlockCopy(RxData, 0, ektpData.RETDS, 0, ektpData.RETDS_LEN);

                byteCOM = new byte[ConfigCommand.AUTOVERIF.Length];
                Buffer.BlockCopy(ConfigCommand.AUTOVERIF, 0, byteCOM, 0, ConfigCommand.AUTOVERIF.Length);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }
                byteCOM = new byte[ConfigCommand.RETDS.Length + 1 + ektpData.RETDS_LEN];
                Buffer.BlockCopy(ConfigCommand.RETDS, 0, byteCOM, 0, ConfigCommand.RETDS.Length);
                byteCOM[ConfigCommand.RETDS.Length] = (byte)ektpData.RETDS_LEN;
                Buffer.BlockCopy(ektpData.RETDS, 0, byteCOM, ConfigCommand.RETDS.Length + 1, ektpData.RETDS_LEN);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool ReadBiography()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.EFBIO;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                int k = 8;
                int l = 2;
                int len_total;

                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byte[] byteCOMRead = new byte[ConfigCommand.ENCRYPT.Length + ConfigCommand.SAMREAD.Length + 3];
                Buffer.BlockCopy(ConfigCommand.ENCRYPT, 0, byteCOMRead, 0, ConfigCommand.ENCRYPT.Length);
                Buffer.BlockCopy(ConfigCommand.SAMREAD, 0, byteCOMRead, ConfigCommand.ENCRYPT.Length, ConfigCommand.SAMREAD.Length);
                byteCOMRead[byteCOMRead.Length - 3] = 0x00;
                byteCOMRead[byteCOMRead.Length - 2] = 0x00;
                byteCOMRead[byteCOMRead.Length - 1] = 0x08;

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOMRead.Length, byteCOMRead, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                UInt16 a = 0;
                Buffer.BlockCopy(RxData, 2, ektpData.byteBio, 0, RxDataLen - 6);
                ektpData.biolen = RxDataLen - 6;

                len_total = (RxData[0] * 256) + RxData[1] + 2;
                while (k < len_total)
                {
                    byteCOMRead[byteCOMRead.Length - 3] = Convert.ToByte(k / 256);
                    byteCOMRead[byteCOMRead.Length - 2] = Convert.ToByte(k % 256);
                    if ((len_total - k) / ConfigCommand.param_DATA_LOOP2 <= 0)
                    {
                        a = (UInt16)(len_total - k);
                    }
                    else
                        a = ConfigCommand.param_DATA_LOOP2;
                    byteCOMRead[byteCOMRead.Length - 1] = (byte)a;

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOMRead.Length, byteCOMRead, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }
                    k = k + a;

                    byteCOM = new byte[RxDataLen - 2];
                    Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                    Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                    byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                    Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    Array.Copy(RxData, 0, ektpData.byteBio, ektpData.biolen, RxDataLen - 4);
                    ektpData.biolen = ektpData.biolen + RxDataLen - 4;

                    l += 1;
                }
                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool RetrievePhoto()
        {
            try
            {
                int rc = 0;
                int l = 0;
                foreach (byte[] bytePhoto in ektpData.lbPhoto)
                {
                    //RETP1
                    l += 1;
                    UInt16 RxDataLen = 0;
                    byte[] RxData = new byte[1024];
                    byte[] byteCOM = new byte[ConfigCommand.RETPHOTO.Length + bytePhoto.Length];
                    Buffer.BlockCopy(ConfigCommand.RETPHOTO, 0, byteCOM, 0, ConfigCommand.RETPHOTO.Length);

                    //RETP2
                    Buffer.BlockCopy(bytePhoto, 0, byteCOM, ConfigCommand.RETPHOTO.Length, bytePhoto.Length);

                    //RETP3
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool AutoDeciphering()
        {
            try
            {
                if (!RetrievePhoto())
                {
                    return false;
                }
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.AUTODECIP;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x90 && RxData[RxDataLen - 1] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool ReadSignature()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.SIGNATURE;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                int k = 8;
                int l = 2;
                int len_total;

                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byte[] byteCOMRead = new byte[ConfigCommand.ENCRYPT.Length + ConfigCommand.SAMREAD.Length + 3];
                Buffer.BlockCopy(ConfigCommand.ENCRYPT, 0, byteCOMRead, 0, ConfigCommand.ENCRYPT.Length);
                Buffer.BlockCopy(ConfigCommand.SAMREAD, 0, byteCOMRead, ConfigCommand.ENCRYPT.Length, ConfigCommand.SAMREAD.Length);
                byteCOMRead[byteCOMRead.Length - 3] = 0x00;
                byteCOMRead[byteCOMRead.Length - 2] = 0x00;
                byteCOMRead[byteCOMRead.Length - 1] = 0x08;

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOMRead.Length, byteCOMRead, ref RxDataLen, RxData);

                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                Array.Copy(RxData, 2, ektpData.byteSignature, 0, RxDataLen - 6);
                ektpData.signlen = RxDataLen - 6;

                len_total = (RxData[0] * 256) + RxData[1] + 2;
                while (k < len_total)
                {
                    UInt16 a = 0;
                    byteCOMRead[byteCOMRead.Length - 3] = (byte)(k / 256);
                    byteCOMRead[byteCOMRead.Length - 2] = (byte)(k % 256);

                    if ((len_total - k) / ConfigCommand.param_DATA_LOOP2 <= 0)
                    {
                        a = (UInt16)(len_total - k);
                        if (a % 8 > 0)
                            a = (UInt16)(((a / 8) + 1) * 8);
                    }
                    else
                        a = ConfigCommand.param_DATA_LOOP2;
                    byteCOMRead[byteCOMRead.Length - 1] = (byte)a;

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOMRead.Length, byteCOMRead, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    k = k + a;

                    byteCOM = new byte[RxDataLen - 2];
                    Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                    Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                    byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                    Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    Array.Copy(RxData, 0, ektpData.byteSignature, ektpData.signlen, RxDataLen - 4);
                    ektpData.signlen = ektpData.signlen + RxDataLen - 4;

                    l += 1;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ReadMinutiae1()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.MINUTIAE1;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                int k = 8;
                int l = 2;
                int len_total;

                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byte[] byteCOMRead = new byte[ConfigCommand.ENCRYPT.Length + ConfigCommand.SAMREAD.Length + 3];
                Buffer.BlockCopy(ConfigCommand.ENCRYPT, 0, byteCOMRead, 0, ConfigCommand.ENCRYPT.Length);
                Buffer.BlockCopy(ConfigCommand.SAMREAD, 0, byteCOMRead, ConfigCommand.ENCRYPT.Length, ConfigCommand.SAMREAD.Length);
                byteCOMRead[byteCOMRead.Length - 3] = 0x00;
                byteCOMRead[byteCOMRead.Length - 2] = 0x00;
                byteCOMRead[byteCOMRead.Length - 1] = 0x08;

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOMRead.Length, byteCOMRead, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                Array.Copy(RxData, 2, ektpData.byteMinu1, 0, RxDataLen - 6);
                ektpData.minu1len = ektpData.minu1len + RxDataLen - 6;

                len_total = (RxData[0] * 256) + RxData[1] + 2;
                while (k < len_total)
                {
                    UInt16 a = 0;
                    byteCOMRead[byteCOMRead.Length - 3] = (byte)(k / 256);
                    byteCOMRead[byteCOMRead.Length - 2] = (byte)(k % 256);

                    if ((len_total - k) / ConfigCommand.param_DATA_LOOP2 <= 0)
                    {
                        a = (UInt16)(len_total - k);
                        if (a % 8 > 0)
                            a = (UInt16)(((a / 8) + 1) * 8);
                    }
                    else
                        a = ConfigCommand.param_DATA_LOOP2;
                    byteCOMRead[byteCOMRead.Length - 1] = (byte)a;

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOMRead.Length, byteCOMRead, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    k = k + a;

                    byteCOM = new byte[RxDataLen - 2];
                    Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                    Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                    byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                    Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    Array.Copy(RxData, 0, ektpData.byteMinu1, ektpData.minu1len, RxDataLen - 4);
                    ektpData.minu1len = ektpData.minu1len + RxDataLen - 4;

                    l += 1;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ReadMinutiae2()
        {
            try
            {
                UInt16 RxDataLen = 0;
                byte[] RxData = new byte[1024];
                byte[] byteCOM = ConfigCommand.MINUTIAE2;

                int rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                int k = 8;
                int l = 2;
                int len_total;

                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                byte[] byteCOMRead = new byte[ConfigCommand.ENCRYPT.Length + ConfigCommand.SAMREAD.Length + 3];
                Buffer.BlockCopy(ConfigCommand.ENCRYPT, 0, byteCOMRead, 0, ConfigCommand.ENCRYPT.Length);
                Buffer.BlockCopy(ConfigCommand.SAMREAD, 0, byteCOMRead, ConfigCommand.ENCRYPT.Length, ConfigCommand.SAMREAD.Length);
                byteCOMRead[byteCOMRead.Length - 3] = 0x00;
                byteCOMRead[byteCOMRead.Length - 2] = 0x00;
                byteCOMRead[byteCOMRead.Length - 1] = 0x08;

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOMRead.Length, byteCOMRead, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[RxDataLen - 2];
                Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                {
                    return false;
                }

                Array.Copy(RxData, 2, ektpData.byteMinu2, 0, RxDataLen - 6);
                ektpData.minu2len = ektpData.minu2len + RxDataLen - 6;

                len_total = (RxData[0] * 256) + RxData[1] + 2;
                while (k < len_total)
                {
                    UInt16 a = 0;
                    byteCOMRead[byteCOMRead.Length - 3] = (byte)(k / 256);
                    byteCOMRead[byteCOMRead.Length - 2] = (byte)(k % 256);

                    if ((len_total - k) / ConfigCommand.param_DATA_LOOP2 <= 0)
                    {
                        a = (UInt16)(len_total - k);
                        if (a % 8 > 0)
                            a = (UInt16)(((a / 8) + 1) * 8);

                        byteCOM = new byte[ConfigCommand.STOPAUTOVERIF.Length];
                        byteCOM = ConfigCommand.STOPAUTOVERIF;

                        Array.Clear(RxData, 0, RxData.Length);
                        rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                        if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                        {
                            return false;
                        }
                    }
                    else
                        a = ConfigCommand.param_DATA_LOOP2;
                    byteCOMRead[byteCOMRead.Length - 1] = (byte)a;

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOMRead.Length, byteCOMRead, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    k = k + a;

                    byteCOM = new byte[RxDataLen - 2];
                    Buffer.BlockCopy(RxData, 0, byteCOM, 0, RxDataLen - 2);

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerRF, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    byteCOM = new byte[ConfigCommand.DECRYPT.Length + 1 + RxDataLen - 2];
                    Buffer.BlockCopy(ConfigCommand.DECRYPT, 0, byteCOM, 0, ConfigCommand.DECRYPT.Length);
                    byteCOM[ConfigCommand.DECRYPT.Length] = (byte)(RxDataLen - 2);
                    Buffer.BlockCopy(RxData, 0, byteCOM, ConfigCommand.DECRYPT.Length + 1, RxDataLen - 2);

                    Array.Clear(RxData, 0, RxData.Length);
                    rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                    if (RxData[RxDataLen - 2] != 0x90 && RxData[RxDataLen - 1] != 0x00)
                    {
                        return false;
                    }

                    Array.Copy(RxData, 0, ektpData.byteMinu2, ektpData.minu2len, RxDataLen - 4);
                    ektpData.minu2len = ektpData.minu2len + RxDataLen - 4;

                    l += 1;
                }

                byteCOM = new byte[ConfigCommand.VERIFYDF.Length];
                byteCOM = ConfigCommand.VERIFYDF;

                Array.Clear(RxData, 0, RxData.Length);
                rc = EKTPDLLHandler.APDU_Transmit(_readerSAM, (UInt16)byteCOM.Length, byteCOM, ref RxDataLen, RxData);

                if (RxData[RxDataLen - 2] == 0x00 && RxData[RxDataLen - 1] == 0x10)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool SAMActivation()
        {
            bool SAMStatus1 = false;
            bool SAMStatus2 = false;
            //Cek SAM Slot 1
            try
            {
                SAMStatus1 = SAMReaderSlot(samSlot1);
                SAMStatus2 = SAMReaderSlot(samSlot2);


                if (SAMStatus1)
                {
                    SAMReaderSlot(samSlot1);
                    return true;
                }
                else if (SAMStatus2)
                {
                    SAMReaderSlot(samSlot2);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public int StartEKTP(string pbPcid, string pbConf, UInt16 _rf, UInt16 _sam)
        {
            try
            {

                SetReaderParameter(_rf, _sam);

                SetSAMParameter(pbPcid, pbConf);
                bool resp;
                resp = InitializeContext();
                if (!resp)
                {
                    return 14;
                }
                resp = SAMActivation();
                if (!resp)
                {
                    return 15;
                }
                resp = ConnectRFReader();
                //if (!resp)
                //{
                //    return 1;
                //}
                resp = ConnectSAM();
                if (!resp)
                {
                    return 2;
                }
                resp = ReadUIDA();
                resp = ReadPhoto();
                //if (!resp)
                //{
                //    return 4;
                //}
                resp = ReadUIDB();
                resp = Authentication();
                if (!resp)
                {
                    return 6;
                }
                resp = ReadDigitalSignature();
                if (!resp)
                {
                    return 7;
                }
                resp = ReadBiography();
                if (!resp)
                {
                    return 8;
                }
                resp = AutoDeciphering();
                if (!resp)
                {
                    return 9;
                }
                resp = ReadSignature();
                if (!resp)
                {
                    return 10;
                }
                resp = ReadMinutiae1();
                if (!resp)
                {
                    return 11;
                }
                resp = ReadMinutiae2();
                //if (!resp)
                //{
                //    return 12;
                //}
                DisconnectReader();
                return 0;
            }
            catch
            {
                return 13;
            }
        }

        public void DisconnectReader()
        {
            CRT_DLLHandler.DisconnectSCardReader(_readerSAM);
            CRT_DLLHandler.DisconnectSCardReader(_readerRF);
        }

        public async Task StartCaptureFingerprint()
        {
            TurnOffA600GreenLight();
            TurnOffA600RedLight();

            TurnOnA600GreenLight();

            int pos = 7;

            pGuid = Guid.NewGuid().ToString();
            Task.Delay(100).ContinueWith(t =>
            {
                string token = pGuid;

                try
                {
                    dev.BeginLiveCapture();
                }
                catch (TrustFingerException e)
                {
                    return;
                }

                LiveCaptureFingerprintBitmapData bitmap = null;
                LiveCaptureFingerprintBitmapData bitmap1 = null;
                LiveCaptureFingerprintBitmapData bitmap2 = null;
                isCapture = true;
                isCaptured = false;
                int raisefinger = 0;
                int raisesound = 0;
                int qulitytemp = 0;
                int qulity = 0;
                int qulityThreshold = 50;
                FingerPosition fingerprintPosition = (FingerPosition)pos;
                IFingerprintFeature fearture = null;
                try
                {
                    while (isCapture && (token == pGuid))
                    {
                        try
                        {
                            bitmap = null;
                            bitmap = dev.LiveCaptureBitmapData();
                        }
                        catch (CaptureException e)
                        {
                            break;
                        }
                        bitmap2 = bitmap;//for invoke show picture

                        if (bitmap.CaptureStatus == LiveCaptureStatus.Stop)
                        {
                            isCapture = false;

                            break;
                        }
                        else
                        {
                            if (bitmap.FakeFingerStatus == IsFakeFinger.UNKNOWN)
                            {
                                //ShowErrorMessage("Unknown Finger");
                            }
                            else if (bitmap.FakeFingerStatus == IsFakeFinger.TRUE)
                            {
                                //ShowErrorMessage("Fake Finger");
                            }
                            else
                            {
                                //ShowErrorMessage("");
                            }

                            Bitmap previewBitmap = ConvertBytesToImage(bitmap.FingerprintImageData);
                            try
                            {
                                qulity = dev.ImageQuality(previewBitmap);
                            }
                            catch (ArgumentNullException e)
                            {
                                break;
                            }
                            catch (TrustFingerException ex)
                            {
                                break;
                            }
                            if (true)

                            {
                                if (qulity > qulitytemp && bitmap.FakeFingerStatus == IsFakeFinger.FALSE)
                                {
                                    bitmap1 = null;
                                    bitmap1 = bitmap;
                                    qulitytemp = qulity;
                                    if (qulity >= qulityThreshold)
                                    {
                                        try
                                        {
                                            fearture = dev.ExtractISOFeature(fingerprintPosition);
                                            TurnOffA600GreenLight();
                                        }
                                        catch (InvalidOperationException ex)
                                        {
                                            if (token == pGuid)
                                            {
                                                dev.EndLiveCapture();
                                                Task.Run(new Action(() =>
                                                {
                                                    isCapture = false;
                                                }));
                                            }

                                            break;
                                        }
                                        catch (ExtractFeatureException ex)
                                        {
                                            if (token == pGuid)
                                            {
                                                dev.EndLiveCapture();
                                                Task.Run(new Action(() =>
                                                {
                                                    isCapture = false;
                                                }));
                                            }

                                            break;
                                        }
                                        catch (TrustFingerException ex)
                                        {
                                            if (token == pGuid)
                                            {
                                                dev.EndLiveCapture();
                                                Task.Run(new Action(() =>
                                                {
                                                    isCapture = false;
                                                }));
                                            }

                                            break;
                                        }


                                        if (raisesound == 0)
                                        {
                                            TurnOffA600RedLight();
                                            TurnOnA600GreenLight();
                                            raisesound = 1;
                                        }
                                    }

                                }

                                if (raisesound == 1)
                                {
                                    raisefinger = 1;
                                }

                                if (raisefinger == 1)
                                {
                                    TurnOffA600GreenLight();

                                    try
                                    {
                                        byte[] isoRaw = FingerprintImageHelper.BitmapToRaw(bitmap1.FingerprintImageData, bitmap1.Width, bitmap1.Height);
                                        byte[] iso = FingerprintImageHelper.RawToISO(isoRaw, bitmap1.Width, bitmap1.Height,
                                            fingerprintPosition, 0, dev.ScannerDescription.DeviceId);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (token == pGuid)
                                        {
                                            dev.EndLiveCapture();
                                            Task.Run(new Action(() =>
                                            {
                                                isCapture = false;
                                            }));
                                        }

                                        break;
                                    }

                                    dev.EndLiveCapture();
                                    Task.Run(new Action(() =>
                                    {
                                        isCapture = false;
                                        byte[] isoRaw = FingerprintImageHelper.BitmapToRaw(bitmap1.FingerprintImageData, bitmap1.Width, bitmap1.Height);
                                        byte[] iso = FingerprintImageHelper.RawToISO(isoRaw, bitmap1.Width, bitmap1.Height,
                                            fingerprintPosition, 0, dev.ScannerDescription.DeviceId);
                                        ektpData.FPR = null;
                                        ektpData.FPR = fearture.FeatureData;
                                        isCaptured = true;
                                    }));
                                    break;
                                }

                            }
                            Task.Run(new Action(() =>
                            {
                                if (isCapture)
                                {
                                }

                            }));
                        }

                    }
                }
                catch (Exception ex)
                {
                }
            });
        }
        private void TurnOffA600GreenLight()
        {
            if (true)
            {
                try
                {
                    dev.SetLedStatus(0, LedStatus.Off);

                }
                catch (TrustFingerException e)
                {
                    return;
                }
            }

        }
        private void TurnOffA600RedLight()
        {
            if (true)
            {
                try
                {
                    dev.SetLedStatus(1, LedStatus.Off);
                }
                catch (TrustFingerException ex)
                {
                    return;
                }
            }
        }
        private void TurnOnA600GreenLight()
        {
            if (true)
            {
                try
                {
                    dev.SetLedStatus(0, LedStatus.On);
                }
                catch (TrustFingerException e)
                {
                    return;
                }
            }
        }
        private void TurnOnA600RedLight()
        {
            if (true)
            {
                try
                {
                    dev.SetLedStatus(1, LedStatus.On);
                }
                catch (TrustFingerException e)
                {
                    return;
                }
            }
        }
        private Bitmap ConvertBytesToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            Bitmap bmp;
            using (var ms = new MemoryStream(imageData))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }
        public async Task FPReader()
        {
            try
            {
                TrustFingerManager.GlobalInitialize();
            }
            catch
            {
                return;
            }
            BindDevice();
            OpenFPReader();
        }

        private void BindDevice()
        {
            try
            {
                devCount = TrustFingerManager.GetDeviceCount();

                devCount = 1;
                DeviceItem autodev = new DeviceItem() { Name = "AUTO", DeviceIndex = 0 };
            }
            catch
            {
                return;
            }
        }
        dynamic result;
        dynamic result2;

        public bool FPMatch(byte[] feature, byte[] minutiae1, byte[] minutiae2)
        {
            result = dev.Verify(5, feature, minutiae1);
            result2 = dev.Verify(5, feature, minutiae2);
            CloseFPReader();
            if (result.IsMatch || result2.IsMatch)
            {
                return true;
            }
            else
                return false;
        }
        public async Task CloseFPReader()
        {
            if (isOpen == true)
            {
                TurnOffA600GreenLight();
                TurnOffA600RedLight();
                var desc = dev.ScannerDescription;

                int FV = 0;
                int.TryParse(desc.FirmwareVersion, out FV);

                if (desc.DeviceId == 600 && FV >= 5400)
                {
                    try
                    {
                        if (dev.IsLFD)
                        {
                            dev.EnableLFD(false, 3);
                        }
                        LFDFlag = false;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                capturing = false;
                dev?.Dispose();
            }
            isOpen = false;
        }
        private async Task OpenFPReader()
        {
            isCaptured = false;
            FingerPosition fingerprintPosition = (FingerPosition)1;
            IFingerprintFeature fearture = null;
            ISOFingerprintFeature isofeature = null;
            if (devCount == 0)
            {
                return;
            }

            DeviceItem selectDev = new DeviceItem() { Name = "AUTO", DeviceIndex = 0 };

            if (isOpen == true)
            {
                
            }
            else
            {
                dev?.Dispose();
                dev = new TrustFingerDevice();

                try
                {
                    dev.Open(selectDev.DeviceIndex);
                }
                catch (TrustFingerException ex)
                {
                    return;
                }
                catch (Exception ex)
                {
                    return;
                }

                var desc = dev.ScannerDescription;

                switch (desc.DeviceId)
                {
                    case 600:
                        if (desc.ProductModel != "A600")
                        {
                            dev.Close();
                            return;
                        }
                        else
                        {
                            int FV = 0;
                            int.TryParse(desc.FirmwareVersion, out FV);
                            if (FV >= 5400)
                            {
                                LFDFlag = true;
                            }
                        }
                        break;
                }
                TurnOnA600RedLight();
            }
            isOpen = !isOpen;
        }
    }
}
