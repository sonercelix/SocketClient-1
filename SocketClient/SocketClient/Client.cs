using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using SocketClient.Utils;
using SocketClient.Utils.Buffer;
using UnityEngine;

namespace SocketClient
{
    /// <summary>
    /// Класс обработки клиента
    /// </summary>
    public class Client : NetworkClient
    {
        //тут мы наследуемся от NetworkClient который можно использовать для хранения данных сессии, сделано это для удобства менеджмента кода, и для того что-бы программисты случайно не повредили код

        /// <summary>
        /// Криптография с помощью которой мы расшифровываем полученные данные
        /// </summary>
        private IPacketCipher inputCipher;

        /// <summary>
        /// Криптография с помощью которой мы разшифровываем данные
        /// </summary>
        private IPacketCipher outputCipher;

        /// <summary>
        /// Сокет, собственно поток для взаимодействия с пользователем
        /// </summary>
        private Socket sclient;

        /// <summary>
        /// Буффер для приема данных
        /// </summary>
        private byte[] receiveBuffer;

        /// <summary>
        /// Текущее положение в буффере, для метода BeginReceive
        /// </summary>
        private int offset;

        /// <summary>
        /// Размер читаемых данных при следующем вызове BeginReceive
        /// </summary>
        private int lenght = InputPacketBuffer.headerLenght;

        /// <summary>
        /// Общие настройки сервера
        /// </summary>
        protected ClientOptions clientOptions;

        /// <summary>
        /// Инициализация прослушивания клиента
        /// </summary>
        /// <param name="options">общие настройки сервера</param>
        public Client(ClientOptions options)
        {
            //установка переменной с общими настройками сервера
            this.clientOptions = options;

            //обзятельная переменная в NetworkClient, для отправки данных, можно использовать привидения типов (Client)NetworkClient но это никому не поможет
            this.Network = this;
        }

        /// <summary>
        /// Запуск цикла приема пакетов
        /// </summary>
        /// <param name="client">клиент</param>
        public void Reconnect(Socket client)
        {
            _instance = this;

            //установка переменной содержащую поток клиента
            this.sclient = client;

            //установка массива для приема данных, размер указан в общих настройках сервера
            this.receiveBuffer = new byte[clientOptions.ReceiveBufferSize];
            //установка криптографии для дешифровки входящих данных, указана в общих настройках сервера
            this.inputCipher = (IPacketCipher)clientOptions.inputCipher.Clone();
            //установка криптографии для шифровки исходящих данных, указана в общих настройках сервера
            this.outputCipher = (IPacketCipher)clientOptions.outputCipher.Clone();

            //Bug fix, в системе Windows это значение берется из реестра, мы не сможем принять больше за раз чем прописанно в нем, если данных будет больше, то цикл зависнет
            sclient.ReceiveBufferSize = clientOptions.ReceiveBufferSize;
            
            //Bug fix, отключение буфферизации пакетов для уменьшения трафика, если не отключить то получим фризы, в случае с игровым соединением эту опцию обычно нужно отключать
            sclient.NoDelay = true;
            sclient.ReceiveTimeout = -1;
            sclient.SendTimeout = -1;

            //Начало приема пакетов от клиента
            sclient.BeginReceive(receiveBuffer, offset, lenght, SocketFlags.None, ReceiveHeader, sclient);
            clientOptions.RunClientConnect(this.Network);
        }

        /// <summary>
        /// прием хедера пакета, который обычно содержит размер и индификатор пакета, etc
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveHeader(IAsyncResult result)
        {
            //замыкаем это все в блок try, если клиент отключился то EndReceive может вернуть ошибку
            try
            {
                //принимаем размер данных которые удалось считать
                int rlen = sclient.EndReceive(result);
                //при некоторых ошибках размер возвращает 0 или -1, проверяем
                if (rlen < 1)
                {
                    Disconnect();
                    return;
                }
                //добавляем offset для дальнейшей считки пакета
                offset += rlen;
                //если полученный размер меньше размера пакета, дополучаем данные
                if (offset < InputPacketBuffer.headerLenght)
                    sclient.BeginReceive(receiveBuffer, offset, lenght - offset, SocketFlags.None, ReceiveHeader, sclient);
                else
                {
                    var peeked = inputCipher.Peek(receiveBuffer);
                    //если все ок
                    //получаем размер пакета
                    lenght = BitConverter.ToInt32(peeked, 0);

                    global::ThreadHelper.Instance.InvokeOnMain(()=> { Debug.Log($"Receive Packet pid:{BitConverter.ToUInt16(peeked, 4)} lenght:{lenght}"); });
                    //если пакет не принимает никаких данных, бывают пустые пакеты, но такой пакет есть, запускаем исполнение
                    if (offset == lenght)
                    {
                        //обработка пакета
                        Process();
                        //перезапускаем последовательность
                        sclient.BeginReceive(receiveBuffer, offset, lenght - offset, SocketFlags.None, ReceiveHeader, sclient);
                    }
                    else
                    {
                        while (receiveBuffer.Length < lenght)
                        {
                            Array.Resize(ref receiveBuffer, receiveBuffer.Length * 2);
                            sclient.ReceiveBufferSize = receiveBuffer.Length;
                        }
                        //если все ок, запускаем считку данных тела пакета
                        sclient.BeginReceive(receiveBuffer, offset, lenght - offset, SocketFlags.None, ReceiveBody, sclient);
                    }
                }
            }
            catch (Exception ex)
            {
                clientOptions.RunExtension(ex, sclient);

                //отключаем клиента, в случае ошибки не в транспортном потоке а где-то в пакете, что-бы клиент не завис 
                Disconnect();
            }
        }

        /// <summary>
        /// Получаем тело пакета с данными
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveBody(IAsyncResult result)
        {
            //замыкаем это все в блок try, если клиент отключился то EndReceive может вернуть ошибку
            try
            {
                //принимаем размер данных которые удалось считать
                int rlen = sclient.EndReceive(result);
                //при некоторых ошибках размер возвращает 0 или -1, проверяем
                if (rlen < 1)
                {
                    Disconnect();
                    return;
                }
                //добавляем offset для дальнейшей считки пакета
                offset += rlen;

                //бывает такое что пакет разделяется на части перед отправкой и приходит по частям, возможно что пакет не дошел полностью
                if (offset != lenght)
                {
                    sclient.BeginReceive(receiveBuffer, offset, lenght - offset, SocketFlags.None, ReceiveBody, sclient);
                }
                else
                {
                    //если все ок
                    //обработка пакета
                    Process();
                    //перезапускаем последовательность
                    sclient.BeginReceive(receiveBuffer, offset, lenght - offset, SocketFlags.None, ReceiveHeader, sclient);
                }
            }
            catch (Exception ex)
            {
                clientOptions.RunExtension(ex, sclient);

                //отключаем клиента, в случае ошибки не в транспортном потоке а где-то в пакете, что-бы клиент не завис 
                Disconnect();
            }
        }

        /// <summary>
        /// Запускаем процесс обработки данных
        /// </summary>
        private void Process()
        {
            //дешефруем и засовываем это все в спец буффер в котором реализованы методы чтения типов, своего рода поток
            InputPacketBuffer pbuff = new InputPacketBuffer(inputCipher.Decode(receiveBuffer, 0, lenght));
            //Переменная для отправки данных после обработки пакета, если они есть
            OutputPacketBuffer rbuff;

            // обнуляем показатели что-бы успешно запустить цикл заново
            lenght = InputPacketBuffer.headerLenght;
            offset = 0;

            //ищем пакет и выполняем его, передаем ему данные сессии, полученные данные, и просим у него данные для передачи
            clientOptions.Packets[pbuff.PacketId](this, pbuff, out rbuff);
            //если данные для передачи есть, отправляем
            if (rbuff != null)
            {
                Send(rbuff);
            }
        }

        /// <summary>
        /// Отправка пакета
        /// </summary>
        /// <param name="rbuff">спец буффер содержащий в себе данные пакета</param>
        public void Send(OutputPacketBuffer rbuff)
        {
            global::ThreadHelper.Instance.InvokeOnMain(() => { Debug.Log($"Send Packet pid:{rbuff.PacketId} lenght:{rbuff.PacketLenght}"); });
            Send(rbuff.CompilePacket(), 0, rbuff.PacketLenght);
        }

        /// <summary>
        /// Отправка массива байт
        /// </summary>
        /// <param name="buf">массив байт</param>
        /// <param name="offset">смещение с которого начинается передача</param>
        /// <param name="lenght">размер передаваемых данных</param>
        public void Send(byte[] buf, int offset, int lenght)
        {
            try
            {
                //шифруем данные
                byte[] sndBuffer = outputCipher.Encode(buf, offset, lenght);
                //начинаем отправку данных
                sclient.BeginSend(sndBuffer, 0, lenght, SocketFlags.None, EndSend, sclient);
            }
            catch (Exception ex)
            {
                this.AddWaitPacket(buf, lenght);
                clientOptions.RunExtension(ex, sclient);

                //отключаем клиента, лишним не будет
                Disconnect();
            }
        }

        /// <summary>
        /// Завершение отправки данных
        /// </summary>
        /// <param name="r"></param>
        private void EndSend(IAsyncResult r)
        {
            //замыкаем это все в блок try, если клиент отключился то EndSend может вернуть ошибку
            try
            {
                //получаем размер переданных данных
                int len = sclient.EndSend(r);
                //при некоторых ошибках размер возвращает 0 или -1, проверяем
                if (len < 1)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                clientOptions.RunExtension(ex, sclient);

                //отключаем клиента, лишним не будет
                Disconnect();
            }
        }

        /// <summary>
        /// Отключить клиента
        /// </summary>
        public void Disconnect()
        {
            //проверяем возможно клиент и не был инициализирован, в случае дос атак, такое возможно
            if (sclient != null)
            {
                //отключаем и очищаем данные о клиенте
                try { sclient.Disconnect(false); } catch { }
                try { sclient.Dispose(); } catch { }
                sclient = null;
            }
            lenght = InputPacketBuffer.headerLenght;
            offset = 0;

            this.clientOptions.RunClientDisconnect(this.Network);
        }
    }
}
