using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Globalization;

namespace MyG5Blazor.Data
{
    public class Card_Dispenser
    {
        //打开串口
        [DllImport("CRT_571.dll")]
        public static extern UInt32 CommOpen(string port);
        //按指定的波特率打开串口
        [DllImport("CRT_571.dll")]
        public static extern long CommOpenWithBaut(string port, UInt32 Baudrate);
        //关闭串口
        [DllImport("CRT_571.dll")]
        public static extern int CommClose(UInt32 ComHandle);

        //int APIENTRY ExecuteCommand(HANDLE ComHandle,BYTE TxAddr,BYTE TxCmCode,BYTE TxPmCode,int TxDataLen,BYTE TxData[],BYTE *RxReplyType,BYTE *RxStCode0,BYTE *RxStCode1,BYTE *RxStCode2,int *RxDataLen,BYTE RxData[]);
        [DllImport("CRT_571.dll")]
        public static extern int ExecuteCommand(UInt32 ComHandle, byte TxAddr, byte TxCmCode, byte TxPmCode, UInt16 TxDataLen, byte[] TxData, ref byte RxReplyType, ref byte RxStCode0, ref byte RxStCode1, ref byte RxStCode2, ref UInt16 RxDataLen, byte[] RxData);

        private UInt32 Hdle = 0;
        //private long Hdle = 0;

        private string Device_Address = "00";
        private string com_Serial = string.Empty;

        public bool Start(ref string p_errorCode, string p_com_Serial, ref string p_message)
        {
            bool result = false;

            p_message = string.Empty;
            p_errorCode = string.Empty;

            com_Serial = p_com_Serial;

            try
            {
                //Hdle = CommOpen(PortCombo.Text);
                uint x = 9600;
                Hdle = (UInt32)CommOpenWithBaut(com_Serial, x);
                if (Hdle != 0)
                {
                    p_message = "Comm. Port is Opened";
                    result = true;
                }
                else
                {
                    p_message = "Open Comm. Port Error [Port : " + com_Serial + "]";
                    p_errorCode = "OCPE"; // Open Comm Port Error
                }
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
                p_errorCode = "unknown";
            }

            return result;
        }

        public bool Stop(ref string p_message)
        {
            bool result = false;

            p_message = string.Empty;

            try
            {
                //if (Hdle != 0) // ignore this Checking. Just close connection to RS-232
                // if error happened while closing. Just ignore it.
                {
                    int i = CommClose(Hdle);
                    Hdle = 0;
                    p_message = "Comm. Port is Closed";
                }

                result = true;
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        public bool Initialize(ref string p_errorCode, ref string p_message)
        {
            bool result = false;

            p_errorCode = string.Empty;
            p_message = string.Empty;

            try
            {
                if (Hdle != 0)
                {
                    byte Addr;
                    byte Cm, Pm;
                    UInt16 TxDataLen, RxDataLen;
                    byte[] TxData = new byte[1024];
                    byte[] RxData = new byte[1024];
                    byte ReType = 0;
                    byte St0, St1, St2;

                    Cm = 0x30;
                    Pm = 0x33;
                    St0 = St1 = St2 = 0;
                    TxDataLen = 0;
                    RxDataLen = 0;

                    Addr = (byte)(byte.Parse(Device_Address.Substring(0, 2), NumberStyles.Number));
                    int i = ExecuteCommand(Hdle, Addr, Cm, Pm, TxDataLen, TxData, ref ReType, ref St0, ref St1, ref St2, ref RxDataLen, RxData);
                    if (i == 0)
                    {
                        if (ReType == 0x50)
                        {
                            p_message = "INITIALIZE OK" + "\r\n" + "Status Code : " + (char)St0 + (char)St1 + (char)St2;
                            return true;
                        }
                        else
                        {
                            p_errorCode = "" + (char)St1 + (char)St2;
                            p_message = "INITIALIZE ERROR" + "\r\n" + "Error Code:  " + (char)St1 + (char)St2;
                            return result;
                        }
                    }
                    else
                    {
                        p_errorCode = "CE";
                        p_message = "Communication Error";
                        return result;
                    }
                }
                else
                {
                    p_errorCode = "CO";
                    p_message = "Comm. port is not Opened";
                    return result;
                }
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        public bool MoveTo_The_Front(ref string p_errorCode, ref string p_message)
        {
            bool result = false;

            p_errorCode = string.Empty;
            p_message = string.Empty;

            try
            {
                if (Hdle != 0)
                {
                    byte Addr;
                    byte Cm, Pm;
                    UInt16 TxDataLen, RxDataLen;
                    byte[] TxData = new byte[1024];
                    byte[] RxData = new byte[1024];
                    byte ReType = 0;
                    byte St0, St1, St2;

                    Cm = 0x32;
                    Pm = 0x39;
                    St0 = St1 = St2 = 0;
                    TxDataLen = 0;
                    RxDataLen = 0;

                    Addr = (byte)(byte.Parse(Device_Address.Substring(0, 2), NumberStyles.Number));
                    int i = ExecuteCommand(Hdle, Addr, Cm, Pm, TxDataLen, TxData, ref ReType, ref St0, ref St1, ref St2, ref RxDataLen, RxData);
                    if (i == 0)
                    {
                        if (ReType == 0x50)
                        {
                            p_message = "Move Card OK" + "\r\n" + "Status Code : " + (char)St0 + (char)St1 + (char)St2;
                            return true;
                        }
                        else
                        {
                            p_errorCode = "" + (char)St1 + (char)St2;
                            p_message = "Move Card ERROR" + "\r\n" + "Error Code:  " + (char)St1 + (char)St2;
                            return result;
                        }
                    }
                    else
                    {
                        p_errorCode = "CE";
                        p_message = "Communication Error";
                        return result;
                    }
                }
                else
                {
                    p_errorCode = "CO";
                    p_message = "Comm. port is not Opened";
                    return result;
                }
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        public bool MoveTo_The_Front_Hold(ref string p_errorCode, ref string p_message)
        {
            bool result = false;

            p_errorCode = string.Empty;
            p_message = string.Empty;

            try
            {
                if (Hdle != 0)
                {
                    byte Addr;
                    byte Cm, Pm;
                    UInt16 TxDataLen, RxDataLen;
                    byte[] TxData = new byte[1024];
                    byte[] RxData = new byte[1024];
                    byte ReType = 0;
                    byte St0, St1, St2;

                    Cm = 0x32;
                    Pm = 0x30;
                    St0 = St1 = St2 = 0;
                    TxDataLen = 0;
                    RxDataLen = 0;

                    Addr = (byte)(byte.Parse(Device_Address.Substring(0, 2), NumberStyles.Number));
                    int i = ExecuteCommand(Hdle, Addr, Cm, Pm, TxDataLen, TxData, ref ReType, ref St0, ref St1, ref St2, ref RxDataLen, RxData);
                    if (i == 0)
                    {
                        if (ReType == 0x50)
                        {
                            p_message = "Move Card OK" + "\r\n" + "Status Code : " + (char)St0 + (char)St1 + (char)St2;
                            return true;
                        }
                        else
                        {
                            p_errorCode = "" + (char)St1 + (char)St2;
                            p_message = "Move Card ERROR" + "\r\n" + "Error Code:  " + (char)St1 + (char)St2;
                            return result;
                        }
                    }
                    else
                    {
                        p_errorCode = "CE";
                        p_message = "Communication Error";
                        return result;
                    }
                }
                else
                {
                    p_errorCode = "CO";
                    p_message = "Comm. port is not Opened";
                    return result;
                }
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        public bool MoveTo_RF_Card(ref string p_errorCode, ref string p_message)
        {
            bool result = false;

            p_errorCode = string.Empty;
            p_message = string.Empty;

            try
            {
                if (Hdle != 0)
                {
                    byte Addr;
                    byte Cm, Pm;
                    UInt16 TxDataLen, RxDataLen;
                    byte[] TxData = new byte[1024];
                    byte[] RxData = new byte[1024];
                    byte ReType = 0;
                    byte St0, St1, St2;

                    Cm = 0x32;
                    Pm = 0x32;
                    St0 = St1 = St2 = 0;
                    TxDataLen = 0;
                    RxDataLen = 0;

                    Addr = (byte)(byte.Parse(Device_Address.Substring(0, 2), NumberStyles.Number));
                    int i = ExecuteCommand(Hdle, Addr, Cm, Pm, TxDataLen, TxData, ref ReType, ref St0, ref St1, ref St2, ref RxDataLen, RxData);
                    if (i == 0)
                    {
                        if (ReType == 0x50)
                        {
                            p_message = "Move Card OK" + "\r\n" + "Status Code : " + (char)St0 + (char)St1 + (char)St2;
                            return true;
                        }
                        else
                        {
                            p_errorCode = "" + (char)St1 + (char)St2;
                            p_message = "Move Card ERROR" + "\r\n" + "Error Code:  " + (char)St1 + (char)St2;
                            return result;
                        }
                    }
                    else
                    {
                        p_errorCode = "CE";
                        p_message = "Communication Error";
                        return result;
                    }
                }
                else
                {
                    p_errorCode = "CO";
                    p_message = "Comm. port is not Opened";
                    return result;
                }
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        public bool MoveTo_IC_Card(ref string p_errorCode, ref string p_message)
        {
            bool result = false;

            p_errorCode = string.Empty;
            p_message = string.Empty;

            try
            {
                if (Hdle != 0)
                {
                    byte Addr;
                    byte Cm, Pm;
                    UInt16 TxDataLen, RxDataLen;
                    byte[] TxData = new byte[1024];
                    byte[] RxData = new byte[1024];
                    byte ReType = 0;
                    byte St0, St1, St2;

                    Cm = 0x32;
                    Pm = 0x31;
                    St0 = St1 = St2 = 0;
                    TxDataLen = 0;
                    RxDataLen = 0;

                    Addr = (byte)(byte.Parse(Device_Address.Substring(0, 2), NumberStyles.Number));
                    int i = ExecuteCommand(Hdle, Addr, Cm, Pm, TxDataLen, TxData, ref ReType, ref St0, ref St1, ref St2, ref RxDataLen, RxData);
                    if (i == 0)
                    {
                        if (ReType == 0x50)
                        {
                            p_message = "Move Card OK" + "\r\n" + "Status Code : " + (char)St0 + (char)St1 + (char)St2;
                            return true;
                        }
                        else
                        {
                            p_errorCode = "" + (char)St1 + (char)St2;
                            p_message = "Move Card ERROR" + "\r\n" + "Error Code:  " + (char)St1 + (char)St2;
                            return result;
                        }
                    }
                    else
                    {
                        p_errorCode = "CE";
                        p_message = "Communication Error";
                        return result;
                    }
                }
                else
                {
                    p_errorCode = "CO";
                    p_message = "Comm. port is not Opened";
                    return result;
                }
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        public bool MoveTo_Bin(ref string p_errorCode, ref string p_message)
        {
            bool result = false;

            p_errorCode = string.Empty;
            p_message = string.Empty;

            try
            {
                if (Hdle != 0)
                {
                    byte Addr;
                    byte Cm, Pm;
                    UInt16 TxDataLen, RxDataLen;
                    byte[] TxData = new byte[1024];
                    byte[] RxData = new byte[1024];
                    byte ReType = 0;
                    byte St0, St1, St2;

                    Cm = 0x32;
                    Pm = 0x33;
                    St0 = St1 = St2 = 0;
                    TxDataLen = 0;
                    RxDataLen = 0;

                    Addr = (byte)(byte.Parse(Device_Address.Substring(0, 2), NumberStyles.Number));
                    int i = ExecuteCommand(Hdle, Addr, Cm, Pm, TxDataLen, TxData, ref ReType, ref St0, ref St1, ref St2, ref RxDataLen, RxData);
                    if (i == 0)
                    {
                        if (ReType == 0x50)
                        {
                            p_message = "Move Card OK" + "\r\n" + "Status Code : " + (char)St0 + (char)St1 + (char)St2;
                            return true;
                        }
                        else
                        {
                            p_errorCode = "" + (char)St1 + (char)St2;
                            p_message = "Move Card ERROR" + "\r\n" + "Error Code:  " + (char)St1 + (char)St2;
                            return result;
                        }
                    }
                    else
                    {
                        p_errorCode = "CE";
                        p_message = "Communication Error";
                        return result;
                    }
                }
                else
                {
                    p_errorCode = "CO";
                    p_message = "Comm. port is not Opened";
                    return result;
                }
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }

        public bool Scan_ICC(ref string p_icc, ref string p_message)
        {
            bool result = false;

            p_message = string.Empty;

            return result;
        }
    }
}
