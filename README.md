# DiveLog

**DiveLog** är en webbaserad loggboksapplikation utvecklad för att dokumentera och hantera dykverksamhet inom räddningstjänst och dykorganisationer.

Applikationen gör det möjligt att:

- Registrera och hantera dyk
- Registrera och hantera personal
- Dokumentera dyktider, dyksyften och deltagare
- Visa dykplatser på karta
- Filtrera och söka bland genomförda dyk
- Generera statistik över genomförda dyk och dyksyften
- Hantera användare och behörigheter

## Tekniker

Projektet är utvecklat med:

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- Leaflet.js
- HTML
- CSS
- JavaScript

## Live Demo

Applikationen finns publicerad på:

https://divelogproject.azurewebsites.net/

## Installation

1. Klona projektet:

```bash
git clone https://github.com/aner2308/divelog.git
```

2. Gå till projektmappen:

```bash
cd divelog
```

3. Uppdatera anslutningssträngen i `appsettings.json`.

4. Kör databasmigreringar:

```bash
dotnet ef database update
```

5. Starta applikationen:

```bash
dotnet run
```

## Projektbakgrund

Projektet utvecklades som examensarbete inom webbutveckling med syftet att skapa ett digitalt system för dokumentation av räddningsdykning. Fokus har legat på användbarhet, förenklad informationshanteing, statistikhantering och automatisering av manuella arbetssätt.

## Författare

*Emma Larsson & Anton Eriksson*
