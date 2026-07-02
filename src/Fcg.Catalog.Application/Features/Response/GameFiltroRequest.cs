using Fcg.Catalog.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Application.Features.Response
{
    public record JogoFiltroRequest
    {
        public int Pagina { get; init; } = 1;
        public int Tamanho { get; init; } = 10;
        public string? Busca { get; init; }
        public GameGenre? Genre { get; init; }
        public bool? ApenasPromovidos { get; init; }
    }
}
