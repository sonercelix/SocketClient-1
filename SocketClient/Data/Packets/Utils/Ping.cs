using SocketClient.Utils.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets.Utils
{
    public class Ping
    {
        public static void Run(SocketClient.NetworkClient client, InputPacketBuffer data, out OutputPacketBuffer output)
        {
            output = null;
            Network.networkClient.ping = false;
        }
    }
}
