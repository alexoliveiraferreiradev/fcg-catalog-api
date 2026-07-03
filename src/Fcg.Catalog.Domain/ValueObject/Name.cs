using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.ValueObject
{
    public class Name : ValueObject<Name>
    {
        public string Value { get; }

        public Name(string value)
        {
            AssertionConcern.AssertArgumentRealValues(value, DomainMessages.GameNameNotReal);
            AssertionConcern.AssertArgumentEmpty(value, DomainMessages.GameNameRequired);
            AssertionConcern.AssertArgumentLength(value, 3, 100, DomainMessages.GameNameLengthInvalid);
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
