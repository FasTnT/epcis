using Dapper;
using FasTnT.Model.Utils;
using System.Data;
using System.Linq;

namespace FasTnT.Data.PostgreSql.DapperConfiguration
{
    public class ArrayOfEnumerationHandler<T> : SqlMapper.TypeHandler<T[]> where T : Enumeration, new()
    {
        public static ArrayOfEnumerationHandler<T> Default = new ArrayOfEnumerationHandler<T>();

        public override void SetValue(IDbDataParameter parameter, T[] value) => parameter.Value = value.Select(x => x.Id).ToArray();
        public override T[] Parse(object value) => (value as short[]).Select(Enumeration.GetById<T>).ToArray();
    }
}
