using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.ValueObject
{
    public class Name : ValueObject<Name>
    {
        public string Value { get; }

        public Name(string value)
        {
            AssertionConcern.AssertArgumentRealValues(value, MensagensDominio.NomeJogoNaoReal);
            AssertionConcern.AssertArgumentEmpty(value, MensagensDominio.JogoNomeObrigatorio);
            AssertionConcern.AssertArgumentLength(value, 3, 100, MensagensDominio.JogoTamanhoNomeInvalido);
            this.Value = value;
        }

        protected override bool EqualsCore(Name other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}
