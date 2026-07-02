$path = "c:\Users\conap\source\repos\fcg-catalog-api"

$extensions = @('.cs')
Get-ChildItem -Path $path -Recurse -File | Where-Object { $extensions -contains $_.Extension -and $_.FullName -notmatch '\\.git\\' -and $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\Migrations\\' } | ForEach-Object {
    $file = $_.FullName
    $content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
    $original = $content
    
    # Backing fields
    $content = $content -replace '_jogos', '_games'
    
    if ($content -cne $original) {
        [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
        Write-Output "Updated $file"
    }
}
