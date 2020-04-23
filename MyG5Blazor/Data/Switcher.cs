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

        public static void UP_eKTP(string p_delay)
        {
            int delay = OurUtility.ToInt32(p_delay);

            OurUtility.Write_Log("ykushcmd.exe UP untuk ekTP [delay : " + delay.ToString() + "]", "step-action");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "ykushcmd.exe";
                process.StartInfo.Arguments = "-u 3";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                Thread.Sleep(delay);
            }
            catch { }
        }

        public static void DOWN_eKTP(string p_delay)
        {
            int delay = OurUtility.ToInt32(p_delay);

            OurUtility.Write_Log("ykushcmd.exe DOWN untuk ekTP [delay : " + delay.ToString() + "]", "step-action");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "ykushcmd.exe";
                process.StartInfo.Arguments = "-d 3";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                Thread.Sleep(delay);
            }
            catch { }
        }

        public static void UP_Reader_1(string p_delay)
        {
            int delay = OurUtility.ToInt32(p_delay);

            OurUtility.Write_Log("ykushcmd.exe UP untuk Reader 1 [delay : " + delay.ToString() + "]", "step-action");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "ykushcmd.exe";
                process.StartInfo.Arguments = "-u 1";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                Thread.Sleep(delay);
            }
            catch { }
        }

        public static void DOWN_Reader_1(string p_delay)
        {
            int delay = OurUtility.ToInt32(p_delay);

            OurUtility.Write_Log("ykushcmd.exe DOWN untuk Reader 1 [delay : " + delay.ToString() + "]", "step-action");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "ykushcmd.exe";
                process.StartInfo.Arguments = "-d 1";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                Thread.Sleep(delay);
            }
            catch { }
        }

        public static void UP_Reader_2(string p_delay)
        {
            int delay = OurUtility.ToInt32(p_delay);

            OurUtility.Write_Log("ykushcmd.exe UP untuk Reader 2 [delay : " + delay.ToString() + "]", "step-action");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "ykushcmd.exe";
                process.StartInfo.Arguments = "-u 2";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                Thread.Sleep(delay);
            }
            catch { }
        }

        public static void DOWN_Reader_2(string p_delay)
        {
            int delay = OurUtility.ToInt32(p_delay);

            OurUtility.Write_Log("ykushcmd.exe DOWN untuk Reader 2 [delay : " + delay.ToString() + "]", "step-action");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "ykushcmd.exe";
                process.StartInfo.Arguments = "-d 2";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                Thread.Sleep(delay);
            }
            catch { }
        }

        public static void Startup(string p_delay)
        {
            DOWN_Reader_1(p_delay);
            DOWN_eKTP(p_delay);
        }

    }
}
