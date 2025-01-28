using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Papper
{
    public static class Converter
    {
        private const string _hexDigits = "0123456789ABCDEF";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBit(ReadOnlySpan<byte> buffer, int bit) => (((byte)(buffer[0] >> bit)) & 1) == 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBit(Span<byte> buffer, int bit, bool value) => buffer[0] = value ? (byte)(buffer[0] | (1U << bit)) : (byte)(buffer[0] & (~(1U << bit)));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(this byte data, int bit)
        {
            // Shift the bit to the first location
            data = (byte)(data >> bit);

            // Isolate the value
            return (data & 1) == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SetBit(this byte data, int bit, bool value)
        {
            if (value)
            {
                return (byte)(data | (1U << bit));
            }

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
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource>? first, int firstStartIndex, IEnumerable<TSource>? second, int secondStartIndex, int length = -1, IEqualityComparer<TSource>? comparer = null)
        {
            comparer ??= EqualityComparer<TSource>.Default;;
            if (first == null)
            {
                ExceptionThrowHelper.ThrowArgumentNullException(nameof(first));
                return false;
            }

            if (second == null)
            {
                ExceptionThrowHelper.ThrowArgumentNullException(nameof(second));
                return false;
            }

            using (var e1 = first!.GetEnumerator())
            using (var e2 = second!.GetEnumerator())
            {
                var skip = Math.Max(firstStartIndex, secondStartIndex);
                for (var i = 0; i < skip; i++)
                {
                    if (i < firstStartIndex)
                    {
                        e1.MoveNext();
                    }

                    if (i < secondStartIndex)
                    {
                        e2.MoveNext();
                    }
                }
                var index = 0;
                while (e1.MoveNext())
                {
                    if (!(e2.MoveNext() && comparer.Equals(e1.Current, e2.Current)))
                    {
                        return false;
                    }

                    index++;
                    if (length > 0 && index >= length)
                    {
                        return true;
                    }
                }
                if (e2.MoveNext())
                {
                    return false;
                }
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
            if (data == null)
            {
                return Array.Empty<T>();
            }

            var dataLength = data.Length;
            if (length == -1)
            {
                length = dataLength - skip;
            }

            if (skip == 0 && length == dataLength && !realloc) //No manipulation and no copying
            {
                return data;
            }

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
            if(value == null)
            {
                return new byte[maxLength];
            }

            if (value is string str)
            {
                return Encoding.UTF7.GetBytes(str).SubArray(0, maxLength);
            }

            var rawdata = new byte[Marshal.SizeOf(value)];
            var handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            handle.Free();
            if (maxLength >= rawdata.Length)
            {
                return rawdata;
            }

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
        public static T? FromByteArray<T>(this byte[] rawValue)
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
            //Accepted Values 00 to 99
            int bt1 = b;
            var neg = (bt1 & 0xf0) == 0xf0;
            if (neg)
            {
                bt1 = -1 * (bt1 & 0x0f);
            }
            else
            {
                bt1 = (bt1 >> 4) * 10 + (bt1 & 0x0f);
            }

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

            //set highest bit == negative value!
            if (value < 0)
            {
                return (byte)((b1 << 4) + b0);
            }

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
        public static int GetBcdWord(this Span<byte> b, int offset = 0)
        {
            int bt1 = b[offset];
            int bt2 = b[offset + 1];
            var neg = (bt1 & 0xf0) == 0xf0;

            bt1 &= 0x0f;
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
            {
                b3 = 0x00;
            }

            var b2 = (value % 1000 / 100);
            var b1 = (value % 100 / 10);
            var b0 = (value % 10);

            b[offset] = (byte)((b3 << 4) + b2);
            b[offset + 1] = (byte)((b1 << 4) + b0);
            return b;
        }

        public static int GetBcdDWord(this Span<byte> b, int offset = 0)
        {
            int bt1 = b[offset];
            int bt2 = b[offset + 1];
            int bt3 = b[offset + 2];
            int bt4 = b[offset + 3];
            var neg = (bt1 & 0xf0) == 0xf0;

            bt1 &= 0x0f;
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
            {
                b7 = 0x00;
            }

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
            {
                binString.Append((b & bitno) != 0 ? "1" : "0");
            }

            return binString.ToString();
        }

        public static string ToBinString(this IEnumerable<byte> bytes, string separator = "", int offset = 0, int length = int.MaxValue)
        {
            var arr = bytes.Skip(offset).Take(length).ToArray();
            var binString = new StringBuilder(arr.Length * 8);

            foreach (var b in arr.Reverse())
            {
                if (binString.Length > 0)
                {
                    binString.Append(separator);
                }

                for (var bitno = 7; bitno >= 0; bitno--)
                {
                    binString.Append(((b >> bitno) & 1) != 0 ? "1" : "0");
                }
            }
            return binString.ToString();
        }

        public static string ToHexString(this IEnumerable<byte> bytes, string separator = "", int offset = 0, int length = int.MaxValue)
        {
            var arr = bytes.Skip(offset).Take(length).ToArray();
            if (arr == null || !arr.Any())
            {
                return string.Empty;
            }

            separator ??= string.Empty;
            var sb = new StringBuilder(arr.Length * (2 + separator.Length));
            foreach (var b in arr.Reverse())
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}{1}", b, separator);
            }

            return sb.ToString(0, sb.Length - separator.Length);
        }

        public static byte[] HexGetBytes(this string hexString) => (HexGetBytes(hexString, out _));

        public static T HexGet<T>(this string hexString) where T : struct
        {
            object value = default(T);

            try
            {
                if (!string.IsNullOrWhiteSpace(hexString))
                {
                    long val = 0;
                    foreach (var b in hexString.Replace("0x", ""))
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
                                val += b - '0';
                                break;
                            case 'a':
                            case 'b':
                            case 'c':
                            case 'd':
                            case 'e':
                            case 'f':
                                val += b - 'a' + 10;
                                break;
                            case 'A':
                            case 'B':
                            case 'C':
                            case 'D':
                            case 'E':
                            case 'F':
                                val += b - 'A' + 10;
                                break;
                        }
                    }

                    value = Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
                }
            }
            catch
            { }
            return (T)value;
        }

        public static byte[] BinGetBytes(this string binString) => (BinGetBytes(binString, out _));

        public static T BinGet<T>(this string binString) where T : struct
        {
            object? value = default(T);

            try
            {
                if (!string.IsNullOrWhiteSpace(binString))
                {
                    long val = 0;
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

                    value = Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
                }
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
            var str = string.Format(CultureInfo.InvariantCulture, "{2}/{1}/{0} {3}:{4}:{5}.{6}{7}",
                data.ToHexString("", offset, 1),
                data.ToHexString("", offset + 1, 1),
                data.ToHexString("", offset + 2, 1),
                data.ToHexString("", offset + 3, 1),
                data.ToHexString("", offset + 4, 1),
                data.ToHexString("", offset + 5, 1),
                data.ToHexString("", offset + 6, 1),
                data.ToHexString("", offset + 7, 1));
            if (DateTime.TryParseExact(str, "dd/MM/yy HH:mm:ss.ffff", null, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Determines if given string is in proper hexadecimal string format
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static bool IsInHexFormat(this string hexString) => hexString.All(IsHexDigit);

        private static bool IsHexDigit(char c) => _hexDigits.Contains(c);

        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored. 
        /// </summary>
        /// <param name="hexString">string to convert to byte array</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        private static byte[] HexGetBytes(string? hexString, out int discarded)
        {
            discarded = 0;
            if (string.IsNullOrEmpty(hexString))
            {
                return Array.Empty<byte>();
            }

            var newString = new StringBuilder();
            // remove all none A-F, 0-9, characters
            foreach (var c in hexString!)
            {
                if (IsHexDigit(c))
                {
                    newString.Append(c);
                }
                else
                {
                    discarded++;
                }
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = new StringBuilder(newString.ToString(0, newString.Length - 1));
            }

            var byteLength = newString.Length / 2;
            var bytes = new byte[byteLength];
            var j = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                var b1 = newString[j] - 48;
                var b2 = newString[j + 1] - 48;
                if (b1 > 9)
                {
                    b1 -= 7;
                }

                if (b2 > 9)
                {
                    b2 -= 7;
                }

                bytes[i] = (byte)(b1 * 16 + b2);
                j += 2;
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
            if (string.IsNullOrEmpty(binString))
            {
                return Array.Empty<byte>();
            }

            var newString = new StringBuilder();

            // remove all none 0-1,characters
            foreach (var c in binString)
            {
                if (c == '0' || c == '1')
                {
                    newString.Append(c);
                }
                else
                {
                    discarded++;
                }
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = new StringBuilder(newString.ToString(0, newString.Length - 1));
            }

            var byteLength = newString.Length / 8;
            var bytes = new byte[byteLength];
            for (var i = 0; i < byteLength; ++i)
            {
                bytes[i] = Convert.ToByte(newString.ToString(8 * i, 8), 2);
            }

            return bytes;
        }
    }
}
