using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace MyG5Blazor.Data
{
    public class Ektp
    {
        private bool initialized = false;
        public IntPtr hContext = IntPtr.Zero;

        /* FIXME: change to the right PCID & CONFIGFILE  */

        byte[] pbPcid = null;
        byte[] pbConf = null;

        public Ektp()
        {
            string pcid_str = OurUtility.ReadAllText("fingerprint_Pcid.txt");
            string pconf_str = OurUtility.ReadAllText("fingerprint_Conf.txt");

            pbPcid = OurUtility.StringToByteArray(pcid_str);
            pbConf = OurUtility.StringToByteArray(pconf_str);
        }
        public void EktpConnectEx()
        {
            try
            {
                Ektp_Sdk.EktpConnect(ref hContext, pbPcid, 16, pbConf, 32);
            }
            catch { }
        }

        public void InitializeEktp()
        {
            Ektp_Sdk.Instantiate();

            if (Ektp_Sdk.EktpEstablishContext(ref hContext) == 0)
            {
                Ektp_Sdk.EktpSetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_PCID, pbPcid, pbPcid.Length);
                Ektp_Sdk.EktpSetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_CONFIG, pbConf, pbConf.Length);
                initialized = true;
            }
        }

        private bool ReadEktp(ref Ektp_Data ktp)
        {
            byte[] length = new byte[4];
            byte[] data;
            int size;

            if (!initialized)
                return false;

            // This can take some time, like 8 seconds, to read full e-KTP contents
            int ret = Ektp_Sdk.EktpExecuteCommand(hContext, Ektp_Sdk.EKTP_COMMAND_READ);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            // Photograph
            size = length.Length;
            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_PHOTOGRAPH_SIZE, length, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            size = BitConverter.ToInt32(length, 0);
            data = new byte[size];

            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_PHOTOGRAPH, data, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            ktp.photograph = data;

            // Demograhic
            size = length.Length;
            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_DEMOGRAPHIC_SIZE, length, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            size = BitConverter.ToInt32(length, 0);
            data = new byte[size];

            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_DEMOGRAPHIC, data, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            ktp.demographic = data;

            // Signagure
            size = length.Length;
            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_SIGNAGURE_SIZE, length, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            size = BitConverter.ToInt32(length, 0);
            data = new byte[size];

            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_SIGNAGURE, data, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            ktp.signagure = data;

            // Minutiae1
            size = length.Length;
            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_MINUTIAE1_SIZE, length, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            size = BitConverter.ToInt32(length, 0);
            data = new byte[size];

            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_MINUTIAE1, data, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            ktp.minutiae1 = data;

            // Minutiae2
            size = length.Length;
            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_MINUTIAE2_SIZE, length, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            size = BitConverter.ToInt32(length, 0);
            data = new byte[size];

            ret = Ektp_Sdk.EktpGetAttrib(hContext, Ektp_Sdk.EKTP_ATTR_MINUTIAE2, data, ref size);
            if (ret != Ektp_Sdk.EKTP_S_SUCCESS)
                return false;

            ktp.minutiae2 = data;

            return true;
        }

        private string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        public Ektp_Data ReadDemographic(ref string p_message)
        {
            Ektp_Data result = null;

            p_message = string.Empty;

            try
            {
                int length = 2048;
                byte[] tempBuffer = new byte[length];
                int ret = -1;
                ret = Ektp_Sdk.ReadDemographic(hContext, ref length, tempBuffer);
                if (length == 0 || ret != 0)
                {
                    p_message = "ReadDemographic from SDK returns 0 length";
                    return result;
                }
                byte[] demoGraphic = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    demoGraphic[i] = tempBuffer[i];
                }

                result = new Ektp_Data();
                result.demographic = demoGraphic;
            }
            catch { }

            return result;
        }

        public Ektp_Data ReadPhotograph(ref string p_message)
        {
            Ektp_Data result = null;

            p_message = string.Empty;

            try
            {
                int length = 4096;
                byte[] tempBuffer = new byte[length];
                int ret = -1;
                ret = Ektp_Sdk.ReadPhotograph(hContext, ref length, tempBuffer);
                if (length == 0 || ret != 0)
                {
                    p_message = "ReadPhotograph from SDK returns 0 length";
                    return result;
                }
                byte[] photograph = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    photograph[i] = tempBuffer[i];
                }

                result = new Ektp_Data();
                result.photograph = photograph;
            }
            catch { }

            return result;
        }

        public Ektp_Data ReadSignature(ref string p_message)
        {
            Ektp_Data result = null;

            p_message = string.Empty;

            try
            {
                int length = 2048;
                byte[] tempBuffer = new byte[length];
                int ret = -1;
                ret = Ektp_Sdk.ReadSignature(hContext, ref length, tempBuffer);

                if (length == 0 || ret != 0)
                {
                    p_message = "ReadSignature from SDK returns 0 length";
                    return result;
                }
                byte[] signature = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    signature[i] = tempBuffer[i];
                }

                result = new Ektp_Data();
                result.signagure = signature;
            }
            catch { }

            return result;
        }

        public string ReadPsamUid(ref string p_message)
        {
            string result = string.Empty;

            p_message = string.Empty;

            try
            {
                //the length of sam uid is 7 bytes
                int bufferLen = 7;
                byte[] tempBuffer = new byte[bufferLen];
                int ret = -1;
                ret = Ektp_Sdk.ReadPsamUid(hContext, bufferLen, tempBuffer);
                if (ret != 0)
                {
                    p_message = "ReadPsamUid from SDK returns 0 length";
                    return result;
                }

                result = ToHexString(tempBuffer);
            }
            catch { }

            return result;
        }
    }
}
