using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITLlib;
using MyG5Blazor.Data.Bill;

namespace MyG5Blazor.Data
{
    public class BillAcceptor
    {
        SSPComms m_eSSP;
        SSP_COMMAND m_cmd;
        SSP_KEYS keys;
        SSP_FULL_KEY sspKey;
        SSP_COMMAND_INFO info;

        int m_ProtocolVersion;
        int m_HoldCount;
        int m_NumberOfChannels;
        int m_ValueMultiplier;
        char m_UnitType;
        int m_NumStackedNotes;
        int m_HoldNumber;
        bool m_NoteHeld;

        List<ChannelData> m_UnitDataList;

        public BillAcceptor()
        {
            m_eSSP = new SSPComms();
            m_cmd = new SSP_COMMAND();
            keys = new SSP_KEYS();
            sspKey = new SSP_FULL_KEY();
            info = new SSP_COMMAND_INFO();

            //m_Comms = new CCommsWindow("NoteValidator");
            m_NumberOfChannels = 0;
            m_ValueMultiplier = 1;
            m_UnitType = (char)0xFF; // 0xFF is bank_validator
            m_UnitDataList = new List<ChannelData>();
            m_HoldCount = 0;
            m_HoldNumber = 0;
        }

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

        public char UnitType
        {
            get { return m_UnitType; }
        }

        public int NumberOfNotesStacked
        {
            get { return m_NumStackedNotes; }
            set { m_NumStackedNotes = value; }
        }

        public int HoldNumber
        {
            get { return m_HoldNumber; }
            set { m_HoldNumber = value; }

        }

        public int GetChannelValue(int channelNum)
        {
            if (channelNum >= 1 && channelNum <= m_NumberOfChannels)
            {
                foreach (ChannelData d in m_UnitDataList)
                {
                    if (d.Channel == channelNum)
                        return d.Value;
                }
            }
            return -1;
        }

        public string GetChannelCurrency(int channelNum)
        {
            if (channelNum >= 1 && channelNum <= m_NumberOfChannels)
            {
                foreach (ChannelData d in m_UnitDataList)
                {
                    if (d.Channel == channelNum)
                        return new string(d.Currency);
                }
            }
            return "";
        }

        public void EnableValidator(string log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            // check response
            if (CheckGenericResponses(log) && log != null)
                OurUtility.Write_Log("Unit enabled\r\n", "step-action");
        }

        public void DisableValidator(string log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_DISABLE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            // check response
            if (CheckGenericResponses(log) && log != null)
                OurUtility.Write_Log("Unit disabled\r\n", "step-action");
        }

        public void ReturnNote(string log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_REJECT_BANKNOTE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log))
            {
                if (log != null)
                {
                    OurUtility.Write_Log("Returning note\r\n", "step-action");
                }
                m_HoldCount = 0;
            }
        }

        public void Reset(string log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_RESET;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log))
                OurUtility.Write_Log("Resetting unit\r\n", "step-sction");
        }

        public void SetProtocolVersion(byte pVersion, string log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_HOST_PROTOCOL_VERSION;
            m_cmd.CommandData[1] = pVersion;
            m_cmd.CommandDataLength = 2;
            if (!SendCommand(log)) return;
        }

        public void QueryRejection(string log)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_LAST_REJECT_CODE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log))
            {
                if (log == null) return;
                switch (m_cmd.ResponseData[1])
                {
                    case 0x00: OurUtility.Write_Log("Note accepted\r\n", "step-sction"); break;
                    case 0x01: OurUtility.Write_Log("Note length incorrect\r\n", "step-sction"); break;
                    case 0x02: OurUtility.Write_Log("Invalid note\r\n", "step-sction"); break;
                    case 0x03: OurUtility.Write_Log("Invalid note\r\n", "step-sction"); break;
                    case 0x04: OurUtility.Write_Log("Invalid note\r\n", "step-sction"); break;
                    case 0x05: OurUtility.Write_Log("Invalid note\r\n", "step-sction"); break;
                    case 0x06: OurUtility.Write_Log("Channel inhibited\r\n", "step-sction"); break;
                    case 0x07: OurUtility.Write_Log("Second note inserted during read\r\n", "step-sction"); break;
                    case 0x08: OurUtility.Write_Log("Host rejected note\r\n", "step-sction"); break;
                    case 0x09: OurUtility.Write_Log("Invalid note\r\n", "step-sction"); break;
                    case 0x0A: OurUtility.Write_Log("Invalid note read\r\n", "step-sction"); break;
                    case 0x0B: OurUtility.Write_Log("Note too long\r\n", "step-sction"); break;
                    case 0x0C: OurUtility.Write_Log("Validator disabled\r\n", "step-sction"); break;
                    case 0x0D: OurUtility.Write_Log("Mechanism slow/stalled\r\n", "step-sction"); break;
                    case 0x0E: OurUtility.Write_Log("Strim attempt\r\n", "step-sction"); break;
                    case 0x0F: OurUtility.Write_Log("Fraud channel reject\r\n", "step-sction"); break;
                    case 0x10: OurUtility.Write_Log("No notes inserted\r\n", "step-sction"); break;
                    case 0x11: OurUtility.Write_Log("Invalid note read\r\n", "step-sction"); break;
                    case 0x12: OurUtility.Write_Log("Twisted note detected\r\n", "step-sction"); break;
                    case 0x13: OurUtility.Write_Log("Escrow time-out\r\n", "step-sction"); break;
                    case 0x14: OurUtility.Write_Log("Bar code scan fail\r\n", "step-sction"); break;
                    case 0x15: OurUtility.Write_Log("Invalid note read\r\n", "step-sction"); break;
                    case 0x16: OurUtility.Write_Log("Invalid note read\r\n", "step-sction"); break;
                    case 0x17: OurUtility.Write_Log("Invalid note read\r\n", "step-sction"); break;
                    case 0x18: OurUtility.Write_Log("Invalid note read\r\n", "step-sction"); break;
                    case 0x19: OurUtility.Write_Log("Incorrect note width\r\n", "step-sction"); break;
                    case 0x1A: OurUtility.Write_Log("Note too short\r\n", "step-sction"); break;
                }
            }
        }

        public bool NegotiateKeys(string log = null)
        {
            // make sure encryption is off
            m_cmd.EncryptionStatus = false;

            // send sync
            if (log != null) OurUtility.Write_Log("Syncing... ", "step-action");
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SYNC;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return false;
            if (log != null) OurUtility.Write_Log("Success", "step-action");

            m_eSSP.InitiateSSPHostKeys(keys, m_cmd);

            // send generator
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_GENERATOR;
            m_cmd.CommandDataLength = 9;
            if (log != null) OurUtility.Write_Log("Setting generator... ", "step-action");

            // Convert generator to bytes and add to command data.
            BitConverter.GetBytes(keys.Generator).CopyTo(m_cmd.CommandData, 1);

            if (!SendCommand(log)) return false;
            if (log != null) OurUtility.Write_Log("Success\r\n", "step-action");

            // send modulus
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_MODULUS;
            m_cmd.CommandDataLength = 9;
            if (log != null) OurUtility.Write_Log("Sending modulus... ", "step-action");

            // Convert modulus to bytes and add to command data.
            BitConverter.GetBytes(keys.Modulus).CopyTo(m_cmd.CommandData, 1);

            if (!SendCommand(log)) return false;
            if (log != null) OurUtility.Write_Log("Success\r\n", "step-action");

            // send key exchange
            m_cmd.CommandData[0] = CCommands.SSP_CMD_REQUEST_KEY_EXCHANGE;
            m_cmd.CommandDataLength = 9;
            if (log != null) OurUtility.Write_Log("Exchanging keys... ", "step-action");

            // Convert host intermediate key to bytes and add to command data.
            BitConverter.GetBytes(keys.HostInter).CopyTo(m_cmd.CommandData, 1);


            if (!SendCommand(log)) return false;
            if (log != null) OurUtility.Write_Log("Success\r\n", "step-action");

            // Read slave intermediate key.
            keys.SlaveInterKey = BitConverter.ToUInt64(m_cmd.ResponseData, 1);

            m_eSSP.CreateSSPHostEncryptionKey(keys);

            // get full encryption key
            m_cmd.Key.FixedKey = 0x0123456701234567;
            m_cmd.Key.VariableKey = keys.KeyHost;

            if (log != null) OurUtility.Write_Log("Keys successfully negotiated\r\n", "step-action");

            return true;
        }

        public void SetInhibits(string log = null)
        {
            // set inhibits
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_CHANNEL_INHIBITS;
            m_cmd.CommandData[1] = 0xFF;
            m_cmd.CommandData[2] = 0xFF;
            m_cmd.CommandDataLength = 3;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null)
            {
                OurUtility.Write_Log("Inhibits set\r\n", "step-action");
            }
        }

        public bool DoPoll(string log)
        {
            byte i;
            // If a not is to be held in escrow, send hold commands, as poll releases note.
            if (m_HoldCount > 0)
            {
                m_NoteHeld = true;
                m_HoldCount--;
                m_cmd.CommandData[0] = CCommands.SSP_CMD_HOLD;
                m_cmd.CommandDataLength = 1;
                OurUtility.Write_Log("Note held in escrow: " + m_HoldCount + "\r\n", "step-action");
                if (!SendCommand(log)) return false;
                return true;
            }
            //send poll
            m_cmd.CommandData[0] = CCommands.SSP_CMD_POLL; // jika hold count habis uang akan di poll
            m_cmd.CommandDataLength = 1;
            m_NoteHeld = false;

            if (!SendCommand(log)) return false;

            //parse poll response
            int noteVal = 0;
            for (i = 1; i < m_cmd.ResponseDataLength; i++)
            {
                switch (m_cmd.ResponseData[i])
                {
                    // This response indicates that the unit was reset and this is the first time a poll
                    // has been called since the reset.
                    case CCommands.SSP_POLL_SLAVE_RESET:
                        OurUtility.Write_Log("Unit reset\r\n", "step-action");
                        break;
                    // A note is currently being read by the validator sensors. The second byte of this response
                    // is zero until the note's type has been determined, it then changes to the channel of the 
                    // scanned note.
                    case CCommands.SSP_POLL_READ_NOTE:
                        if (m_cmd.ResponseData[i + 1] > 0)
                        {
                            noteVal = GetChannelValue(m_cmd.ResponseData[i + 1]);
                            OurUtility.Write_Log("Note in escrow, amount: " + CHelpers.FormatToCurrency(noteVal) + " " + GetChannelCurrency(m_cmd.ResponseData[i + 1]) + "\r\n", "step-action");
                            m_HoldCount = m_HoldNumber;
                        }
                        else
                            OurUtility.Write_Log("Reading note...\r\n", "step-action");
                        i++;
                        break;
                    // A credit event has been detected, this is when the validator has accepted a note as legal currency.
                    case CCommands.SSP_POLL_CREDIT_NOTE:
                        noteVal = GetChannelValue(m_cmd.ResponseData[i + 1]);
                        OurUtility.Write_Log("Credit " + CHelpers.FormatToCurrency(noteVal) + " " + GetChannelCurrency(m_cmd.ResponseData[i + 1]) + "\r\n", "step-action");
                        m_NumStackedNotes++;
                        i++;
                        break;
                    // A note is being rejected from the validator. This will carry on polling while the note is in transit.
                    case CCommands.SSP_POLL_NOTE_REJECTING:
                        OurUtility.Write_Log("Rejecting note...\r\n", "step-action");
                        break;
                    // A note has been rejected from the validator, the note will be resting in the bezel. This response only
                    // appears once.
                    case CCommands.SSP_POLL_NOTE_REJECTED:
                        OurUtility.Write_Log("Note rejected\r\n", "step-action");
                        QueryRejection(log);
                        break;
                    // A note is in transit to the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKING:
                        OurUtility.Write_Log("Stacking note...\r\n", "step-action");
                        break;
                    // A note has reached the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKED:
                        OurUtility.Write_Log("Note stacked\r\n", "step-action");
                        break;
                    // A safe jam has been detected. This is where the user has inserted a note and the note
                    // is jammed somewhere that the user cannot reach.
                    case CCommands.SSP_POLL_SAFE_NOTE_JAM:
                        OurUtility.Write_Log("Safe jam\r\n", "step-action");
                        break;
                    // An unsafe jam has been detected. This is where a user has inserted a note and the note
                    // is jammed somewhere that the user can potentially recover the note from.
                    case CCommands.SSP_POLL_UNSAFE_NOTE_JAM:
                        OurUtility.Write_Log("Unsafe jam\r\n", "step-action");
                        break;
                    // The validator is disabled, it will not execute any commands or do any actions until enabled.
                    case CCommands.SSP_POLL_DISABLED:
                        break;
                    // A fraud attempt has been detected. The second byte indicates the channel of the note that a fraud
                    // has been attempted on.
                    case CCommands.SSP_POLL_FRAUD_ATTEMPT:
                        OurUtility.Write_Log("Fraud attempt, note type: " + GetChannelValue(m_cmd.ResponseData[i + 1]) + "\r\n", "step-action");
                        i++;
                        break;
                    // The stacker (cashbox) is full. 
                    case CCommands.SSP_POLL_STACKER_FULL:
                        OurUtility.Write_Log("Stacker full\r\n", "step-action");
                        break;
                    // A note was detected somewhere inside the validator on startup and was rejected from the front of the
                    // unit.
                    case CCommands.SSP_POLL_NOTE_CLEARED_FROM_FRONT:
                        OurUtility.Write_Log(GetChannelValue(m_cmd.ResponseData[i + 1]) + " note cleared from front at reset." + "\r\n", "step-action");
                        i++;
                        break;
                    // A note was detected somewhere inside the validator on startup and was cleared into the cashbox.
                    case CCommands.SSP_POLL_NOTE_CLEARED_TO_CASHBOX:
                        OurUtility.Write_Log(GetChannelValue(m_cmd.ResponseData[i + 1]) + " note cleared to stacker at reset." + "\r\n", "step-action");
                        i++;
                        break;
                    // The cashbox has been removed from the unit. This will continue to poll until the cashbox is replaced.
                    case CCommands.SSP_POLL_CASHBOX_REMOVED:
                        OurUtility.Write_Log("Cashbox removed...\r\n", "step-action");
                        break;
                    // The cashbox has been replaced, this will only display on a poll once.
                    case CCommands.SSP_POLL_CASHBOX_REPLACED:
                        OurUtility.Write_Log("Cashbox replaced\r\n", "step-action");
                        break;
                    // A bar code ticket has been detected and validated. The ticket is in escrow, continuing to poll will accept
                    // the ticket, sending a reject command will reject the ticket.
                    //case CCommands.SSP_POLL_BAR_CODE_VALIDATED:
                    //    OurUtility.Write_Log("Bar code ticket validated\r\n");
                    //    break;
                    //// A bar code ticket has been accepted (equivalent to note credit).
                    //case CCommands.SSP_POLL_BAR_CODE_ACK:
                    //    OurUtility.Write_Log("Bar code ticket accepted\r\n");
                    //    break;
                    // The validator has detected its note path is open. The unit is disabled while the note path is open.
                    // Only available in protocol versions 6 and above.
                    case CCommands.SSP_POLL_NOTE_PATH_OPEN:
                        OurUtility.Write_Log("Note path open\r\n", "step-action");
                        break;
                    // All channels on the validator have been inhibited so the validator is disabled. Only available on protocol
                    // versions 7 and above.
                    case CCommands.SSP_POLL_CHANNEL_DISABLE:
                        OurUtility.Write_Log("All channels inhibited, unit disabled\r\n", "step-action");
                        break;
                    default:
                        OurUtility.Write_Log("Unrecognised poll response detected " + (int)m_cmd.ResponseData[i] + "\r\n", "step-action");
                        break;
                }
            }
            return true;
        }

        public bool SendCommand(string log)
        {
            // Backup data and length in case we need to retry
            byte[] backup = new byte[255];
            m_cmd.CommandData.CopyTo(backup, 0);
            byte length = m_cmd.CommandDataLength;

            // attempt to send the command
            if (m_eSSP.SSPSendCommand(m_cmd, info) == false)
            {
                m_eSSP.CloseComPort();
                OurUtility.Write_Log(info.ToString(), "step-action"); // update the log on fail as well
                if (log != null) OurUtility.Write_Log("Sending command failed\r\nPort status: " + m_cmd.ResponseStatus.ToString() + "\r\n", "step-action");
                return false;
            }

            // update the log after every command
            // m_Comms.UpdateLog(info);
            OurUtility.Write_Log(info.ToString(), "step-action");
            return true;
        }

        public bool OpenComPort(string log = null)
        {
            if (log != null)
                OurUtility.Write_Log("Opening com port\r\n","step-action");
            if (!m_eSSP.OpenSSPComPort(m_cmd))
            {
                return false;
            }
            return true;
        }

        private bool CheckGenericResponses(string log)
        {
            if (m_cmd.ResponseData[0] == CCommands.SSP_RESPONSE_OK)
                return true;
            else
            {
                if (log != null)
                {
                    switch (m_cmd.ResponseData[0])
                    {
                        case CCommands.SSP_RESPONSE_COMMAND_CANNOT_BE_PROCESSED:
                            if (m_cmd.ResponseData[1] == 0x03)
                            {
                                OurUtility.Write_Log("Validator has responded with \"Busy\", command cannot be processed at this time\r\n", "step-action");
                            }
                            else
                            {
                                OurUtility.Write_Log("Command response is CANNOT PROCESS COMMAND, error code - 0x"
                                + BitConverter.ToString(m_cmd.ResponseData, 1, 1) + "\r\n", "step-action");
                            }
                            return false;
                        case CCommands.SSP_RESPONSE_FAIL:
                            OurUtility.Write_Log("Command response is FAIL\r\n", "step-action");
                            return false;
                        case CCommands.SSP_RESPONSE_KEY_NOT_SET:
                            OurUtility.Write_Log("Command response is KEY NOT SET, Validator requires encryption on this command or there is"
                                + "a problem with the encryption on this request\r\n", "step-action");
                            return false;
                        case CCommands.SSP_RESPONSE_PARAMETER_OUT_OF_RANGE:
                            OurUtility.Write_Log("Command response is PARAM OUT OF RANGE\r\n", "step-action");
                            return false;
                        case CCommands.SSP_RESPONSE_SOFTWARE_ERROR:
                            OurUtility.Write_Log("Command response is SOFTWARE ERROR\r\n", "step-action");
                            return false;
                        case CCommands.SSP_RESPONSE_COMMAND_NOT_KNOWN:
                            OurUtility.Write_Log("Command response is UNKNOWN\r\n", "step-action");
                            return false;
                        case CCommands.SSP_RESPONSE_WRONG_NO_PARAMETERS:
                            OurUtility.Write_Log("Command response is WRONG PARAMETERS\r\n", "step-action");
                            return false;
                        default:
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
