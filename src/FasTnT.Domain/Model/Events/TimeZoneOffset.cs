using System;
using System.Globalization;

namespace FasTnT.Model
{
    public class TimeZoneOffset
    {
        public static TimeZoneOffset Default = new TimeZoneOffset();

        public string Representation { get { return ComputeRepresentation(Value); } set { Value = ComputeValue(value); } }
        public short Value { get; set; }

        private static string ComputeRepresentation(int value)
        {
            var sign = value >= 0 ? "+" : "-";
            var hours = (Math.Abs(value) / 60).ToString("D2", CultureInfo.InvariantCulture);
            var minutes = (Math.Abs(value % 60)).ToString("D2", CultureInfo.InvariantCulture);

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}:{2}", sign, hours, minutes);
        }

        private static short ComputeValue(string value)
        {
            var sign = (value[0] == '-') ? -1 : +1;
            var parts = value.Split(':');

            return (short)(sign * (Math.Abs(int.Parse(parts[0], CultureInfo.InvariantCulture)) * 60 + int.Parse(parts[1], CultureInfo.InvariantCulture)));
        }
    }
}
