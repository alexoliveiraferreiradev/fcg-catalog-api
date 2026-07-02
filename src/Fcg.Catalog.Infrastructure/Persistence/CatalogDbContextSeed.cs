using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Persistence
{
    public static class CatalogDbContextSeed
    {        
        public static async Task SeedDataAsync(CatalogDbContext context)
        {
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            if (!await context.Jogos.AnyAsync())
            {
                context.Jogos.AddRange(
                    new Jogo(new Nome("Elden Ring"), new Descricao("Levante-se, Maculado, e seja guiado pela graça para portar o poder do Anel Pratinado e se tornar um Lorde Pratinado nas Terras Intermédias."), new Preco(199.00m), GeneroJogo.Soulslike),
                    new Jogo(new Nome("Counter-Strike 2"), new Descricao("A evolução tática do shooter em primeira pessoa mais popular do mundo, com gráficos aprimorados e mapas reaginados."), new Preco(0.00m), GeneroJogo.FPS),
                    new Jogo(new Nome("Cyberpunk 2077"), new Descricao("Um RPG de ação e aventura em mundo aberto ambientado na megalópole de Night City, onde você joga como um mercenário urbano."), new Preco(199.00m), GeneroJogo.RPG),
                    new Jogo(new Nome("The Witcher 3: Wild Hunt"), new Descricao("Torne-se o bruxo Geralt de Rivia e embarque em uma jornada épica em busca da criança da profecia em um mundo aberto devastado pela guerra."), new Preco(129.90m), GeneroJogo.RPG),
                    new Jogo(new Nome("Stardew Valley"), new Descricao("Você herdou a antiga fazenda do seu avô. Com ferramentas usadas e algumas moedas, comece sua nova vida no campo!"), new Preco(24.99m), GeneroJogo.Simulacao),
                    new Jogo(new Nome("Red Dead Redemption 2"), new Descricao("Uma saga épica sobre a vida no coração imperdoável da América no final do século XIX, seguindo o fora-da-lei Arthur Morgan."), new Preco(239.00m), GeneroJogo.Acao),
                    new Jogo(new Nome("God of War Ragnarök"), new Descricao("Kratos e Atreus devem embarcar em uma jornada mítica pelos Nove Reinos em busca de respostas enquanto as forças asgardianas se preparam para a batalha profetizada."), new Preco(299.90m), GeneroJogo.Acao),
                    new Jogo(new Nome("Hades"), new Descricao("Desafie o deus dos mortos enquanto batalha para escapar do Submundo da mitologia grega neste aclamado RPG roguelike."), new Preco(73.99m), GeneroJogo.Roguelike),
                    new Jogo(new Nome("Baldur's Gate 3"), new Descricao("Reúna seu grupo e retorne aos Reinos Esquecidos em uma história de companheirismo e traição, sacrifício e sobrevivência, e a atração do poder absoluto."), new Preco(199.99m), GeneroJogo.RPG),
                    new Jogo(new Nome("Minecraft"), new Descricao("Explore mundos infinitos, construa desde simples casas a castelos grandiosos. Jogue no modo criativo com recursos ilimitados ou sobreviva no modo sobrevivência."), new Preco(99.00m), GeneroJogo.MundoAberto),
                    new Jogo(new Nome("Forza Horizon 5"), new Descricao("Sua aventura Horizon definitiva aguarda! Explore as paisagens vibrantes e em constante evolução do México com ação de pilotagem ilimitada e divertida."), new Preco(249.00m), GeneroJogo.Corrida),
                    new Jogo(new Nome("Valorant"), new Descricao("Misture mira precisa com habilidades táticas únicas neste shooter tático 5v5 focado na competitividade e estratégia."), new Preco(0.00m), GeneroJogo.FPS),
                    new Jogo(new Nome("League of Legends"), new Descricao("Um jogo de estratégia em equipe onde duas equipes de cinco campeões poderosos se enfrentam para destruir a base inimiga."), new Preco(0.00m), GeneroJogo.MOBA),
                    new Jogo(new Nome("Resident Evil 4 Remake"), new Descricao("O pesadelo retorna. Acompanhe Leon S. Kennedy em uma missão para resgatar a filha do presidente em uma vila europeia isolada e aterrorizante."), new Preco(199.90m), GeneroJogo.Terror),
                    new Jogo(new Nome("Spider-Man 2"), new Descricao("Os Spider-Men Peter Parker e Miles Morales retornam para uma nova aventura espetacular na Nova York da Marvel, enfrentando o vilão Venom."), new Preco(299.90m), GeneroJogo.Acao),
                    new Jogo(new Nome("Final Fantasy VII Rebirth"), new Descricao("A jornada para o desconhecido continua. Cloud e seus amigos escapam da cidade distópica de Midgar para explorar o vasto mundo afora."), new Preco(349.90m), GeneroJogo.RPG),
                    new Jogo(new Nome("Sekiro: Shadows Die Twice"), new Descricao("Explore o Japão do final do século XVI enquanto enfrenta inimigos colossais neste jogo de ação focado no combate de precisão."), new Preco(199.00m), GeneroJogo.Acao),
                    new Jogo(new Nome("Dark Souls III"), new Descricao("Enquanto o fogo se apaga e o mundo cai em ruínas, aventure-se em um universo repleto de inimigos e ambientes colossais em um combate desafiador."), new Preco(159.00m), GeneroJogo.Soulslike),
                    new Jogo(new Nome("Hollow Knight"), new Descricao("Uma aventura épica de ação clássica em 2D por um vasto reino em ruínas de insetos e heróis, focado na exploração e combate difícil."), new Preco(27.99m), GeneroJogo.Metroidvania),
                    new Jogo(new Nome("Terraria"), new Descricao("O mundo está ao seu alcance enquanto você luta pela sobrevivência, fortuna e glória. Cavar, lutar, explorar, construir!"), new Preco(19.99m), GeneroJogo.MundoAberto),
                    new Jogo(new Nome("Apex Legends"), new Descricao("Domine com estilo em Apex Legends, um jogo Battle Royale gratuito onde personagens lendários com habilidades poderosas lutam nas bordas da Fronteira."), new Preco(0.00m), GeneroJogo.Battle_Royale),
                    new Jogo(new Nome("Dota 2"), new Descricao("O MOBA mais profundo e complexo do mercado, onde a estratégia de equipe e a habilidade individual definem o vencedor em partidas intensas."), new Preco(0.00m), GeneroJogo.MOBA),
                    new Jogo(new Nome("Overwatch 2"), new Descricao("Um jogo de tiro em equipe vibrante e em constante evolução, onde heróis únicos batalham em cenários ao redor do mundo."), new Preco(0.00m), GeneroJogo.FPS),
                    new Jogo(new Nome("Cuphead"), new Descricao("Um jogo de ação clássico focado em batalhas contra chefes, inspirado nos desenhos animados da década de 1930."), new Preco(36.99m), GeneroJogo.Acao),
                    new Jogo(new Nome("Celeste"), new Descricao("Ajude Madeline a sobreviver aos seus demônios internos em sua jornada até o topo da Montanha Celeste, neste jogo de plataforma superafiado."), new Preco(36.99m), GeneroJogo.Plataforma),
                    new Jogo(new Nome("Dead Cells"), new Descricao("Explore um castelo em constante mudança e expansão... assumindo que você consiga lutar para passar pelos guardiões neste metroidvania roguelike."), new Preco(47.49m), GeneroJogo.Roguelike),
                    new Jogo(new Nome("Sea of Thieves"), new Descricao("Torne-se a lenda pirata que você sempre quis ser em uma vasta aventura de mundo aberto, explorando ilhas e combatendo outros jogadores."), new Preco(89.99m), GeneroJogo.Aventura),
                    new Jogo(new Nome("Starfield"), new Descricao("Em Starfield, o primeiro novo universo em 25 anos da Bethesda Game Studios, crie qualquer personagem que quiser e explore o espaço com liberdade incomparável."), new Preco(299.00m), GeneroJogo.RPG),
                    new Jogo(new Nome("Grand Theft Auto V"), new Descricao("Um jovem traficante, um ladrão de bancos aposentado e um psicopata aterrorizante devem realizar uma série de roubos perigosos para sobreviver em uma cidade implacável."), new Preco(69.90m), GeneroJogo.MundoAberto),
                    new Jogo(new Nome("The Last of Us Part I"), new Descricao("Uma jornada emocional em um mundo pós-apocalíptico, onde Joel precisa escoltar a jovem Ellie através dos Estados Unidos devastados."), new Preco(249.90m), GeneroJogo.Aventura)
                    );

                await context.SaveChangesAsync();
            }
        }
    }
}
