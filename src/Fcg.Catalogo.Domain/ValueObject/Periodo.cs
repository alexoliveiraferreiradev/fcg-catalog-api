using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalogo.Domain.ValueObject
{
    public class Periodo : ValueObject<Periodo>
    {
        public DateTime DataInicio { get; }
        public DateTime DataFim { get; }

        public Periodo(DateTime dataInicio, DateTime dataFim)
        {
            if (dataFim <= dataInicio) throw new DomainException(MensagensDominio.DataFimInvalida);
            DataInicio = dataInicio;
            DataFim = dataFim;
        }
        protected Periodo() { }
        public Periodo(DateTime dataFim) : this(DateTime.UtcNow, dataFim) { }

        protected override bool EqualsCore(Periodo other)
        {
            return DataInicio == other.DataInicio && DataFim == other.DataFim;
        }

        protected override int GetHashCodeCore()
        {
            return HashCode.Combine(DataInicio, DataFim);
        }
    }
}
