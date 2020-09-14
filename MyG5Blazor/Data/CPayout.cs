using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITLlib;
using System.Text;
using System.IO.Ports;

namespace MyG5Blazor.Data
{
    public class CPayout
    {
        // ssp library variables
        SSPComms m_eSSP = new SSPComms();
        SSP_COMMAND m_cmd = new SSP_COMMAND();
        SSP_KEYS keys = new SSP_KEYS();
        SSP_FULL_KEY sspKey = new SSP_FULL_KEY();
        SSP_COMMAND_INFO info = new SSP_COMMAND_INFO();
        // variable declarations
        // The logging class

        //CCommsWindow m_Comms;

        // The protocol version this validator is using, set in setup request
        int m_ProtocolVersion;

        // The number of channels used in this validator
        int m_NumberOfChannels;

        // The type of unit this class represents, set in the setup request
        char m_UnitType;

        // The multiplier by which the channel values are multiplied to get their
        // true penny value.
        int m_ValueMultiplier;
        //Integer to hold total number of Hold messages to be issued before releasing note from escrow
        int m_HoldNumber;

        //Integer to hold number of hold messages still to be issued
        int m_HoldCount;

        //Bool to hold flag set to true if a note is being held in escrow
        bool m_NoteHeld;

        // A list of dataset data, sorted by value. Holds the info on channel number, value, currency,
        // level and whether it is being recycled.
        List<ChannelData> m_UnitDataList;
        int UangMasukSekarang = 0; //nominal uang setiap masuk bezel
        public int total = 0; //total nominal uang yang diterima
        public int uangCount = 0;
        public CPayout()
        {
            m_eSSP = new SSPComms();
            m_cmd = new SSP_COMMAND();
            //storedCmd = new SSP_COMMAND();
            keys = new SSP_KEYS();
            sspKey = new SSP_FULL_KEY();
            info = new SSP_COMMAND_INFO();

            //m_Comms = new CCommsWindow();
            //m_Comms.Text = "SMART Payout Comms";
            m_ProtocolVersion = 0;
            m_NumberOfChannels = 0;
            m_ValueMultiplier = 1;
            m_UnitDataList = new List<ChannelData>();
            m_HoldCount = 0;
            m_HoldNumber = 0;
        }
        /* Variable Access */

        public static int Jumlah { get; set; }
        public static int InputJumlah { get; set; }
        // access to ssp vars

        public SSPComms SSPComms
        {
            get { return m_eSSP; }
            set { m_eSSP = value; }
        }

        public SSP_COMMAND CommandStructure
        {
            get { return m_cmd; }
            set { m_cmd = value; }
        }

        public SSP_COMMAND_INFO InfoStructure
        {
            get { return info; }
            set { info = value; }
        }

        // access to the comms log
        /*
        public CCommsWindow CommsLog
        {
            get { return m_Comms; }
            set { m_Comms = value; }
        }
        */

        // access to number of channels
        public int NumberOfChannels
        {
            get { return m_NumberOfChannels; }
            set { m_NumberOfChannels = value; }
        }

        // access to value multiplier
        public int Multiplier
        {
            get { return m_ValueMultiplier; }
            set { m_ValueMultiplier = value; }
        }

        // acccess to hold number
        public int HoldNumber
        {
            get { return m_HoldNumber; }
            set { m_HoldNumber = value; }

        }
        //Access to flag showing note is held in escrow
        public bool NoteHeld
        {
            get { return m_NoteHeld; }
        }

        // access to sorted list of hash entries
        public List<ChannelData> UnitDataList
        {
            get { return m_UnitDataList; }
        }

        // access to the type of unit
        public char UnitType
        {
            get { return m_UnitType; }
        }

        //Command Function
        public bool OpenComPort()
        {
            if (m_eSSP.OpenSSPComPort(m_cmd)) return true;
            return false;
        }

        public void EnableValidator()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand()) return;
            if (CheckGenericResponses());
        }

        public void DisableValidator()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_DISABLE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand()) return;
            if (CheckGenericResponses()) ;
        }

        public void EnablePayout()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE_PAYOUT_DEVICE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return;
            if (CheckGenericResponses()) ;
        }
        public void EmptyPayoutDevice()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_EMPTY_ALL;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return;
            if (CheckGenericResponses())
            {

            }
        }

        public int CheckNoteLevel(int note, char[] currency)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_DENOMINATION_LEVEL;
            byte[] b = CHelper.ConvertIntToBytes(note);
            m_cmd.CommandData[1] = b[0];
            m_cmd.CommandData[2] = b[1];
            m_cmd.CommandData[3] = b[2];
            m_cmd.CommandData[4] = b[3];

            m_cmd.CommandData[5] = (byte)currency[0];
            m_cmd.CommandData[6] = (byte)currency[1];
            m_cmd.CommandData[7] = (byte)currency[2];
            m_cmd.CommandDataLength = 8;

            if (!SendCommand()) return 0;
            if (CheckGenericResponses())
            {
                int i = (int)m_cmd.ResponseData[1];
                return i;
            }
            return 0;
        }

        public void ReturnNote()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_REJECT_BANKNOTE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand()) return;

            if (CheckGenericResponses())
            {

                m_HoldCount = 0;
            }
        }

        public void GetAllLevels()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_ALL_LEVELS;
            m_cmd.CommandDataLength = 1;
            StringBuilder sbOutput = new StringBuilder(500);
            int noteValue;
            int noteNumber;
            if (!SendCommand())
            {
                return;
            }
            // Proceed if SSP_RESPONSE_OK returned
            if (CheckGenericResponses())
            {
                sbOutput.Append("Number of Denominations = ");
                sbOutput.Append(m_cmd.ResponseData[1]);
                sbOutput.AppendLine();
                for (int i = 1; i <= m_cmd.ResponseData[1]; i++)
                {
                    noteNumber = BitConverter.ToInt16(m_cmd.ResponseData, (9 * i) - 7);
                    noteValue = BitConverter.ToInt32(m_cmd.ResponseData, (9 * i) - 5) / m_ValueMultiplier;
                    sbOutput.Append(noteNumber);
                    sbOutput.Append(" x ");
                    sbOutput.Append(noteValue);
                    sbOutput.Append(" ");
                    sbOutput.Append((char)m_cmd.ResponseData[(9 * i) - 1]);
                    sbOutput.Append((char)m_cmd.ResponseData[(9 * i)]);
                    sbOutput.Append((char)m_cmd.ResponseData[(9 * i) + 1]);
                    sbOutput.Append(" = ");
                    sbOutput.Append(noteNumber * noteValue);
                    sbOutput.AppendLine();
                }
                sbOutput.AppendLine();
            }
        }

        public void ChangeNoteRoute(int note, char[] currency, bool stack)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_DENOMINATION_ROUTE;

            // if this note is being changed to stack (cashbox) --> default to cashbox but change to payout
            if (!stack) // default stack
                m_cmd.CommandData[1] = 0x01;
            // note being stored (payout) --> default to payout butchange to cashbox
            else
                m_cmd.CommandData[1] = 0x00;

            // get the note as a byte array
            byte[] b = BitConverter.GetBytes(note);
            m_cmd.CommandData[2] = b[0];
            m_cmd.CommandData[3] = b[1];
            m_cmd.CommandData[4] = b[2];
            m_cmd.CommandData[5] = b[3];

            // send country code (protocol 6+)
            m_cmd.CommandData[6] = (byte)currency[0];
            m_cmd.CommandData[7] = (byte)currency[1];
            m_cmd.CommandData[8] = (byte)currency[2];

            m_cmd.CommandDataLength = 9;

            if (!SendCommand()) return;
            if (CheckGenericResponses())
            {
                string s = new string(currency);
                if (stack)
                    s += " to storage )\r\n";
                else
                    s += " to cashbox )\r\n";
            }
        }

        public void Reset()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_RESET;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return;
            CheckGenericResponses();
        }

        public bool SendSync()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SYNC;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return false;
            if (CheckGenericResponses()) ;

            return true;
        }

        public void SetProtocolVersion(byte pVersion)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_HOST_PROTOCOL_VERSION;
            m_cmd.CommandData[1] = pVersion;
            m_cmd.CommandDataLength = 2;
            if (!SendCommand()) return;
        }

        public void PayoutAmount(int amount, char[] currency)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_PAYOUT_AMOUNT;
            byte[] b = CHelper.ConvertIntToBytes(amount);
            m_cmd.CommandData[1] = b[0];
            m_cmd.CommandData[2] = b[1];
            m_cmd.CommandData[3] = b[2];
            m_cmd.CommandData[4] = b[3];

            m_cmd.CommandData[5] = (byte)currency[0];
            m_cmd.CommandData[6] = (byte)currency[1];
            m_cmd.CommandData[7] = (byte)currency[2];

            m_cmd.CommandData[8] = 0x58; // real payout

            m_cmd.CommandDataLength = 9;
            if (!SendCommand()) return;

            if (CheckGenericResponses())
            {
            }
        }

        public void PayoutByDenomination(byte numDenoms, byte[] data, byte dataLength)
        {
            // First is the command byte
            m_cmd.CommandData[0] = CCommands.SSP_CMD_PAYOUT_BY_DENOMINATION;

            // Next is the number of denominations to be paid out
            m_cmd.CommandData[1] = numDenoms;

            // Copy over data byte array parameter into command structure
            int currentIndex = 2;
            for (int i = 0; i < dataLength; i++)
                m_cmd.CommandData[currentIndex++] = data[i];

            // Perform a real payout (0x19 for test)
            m_cmd.CommandData[currentIndex++] = 0x58;

            // Length of command data (add 3 to cover the command byte, num of denoms and real/test byte)
            dataLength += 3;
            m_cmd.CommandDataLength = dataLength;

            if (!SendCommand()) return;
            if (CheckGenericResponses())
            {
            }
        }

        public bool NegotiateKeys()
        {
            // make sure encryption is off
            m_cmd.EncryptionStatus = false;

            // send sync
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SYNC;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return false;

            m_eSSP.InitiateSSPHostKeys(keys, m_cmd);

            // send generator
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_GENERATOR;
            m_cmd.CommandDataLength = 9;

            // Convert generator to bytes and add to command data.
            BitConverter.GetBytes(keys.Generator).CopyTo(m_cmd.CommandData, 1);

            if (!SendCommand()) return false;

            // send modulus
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_MODULUS;
            m_cmd.CommandDataLength = 9;

            // Convert modulus to bytes and add to command data.
            BitConverter.GetBytes(keys.Modulus).CopyTo(m_cmd.CommandData, 1);

            if (!SendCommand()) return false;

            // send key exchange
            m_cmd.CommandData[0] = CCommands.SSP_CMD_REQUEST_KEY_EXCHANGE;
            m_cmd.CommandDataLength = 9;
            // Convert host intermediate key to bytes and add to command data.
            BitConverter.GetBytes(keys.HostInter).CopyTo(m_cmd.CommandData, 1);
            if (!SendCommand()) return false;

            // Read slave intermediate key.
            keys.SlaveInterKey = BitConverter.ToUInt64(m_cmd.ResponseData, 1);
            m_eSSP.CreateSSPHostEncryptionKey(keys);
            // get full encryption key
            m_cmd.Key.FixedKey = 0x0123456701234567;
            m_cmd.Key.VariableKey = keys.KeyHost;
            return true;
        }

        public void PayoutSetupRequest()
        {
            StringBuilder sbDisplay = new StringBuilder(1000);

            // send setup request
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SETUP_REQUEST;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return;

            // display setup request

            // unit type
            int index = 1;
            sbDisplay.Append("Unit Type: ");
            m_UnitType = (char)m_cmd.ResponseData[index++];
            switch (m_UnitType)
            {
                case (char)0x00: sbDisplay.Append("Validator"); break;
                case (char)0x03: sbDisplay.Append("SMART Hopper"); break;
                case (char)0x06: sbDisplay.Append("SMART Payout"); break;
                case (char)0x07: sbDisplay.Append("NV11"); break;
                default: sbDisplay.Append("Unknown Type"); break;
            }

            // firmware
            sbDisplay.AppendLine();
            sbDisplay.Append("Firmware: ");
            while (index <= 5)
            {
                sbDisplay.Append((char)m_cmd.ResponseData[index++]);
                if (index == 4)
                    sbDisplay.Append(".");
            }
            sbDisplay.AppendLine();
            // country code.
            // legacy code so skip it.
            index += 3;

            // value multiplier.
            // legacy code so skip it.
            index += 3;

            // Number of channels
            sbDisplay.AppendLine();
            sbDisplay.Append("Number of Channels: ");
            m_NumberOfChannels = m_cmd.ResponseData[index++];
            sbDisplay.Append(m_NumberOfChannels);
            sbDisplay.AppendLine();

            // channel values.
            // legacy code so skip it.
            index += m_NumberOfChannels; // Skip channel values

            // channel security
            // legacy code so skip it.
            index += m_NumberOfChannels;

            // real value multiplier
            // (big endian)
            sbDisplay.Append("Real Value Multiplier: ");
            m_ValueMultiplier = m_cmd.ResponseData[index + 2];
            m_ValueMultiplier += m_cmd.ResponseData[index + 1] << 8;
            m_ValueMultiplier += m_cmd.ResponseData[index] << 16;
            sbDisplay.Append(m_ValueMultiplier);
            sbDisplay.AppendLine();
            index += 3;


            // protocol version
            sbDisplay.Append("Protocol Version: ");
            m_ProtocolVersion = m_cmd.ResponseData[index++];
            sbDisplay.Append(m_ProtocolVersion);
            sbDisplay.AppendLine();

            // Add channel data to list then display.
            // Clear list.
            m_UnitDataList.Clear();
            // Loop through all channels.

            for (byte i = 0; i < m_NumberOfChannels; i++)
            {
                ChannelData loopChannelData = new ChannelData();
                // Channel number.
                loopChannelData.Channel = (byte)(i + 1);

                // Channel value.
                loopChannelData.Value = BitConverter.ToInt32(m_cmd.ResponseData, index + (m_NumberOfChannels * 3) + (i * 4)) * m_ValueMultiplier;

                // Channel Currency
                loopChannelData.Currency[0] = (char)m_cmd.ResponseData[index + (i * 3)];
                loopChannelData.Currency[1] = (char)m_cmd.ResponseData[(index + 1) + (i * 3)];
                loopChannelData.Currency[2] = (char)m_cmd.ResponseData[(index + 2) + (i * 3)];

                // Channel level.
                loopChannelData.Level = CheckNoteLevel(loopChannelData.Value, loopChannelData.Currency);

                // Channel recycling
                bool recycling = false; // default false
                IsNoteRecycling(loopChannelData.Value, loopChannelData.Currency, ref recycling);
                loopChannelData.Recycling = recycling;

                // Add data to list.
                m_UnitDataList.Add(loopChannelData);

                //Display data
                sbDisplay.Append("Channel ");
                sbDisplay.Append(loopChannelData.Channel);
                sbDisplay.Append(": ");
                sbDisplay.Append(loopChannelData.Value / m_ValueMultiplier);
                sbDisplay.Append(" ");
                sbDisplay.Append(loopChannelData.Currency);
                sbDisplay.AppendLine();
            }

            // Sort the list by .Value.
            m_UnitDataList.Sort((d1, d2) => d1.Value.CompareTo(d2.Value));
        }

        public void SetInhibits()
        {
            // set inhibits
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_CHANNEL_INHIBITS;
            m_cmd.CommandData[1] = 0xFF;
            m_cmd.CommandData[2] = 0xFF;
            m_cmd.CommandDataLength = 3;

            if (!SendCommand()) return;
            if (CheckGenericResponses()) ;
        }

        // This function uses the GET ROUTING command to see if a specified note is recycling. The
        // caller passes a bool across which is set by the function.
        public void IsNoteRecycling(int noteValue, char[] currency, ref bool response)
        {
            // Determine if the note is currently being recycled
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_DENOMINATION_ROUTE;
            byte[] b = CHelper.ConvertIntToBytes(noteValue);
            m_cmd.CommandData[1] = b[0];
            m_cmd.CommandData[2] = b[1];
            m_cmd.CommandData[3] = b[2];
            m_cmd.CommandData[4] = b[3];

            // Add currency
            m_cmd.CommandData[5] = (byte)currency[0];
            m_cmd.CommandData[6] = (byte)currency[1];
            m_cmd.CommandData[7] = (byte)currency[2];
            m_cmd.CommandDataLength = 8;

            if (!SendCommand()) return;
            if (CheckGenericResponses())
            {
                // True if it is currently being recycled
                if (m_cmd.ResponseData[1] == 0x00)
                {
                    response = false; // default true;

                }
                // False if not
                else if (m_cmd.ResponseData[1] == 0x01)
                {
                    response = true;//default false
                }
            }
        }

        public void SetFloat(int minPayout, int floatAmount, char[] currency)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_FLOAT_AMOUNT;
            byte[] b = CHelper.ConvertIntToBytes(minPayout);
            m_cmd.CommandData[1] = b[0];
            m_cmd.CommandData[2] = b[1];
            m_cmd.CommandData[3] = b[2];
            m_cmd.CommandData[4] = b[3];
            b = CHelper.ConvertIntToBytes(floatAmount);
            m_cmd.CommandData[5] = b[0];
            m_cmd.CommandData[6] = b[1];
            m_cmd.CommandData[7] = b[2];
            m_cmd.CommandData[8] = b[3];

            // Add currency
            m_cmd.CommandData[9] = (byte)currency[0];
            m_cmd.CommandData[10] = (byte)currency[1];
            m_cmd.CommandData[11] = (byte)currency[2];

            m_cmd.CommandData[12] = 0x58; // real float (could use 0x19 for test)

            m_cmd.CommandDataLength = 13;

            if (!SendCommand()) return;
            if (CheckGenericResponses())
            {
            }
        }

        public void SmartEmpty()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SMART_EMPTY;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return;
            if (CheckGenericResponses())
            {
            }
        }

        public string GetCashboxPayoutOpData()
        {
            StringBuilder sbDisplay = new StringBuilder(100);
            // first send the command
            m_cmd.CommandData[0] = CCommands.SSP_CMD_CASHBOX_PAYOUT_OPERATION_DATA;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return "";

            // now deal with the response data
            if (CheckGenericResponses())
            {
                // number of different notes
                int numberOfDenominations = m_cmd.ResponseData[1];

                sbDisplay.Append("Total Number of Notes: ");
                sbDisplay.Append(numberOfDenominations.ToString());
                sbDisplay.AppendLine();
                sbDisplay.AppendLine();

                int i = 0;
                for (i = 2; i < (9 * numberOfDenominations); i += 9)
                {
                    sbDisplay.Append("Moved ");
                    sbDisplay.Append(CHelper.ConvertBytesToInt16(m_cmd.ResponseData, i));
                    sbDisplay.Append(" x ");
                    sbDisplay.Append(CHelper.FormatToCurrency(CHelper.ConvertBytesToInt32(m_cmd.ResponseData, i + 2)));
                    sbDisplay.Append(" ");
                    sbDisplay.Append((char)m_cmd.ResponseData[i + 6]);
                    sbDisplay.Append((char)m_cmd.ResponseData[i + 7]);
                    sbDisplay.Append((char)m_cmd.ResponseData[i + 8]);
                    sbDisplay.Append(" to cashbox");
                    sbDisplay.AppendLine();
                }

                sbDisplay.Append(CHelper.ConvertBytesToInt32(m_cmd.ResponseData, i));
                sbDisplay.Append(" notes not recognised");
                sbDisplay.AppendLine();

                return sbDisplay.ToString();
            }
            return "";
        }

        // This function changes the colour of a supported bezel.  As command data byte 4 is set to 0x00, the change will not
        // be stored in EEPROM.
        public void ConfigureBezel(byte red, byte green, byte blue)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_CONFIGURE_BEZEL;
            m_cmd.CommandData[1] = red;
            m_cmd.CommandData[2] = green;
            m_cmd.CommandData[3] = blue;
            m_cmd.CommandData[4] = 0x00;
            m_cmd.CommandDataLength = 5;
            if (!SendCommand()) return;

        }

        // This function sends the command LAST REJECT CODE which gives info about why a note has been rejected. It then
        // outputs the info to a passed across textbox.
        public void QueryRejection()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_LAST_REJECT_CODE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand()) return;

            if (CheckGenericResponses())
            {
                switch (m_cmd.ResponseData[1])
                {
                    case 0x00: break;
                    case 0x01: break;
                    case 0x02: break;
                    case 0x03: break;
                    case 0x04: break;
                    case 0x05: break;
                    case 0x06: break;
                    case 0x07: break;
                    case 0x08: break;
                    case 0x09: break;
                    case 0x0A: break;
                    case 0x0B: break;
                    case 0x0C: break;
                    case 0x0D: break;
                    case 0x0E: break;
                    case 0x0F: break;
                    case 0x10: break;
                    case 0x11: break;
                    case 0x12: break;
                    case 0x13: break;
                    case 0x14: break;
                    case 0x15: break;
                    case 0x16: break;
                    case 0x17: break;
                    case 0x18: break;
                    case 0x19: break;
                    case 0x1A: break;
                }
            }
        }
        // This function gets the serial number of the device.  An optional Device parameter can be used
        // for TEBS systems to specify which device's serial number should be returned.
        // 0x00 = NV200
        // 0x01 = SMART Payout
        // 0x02 = Tamper Evident Cash Box.
        public void GetSerialNumber(byte Device)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_SERIAL_NUMBER;
            m_cmd.CommandData[1] = Device;
            m_cmd.CommandDataLength = 2;


            if (!SendCommand()) return;
            if (CheckGenericResponses())
            {
                // Response data is big endian, so reverse bytes 1 to 4.
                Array.Reverse(m_cmd.ResponseData, 1, 4);
            }
        }

        public void GetSerialNumber()
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_SERIAL_NUMBER;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand()) return;
            if (CheckGenericResponses())
            {
                // Response data is big endian, so reverse bytes 1 to 4.
                Array.Reverse(m_cmd.ResponseData, 1, 4);
            }
        }

        public bool DoPoll()
        {
            byte i;
            // If a not is to be held in escrow, send hold commands, as poll releases note.
            if (m_HoldCount > 0)
            {
                m_NoteHeld = true;
                m_HoldCount--;
                m_cmd.CommandData[0] = CCommands.SSP_CMD_HOLD;
                m_cmd.CommandDataLength = 1;
                if (!SendCommand()) return false;
                return true;
            }
            //send poll
            m_cmd.CommandData[0] = CCommands.SSP_CMD_POLL;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand()) return false;

            // store response locally so data can't get corrupted by other use of the cmd variable
            byte[] response = new byte[255];
            m_cmd.ResponseData.CopyTo(response, 0);
            byte responseLength = m_cmd.ResponseDataLength;

            //parse poll response
            ChannelData data = new ChannelData();
            for (i = 1; i < responseLength; ++i)
            {
                switch (response[i])
                {
                    // This response indicates that the unit was reset and this is the first time a poll
                    // has been called since the reset.
                    case CCommands.SSP_POLL_SLAVE_RESET:
                        UpdateData();
                        break;
                    // A note is currently being read by the validator sensors. The second byte of this response
                    // is zero until the note's type has been determined, it then changes to the channel of the 
                    // scanned note.
                    case CCommands.SSP_POLL_READ_NOTE:
                        if (m_cmd.ResponseData[i + 1] > 0)
                        {
                            GetDataByChannel(response[i + 1], ref data);
                            OurUtility.Write_Log("Uang Yang Dimasukkan : " + CHelper.FormatToCurrency(data.Value),"step-action");
                            if ((data.Value / 100) < 10000)
                            {
                                OurUtility.Write_Log("Uang Dikembalikan","step-action");
                                ReturnNote();
                            }
                            // log.AppendText("Nilai " + nilai_value+ "\r\n");
                            // log.AppendText("Uang masuk - uang harus bayar : " + CHelpers.FormatToCurrency(data.Value) + " - " + jmlBayar + "\r\n") ;
                            m_HoldCount = m_HoldNumber;
                        }
                        else
                            i++;
                        break;
                    // A credit event has been detected, this is when the validator has accepted a note as legal currency.
                    case CCommands.SSP_POLL_CREDIT_NOTE:
                        GetDataByChannel(response[i + 1], ref data);
                        OurUtility.Write_Log("Uang Yang Diterima: " + CHelper.FormatToCurrency(data.Value),"step-action");
                        UangMasukSekarang = data.Value / 100;
                        total += UangMasukSekarang;
                        uangCount += 1;
                        OurUtility.Write_Log("Total Uang Yang Diterima : " + total,"step-action");
                        OurUtility.Write_Log("UangMasukSekarang : " + UangMasukSekarang,"step-action");
                        UpdateData();
                        i++;
                        return true;
                        break;
                    // A note is being rejected from the validator. This will carry on polling while the note is in transit.
                    case CCommands.SSP_POLL_NOTE_REJECTING:
                        break;
                    // A note has been rejected from the validator, the note will be resting in the bezel. This response only
                    // appears once.
                    case CCommands.SSP_POLL_NOTE_REJECTED:
                        QueryRejection();
                        break;
                    // A note is in transit to the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKING:
                        break;
                    // A note has reached the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKED:
                        break;
                    // A safe jam has been detected. This is where the user has inserted a note and the note
                    // is jammed somewhere that the user cannot reach.
                    case CCommands.SSP_POLL_SAFE_NOTE_JAM:
                        break;
                    // An unsafe jam has been detected. This is where a user has inserted a note and the note
                    // is jammed somewhere that the user can potentially recover the note from.
                    case CCommands.SSP_POLL_UNSAFE_NOTE_JAM:
                        break;
                    // The validator is disabled, it will not execute any commands or do any actions until enabled.
                    case CCommands.SSP_POLL_DISABLED:
                        break;
                    // A fraud attempt has been detected. 
                    case CCommands.SSP_POLL_FRAUD_ATTEMPT:
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The stacker (cashbox) is full.
                    case CCommands.SSP_POLL_STACKER_FULL:
                        break;
                    // A note was detected somewhere inside the validator on startup and was rejected from the front of the
                    // unit.
                    case CCommands.SSP_POLL_NOTE_CLEARED_FROM_FRONT:
                        i++;
                        break;
                    // A note was detected somewhere inside the validator on startup and was cleared into the cashbox.
                    case CCommands.SSP_POLL_NOTE_CLEARED_TO_CASHBOX:
                        i++;
                        break;
                    // A note has been detected in the validator on startup and moved to the payout device 
                    case CCommands.SSP_POLL_NOTE_PAID_INTO_STORE_AT_POWER_UP:
                        i += 7;
                        break;
                    // A note has been detected in the validator on startup and moved to the cashbox
                    case CCommands.SSP_POLL_NOTE_PAID_INTO_STACKER_AT_POWER_UP:
                        i += 7;
                        break;
                    // The cashbox has been removed from the unit. This will continue to poll until the cashbox is replaced.
                    case CCommands.SSP_POLL_CASHBOX_REMOVED:
                        break;
                    // The cashbox has been replaced, this will only display on a poll once.
                    case CCommands.SSP_POLL_CASHBOX_REPLACED:
                        break;
                    // The payout device is in the process of emptying all its stored notes to the cashbox. This
                    // will continue to poll until the device is empty.
                    case CCommands.SSP_POLL_EMPTYING:
                        break;
                    // This single poll response indicates that the payout device has finished emptying.
                    case CCommands.SSP_POLL_EMPTIED:
                        UpdateData();
                        EnableValidator();
                        break;
                    // The payout device has encountered a jam. This will not clear until the jam has been removed and the unit
                    // reset.
                    case CCommands.SSP_POLL_JAMMED:
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // This is reported when the payout has been halted by a host command. This will report the value of
                    // currency dispensed upto the point it was halted. 
                    case CCommands.SSP_POLL_HALTED:
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // This is reported when the payout was powered down during a payout operation. It reports the original amount
                    // requested and the amount paid out up to this point for each currency.
                    case CCommands.SSP_POLL_INCOMPLETE_PAYOUT:
                        i += (byte)((response[i + 1] * 11) + 1);
                        break;
                    // This is reported when the payout was powered down during a float operation. It reports the original amount
                    // requested and the amount paid out up to this point for each currency.
                    case CCommands.SSP_POLL_INCOMPLETE_FLOAT:
                        i += (byte)((response[i + 1] * 11) + 1);
                        break;
                    // A note has been transferred from the payout unit to the stacker.
                    // Typo as in SSP protocol manual.
                    case CCommands.SSP_POLL_NOTE_TRANSFERED_TO_STACKER:
                        i += 7;
                        break;
                    // A note is resting in the bezel waiting to be removed by the user.
                    case CCommands.SSP_POLL_NOTE_HELD_IN_BEZEL:
                        i += 7;
                        break;
                    // The payout has gone out of service, the host can attempt to re-enable the payout by sending the enable payout
                    // command.
                    case CCommands.SSP_POLL_PAYOUT_OUT_OF_SERVICE:
                        break;
                    // The unit has timed out while searching for a note to payout. It reports the value dispensed before the timeout
                    // event.
                    case CCommands.SSP_POLL_TIME_OUT:
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    default:
                        break;
                }
            } //end of response[i]
            return true;
        }
        public bool DoPoll(int totalTagihan)
        {
            byte i;
            uangCount = 0;
            // If a not is to be held in escrow, send hold commands, as poll releases note.
            if (m_HoldCount > 0)
            {
                m_NoteHeld = true;
                m_HoldCount--;
                m_cmd.CommandData[0] = CCommands.SSP_CMD_HOLD;
                m_cmd.CommandDataLength = 1;
                if (!SendCommand()) return false;
                return true;
            }
            //send poll
            m_cmd.CommandData[0] = CCommands.SSP_CMD_POLL;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand()) return false;

            // store response locally so data can't get corrupted by other use of the cmd variable
            byte[] response = new byte[255];
            m_cmd.ResponseData.CopyTo(response, 0);
            byte responseLength = m_cmd.ResponseDataLength;

            //parse poll response
            ChannelData data = new ChannelData();
            for (i = 1; i < responseLength; ++i)
            {
                switch (response[i])
                {
                    // This response indicates that the unit was reset and this is the first time a poll
                    // has been called since the reset.
                    case CCommands.SSP_POLL_SLAVE_RESET:
                        UpdateData();
                        break;
                    // A note is currently being read by the validator sensors. The second byte of this response
                    // is zero until the note's type has been determined, it then changes to the channel of the 
                    // scanned note.
                    case CCommands.SSP_POLL_READ_NOTE:
                        if (m_cmd.ResponseData[i + 1] > 0)
                        {
                            GetDataByChannel(response[i + 1], ref data);
                            OurUtility.Write_Log("Uang Yang Dimasukkan : " + CHelper.FormatToCurrency(data.Value), "step-action");
                            if ((data.Value / 100) < 10000 || (total + data.Value/100)>totalTagihan)
                            {
                                OurUtility.Write_Log("Uang Dikembalikan", "step-action");
                                ReturnNote();
                            }
                            // log.AppendText("Nilai " + nilai_value+ "\r\n");
                            // log.AppendText("Uang masuk - uang harus bayar : " + CHelpers.FormatToCurrency(data.Value) + " - " + jmlBayar + "\r\n") ;
                            m_HoldCount = m_HoldNumber;
                        }
                        else
                            i++;
                        break;
                    // A credit event has been detected, this is when the validator has accepted a note as legal currency.
                    case CCommands.SSP_POLL_CREDIT_NOTE:
                        GetDataByChannel(response[i + 1], ref data);
                        OurUtility.Write_Log("Uang Yang Diterima: " + CHelper.FormatToCurrency(data.Value), "step-action");
                        UangMasukSekarang = data.Value / 100;
                        total += UangMasukSekarang;
                        uangCount += 1;
                        OurUtility.Write_Log("Total Uang Yang Diterima : " + total, "step-action");
                        OurUtility.Write_Log("UangMasukSekarang : " + UangMasukSekarang, "step-action");
                        UpdateData();
                        i++;
                        return true;
                        break;
                    // A note is being rejected from the validator. This will carry on polling while the note is in transit.
                    case CCommands.SSP_POLL_NOTE_REJECTING:
                        break;
                    // A note has been rejected from the validator, the note will be resting in the bezel. This response only
                    // appears once.
                    case CCommands.SSP_POLL_NOTE_REJECTED:
                        QueryRejection();
                        break;
                    // A note is in transit to the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKING:
                        break;
                    // A note has reached the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKED:
                        break;
                    // A safe jam has been detected. This is where the user has inserted a note and the note
                    // is jammed somewhere that the user cannot reach.
                    case CCommands.SSP_POLL_SAFE_NOTE_JAM:
                        break;
                    // An unsafe jam has been detected. This is where a user has inserted a note and the note
                    // is jammed somewhere that the user can potentially recover the note from.
                    case CCommands.SSP_POLL_UNSAFE_NOTE_JAM:
                        break;
                    // The validator is disabled, it will not execute any commands or do any actions until enabled.
                    case CCommands.SSP_POLL_DISABLED:
                        break;
                    // A fraud attempt has been detected. 
                    case CCommands.SSP_POLL_FRAUD_ATTEMPT:
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The stacker (cashbox) is full.
                    case CCommands.SSP_POLL_STACKER_FULL:
                        break;
                    // A note was detected somewhere inside the validator on startup and was rejected from the front of the
                    // unit.
                    case CCommands.SSP_POLL_NOTE_CLEARED_FROM_FRONT:
                        i++;
                        break;
                    // A note was detected somewhere inside the validator on startup and was cleared into the cashbox.
                    case CCommands.SSP_POLL_NOTE_CLEARED_TO_CASHBOX:
                        i++;
                        break;
                    // A note has been detected in the validator on startup and moved to the payout device 
                    case CCommands.SSP_POLL_NOTE_PAID_INTO_STORE_AT_POWER_UP:
                        i += 7;
                        break;
                    // A note has been detected in the validator on startup and moved to the cashbox
                    case CCommands.SSP_POLL_NOTE_PAID_INTO_STACKER_AT_POWER_UP:
                        i += 7;
                        break;
                    // The cashbox has been removed from the unit. This will continue to poll until the cashbox is replaced.
                    case CCommands.SSP_POLL_CASHBOX_REMOVED:
                        break;
                    // The cashbox has been replaced, this will only display on a poll once.
                    case CCommands.SSP_POLL_CASHBOX_REPLACED:
                        break;
                    // The payout device is in the process of emptying all its stored notes to the cashbox. This
                    // will continue to poll until the device is empty.
                    case CCommands.SSP_POLL_EMPTYING:
                        break;
                    // This single poll response indicates that the payout device has finished emptying.
                    case CCommands.SSP_POLL_EMPTIED:
                        UpdateData();
                        EnableValidator();
                        break;
                    // The payout device has encountered a jam. This will not clear until the jam has been removed and the unit
                    // reset.
                    case CCommands.SSP_POLL_JAMMED:
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // This is reported when the payout has been halted by a host command. This will report the value of
                    // currency dispensed upto the point it was halted. 
                    case CCommands.SSP_POLL_HALTED:
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // This is reported when the payout was powered down during a payout operation. It reports the original amount
                    // requested and the amount paid out up to this point for each currency.
                    case CCommands.SSP_POLL_INCOMPLETE_PAYOUT:
                        i += (byte)((response[i + 1] * 11) + 1);
                        break;
                    // This is reported when the payout was powered down during a float operation. It reports the original amount
                    // requested and the amount paid out up to this point for each currency.
                    case CCommands.SSP_POLL_INCOMPLETE_FLOAT:
                        i += (byte)((response[i + 1] * 11) + 1);
                        break;
                    // A note has been transferred from the payout unit to the stacker.
                    // Typo as in SSP protocol manual.
                    case CCommands.SSP_POLL_NOTE_TRANSFERED_TO_STACKER:
                        i += 7;
                        break;
                    // A note is resting in the bezel waiting to be removed by the user.
                    case CCommands.SSP_POLL_NOTE_HELD_IN_BEZEL:
                        i += 7;
                        break;
                    // The payout has gone out of service, the host can attempt to re-enable the payout by sending the enable payout
                    // command.
                    case CCommands.SSP_POLL_PAYOUT_OUT_OF_SERVICE:
                        break;
                    // The unit has timed out while searching for a note to payout. It reports the value dispensed before the timeout
                    // event.
                    case CCommands.SSP_POLL_TIME_OUT:
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    default:
                        break;
                }
            } //end of response[i]
            return true;
        }

        public bool SendCommand()
        {
            byte[] backup = new byte[255];
            m_cmd.CommandData.CopyTo(backup, 0);
            byte length = m_cmd.CommandDataLength;

            if (m_eSSP.SSPSendCommand(m_cmd, info) == false)
            {
                m_eSSP.CloseComPort();
                //m_comms.UpdateLog(info, true);
                return true;
            }
            //m_Comms.Updatelog(info)
            return true;
        }

        private bool CheckGenericResponses()
        {
            if (m_cmd.ResponseData[0] == CCommands.SSP_RESPONSE_OK)
                return true;
            else
            {
                switch (m_cmd.ResponseData[0])
                {
                    case CCommands.SSP_RESPONSE_COMMAND_CANNOT_BE_PROCESSED:
                        if (m_cmd.ResponseData[1] == 0x03)
                        {

                        }
                        else
                        {

                        }
                        return false;
                    case CCommands.SSP_RESPONSE_FAIL:

                        return false;
                    case CCommands.SSP_RESPONSE_KEY_NOT_SET:

                        return false;
                    case CCommands.SSP_RESPONSE_PARAMETER_OUT_OF_RANGE:

                        return false;
                    case CCommands.SSP_RESPONSE_SOFTWARE_ERROR:

                        return false;
                    case CCommands.SSP_RESPONSE_COMMAND_NOT_KNOWN:

                        return false;
                    case CCommands.SSP_RESPONSE_WRONG_NO_PARAMETERS:

                        return false;
                    default:
                        return false;
                }
            }
        }

        public void CekStatus()
        {
            byte[] response = new byte[255];
            m_cmd.ResponseData.CopyTo(response, 0);
            byte responseLength = m_cmd.ResponseDataLength;
            byte i;
            for (i = 0; i < responseLength; ++i)
            {
                //Console.WriteLine(response[i]);
            }
        }

        /* Non-Command functions */

        // Returns a nicely formatted string of the values and currencies of notes stored in each channel
        public string GetChannelLevelInfo()
        {
            string s = "";
            foreach (ChannelData d in m_UnitDataList)
            {
                s += (d.Value / 100f).ToString() + " " + d.Currency[0] + d.Currency[1] + d.Currency[2];
                s += " [" + d.Level + "] = " + ((d.Level * d.Value) / 100f).ToString();
                s += " " + d.Currency[0] + d.Currency[1] + d.Currency[2] + "\r\n";
            }
            return s;
        }

        // Updates all the data in the list.
        public void UpdateData()
        {
            foreach (ChannelData d in m_UnitDataList)
            {
                d.Level = CheckNoteLevel(d.Value, d.Currency);
                IsNoteRecycling(d.Value, d.Currency, ref d.Recycling);
            }
        }

        // This allows the caller to access all the data stored with a channel. An empty ChannelData struct is passed
        // over which gets filled with info.
        public void GetDataByChannel(int channel, ref ChannelData d)
        {
            // Iterate through each 
            foreach (ChannelData dList in m_UnitDataList)
            {
                if (dList.Channel == channel) // Compare channel
                {
                    d = dList; // Copy data from list to param
                    break;
                }
            }
        }

        /* Exception and Error Handling */

        // This is used for generic response error catching, it outputs the info in a
        // meaningful way.

    }
}
