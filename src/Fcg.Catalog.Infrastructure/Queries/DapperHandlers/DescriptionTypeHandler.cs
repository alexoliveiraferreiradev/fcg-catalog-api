using Dapper;
using Fcg.Catalog.Domain.ValueObject;
using System.Data;

namespace Fcg.Catalog.Infrastructure.Queries.DapperHandlers
{
    public class DescriptionTypeHandler : SqlMapper.TypeHandler<Description>
    {
        public override Description? Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Description(valorStr);
        }

        public override void SetValue(IDbDataParameter parameter, Description? value)
        {
            parameter.Value = value?.Value ?? (object)DBNull.Value;
        }
    }
}
