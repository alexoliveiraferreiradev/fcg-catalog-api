using Fcg.Catalogo.Domain.Resources;
using Fcg.Core.Abstractions.Common;

namespace Fcg.Catalogo.Domain.ValueObject
{
    public class Descricao : ValueObject<Descricao>
    {
        public string Valor { get; set; }

        public Descricao(string valor)
        {
            AssertionConcern.AssertArgumentRealValues(valor, MensagensDominio.DescricaoJogoNaoReal);
            AssertionConcern.AssertArgumentEmpty(valor, MensagensDominio.JogoDescricaoObrigatoria);
            AssertionConcern.AssertArgumentLength(valor, 5, 500, MensagensDominio.JogoDescricaoTamanhoInvalido);
            Valor = valor;
        }

        protected override bool EqualsCore(Descricao other)
        {
            return Valor == other.Valor;
        }

        protected override int GetHashCodeCore()
        {
            return Valor.GetHashCode();
        }
    }
}
