================================================================================
                     V-Log Verwerkings Eenheid: VVE
================================================================================
Datum: 4-2-2019

VVE is een applicatie om ruwe V-Log data uit VRI's te verwerken tot bruikbare 
verkeerskundige data. De verwerkte data kan worden uitgevoerd naar CSV of in
het oude Data Platform Fiets formaat worden gepost naar een API.

VVE is bedacht door Gemeente Den Haag, afdeling Bereikbaarheid en 
Verkeersmanagement en ontwikkeld door Claassens Solutions.
De broncode is als open source software beschikbaar gesteld, om het voor alle 
wegbeheerders mogelijk te maken om gebruik te maken van deze ontwikkeling. Het 
formele auteursrecht berust bij de Gemeente Den Haag.

================================================================================
Toelichting code
================================================================================
De VVE is ontwikkeld in Microsoft Visual Studio Community 2017 volgens de 
programmeertaal C#.
Voor het bewerken en compileren van de code is alleen Microsoft Visual Studio 
Community 2017 benodigd.
Met Visual Studio kan de solution VVE.sln geopend worden.
Onder 'Build'->'Build Solution' wordt de code gecompileerd.
Dit kan onder twee configuraties:
- Debug: 
	- uitvoer in \bin\debug\vve.exe
	- voor debug gebruik
	- onder 'Tools' zijn alle functies actief zijn
	- geschikt om te debuggen met Visual Studio
- Release:
	- uitvoer in \bin\release\vve.exe
	- voor gebruik in productieomgeving
	- onder 'Tools' is alleen 'test actuele configuraties' mogelijk
	- niet geschikt om te debuggen met Visual Studio

    
================================================================================
Toelichting gebruik
================================================================================
Voor het gebruik van de VVE is het volgende benodigd:
- VVE.exe uit \bin\release\ map
- .Net Framework versie 4.7.1 of hoger

De VVE maakt gebruik van de volgende informatie welke is opgeslagen volgens de 
door Den Haag gebruikte standaarden:
- V-Log data archief
- Configuratiebestanden
- KML configuratie
Voor het correct functioneren van de VVE, dienen de drie mappen ingevoerd te 
worden.

Alle mappen en instellingen worden onder het account van de gebruiker o
pgeslagen zodra de VVE afsluit. Bij het starten van de applicatie, worden deze 
instellingen weer ingelezen.

Eisen voor het gebruik van 'Open met viewer':
- een viewer is beschikbaar, zoals CuteView, waarbij .vlg bestanden gelinkt zijn 
aan de viewer

Bij gebruik van het automatisch POSTen van voorgaande dag, worden alle 
mogelijkheden in de applicatie bevroren. Zo is duidelijk dat de applicatie 
ingesteld is en kunnen parameters niet ongewenst gewijzigd worden.

De applicatie is nog niet multithreaded uitgevoerd. Hierdoor reageert de 
interface niet, tijdens het uitvoeren van een analyse. Bij analyse van een meer 
dan een dag over alle VRI's, duurt dit al snel meerdere minuten.


================================================================================
Licentie
================================================================================

De broncode van fietsviewer is vrijgegeven onder de voorwaarde van de 
GNU General Public License versie 3 of hoger. Voor gebundelde libraries kunnen 
andere licentievoorwaarden van toepassing zijn. Zie hiervoor de documentatie in 
de betreffende submappen.

Met uitzondering van gebundelde libraries is voor fietsviewer het volgende van 
toepassing:

    V-Log Verwerkings Eenheid: VVE
    Copyright (C) 2018-2019 Gemeente Den Haag, Netherlands
    Developed by Claassens Solutions
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
 
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
 
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.


================================================================================
Verkrijgen van de broncode
================================================================================

De broncode van de fietsviewer is gepubliceerd op Bitbucket.
https://bitbucket.org/vcdh/vve
