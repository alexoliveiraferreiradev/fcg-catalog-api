using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.ValueObject
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
