using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyG5Blazor.Data
{
    public class Paper
    {
        public static void Set(string p_value, ref Config p_config)
        {
            // Always refresh Config
            p_config.Init();

            // save
            p_config.Write("Printer", Config.PARAM_PAPER_LENGTH, p_value);

            // logging
            //Log("add_stock", p_dispenser, ref p_config);
        }

        public static string Read(ref Config p_config)
        {
            // Always refresh Config
            p_config.Init();

            return p_config.Read("Printer", Config.PARAM_PAPER_LENGTH);
        }

        public static void Decrease(int p_usage, ref Config p_config)
        {
            string x = Read(ref p_config);

            int y = OurUtility.ToInt32(x);
            y = y - 73;

            // save
            p_config.Write("Printer", Config.PARAM_PAPER_LENGTH, y.ToString());

            // logging
            //Log(p_activityType, p_dispenser, ref p_config);
        }

        // p_activityType : [add_stock
        //                      , eject_to_front1, eject_to_front2
        //                      , eject_to_bin1, eject_to_bin2, eject_to_bin3, ]
        private static void Log(string p_activityType, string p_dispenser, ref Config p_config)
        {

        }
    }
}
