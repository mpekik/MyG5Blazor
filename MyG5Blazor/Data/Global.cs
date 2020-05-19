using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyG5Blazor.Data
{
    public class Global
    {
        public string ComPort = "COM";
        public byte SSPAddress = 0;
        Config config = new Config();
        public Global()
        {
            ComPort = ComPort + config.Read("BILL",Config.PARAM_BILL_PORT);
        }
    }
}
