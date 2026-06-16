using Fcg.Catalogo.Domain.Common;
using Fcg.Catalogo.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalogo.Domain.ValueObject
{
    public class Preco : ValueObject<Preco>
    {
        public decimal Valor { get; }

        public Preco(decimal valor)
        {
            AssertionConcern.AssertArgumentValueFormat(valor, MensagensDominio.ValorInvalido);
            Valor = valor;
        }

        protected override bool EqualsCore(Preco other)
        {
            return Valor == other.Valor;
        }

        protected override int GetHashCodeCore()
        {
            return Valor.GetHashCode();
        }
    }
}
