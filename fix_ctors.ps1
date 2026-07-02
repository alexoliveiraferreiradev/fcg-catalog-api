$paths = @("c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Domain\Entities", "c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Domain\ValueObject", "c:\Users\conap\source\repos\fcg-catalog-api\src\Fcg.Catalog.Application\Features")

foreach ($path in $paths) {
    Get-ChildItem -Path $path -Recurse -File -Filter *.cs | ForEach-Object {
        $file = $_.FullName
        $content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
        $original = $content
        
        $content = $content -replace '\bName = Name;\b', 'Name = name;'
        $content = $content -replace '\bDescription = Description;\b', 'Description = description;'
        $content = $content -replace '\bBasePrice = BasePrice;\b', 'BasePrice = basePrice;'
        $content = $content -replace '\bStartDate = StartDate;\b', 'StartDate = startDate;'
        $content = $content -replace '\bEndDate = EndDate;\b', 'EndDate = endDate;'
        $content = $content -replace '\bUserId = UserId;\b', 'UserId = userId;'
        $content = $content -replace '\bGameId = GameId;\b', 'GameId = gameId;'
        $content = $content -replace '\bGame = Game;\b', 'Game = game;'
        $content = $content -replace '\bAmount = Amount;\b', 'Amount = amount;'
        $content = $content -replace '\bGenre = Genre;\b', 'Genre = genre;'
        $content = $content -replace '\bOrderId = OrderId;\b', 'OrderId = orderId;'
        $content = $content -replace '\bOrder = Order;\b', 'Order = order;'
        $content = $content -replace '\bStatus = Status;\b', 'Status = status;'
        $content = $content -replace '\bPromotionId = PromotionId;\b', 'PromotionId = promotionId;'
        
        # also fixing Some parameter names like precoJogo -> basePrice
        $content = $content -replace '\bName = nomeJogo;\b', 'Name = name;'
        $content = $content -replace '\bDescription = descricaoJogo;\b', 'Description = description;'
        $content = $content -replace '\bBasePrice = precoJogo;\b', 'BasePrice = basePrice;'
        
        # update command/query fields assigned to themselves
        $content = $content -replace '\bName = name;\b', 'this.Name = name;'
        $content = $content -replace '\bDescription = description;\b', 'this.Description = description;'
        
        if ($content -cne $original) {
            [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
            Write-Output "Updated $file"
        }
    }
}
