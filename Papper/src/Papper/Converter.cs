﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Papper
{
    public static class Converter
    {
        private const string HexDigits = "0123456789ABCDEF";

        /// <summary>
        /// Convert numeric values to a byte array with swapped bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SetSwap<T>(this T value)
        {
            byte[] buffer = null;
            object temp = value;
            if (value is UInt32)
            {
                buffer = BitConverter.GetBytes(SwapDWord(Convert.ToUInt32(temp)));
            }
            else if (value is Int32)
            {
                buffer = BitConverter.GetBytes(SwapDInt(Convert.ToInt32(temp)));
            }
            else if (value is byte)
            {
                buffer = new byte[1];
                buffer[0] = (byte)temp;
            }
            else if (value is UInt16)
            {
                buffer = BitConverter.GetBytes(SwapWord(Convert.ToUInt16(temp)));
            }
            else if (value is Int16)
            {
                buffer = BitConverter.GetBytes(SwapInt(Convert.ToInt16(temp)));
            }
            else if (value is Single)
            {
                buffer = BitConverter.GetBytes(SwapSingle(Convert.ToSingle(temp)));
            }
            return buffer;
        }

        /// <summary>
        /// Convert numeric values to a byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SetNoSwap<T>(this T value)
        {
            byte[] buffer = null;
            object temp = value;
            if (value is UInt32)
            {
                buffer = BitConverter.GetBytes((UInt32)temp);
            }
            else if (value is Int32)
            {
                buffer = BitConverter.GetBytes((Int32)temp);
            }
            else if (value is byte)
            {
                buffer = new byte[1];
                buffer[0] = (byte)temp;
            }
            else if (value is UInt16)
            {
                buffer = BitConverter.GetBytes((UInt16)temp);
            }
            else if (value is Int16)
            {
                buffer = BitConverter.GetBytes((Int16)temp);
            }
            else if (value is Single)
            {
                buffer = ToByteArray(value, 4);
            }

            return buffer;
        }

        /// <summary>
        /// Extract a numeric value from an IEnumerable of byte with swapped bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T GetSwap<T>(this IEnumerable<byte> buffer, int offset = 0)
        {
            return buffer.Skip(offset).Take(sizeof(Single)).ToArray().GetSwap<T>();
        }

        /// <summary>
        /// Extract a numeric value from an byte array with swapped bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T GetSwap<T>(this byte[] buffer, int offset = 0)
        {
            object value = default(T);

            if (value is UInt32)
            {
                value = SwapDWord(BitConverter.ToUInt32(buffer, offset));
            }
            else if (value is Int32)
            {
                value = SwapDInt(BitConverter.ToInt32(buffer, offset));
            }
            else if (value is byte)
            {
                value = buffer[offset];
            }
            else if (value is UInt16)
            {
                value = SwapWord(BitConverter.ToUInt16(buffer, offset));
            }
            else if (value is Int16)
            {
                value = SwapInt(BitConverter.ToInt16(buffer, offset));
            }
            else if (value is Single)
            {
                value = SwapSingle(BitConverter.ToSingle(buffer, offset));
            }

            return (T)value;
        }

        /// <summary>
        /// Extract a numeric value from an IEnumerable of byte
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T GetNoSwap<T>(this IEnumerable<byte> buffer, int offset = 0)
        {
            return buffer.Skip(offset).Take(sizeof(Single)).ToArray().GetNoSwap<T>();
        }

        /// <summary>
        /// Extract a numeric value from an byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T GetNoSwap<T>(this byte[] buffer, int offset = 0)
        {
            object value = default(T);

            if (value is UInt32)
            {
                value = BitConverter.ToUInt32(buffer, offset);
            }
            else if (value is Int32)
            {
                value = BitConverter.ToInt32(buffer, offset);
            }
            else if (value is byte)
            {
                value = buffer[0];
            }
            else if (value is UInt16)
            {
                value = BitConverter.ToUInt16(buffer, offset);
            }
            else if (value is Int16)
            {
                value = BitConverter.ToInt16(buffer, offset);
            }
            else if (value is Single)
            {
                value = (Single)BitConverter.ToSingle(buffer, offset);
            }

            return (T)value;
        }

        /// <summary>
        /// Swap bytes in a word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static UInt16 SwapWord(this UInt16 word)
        {
            return (UInt16)(
                    ((word & 0xFFU) << 8) |
                    ((word & 0xFF00U) >> 8));
        }

        /// <summary>
        /// swap bytes in a dword
        /// </summary>
        /// <param name="dword"></param>
        /// <returns></returns>
        public static UInt32 SwapDWord(this UInt32 dword)
        {
            return (
                    (UInt32)((dword & 0x000000FFUL) << 24) |
                    (UInt32)((dword & 0x0000FF00UL) << 8) |
                    (UInt32)((dword & 0x00FF0000UL) >> 8) |
                    (UInt32)((dword & 0xFF000000UL) >> 24)
                    );
        }

        /// <summary>
        /// swap bytes in an integer
        /// </summary>
        /// <param name="intVal"></param>
        /// <returns></returns>
        public static Int16 SwapInt(this Int16 intVal)
        {
            var buffer = new byte[2];
            var array = intVal.ToByteArray(2);
            buffer[0] = array[1];
            buffer[1] = array[0];
            return BitConverter.ToInt16(buffer, 0);
        }

        /// <summary>
        /// swap bytes in an double integer
        /// </summary>
        /// <param name="intVal"></param>
        /// <returns></returns>
        public static Int32 SwapDInt(this Int32 intVal)
        {
            var buffer = new byte[4];
            var array = intVal.ToByteArray(4);
            buffer[0] = array[3];
            buffer[1] = array[2];
            buffer[2] = array[1];
            buffer[3] = array[0];
            return BitConverter.ToInt32(buffer, 0);
        }


        /// <summary>
        /// swap bytes in an single
        /// </summary>
        /// <param name="intVal"></param>
        /// <returns></returns>
        public static Single SwapSingle(this Single intVal)
        {
            var buffer = new byte[4];
            var array = intVal.ToByteArray(4);
            buffer[0] = array[3];
            buffer[1] = array[2];
            buffer[2] = array[1];
            buffer[3] = array[0];
            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>
        /// Get a bit by its nummer brom a byte
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static bool GetBit(this byte data, int bit)
        {
            // Shift the bit to the first location
            data = (byte)(data >> bit);

            // Isolate the value
            return (data & 1) == 1;
        }

        /// <summary>
        /// Set a bit by its number in a byte
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte SetBit(this byte data, int bit, bool value)
        {
            if (value)
                return (byte)(data | (1U << bit));
            return (byte)(data & (~(1U << bit)));
        }

        /// <summary>
        /// Sequential Equal with offset and length specification
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first"></param>
        /// <param name="firstStartIndex"></param>
        /// <param name="second"></param>
        /// <param name="secondStartIndex"></param>
        /// <param name="length"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, int firstStartIndex, IEnumerable<TSource> second, int secondStartIndex, int length = -1, IEqualityComparer<TSource> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<TSource>.Default; ;
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            using (IEnumerator<TSource> e1 = first.GetEnumerator())
            using (IEnumerator<TSource> e2 = second.GetEnumerator())
            {
                var skip = Math.Max(firstStartIndex, secondStartIndex);
                for (int i = 0; i < skip; i++)
                {
                    if (i < firstStartIndex)
                        e1.MoveNext();
                    if(i < secondStartIndex)
                        e2.MoveNext();
                }
                var index = 0;
                while (e1.MoveNext())
                {
                    if (!(e2.MoveNext() && comparer.Equals(e1.Current, e2.Current))) return false;
                    index++;
                    if (length > 0 && index >= length)
                        return true;
                }
                if (e2.MoveNext()) return false;
            }
            return true;
        }

        /// <summary>
        /// Extract subarray from byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="skip"></param>
        /// <param name="length"></param>
        /// <param name="realloc"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] data, int skip, int length = -1, bool realloc = false)
        {
            var dataLength = data.Length;
            if (length == -1)
                length = dataLength - skip;
            if (skip == 0 && length == dataLength && !realloc) //No manipulation and no copying
                return data;
            var result = new T[length];
            Array.Copy(data, skip, result, 0, length);
            return result;
        }

        /// <summary>
        /// Convert a struct to a byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static byte[] ToByteArray<T>(this T value, int maxLength)
        {
            var str = value as string;
            if(str != null) return Encoding.ASCII.GetBytes(str).SubArray(0, maxLength);
            var rawdata = new byte[Marshal.SizeOf(value)];
            var handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            handle.Free();
            if (maxLength >= rawdata.Length)
                return rawdata;

            var temp = new byte[maxLength];
            Array.Copy(rawdata, temp, maxLength);
            return temp;
        }

        /// <summary>
        /// Convert a byte array to a struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        public static T FromByteArray<T>(this byte[] rawValue)
        {
            var handle = GCHandle.Alloc(rawValue, GCHandleType.Pinned);
            var structure = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
            return structure;
        }

        /// <summary>
        /// Get bcd value from byte
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetBcdByte(this byte b)
        {
            //Acepted Values 00 to 99
            int bt1 = b;
            var neg = (bt1 & 0xf0) == 0xf0;
            if (neg)
                bt1 = -1 * (bt1 & 0x0f);
            else
                bt1 = (bt1 >> 4) * 10 + (bt1 & 0x0f);
            return bt1;
        }

        /// <summary>
        /// Set bcd value in byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte SetBcdByte(this int value)
        {
            int b0 = 0, b1 = 0;

            //setze höchstes bit == negativer wert!
            if (value < 0)
                return (byte)((b1 << 4) + b0);
            b1 = (value % 100 / 10);
            b0 = value % 10;
            return (byte)((b1 << 4) + b0);
        }

        /// <summary>
        /// Get bcd word from byte array
        /// </summary>
        /// <param name="b"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int GetBcdWord(this byte[] b, int offset = 0)
        {
            int bt1 = b[offset];
            int bt2 = b[offset + 1];
            var neg = (bt1 & 0xf0) == 0xf0;

            bt1 = bt1 & 0x0f;
            bt2 = (bt2 / 0x10) * 10 + (bt2 & 0x0f % 0x10);

            return (neg ? (bt1 * 100 + bt2) * -1 : bt1 * 100 + bt2);
        }

        /// <summary>
        /// set bcd word in byte array
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static byte[] SetBcdWord(this int value, int offset = 0)
        {
            //Acepted Values -999 to +999
            var b = new byte[2];
            int b3;

            if (value < 0)
            {
                b3 = 0x0f;
                value = -1 * value;
            }
            else
                b3 = 0x00;

            var b2 = (value % 1000 / 100);
            var b1 = (value % 100 / 10);
            var b0 = (value % 10);

            b[offset] = (byte)((b3 << 4) + b2);
            b[offset + 1] = (byte)((b1 << 4) + b0);
            return b;
        }

        public static int GetBcdDWord(this byte[] b, int offset = 0)
        {
            int bt1 = b[offset];
            int bt2 = b[offset + 1];
            int bt3 = b[offset + 2];
            int bt4 = b[offset + 3];
            var neg = (bt1 & 0xf0) == 0xf0;

            bt1 = bt1 & 0x0f;
            bt2 = (bt2 / 0x10) * 10 + (bt2 % 0x10);
            bt3 = (bt3 / 0x10) * 10 + (bt3 % 0x10);
            bt4 = (bt4 / 0x10) * 10 + (bt4 % 0x10);
            return neg ? (bt1 * 1000000 + bt2 * 10000 + bt3 * 100 + bt4) * -1 : bt1 * 1000000 + bt2 * 10000 + bt3 * 100 + bt4;
        }

        public static byte[] SetBcdDWord(this int value, int offset = 0)
        {
            //Acepted Values -9999999 to +9999999
            var b = new byte[4];
            int b7;

            if (value < 0)
            {
                b7 = 0x0f;
                value = -1 * value;
            }
            else
                b7 = 0x00;
            var b6 = (value % 10000000 / 1000000);
            var b5 = (value % 1000000 / 100000);
            var b4 = (value % 100000 / 10000);
            var b3 = (value % 10000 / 1000);
            var b2 = (value % 1000 / 100);
            var b1 = (value % 100 / 10);
            var b0 = (value % 10);

            b[offset] = (byte)((b7 << 4) + b6);
            b[offset + 1] = (byte)((b5 << 4) + b4);
            b[offset + 2] = (byte)((b3 << 4) + b2);
            b[offset + 3] = (byte)((b1 << 4) + b0);
            return b;
        }

        public static string ToBinString(this byte b)
        {
            var binString = new StringBuilder(8);
            for (var bitno = 1; bitno < 0x0100; bitno <<= 2)
                binString.Append((b & bitno) != 0 ? "1" : "0");
            return binString.ToString();
        }

        public static string ToBinString(this IEnumerable<byte> bytes, string separator = "", int offset = 0, int length = int.MaxValue)
        {
            var arr = bytes.Skip(offset).Take(length).ToArray();
            var binString = new StringBuilder(arr.Length * 8);

            foreach (var b in arr.Reverse())
            {
                if (binString.Length > 0)
                    binString.Append(separator);

                for (var bitno = 7; bitno >= 0; bitno--)
                    binString.Append(((b >> bitno) & 1) != 0 ? "1" : "0");
            }
            return binString.ToString();
        }

        public static string ToHexString(this IEnumerable<byte> bytes, string separator = "", int offset = 0, int length = int.MaxValue)
        {
            var arr = bytes.Skip(offset).Take(length).ToArray();
            if (!arr.Any())
                return string.Empty;
            var sb = new StringBuilder(arr.Count() * (2 + separator.Length));
            foreach (var b in arr.Reverse())
                sb.AppendFormat("{0:X2}{1}", b, separator);
            return sb.ToString(0, sb.Length - separator.Length);
        }

        public static byte[] HexGetBytes(this string hexString)
        {
            int discarded;
            return (HexGetBytes(hexString, out discarded));
        }

        public static T HexGet<T>(this string hexString)
        {
            object value = default(T);

            try
            {
                Int64 val = 0;
                foreach (var b in hexString.ToLower().Replace("0x", ""))
                {
                    val *= 16;
                    switch (b)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            val += (Int64)(b - '0');
                            break;
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            val += (Int64)(b - 'a' + 10);
                            break;
                    }
                }

                value = Convert.ChangeType(val, typeof(T));
            }
            catch
            { }
            return (T)value;
        }

        public static byte[] BinGetBytes(this string binString)
        {
            int discarded;
            return (BinGetBytes(binString, out discarded));
        }

        public static T BinGet<T>(this string binString)
        {
            object value = default(T);

            try
            {
                Int64 val = 0;
                foreach (var b in binString)
                {
                    switch (b)
                    {
                        case '1':
                            val *= 2;
                            val += 1;
                            break;
                        case '0':
                            val *= 2;
                            break;
                    }
                }

                value = Convert.ChangeType(val, typeof(T));
            }
            catch { }
            return (T)value;
        }

        /// <summary>
        /// converts the given byte array to an DateTime, if the value is not in range, DateTime.MinValue will be returned
        /// </summary>
        /// <param name="data">minimum 8 byte - offset</param>
        /// <param name="offset">offset to first byte</param>
        /// <returns>DateTime</returns>
        public static DateTime ToDateTime(this byte[] data, int offset = 0)
        {
            var str = string.Format("{2}/{1}/{0} {3}:{4}:{5}.{6}{7}",
                data.ToHexString("", offset, 1),
                data.ToHexString("", offset + 1, 1),
                data.ToHexString("", offset + 2, 1),
                data.ToHexString("", offset + 3, 1),
                data.ToHexString("", offset + 4, 1),
                data.ToHexString("", offset + 5, 1),
                data.ToHexString("", offset + 6, 1),
                data.ToHexString("", offset + 7, 1));
            DateTime parsedDate;
            if (DateTime.TryParseExact(str, "dd/MM/yy HH:mm:ss.ffff", null, DateTimeStyles.None, out parsedDate))
                return parsedDate;
            return DateTime.MinValue;
        }

        /// <summary>
        /// Determines if given string is in proper hexadecimal string format
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static bool IsInHexFormat(this string hexString)
        {
            return hexString.All(IsHexDigit);
        }

        private static bool IsHexDigit(char c)
        {
            return HexDigits.IndexOf(c) >= 0;
        }

        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored. 
        /// </summary>
        /// <param name="hexString">string to convert to byte array</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        private static byte[] HexGetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            var newString = new StringBuilder();
            // remove all none A-F, 0-9, characters
            foreach (var c in hexString)
            {
                if (IsHexDigit(c))
                    newString.Append(c);
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = new StringBuilder(newString.ToString().Substring(0, newString.Length - 1));
            }

            var byteLength = newString.Length / 2;
            var bytes = new byte[byteLength];
            var j = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                var b1 = newString[j] - 48;
                var b2 = newString[j + 1] - 48;
                if (b1 > 9)
                    b1 -= 7;
                if (b2 > 9)
                    b2 -= 7;
                bytes[i] = (byte)(b1 * 16 + b2);
                j = j + 2;
            }
            return bytes;
        }

        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored. 
        /// </summary>
        /// <param name="binString">string to convert to byte array</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        private static byte[] BinGetBytes(string binString, out int discarded)
        {
            discarded = 0;
            var newString = new StringBuilder();

            // remove all none 0-1,characters
            foreach (var c in binString)
            {
                if (c == '0' || c == '1')
                    newString.Append(c);
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = new StringBuilder(newString.ToString().Substring(0, newString.Length - 1));
            }

            var byteLength = newString.Length / 8;
            var bytes = new byte[byteLength];
            for (var i = 0; i < byteLength; ++i)
                bytes[i] = Convert.ToByte(newString.ToString().Substring(8 * i, 8), 2);

            return bytes;
        }
    }
}
