using Dapper;
using Fcg.Catalog.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Infrastructure.DapperHandlers
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
