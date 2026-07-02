using Fcg.Catalog.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Application.Features.Response
{
    public record GameFilterRequest
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? Search { get; init; }
        public GameGenre? Genre { get; init; }
        public bool? OnlyPromoted { get; init; }
    }
}
