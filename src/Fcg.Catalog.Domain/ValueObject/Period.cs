using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.ValueObject
{
    public class Period : ValueObject<Period>
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public Period(DateTime StartDate, DateTime EndDate)
        {
            if (EndDate <= StartDate) throw new DomainException(DomainMessages.EndDateInvalid);
            this.StartDate = StartDate;
            this.EndDate = EndDate;
        }
        protected Period() { }
        public Period(DateTime EndDate) : this(DateTime.UtcNow, EndDate) { }

        protected override bool EqualsCore(Period other)
        {
            return StartDate == other.StartDate && EndDate == other.EndDate;
        }

        protected override int GetHashCodeCore()
        {
            return HashCode.Combine(StartDate, EndDate);
        }
    }
}
