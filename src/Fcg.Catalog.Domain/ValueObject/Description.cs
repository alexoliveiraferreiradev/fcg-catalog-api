using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.ValueObject
{
    public class Description : ValueObject<Description>
    {
        public string Value { get; set; }

        public Description(string value)
        {
            AssertionConcern.AssertArgumentRealValues(value, DomainMessages.GameDescriptionNotReal);
            AssertionConcern.AssertArgumentEmpty(value, DomainMessages.GameDescriptionRequired);
            AssertionConcern.AssertArgumentLength(value, 5, 500, DomainMessages.GameDescriptionLengthInvalid);
            this.Value = value;
        }

        protected override bool EqualsCore(Description other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}
