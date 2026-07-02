$replacements = @{
    'GameUsuarioResponse' = 'GameUserResponse';
    '\.Usuario\b' = '.User';
    'namespace Fcg.Catalog.API.Endpoints.Usuario' = 'namespace Fcg.Catalog.API.Endpoints.User';
    '\.Biblioteca' = '.Library';
    '\.ObtemBibliotecaPaginada' = '.GetPagedLibrary';
    '\.AdicionarJogo' = '.AddGame';
    '\.AdicionarPromocaoJogo' = '.AddPromotionGame';
    '\.AtualizarJogo' = '.UpdateGame';
    '\.AtualizarPromocao' = '.UpdatePromotion';
    '\.DesativarJogo' = '.DeactivateGame';
    '\.DesativarPromocao' = '.DeactivatePromotion';
    '\.DesativarPromocaoInvalida' = '.DeactivateInvalidPromotion';
    '\.ReativarJogo' = '.ReactivateGame';
    '\.ObtemCatalogJogosPromovidosPaginado' = '.GetPagedPromotedCatalogGames';
    '\.ObtemCatalogPaginados' = '.GetPagedCatalog';
    '\.ObtemCatalogPorGeneroPaginado' = '.GetPagedCatalogByGenre';
    '\.ObtemTodosJogos' = '.GetAllGames';
    '\.ObterJogoPorId' = '.GetGameById';
    '\.ObterPromocaoPorJogoId' = '.GetPromotionByGameId';
    '\.VerificaDuplicidadeDoNome' = '.CheckNameDuplicity';
    '\.Pedidos' = '.Orders';
    '\.RealizarPedido' = '.PlaceOrder';
    '\.ObtemHistoricoPedido' = '.GetOrderHistory';
}

$dir = "c:\Users\conap\source\repos\fcg-catalog-api"
Get-ChildItem -Path $dir -Include *.cs -Recurse | ForEach-Object {
    $file = $_.FullName
    $content = Get-Content $file -Raw
    $changed = $false
    
    foreach ($key in $replacements.Keys) {
        $val = $replacements[$key]
        if ($content -match $key) {
            $content = $content -replace $key, $val
            $changed = $true
        }
    }
    
    if ($changed) {
        Write-Host "Updating $file"
        Set-Content -Path $file -Value $content -Encoding UTF8 -NoNewline
    }
}
