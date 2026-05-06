# fcg-catalog-api
Microsserviço central de domínio da FIAP Cloud Games, responsável pelo catálogo de jogos e gestão da biblioteca dos usuários. Gerencia o CRUD de jogos e inicia o fluxo de compra disparando o evento OrderPlacedEvent. Atua de forma resiliente ao consumir eventos de pagamento para atualizar o acesso aos jogos
