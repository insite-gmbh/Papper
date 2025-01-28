using Papper.Internal;
using Papper.Types;
using System;
using Xunit;

namespace Papper.Tests
{
    public class DataTypeTests
    {
        [Fact]
        public void TestSInt()
        {
            sbyte value = 44;
            var type = new PlcSInt("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (sbyte)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

            value = -44;
            type.ConvertToRaw(value, binding, data);
            var result2 = (sbyte)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result2);
        }


        [Fact]
        public void TestUSInt()
        {
            byte value = 44;

            var type = new PlcUSInt("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (byte)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);
        }



        [Fact]
        public void TestLInt()
        {
            long value = 44;

            var type = new PlcLInt("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (long)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

            value = -44;
            type.ConvertToRaw(value, binding, data);
            var result2 = (long)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result2);
        }

        [Fact]
        public void TestLWord()
        {
            ulong value = 44;

            var type = new PlcLWord("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (ulong)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

        }

        [Fact]
        public void TestTIme()
        {
            var value = TimeSpan.FromHours(2);

            var type = new PlcTime("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

        }


        [Fact]
        public void TestLTIme()
        {
            var value = TimeSpan.FromHours(2);

            var type = new PlcLTime("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

            // The following conversation are not equal because the datatype us it's own range values
            value = TimeSpan.MinValue;
            type.ConvertToRaw(value, binding, data);
            result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.NotEqual(value, result1);

            value = TimeSpan.MaxValue;
            type.ConvertToRaw(value, binding, data);
            result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.NotEqual(value, result1);

        }

        [Fact]
        public void TestLTimeOfDay()
        {
            var value = TimeSpan.FromHours(2);

            var type = new PlcLTimeOfDay("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

            // The following conversation are not equal because the datatype us it's own range values
            value = TimeSpan.MinValue;
            type.ConvertToRaw(value, binding, data);
            result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.NotEqual(value, result1);

            value = TimeSpan.MaxValue;
            type.ConvertToRaw(value, binding, data);
            result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.NotEqual(value, result1);

        }

        [Fact]
        public void TestTimeOfDay()
        {
            var value = TimeSpan.FromHours(2);

            var type = new PlcTimeOfDay("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

            // The following conversation are not equal because the datatype us it's own range values
            value = TimeSpan.MinValue;
            type.ConvertToRaw(value, binding, data);
            result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.NotEqual(value, result1);

            value = TimeSpan.MaxValue;
            type.ConvertToRaw(value, binding, data);
            result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.NotEqual(value, result1);

        }

        [Fact]
        public void Tests5Time()
        {
            var value = TimeSpan.FromHours(2);

            var type = new PlcS5Time("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

            // The following conversation are not equal because the datatype us it's own range values
            value = TimeSpan.MinValue;
            type.ConvertToRaw(value, binding, data);
            result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.NotEqual(value, result1);

            value = TimeSpan.MaxValue;
            type.ConvertToRaw(value, binding, data);
            result1 = (TimeSpan)type.ConvertFromRaw(binding, data);
            Assert.NotEqual(value, result1);

        }

        [Theory]
        [InlineData(1990, 1, 1)]
        [InlineData(1991, 1, 1)]
        [InlineData(2019, 2, 5)]
        [InlineData(2169, 6, 6)]
        public void TestDate(int year, int month, int day)
        {
            var value = new DateTime(year, month, day);

            var type = new PlcDate("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (DateTime)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

        }

        [Theory]
        [InlineData(1990, 1, 1, 0, 0, 0, 0)]
        [InlineData(2089, 12, 31, 23, 59, 59, 999)]
        public void TestDateTime(int year, int month, int day, int hour, int min, int sec, int millis)
        {
            var value = new DateTime(year, month, day, hour, min, sec, millis);

            var type = new PlcDateTime("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (DateTime)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

        }

        [Fact]
        public void TestLDateTIme()
        {
            var value = DateTime.Now;

            var type = new PlcLDateTime("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (DateTime)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

        }


        [Fact]
        public void TestUDINt()
        {
            uint value = 44;

            var type = new PlcUDInt("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (uint)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

        }

        [Fact]
        public void TestDINt()
        {
            var value = 44;

            var type = new PlcDInt("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (int)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

            value = -44;

            type.ConvertToRaw(value, binding, data);
            var result2 = (int)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result2);

        }

        [Fact]
        public void TestULInt()
        {
            ulong value = 44;

            var type = new PlcULInt("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (ulong)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);
        }


        [Fact]
        public void TestWChar()
        {
            var value = "↕";

            var type = new PlcWChar("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (string)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);
        }

        [Fact]
        public void TestWString()
        {
            var value = "↕TEst░iä";

            var type = new PlcWString("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(1024), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (string)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);
        }

        [Fact]
        public void TestWStringNormal()
        {
            var value = "Test123";

            var type = new PlcWString("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(1024), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (string)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);
        }

        [Fact]
        public void TestLReal()
        {
            double value = 44.33;

            var type = new PlcLReal("TEST");
            var data = new byte[type.ByteSize];
            var binding = new PlcObjectBinding(new PlcRawData(512), type, 0, 0);

            type.ConvertToRaw(value, binding, data);
            var result1 = (double)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result1);

            value = -44.0;
            type.ConvertToRaw(value, binding, data);
            var result2 = (double)type.ConvertFromRaw(binding, data);
            Assert.Equal(value, result2);
        }
    }
}
