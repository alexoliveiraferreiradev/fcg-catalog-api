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

            if (!await context.Games.AnyAsync())
            {
                context.Games.AddRange(
                    new Game(new Name("Elden Ring"), new Description("Levante-se, Maculado, e seja guiado pela graça para portar o poder do Anel Pratinado e se tornar um Lorde Pratinado nas Terras Intermédias."), new Price(199.00m), GameGenre.Soulslike),
                    new Game(new Name("Counter-Strike 2"), new Description("A evolução tática do shooter em primeira pessoa mais popular do mundo, com gráficos aprimorados e mapas reaginados."), new Price(0.00m), GameGenre.FPS),
                    new Game(new Name("Cyberpunk 2077"), new Description("Um RPG de ação e aventura em mundo aberto ambientado na megalópole de Night City, onde você joga como um mercenário urbano."), new Price(199.00m), GameGenre.RPG),
                    new Game(new Name("The Witcher 3: Wild Hunt"), new Description("Torne-se o bruxo Geralt de Rivia e embarque em uma jornada épica em Search da criança da profecia em um mundo aberto devastado pela guerra."), new Price(129.90m), GameGenre.RPG),
                    new Game(new Name("Stardew Valley"), new Description("Você herdou a antiga fazenda do seu avô. Com ferramentas usadas e algumas moedas, comece sua nova vida no campo!"), new Price(24.99m), GameGenre.Simulacao),
                    new Game(new Name("Red Dead Redemption 2"), new Description("Uma saga épica sobre a vida no coração imperdoável da América no final do século XIX, seguindo o fora-da-lei Arthur Morgan."), new Price(239.00m), GameGenre.Acao),
                    new Game(new Name("God of War Ragnarök"), new Description("Kratos e Atreus devem embarcar em uma jornada mítica pelos Nove Reinos em Search de respostas enquanto as forças asgardianas se preparam para a batalha profetizada."), new Price(299.90m), GameGenre.Acao),
                    new Game(new Name("Hades"), new Description("Desafie o deus dos mortos enquanto batalha para escapar do Submundo da mitologia grega neste aclamado RPG roguelike."), new Price(73.99m), GameGenre.Roguelike),
                    new Game(new Name("Baldur's Gate 3"), new Description("Reúna seu grupo e retorne aos Reinos Esquecidos em uma história de companheirismo e traição, sacrifício e sobrevivência, e a atração do poder absoluto."), new Price(199.99m), GameGenre.RPG),
                    new Game(new Name("Minecraft"), new Description("Explore mundos infinitos, construa desde simples casas a castelos grandiosos. Jogue no modo criativo com recursos ilimitados ou sobreviva no modo sobrevivência."), new Price(99.00m), GameGenre.MundoAberto),
                    new Game(new Name("Forza Horizon 5"), new Description("Sua aventura Horizon definitiva aguarda! Explore as paisagens vibrantes e em constante evolução do México com ação de pilotagem ilimitada e divertida."), new Price(249.00m), GameGenre.Corrida),
                    new Game(new Name("Valorant"), new Description("Misture mira precisa com habilidades táticas únicas neste shooter tático 5v5 focado na competitividade e estratégia."), new Price(0.00m), GameGenre.FPS),
                    new Game(new Name("League of Legends"), new Description("Um Game de estratégia em equipe onde duas equipes de cinco campeões poderosos se enfrentam para destruir a base inimiga."), new Price(0.00m), GameGenre.MOBA),
                    new Game(new Name("Resident Evil 4 Remake"), new Description("O pesadelo retorna. Acompanhe Leon S. Kennedy em uma missão para resgatar a filha do presidente em uma vila europeia isolada e aterrorizante."), new Price(199.90m), GameGenre.Terror),
                    new Game(new Name("Spider-Man 2"), new Description("Os Spider-Men Peter Parker e Miles Morales retornam para uma nova aventura espetacular na Nova York da Marvel, enfrentando o vilão Venom."), new Price(299.90m), GameGenre.Acao),
                    new Game(new Name("Final Fantasy VII Rebirth"), new Description("A jornada para o desconhecido continua. Cloud e seus amigos escapam da cidade distópica de Midgar para explorar o vasto mundo afora."), new Price(349.90m), GameGenre.RPG),
                    new Game(new Name("Sekiro: Shadows Die Twice"), new Description("Explore o Japão do final do século XVI enquanto enfrenta inimigos colossais neste Game de ação focado no combate de precisão."), new Price(199.00m), GameGenre.Acao),
                    new Game(new Name("Dark Souls III"), new Description("Enquanto o fogo se apaga e o mundo cai em ruínas, aventure-se em um universo repleto de inimigos e ambientes colossais em um combate desafiador."), new Price(159.00m), GameGenre.Soulslike),
                    new Game(new Name("Hollow Knight"), new Description("Uma aventura épica de ação clássica em 2D por um vasto reino em ruínas de insetos e heróis, focado na exploração e combate difícil."), new Price(27.99m), GameGenre.Metroidvania),
                    new Game(new Name("Terraria"), new Description("O mundo está ao seu alcance enquanto você luta pela sobrevivência, fortuna e glória. Cavar, lutar, explorar, construir!"), new Price(19.99m), GameGenre.MundoAberto),
                    new Game(new Name("Apex Legends"), new Description("Domine com estilo em Apex Legends, um Game Battle Royale gratuito onde personagens lendários com habilidades poderosas lutam nas bordas da Fronteira."), new Price(0.00m), GameGenre.Battle_Royale),
                    new Game(new Name("Dota 2"), new Description("O MOBA mais profundo e complexo do mercado, onde a estratégia de equipe e a habilidade individual definem o vencedor em partidas intensas."), new Price(0.00m), GameGenre.MOBA),
                    new Game(new Name("Overwatch 2"), new Description("Um Game de tiro em equipe vibrante e em constante evolução, onde heróis únicos batalham em cenários ao redor do mundo."), new Price(0.00m), GameGenre.FPS),
                    new Game(new Name("Cuphead"), new Description("Um Game de ação clássico focado em batalhas contra chefes, inspirado nos desenhos animados da década de 1930."), new Price(36.99m), GameGenre.Acao),
                    new Game(new Name("Celeste"), new Description("Ajude Madeline a sobreviver aos seus demônios internos em sua jornada até o topo da Montanha Celeste, neste Game de plataforma superafiado."), new Price(36.99m), GameGenre.Plataforma),
                    new Game(new Name("Dead Cells"), new Description("Explore um castelo em constante mudança e expansão... assumindo que você consiga lutar para passar pelos guardiões neste metroidvania roguelike."), new Price(47.49m), GameGenre.Roguelike),
                    new Game(new Name("Sea of Thieves"), new Description("Torne-se a lenda pirata que você sempre quis ser em uma vasta aventura de mundo aberto, explorando ilhas e combatendo outros jogadores."), new Price(89.99m), GameGenre.Aventura),
                    new Game(new Name("Starfield"), new Description("Em Starfield, o primeiro novo universo em 25 anos da Bethesda Game Studios, crie qualquer personagem que quiser e explore o espaço com liberdade incomparável."), new Price(299.00m), GameGenre.RPG),
                    new Game(new Name("Grand Theft Auto V"), new Description("Um jovem traficante, um ladrão de bancos aposentado e um psicopata aterrorizante devem realizar uma série de roubos perigosos para sobreviver em uma cidade implacável."), new Price(69.90m), GameGenre.MundoAberto),
                    new Game(new Name("The Last of Us Part I"), new Description("Uma jornada emocional em um mundo pós-apocalíptico, onde Joel precisa escoltar a jovem Ellie através dos Estados Unidos devastados."), new Price(249.90m), GameGenre.Aventura)
                    );

                await context.SaveChangesAsync();
            }
        }
    }
}
