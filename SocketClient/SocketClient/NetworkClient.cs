using System;
using System.Collections.Generic;
using System.Text;

namespace SocketClient
{
    ///Класс содержащий данные о клиенте необходимые в проекте
    ///
    public class NetworkClient
    {
        protected static NetworkClient _instance;

        public static NetworkClient Instance => _instance;

        /// <summary>
        /// Клиент для отправки данных, эта переменная обязательна
        /// </summary>
        public Client Network;
        
        /// <summary>
        /// Буффер для хранения отправленных пакетов во время разрыва соединения
        /// </summary>
        private Queue<byte[]> WaitPacketBuffer { get; set; }

        /// <summary>
        /// Инициализация хранилища пакетов во время разрыва соединения
        /// </summary>
        public void InitializeWaitPacketBuffer()
        {
            if (WaitPacketBuffer == null)
                WaitPacketBuffer = new Queue<byte[]>();
        }

        /// <summary>
        /// Добавить пакет в список ожидания восстановления подключения
        /// </summary>
        /// <param name="packet_data"></param>
        /// <param name="lenght"></param>
        internal void AddWaitPacket(byte[] packet_data, int lenght)
        {
            if (WaitPacketBuffer == null)
                return;
            Array.Resize(ref packet_data, lenght);
            WaitPacketBuffer.Enqueue(packet_data);
        }

        /// <summary>
        /// Получить пакет из списка ожидания
        /// </summary>
        /// <returns></returns>
        public byte[] GetWaitPacket()
        {
            if (WaitPacketBuffer == null || WaitPacketBuffer.Count == 0)
                return null;
            return WaitPacketBuffer.Dequeue();
        }

        /// <summary>
        /// Копировать пакеты в буффер с новым подключением
        /// </summary>
        /// <param name="other_client"></param>
        public void CopyWaitPacketBuffer(NetworkClient other_client)
        {
            other_client.WaitPacketBuffer = WaitPacketBuffer;
        }

        public void ClearWaitPacketBuffer()
        {
            if (WaitPacketBuffer != null)
                WaitPacketBuffer.Clear();
        }

        public string Session;

        /// <summary>
        /// Указывает является ли соединение не восстанавливаемым (в случае с неверным паролем, или аккаунтом уже в игре)
        /// </summary>
        public bool NoReconnectDisconnect = true;

        public bool FastFightState = false;

        public string[] RefreshKeys = null;

        public string LoginName = null;

        public string Password = null;
    }
}
