using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITLlib;

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
        //List<ChannelData> m_UnitDataList;
    }
}
