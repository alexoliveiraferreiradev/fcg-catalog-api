using Fcg.Catalog.Domain.Resources;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;

namespace Fcg.Catalog.Domain.ValueObject
{
    /// <summary>
    /// Onde a capa do jogo está: caminho do site ("/covers/jogo.svg") ou URL http(s).
    /// O Catalog guarda só o link, nunca a imagem.
    /// </summary>
    public class CoverUrl : ValueObject<CoverUrl>
    {
        public const int MaxLength = 500;

        public string Value { get; set; }

        public CoverUrl(string value)
        {
            AssertionConcern.AssertArgumentEmpty(value, CatalogMessages.CoverUrlRequired);
            AssertionConcern.AssertArgumentLength(value, MaxLength, CatalogMessages.CoverUrlLengthInvalid);

            var trimmed = value.Trim();
            if (!IsSitePath(trimmed) && !IsHttpUrl(trimmed))
                throw new DomainException(CatalogMessages.CoverUrlFormatInvalid);

            Value = trimmed;
        }

        /// <summary>null continua null (jogo sem capa); qualquer outro valor passa pela validação.</summary>
        public static CoverUrl? FromNullable(string? value) => value is null ? null : new CoverUrl(value);

        public static bool IsValidFormat(string value)
        {
            var trimmed = value.Trim();
            return IsSitePath(trimmed) || IsHttpUrl(trimmed);
        }

        // "/covers/x.svg" sim; "//host/x.svg" e "/\host/x.svg" não: o navegador lê os dois como URL de outro host.
        private static bool IsSitePath(string value) =>
            value.StartsWith('/') && !value.StartsWith("//") && !value.StartsWith("/\\");

        private static bool IsHttpUrl(string value) =>
            Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        protected override bool EqualsCore(CoverUrl other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}
