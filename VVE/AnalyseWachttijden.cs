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
using VVE.VLog;

namespace VVE
{
    public class AnalyseWachttijdSettings
    {
        public int IntervalMin=60;

        public int BezettijdVerweglus=0;
        public int BezettijdKoplus=0;
        public int NormRijtijdVerweglus=0;
        public int MaxRijtijdVerweglus=0;
        public int IntrekkenAanvraag=0;

        public override string ToString()
        {
            return string.Format("Interval: {0} min.\r\nBezettijd verweglus: {1} ms.\r\nBezettijd koplus: {2} ms.\r\nNormale rijtijd verweglus: {3} ms.\r\nMax. rijtijd verweglus: {4} ms.\r\nIntrekken aanvraag: {5} ms.",
                IntervalMin,
                BezettijdVerweglus,
                BezettijdKoplus,
                NormRijtijdVerweglus,
                MaxRijtijdVerweglus,
                IntrekkenAanvraag
                );
        }
    }

    public class AnalyseWachttijdUitvoerSettingsCsv
    {
        public CsvFilter Filter = new CsvFilter();
        public bool IndividueleFietsers = false;
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Filter.ToString());
            sb.AppendFormat("Individuele fietsers in uitvoer: {0}",IndividueleFietsers?"aan":"uit");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Meetpunt waarop het algoritme toegepast kan worden
    /// </summary>
    public class WachttijdFietsMeetpunt
    {
        public VLogCfgItem Fc = null;
        public VLogCfgItem Koplus = null;
        public VLogCfgItem Verweglus = null;
        public VLogCfgItem Drukknop = null;
        public WgsLocation Location = new WgsLocation();
        public int Heading = 0;

        public override string ToString()
        {
            return string.Format("FC{0}\r\nKoplus {2}\r\nVerweglus {1}\r\nDrukknop {3}\r\nWGS84 {4}\r\nHeading {5}", Fc == null ? "NULL" : Fc.name, Verweglus == null ? "NULL" : Verweglus.name, Koplus == null ? "NULL" : Koplus.name, Drukknop == null ? "-" : Drukknop.name, Location, Heading);
        }
    }

    public enum FietsWaarnemingsPunt { verweglus, stopstreep}
    /// <summary>
    /// Individuele waargenomen fietser
    /// </summary>
    public class WachttijdFietser
    {
        public DateTime WaarnemingMoment;
        public FietsWaarnemingsPunt WaarnemingLocatie;
        public DateTime StartgroenMoment;
        public TimeSpan Wachttijd;
    }
        
    /// <summary>
    /// Lokaal gebruikte class voor het analyse algoritme
    /// </summary>
    public class WachttijdRichting
    {
        public WachttijdFietsMeetpunt Meetpunt;

        public List<WachttijdFietser> WachttijdFieters = new List<WachttijdFietser>();
        public List<WachttijdFietser> AanwezigeFietsers = new List<WachttijdFietser>();
        public DateTime WachtendVanaf = new DateTime();
        public bool WachtendMetDrukknop = false;
        public bool TellenMetVerweglus = false;

        public DateTime VerwegUpDt = new DateTime();
        public DateTime KopUpDt = new DateTime();
        public bool KopFietserAanwezigMaarNogNietAfgehandeld = false;
        public DateTime KopDownDt = new DateTime();
        public DateTime Startgroen = new DateTime();
        public bool LusError = false;
    }

    public class AnalyseWachttijden
    {
        public enum State { WaitForTimeRef = 1, WaitForStatus = 2, Active = 3 }

        //analyseert de V-Log detectiesignalen

        //processed berichten         | vereist | optioneel |
        // - tijdreferentie 0x01           x          -
        // - informatie 0x04               -          x
        // - status detectie 0x05          x          -
        // - wijziging detectie 0x06       x          -
        // - status ingangen 0x07          -          x
        // - status fc intern 0x09         -          -
        // - wijziging fc intern 0x0A      -          -
        // - status fc extern 0x0D         x          -
        // - wijziging fc extern 0x0E      x          -
        // - status uitgangen GUS 0x0B     -          x
        // - status WPS 0x13               x          -
        // - wijziging WPS 0x14            x          -

        State _state = State.WaitForTimeRef;
        DateTime waitForStatusTimeRef;
        VLogInfo waitForStatusInfo;
        VLog.VriState stateCurrent = new VLog.VriState();
        VLog.VriState stateNew = new VLog.VriState();

        VLogDataCfgMatch matchPeriods = new VLogDataCfgMatch();

        /// <summary>
        /// Informatie over in welke perioden wel en geen match is tussen de v-log data en configuratie
        /// </summary>
        public String VlcAllMatchInfo
        {
            get
            {
                return matchPeriods.GetText();
            }
        }

        public String VlcNoMatchInfo
        {
            get
            {
                return matchPeriods.GetText(onlyNoMatchPeriods: true);
            }
        }

        private StringBuilder errors = new StringBuilder();
        public String Errors
        {
            get
            {
                return errors.ToString().TrimEnd(new char[] { ' ', '\r', '\n' });
            }
        }
        private string lastError = "";
        private void addError(string error)
        {
            //herhalende errors overslaan
            if (lastError == error) return;
            lastError = error;

            if (stateNew.Time.Ticks > 0) this.errors.AppendFormat("{0}: {1}\r\n", stateNew.Time.ToString("yyyy-MM-dd HH:mm:ss.f"), error);
            else this.errors.AppendFormat("-: {0}\r\n", error);
        }

        private string vriName = "";
        public string VriName
        {
            get
            {
                return vriName;
            }
            set
            {
                vriName = value;
            }
        }

        private VLogCfg vlogConfig = null;
        public VLogCfg VLogConfig
        {
            set
            {
                vlogConfig = value;
            }
        }

        private AnalyseWachttijdSettings settings = new AnalyseWachttijdSettings();
        private TimeSpan intervalMin = new TimeSpan(1, 0, 0);//default 1 hour interval
        public AnalyseWachttijdSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = value;
                intervalMin = new TimeSpan((long)value.IntervalMin * 60 * 10 * 1000 * 1000);
            }
        }

        private AnalyseWachttijdUitvoerSettingsCsv uitvoerSettingsCsv = new AnalyseWachttijdUitvoerSettingsCsv();
        public AnalyseWachttijdUitvoerSettingsCsv UitvoerSettingsCsv
        {
            get
            {
                return uitvoerSettingsCsv;
            }
            set
            {
                uitvoerSettingsCsv = value;
            }
        }
        
        private WachttijdRichting[] fietsRichtingen = new WachttijdRichting[0];
        public WachttijdFietsMeetpunt[] Meetpunten
        {
            set
            {
                if (value == null) return;
                //meetpunten = value;

                //instellen fietsrichtingen
                fietsRichtingen = new WachttijdRichting[value.Length];
                for(int i = 0;i<value.Length;i++)
                {
                    fietsRichtingen[i] = new WachttijdRichting() { Meetpunt = value[i] };
                }
            }
        }

        private List<PeriodResultWachttijden> periodResults = new List<PeriodResultWachttijden>();
        /// <summary>
        /// Meetperiode resultaten
        /// </summary>
        public PeriodResultWachttijden[] PeriodResults
        {
            get
            {
                return periodResults.ToArray();
            }
        }    

        public string GetCsvResult(DateTime from, DateTime to)
        {
            StringBuilder result = new StringBuilder();

            if (uitvoerSettingsCsv.Filter == null || uitvoerSettingsCsv.Filter.Weekdagen == null || uitvoerSettingsCsv.Filter.Weekdagen.Length != 7) return "ERROR weekdagen";

            int aantalMeetpunten = 0;

            if (periodResults.Count == 0)
            {
                //geen resultaten
                return "";
            }
            else
            {
                PeriodResultWachttijden prFirst = periodResults[0];
                aantalMeetpunten = prFirst.Result.Length;

                //header
                result.AppendFormat("VRI {0}", vriName);

                foreach (PeriodResultWachttijdFC prwFc in prFirst.Result)
                {
                    result.AppendFormat(";FC{0};;;;;", prwFc.FcName);
                }
                result.AppendLine();
                for (int i = 0; i < prFirst.Result.Length; i++)
                {
                    result.AppendFormat(";Gemiddelde;Max;Min;Aantal bemeten;Verliesminuten;Percentage groenaankomst");
                }
                result.AppendLine();


                //resultaten in tabel met gemiddeld-/max-/min-wachttijd en aantal
                DateTime nextPrStart = startMomentPeriod(from); //verwachtte eerste periode met resultaten
                foreach (PeriodResultWachttijden pr in periodResults)
                {
                    if (pr.Result == null && pr.Result.Length != aantalMeetpunten) continue; //indien aantal meetpunten is gewijzigd, geen uitvoer meer uitsturen

                    //checken of er gaten tussen de perioden zitten
                    while (nextPrStart < pr.PeriodStart) //gat = genereren lege regels (tussen begin datum en eerste pr of tussen pr's in)
                    {
                        //lege regel toevoegen
                        if (uitvoerSettingsCsv.Filter.ValidInFilter(nextPrStart))
                        {
                            result.AppendFormat("{0}", nextPrStart.ToString("yyyy-MM-dd HH:mm"));
                            for (int i = 0; i < aantalMeetpunten; i++)
                            {
                                // if (filterAlleenKoplussen && koplussen[i] == false) continue;
                                result.Append(";;;;;;"); //gehele periode ongeldige telling
                            }
                            result.AppendLine();
                        }

                        nextPrStart += intervalMin;
                    }
                    nextPrStart = pr.PeriodStart + pr.PeriodLength;

                    if (uitvoerSettingsCsv.Filter.ValidInFilter(pr.PeriodStart))
                    {
                        result.AppendFormat("{0}", pr.PeriodStart.ToString("yyyy-MM-dd HH:mm"));
                        foreach (PeriodResultWachttijdFC prFc in pr.Result)
                        {
                            if (pr.DataComplete && !prFc.LusError)
                            {
                                result.AppendFormat(";{0};{1};{2};{3};{4};{5}",
                                                    prFc.WachttijdGemiddelde.TotalSeconds.ToString("F1"),
                                                    prFc.WachttijdMax.TotalSeconds.ToString("F1"),
                                                    prFc.WachttijdMin.TotalSeconds.ToString("F1"),
                                                    prFc.AantalFietsers,
                                                    prFc.FietsVerliesTijd.TotalMinutes.ToString("F1"),
                                                    prFc.PercentageGroenAankomst == -1 ? "" : prFc.PercentageGroenAankomst.ToString("F1"));
                            }
                            else
                            {
                                //geen gehele periode aan data en/of een lusfout geconstateerd door de regeling
                                result.AppendFormat(";;;;;;");
                            }
                        }
                        result.AppendLine();
                    }
                }
                if (nextPrStart < to)
                {
                    //gat = genereren lege regels (tussen laatste prw en einddatum)
                    while (nextPrStart < to)
                    {
                        if (uitvoerSettingsCsv.Filter.ValidInFilter(nextPrStart))
                        {
                            //lege regel toevoegen
                            result.AppendFormat("{0}", nextPrStart.ToString("yyyy-MM-dd HH:mm"));
                            for (int i = 0; i < aantalMeetpunten; i++)
                            {
                                //         if (filterAlleenKoplussen && koplussen[i] == false) continue;
                                result.Append(";;;;;;"); //gehele periode ongeldige telling
                            }
                            result.AppendLine();
                        }

                        nextPrStart += intervalMin;
                    }
                }
                result.AppendLine();

                //eventueel uitgebreide uitvoer opnemen
                if (uitvoerSettingsCsv.IndividueleFietsers)
                {
                    //alle richtingen en individuele fietsers
                    for (int i = 0; i < fietsRichtingen.Length; i++)
                    {
                        WachttijdRichting wtr = fietsRichtingen[i];
                        result.AppendFormat("{0}\r\n", wtr.Meetpunt);
                        result.AppendFormat("waarneming;locatie;groenmoment;wachttijd\r\n");
                        foreach (PeriodResultWachttijden pr in periodResults)
                        {
                            foreach (WachttijdFietser wtf in pr.Result[i].Wachttijden)
                            {
                                result.AppendFormat("{0};{1};{2};{3}\r\n", wtf.WaarnemingMoment.ToString("yyyy-MM-dd HH:mm:ss.f"), wtf.WaarnemingLocatie == FietsWaarnemingsPunt.stopstreep ? "stopstreep" : "verweglus", wtf.StartgroenMoment.ToString("yyyy-MM-dd HH:mm:ss.f"), wtf.Wachttijd.TotalSeconds.ToString("F1"));
                            }
                        }
                        result.AppendLine();
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Generatie DPF resultaat. Return DPF data of null=fout        
        /// </summary>
        public DpfData DpfResult
        {
            get
            {
                DpfData result = new DpfData();

                //header
                result.Header = "location-id;lat;lon;heading;method;quality;period-from;period-to;time-from;time-to;per;wait-time;max-wait-time;time-loss-minutes;percentage-green-arrival";

                //prefix genereren voor ieder meetpunt
                Dictionary<string, string> prefixen = new Dictionary<string, string>(); //key=fcname

                //zoeken meetpunt
                foreach (WachttijdRichting wtr in fietsRichtingen)
                {
                    string fc = wtr.Meetpunt.Fc.name;
                    string prefix = string.Format("{0}-{1};{2};{3};{4};trafficlight-induction;100;",
                                              // return string.Format("K001_{0}-{1};{2};{3};{4};trafficlight-induction;100;",
                                              VLogArchiefDenHaag.VriNameToFullKName(vriName),
                                              fc,
                                              wtr.Meetpunt.Location.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                                              wtr.Meetpunt.Location.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                                              wtr.Meetpunt.Heading);
                    prefixen.Add(fc, prefix);
                }

                //data
                foreach (PeriodResultWachttijden prw in periodResults) //voor iedere periode
                {
                    if (prw.DataComplete)
                    {
                        foreach (PeriodResultWachttijdFC prwFc in prw.Result) //voor iedere FC in de periode
                        {
                            string fc = prwFc.FcName;
                            if (prefixen.ContainsKey(fc))
                            {
                                if (prwFc.LusError) continue; //bij een detectorfout geen resultaat naar DPF 

                                string record = string.Format("{0}{1};{2};{3};{4};0;{5};{6};{7};{8}",
                                                              prefixen[fc],
                                                              prw.PeriodStart.ToString("yyyy-MM-dd"),
                                                              prw.PeriodStart.Add(prw.PeriodLength).ToString("yyyy-MM-dd"),
                                                              prw.PeriodStart.ToString("HH:mm:ss"),
                                                              prw.PeriodStart.Add(prw.PeriodLength).ToString("HH:mm:ss"),
                                                              prwFc.WachttijdGemiddeldeSec.ToString("F1", System.Globalization.CultureInfo.InvariantCulture),
                                                              prwFc.WachttijdMax.TotalSeconds.ToString("F1", System.Globalization.CultureInfo.InvariantCulture),
                                                              prwFc.FietsVerliesTijd.TotalMinutes.ToString("F1", System.Globalization.CultureInfo.InvariantCulture),
                                                              prwFc.PercentageGroenAankomst == -1 ? "" : prwFc.PercentageGroenAankomst.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)
                                                              );
                                result.Records.Add(record);
                            }
                            else return null; //FC niet aanwezig in de prefix, dus niet aanwezig in de meetpunten
                        }
                    }
                }

                return result;
            }
        }

        private bool _initRun = true;
        /// <summary>
        /// Parsed de V-Log data en indien alle benodigde informatie ontvangen is en vrij van fouten, wordt de analyse uitgevoerd.
        /// </summary>
        /// <param name="vlogBericht"></param>
        public void Process(string vlogBericht)
        {
            if (vlogBericht.Length < 2) return;
            
            string berichttype = vlogBericht.Substring(0, 2);

            bool error = false;
            bool errorToWaitForStatus = false;
            bool process = false;
            bool checkDataVsConfig = false;

            if (_state == State.WaitForTimeRef &&
                berichttype == "01")
            {
                //tijdreferentie
                DateTime tijdref = VLog.Parser.DecodeTijdRef(vlogBericht);
                bool okMsg = (tijdref.Ticks > 0);
                if (okMsg)
                {
                    //OK
                    waitForStatusTimeRef = tijdref;
                    _state = State.WaitForStatus;
                }
                else
                {
                    //error
                    error = true;

                    addError(string.Format("Incorrect timeref message {0}", vlogBericht));
                }
            }
            else if (_state == State.WaitForStatus)
            {
                //wachten op statusbericht
                switch (berichttype)
                {
                    case "01":
                        {
                            //tijdreferentie ontvangen: de huidige overschrijven
                            DateTime tijdref = VLog.Parser.DecodeTijdRef(vlogBericht);
                            bool okMsg = (tijdref.Ticks > 0);
                            if (okMsg)
                            {
                                //OK
                                waitForStatusTimeRef = tijdref;
                                _state = State.WaitForStatus; //in state blijven
                            }
                            else
                            {
                                //error
                                error = true;

                                addError(string.Format("Incorrect timeref message {0}", vlogBericht));
                            }
                            break;
                        }
                    case "04":
                        {
                            VLogInfo info = VLog.Parser.DecodeInfo(vlogBericht);

                            bool okMsg = (info != null);
                            if (okMsg)
                            {
                                //OK
                                waitForStatusInfo = info;
                            }
                            else
                            {
                                //error
                                error = true;

                                addError(string.Format("Incorrect info message {0}", vlogBericht));
                            }
                            break;
                        }
                    case "05":
                        {
                            //status detectoren ontvangen
                            int[] detectors = VLog.Parser.DecodeStatusDet(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (detectors != null); //geldig bericht
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct

                            if (okMsg &&
                                okDeltaMsg)
                            {
                                //OK
                                stateNew.TimeRef = waitForStatusTimeRef;
                                stateNew.Delta = delta;
                                stateNew.Info = waitForStatusInfo;
                                waitForStatusInfo = null;
                                stateNew.Detectors = detectors;

                                if (stateNew.Detectors != null && 
                                    stateNew.FcExtern != null &&
                                    stateNew.Wps != null)
                                {
                                    //zowel detectoren als signaalgroepen aanwezig, dus verwerken
                                    _state = State.Active;
                                    process = true;

                                    checkDataVsConfig = true;
                                }
                            }
                            else
                            {
                                //error
                                error = true;

                                addError(string.Format("Incorrect status detector message {0}", vlogBericht));
                            }
                            break;
                        }
                    case "0D":
                        {
                            //status FC extern ontvangen
                            int[] fcs = VLog.Parser.DecodeStatusFcExt(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (fcs != null); //geldig bericht
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct

                            if (okMsg &&
                                okDeltaMsg)
                            {
                                //OK
                                stateNew.TimeRef = waitForStatusTimeRef;
                                stateNew.Delta = delta;
                                stateNew.Info = waitForStatusInfo;
                                waitForStatusInfo = null;
                                stateNew.FcExtern = fcs;

                                if (stateNew.Detectors != null && 
                                    stateNew.FcExtern != null &&
                                    stateNew.Wps != null)
                                {
                                    //zowel detectoren als signaalgroepen aanwezig, dus verwerken
                                    _state = State.Active;
                                    process = true;

                                    checkDataVsConfig = true;
                                }
                            }
                            else
                            {
                                //error
                                error = true;

                                addError(string.Format("Incorrect status detector message {0}", vlogBericht));
                            }
                            break;
                        }
                    case "13":
                        {
                            //status WPS
                            int[] wps = VLog.Parser.DecodeStatusWPS(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (wps != null); //geldig bericht
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct

                            if (okMsg &&
                                okDeltaMsg)
                            {
                                //OK
                                stateNew.TimeRef = waitForStatusTimeRef;
                                stateNew.Delta = delta;
                                stateNew.Info = waitForStatusInfo;
                                waitForStatusInfo = null;
                                stateNew.Wps = wps;

                                if (stateNew.Detectors != null && 
                                    stateNew.FcExtern != null &&
                                    stateNew.Wps != null)
                                {
                                    //zowel detectoren als signaalgroepen aanwezig, dus verwerken
                                    _state = State.Active;
                                    process = true;

                                    checkDataVsConfig = true;
                                }
                            }
                            else
                            {
                                //error
                                error = true;

                                addError(string.Format("Incorrect status detector message {0}", vlogBericht));
                            }
                            break;
                        }
                    default:
                        {
                            //ander berichttype: alleen controleren op hex ascii data
                            if (!VLog.Parser.IsHexAscii(vlogBericht))
                            {
                                error = true;
                                addError(string.Format("Incorrect message {0}, geen Hex Ascii", vlogBericht));
                            }
                            break;
                        }
                }
            }
            else if (_state == State.Active)
            {
                switch (berichttype)
                {
                    case "01":
                        {
                            //tijdreferentie
                            DateTime tijdref = VLog.Parser.DecodeTijdRef(vlogBericht);

                            bool okMsg = (tijdref.Ticks > 0);
                            bool okMsgCorrect = (tijdref.TimeOfDay.TotalSeconds % (60 * 5) == 0 &&
                                                (tijdref - stateNew.TimeRef).TotalSeconds <= 5 * 60);//controleren of het tijdstip op de hele 5 minuten ligt en binnen 5 minuten na de voorgaande
                            if (okMsg &&
                                okMsgCorrect)
                            {
                                //OK
                                stateNew.TimeRef = tijdref;
                                stateNew.Delta = 0;
                                process = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect timeref message {0}", vlogBericht));
                                else if (!okMsgCorrect)
                                {
                                    //gat in de data of tijd gewijzigd. Deze tijdref wel als nieuwe start zien.
                                    waitForStatusTimeRef = tijdref;
                                    errorToWaitForStatus = true;

                                    addError(string.Format("Unexpected timeref {0} received after last timeref {1} (time changed or data gap), time difference: {2}", tijdref.ToString("yyyy-MM-dd HH:mm:ss.f"), stateNew.TimeRef.ToString("yyyy-MM-dd HH:mm:ss.f"), (tijdref - stateCurrent.Time).TotalSeconds.ToString("F1")));
                                }
                            }
                            break;
                        }
                    case "04":
                        {
                            VLogInfo info = VLog.Parser.DecodeInfo(vlogBericht);

                            bool okMsg = (info != null);
                            bool okNoChange = (stateNew.Info == null || info == null || (stateNew.Info.VLogVersie == info.VLogVersie && stateNew.Info.VriId == info.VriId));
                            if (okMsg && okNoChange)
                            {
                                //OK
                                if (stateNew.Info == null) stateNew.Info = info;

                                checkDataVsConfig = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect info message {0}", vlogBericht));
                                else if (!okNoChange) addError(string.Format("Content of info message changed {0}", vlogBericht));
                            }
                            break;
                        }
                    case "05":
                        {
                            //status detectie
                            int[] detectors = VLog.Parser.DecodeStatusDet(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (detectors != null); //geldig bericht
                            bool okLength = (stateNew.Detectors == null || detectors == null || detectors.Length == stateNew.Detectors.Length); //aantal detectoren gelijk
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande

                            if (okMsg &&
                                okLength &&
                                okDeltaMsg &&
                                okDeltaCorrect)
                            {
                                //OK
                                stateNew.Delta = delta;
                                stateNew.Detectors = detectors;
                                process = true;

                                checkDataVsConfig = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect status detector message {0} (format incorrect)", vlogBericht));
                                else if (!okLength) addError(string.Format("Incorrect status detector message {0} (amount of detector {1} instead of {2})", vlogBericht, detectors.Length, stateNew.Detectors.Length));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect status detector message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect status detector message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
                            }
                            break;
                        }
                    case "06":
                        {
                            //wijziging detectie
                            Change[] changes = VLog.Parser.DecodeWijzigingDet(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (changes != null); //geldig bericht
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande

                            if (okMsg &&
                                okDeltaMsg &&
                                okDeltaCorrect)
                            {
                                //OK
                                stateNew.Delta = delta;
                                for (int i = 0; i < changes.Length; i++)
                                {
                                    Change dc = changes[i];
                                    if (dc.index >= stateNew.Detectors.Length)
                                    {
                                        //error: index te hoog, detector niet aanwezig in eerder ontvangen statusbericht
                                        error = true;

                                        addError(string.Format("Incorrect detector changed message {0} (index {1} higher than available {2} detectors)", vlogBericht, dc.index, stateNew.Detectors.Length));
                                        break;
                                    }
                                    else
                                    {
                                        stateNew.Detectors[dc.index] = dc.value;
                                    }
                                }
                                process = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect change detector message {0} (format incorrect)", vlogBericht));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect change detector message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect change detector message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
                            }
                            break;
                        }
                    case "07":
                        {
                            //status ingangssignalen
                            bool[] overigeIs = VLog.Parser.DecodeStatusIs(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (overigeIs != null); //geldig bericht
                            bool okLength = (stateNew.OverigeIs == null || overigeIs == null || overigeIs.Length == stateNew.OverigeIs.Length); //aantal detectoren gelijk
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande
                            
                            if (okMsg &&
                                okLength &&
                                okDeltaMsg &&
                                okDeltaCorrect)
                            {
                                //OK                       
                                stateNew.Delta = delta;
                                stateNew.OverigeIs = overigeIs;

                                checkDataVsConfig = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect status inputsignals message {0} (format incorrect)", vlogBericht));
                                else if (!okLength) addError(string.Format("Incorrect status inputsignals message {0} (amount of inputsignals {1} instead of {2})", vlogBericht, overigeIs.Length, stateNew.OverigeIs.Length));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect status inputsignals message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect status inputsignals message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
                            }
                            break;
                        }
                    case "0D":
                        {
                            //status fc extern
                            int[] fcs = VLog.Parser.DecodeStatusFcExt(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (fcs != null); //geldig bericht
                            bool okLength = (stateNew.FcExtern == null || fcs == null || fcs.Length == stateNew.FcExtern.Length); //aantal signaalgroepen gelijk
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande
                            bool okFcSequence = true; //checken of fcextern wel netjes rondjes van groen-geel-rood maakt
                            if (stateNew.FcExtern != null && okMsg && okLength)
                            {
                                //per bemeten signaalgroep checken of de fc volgorde correct is
                                foreach (WachttijdRichting ri in fietsRichtingen)
                                {
                                    int fcIndex = ri.Meetpunt.Fc.index;
                                    bool ok = Parser.FcExternSequenceOk(stateNew.FcExtern[fcIndex], fcs[fcIndex], stateNew.Wps[0]);
                                    if (!ok) okFcSequence = false;
                                }
                            }

                            if (okMsg &&
                            okLength &&
                            okDeltaMsg &&
                            okDeltaCorrect &&
                            okFcSequence)
                            {
                                //OK
                                stateNew.Delta = delta;
                                stateNew.FcExtern = fcs;
                                process = true;

                                checkDataVsConfig = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect status external signalgroup message {0} (format incorrect)", vlogBericht));
                                else if (!okLength) addError(string.Format("Incorrect status external signalgroup message {0} (amount of signalgroups {1} instead of {2})", vlogBericht, fcs.Length, stateNew.FcExtern.Length));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect status external signalgroup message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect status external signalgroup message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
                                else if (!okFcSequence) addError(string.Format("Unexpected sequence detected for an external signalgroup in message {0}", vlogBericht));
                            }
                            break;
                        }
                    case "0E":
                        {
                            //wijziging fc extern
                            Change[] changes = VLog.Parser.DecodeWijzigingFcExt(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (changes != null); //geldig bericht
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande

                            if (okMsg &&
                                okDeltaMsg &&
                                okDeltaCorrect)
                            {
                                //OK
                                stateNew.Delta = delta;
                                for (int i = 0; i < changes.Length; i++)
                                {
                                    Change dc = changes[i];
                                    if (dc.index >= stateNew.FcExtern.Length)
                                    {
                                        //error: index te hoog, fc niet aanwezig in eerder ontvangen statusbericht
                                        error = true;

                                        addError(string.Format("Incorrect fc external changed message {0} (index {1} higher than available {2} fc's)", vlogBericht, dc.index, stateNew.FcExtern.Length));
                                        break;
                                    }
                                    else
                                    {
                                        //per bemeten signaalgroep checken of de fc volgorde correct is
                                        bool okFcSequence = true;
                                        foreach (WachttijdRichting ri in fietsRichtingen)
                                        {
                                            if (dc.index == ri.Meetpunt.Fc.index)
                                            {
                                                bool ok = Parser.FcExternSequenceOk(stateNew.FcExtern[dc.index], dc.value, stateNew.Wps[0]);
                                                if (!ok) okFcSequence = false;
                                                break;
                                            }
                                        }

                                        //checken of fcextern wel netjes rondjes van groen-geel-rood maakt
                                        if (!okFcSequence)
                                        {
                                            error = true;
                                            addError(string.Format("Unexpected sequence detected for an external signalgroup in message {0} (index {1} value from {2} to {3})", vlogBericht, dc.index, stateNew.FcExtern[dc.index], dc.value));
                                            break;
                                        }
                                       
                                        stateNew.FcExtern[dc.index] = dc.value;
                                    }
                                }
                                process = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect change fc external message {0} (format incorrect)", vlogBericht));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect change fc external message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect change fc external message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
                            }
                            break;
                        }
                    case "0B":
                        {
                            //status uitgangssignalen GUS
                            bool[] overigeUsGus = VLog.Parser.DecodeStatusUsGus(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (overigeUsGus != null); //geldig bericht
                            bool okLength = (stateNew.OverigeUsGus == null || overigeUsGus == null || overigeUsGus.Length == stateNew.OverigeUsGus.Length); //aantal detectoren gelijk
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande

                            if (okMsg &&
                                okLength &&
                                okDeltaMsg &&
                                okDeltaCorrect)
                            {
                                //OK                       
                                stateNew.Delta = delta;
                                stateNew.OverigeUsGus = overigeUsGus;

                                checkDataVsConfig = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect status outputsignals GUS message {0} (format incorrect)", vlogBericht));
                                else if (!okLength) addError(string.Format("Incorrect status outputsignals GUS message {0} (amount of outputsignals {1} instead of {2})", vlogBericht, overigeUsGus.Length, stateNew.OverigeUsGus.Length));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect status outputsignals GUS message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect status outputsignals GUS message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
                            }
                            break;
                        }
                    case "13":
                        {
                            //status wps
                            int[] wps = VLog.Parser.DecodeStatusWPS(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (wps != null); //geldig bericht
                            bool okLength = (stateNew.Wps == null || wps == null || wps.Length == stateNew.Wps.Length); //aantal wps gelijk
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande

                            if (okMsg &&
                                okLength &&
                                okDeltaMsg &&
                                okDeltaCorrect)
                            {
                                //OK
                                stateNew.Delta = delta;
                                stateNew.Wps = wps;
                                process = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect status WPS message {0} (format incorrect)", vlogBericht));
                                else if (!okLength) addError(string.Format("Incorrect status WPS message {0} (amount of WPS {1} instead of {2})", vlogBericht, wps.Length, stateNew.Wps.Length));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect status WPS message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect status WPS message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));                                
                            }
                            break;
                        }
                    case "14":
                        {
                            //wijziging wps
                            Change[] changes = VLog.Parser.DecodeWijzigingWPS(vlogBericht);
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);

                            bool okMsg = (changes != null); //geldig bericht
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande

                            if (okMsg &&
                                okDeltaMsg &&
                                okDeltaCorrect)
                            {
                                //OK
                                stateNew.Delta = delta;
                                for (int i = 0; i < changes.Length; i++)
                                {
                                    Change dc = changes[i];
                                    if (dc.index >= stateNew.Wps.Length)
                                    {
                                        //error: index te hoog, wps niet aanwezig in eerder ontvangen statusbericht
                                        error = true;

                                        addError(string.Format("Incorrect WPS changed message {0} (index {1} higher than available {2} WPS)", vlogBericht, dc.index, stateNew.Wps.Length));
                                        break;
                                    }
                                    else
                                    {
                                        stateNew.Wps[dc.index] = dc.value;
                                    }
                                }
                                process = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect change WPS message {0} (format incorrect)", vlogBericht));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect change WPS message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect change WPS message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
                            }
                            break;
                        }
                    default:
                        {
                            //ander berichttype: alleen controleren op hex ascii data
                            if (!VLog.Parser.IsHexAscii(vlogBericht))
                            {
                                error = true;
                                addError(string.Format("Incorrect message {0}, geen Hex Ascii", vlogBericht));
                            }

                            //delta extraheren
                            int delta = VLog.Parser.DecodeDelta(vlogBericht);
                            bool okDeltaMsg = (delta != -1); //delta in bericht correct
                            bool okDeltaCorrect = (delta >= stateNew.Delta); //delta gelijk aan of groter dan voorgaande
                            if (!okDeltaMsg) break; //niet kunnen bepalen van delta kan voorkomen doordat niet alle berichten zijn geimplementeerd
                            if (okDeltaCorrect)
                            {
                                //OK                       
                                stateNew.Delta = delta;
                            }
                            else
                            {
                                //error
                                error = true;
                                addError(string.Format("Incorrect message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
                            }
                        }
                        break;
                }
            } //einde _state conditie

            //analyse uitvoeren
            if (_state == State.WaitForStatus) matchPeriods.UpdateLastKnowTime(waitForStatusTimeRef);
            if (_state == State.Active)
            {
                matchPeriods.UpdateLastKnowTime(stateNew.Time);

                if (checkDataVsConfig)
                {
                    //check op correctheid data t.o.v. configuratie                        
                    DataInfo vdi = stateNew.DataInfo;
                    bool compareOk = vdi.EqualTo(vlogConfig.Sys, vlogConfig.VLogVersie, vlogConfig.Dp.Length, vlogConfig.Fc.Length, vlogConfig.Is.Length, vlogConfig.Us.Length);
                    if (!compareOk)
                    {
                        //error
                        error = true;

                        addError(string.Format("V-Log data doesn't fit configuration. Data: {1}, Config: {2}.", vlogBericht, vdi, vlogConfig));
                    }
                    matchPeriods.StartPeriod(compareOk, stateNew.Time);
                }

                if (!error && process)
                {
                    if (_initRun)
                    {
                        analyseInit();
                        stateCurrent = stateNew.Clone();
                        _initRun = false;
                    }
                    analyse();
                    stateCurrent = stateNew.Clone();
                }

                if (!error && !process)
                {
                    //alleen delta kopieren
                    stateCurrent.Delta = stateNew.Delta;
                }
            }

            if (error)
            {
                //resetten
                if (errorToWaitForStatus) _state = State.WaitForStatus;
                else _state = State.WaitForTimeRef;
                errorInData();
                stateNew = new VLog.VriState();
                stateCurrent = new VLog.VriState();
            }
        }

        /// <summary>
        /// Einde van het parsen van v-log data
        /// </summary>
        public void End()
        {
            //matched periods afronden
            matchPeriods.End(stateCurrent.Time);
        }

        // ---- tijdelijke parameters voor analyse algoritme ----

        //meetperiode gegevens
        bool _periodeActive = false;
        DateTime _startPeriod;
        DateTime _endPeriod;
        bool _dataFromStartPeriod = false;
        bool _waitUntilNextPeriodMinimalDT = false; //geeft aan of het analyseren in de huidige periode overgeslagen moet worden
        DateTime _nextPeriodMinimalDT;
        DateTime _firstDtInData;
        //meetperiode resultaten

        private void analyseInit()
        {
            //wordt uitgevoerd indien voor het eerst geldige V-Log informatie aanwezig is in stateNew, nog geen informatie aanwezig in stateCurrent           
            _firstDtInData = stateNew.Time;

            if (!_periodeActive && allowedToStartPeriodResult())
            {
                //nog geen resultaten beschikbaar
                _waitUntilNextPeriodMinimalDT = false;

                //aanmaken eerste perioderesultaat
                _startPeriod = startMomentPeriod(stateNew.Time);
                _endPeriod = _startPeriod.Add(intervalMin);
                _dataFromStartPeriod = (stateNew.Time == _startPeriod); //bepalen of vanaf begin van periode data aanwezig is   

                _periodeActive = true;
            }

            foreach (WachttijdRichting ri in fietsRichtingen)
            {
                /*if (stateNew.FC(ri.Config.Fc.index) == VriState.FcState.rood) ri.State = StateAnalyse.neutraal;
                 else ri.State =  StateAnalyse.*/

                ri.AanwezigeFietsers = new List<WachttijdFietser>();
                ri.WachtendVanaf = new DateTime();
                ri.WachtendMetDrukknop = false;
                ri.TellenMetVerweglus = false;

                if (stateNew.Detectors[ri.Meetpunt.Verweglus.index] == 1) ri.VerwegUpDt = stateNew.Time;
                if (stateNew.Detectors[ri.Meetpunt.Koplus.index] == 1)
                {
                    ri.KopUpDt = stateNew.Time;
                    ri.KopFietserAanwezigMaarNogNietAfgehandeld = true;
                }
                if (stateNew.Detectors[ri.Meetpunt.Koplus.index] == 0) ri.KopDownDt = stateNew.Time;

                if (stateNew.FcStateFromFcExternalWps(ri.Meetpunt.Fc.index) == Parser.FcState.groen) ri.Startgroen = stateNew.Time;
            }
        }

        private void analyse()
        {
            //wordt uitgevoerd indien er nieuwe informatie aanwezig is in stateNew, stateCurrent bevat altijd de informatie van de voorgaande stateNew

            if (!_periodeActive)
            {
                if (allowedToStartPeriodResult())
                {
                    //nieuwe periode aangebroken waarin geteld mag worden
                    _waitUntilNextPeriodMinimalDT = false;

                    //aanmaken eerste perioderesultaat
                    foreach (WachttijdRichting wtr in fietsRichtingen) wtr.WachttijdFieters.Clear();                    
                    _startPeriod = startMomentPeriod(stateNew.Time);
                    _endPeriod = _startPeriod.Add(intervalMin);
                    _dataFromStartPeriod = (_firstDtInData <= _startPeriod); //bepalen of vanaf begin van periode data aanwezig is    

                    _periodeActive = true;
                }
            }
            else
            {
                if (stateNew.Time >= _startPeriod &&
                    stateNew.Time < _endPeriod)
                {
                    //huidige perioderesultaat gebruiken
                }
                else if (stateNew.Time >= _endPeriod)
                {
                    //nieuwe perioderesultaat aangebroken

                    //oude resultaat opslaan
                    PeriodResultWachttijden pr = new PeriodResultWachttijden();
                    pr.DataComplete = _dataFromStartPeriod;
                    pr.PeriodStart = _startPeriod;
                    pr.PeriodLength = _endPeriod - _startPeriod;
                    pr.Result = new PeriodResultWachttijdFC[fietsRichtingen.Length];
                    for (int i = 0; i < fietsRichtingen.Length; i++)
                    {
                        WachttijdRichting wtr = fietsRichtingen[i];

                        PeriodResultWachttijdFC prFc = new PeriodResultWachttijdFC
                        {
                            FcName = wtr.Meetpunt.Fc.name,
                            Wachttijden = wtr.WachttijdFieters.ToArray(),
                            LusError = wtr.LusError
                        };

                        pr.Result[i] = prFc;
                    }
                    periodResults.Add(pr);

                    //nieuwe periode aanmaken
                    foreach (WachttijdRichting wtr in fietsRichtingen)
                    {
                        wtr.WachttijdFieters.Clear();
                        wtr.LusError = false;
                    }
                    _startPeriod = startMomentPeriod(stateNew.Time);
                    _endPeriod = _startPeriod.Add(intervalMin);
                    _dataFromStartPeriod = true; //hier wordt niet gecontroleerd of de periode aansluitend aan de voorgaand ligt, maar wel vanuit gegaan. Bij de controle van V-Log data wordt namelijk al gecontroleerd op data gaten en dus mogen we ervanuit gaan dat het aaneengesloten data is.                    
                }
                else if (stateNew.Time < _startPeriod)
                {
                    //nieuwe tijd in een eerdere periode: dit wordt niet ondersteund en situatie wordt eerder afgevangen door controle van V-Log data
                    //deze conditie komt dus niet voor

                    throw new Exception("Nieuwe tijd eerder dan oude tijd: wordt niet ondersteund");
                }
            }

            foreach (WachttijdRichting ri in fietsRichtingen)
            {
                //verdere runs bij valide data

                //signaalgroepen pre-processing
                Parser.FcState fcNew = stateNew.FcStateFromFcExternalWps(ri.Meetpunt.Fc.index);
                Parser.FcState fcCurrent = stateCurrent.FcStateFromFcExternalWps(ri.Meetpunt.Fc.index);

                bool fcStartgroen = (fcCurrent != Parser.FcState.groen && fcNew == Parser.FcState.groen);
                bool fcStartgeel = (fcCurrent != Parser.FcState.geel && fcNew == Parser.FcState.geel);
                bool fcStartrood = (fcCurrent != Parser.FcState.rood && fcNew == Parser.FcState.rood);
                bool fcGroen = (fcNew == Parser.FcState.groen);
                bool fcGeel = (fcNew == Parser.FcState.geel);
                bool fcRood = (fcNew == Parser.FcState.rood);

                //verweglus pre-processing
                int verwegIdx = ri.Meetpunt.Verweglus.index;
                int verwegNew = stateNew.Detectors[verwegIdx];
                int verwegCurrent = stateCurrent.Detectors[verwegIdx];
                bool verwegError = verwegNew > 1;
                bool verwegUp = (verwegCurrent != 1 && verwegNew == 1);
                bool verwegDown = (verwegCurrent != 0 && verwegNew == 0);

                //koplus pre-processing
                int kopIdx = ri.Meetpunt.Koplus.index;
                int kopNew = stateNew.Detectors[kopIdx];
                int kopCurrent = stateCurrent.Detectors[kopIdx];
                bool kopError = kopNew > 1;
                bool kopUp = (kopCurrent != 1 && kopNew == 1);
                bool kopDown = (kopCurrent != 0 && kopNew == 0);

                //lusdetectie error afhandeling
                if (verwegError || kopError) ri.LusError = true;

                //drukknop pre-processing
                bool drukknopAanwezig = (ri.Meetpunt.Drukknop != null);
                int drukknopIdx = -1;
                int drukknopNew = 0;
                int drukknopCurrent = 0;
                bool drukknopError = false;
                bool drukknopUp = false;
                if (drukknopAanwezig)
                {
                    drukknopIdx = ri.Meetpunt.Drukknop.index;
                    drukknopNew = stateNew.Detectors[drukknopIdx];
                    drukknopCurrent = stateCurrent.Detectors[drukknopIdx];
                    drukknopError = drukknopNew > 1;
                    drukknopUp = (drukknopCurrent != 1 && drukknopNew == 1);
                }
                
                // - beveiliging aanvraag door roodlichtnegatie
                if (kopDown)
                {
                    ri.KopDownDt = stateNew.Time;
                }
                if (ri.KopDownDt.Ticks > 0)
                {
                    TimeSpan kopLowTime = stateNew.Time - ri.KopDownDt;

                    if (fcRood &&
                        kopLowTime.TotalMilliseconds >= settings.IntrekkenAanvraag &&
                        ri.WachtendVanaf.Ticks > 0 &&
                        !ri.WachtendMetDrukknop)
                    {
                        //aanvraag intrekken in rood, indien de drukknop niet gebruikt is en hiaat gevallen is
                        ri.WachtendVanaf = new DateTime();
                        ri.TellenMetVerweglus = false;
                    }
                }
                if (kopUp)
                {
                    ri.KopDownDt = new DateTime(); //resetten
                }

                // - situatie A
                bool sitAfietserOpVerweglus = false;
                if (verwegUp) ri.VerwegUpDt = stateNew.Time;
                if (verwegDown)
                {
                    if (ri.VerwegUpDt.Ticks > 0)
                    {
                        TimeSpan verwegHighTime = stateNew.Time - ri.VerwegUpDt;
                        if (verwegHighTime.TotalMilliseconds >= settings.BezettijdVerweglus)
                        {
                            //fietser gedetecteerd op verweglus
                            sitAfietserOpVerweglus = true;
                        }
                    }
                }

                // - situatie B, C en D
                bool sitBwachtendeFietserOpKoplus = false;
                DateTime sitBwachtendeFietserOpKoplusDt = new DateTime();
                bool sitCwachtendeFietserBijStartgroen = false;
                DateTime sitCwachtendeFietserBijStartgroenDt = new DateTime();
                bool sitDfietserBijGroenEnGeel = false;
                DateTime sitDfietserBijGroenEnGeelDt = new DateTime();
                if (kopUp)
                {
                    ri.KopUpDt = stateNew.Time;
                    ri.KopFietserAanwezigMaarNogNietAfgehandeld = true;
                }
                if (ri.KopFietserAanwezigMaarNogNietAfgehandeld)
                {
                    TimeSpan kopHighTime = stateNew.Time - ri.KopUpDt;

                    //situatie B
                    if (fcRood &&
                        kopHighTime.TotalMilliseconds >= settings.BezettijdKoplus)
                    {
                        //wachtende fietser gedetecteerd
                        sitBwachtendeFietserOpKoplus = true;
                        sitBwachtendeFietserOpKoplusDt = ri.KopUpDt;
                        ri.KopFietserAanwezigMaarNogNietAfgehandeld = false; //resetten
                    }
                    //situatie C checken of er al een fietser op de koplus aanwezig is die bij startgroen nog niet de bezettijd heeft bereikt
                    if (fcStartgroen)
                    {
                        //wachttijd vanaf opkomen koplus
                        sitCwachtendeFietserBijStartgroen = true;
                        sitCwachtendeFietserBijStartgroenDt = ri.KopUpDt;
                        ri.KopFietserAanwezigMaarNogNietAfgehandeld = false; //resetten
                    }
                    //situatie D
                    else if (fcGroen || fcGeel)
                    {
                        sitDfietserBijGroenEnGeel = true;
                        sitDfietserBijGroenEnGeelDt = ri.KopUpDt;
                        ri.KopFietserAanwezigMaarNogNietAfgehandeld = false; //resetten
                    }
                }
                if (kopDown)
                {
                    ri.KopUpDt = new DateTime(); //resetten
                    ri.KopFietserAanwezigMaarNogNietAfgehandeld = false;
                }
              
                // - situatie E en F
                bool sitEFwachtendeFietserBijDrukknop = false;
                DateTime sitEFwachtendeFietserBijDrukknopDt = new DateTime();
                if (drukknopUp && fcRood)
                {
                    //wachtende fietser gedetecteerd
                    sitEFwachtendeFietserBijDrukknop = true;
                    if (ri.KopFietserAanwezigMaarNogNietAfgehandeld)
                    {
                        //situatie E: fietser al aanwezigkoplus actief: wachttijd vanaf opkomen koplus
                        sitEFwachtendeFietserBijDrukknopDt = ri.KopUpDt;
                        ri.KopFietserAanwezigMaarNogNietAfgehandeld = false; //resetten dat er een fietser gezien is op de koplus
                    }
                    else
                    {
                        //situatie F: koplus was niet actief: wachttijd vanaf indrukken drukknop
                        sitEFwachtendeFietserBijDrukknopDt = stateNew.Time;
                    }
                }

                //acties
                bool wachtendVanafSet = false;
                // acties 1
                if (sitAfietserOpVerweglus)
                {
                    WachttijdFietser wtf = new WachttijdFietser() { WaarnemingMoment = stateNew.Time, WaarnemingLocatie = FietsWaarnemingsPunt.verweglus };
                    if (ri.WachtendVanaf.Ticks > 0) ri.TellenMetVerweglus = true;
                    ri.AanwezigeFietsers.Add(wtf);
                }
                if ((sitBwachtendeFietserOpKoplus || sitCwachtendeFietserBijStartgroen || sitEFwachtendeFietserBijDrukknop) &&
                    ri.WachtendVanaf.Ticks == 0)
                {
                    //wachttijd nog niet actief
                    if (sitBwachtendeFietserOpKoplus) ri.WachtendVanaf = sitBwachtendeFietserOpKoplusDt;
                    if (sitCwachtendeFietserBijStartgroen) ri.WachtendVanaf = sitCwachtendeFietserBijStartgroenDt;
                    if (sitEFwachtendeFietserBijDrukknop) ri.WachtendVanaf = sitEFwachtendeFietserBijDrukknopDt;
                    wachtendVanafSet = true;

                    //testen op aanwezige fietsers binnen rijtijd verweglus naar koplus
                    bool toevoegenFietser = false;
                    if (ri.AanwezigeFietsers.Count > 0) //er wordt niet getest of de aanwezige fietser wel van de verweglus afwezig is, maar gezien het algoritme moet dit altijd het geval zijn
                    {
                        //testen aanwezige fietsers op recente aanwezigheid
                        WachttijdFietser wtf = ri.AanwezigeFietsers[ri.AanwezigeFietsers.Count - 1];
                        DateTime momentVanafWanneerVerwegDetectieGeldigIs = ri.WachtendVanaf.AddMilliseconds(-settings.MaxRijtijdVerweglus);
                        if (wtf.WaarnemingMoment < momentVanafWanneerVerwegDetectieGeldigIs)
                        {
                            //geen recente fietser aanwezig: één toevoegen
                            toevoegenFietser = true;
                        }
                        //else: al minimaal één recente fietser aanwezig
                    }
                    else toevoegenFietser = true; //geen fietsers aanwezig: altijd één toevoegen                  

                    if (toevoegenFietser)
                    {
                        WachttijdFietser wtf = new WachttijdFietser() { WaarnemingMoment = ri.WachtendVanaf, WaarnemingLocatie = FietsWaarnemingsPunt.stopstreep };
                        ri.AanwezigeFietsers.Add(wtf);
                    }
                }
                if (sitEFwachtendeFietserBijDrukknop)
                {
                    ri.WachtendMetDrukknop = true;
                }
                // acties 2
                if (fcStartgroen && ri.WachtendVanaf.Ticks == 0)
                {
                    //nog geen fietsers wachtende, maar wel wellicht rijdende fietsers vanaf verweglus: wachtende tijd instellen op startgroen moment, zodat de aankomende fietsers vanaf de verweglus worden meegenomen
                    ri.WachtendVanaf = stateNew.Time;
                    wachtendVanafSet = true;
                }
                // acties 3
                if (wachtendVanafSet)
                {
                    //alleen fietsers onthouden die vanaf ('wacht moment' - 'maxrijtijd vanaf verweglus') gedetecteerd zijn
                    List<WachttijdFietser> nieuweLijst = new List<WachttijdFietser>();
                    DateTime momentVanafWanneerVerwegDetectieGeldigIs = ri.WachtendVanaf.AddMilliseconds(-settings.MaxRijtijdVerweglus);
                    foreach (WachttijdFietser wtf in ri.AanwezigeFietsers)
                    {
                        if (wtf.WaarnemingMoment >= momentVanafWanneerVerwegDetectieGeldigIs)
                        {
                            //fietser behouden
                            nieuweLijst.Add(wtf);
                            if (wtf.WaarnemingLocatie == FietsWaarnemingsPunt.verweglus) ri.TellenMetVerweglus = true; //alleen nog fietsers tellen met verweglus
                        }
                    }
                    ri.AanwezigeFietsers = nieuweLijst;//lijst vervangen
                }
                // acties 4
                if (sitDfietserBijGroenEnGeel)
                {
                    if(!ri.TellenMetVerweglus)
                    {
                        //toevoegen fietser op basis van de koplus
                        WachttijdFietser wtf = new WachttijdFietser() { WaarnemingMoment = stateNew.Time, WaarnemingLocatie = FietsWaarnemingsPunt.stopstreep };
                        ri.AanwezigeFietsers.Add(wtf);
                    }
                }
                // acties 5
                if (fcStartgroen)
                {
                    ri.WachtendMetDrukknop = false;
                    ri.Startgroen = stateNew.Time;
                }

                //resultaat opmaken
                if (fcStartgroen ||  fcGroen || fcStartgeel)
                {
                    //aanwezige fietsers overzetten
                    foreach (WachttijdFietser wtf in ri.AanwezigeFietsers)
                    {
                        //fietser overzetten
                        wtf.StartgroenMoment = ri.Startgroen;
                        TimeSpan wachttijd;
                        if (wtf.WaarnemingLocatie == FietsWaarnemingsPunt.stopstreep)
                        {
                            wachttijd = wtf.StartgroenMoment - wtf.WaarnemingMoment;
                        }
                        else //verweglus
                        {
                            wachttijd = (wtf.StartgroenMoment - wtf.WaarnemingMoment) - TimeSpan.FromMilliseconds(settings.NormRijtijdVerweglus);
                        }
                        if (wachttijd.Ticks < 0) wachttijd = new TimeSpan(0);
                        wtf.Wachttijd = wachttijd;

                        ri.WachttijdFieters.Add(wtf);
                    }
                    ri.AanwezigeFietsers.Clear(); //lijst volledig legen
                }

                if ((fcGeel && !fcStartgeel) || fcStartrood)
                {
                    List<WachttijdFietser> nieuweLijst = new List<WachttijdFietser>();
                    foreach (WachttijdFietser wtf in ri.AanwezigeFietsers)
                    {
                        if(wtf.WaarnemingLocatie == FietsWaarnemingsPunt.stopstreep)
                        {
                            wtf.StartgroenMoment = ri.Startgroen;
                            TimeSpan wachttijd = wtf.StartgroenMoment - wtf.WaarnemingMoment;
                            if (wachttijd.Ticks < 0) wachttijd = new TimeSpan(0);
                            wtf.Wachttijd = wachttijd;

                            ri.WachttijdFieters.Add(wtf); //fietsers gemeten met de koplus gedurende geel wel overnemen
                        }
                        else
                        {
                            nieuweLijst.Add(wtf); //fietsers gemeten vanaf de verweglus gedurende geel niet overnemen
                        }
                    }
                    ri.AanwezigeFietsers = nieuweLijst; //alleen de verweglusfietsers nog onthouden voor de volgende realisatie
                }
            }
        }

        /// <summary>
        /// Returns the start moment of the period in which 'dt' exists
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DateTime startMomentPeriod(DateTime dt)
        {
            return new DateTime((dt.Ticks / intervalMin.Ticks) * intervalMin.Ticks);
        }

        private void errorInData()
        {
            //v-log data bevat fout
            if (_periodeActive)
            {
                //resultaten beschikbaar: deze nog opslaan
                PeriodResultWachttijden pr = new PeriodResultWachttijden();
                pr.DataComplete = false;
                pr.PeriodStart = _startPeriod;
                pr.PeriodLength = _endPeriod - _startPeriod;
                pr.Result = new PeriodResultWachttijdFC[fietsRichtingen.Length];
                for(int i=0;i<fietsRichtingen.Length;i++)
                {
                    WachttijdRichting wtr = fietsRichtingen[i];

                    PeriodResultWachttijdFC prFc = new PeriodResultWachttijdFC
                    {
                        FcName = wtr.Meetpunt.Fc.name,
                        Wachttijden = wtr.WachttijdFieters.ToArray(),
                        LusError = wtr.LusError
                    };

                    pr.Result[i] = prFc;
                }                                    
                periodResults.Add(pr);

                //volgende periode mag pas gestart worden na deze getelde periode
                _nextPeriodMinimalDT = _endPeriod;
                _waitUntilNextPeriodMinimalDT = true;

                _periodeActive = false;
                foreach (WachttijdRichting wtr in fietsRichtingen) wtr.WachttijdFieters.Clear();
            }

            //resetten status info
            _initRun = true;
        }

        private bool allowedToStartPeriodResult()
        {
            if (_waitUntilNextPeriodMinimalDT)
            {
                if (stateNew.Time >= _nextPeriodMinimalDT) return true;
                else return false;
            }
            else
            {
                return true;
            }
        }
    }
}
