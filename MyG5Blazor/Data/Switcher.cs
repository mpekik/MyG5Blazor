using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace MyG5Blazor.Data
{
    public class Switcher
    {
        private Card_Dispenser m_active_Dispenser = null;
        private Card_Reader m_active_Reader = null;

        public Card_Dispenser Get_Active_Dispenser()
        { return m_active_Dispenser; }

        public Card_Reader Get_Active_Reader()
        { return m_active_Reader; }

        public bool Light_Switch_On()
        { return false; }

        public bool Light_Switch_Off()
        { return false; }

        public static void UP(string p_delay,string _port)
        {
            int delay = OurUtility.ToInt32(p_delay);

            OurUtility.Write_Log("ykushcmd.exe UP port: "+_port+" [delay : " + delay.ToString() + "]", "step-action");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "ykushcmd.exe";
                process.StartInfo.Arguments = "-u "+_port;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                Thread.Sleep(delay);
            }
            catch { }
        }

        public static void DOWN(string p_delay,string _port)
        {
            int delay = OurUtility.ToInt32(p_delay);

            OurUtility.Write_Log("ykushcmd.exe DOWN port "+_port+" [delay : " + delay.ToString() + "]", "step-action");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "ykushcmd.exe";
                process.StartInfo.Arguments = "-d " + _port;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                Thread.Sleep(delay);
            }
            catch { }
        }

    }
}
