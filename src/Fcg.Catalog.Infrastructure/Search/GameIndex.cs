using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;

namespace Fcg.Catalog.Infrastructure.Search
{
    /// <summary>Nome e mapping do índice de jogos: única fonte de verdade sobre o formato do índice.</summary>
    internal static class GameIndex
    {
        public const string Name = "games";

        /// <summary>
        /// Versão do mapping, gravada no _meta do índice. Ao subir, o initializer recria o índice quando a versão
        /// gravada é diferente desta (ou não existe, caso de índice criado automaticamente pelo Elasticsearch).
        /// Aumente sempre que mudar o Configure: analyzer, campo, tipo ou subcampo.
        /// </summary>
        public const int MappingVersion = 1;
        public const string MappingVersionKey = "mapping_version";

        /// <summary>Ensina o client a gravar GameDocument no índice "games" usando o Id do jogo como _id.</summary>
        public static ElasticsearchClientSettings MapDocument(ElasticsearchClientSettings settings) =>
            settings.DefaultMappingFor<GameDocument>(m => m
                .IndexName(Name)
                .IdProperty(d => d.Id));

        // Analyzers
        public const string FoldingAnalyzer = "folding";                 // sem stemmer: minúsculas e sem acento
        public const string EnglishStemAnalyzer = "english_stem";        // para o nome (jogos com nome em inglês)
        public const string PortugueseStemAnalyzer = "portuguese_stem";  // para a descrição (texto em português)

        // Filtros de stemmer
        private const string EnglishPossessiveFilter = "english_possessive";      // "Assassin's" -> "assassin"
        private const string EnglishLightStemFilter = "english_light_stem";       // plural e sufixos leves: "knights" -> "knight"
        private const string PortugueseLightStemFilter = "portuguese_light_stem"; // plural e gênero: "corridas" -> "corrida"

        // Subcampo com stemmer: "name.stemmed" e "description.stemmed".
        public const string StemmedSubField = "stemmed";

        // Campos da busca com peso. Nome e descrição existem em duas versões: a original (folding) e a com stemmer.
        // O multi_match (best_fields) usa a maior nota entre os campos, e a versão original pesa mais:
        // quem digitou a palavra como está no jogo fica acima de quem só bateu pelo radical.
        public static readonly string[] SearchFields =
        [
            "name^3",
            $"name.{StemmedSubField}^2",
            "genre^2",
            "description",
            $"description.{StemmedSubField}^0.5"
        ];

        public static void Configure(CreateIndexRequestDescriptor<GameDocument> index) => index
            .Settings(s => s.Analysis(a => a
                .TokenFilters(tf => tf
                    .Stemmer(EnglishPossessiveFilter, st => st.Language("possessive_english"))
                    .Stemmer(EnglishLightStemFilter, st => st.Language("light_english"))
                    .Stemmer(PortugueseLightStemFilter, st => st.Language("light_portuguese")))               
                .Analyzers(an => an
                    .Custom(FoldingAnalyzer, ca => ca.Tokenizer("standard")
                        .Filter(["lowercase", "asciifolding"]))
                    .Custom(EnglishStemAnalyzer, ca => ca.Tokenizer("standard")
                        .Filter(["lowercase", EnglishPossessiveFilter, EnglishLightStemFilter, "asciifolding"]))
                    .Custom(PortugueseStemAnalyzer, ca => ca.Tokenizer("standard")
                        .Filter(["lowercase", PortugueseLightStemFilter, "asciifolding"])))))
            .Mappings(m => m
                .Meta(new Dictionary<string, object> { [MappingVersionKey] = MappingVersion })
                .Properties(p => p
                .Keyword(d => d.Id)
                .Text(d => d.Name, t => t
                    .Analyzer(FoldingAnalyzer)
                    .Fields(f => f.Text(StemmedSubField, st => st.Analyzer(EnglishStemAnalyzer))))
                .Text(d => d.Description, t => t
                    .Analyzer(FoldingAnalyzer)
                    .Fields(f => f.Text(StemmedSubField, st => st.Analyzer(PortugueseStemAnalyzer))))
                .Text(d => d.Genre, t => t.Analyzer(FoldingAnalyzer))
                .Keyword(d => d.CoverUrl, k => k.Index(false))
                .DoubleNumber(d => d.OriginalPrice)
                .DoubleNumber(d => d.PromotionPrice)
                .Date(d => d.PromotionStartDate)
                .Date(d => d.PromotionEndDate)
                .Boolean(d => d.IsActive)
                .DoubleNumber(d => d.AverageRating)
                .IntegerNumber(d => d.RatingCount)));
    }
}
