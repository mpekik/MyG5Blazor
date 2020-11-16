using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MyG5Blazor.Data
{
    public class Ektp_Sdk
    {
        public const int EKTP_ATR_LENGTH = 33;

        public const int EKTP_S_SUCCESS = 0;

        public const int EKTP_ATTR_PCID = 0;

        public const int EKTP_ATTR_CONFIG = 1;

        public const int EKTP_ATTR_PHOTOGRAPH_SIZE = 2;

        public const int EKTP_ATTR_PHOTOGRAPH = 3;

        public const int EKTP_ATTR_DEMOGRAPHIC_SIZE = 4;

        public const int EKTP_ATTR_DEMOGRAPHIC = 5;

        public const int EKTP_ATTR_SIGNAGURE_SIZE = 6;

        public const int EKTP_ATTR_SIGNAGURE = 7;

        public const int EKTP_ATTR_MINUTIAE1_SIZE = 8;

        public const int EKTP_ATTR_MINUTIAE1 = 9;

        public const int EKTP_ATTR_MINUTIAE2_SIZE = 10;

        public const int EKTP_ATTR_MINUTIAE2 = 11;

        public const int EKTP_COMMAND_READ = 0;

        [DllImport("EktpSdk.dll")]
        public static extern int EktpEstablishContext(ref IntPtr phContext);

        [DllImport("EktpSdk.dll")]
        public static extern int EktpReleaseContext(IntPtr hContext);

        [DllImport("EktpSdk.dll")]
        public static extern int EktpGetAttrib(IntPtr hContext, int dwAttrId, byte[] pbAttr, ref int dwAttrLen);

        [DllImport("EktpSdk.dll")]
        public static extern int EktpSetAttrib(IntPtr hContext, int dwAttrId, byte[] pbAttr, int dwAttrLen);

        [DllImport("EktpSdk.dll")]
        public static extern int EktpExecuteCommand(IntPtr hContext, int dwCmdId);

        [DllImport("EktpSdk.dll")]
        public static extern int OpenFingerPrintReader(ref IntPtr readerContext, string readerName);

        [DllImport("EktpSdk.dll")]
        public static extern int CaptureFingerPrint(IntPtr reader, byte[] image, ref int size, ref int nfiqScore);

        [DllImport("EktpSdk.dll")]
        public static extern int StartEngine(int type, ref IntPtr myEngine);

        [DllImport("EktpSdk.dll")]
        public static extern int MatchFingerPrint(IntPtr engine,
                                                byte[] template1, int template1_size,
                                                byte[] template2, int template2_size,
                                                ref int score, ref int matched, int format);

        [DllImport("EktpSdk.dll")]
        public static extern int EktpConnect(ref IntPtr phContext,
                                            byte[] pcid,
                                            int pcidLen,
                                            byte[] conf,
                                            int confLen);

        [DllImport("EktpSdk.dll")]
        public static extern int ReadPsamUid(IntPtr phContext,
                                                int bufferLen,
                                                byte[] cardUid);

        [DllImport("EktpSdk.dll")]
        public static extern int ReadPhotograph(IntPtr phContext,
                                                ref int length,
                                                byte[] buffer);

        [DllImport("EktpSdk.dll")]
        public static extern int ReadDemographic(IntPtr phContext,
                                                ref int length,
                                                byte[] buffer);

        [DllImport("EktpSdk.dll")]
        public static extern int ReadSignature(IntPtr phContext,
                                                ref int length,
                                                byte[] buffer);

        [DllImport("EktpSdk.dll")]
        public static extern int ReadLeftFingerPrint(IntPtr phContext,
                                                ref int length,
                                                byte[] buffer);

        [DllImport("EktpSdk.dll")]
        public static extern int ReadRightFingerPrint(IntPtr phContext,
                                                ref int length,
                                                byte[] buffer);

        [DllImport("EktpSdk.dll")]
        public static extern int ReadFingerPrints(IntPtr phContext,
                                                ref int leftLen,
                                                byte[] leftFinger,
                                                ref int rightLen,
                                                byte[] rightFinger);


        //----------------new interface------------------
        [DllImport("EktpSdk.dll")]
        public static extern int GetTemplateFromImage(IntPtr engine,
                                              byte[] image, int image_size,
                                              byte[] templatebuf, ref int templatebuf_size,
                                              int format);
        [DllImport("EktpSdk.dll")]
        public static extern int SetMatchFingerPrintThreshold(IntPtr engine, int threshold);

        [DllImport("EktpSdk.dll")]
        public static extern int GetMatchFingerPrintThreshold(IntPtr engine, ref int threshold);
        //---------------------------------------------------------------------------


        [DllImport("EktpSdk.dll")]
        public static extern int CloseReader(IntPtr phContext);

        [DllImport("kernel32.dll")]
        private static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll")]
        private static extern int GetDllDirectory(int nBufferLength, byte[] lpBuffer);

        public static void Instantiate()
        {
            // setup the path for loading DLL
            SetDllDirectory(Environment.SystemDirectory);
            SetDllDirectory(Environment.CurrentDirectory);
            SetDllDirectory(Environment.CurrentDirectory + @"..\..\libs");
        }
    }
}
