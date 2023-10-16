# Plan van aanpak - EasySockets
In dit document staat de probleemstelling en de gekozen oplossing voor het probleem.

In dit document staat een globale planning. Hierin staat waar wanneer aan gewerkt wordt, wanneer het af is en wat er dan af is.

## Probleemstelling
Een C# ASP.NET Core applicatie die websocket-componenten bevat of zelfs compleet websocket-gedreven is, kan een terugkomend probleem ervaren: websockets zijn complex.

Daarom zijn er pakketten geschreven die veel werk voor je kunnen doen. Hier zijn er heel veel van. `SignalR` is de grootste, ontwikkeld door Microsoft zelf. Deze pakketten zorgen ervoor dat het werken met websocket-gedreven applicaties een stuk makkelijker wordt.

Echter ligt hier een probleem. `SignalR` kan veel werk voor je doen, maar dit limiteert de use-cases van `SignalR`. Een paar voorbeelden:

1. Authenticatie en Autorisatie wordt gedaan door middel van de gebruiker. De 'gebruiker' is standaard de web-gebruiker op basis van cookie-authenticatie. Dit gedrag kan veranderd worden tot een bepaald formaat (door bijvoorbeeld JWT) te gebruiken, maar authenticatie precies configureren zoals de developer dit wilt is niet mogelijk.
2. Event-driven development is mogelijk in SignalR, maar de developer zit vast in het protocol van `SignalR`. Vaak is dit geen probleem, want er is ook een client-side javascript library beschikbaar die dit protocol netjes volgt. Het kan zijn dat de developer echter andere protocollen wilt volgen, die hij zelf wilt coderen. Dit is niet mogelijk in signalR.

## Oplossing
De oplossing hiervoor is een pakket dat de ontwikkelaar controle geeft over zijn code. Dit pakket moet voldoen aan de requirements beschreven in [de user stories](./Requirements.md).

## Planning

| Wanneer | Wat gedaan |
|---------|------------|
| Voor vrijdag Week 6 | Planning gemaakt, requirements geschreven. |
| Tijdens vrijdag Week 6 | Bespreken requirements/planning, bespreken welke ontwerpen relevant zijn voor EasySockets. |
| Voor dinsdag Week 7 | In ieder geval 1 relevant ontwerp gemaakt voor EasySockets, een 1e opzet van de handleiding gemaakt |
| Tijdens dinsdag Week 7 | In ieder geval 1 relevant ontwerp bespreken, de handleiding doorlopen |