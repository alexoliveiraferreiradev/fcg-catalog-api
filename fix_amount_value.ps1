$paths = @("c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features")

foreach ($path in $paths) {
    Get-ChildItem -Path $path -Recurse -File -Filter *.cs | ForEach-Object {
        $file = $_.FullName
        $content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
        $original = $content
        
        $content = $content -replace '\.Name\.Amount', '.Name.Value'
        $content = $content -replace '\.Description\.Amount', '.Description.Value'
        $content = $content -replace 'Name\.Amount', 'Name.Value'
        $content = $content -replace 'Description\.Amount', 'Description.Value'
        $content = $content -replace '\bAdicionarItem\b', 'AddItem'
        $content = $content -replace '\bFinalizarPedido\b', 'FinalizeOrder'
        
        if ($content -cne $original) {
            [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
            Write-Output "Updated $file"
        }
    }
}
