$replacements = @{
    'AdicionarPromocaoJogo' = 'AddPromotionGame';
    'AdicionarJogo' = 'AddGame';
    'AtualizarJogo' = 'UpdateGame';
    'AtualizarPromocao' = 'UpdatePromotion';
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
        Write-Host "Updating text in $file"
        Set-Content -Path $file -Value $content -Encoding UTF8 -NoNewline
    }
}

$filesRenamed = $true
while ($filesRenamed) {
    $filesRenamed = $false
    $allFiles = Get-ChildItem -Path $dir -Include *.cs -Recurse
    foreach ($file in $allFiles) {
        $oldName = $file.Name
        $newName = $oldName
        foreach ($key in $replacements.Keys) {
            $val = $replacements[$key]
            if ($newName -match $key) {
                $newName = $newName -replace $key, $val
            }
        }
        if ($newName -ne $oldName) {
            Rename-Item -Path $file.FullName -NewName $newName -Force
            Write-Host "Renamed file $oldName to $newName"
            $filesRenamed = $true
            break
        }
    }
}
