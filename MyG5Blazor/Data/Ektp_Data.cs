using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace MyG5Blazor.Data
{
    public class Ektp_Data
    {
        private static string[] FingerPositionArray = new string[]
        {
            "",
            "Ibu Jari Kanan",
            "Jari Telunjuk Kanan",
            "Jari Tengah Kanan",
            "Jari Manis Kanan",
            "Jari Kelingking Kanan",
            "Ibu Jari Kiri",
            "Jari Telunjuk Kiri",
            "Jari Tengah Kiri",
            "Jari Manis Kiri",
            "Jari Kelingking Kiri"
        };
        public byte[] photograph { get; set; }

        public byte[] signagure { get; set; }

        public byte[] minutiae1 { get; set; }
        public int minu1len { get; set; }

        public byte[] minutiae2 { get; set; }
        public int minu2len { get; set; }

        public Bitmap Signature
        {
            get
            {
                try
                {
                    int height = 44;
                    int width = 168;
                    byte[] signatureData = signagure;

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

        public byte[] demographic
        {
            set
            {
                byte[] chars = value;
                int index = 0;
                Demographic = new string[21];

                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i] == '"')
                    {
                        StringBuilder builder = new StringBuilder();

                        for (int j = i + 1; j < chars.Length; j++)
                        {
                            if (chars[j] == '"')
                            {
                                i = j;
                                break;
                            }
                            builder.Append((char)chars[j]);
                        }
                        Demographic[index++] = builder.ToString();
                        if (index >= Demographic.Length)
                            break;
                    }
                }
            }
        }

        private string[] seperator = new string[1] { "\",\"" };

        private string[] Demographic;

        private string GetDemograhicData(int index)
        {
            if (Demographic != null && Demographic.Length > index)
                return Demographic[index];
            return string.Empty;
        }

        public string Id { get { return GetDemograhicData(0); } }
        public string Address { get { return GetDemograhicData(1); } }
        public string Neighbourhood { get { return GetDemograhicData(2); } }
        public string CommunityAssociation { get { return GetDemograhicData(3); } }
        public string PlaceOfBirth { get { return GetDemograhicData(4); } }
        public string District { get { return GetDemograhicData(5); } }
        public string Village { get { return GetDemograhicData(6); } }
        public string City { get { return GetDemograhicData(7); } }
        public string Gender { get { return GetDemograhicData(8); } }
        public string BloodType { get { return GetDemograhicData(9); } }
        public string Religion { get { return GetDemograhicData(10); } }
        public string MarriageStatus { get { return GetDemograhicData(11); } }
        public string Occupation { get { return GetDemograhicData(12); } }
        public string Name { get { return GetDemograhicData(13); } }
        public string DateOfBirth { get { return GetDemograhicData(14); } }
        public string Province { get { return GetDemograhicData(15); } }
        public string DateOfExpiry { get { return GetDemograhicData(16); } }
        public int RightFingerIndex
        {
            get
            {
                try
                {
                    int index = Convert.ToByte(GetDemograhicData(17));
                    if (index > 0 && index < 6)
                        return index;
                }
                catch (Exception)
                { }
                return 2; // Default is 2 (Right index finger)
            }
        }
        public string RightFingerPosition { get { return FingerPositionArray[RightFingerIndex]; } }

        public int LeftFingerIndex
        {
            get
            {
                try
                {
                    int index = Convert.ToByte(GetDemograhicData(18));
                    if (index > 5 && index < 11)
                        return index;
                }
                catch (Exception)
                { }
                return 7; // default is 7 (Left index finger);
            }
        }
        public string LeftFingerPosition { get { return FingerPositionArray[LeftFingerIndex]; } }
        public string Nationality { get { return GetDemograhicData(19); } }
    }
}
