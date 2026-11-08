using Fcg.Catalog.Domain.Resources;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;

namespace Fcg.Catalog.Domain.ValueObject
{
    /// <summary>Nota que um usuário deu para um jogo, de 0 a 5.</summary>
    public class Avaliation : ValueObject<Avaliation>
    {
        public const double MinValue = 0;
        public const double MaxValue = 5;

        public double Value { get; private set; }

        public Avaliation(double value)
        {
            if (double.IsNaN(value) || value < MinValue || value > MaxValue)
                throw new DomainException(CatalogMessages.RatingOutOfRange);

            Value = value;
        }

        protected override bool EqualsCore(Avaliation other)
        {
            return Value == other.Value;
        }
        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}
