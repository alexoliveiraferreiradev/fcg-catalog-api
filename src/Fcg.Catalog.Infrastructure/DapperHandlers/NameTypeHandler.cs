using Dapper;
using Fcg.Catalog.Domain.ValueObject;
using System.Data;

namespace Fcg.Catalog.Infrastructure.DapperHandlers
{
    public class NameTypeHandler: SqlMapper.TypeHandler<Name>
    {
        public override void SetValue(IDbDataParameter parameter, Name? value)
        {
            parameter.Value = value?.Value ?? (object)DBNull.Value;
        }
        
        public override Name Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Name(valorStr); 
        }
    }
}
