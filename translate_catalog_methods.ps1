$path = "c:\Users\conap\source\repos\fcg-catalog-api"

$extensions = @('.cs')
Get-ChildItem -Path $path -Recurse -File | Where-Object { $extensions -contains $_.Extension -and $_.FullName -notmatch '\\.git\\' -and $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\Migrations\\' } | ForEach-Object {
    $file = $_.FullName
    $content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
    $original = $content
    
    # Backing fields
    $content = $content -replace '_promocoes', '_promotions'
    
    # Properties & Fields
    $content = $content -replace '\bPrecoBase\b', 'BasePrice'
    $content = $content -replace '\bGenero\b', 'Genre'
    
    # Methods
    $content = $content -replace '\bDesativar\b', 'Deactivate'
    $content = $content -replace '\bReativar\b', 'Reactivate'
    $content = $content -replace '\bAtualizarNome\b', 'UpdateName'
    $content = $content -replace '\bAtualizarDescricao\b', 'UpdateDescription'
    $content = $content -replace '\bAtualizarPreco\b', 'UpdatePrice'
    $content = $content -replace '\bAtualizarGenero\b', 'UpdateGenre'
    $content = $content -replace '\bAdicionarPromocao\b', 'AddPromotion'
    $content = $content -replace '\bAlteraPromocao\b', 'UpdatePromotion'
    $content = $content -replace '\bObterPrecoAtual\b', 'GetCurrentPrice'
    $content = $content -replace '\bDesativarPromocao\b', 'DeactivatePromotion'
    $content = $content -replace '\bAtualizarPromocao\b', 'UpdatePromotion'
    $content = $content -replace '\bEstaValida\b', 'IsValid'
    $content = $content -replace '\bAtualizarStatus\b', 'UpdateStatus'
    $content = $content -replace '\bObterDescricao\b', 'GetDescription'
    $content = $content -replace '\bDesativarInvalidas\b', 'DeactivateInvalid'
    
    # EF Mapping Properties in CatalogDbContext / Maps
    $content = $content -replace '\bBibliotecas\b', 'Libraries'
    $content = $content -replace '\bJogos\b', 'Games'
    $content = $content -replace '\bPromocoes\b', 'Promotions'
    $content = $content -replace '\bPedidos\b', 'Orders'
    
    if ($content -cne $original) {
        [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
        Write-Output "Updated $file"
    }
}
