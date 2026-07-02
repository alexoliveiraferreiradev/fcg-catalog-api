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
    public class DescricaoTypeHandler : SqlMapper.TypeHandler<Descricao>
    {
        public override Descricao? Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Descricao(valorStr);
        }

        public override void SetValue(IDbDataParameter parameter, Descricao? value)
        {
            parameter.Value = value?.Valor ?? (object)DBNull.Value;
        }
    }
}
