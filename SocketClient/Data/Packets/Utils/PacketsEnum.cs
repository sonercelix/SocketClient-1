
using SocketClient;
using System;
using System.Collections.Generic;
using System.Text;
using static SocketClient.ClientOptions;

namespace Packets.Utils
{
    /// <summary>
    /// Входящие индификаторы пакетов
    /// </summary>
    public enum InputPacketsEnum : ushort
    {
        #region Utils
        Ping
        #endregion
    }

    /// <summary>
    /// Исходящие идентификаторы пакетов
    /// </summary>
    public enum OutputPacketsEnum : ushort
    {
    
    }

    public static class AddEnumPacket
    {
        /// <summary>
        /// Добавить пакет для обработки сервером
        /// </summary>
        /// <param name="packetId">Индификатор пакета в системе</param>
        /// <param name="packet">Обработчик пакета</param>
        /// <returns></returns>
        public static bool AddPacket(this ClientOptions options, InputPacketsEnum packetId, PacketHandle packet)
        {
            var r = options.Packets.ContainsKey((ushort)packetId);
            if (!r)
                options.Packets.Add((ushort)packetId, packet);
            return !r;
        }
    }
}
