$folderRenames = @(
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.API\Endpoints\Usuario', 'User'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Biblioteca', 'Library'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Library\Queries\ObtemBibliotecaPaginada', 'GetPagedLibrary'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Commands\Admin\AdicionarJogo', 'AddGame'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Commands\Admin\AdicionarPromocaoJogo', 'AddPromotionGame'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Commands\Admin\AtualizarJogo', 'UpdateGame'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Commands\Admin\AtualizarPromocao', 'UpdatePromotion'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Commands\Admin\DesativarJogo', 'DeactivateGame'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Commands\Admin\DesativarPromocao', 'DeactivatePromotion'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Commands\Admin\DesativarPromocaoInvalida', 'DeactivateInvalidPromotion'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Commands\Admin\ReativarJogo', 'ReactivateGame'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Queries\ObtemCatalogJogosPromovidosPaginado', 'GetPagedPromotedCatalogGames'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Queries\ObtemCatalogPaginados', 'GetPagedCatalog'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Queries\ObtemCatalogPorGeneroPaginado', 'GetPagedCatalogByGenre'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Queries\ObtemTodosJogos', 'GetAllGames'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Queries\ObterJogoPorId', 'GetGameById'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Queries\ObterPromocaoPorJogoId', 'GetPromotionByGameId'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Catalog\Queries\VerificaDuplicidadeDoNome', 'CheckNameDuplicity'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Pedidos', 'Orders'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Orders\Commands\RealizarPedido', 'PlaceOrder'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Orders\Query\ObtemHistoricoPedido', 'GetOrderHistory'),
    @('c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features\Orders\Query', 'Queries')
)

foreach ($pair in $folderRenames) {
    $oldPath = $pair[0]
    $newName = $pair[1]
    if (Test-Path $oldPath) {
        Rename-Item -Path $oldPath -NewName $newName -Force
        Write-Host "Renamed $oldPath to $newName"
    } else {
        Write-Host "Path not found: $oldPath"
    }
}

$fileRenames = @{
    'GameUsuarioResponse.cs' = 'GameUserResponse.cs';
}

$dir = "c:\Users\conap\source\repos\fcg-catalog-api"
foreach ($oldFile in $fileRenames.Keys) {
    Get-ChildItem -Path $dir -Filter $oldFile -Recurse | ForEach-Object {
        $newName = $fileRenames[$oldFile]
        Rename-Item -Path $_.FullName -NewName $newName -Force
        Write-Host "Renamed $_.FullName to $newName"
    }
}
