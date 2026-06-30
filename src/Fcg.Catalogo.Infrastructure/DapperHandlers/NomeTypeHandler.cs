using Dapper;
using Fcg.Catalogo.Domain.ValueObject;
using System.Data;

namespace Fcg.Catalogo.Infrastructure.DapperHandlers
{
    public class NomeTypeHandler: SqlMapper.TypeHandler<Nome>
    {
        public override void SetValue(IDbDataParameter parameter, Nome? value)
        {
            parameter.Value = value?.Valor ?? (object)DBNull.Value;
        }
        
        public override Nome Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Nome(valorStr); 
        }
    }
}
