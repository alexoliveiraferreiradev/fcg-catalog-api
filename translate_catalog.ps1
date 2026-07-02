$path = "c:\Users\conap\source\repos\fcg-catalog-api"

# 1. Rename Folders
$folderRenames = @{
    "src\Fcg.Catalog.Application\Features\Commands\AdicionarJogo" = "AddGame"
    "src\Fcg.Catalog.Application\Features\Commands\AtualizarJogo" = "UpdateGame"
    "src\Fcg.Catalog.Application\Features\Commands\ComprarJogo" = "PurchaseGame"
    "src\Fcg.Catalog.Application\Features\Queries\ObterJogosAdquiridosPorUser" = "GetPurchasedGamesByUser"
    "src\Fcg.Catalog.Application\Features\Queries\ObterJogosPorId" = "GetGamesByIds"
    "src\Fcg.Catalog.Application\Features\Queries\ObterPorId" = "GetGameById"
}

foreach ($key in $folderRenames.Keys) {
    $fullPath = Join-Path $path $key
    if (Test-Path $fullPath) {
        Rename-Item $fullPath -NewName $folderRenames[$key]
    }
}

# 2. Rename Test Folders (if any exist)
$testFolderRenames = @{
    "tests\Fcg.Catalog.Application.Tests\Features\Commands\AdicionarJogo" = "AddGame"
    "tests\Fcg.Catalog.Application.Tests\Features\Commands\AtualizarJogo" = "UpdateGame"
    "tests\Fcg.Catalog.Application.Tests\Features\Commands\ComprarJogo" = "PurchaseGame"
    "tests\Fcg.Catalog.Application.Tests\Features\Queries\ObterJogosAdquiridosPorUser" = "GetPurchasedGamesByUser"
    "tests\Fcg.Catalog.Application.Tests\Features\Queries\ObterJogosPorId" = "GetGamesByIds"
    "tests\Fcg.Catalog.Application.Tests\Features\Queries\ObterPorId" = "GetGameById"
}
foreach ($key in $testFolderRenames.Keys) {
    $fullPath = Join-Path $path $key
    if (Test-Path $fullPath) {
        Rename-Item $fullPath -NewName $testFolderRenames[$key]
    }
}


# 3. Search and Replace in .cs files FIRST (so we replace references before renaming files, otherwise file rename script could miss things if we replace text after renaming? Actually it doesn't matter much if we run Get-ChildItem freshly each time)
$extensions = @('.cs')
Get-ChildItem -Path $path -Recurse -File | Where-Object { $extensions -contains $_.Extension -and $_.FullName -notmatch '\\.git\\' -and $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\Migrations\\' } | ForEach-Object {
    $file = $_.FullName
    $content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
    $original = $content
    
    # Entidades / Enums / Repos / Workers / Handlers
    $content = $content -replace '\bJogo\b', 'Game'
    $content = $content -replace '\bJogos\b', 'Games'
    $content = $content -replace '\bBiblioteca\b', 'Library'
    $content = $content -replace '\bBibliotecas\b', 'Libraries'
    $content = $content -replace '\bPromocao\b', 'Promotion'
    $content = $content -replace '\bPromocoes\b', 'Promotions'
    $content = $content -replace '\bPedido\b', 'Order'
    $content = $content -replace '\bPedidos\b', 'Orders'
    $content = $content -replace '\bPedidoJogo\b', 'OrderGame'
    $content = $content -replace '\bPedidoStatus\b', 'OrderStatus'
    $content = $content -replace '\bGeneroJogo\b', 'GameGenre'
    $content = $content -replace '\bBibliotecaRepository\b', 'LibraryRepository'
    $content = $content -replace '\bIBibliotecaRepository\b', 'ILibraryRepository'
    $content = $content -replace '\bJogoRepository\b', 'GameRepository'
    $content = $content -replace '\bIJogoRepository\b', 'IGameRepository'
    $content = $content -replace '\bDesativaPromocaoInvalidaWorker\b', 'DeactivateInvalidPromotionWorker'
    $content = $content -replace '\bNomeTypeHandler\b', 'NameTypeHandler'
    $content = $content -replace '\bPrecoTypeHandler\b', 'PriceTypeHandler'
    $content = $content -replace '\bDescricaoTypeHandler\b', 'DescriptionTypeHandler'
    
    # Value Objects & Props
    $content = $content -replace '\bPreco\b', 'Price'
    $content = $content -replace '\bDescricao\b', 'Description'
    $content = $content -replace '\bPeriodo\b', 'Period'
    $content = $content -replace '\bNome\b', 'Name'
    
    $content = $content -replace '\bJogoId\b', 'GameId'
    $content = $content -replace '\bBibliotecaId\b', 'LibraryId'
    $content = $content -replace '\bPromocaoId\b', 'PromotionId'
    $content = $content -replace '\bPedidoId\b', 'OrderId'
    $content = $content -replace '\bUsuarioId\b', 'UserId'
    $content = $content -replace '\bValor\b', 'Amount'
    $content = $content -replace '\bPreco\b', 'Price'
    $content = $content -replace '\bDataCadastro\b', 'CreatedAt'
    $content = $content -replace '\bDataAlteracao\b', 'UpdatedAt'
    $content = $content -replace '\bDataInicio\b', 'StartDate'
    $content = $content -replace '\bDataFim\b', 'EndDate'
    $content = $content -replace '\bAtivo\b', 'IsActive'
    
    # Enum Values
    $content = $content -replace '\bAguardandoPagamento\b', 'AwaitingPayment'
    $content = $content -replace '\bAprovado\b', 'Approved'
    $content = $content -replace '\bRecusado\b', 'Rejected'
    
    # Events
    $content = $content -replace '\bBibliotecaEvent\b', 'LibraryEvent'
    $content = $content -replace '\bJogoAdicionadoEvent\b', 'GameAddedEvent'
    $content = $content -replace '\bJogoAtualizadoEvent\b', 'GameUpdatedEvent'
    $content = $content -replace '\bJogoDesativadoEvent\b', 'GameDeactivatedEvent'
    $content = $content -replace '\bJogoReativadoEvent\b', 'GameReactivatedEvent'
    $content = $content -replace '\bPromocaoAdicionadaEvent\b', 'PromotionAddedEvent'
    $content = $content -replace '\bPromocaoAtualizadaEvent\b', 'PromotionUpdatedEvent'
    $content = $content -replace '\bPromocaoDesativadaEvent\b', 'PromotionDeactivatedEvent'
    $content = $content -replace '\bPromocaoInvalidaEvent\b', 'InvalidPromotionEvent'
    
    # Methods
    $content = $content -replace '\bAdicionar\b', 'Add'
    $content = $content -replace '\bAtualizar\b', 'Update'
    $content = $content -replace '\bObterPorId\b', 'GetById'
    $content = $content -replace '\bObterJogosAdquiridosPorUsuario\b', 'GetPurchasedGamesByUser'
    $content = $content -replace '\bVerificaSeUsuarioPossuiJogo\b', 'CheckIfUserOwnsGame'
    $content = $content -replace '\bExisteJogoComNome\b', 'GameExistsWithName'
    $content = $content -replace '\bObterPromocaoPorId\b', 'GetPromotionById'
    $content = $content -replace '\bObterJogosPorIds\b', 'GetGamesByIds'
    $content = $content -replace '\bDesativaPromocoesInvalidas\b', 'DeactivateInvalidPromotions'
    
    # Commands/Queries
    $content = $content -replace '\bAdicionarJogoCommand\b', 'AddGameCommand'
    $content = $content -replace '\bAdicionarJogoCommandHandler\b', 'AddGameCommandHandler'
    $content = $content -replace '\bAtualizarJogoCommand\b', 'UpdateGameCommand'
    $content = $content -replace '\bAtualizarJogoCommandHandler\b', 'UpdateGameCommandHandler'
    $content = $content -replace '\bComprarJogoCommand\b', 'PurchaseGameCommand'
    $content = $content -replace '\bComprarJogoCommandHandler\b', 'PurchaseGameCommandHandler'
    $content = $content -replace '\bObterJogosAdquiridosPorUserQuery\b', 'GetPurchasedGamesByUserQuery'
    $content = $content -replace '\bObterJogosAdquiridosPorUserQueryHandler\b', 'GetPurchasedGamesByUserQueryHandler'
    $content = $content -replace '\bObterJogosPorIdQuery\b', 'GetGamesByIdsQuery'
    $content = $content -replace '\bObterJogosPorIdQueryHandler\b', 'GetGamesByIdsQueryHandler'
    $content = $content -replace '\bObterJogoPorIdQuery\b', 'GetGameByIdQuery'
    $content = $content -replace '\bObterJogoPorIdQueryHandler\b', 'GetGameByIdQueryHandler'

    if ($content -cne $original) {
        [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
    }
}

# 4. Rename Files
Get-ChildItem -Path $path -Recurse -File | Where-Object { $extensions -contains $_.Extension -and $_.FullName -notmatch '\\.git\\' -and $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\Migrations\\' } | ForEach-Object {
    $newName = $_.Name -replace 'Jogo', 'Game' `
                       -replace 'Jogos', 'Games' `
                       -replace 'Biblioteca', 'Library' `
                       -replace 'Bibliotecas', 'Libraries' `
                       -replace 'Promocao', 'Promotion' `
                       -replace 'Promocoes', 'Promotions' `
                       -replace 'Pedido', 'Order' `
                       -replace 'Pedidos', 'Orders' `
                       -replace 'Nome', 'Name' `
                       -replace 'Descricao', 'Description' `
                       -replace 'Preco', 'Price' `
                       -replace 'Periodo', 'Period' `
                       -replace 'GeneroGame', 'GameGenre' `
                       -replace 'OrderStatus', 'OrderStatus' `
                       -replace 'Adicionado', 'Added' `
                       -replace 'Atualizado', 'Updated' `
                       -replace 'Desativado', 'Deactivated' `
                       -replace 'Reativado', 'Reactivated' `
                       -replace 'Adicionada', 'Added' `
                       -replace 'Atualizada', 'Updated' `
                       -replace 'Desativada', 'Deactivated' `
                       -replace 'Invalida', 'Invalid' `
                       -replace 'Desativa', 'Deactivate' `
                       -replace 'Adicionar', 'Add' `
                       -replace 'Atualizar', 'Update' `
                       -replace 'Comprar', 'Purchase' `
                       -replace 'Obter', 'Get' `
                       -replace 'AdquiridosPorUser', 'PurchasedByUser' `
                       -replace 'PorId', 'ById' `
                       -replace 'PorIds', 'ByIds'

    if ($_.Name -cne $newName) {
        Rename-Item $_.FullName -NewName $newName
    }
}
