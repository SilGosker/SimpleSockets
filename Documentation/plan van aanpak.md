# Plan van aanpak - EasySockets
In dit document wordt de probleemstelling beschreven met de daarbij gekozen oplossing inclusief onderbouwing.

Onderaan het document staat een globale planning waarbij per onderdeel aangegeven is wanneer dit afgerond moet zijn.

## Probleemstelling
ASP.NET Core applicaties die websocket-componenten bevatten kunnen een terugkomend probleem ervaren: websockets zijn complex.

De ontwikkelaar moet bij het werken met websockets bijvoorbeeld werken met een specifieke objecten waarbij veel kennis over geheugengebruik is vereist. Deze objecten moeten gebruikt worden om data te kunnen ontvangen en te versturen. Per bericht moet de ontwikkelaar handmatig aangeven van welk type deze is (tekst of binary). Specifieke code moet ook nog eens geschreven worden om een bericht te kunnen verwerken. Dit alles zorgt ervoor dat de code niet makkelijk te begrijpen is. Hierdoor wordt het steeds lastiger om de code uit te breiden en te onderhouden.

Doordat Microsoft .NET een grote community heeft zijn er veel pakketten die dit probleem kunnen oplossen. `SignalR` is hiervan de grootste en meest gebruikte, ontwikkeld door Microsoft zelf. Daarom nemen we `SignalR` als voorbeeld in de probleemstelling. Deze pakketten zorgen ervoor dat het werken met websocket-gedreven applicaties een stuk makkelijker wordt. `SignalR` doet veel werk voor je. Voorbeelden hiervan zijn authenticatie, autorisatie of het versturen en ontvangen van berichten door middel van een `string`. Echter ligt hier direct het probleem want dit werk limiteert de oplossingen die `SignalR` kan bieden. Een voorbeeld van iets dat `SignalR` niet kan bieden is een websocket gebaseerde koppeling, waarbij de client geen ondersteund pakket van SignalR gebruikt.

Authenticatie en autorisatie kan langzamerhand een nachtmerrie worden omdat `SignalR` hier de controle over heeft. Functies die aangeroepen worden moeten zich heel strak houden aan `SignalR`'s standaarden anders worden ze niet aangeroepen.

## Oplossing
De oplossing hiervoor is een vervangend `SignalR` pakket waarbij de ontwikkelaar controle krijgt over zijn code. De ontwikkelaar moet zijn eigen authenticatie en autorisatie kunnen ontwikkelen, events kunnen registreren en code kunnen toepassen per event. Dit terwijl het pakket de moeilijkste zaken regelt.

Dit pakket is de juiste keuze voor ontwikkelaars die intensieve controle willen over authenticatie en autorisatie en/of event registratie en verwerking.

Dit pakket is niet de juiste keuze voor ontwikkelaars die een websocket gedreven applicatie ontwikkelen waarbij de client `SignalR` als ondersteunend pakket gebruikt.

Het pakket bevat geen code voor de client. De ontwikkelaar wordt aangemoedigd om de code voor de client zelf te ontwikkelen. De reden hiervoor is dat de ontwikkelaar zijn eigen event registratie kan ontwikkelen. Het is onmogelijk om rekening te houden met alle soorten event registraties die ontwikkelaars bedenken.

Dit pakket moet om kunnen gaan met iedere ontwikkelaar en dat betekent dat het pakket erg dynamisch moet werken. Doordat de ontwikkelaar veel controle heeft over zijn code is het zo dat de productiecode van `EasySockets` niet zo mooi is als de productiecode van `SignalR`. 

Dit pakket moet voldoen aan de requirements beschreven in [de user stories](Requirements%20EasySockets.md).

## Planning

| Wanneer | Wat gedaan |
|---------|------------|
| Uiterlijk vrijdag Week 6 | Planning gemaakt, requirements geschreven. |
| Uiterlijk dinsdag 16 oktober 2023 | Probleemstelling en oplossing beschreven. |
| Uiterlijk woensdag 17 oktober 2023 | Probleemstelling en oplossing feedback verwerkt. |
| Uiterlijk dinsdag 31 oktober 2023 - Dinsdag week 9 | Argumentatie gebruiksredenen geschreven |
