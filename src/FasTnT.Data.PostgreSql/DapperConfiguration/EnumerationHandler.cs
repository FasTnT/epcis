using Dapper;
using FasTnT.Model.Utils;
using System.Data;

namespace FasTnT.Data.PostgreSql.DapperConfiguration
{
    public class EnumerationHandler<T> : SqlMapper.TypeHandler<T> where T : Enumeration, new()
    {
        public static EnumerationHandler<T> Default = new EnumerationHandler<T>();

        public override void SetValue(IDbDataParameter parameter, T value) => parameter.Value = value.Id;
        public override T Parse(object value) => Enumeration.GetById<T>(value as short? ?? 0);
    }
}
