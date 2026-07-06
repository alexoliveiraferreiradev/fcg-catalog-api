using Dapper;
using Fcg.Catalog.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Infrastructure.Queries.DapperHandlers
{
    public class PriceTypeHandler : SqlMapper.TypeHandler<Price>
    {
        public override void SetValue(IDbDataParameter parameter, Price? value)
        {
            parameter.Value = value?.Amount ?? (object)DBNull.Value;
        }
        
        public override Price Parse(object value)
        {
            var valorStr = Convert.ToDecimal(value?.ToString() ?? "0");
            return new Price(valorStr); 
        }
    }
}
