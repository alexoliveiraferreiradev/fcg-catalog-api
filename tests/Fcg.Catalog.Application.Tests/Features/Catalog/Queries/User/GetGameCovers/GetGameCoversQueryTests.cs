using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Catalog.Queries.User.GetGameCovers;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Resources;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Queries.User.GetGameCovers
{
    public class GetGameCoversQueryTests
    {
        private readonly GetGameCoversQueryValidator _validator = new();
        private readonly Mock<IGameQueryRepository> _gameQueryRepositoryMock = new();

        [Fact]
        public void Validar_SemIds_DeveApontarErro()
        {
            // Act
            var resultado = _validator.Validate(new GetGameCoversQuery([]));

            // Assert
            resultado.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(CatalogMessages.GameCoversIdsRequired);
        }

        [Fact]
        public void Validar_ComMaisDe50IdsDistintos_DeveApontarErro()
        {
            // Arrange
            var ids = Enumerable.Range(0, GetGameCoversQuery.MaxIds + 1).Select(_ => Guid.NewGuid()).ToList();

            // Act
            var resultado = _validator.Validate(new GetGameCoversQuery(ids));

            // Assert
            resultado.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(CatalogMessages.GameCoversIdsLimit);
        }

        [Fact]
        public void Validar_Com50IdsDistintosEUmRepetido_DeveAceitar()
        {
            // Arrange
            var ids = Enumerable.Range(0, GetGameCoversQuery.MaxIds).Select(_ => Guid.NewGuid()).ToList();
            ids.Add(ids[0]);

            // Act
            var resultado = _validator.Validate(new GetGameCoversQuery(ids));

            // Assert
            resultado.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ComIdsRepetidos_DeveConsultarIdsDistintosEDevolverResultadoDoRepositorio()
        {
            // Arrange
            var id = Guid.NewGuid();
            var esperado = new List<GameCoverResponse> { new() { GameId = id, Name = "Pixel Pals", CoverUrl = "/covers/pixel-pals.svg" } };
            _gameQueryRepositoryMock
                .Setup(r => r.GetGameCoversAsync(It.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 1 && ids.Contains(id)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(esperado);
            var handler = new GetGameCoversQueryHandler(_gameQueryRepositoryMock.Object);

            // Act
            var resultado = await handler.Handle(new GetGameCoversQuery([id, id]), CancellationToken.None);

            // Assert
            resultado.Should().BeEquivalentTo(esperado);
        }
    }
}
