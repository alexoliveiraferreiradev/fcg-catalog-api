$paths = @("c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Infrastructure", "c:\Users\conap\source\repos\fcg-catalog-api\tests")

foreach ($path in $paths) {
    Get-ChildItem -Path $path -Recurse -File -Filter *.cs | ForEach-Object {
        $file = $_.FullName
        $content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
        $original = $content
        
        $content = $content -replace '\.Name\.Amount', '.Name.Value'
        $content = $content -replace '\.Description\.Amount', '.Description.Value'
        $content = $content -replace 'Name\.Amount', 'Name.Value'
        $content = $content -replace 'Description\.Amount', 'Description.Value'
        
        if ($content -cne $original) {
            [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
            Write-Output "Updated $file"
        }
    }
}

# Fix GameMapping specifically
$mappingFile = "c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Infrastructure\Persistence\Mappings\GameMapping.cs"
$content = [System.IO.File]::ReadAllText($mappingFile, [System.Text.Encoding]::UTF8)
$content = $content -replace 'Name =>', 'nameBuilder =>' -replace 'Name\.Property', 'nameBuilder.Property'
$content = $content -replace 'Price =>', 'priceBuilder =>' -replace 'Price\.Property', 'priceBuilder.Property'
[System.IO.File]::WriteAllText($mappingFile, $content, [System.Text.Encoding]::UTF8)
Write-Output "Fixed GameMapping"
