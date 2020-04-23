using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using CardConnection;
using CSL_card;
using IMSI_Utility;

namespace MyG5Blazor.Data
{
    public class Card_Reader
    {
        CardObject theCard;
        string readerName = string.Empty;

        public Card_Reader()
        {
            Init();
        }

        private void Init()
        {
            try
            {
                theCard = new CardObject();
                theCard.InitCard();
            }
            catch { }
        }

        public string[] GetCardList()
        {
            string[] result = null;

            try
            {
                result = theCard.GetCardList();
            }
            catch { }

            return result;
        }

        public bool Read(string p_readerName, ref string p_icc_id, ref string p_message)
        {
            bool result = false;

            OurUtility.Write_Log("==== 2. Baca SN", "step-action");

            p_icc_id = string.Empty;
            p_message = string.Empty;

            readerName = p_readerName;

            result = ShowCardInfo(ref p_message);
            if (result)
            {
                result = ReadICCID(ref p_icc_id, ref p_message);
                //TODO: Set_TB_Simcard(xtemp);
            }

            if (result)
            {
                OurUtility.Write_Log("==== 3. SN : " + p_icc_id, "step-action");
            }
            else
            {
                OurUtility.Write_Log("==== 3. Error : " + p_message, "step-action");
            }

            return result;
        }

        private bool ReadICCID(ref string p_icc_id, ref string p_message)
        {
            bool result = false;
            p_icc_id = string.Empty;
            p_message = string.Empty;

            string cmd_3F00 = "A0A40000023F00";
            string cmd_2FE2 = "A0A40000022FE2";
            //string cmd_2FE3 = "A0A40000022FE3";
            string cmd_readEF = "A0B000000A";

            try
            {
                int milliseconds = 1500;
                Thread.Sleep(milliseconds);

                theCard.ResetCard();

                theCard.sendApdu(cmd_3F00);
                theCard.sendApdu(cmd_2FE2);
                string response = theCard.sendApdu(cmd_readEF);

                if (response.Length <= 4)
                {
                    p_message = response + " [ less than 4 bytes ]";
                    return result;
                }

                if (response.ToLower().Contains("error"))
                {
                    p_message = response;
                    return result;
                }

                p_icc_id = Utility.nible_swap(response.Substring(0, response.Length - 4));
                if (p_icc_id[p_icc_id.Length - 1].Equals('F'))
                {
                    p_icc_id = p_icc_id.Remove(p_icc_id.Length - 1);
                }

                result = true;
                //retval += " [" + response  + "]";
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        private bool ShowCardInfo(ref string p_message)
        {
            bool result = false;

            p_message = string.Empty;

            try
            {
                if (theCard.ConnectCard(readerName))
                {
                    SetATRText(theCard.GetATR());
                    result = true;
                }
                else
                {
                    p_message = "No card in " + readerName;
                }
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        delegate void SetATRTextDelegate(string value);

        public void SetATRText(string value)
        {
            /*TODO:if (InvokeRequired)
                Invoke(new SetATRTextDelegate(SetATRText), value);
            else
                label_Ekarip_ATR.Text = value;*/
        }

        delegate void Set_TB_SimcardDelegate(string value);

        public void Set_TB_Simcard(string value)
        {
            /*TODO:if (InvokeRequired)
                Invoke(new Set_TB_SimcardDelegate(Set_TB_Simcard), value);
            else
                txtCardReading_ICCID.Text = value;*/
        }

        public void DisconnectedCard()
        {
            try
            {
                theCard.DisconnectedCard();
            }
            catch { }
        }
    }
}
