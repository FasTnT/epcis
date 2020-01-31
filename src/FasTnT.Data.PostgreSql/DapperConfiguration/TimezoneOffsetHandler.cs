using Dapper;
using FasTnT.Model;
using System.Data;

namespace FasTnT.Data.PostgreSql.DapperConfiguration
{
    public class TimezoneOffsetHandler : SqlMapper.TypeHandler<TimeZoneOffset>
    {
        public static readonly TimezoneOffsetHandler Default = new TimezoneOffsetHandler();
        public override void SetValue(IDbDataParameter parameter, TimeZoneOffset value) => parameter.Value = value.Value;
        public override TimeZoneOffset Parse(object value) => new TimeZoneOffset { Value = ((value as short?) ?? 0) };
    }
}
