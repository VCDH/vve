/*
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
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using VVE.VLog;

namespace VVE
{
    /// <summary>
    /// Hulpfuncties voor het zoeken in de archiefbestanden van Den Haag conform de structuur sinds 1-1-2014.
    /// </summary>
    public static class VLogArchiefDenHaag
    {
        public enum findMode { before, after }
        /// <summary>
        /// Zoeken naar locatie 
        /// </summary>
        /// <param name="archiveDir">Map Den Haag Archief</param>
        /// <param name="vri">kruispuntnummer </param>
        /// <param name="tSearch">tijdstip waarnaar gezocht wordt</param>
        /// <param name="mode">indien de tijdreferentie niet exact gevonden wordt, wordt de eerste tijdreferentie voor of na gezocht</param>
        /// <returns></returns>
        public static TimeRefLocation FindTimeRefLocation(string archiveDir, string vri, DateTime tSearch, findMode mode)
        {
            TimeRefLocation result = new TimeRefLocation();

            TimeRefFindResult trlFindRes = FindTimeRefs(archiveDir, vri, tSearch);
            if (mode == findMode.before)
            {
                if (trlFindRes.TrlBefore.Valid)
                {
                    //tijdreferentie gevonden op of voor de gewenste tijd
                    result = trlFindRes.TrlBefore;
                }
                else if (trlFindRes.TrlAfter.Valid)
                {
                    //alleen na de gewenste tijd een tijdreferentie gevonden
                    result = trlFindRes.TrlAfter;
                }
                else
                {
                    //helemaal geen tijdreferenties gevonden: dan als beginpunt het gewenste tijdstip gebruiken
                    result.Valid = true;
                    result.DtFile = tSearch;
                    //begin.DtTimeRefVLog leeg laten: deze is niet beschikbaar
                }
            }
            else//find == after
            {
                if (trlFindRes.TrlAfter.Valid)
                {
                    //tijdreferentie gevonden op of na de gewenste tijd
                    result = trlFindRes.TrlAfter;
                }
                else if (trlFindRes.TrlBefore.Valid)
                {
                    //alleen voor de gewenste tijd een tijdreferentie gevonden
                    result = trlFindRes.TrlBefore;
                }
                else
                {
                    //helemaal geen tijdreferenties gevonden: dan als eindpunt het gewenste tijdstip gebruiken
                    result.Valid = true;
                    result.DtFile = tSearch;
                    //end.DtTimeRefVLog leeg laten: deze is niet beschikbaar
                }
            }

            return result;
        }

        enum stateTimeRefs { nothingFound, tBeforeFound, tAfterFound, tBothFound }
        /// <summary>
        /// Zoekt naar de tijdreferentieberichten in het Den Haag archief.
        /// </summary>
        /// <param name="archiveDir">Map Den Haag Archief</param>
        /// <param name="vri">Kruispuntnummer</param>
        /// <param name="tSearch">Te zoeken tijdstip</param>
        /// <param name="maxOffsetHours">Maximaal aantal uren dat vooruit of terug in de tijd mag worden gezocht</param>
        /// <returns>Twee gevonden tijdreferenties: één voor op op het gewenste moment, één na of op het gewenste moment</returns>
        public static TimeRefFindResult FindTimeRefs(string archiveDir, string vri, DateTime tSearch, int maxOffsetHours = 26)
        {
            stateTimeRefs _state = stateTimeRefs.nothingFound;

            //zoekt naar eerste V-Log tijdreferentie die op of net voor 'moment' ligt
            TimeRefFindResult result = new TimeRefFindResult();

            TimeRefLocation trlBefore = new TimeRefLocation();
            TimeRefLocation trlAfter = new TimeRefLocation();

            bool negativeOffsetSearched = false; //actief als er van 0 tot offset -max is doorzocht en geen tijdrefs zijn aangetroffen.
            bool ready = false;
            bool searchPreviousFile = false;
            bool searchNextFile = false;

            int offsetFile = 0; //welk uurbestand bekeken moet worden

            //het eerste te doorzoeken bestand komt overeen met waar de data verwacht wordt
            DateTime fileBase = new DateTime(tSearch.Year, tSearch.Month, tSearch.Day, tSearch.Hour, 0, 0); //bestanden op hele uren gebaseerd            

            while (!ready)
            {
                //bestand bepalen
                DateTime dtFile = fileBase.AddHours(offsetFile);

                //tijdreferenties inlezen
                DateTime[] timerefs = FindAllTimeRefs(archiveDir, vri, dtFile);

                if (timerefs.Length == 0)
                {
                    //geen tijdreferenties gevonden in bestand of geen bestand
                    if (_state == stateTimeRefs.nothingFound)
                    {
                        if (!negativeOffsetSearched) searchPreviousFile = true;
                        else searchNextFile = true;
                    }
                    else if (_state == stateTimeRefs.tAfterFound)
                    {
                        searchPreviousFile = true;
                    }
                    else if (_state == stateTimeRefs.tBeforeFound)
                    {
                        searchNextFile = true;
                    }
                    else if (_state == stateTimeRefs.tBothFound)
                    {
                        throw new Exception("FindTimeRefs() error 1: in een ongeldige state beland");
                    }
                }
                for (int i = 0; i < timerefs.Length && !ready && !searchPreviousFile && !searchNextFile; i++)
                {
                    DateTime t = timerefs[i];

                    if (_state == stateTimeRefs.nothingFound)
                    {
                        if (t < tSearch)
                        {
                            _state = stateTimeRefs.tBeforeFound;

                            trlBefore.Valid = true;
                            trlBefore.DtTimeRefVLog = t;
                            trlBefore.DtFile = dtFile;

                            if (i == (timerefs.Length - 1))
                            {
                                //laatste tijdref, dus in volgende file zoeken
                                searchNextFile = true;
                            }
                            //else volgende tijdreferenties bekijken in bestand
                        }
                        else if (t > tSearch)
                        {
                            _state = stateTimeRefs.tAfterFound;

                            trlAfter.Valid = true;
                            trlAfter.DtTimeRefVLog = t;
                            trlAfter.DtFile = dtFile;

                            searchPreviousFile = true; //direct vorige bestand doorzoeken
                        }
                        else //t==tSearch
                        {
                            trlBefore.Valid = true;
                            trlBefore.DtTimeRefVLog = t;
                            trlBefore.DtFile = dtFile;

                            trlAfter.Valid = true;
                            trlAfter.DtTimeRefVLog = t;
                            trlAfter.DtFile = dtFile;

                            ready = true;
                        }
                    }
                    else if (_state == stateTimeRefs.tBeforeFound)
                    {
                        if (t < tSearch)
                        {
                            //tBefore overschrijven
                            trlBefore.Valid = true;
                            trlBefore.DtTimeRefVLog = t;
                            trlBefore.DtFile = dtFile;

                            if (i == (timerefs.Length - 1))
                            {
                                //laatste tijdref, dus in volgende file zoeken
                                searchNextFile = true;
                            }
                            //else volgende tijdreferenties bekijken in bestand
                        }
                        else if (t > tSearch)
                        {
                            //tAfter gevonden, dus klaar
                            trlAfter.Valid = true;
                            trlAfter.DtTimeRefVLog = t;
                            trlAfter.DtFile = dtFile;

                            ready = true;
                        }
                        else //t==tSearch
                        {
                            //t is gezochte waarde
                            trlBefore.Valid = true;
                            trlBefore.DtTimeRefVLog = t;
                            trlBefore.DtFile = dtFile;

                            trlAfter.Valid = true;
                            trlAfter.DtTimeRefVLog = t;
                            trlAfter.DtFile = dtFile;

                            ready = true;
                        }
                    }
                    else if (_state == stateTimeRefs.tAfterFound)
                    {
                        //hier komt hij alleen als hij terug in de tijd aan het zoeken is
                        if (t < tSearch)
                        {
                            //tBefore nu ook gevonden: verder zoeken binnen bestand naar t die dichter bij tSearch ligt
                            _state = stateTimeRefs.tBothFound;
                            trlBefore.Valid = true;
                            trlBefore.DtTimeRefVLog = t;
                            trlBefore.DtFile = dtFile;

                            if (i == (timerefs.Length - 1))
                            {
                                //laatste tijdref, dus geen tijdrefs die dichter bij tSearch liggen
                                ready = true;
                            }
                        }
                        else if (t > tSearch)
                        {
                            //t gevonden die dichter bij tSearch ligt dan tAfter: tAfter overschrijven
                            trlAfter.Valid = true;
                            trlAfter.DtTimeRefVLog = t;
                            trlAfter.DtFile = dtFile;

                            searchPreviousFile = true;
                        }
                        else //t==tSearch
                        {
                            //t is gezochte waarde
                            trlBefore.Valid = true;
                            trlBefore.DtTimeRefVLog = t;
                            trlBefore.DtFile = dtFile;

                            trlAfter.Valid = true;
                            trlAfter.DtTimeRefVLog = t;
                            trlAfter.DtFile = dtFile;

                            ready = true;
                        }
                    }
                    else if (_state == stateTimeRefs.tBothFound)
                    {
                        if (t < tSearch)
                        {
                            //t gevonden die dichter bij tSearch ligt dan tBefore: tBefore overschrijven
                            trlBefore.Valid = true;
                            trlBefore.DtTimeRefVLog = t;
                            trlBefore.DtFile = dtFile;
                        }
                        else if (t > tSearch)
                        {
                            //t gevonden direct na tSearch: overschrijven en klaar
                            trlAfter.Valid = true;
                            trlAfter.DtTimeRefVLog = t;
                            trlAfter.DtFile = dtFile;

                            ready = true;
                        }
                        else //t==tSearch
                        {
                            //t is gezochte waarde
                            trlBefore.Valid = true;
                            trlBefore.DtTimeRefVLog = t;
                            trlBefore.DtFile = dtFile;

                            trlAfter.Valid = true;
                            trlAfter.DtTimeRefVLog = t;
                            trlAfter.DtFile = dtFile;

                            ready = true;
                        }

                        if (i == (timerefs.Length - 1))
                        {
                            //laatste tijdref, dus geen tijdrefs die dichter bij tSearch liggen
                            ready = true;
                        }
                    }
                }

                if (searchPreviousFile)
                {
                    searchPreviousFile = false;

                    if (negativeOffsetSearched)
                    {
                        //alle voorgaande files zijn al doorzocht
                        ready = true; 
                    }
                    else
                    {
                        offsetFile--;
                        if (offsetFile < -maxOffsetHours)
                        {
                            if (_state == stateTimeRefs.tAfterFound) ready = true; //geen eerdere tijdrefs gevonden: klaar
                            else
                            {
                                //indien nog geen tijdrefs gevonden: doorzoeken in de positieve offset
                                negativeOffsetSearched = true;
                                offsetFile = 1;
                            }
                        }
                    }
                }
                else if (searchNextFile)
                {
                    searchNextFile = false;

                    if (offsetFile < 0) offsetFile = 0; //negatieve offsets zijn altijd al doorzocht
                    offsetFile++;
                    if (offsetFile > maxOffsetHours)
                    {
                        //alle bestanden, zowel voor als na het tijdstip zijn doorzocht, dus eindigd het hier
                        ready = true;
                    }
                }
                else if (!ready)
                {
                    throw new Exception("FindTimeRefs() error 2: in een ongeldige state beland");
                    //ready = true;
                }
            }

            if (ready)
            {
                result.TrlBefore = trlBefore;
                result.TrlAfter = trlAfter;
            }

            return result;
        }

        /// <summary>
        /// Van korte notatie [vri nummer][eventuele toevoeging] naar K[3 cijferig vri nummer][eventuele toevoeging]
        /// Voorbeeld: "57" naar "K057"
        /// Voorbeeld: "175XP"naar "K175XP"
        /// </summary>
        /// <param name="vri"></param>
        /// <returns></returns>
        public static string VriNameToFullNumber(string vri)
        {
            int cntCijfer = vri.Length - vri.TrimStart(new char[] { '0','1','2','3','4','5','6','7','8','9'}).Length;
            for (int cnt = cntCijfer; cnt < 3; cnt++) vri = "0" + vri;
            return vri;
        }

        public static string VriNameToFullKName(string vri)
        {
            return "K"+VriNameToFullNumber(vri);
        }

        /// <summary>
        /// Leest alle tijdreferenties van een aangewezen V-Log bestand
        /// </summary>
        /// <param name="archiveDir">Map van archief</param>
        /// <param name="vri">kruispuntnummer</param>
        /// <param name="dtFile">Datum/uur</param>
        /// <returns></returns>
        private static DateTime[] FindAllTimeRefs(string archiveDir, string vri, DateTime dtFile)
        {
            List<DateTime> result = new List<DateTime>();
            
            //begin zoeken
            string archFile = archiveDir + FileNameArchiveDenHaag(dtFile);
            try
            {
                if (File.Exists(archFile))
                {
                    //zoek in zip file naar gewenste vlog bestand
                    string vlogFile = FileNameVlogDenHaag(vri, dtFile);

                    using (ZipArchive archive = ZipFile.OpenRead(archFile))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.FullName == vlogFile)// h(".vlog", StringComparison.OrdinalIgnoreCase))
                            {
                                //uurbestand in zip archief gevonden
                                using (Stream sf = entry.Open())
                                {
                                    StreamReader sr = new StreamReader(sf);
                                    string bericht = sr.ReadLine();
                                    while (bericht != null)
                                    {
                                        if (bericht.Length > 2 && bericht.Substring(0, 2) == "01")
                                        {
                                            //tijdreferentie bericht
                                            DateTime tijdstip = VLog.Parser.DecodeTijdRef(bericht);
                                            if (tijdstip.Ticks > 0) result.Add(tijdstip);
                                        }
                                        bericht = sr.ReadLine();
                                    }
                                    sr.Close();
                                }
                                break; //verder zoeken niet nodig
                            }
                        }
                    }
                }
            }
            catch
            {
                result.Clear();
            }
            return result.ToArray();
        }

        /// <summary>
        /// Genereert de relatieve locatie en bestandsnaam van het ZIP archief bestand die bij de aangegeven datum hoort.
        /// Formaat volgens Den Haag structuur sinds 1-1-2014.
        /// </summary>
        /// <param name="dt">Datum/uur data</param>
        /// <returns></returns>
        private static string FileNameArchiveDenHaag(DateTime dt)
        {
            return string.Format("\\{0:yyyy}\\{0:MM}\\{0:dd}\\archive-{0:yyyy-MM-dd-HH}.zip", dt);
        }

        /// <summary>
        /// Geeft de bestandsnaam van het V-Log bestand, behorende bij de aangegeven datum.
        /// Formaat volgens Den Haag structuur sinds 1-1-2014.
        /// </summary>
        /// <param name="vri">kruispuntnummer</param>
        /// <param name="dt">Datum/uur data</param>
        /// <returns></returns>
        private static string FileNameVlogDenHaag(string vri, DateTime dt)
        {
            string vriFullName = VriNameToFullNumber(vri);
            return string.Format("{0}-{1:yyyy-MM-dd-HH}.vlog", vriFullName, dt);
        }

        public enum procesMode { excludeToTimeRef, includeToTimeRef}
        /// <summary>
        /// Leest V-Log data uit het archief en stuurt de data tussen de tijdreferenties 'dataBegin' en 'dataEnd' door naar 'procesMethod'
        /// </summary>
        /// <param name="rootDir">V-Log archief map</param>
        /// <param name="vri">kruispuntnummer</param>
        /// <param name="dataBegin">Beginpunt data (tijdreferentie)</param>
        /// <param name="dataEnd">Endpunt data (tijdreferentie)</param>
        /// <param name="procesMode">Bepaalt of de tijdrefentie van het einde ook doorgestuurd dient te worden</param>
        /// <param name="procesMethod">Functie waarnaar de V-Log berichten verstuurd worden</param>
        public static void ProcesData(string rootDir, string vri, TimeRefLocation dataBegin, TimeRefLocation dataEnd, procesMode procesMode, Action<string> procesMethod)
        {
            DateTime dtFile = dataBegin.DtFile;
            bool processing = false;
            bool finished = false;

            while ((dtFile <= dataEnd.DtFile) && !finished)
            {
                string vlogArchiveFile = rootDir + FileNameArchiveDenHaag(dtFile);
                try
                {
                    if (File.Exists(vlogArchiveFile))
                    {
                        //zoek in zip file naar gewenste vlog bestand
                        string vlogFileNeeded = FileNameVlogDenHaag(vri, dtFile);

                        using (ZipArchive archive = ZipFile.OpenRead(vlogArchiveFile))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.FullName == vlogFileNeeded)
                                {
                                    using (Stream sf = entry.Open())
                                    {
                                        StreamReader sr = new StreamReader(sf, Encoding.ASCII);
                                        string vlogMsg = sr.ReadLine();
                                        while (vlogMsg != null)
                                        {
                                            //foute karakters wissen, zodat deze in de loggings goed worden weergegeven                                            
                                            if(!VLog.Parser.IsHexAscii(vlogMsg)) vlogMsg = VLog.Parser.RetainOnlyHex(vlogMsg);
                                            if (!processing)
                                            {
                                                //beginpunt bepalen
                                                if (dataBegin.DtTimeRefVLog.Ticks != 0)
                                                {
                                                    //het startpunt is dtTimeRefVlog binnen bestand dataBegin.DtFile
                                                    if (dtFile == dataBegin.DtFile)
                                                    {
                                                        //startpunt bepalen
                                                        if (vlogMsg.Length > 2 && vlogMsg.Substring(0, 2) == "01")
                                                        {
                                                            //tijdreferentie bericht
                                                            DateTime tijdstip = VLog.Parser.DecodeTijdRef(vlogMsg);
                                                            if ((tijdstip == dataBegin.DtTimeRefVLog) || dataBegin.DtTimeRefVLog.Ticks == 0)
                                                            {
                                                                //beginpunt gevonden
                                                                processing = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //het startpunt is de eerste gevonden tijdreferentie in alle bestanden vanaf begin.DtFile 
                                                    if (vlogMsg.Length > 2 && vlogMsg.Substring(0, 2) == "01")
                                                    {
                                                        //tijdreferentie bericht
                                                        DateTime tijdstip = VLog.Parser.DecodeTijdRef(vlogMsg);
                                                        if (tijdstip.Ticks > 0) processing = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //eindpunt bepalen
                                                if (dtFile == dataEnd.DtFile &&
                                                   dataEnd.DtTimeRefVLog.Ticks != 0)
                                                {
                                                    //eindpunt bepalen op basis van bekende vlogtijdreferentie
                                                    if (vlogMsg.Length > 2 && vlogMsg.Substring(0, 2) == "01")
                                                    {
                                                        DateTime tijdstip = VLog.Parser.DecodeTijdRef(vlogMsg);
                                                        if ((tijdstip == dataEnd.DtTimeRefVLog) || dataEnd.DtTimeRefVLog.Ticks == 0)
                                                        {
                                                            //eindpunt gevonden
                                                            processing = false;
                                                            finished = true;
                                                            if (procesMode == procesMode.includeToTimeRef) procesMethod(vlogMsg);
                                                            break;
                                                        }
                                                    }
                                                }
                                                //else //eindpunt wordt bepaald door dataEnd.dtFile
                                            }

                                            //data verwerken
                                            if (processing) procesMethod(vlogMsg);

                                            vlogMsg = sr.ReadLine();
                                        }
                                        sr.Close();
                                    }
                                    break; //entry was gevonden, dus zip archief afsluiten
                                }
                            }
                        }
                    }
                }
                catch { }
                dtFile = dtFile.AddHours(1);
            }
        }
    }
}
