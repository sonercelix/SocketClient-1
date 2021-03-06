﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SocketClient.Utils.Buffer
{
    public class OutputPacketBuffer
    {
        /// <summary>
        /// Текущая кодировка типа String
        /// </summary>
        Encoding coding = Encoding.UTF8;

        /// <summary>
        /// Буффер с полученными данными
        /// </summary>
        byte[] buffer;

        /// <summary>
        /// Текущий размер буффера (включая пустые байты)
        /// </summary>
        int bufferLenght;

        /// <summary>
        /// маскировка хедера пакета
        /// </summary>
        int offs;
        int offset { get { return offs + headerLenght; } set { offs = value; } }
        int lenght;

        /// <summary>
        /// Текущая позиция чтения в потоке
        /// </summary>
        public int Offset { get { return offs; } }

        /// <summary>
        /// Размер данных пакета
        /// </summary>
        public int Lenght { get { return lenght; } }

        /// <summary>
        /// Полный размер пакета
        /// </summary>
        public int PacketLenght { get { return lenght + headerLenght; } }

        /// <summary>
        /// Индификатор пакета
        /// </summary>
        public ushort PacketId { get; set; }

        /// <summary>
        /// Хеш байт пакета
        /// </summary>
        public bool AppendHash { get; set; }

        /// <summary>
        /// Размер шапки пакета
        /// </summary>
        public const int headerLenght = 7;

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="len">начальный размер буффера</param>
        public OutputPacketBuffer(int len = 32)
        {
            //начальный размер буффера необходим для оптимизации пакетов, в случае если пакет имеет заведомо известный размер, его не придется увеличивать что будет экономить время
            //инициализация буффера
            buffer = new byte[headerLenght + len];
            //установка размера буффера
            bufferLenght = headerLenght + len;

            offset = 0;
        }

        /// <summary>
        /// Запись значения float (4 bytes)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteFloat(float value)
        {
            Write(BitConverter.GetBytes(value), 0, 4);
        }

        /// <summary>
        /// Запись значения double (8 bytes)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteDouble(double value)
        {
            Write(BitConverter.GetBytes(value), 0, 8);
        }

        /// <summary>
        /// не реализовано
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteDecimal(decimal value)
        {
            throw new Exception();
        }

        /// <summary>
        /// Запись значения short (int16, 2 байта)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteInt16(short value)
        {
            Write(BitConverter.GetBytes(value), 0, 2);
        }

        /// <summary>
        /// Запись значения ushort (uint16, 2 байта)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteUInt16(ushort value)
        {
            Write(BitConverter.GetBytes(value), 0, 2);
        }

        /// <summary>
        /// Запись значения int (int32, 4 байта)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteInt32(int value)
        {
            Write(BitConverter.GetBytes(value), 0, 4);
        }

        /// <summary>
        /// Запись значения uint (uint32, 4 байта)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteUInt32(uint value)
        {
            Write(BitConverter.GetBytes(value), 0, 4);
        }

        /// <summary>
        /// Запись значения long (int64, 8 байта)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteInt64(long value)
        {
            Write(BitConverter.GetBytes(value), 0, 8);
        }

        /// <summary>
        /// Запись значения ulong (uint64, 8 байта)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteUInt64(ulong value)
        {
            Write(BitConverter.GetBytes(value), 0, 8);
        }

        /// <summary>
        /// Запись значения bool (1 байт)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteBool(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// Запись значения byte (1 байт)
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteByte(byte value)
        {
            if (offset + 1 >= bufferLenght)
                SetLenght(1);
            buffer[offset] = value;
            offs++;
            if (offs >= lenght)
                lenght = offs;
        }

        /// <summary>
        /// Запись значения string, с записью заголовка c размером ushort (2 байта), до 36к симв
        /// </summary>
        /// <param name="value">значение</param>
        public void WriteString16(string value)
        {
            if (value == null)
                value = "";
            byte[] buf = coding.GetBytes(value);

            WriteUInt16((ushort)buf.Length);
            if (buf.Length > 0)
                Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Запись значения string, с записью заголовка c размером  (4 байта), до 1.2ккк симв
        /// </summary>
        /// <param name="value">текст</param>
        public void WriteString32(string value)
        {
            if (value == null)
                value = "";
            byte[] buf = coding.GetBytes(value);

            WriteUInt32((uint)buf.Length);
            if (buf.Length > 0)
                Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Запись массива байт
        /// </summary>
        /// <param name="buf">буффер</param>
        /// <param name="off">позиция в буффере</param>
        /// <param name="len">размер для записи</param>
        public void Write(byte[] buf, int off, int len)
        {
            if (offset + len >= bufferLenght)
                SetLenght(len);
            for (int i = len - 1; i >= 0; i--)
            {
                buffer[offset + i] = buf[i - off];
            }
            offs += len;
            if (offs >= lenght)
                lenght = offs;
        }

        /// <summary>
        /// Добавление размера пакета в случае недостающего размера буффера
        /// </summary>
        /// <param name="appendLen">кол-во байт добавляемых в буффер</param>
        private void SetLenght(int appendLen)
        {
            while (offset + appendLen >= bufferLenght)
            {
                bufferLenght = bufferLenght * 2;
            }
            Array.Resize(ref buffer, bufferLenght);
        }

        /// <summary>
        /// Смещение положения в массиве
        /// </summary>
        /// <param name="len">размер на который нужно сместить положение</param>
        /// <param name="seek">откуда смещать</param>
        /// <returns></returns>
        public int Seek(int len, SeekOrigin seek)
        {
            if (seek == SeekOrigin.Begin)
            {
                offs = len;
            }
            else if (seek == SeekOrigin.Current)
            {
                offs += len;
            }
            else if (seek == SeekOrigin.End)
            {
                offs = lenght + len;
            }
            if (offs < 0)
                offs = 0;
            return offset;
        }

        /// <summary>
        /// Сборка пакета в финальный вид перед отправкой
        /// </summary>
        /// <param name="appendHash">добавить хеш в пакет</param>
        /// <returns></returns>
        public byte[] CompilePacket()
        {
            int off = offs;

            offset = 0 - headerLenght;

            WriteInt32(PacketLenght);
            WriteUInt16(PacketId);

            if (AppendHash)
            {
                WriteByte((byte)((Lenght + PacketId) % 14));
            }

            offs = off;

            return buffer;
        }
    }
}
