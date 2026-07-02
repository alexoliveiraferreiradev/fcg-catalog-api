using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.ValueObject
{
    public class Nome : ValueObject<Nome>
    {
        public string Valor { get; }

        public Nome(string valor)
        {
            AssertionConcern.AssertArgumentRealValues(valor, MensagensDominio.NomeJogoNaoReal);
            AssertionConcern.AssertArgumentEmpty(valor, MensagensDominio.JogoNomeObrigatorio);
            AssertionConcern.AssertArgumentLength(valor, 3, 100, MensagensDominio.JogoTamanhoNomeInvalido);
            Valor = valor;
        }

        protected override bool EqualsCore(Nome other)
        {
            return Valor == other.Valor;
        }

        protected override int GetHashCodeCore()
        {
            return Valor.GetHashCode();
        }
    }
}
