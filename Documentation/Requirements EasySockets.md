Hieronder staan de requirements van EasySocket beschreven. Het doel van dit document is om helder te krijgen wat EasySockets precies moet doen en waarom. De formulering gaat in de vorm van user stories in combinatie met acceptatiecriteria.

Het doel van EasySockets is om het ontwikkelen van websocket-gedreven applicaties voor ontwikkelaars die intensieve controle nodig hebben over hun connectie makkelijker te maken.

# Beloftes
Hieronder staan een aantal beloftes die gemaakt zijn om het ontwikkelen met EasySockets stabiel en prettig te houden.
## leesbaarheid
Leesbaarheid is een van de belangrijkste onderdelen gaat als het gaat om de onder houdbaarheid van goede code. Als variabelen slechte namen hebben of functies niet beschrijvend zijn wordt het moeilijk om code te onderhouden. Zo ook bij het gebruikmaken van een pakket. Als de componenten (functies, klassen, variabelen, etc.) niet logisch benoemd zijn, zal het moeilijk worden om gebruik te maken van het pakket.

Om het pakket zo aantrekkelijk mogelijk te maken voor developers worden de volgende standaarden aangehouden:

1. Alle componenten bevatten Engelstalige namen.
2. Componenten bevatten beschrijvende namen. De naam van ieder component moet als geheugensteun kunnen dienen voor de werking van het component.
3. Geen van de componenten bevatten onbekende afkortingen. Een component dat bijvoorbeeld `StrReplace(string, string, string)` heet moet hernoemd worden naar `StringReplace(string, string, string)`. Bekende afkortingen zoals `Async` i.p.v. `Asynchronous` mogen wel afgekort blijven.
4. Alle componenten die de developer gebruikt moeten xml-commentaar bevatten om uit te leggen wat de onderliggende functie van het component is. Componenten die niet door de developer gebruikt worden zijn niet verplicht om xml-commentaar te bevatten.
5. De code moet gecompileerd kunnen worden zonder errors, warnings of messages.
6. De code die de ontwikkelaar gebruikt moet zelf-uitleggend zijn of bekend voorkomen. Het automatisch aanroepen van functies in `SignalR` is niet perse logisch, maar komt bekend voor. Het gebruik van de `Socket.IO` `on(string, function)` kan ook bekend voorkomen.

Door deze regels aan te houden willen wij ervoor zorgen dat de developer op natuurlijke wijze gebruik kan maken van het EasySockets pakket.

## Breaking changes
Breaking changes zijn veranderingen die ervoor zorgen dat oude code niet meer werkt na een update. Dit willen we op alle mogelijke wijzen voorkomen. De achterliggende reden is dat (veel) breaking changes het pakket zeer onaantrekkelijk en instabiel maakt. Als developer wil je niet dat je na iedere update de werking van EasySockets opnieuw moet testen. EasySockets moet werken zoals de developer heeft geleerd dat het werkt!

Daarom houden we de volgende standaarden aan:
1. Unit testen moeten ervoor zorgen dat functionaliteiten van EasySockets grondig getest worden.
2. Er wordt git als versiebeheer tool om ervoor te zorgen dat de code teruggedraaid kan worden, mocht dit nodig zijn.
3. Github wordt gebruikt om Continuous Integration Pipelines (CI pipelines) te integreren zodat na iedere commit alle unit tests gestart kunnen worden om te kijken of er geen breaking changes zijn.
4. Github wordt gebruikt om Continuous Deployment Pipelines (CD pipelines) te integreren zodat er foutloos gepubliceerd kan worden.
5. Een feature kan nooit zodanig worden aangepast dat er zonder andere naamgeving een breaking change voordoet. Als er andere functionaliteit achter een component gewenst is, wordt er een nieuw component gemaakt met de gewenste functionaliteit daarachter. Het oude component blijft bestaan en werkt zoals het altijd heeft gewerkt.
6. Mocht het zo zijn dat een component gepland staat om verwijderd te worden, wordt dit in tevoren aangekondigd. Daarnaast wordt dat component gemarkeerd met de `ObsoleteAttribute` zodat ook in de code duidelijk is dat dit component gaat verdwijnen in een toekomstige versie. **Het component wordt niet direct verwijderd!** Developers moeten de tijd krijgen om hun code in hun tijd aan te passen, zonder druk van verschillende pakketten die willen dat je component `B` gebruikt i.p.v. component `A`.

Door deze regels aan te houden willen wij ervoor zorgen dat de developer zich geen zorgen hoeft te maken over het gebruik van het EasySockets pakket. Oude code moet blijven werken.

# Rollen
Hieronder worden de verschillende rollen die in EasySockets spelen uitgelegd en uitgewerkt.

De rollen zijn:
1. Developer
2. Ontwikkelaar van EasySockets
## Developer
De developer is degene die de code van EasySockets (ook wel 'source code' genoemd) downloadt en gebruikt in zijn applicatie. De developer is de eindgebruiker van EasySockets.

De developer wilt met gemak krachtige en aangepaste websocket applicaties kunnen coderen. Hij wilt krachtige features om ervoor te zorgen dat hij zijn eigen protocollen kan schrijven. Dit bevat een eigen authenticatie en autorisatie pipeline en event driven development in combinatie met custom event binding. De websockets moeten gemanipuleerd kunnen worden buiten de websocket instance om.
## Ontwikkelaar van EasySockets
De ontwikkelaar van EasySockets (vanaf nu 'ontwikkelaar') is degene die de achterliggende code van EasySockets beheert.

De ontwikkelaar wilt dat het pakket zo aantrekkelijk mogelijk is. Dit betekent dat de code werkt en veilig is. Andere ontwikkelaars moeten zonder zorgen de code kunnen gebruiken. Ook moet de code 'snel' zijn. Dit betekent dat er een minimale hoeveelheid geheugen moet worden ingenomen door het EasySockets pakket.

De ontwikkelaar van EasySockets wilt dat oude code blijft werken, ook als er later veranderingen worden gedaan. Als dit niet gebeurt noemen we dit een 'Breaking Change'.

## Clients
Het is zo dat in sommige user stories wordt gerefereerd naar 'client' of 'clients'. Hiermee wordt niet verwezen naar de Developer of Ontwikkelaar van EasySockets. De client kan een web-gebruiker zijn die verbindt met de server, maar ook een microcontroller met internetverbinding, of een netwerkapplicatie zoals Postman. Het is aan de Developer om te bepalen wie/wat de client is. EasySockets zorgt ervoor dat je dit zelf kan bepalen en aanpassen naar de behoeften van de Developer.

Dit is ook de reden dat er geen user stories te vinden zijn voor deze clients. Omdat een client alles kan zijn (zolang het een websocket connectie kan maken), is het onmogelijk om vanuit EasySockets iets te ontwikkelen. Het is aan de Developer om zijn client zo te ontwikkelen dat hij met de server kan communiceren.
# User stories developer
De onderstaande user stories zijn de user stories waar de developer gebruik van maakt.

## Koppelen van websocket-typen
De volgende user stories zijn gerelateerd aan het koppelen van websocket-types aan urls.

#### Als developer wil ik EasySocket-types kunnen koppelen aan een url zodat een websocket connectie verbonden kan worden met de server.
Acceptatiecriteria:
1. Voordat de `IApplicationBuilder.Run` methode wordt aangeroepen moet er geconfigureerd kunnen worden welk websocket-type van toepassing is op welke url.
2. EasySocket-types moeten herbruikt kunnen worden op verschillende urls.
3. Urls mogen niet meerdere keren genoemd worden. Als dit wel gebeurt, moet het systeem een exceptie gooien.

#### Als developer wil ik een encoding aan kunnen geven zodat de websocket berichten van andere talen/encodings kan ontvangen.
Acceptatiecriteria:
1. Per configuratie voor een EasySocket type moet er een encoding aangegeven kunnen worden.
2. De standaard encoding is UTF8.

## Authenticatie en autorisatie
De volgende user stories zijn gerelateerd aan het authenticatie en autorisatie proces van EasySockets.

#### Als developer wil ik authenticatie zelf beheren zodat ik controle heb wie met de server verbonden mag worden.
Acceptatiecriteria:
1. De developer moet functionaliteit kunnen coderen om de client to authenticeren. Dit wordt gedaan door middel van een boolean waarde.
2. Als de teruggestuurde waarde `false` is, zal de pipeline beëindigd worden en de request in een `401` statuscode resulteren.
3. Als de teruggestuurde waarde `true` is, zal het volgende item in de pipeline uitgevoerd worden.
4. Als er geen volgend item in de pipeline is, zal de websocket geaccepteerd worden.

#### Als developer wil ik autorisatie zelf beheren zodat ik controle heb over wie de client is volgens het systeem.
Acceptatiecriteria:
1. De developer moet een unieke (string)waarde kunnen specificeren tijdens het autorisatieproces.
2. Als de developer geen waarde specificeert, wordt een standaard procedure gevolgd om de connectie alsnog een unieke waarde te geven.

####  Als developer wil ik dat clients opgedeeld worden in kamers zodat niet iedereen gedeelde informatie kan ontvangen.
Acceptatiecriteria:
1. Berichten worden die vanuit de server naar clients worden gestuurd, worden alleen ontvangen door clients in dezelfde kamer.

#### Als developer wil ik beheren welke clients in welke kamers opgedeeld worden zodat ik kan bepalen welke informatie naar welke clients gaat.
Acceptatiecriteria:
1. De developer moet kunnen specificeren in welke kamer de client terechtkomt tijdens het autorisatieproces.
2. Als de developer geen waarde specificeert wordt een standaard procedure gevolgd om de connectie alsnog in een kamer te zetten.

#### Als developer wil ik dat ik authenticatie en autorisatie processen kan herbruiken zodat ik minder dubbele code hoef te schrijven.
Acceptatiecriteria:
1. authenticatie functionaliteit die de developer codeert moet herbruikt kunnen worden.
2. autorisatie functionaliteit die de developer codeert moet herbruikt kunnen worden.

#### Als developer wil ik de standaardwaardes voor authenticatie aan kunnen passen zodat ik minder dubbele code hoef te schrijven.
Acceptatiecriteria:
1. Als er geen authenticatieproces is gekoppeld aan een EasySocket-type, moet het systeem een standaardwaarde pakken die de client autoriseert.
2. De developer moet deze standaardwaarde globaal kunnen configureren.
3. De developer moet deze standaardwaarde per EasySocket-type kunnen configureren.

#### Als developer wil ik de standaardwaardes voor autorisatie aan kunnen passen zodat ik minder dubbele code hoef te schrijven.
1. Als er na autorisatie geen unieke waarde is gekoppeld aan een connectie, moet het systeem een standaardprocedure volgen die de unieke waarde teruggeeft.
2. De developer moet dit proces zelf kunnen coderen naar zijn behoeften.
3. Als er na autorisatie geen kamer is gekoppeld aan de connectie van een client moet het systeem een standaardprocedure volgen om de connectie in een kamer te krijgen.
4. De developer moet dit proces zelf kunnen coderen naar zijn behoeften.

#### Als developer wil ik mijn eigen services kunnen gebruiken in het authenticatieproces zodat ik mijn eigen authenticatieproces kan maken.
Acceptatiecriteria:
1. Er moet dependency injection (DI) mogelijk zijn in de authenticatieprocessen.

#### Als developer wil ik mijn eigen services kunnen gebruiken in het autorisatieproces zodat ik mijn eigen autorisatieproces kan maken.
Acceptatiecriteria:
1. Er moet DI mogelijk zijn in de autorisatieprocessen.

## Berichten ontvangen en sturen
De volgende user stories zijn gerelateerd aan het (gemakkelijk) ontvangen en versturen van berichten door middel van de websocket-connectie van de client.

#### Als developer wil ik genotificeerd worden van een bericht als de client er een stuurt zodat ik dat bericht kan verwerken.
Acceptatiecriteria:
1. De developer wordt binnen het EasySocket-type genotificeerd van het bericht als de client er een stuurt.
2. De developer ontvangt het bericht als hij genotificeerd wordt.

#### Als developer wil ik eigen berichten kunnen sturen naar de client zodat de client genotificeerd wordt van het bericht.
Acceptatiecriteria:
1. De developer moet binnen het EasySocket-type een methode aan kunnen roepen die een bericht van de server naar de client stuurt.
2. De developer moet buiten het EasySocket-type een methode aan kunnen roepen die een bericht van de server naar de gespecificeerde client stuurt.

#### Als developer wil ik de grootte van de berichten zelf instellen zodat ik meer controle heb over performance.
Acceptatiecriteria:
1. De developer moet tijdens het configureren van ieder websocket-type de grootte van de chunk formaat kunnen specificeren.
2. Berichten worden ontvangen aan de hand van de grootte van het chunk formaat. Als berichten groter zijn dan het formaat, wordt de nieuwe informatie opnieuw geprobeerd ontvangen te worden totdat het volledige bericht ontvangen is.

#### Als developer wil ik kunnen coderen welke berichten naar welke kamer gestuurd worden zodat ik berichten naar clients binnen en buiten de client zijn kamer kan sturen.
Acceptatiecriteria:
1. De developer moet binnen het EasySocket-type een methode aan kunnen roepen die een bericht stuurt naar alle clients binnen de client zijn kamer.
3. De developer kan niet binnen het EasySocket-type een methode aanroepen die een bericht stuurt naar clients van een specifieke kamer.
4. De developer kan buiten het EasySocket-type een methode aanroepen die een bericht stuurt naar clients van een specifieke kamer.
5. De developer kan buiten het EasySocket-type een methode aanroepen die een bericht stuurt naar specifieke clients.

#### Als developer wil ik kunnen coderen welke berichten naar alle gebruikers gestuurd worden zodat ik alle clients op de hoogte kan houden.
Acceptatiecriteria:
1. De developer kan buiten het EasySocket-type een methode aanroepen die een bericht stuurt naar alle clients.
2. De developer kan binnen het EasySocket-type een methode aanroepen die een bericht stuurt naar alle clients buiten de client zijn kamer.

## Event driven development
De volgende user stories zijn gerelateerd aan het (gemakkelijk) verwerken en opdelen van de code van een developer.

#### Als developer wil ik namen van evenement-types kunnen registreren zodat ik code kan aanroepen op basis van het evenement.
Acceptatiecriteria:
1. De developer moet zijn eigen code kunnen linken aan de naam van een evenement.
2. Het systeem registreert de namen en de achterliggende code.
3. Als een bericht ontvangen wordt waarvan de naam van het evenement gelijk is aan een van de aangegeven namen, moet de gelinkte code uitgevoerd worden.

#### Als developer wil ik evenement-types kunnen registreren zodat ik zelf kan bepalen hoe een evenement eruit ziet.
Acceptatiecriteria:
1. De developer moet een type kunnen registreren die als evenement dient.
2. De developer moet methodes implementeren die aangeven wat de naam en het bericht van het evenement is.
3. Als de client een bericht verstuurd, moet het systeem de naam en bericht van het evenement extraheren en de bijhorende code aanroepen.

#### Als developer wil ik dat ik een melding krijg van een ongeregistreerd evenementen-type zodat ik sneller fouten kan opsporen.
Acceptatiecriteria:
1. De developer moet genotificeerd worden als er een naam van een evenement geregistreerd wordt dat de developer niet heeft gespecificeerd.

#### Als developer wil ik een melding krijgen van een fout tijdens het bepalen van evenementen-types zodat ik sneller fouten kan opsporen.
Acceptatiecriteria:
1. De developer moet genotificeerd worden als er een fout optreed tijdens het registreren van een evenement.

## Verwijderen websockets
De volgende user stories zijn gerelateerd aan het afsluiten van de connecties.

##### Als developer wil ik een melding ontvangen de connectie met een client afsluit zodat ik dit kan verwerken.
Acceptatiecriteria:
1. De developer wordt genotificeerd als de connectie gesloten wordt, op welke manier ook.

#### Als developer wil ik de connectie af kunnen sluiten zodat de verbinding tussen de client en de server wegvalt.
Acceptatiecriteria:
1. De developer kan binnen het EasySocket-type de connectie met de client afsluiten.
2. De developer kan buiten het EasySocket-type de connectie met een client afsluiten.

#### Als developer wil ik de sluit-status-beschrijving van de websocket per connectie in kunnen stellen zodat ik de client de reden kan laten weten van de sluiting van de connectie.
Acceptatiecriteria:
1. Tijdens het configureren van een EasySocket-type wil ik de mogelijkheid hebben om de sluitbeschrijving te configureren. 

#### Als developer wil ik alle connecties binnen een kamer afsluiten zodat de verbinding van alle clients wegvalt.
Acceptatiecriteria:
1. De developer kan buiten het EasySocket-type de connectie met alle clients binnen een kamer afsluiten.
2. De developer kan buiten het EasySocket-type de connectie met alle clients afsluiten.

## Compatibility
Dit zijn de user stories die gerelateerd zijn aan de compatibiliteit van EasySockets. Dit betekent dat EasySockets niet alleen op de nieuwste versies moet werken, maar ook op oudere. 

#### Als developer wil ik dat EasySocket werkt op alle .NET Core versies zodat ik het pakket op zoveel mogelijk plekken kan gebruiken.
1. EasySockets moet werken op de volgende .NET versies:
	3. .NET 6.0
	4. .NET 7.0
2. EasySockets moet werken op iedere toekomstige .NET Core versie. 
## User stories Ontwikkelaar van EasySockets
De onderstaande user stories zijn de user stories waar de ontwikkelaar van EasySockets (vanaf nu 'ontwikkelaar') gebruik van maakt.

## Breaking Changes voorkomen

#### Als ontwikkelaar wil ik dat na een update van EasySockets oudere code blijft werken zodat developers geen veranderingen in hun code hoeven te maken na een update.
Acceptatiecriteria:
1. Na een toevoeging van een feature binnen EasySockets moeten oude functionaliteiten op dezelfde manier werken.

#### Als ontwikkelaar wil ik automatische tests zodat ik de code na een update niet handmatig hoef te testen.
Acceptatiecriteria:
1. De code wordt getest door middel van unit-tests.

##### Als ontwikkelaar wil ik genotificeerd worden als oudere code niet werkt na een update zodat ik de nieuwere code kan aanpassen.
Acceptatiecriteria:
1. Als er een update wordt uitgevoerd op het pakket, moeten er automatische checks gedaan worden die valideren of (oudere) code nog steeds werkt.
2. Dit wordt gedaan door middel van een Continuous Integration(CI) pipeline in github.
3. Als een van deze tests falen, moet de ontwikkelaar een notificatie ontvangen zodat hij deze fouten op kan lossen.

## Performance
Dit zijn alle user stories die gerelateerd zijn aan de snelheid van EasySockets. Als hier naar 'snelheid' wordt gerefereerd, bedoelen we daar minimale geheugengebruik mee.

In de praktijk is het gebruiken van geheugen de grootste snelheidskiller. Als er in de user stories iets staat als 'ik wil dat EasySockets snel is', bedoelen we daar mee 'ik wil dat het EasySockets pakket zo min mogelijk geheugen gebruikt'.

#### Als ontwikkelaar wil ik dat EasySockets snel is zodat het pakket aantrekkelijker wordt voor andere developers.

#### Als ontwikkelaar wil ik kunnen testen hoe snel mijn code is zodat ik fouten kan voorkomen.
Acceptatiecriteria:
1. Er worden automatische tests uitgevoerd die letten op de hoeveelheid geheugen dat in gebruik is genomen.
2. Er wordt een waarschuwing gegeven als de hoeveelheid geheugen boven een afgesproken hoeveelheid is.

## Versiebeheer

#### Als ontwikkelaar wil ik de versie van mijn code kunnen beheren zodat ik fouten kan herstellen door de code terug te draaien.
Acceptatiecriteria:
1. Het versiebeheer wordt gedaan door middel van git.