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
    public class AnalyseTellenSettings
    {
        public int IntervalMin = 60;
        public int DetFilterMs = 0;

        public override string ToString()
        {
            return string.Format("Interval: {0} min.\r\nFilter minimaal hoog: {1} ms.",
                IntervalMin,
                DetFilterMs);
        }
    }

    public class AnalyseTellenUitvoerSettingsCsv
    {
        public CsvFilter Filter = new CsvFilter();    
        public bool AlleenKoplussen = false;
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Filter.ToString());
            sb.AppendFormat("Alleen koplussen: {0}", AlleenKoplussen ? "aan" : "uit");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Meetpunt waarop het algoritme toegepast kan worden
    /// </summary>
    public class TelMeetpunt
    {
       // public VLogCfgItem Fc = null;
        public VLogCfgItem Lus = null;        
        public WgsLocation Location = new WgsLocation();
        public int Heading = 0;

        public override string ToString()
        {
            return string.Format("Lus {0}\r\nWGS84 {1}\r\nHeading {2}", Lus == null ? "NULL" : Lus.name, Location, Heading);
        }
    }

    public class AnalyseTellen
    {
        public enum State { WaitForTimeRef = 1, WaitForStatus = 2, Active = 3 }

        //analyseert de V-Log detectiesignalen

        //processed berichten         | vereist | optioneel |
        // - tijdreferentie 0x01           x          -
        // - informatie 0x04               -          x
        // - status detectie 0x05          x          -
        // - wijziging detectie 0x06       x          -
        // - status ingangen 0x07          -          x
        // - status fc intern 0x09         -          x
        // - status uitgangen GUS 0x0B     -          x

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

        private AnalyseTellenSettings settings = new AnalyseTellenSettings();
        private TimeSpan intervalMin = new TimeSpan(1, 0, 0);//default 1 hour interval
        public AnalyseTellenSettings Settings
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

        private AnalyseTellenUitvoerSettingsCsv uitvoerSettingsCsv = new AnalyseTellenUitvoerSettingsCsv();
        public AnalyseTellenUitvoerSettingsCsv UitvoerSettingsCsv
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

        private TelMeetpunt[] meetpunten = new TelMeetpunt[0];
        public TelMeetpunt[] Meetpunten
        {
            set
            {
                if (value == null) meetpunten = new TelMeetpunt[0];
                else meetpunten = value;
            }
        }

        private List<PeriodResultTellen> periodResults = new List<PeriodResultTellen>();
        /// <summary>
        /// Meetperiode resultaten
        /// </summary>
        /// <returns></returns>
        public PeriodResultTellen[] PeriodResults
        {
            get
            {
                return periodResults.ToArray();
            }
        }
        
        public string GetCsvResult(DateTime from, DateTime to)
        {
            StringBuilder result = new StringBuilder();

            if (uitvoerSettingsCsv.Filter==null || uitvoerSettingsCsv.Filter.Weekdagen == null || uitvoerSettingsCsv.Filter.Weekdagen.Length != 7) return "ERROR weekdagen";

            if (periodResults.Count == 0)
            {
                //geen resultaten
                return "";
            }
            else
            {
                int aantalDetectoren = periodResults[0].Result.Length;
                bool vlgcfgOk = vlogConfig != null && vlogConfig.Valid && (vlogConfig.Dp.Length == aantalDetectoren);

                if (uitvoerSettingsCsv.AlleenKoplussen && !vlgcfgOk)
                {
                    //kan geen koplussen bepalen, dus ook geen informatie weer geven
                    return "";
                }

                //bepalen welke detectie koplussen betreft
                bool[] koplussen = new bool[0];
                if (uitvoerSettingsCsv.AlleenKoplussen)
                {
                    koplussen = new bool[vlogConfig.Dp.Length];
                    for (int i = 0; i < vlogConfig.Dp.Length; i++)
                    {
                        koplussen[i] = (vlogConfig.Dp[i].type & 0x0101) == 0x0101; //kop + detectielus
                    }
                }

                //header
                result.Append(VLogArchiefDenHaag.VriNameToFullKName(vriName));
                for (int i = 0; i < aantalDetectoren; i++)
                {
                    if (uitvoerSettingsCsv.AlleenKoplussen && koplussen[i] == false) continue;
                    if (vlgcfgOk) result.AppendFormat(";{0}", vlogConfig.Dp[i].name); //indien VLog correct, de naamgeving weergeven
                    else result.AppendFormat(";[{0}]", i);//geen vlog, dan index nr weergeven
                }
                result.AppendLine();

                //data per periode
                int aantalDetRegel = 0;
                DateTime nextPrStart = startMomentPeriod(from);
                foreach (PeriodResultTellen pr in periodResults)
                {
                    aantalDetRegel = pr.Result.Length;
                    if (aantalDetRegel != aantalDetectoren) continue; //indien aantal detectoren is gewijzigd, geen uitvoer meer uitsturen

                    //checken of er gaten in de perioden zitten             
                    while (nextPrStart < pr.PeriodStart) //gat = genereren lege regels (tussen begin datum en eerste pr of tussen pr's in)
                    {
                        //lege regel toevoegen
                        if (uitvoerSettingsCsv.Filter.ValidInFilter(nextPrStart))
                        {
                            result.AppendFormat("{0}", nextPrStart.ToString("yyyy-MM-dd HH:mm"));
                            for (int i = 0; i < aantalDetRegel; i++)
                            {
                                if (uitvoerSettingsCsv.AlleenKoplussen && koplussen[i] == false) continue;
                                result.Append(";"); //gehele periode ongeldige telling
                            }
                            result.AppendLine();
                        }

                        nextPrStart += intervalMin;
                    }
                    nextPrStart = pr.PeriodStart + pr.PeriodLength;

                    if (uitvoerSettingsCsv.Filter.ValidInFilter(pr.PeriodStart))
                    {
                        result.AppendFormat("{0}", pr.PeriodStart.ToString("yyyy-MM-dd HH:mm"));
                        if (pr.DataComplete)
                        {
                            for (int i = 0; i < aantalDetRegel; i++)
                            {
                                if (uitvoerSettingsCsv.AlleenKoplussen && koplussen[i] == false) continue;
                                if (pr.DetError[i]) result.Append(";"); //individuele fout in detector: ongeldige telling
                                else result.AppendFormat(";{0}", pr.Result[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < aantalDetRegel; i++)
                            {
                                if (uitvoerSettingsCsv.AlleenKoplussen && koplussen[i] == false) continue;
                                result.Append(";"); //gehele periode ongeldige telling
                            }
                        }
                        result.AppendLine();
                    }
                }
                if (nextPrStart < to)
                {
                    //gat = genereren lege regels (tussen laatste pr en einddatum)
                    while (nextPrStart < to)
                    {
                        if (uitvoerSettingsCsv.Filter.ValidInFilter(nextPrStart))
                        {
                            //lege regel toevoegen
                            result.AppendFormat("{0}", nextPrStart.ToString("yyyy-MM-dd HH:mm"));
                            for (int i = 0; i < aantalDetRegel; i++)
                            {
                                if (uitvoerSettingsCsv.AlleenKoplussen && koplussen[i] == false) continue;
                                result.Append(";"); //gehele periode ongeldige telling
                            }
                            result.AppendLine();
                        }

                        nextPrStart += intervalMin;
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
                StringBuilder sbDebugInfo = new StringBuilder();
                //header
                result.Header = "location-id;lat;lon;heading;method;quality;period-from;period-to;time-from;time-to;per;bicycle";

                //prefix genereren voor ieder meetpunt
                Dictionary<int, string> prefixen = new Dictionary<int, string>(); //key=vlog index

                //zoeken meetpunt
                foreach (TelMeetpunt meetpunt in meetpunten)
                {
                    int vlogIdx = meetpunt.Lus.index;
                    string prefix = string.Format("{0}-{1};{2};{3};{4};trafficlight-induction;80;",
                                              VLogArchiefDenHaag.VriNameToFullKName(vriName),
                                              meetpunt.Lus.name,
                                              meetpunt.Location.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                                              meetpunt.Location.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                                              meetpunt.Heading);
                    prefixen.Add(vlogIdx, prefix);
                }

                //data
                bool mismatchDataConfig = false;
                int aantalDetInVlogCfg = vlogConfig.Dp.Length;
                foreach (PeriodResultTellen pri in periodResults) //voor iedere periode
                {
                    if (pri.DataComplete)
                    {
                        if (pri.Result.Length == aantalDetInVlogCfg &&
                            pri.DetError.Length == aantalDetInVlogCfg)
                        {
                            foreach (TelMeetpunt meetpunt in meetpunten) //voor ieder meetpunt in de periode
                            {
                                int vlogIdx = meetpunt.Lus.index;
                                if (prefixen.ContainsKey(vlogIdx))
                                {
                                    if (pri.DetError[vlogIdx]) continue; //bij een detectorfout geen resultaat naar DPF 

                                    string record = string.Format("{0}{1};{2};{3};{4};0;{5}",
                                                                  prefixen[vlogIdx],
                                                                  pri.PeriodStart.ToString("yyyy-MM-dd"),
                                                                  pri.PeriodStart.Add(pri.PeriodLength).ToString("yyyy-MM-dd"),
                                                                  pri.PeriodStart.ToString("HH:mm:ss"),
                                                                  pri.PeriodStart.Add(pri.PeriodLength).ToString("HH:mm:ss"),
                                                                  pri.Result[vlogIdx]);
                                    result.Records.Add(record);
                                }
                                else return null; //FC niet aanwezig in de prefix, dus niet aanwezig in de meetpunten
                            }

                            if (mismatchDataConfig)
                            {
                                //eenmalig melden dat er weer een match is
                                mismatchDataConfig = false;

                                sbDebugInfo.AppendFormat("V-Log Data en Configuratie komen overeen vanaf {0}.\r\n", pri.PeriodStart.ToString("yyyy-MM-dd HH:mm"));
                            }
                        }
                        else
                        {
                            //mismatch tussen aantal detectoren in V-Log data en configuratie
                            if (!mismatchDataConfig)
                            {
                                //eenmalig melden dat er een mismatch is
                                mismatchDataConfig = true;

                                sbDebugInfo.AppendFormat("V-Log Data en Configuratie komen niet overeen vanaf {0} aantal detectoren data={1} config={2}.\r\n", pri.PeriodStart.ToString("yyyy-MM-dd HH:mm"), pri.Result.Length, aantalDetInVlogCfg);
                            }
                        }
                    }
                }
                result.DebugInfo = sbDebugInfo.ToString();
                return result;
            }
        }

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

                                _state = State.Active;
                                process = true;

                                checkDataVsConfig = true;
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
                                addError(string.Format("Incorrect message {0}, geen Hex Ascii",vlogBericht));
                            }
                        }
                        break;

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

                            if (okMsg &&
                               okLength &&
                               okDeltaMsg &&
                               okDeltaCorrect)

                            {
                                //OK
                                stateNew.Delta = delta;
                                stateNew.FcExtern = fcs;

                                checkDataVsConfig = true;
                            }
                            else
                            {
                                //error
                                error = true;

                                if (!okMsg) addError(string.Format("Incorrect status internal signalgroup message {0} (format incorrect)", vlogBericht));
                                else if (!okLength) addError(string.Format("Incorrect status internal signalgroup message {0} (amount of signalgroups {1} instead of {2})", vlogBericht, fcs.Length, stateNew.FcExtern.Length));
                                else if (!okDeltaMsg) addError(string.Format("Incorrect status internal signalgroup message {0} (delta incorrect)", vlogBericht));
                                else if (!okDeltaCorrect) addError(string.Format("Incorrect status internal signalgroup message {0} (new delta {1} is lower than last delta {2})", vlogBericht, delta, stateNew.Delta));
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
            }

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
        DateTime[] dtGetHigh = null; //onthouden wanneer de detector hoog is geworden
        int aantalDet = 0;
        //meetperiode gegevens
        DateTime _startPeriod;
        DateTime _endPeriod;
        bool _dataFromStartPeriod = false;
        bool _waitUntilNextPeriodMinimalDT = false; //geeft aan of het analyseren in de huidige periode overgeslagen moet worden
        DateTime _nextPeriodMinimalDT;
        DateTime _firstDtInData;
        //meetperiode resultaten
        int[] _periodResult = null;
        bool[] _periodDetError = null; //fout in periode per individuele detector

        private void analyse()
        {
            //wordt uitgevoerd indien er geldige V-Log informatie aanwezig is in stateNew, na latere aanroepen bevat stateCurrent ook informatie
            //telt per interval periode het aantal afgevallen detectoren dat langer dan de ingesteld duur hoog is

            if (dtGetHigh == null)
            {
                //1e run geldige v-log data
                aantalDet = stateNew.Detectors.Length;
                dtGetHigh = new DateTime[aantalDet];
                _firstDtInData = stateNew.Time;

                //initieel de al hoge detectoren opnemen zodat het voertuig bij de afgaande flank geteld wordt.
                for (int i = 0; i < aantalDet; i++)
                {
                    if (stateNew.Detectors[i] == 1) dtGetHigh[i] = stateNew.Time;
                }

                if (_periodResult == null && allowedToStartPeriodResult())
                {
                    //nog geen resultaten beschikbaar
                    _waitUntilNextPeriodMinimalDT = false;

                    //aanmaken eerste perioderesultaat
                    _periodResult = new int[aantalDet];
                    _periodDetError = new bool[aantalDet];
                    _startPeriod = startMomentPeriod(stateNew.Time);
                    _endPeriod = _startPeriod.Add(intervalMin);
                    _dataFromStartPeriod = (stateNew.Time == _startPeriod); //bepalen of vanaf begin van periode data aanwezig is                    
                }
            }
            else
            {
                //latere runs
                if (_periodResult == null)
                {
                    if (allowedToStartPeriodResult())
                    {
                        //nieuwe periode aangebroken waarin geteld mag worden
                        _waitUntilNextPeriodMinimalDT = false;

                        //aanmaken eerste perioderesultaat
                        _periodResult = new int[aantalDet];
                        _periodDetError = new bool[aantalDet];
                        _startPeriod = startMomentPeriod(stateNew.Time);
                        _endPeriod = _startPeriod.Add(intervalMin);
                        _dataFromStartPeriod = (_firstDtInData <= _startPeriod); //bepalen of vanaf begin van periode data aanwezig is                    
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
                        PeriodResultTellen pr = new PeriodResultTellen();
                        pr.DataComplete = _dataFromStartPeriod;
                        pr.PeriodStart = _startPeriod;
                        pr.PeriodLength = _endPeriod - _startPeriod;
                        pr.Result = _periodResult;
                        pr.DetError = _periodDetError;
                        periodResults.Add(pr);

                        //nieuwe periode aanmaken
                        _periodResult = new int[aantalDet];
                        _periodDetError = new bool[aantalDet];
                        _startPeriod = startMomentPeriod(stateNew.Time);
                        _endPeriod = _startPeriod.Add(intervalMin);
                        _dataFromStartPeriod = true; //hier wordt niet gecontroleerd of de periode aansluitend aan de voorgaande ligt, maar wel vanuit gegaan. Bij de controle van V-Log data wordt namelijk al gecontroleerd op data gaten en dus mogen we ervanuit gaan dat het aaneengesloten data is.                    
                    }
                    else if (stateNew.Time < _startPeriod)
                    {
                        //nieuwe tijd in een eerdere periode: dit wordt niet ondersteund en situatie wordt eerder afgevangen door controle van V-Log data
                        //deze conditie zou dus niet mogen voorkomen

                        throw new Exception("Nieuwe tijd eerder dan oude tijd: wordt niet ondersteund");
                    }
                }

                //het daadwerkelijke tellen
                for (int i = 0; i < aantalDet; i++)
                {
                    bool detUp = (stateCurrent.Detectors[i] == 0 && stateNew.Detectors[i] == 1);
                    bool detDown = (stateCurrent.Detectors[i] == 1 && stateNew.Detectors[i] == 0);
                    bool error = stateNew.Detectors[i] > 1;

                    if (detUp) dtGetHigh[i] = stateNew.Time;
                    if (detDown)
                    {
                        if (dtGetHigh[i].Ticks > 0 && _periodResult != null)
                        {
                            TimeSpan duration = stateNew.Time - dtGetHigh[i];
                            if (duration.TotalMilliseconds >= settings.DetFilterMs)
                            {
                                //tellen
                                _periodResult[i]++;
                            }
                        }
                    }
                    if (error && _periodDetError != null) _periodDetError[i] = true;
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
            if (_periodResult != null)
            {
                //resultaten beschikbaar: deze nog opslaan
                PeriodResultTellen pr = new PeriodResultTellen();
                pr.DataComplete = false;
                pr.PeriodStart = _startPeriod;
                pr.PeriodLength = _endPeriod - _startPeriod;
                pr.Result = _periodResult;
                pr.DetError = _periodDetError;
                periodResults.Add(pr);

                //volgende periode mag pas gestart worden na deze getelde periode
                _nextPeriodMinimalDT = _endPeriod;
                _waitUntilNextPeriodMinimalDT = true;

                _periodResult = null;
                _periodDetError = null;
            }

            dtGetHigh = null; //resetten status info
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
