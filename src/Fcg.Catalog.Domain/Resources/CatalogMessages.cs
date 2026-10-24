namespace Fcg.Catalog.Domain.Resources
{
    /// <summary>
    /// Mensagens só do Catalog. As compartilhadas ficam em Fcg.Core.Abstractions.Resources.DomainMessages.
    /// </summary>
    public static class CatalogMessages
    {
        #region Capa
        public const string CoverUrlRequired = "O link da capa não pode ser vazio. Para remover a capa, envie null.";
        public const string CoverUrlLengthInvalid = "O link da capa deve ter no máximo 500 caracteres.";
        public const string CoverUrlFormatInvalid = "O link da capa deve ser um caminho do site (ex.: /covers/jogo.svg) ou uma URL http(s).";
        #endregion

        #region Capas em lote
        public const string GameCoversIdsRequired = "Informe ao menos um id de jogo.";
        public const string GameCoversIdsLimit = "Informe no máximo 50 ids de jogo por consulta.";
        #endregion
    }
}
