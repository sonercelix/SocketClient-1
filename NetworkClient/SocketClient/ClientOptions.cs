using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using SocketClient.Utils;

namespace SocketClient
{
    public class ClientOptions
    {
        /// <summary>
        /// Делегат для регистрации пакета
        /// </summary>
        /// <param name="client">Данные клиента</param>
        /// <param name="data">Входящий буффер с данными</param>
        /// <param name="output">Исходящий буффер с данными(не обязательно)</param>
        public delegate void PacketHandle(NetworkClient client, Utils.Buffer.InputPacketBuffer data, out Utils.Buffer.OutputPacketBuffer output);

        /// <summary>
        /// Делегат для регистрации события перехвата сетевых ошибок
        /// </summary>
        /// <param name="ex">Возникшая ошибка</param>
        /// <param name="s">Сокет с которым произошла ошибка</param>
        public delegate void ExtensionHandle(Exception ex, Socket s);

        /// <summary>
        /// Делегат для регистрации события перехвата подключения клиента
        /// </summary>
        /// <param name="client">Данные клиента</param>
        public delegate void ClientConnect(NetworkClient client);

        /// <summary>
        /// Делегат для регистрации события перехвата отключения клиента
        /// </summary>
        /// <param name="client">Данные клиента</param>
        public delegate void ClientDisconnect(NetworkClient client);

        /// <summary>
        /// События вызываемое при получении ошибки
        /// </summary>
        public event ExtensionHandle OnExtensionEvent;

        /// <summary>
        /// Событие вызываемое при подключении клиента
        /// </summary>
        public event ClientConnect OnClientConnectEvent;

        /// <summary>
        /// Событие вызываемое при отключении клиента
        /// </summary>
        public event ClientDisconnect OnClientDisconnectEvent;

        /// <summary>
        /// Вызов события ошибка
        /// </summary>
        public void RunExtension(Exception ex, Socket s)
        {
            OnExtensionEvent?.Invoke(ex, s);
        }

        /// <summary>
        /// Вызов события подключения клиента
        /// </summary>
        /// <param name="client"></param>
        public void RunClientConnect(NetworkClient client)
        {
            OnClientConnectEvent?.Invoke(client);
        }

        /// <summary>
        /// Вызов события отключения клиента
        /// </summary>
        /// <param name="client"></param>
        public void RunClientDisconnect(NetworkClient client)
        {
            OnClientDisconnectEvent?.Invoke(client);
        }

        #region ServerSettings
        //Данные для настройки сервера

        /// <summary>
        /// Тип ип адресса, InterNetwork - IPv4, InterNetworkV6 - IPv6
        /// </summary>
        public AddressFamily AddressFamily { get; set; }

        /// <summary>
        /// Тип сервера, обычно используется Tcp/Udp
        /// </summary>
        public ProtocolType ProtocolType { get; set; }

        /// <summary>
        /// Ип для инициализации сервера на определенном адаптере (0.0.0.0 - на всех, стандартное значение)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Порт для инициализации сервера 
        /// </summary>
        public int Port { get; set; }

        #endregion

        /// <summary>
        /// Размер буффера приходящих данных, если пакет больше этого значения то данные по реализованному алгоритму принять не получиться
        /// </summary>
        public int ReceiveBufferSize { get; set; }

        /// <summary>
        /// Алгоритм шифрования входящих пакетов
        /// </summary>
        public IPacketCipher inputCipher { get; set; }

        /// <summary>
        /// Алгоритм шифрования входящих пакетов
        /// </summary>
        public IPacketCipher outputCipher { get; set; }

        /// <summary>
        /// Пакеты которые будет принимать и обрабатывать сервер
        /// </summary>
        public Dictionary<ushort, PacketHandle> Packets = new Dictionary<ushort, PacketHandle>();

        /// <summary>
        /// Добавить пакет для обработки сервером
        /// </summary>
        /// <param name="packetId">Индификатор пакета в системе</param>
        /// <param name="packet">Обработчик пакета</param>
        /// <returns></returns>
        public bool AddPacket(ushort packetId, PacketHandle packet)
        {
            var r = Packets.ContainsKey(packetId);
            if (!r)
                Packets.Add(packetId, packet);
            return !r;
        }
    }
}
