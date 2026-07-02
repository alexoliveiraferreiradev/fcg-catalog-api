using Dapper;
using Fcg.Catalog.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Infrastructure.DapperHandlers
{
    public class PrecoTypeHandler : SqlMapper.TypeHandler<Preco>
    {
        public override void SetValue(IDbDataParameter parameter, Preco? value)
        {
            parameter.Value = value?.Valor ?? (object)DBNull.Value;
        }
        
        public override Preco Parse(object value)
        {
            var valorStr = Convert.ToDecimal(value?.ToString() ?? "0");
            return new Preco(valorStr); 
        }
    }
}
