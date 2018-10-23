using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
    public class SocketClient : Client
    {

        Socket client;
        public bool ping = false;

        public SocketClient(ClientOptions options) : base(options)
        {
            //System.Threading.Tasks.Task.Run(() => { Ping(); });
        }

        public void Connect()
        {
            try
            {
                client = new Socket(clientOptions.AddressFamily, SocketType.Stream, clientOptions.ProtocolType);
                client.Connect(clientOptions.IpAddress, clientOptions.Port);

                var old_client = NetworkClient.Instance;

                Reconnect(client);

                NetworkClient.Instance.InitializeWaitPacketBuffer();

                if (old_client != null)
                {
                    old_client.CopyWaitPacketBuffer(NetworkClient.Instance);
                }
            }
            catch (Exception ex)
            {
                clientOptions.RunExtension(ex, client);
                clientOptions.RunClientDisconnect(null);
            }
        }

        private void Ping()
        {
            while (true)
            {
                ping = true;

                while (!ping)
                {
                    ping = true;
                    System.Threading.Thread.Sleep(5000); // 5s
                }
                System.Threading.Thread.Sleep(2000); // 2s

                if (!ping)
                    continue;

                try
                {
                    if (client != null && client.Connected)
                    {
                        client.Poll(1, SelectMode.SelectRead);
                    }
                }
                catch
                {

                }

            }
        }
    }
}
