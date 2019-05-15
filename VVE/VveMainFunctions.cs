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
using System.Runtime.InteropServices;
using System.Diagnostics;
using VVE.VLog;

namespace VVE
{
    public static class VveMainFunctions
    {
        public static VveFuncResult GetAndSaveVLogData(string rootdir,
                                                       string vri,
                                                       DateTime from,
                                                       DateTime to,
                                                       string saveFileName,
                                                       Dictionary<string, byte[]> configuraties,
                                                       bool dagBestanden)
        {
            VveFuncResult result = new VveFuncResult();

            //controle invoer
            if (!Directory.Exists(rootdir) ||
                vri.Trim().Length == 0 ||
                to <= from ||
                saveFileName.Trim().Length == 0 ||
                configuraties == null)
            {
                result.Executed = false;
                result.Errors = "Input incorrect";
                return result;
            }

            string vriKName = VLogArchiefDenHaag.VriNameToFullKName(vri);

            //begin en eind zoeken

            //tijdref bericht dat op 'from' ligt of zo dicht mogelijk hiervoor, wordt als uitgangspunt genomen
            //   maximaal 26 uur terug in de tijd zoeken
            //data inlezen tot tijdref gevonden is die op of zo dicht mogelijk na 'to' ligt
            //   maximaal 26 uur voorin de tijd zoeken naar to t.o.v. de bestandsnaam die bij 'to' hoort
            TimeRefLocation dataBegin = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, from, VLogArchiefDenHaag.findMode.before);
            TimeRefLocation dataEnd = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, to, VLogArchiefDenHaag.findMode.after);

            //DEBUG: aangeven gevonden perioden
            /*    StringBuilder sbRes = new StringBuilder();
                sbRes.AppendFormat("From: {0} To: {1}\r\n\r\n", from.ToString("yyyy-MM-dd HH:mm:ss.f"), to.ToString("yyyy-MM-dd HH:mm:ss.f"));
                sbRes.AppendFormat("Begin:\r\n{0}\r\n", trlFindResFrom.ToString());
                sbRes.AppendFormat("End:\r\n{0}\r\n", trlFindResTo.ToString());
                MessageBox.Show(sbRes.ToString());
    */

            //verzamelen vlog data
            CollectVLogData cvd = new CollectVLogData();
            FileCollection fc = cvd.Collect(rootdir, vri, dataBegin, dataEnd, dagBestanden ? CollectVLogData.FileStructure.DagBestanden : CollectVLogData.FileStructure.Eenbestand);

            //toevoegen configuratiebestand
            try
            {
                SingleFile sfVlgcfg = new SingleFile();

                sfVlgcfg.Data = configuraties[vri];
                sfVlgcfg.FileName = vriKName + ".vlc";
                fc.Files.Add(sfVlgcfg);
            }
            catch (Exception ex)
            {
                result.Executed = false;
                result.Errors = "Fout bij toevoegen vlgcfg bestand: " + ex.Message;
                return result;
            }

            //opslaan in zip file
            try
            {
                using (var fileStream = new FileStream(saveFileName, FileMode.Create))
                {
                    using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                    {
                        foreach (SingleFile sf in fc.Files)
                        {
                            ZipArchiveEntry zfile = archive.CreateEntry(sf.FileName, CompressionLevel.Optimal);
                            using (var entryStream = zfile.Open())
                            {
                                entryStream.Write(sf.Data, 0, sf.Data.Length);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Executed = false;
                result.Errors = "Fout bij aanmaken ZIP: " + ex.Message;
                return result;
            }

            result.Executed = true;
            return result;
        }

        public static VveFuncResult SaveVLogDataForViewer(string rootdir,
                                                            string vri,
                                                            DateTime from,
                                                            DateTime to,
                                                            Dictionary<string, byte[]> configuraties)
        {
            VveFuncResult result = new VveFuncResult();

            //controle invoer
            if (!Directory.Exists(rootdir) ||
                vri.Trim().Length == 0 ||
                to <= from ||
                configuraties == null)
            {
                result.Executed = false;
                result.Errors = "Input incorrect";
                return result;
            }

            string vriNameLong = VLogArchiefDenHaag.VriNameToFullNumber(vri);

            //begin en eind zoeken

            //tijdref bericht dat op 'from' ligt of zo dicht mogelijk hiervoor, wordt als uitgangspunt genomen
            //   maximaal 26 uur terug in de tijd zoeken
            //data inlezen tot tijdref gevonden is die op of zo dicht mogelijk na 'to' ligt
            //   maximaal 26 uur voorin de tijd zoeken naar to t.o.v. de bestandsnaam die bij 'to' hoort
            TimeRefLocation dataBegin = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, from, VLogArchiefDenHaag.findMode.before);
            TimeRefLocation dataEnd = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, to, VLogArchiefDenHaag.findMode.after);

            //DEBUG: aangeven gevonden perioden
            /*    StringBuilder sbRes = new StringBuilder();
                sbRes.AppendFormat("From: {0} To: {1}\r\n\r\n", from.ToString("yyyy-MM-dd HH:mm:ss.f"), to.ToString("yyyy-MM-dd HH:mm:ss.f"));
                sbRes.AppendFormat("Begin:\r\n{0}\r\n", trlFindResFrom.ToString());
                sbRes.AppendFormat("End:\r\n{0}\r\n", trlFindResTo.ToString());
                MessageBox.Show(sbRes.ToString());
    */

            //verzamelen vlog data
            CollectVLogData cvd = new CollectVLogData();
            FileCollection fc = cvd.Collect(rootdir, vri, dataBegin, dataEnd, CollectVLogData.FileStructure.Eenbestand);

            //toevoegen configuratiebestand
            try
            {
                SingleFile sfVlgcfg = new SingleFile();

                sfVlgcfg.Data = configuraties[vri];
                sfVlgcfg.FileName = "K" + vriNameLong + ".vlc";
                fc.Files.Add(sfVlgcfg);
            }
            catch (Exception ex)
            {
                result.Executed = false;
                result.Errors = "Fout bij toevoegen vlgcfg bestand: " + ex.Message;
                return result;
            }

            //opslaan in temp dir
            string tempDir = Path.GetTempPath();
            string vlogFile = "";
            try
            {
                foreach (SingleFile sf in fc.Files)
                {
                    string path = tempDir + sf.FileName;
                    File.WriteAllBytes(path, sf.Data);
                    if (vlogFile == "" && path.Contains(".vlg")) vlogFile = path;
                }

                if (vlogFile != "")
                {
                    System.Diagnostics.Process.Start(vlogFile);
                }
                else
                {
                    result.Executed = false;
                    result.Errors = "Geen V-Log data gevonden";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Executed = false;
                result.Errors = "Fout bij opslaan data: " + ex.Message;
                return result;
            }

            result.Executed = true;
            return result;
        }

        public static VveFuncResult PerformCountAnalyseCsv(string rootdir,
                                                           string vri,
                                                           DateTime from,
                                                           DateTime to,
                                                           string saveFileName,
                                                           Dictionary<string, byte[]> configuraties,
                                                           AnalyseTellenSettings settings,
                                                           AnalyseTellenUitvoerSettingsCsv uitvoerSettingsCsv)
        {
            VveFuncResult result = new VveFuncResult();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //controle invoer
            if (!Directory.Exists(rootdir) ||
                vri.Trim().Length == 0 ||
                to <= from ||
                configuraties == null ||
                settings == null ||
                uitvoerSettingsCsv == null ||
                uitvoerSettingsCsv.Filter == null||
                uitvoerSettingsCsv.Filter.Weekdagen == null ||
                uitvoerSettingsCsv.Filter.Weekdagen.Length != 7)
            {
                result.Executed = false;
                result.Errors = "Input incorrect";
                return result;
            }

            //begin en eind zoeken
            TimeRefLocation dataBegin = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, from, VLogArchiefDenHaag.findMode.before);
            TimeRefLocation dataEnd = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, to, VLogArchiefDenHaag.findMode.after);

            //configuratie inlezen
            VLogCfg vc = new VLogCfg();
            try
            {
                if (!configuraties.ContainsKey(vri))
                {
                    result.Executed = false;
                    result.Errors = "Vlgcfg niet aanwezig";
                    return result;
                }
                byte[] dataVlogCfg = configuraties[vri];
                if (!vc.ReadConfig(dataVlogCfg))
                {
                    //algoritme mag doorgaan met geen vlog configuratie
                    vc = null;
                    /*result.ExecutionOk = false;
                    result.Errors = string.Format("Vlgcfg bestand van VRI {0} kon niet ingelezen worden: ", vri);
                    return result;*/
                }
            }
            catch (Exception ex)
            {
                result.Executed = false;
                result.Errors = "Fout bij lezen vlgcfg bestand: " + ex.Message;
                return result;
            }

            //analyse init
            AnalyseTellen analyse = new AnalyseTellen();
            analyse.VLogConfig = vc;
            analyse.Settings = settings;
            analyse.UitvoerSettingsCsv = uitvoerSettingsCsv;
            analyse.VriName = vri;

            //v-log data doorlopen en analyse uitvoeren
            VLogArchiefDenHaag.ProcesData(rootdir, vri, dataBegin, dataEnd, VLogArchiefDenHaag.procesMode.includeToTimeRef, analyse.Process);
            analyse.End();

            //resultaat ophalen
            try
            {
                result.Results = analyse.GetCsvResult(from, to);
                result.ResultsSet = true;
            }
            catch (Exception ex)
            {
                result.Executed = false;
                result.Errors = string.Format("Fout bij samenstellen resultaat: {0}", ex.Message);
                return result;
            }

            //opslaan naar bestand indien opgegeven
            if (saveFileName != null && saveFileName != "")
            {
                try
                {
                    File.WriteAllText(saveFileName, result.Results);
                }
                catch (Exception ex)
                {
                    result.Executed = false;
                    result.Errors = string.Format("Fout bij wegschrijven bestand {0}: {1}", saveFileName, ex.Message);
                    return result;
                }
            }

            //informatie van analyse verzamelen
            StringBuilder info = new StringBuilder();

            //input info
            info.AppendFormat("VRI: {0}\r\nVan: {1}\r\nTot: {2}\r\n\r\nSettings analyse:\r\n{3}\r\n\r\nSettings CSV uitvoer:\r\n{4}", vri, from.ToString("yyyy-MM-dd HH:mm:ss.f"), to.ToString("yyyy-MM-dd HH:mm:ss.f"), settings, uitvoerSettingsCsv);
            info.AppendLine();
            string analyseErrors = analyse.Errors;
            if (analyseErrors.Length > 0)
            {
                info.AppendLine();
                info.AppendLine("Analyse errors:");
                info.AppendLine(analyseErrors);
            }
            string analyseVlcAllMatchInfo = analyse.VlcAllMatchInfo;
            if (analyseVlcAllMatchInfo.Length > 0)
            {
                info.AppendLine();
                info.AppendLine("Data-VLC match info:");
                info.AppendLine(analyseVlcAllMatchInfo);
            }

            result.Info = info.ToString();

            result.Executed = true;

            sw.Stop();
            result.ProcessTime = sw.Elapsed;

            return result;
        }

        public static VveFuncResult PerformCountAnalyseDpf(string rootdir,
                                                           string kmlConfigDir,
                                                           DateTime from,
                                                           DateTime to,
                                                           Dictionary<string, byte[]> configuraties,
                                                           AnalyseTellenSettings settings,
                                                           string vriSelection = "")
        {
            VveFuncResult result = new VveFuncResult();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //controle invoer
            if (!Directory.Exists(rootdir) ||
                !Directory.Exists(kmlConfigDir) ||
                to <= from ||
                configuraties == null ||
                settings == null ||
                vriSelection == null)
            {
                result.Executed = false;
                result.Errors = "Input incorrect";
                return result;
            }

            StringBuilder info = new StringBuilder();
            StringBuilder infoDataVlcMatch = new StringBuilder();

            //input info
            info.AppendFormat("VRI: {0}\r\nVan: {1}\r\nTot: {2}\r\n\r\nSettings analyse:\r\n{3}", vriSelection == "" ? "alle" : vriSelection, from.ToString("yyyy-MM-dd HH:mm:ss.f"), to.ToString("yyyy-MM-dd HH:mm:ss.f"), settings);
            info.AppendLine();
            info.AppendLine();

            Kml kml = new Kml();
            kml.LoadLastFile(kmlConfigDir);
            if (!kml.LoadLastFile(kmlConfigDir))
            {
                //error
                result.Executed = false;
                result.Errors = String.Format("Fout bij lezen KML configuratiebestand {0}:\r\n{1}\r\n", kml.FileName, kml.ErrorText);
                return result;
            }
            else if (kml.FietsVerweglussen.Length == 0 && kml.FietsTellussen.Length == 0)
            {
                //error
                result.Executed = false;
                result.Errors = String.Format("Geen KML configuratiedata aanwezig.\r\n");
                return result;
            }
            else //valid en data
            {
                info.AppendFormat("Correct ingelezen KML configuratiebestand {0}.\r\n", kml.FileName);
                info.AppendFormat("");
            }

            //omzetten KML informatie naar GeoData voor VVE
            GeoData geoData = new GeoData(kml);

            //alleen vri's met verweg- en tellussen selecteren
            List<GeoDataVri> vrisMetVerwegOfTellussen = new List<GeoDataVri>();
            foreach (KeyValuePair<string, GeoDataVri> kvp in geoData.Vris)
            {
                GeoDataVri gdVri = kvp.Value;

                //alleen de geselecteerde vri indien opgegeven
                if (vriSelection != "" && gdVri.Name != vriSelection)
                {
                    continue;
                }

                if (gdVri.FietsVerweglussen.Count > 0 || gdVri.FietsTellussen.Count > 0) vrisMetVerwegOfTellussen.Add(gdVri);
            }

            //analyse uitvoeren voor iedere VRI met meetpunten
            foreach (GeoDataVri v in vrisMetVerwegOfTellussen)
            {
                string vri = v.Name;               

                //begin- en eindpunt zoeken in de data
                TimeRefLocation dataBegin = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, from, VLogArchiefDenHaag.findMode.before);
                TimeRefLocation dataEnd = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, to, VLogArchiefDenHaag.findMode.after);

                //configuratie inlezen
                VLogCfg vc = new VLogCfg();
                try
                {
                    //check of configuratie wel aanwezig is
                    if (configuraties.ContainsKey(vri))
                    {
                        byte[] dataVlogCfg = configuraties[vri];

                        //check of het configuratiebestand correct is
                        if (!vc.ReadConfig(dataVlogCfg))
                        {
                            result.Executed = false;
                            info.AppendLine(String.Format("VRI {0}: VLog configuratie: formaat incorrect. {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), vc.ErrorText));
                            continue;
                        }
                    }
                    else
                    {
                        result.Executed = false;
                        info.AppendLine(String.Format("VRI {0}: VLog configuratie: niet aanwezig", VLogArchiefDenHaag.VriNameToFullKName(vri)));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    //error
                    result.Executed = false;
                    info.AppendLine(String.Format("VRI {0}: VLog configuratie: fout bij inlezen: {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), ex.Message));
                    continue;
                }

                //bepalen meetpunten voor tellen
                ResultMeetpuntenTellen meetpunten= bepaalTelMeetpunten(vc, v);

                if (meetpunten.ErrorText.Length > 0) info.AppendFormat("VRI {0}: Fouten bij bepalen meetpunten: {1}\r\n", VLogArchiefDenHaag.VriNameToFullKName(vri), meetpunten.ErrorText);
                if (meetpunten.Meetpunten.Length == 0) info.AppendLine(String.Format("VRI {0}: Overgeslagen: geen meetpunten", VLogArchiefDenHaag.VriNameToFullKName(vri)));
                if (meetpunten.Meetpunten.Length == 0 || !meetpunten.Succes) continue; //geen meetpunten, dus deze VRI overslaan
                
                AnalyseTellen analyse = new AnalyseTellen();
                analyse.Settings = settings;
                analyse.VriName = vri;
                analyse.Meetpunten = meetpunten.Meetpunten;
                analyse.VLogConfig = vc;

                //v-log data doorlopen en analyse uitvoeren
                VLogArchiefDenHaag.ProcesData(rootdir, vri, dataBegin, dataEnd, VLogArchiefDenHaag.procesMode.includeToTimeRef, analyse.Process);
                analyse.End();

                //DPF resultaten ophalen
                DpfData dpfResult = analyse.DpfResult;
                if (dpfResult == null)
                {
                    //error
                    info.AppendLine(String.Format("VRI {0}: Generatie DPF fout", VLogArchiefDenHaag.VriNameToFullKName(vri)));
                    continue;
                }
                if (dpfResult.DebugInfo.Length > 0) info.AppendFormat("VRI {0}: Generatie DPF fout: {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), dpfResult.DebugInfo);
                dpfResult.Name = String.Format("{0} TellingFiets {1} tot {2} DPF", VLogArchiefDenHaag.VriNameToFullKName(vri), from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));
                result.DpfResults.Add(dpfResult);
                
                //debug info van analyse overnemen
                string analyseErrors = analyse.Errors;
                if (analyseErrors.Length > 0)
                {
                    info.AppendLine(String.Format("VRI {0}: Analyse uitgevoerd met errors:\r\n{1}", VLogArchiefDenHaag.VriNameToFullKName(vri), analyseErrors));
                }
                else
                {
                    info.AppendLine(String.Format("VRI {0}: Analyse uitgevoerd zonder errors", VLogArchiefDenHaag.VriNameToFullKName(vri)));
                }

                string analyseVlcNoMatchInfo = analyse.VlcNoMatchInfo;
                if (analyseVlcNoMatchInfo.Length > 0)
                {
                    infoDataVlcMatch.AppendFormat("VRI {0}: {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), analyseVlcNoMatchInfo);
                }
            }

            result.Info = info.ToString();
            if(infoDataVlcMatch.Length>0)
            {
                result.Info += string.Format("\r\n{0}", infoDataVlcMatch);
            }

            result.Executed = true;

            sw.Stop();
            result.ProcessTime = sw.Elapsed;

            return result;
        }


        public static VveFuncResult PerformWaitAnalyseCsvFile(string rootdir,
                                                          string vri,
                                                          DateTime from,
                                                          DateTime to,
                                                          string saveFileName,
                                                          Dictionary<string, byte[]> configuraties,
                                                          AnalyseWachttijdSettings settings,
                                                          AnalyseWachttijdUitvoerSettingsCsv uitvoerSettingsCsv)
        {
            VveFuncResult result = new VveFuncResult();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //controle invoer
            if (!Directory.Exists(rootdir) ||
                vri.Trim().Length == 0 ||
                to <= from ||
                configuraties == null ||
                settings == null ||
                uitvoerSettingsCsv == null ||
                uitvoerSettingsCsv.Filter == null ||
                uitvoerSettingsCsv.Filter.Weekdagen == null ||
                uitvoerSettingsCsv.Filter.Weekdagen.Length != 7l)
            {
                result.Executed = false;
                result.Errors = "Input incorrect";
                return result;
            }

            //begin en eind zoeken
            TimeRefLocation dataBegin = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, from, VLogArchiefDenHaag.findMode.before);
            TimeRefLocation dataEnd = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, to, VLogArchiefDenHaag.findMode.after);

            //configuratie inlezen
            VLogCfg vc = new VLogCfg();
            try
            {
                if (!configuraties.ContainsKey(vri))
                {
                    result.Executed = false;
                    result.Errors = "Vlgcfg niet aanwezig";
                    return result;
                }
                byte[] dataVlogCfg = configuraties[vri];
                if (!vc.ReadConfig(dataVlogCfg))
                {
                    result.Executed = false;
                    result.Errors = string.Format("Vlgcfg bestand van VRI {0} kon niet ingelezen worden: ", vri);
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Executed = false;
                result.Errors = "Fout bij lezen vlgcfg bestand: " + ex.Message;
                return result;
            }

            ResultMeetpuntenWachttijdFiets resMeetpunten = bepaalWachttijdFietsMeetpunten(vc);
            if (!resMeetpunten.Succes)
            {
                result.Executed = false;
                result.Errors = resMeetpunten.ErrorText;
                return result;
            }
            if (resMeetpunten.Meetpunten.Length == 0)
            {
                result.Executed = false;
                result.Errors = "Geen meetpunten aanwezig voor de VRI";
                return result;
            }

            //analyse init
            AnalyseWachttijden analyse = new AnalyseWachttijden();
            analyse.VLogConfig = vc;
            analyse.Settings = settings;
            analyse.UitvoerSettingsCsv = uitvoerSettingsCsv;
            analyse.Meetpunten = resMeetpunten.Meetpunten;
            analyse.VriName = vri;

            //v-log data doorlopen en analyse uitvoeren
            VLogArchiefDenHaag.ProcesData(rootdir, vri, dataBegin, dataEnd, VLogArchiefDenHaag.procesMode.includeToTimeRef, analyse.Process);
            analyse.End();

            //resultaat ophalen
            try
            {
                result.Results = analyse.GetCsvResult(from, to);
                result.ResultsSet = true;
            }
            catch (Exception ex)
            {
                result.Executed = false;
                result.Errors = string.Format("Fout bij samenstellen resultaat: {0}", ex.Message);
                return result;
            }

            //opslaan naar bestand indien opgegeven
            if (saveFileName != null && saveFileName != "")
            {
                try
                {
                    File.WriteAllText(saveFileName, result.Results);
                }
                catch (Exception ex)
                {
                    result.Executed = false;
                    result.Errors = string.Format("Fout bij wegschrijven bestand {0}: {1}", saveFileName, ex.Message);
                    return result;
                }
            }

            //informatie van analyse verzamelen
            StringBuilder info = new StringBuilder();

            //input info
            info.AppendFormat("VRI: {0}\r\nVan: {1}\r\nTot: {2}\r\n\r\nSettings analyse:\r\n{3}\r\n\r\nSettings CSV uitvoer:\r\n{4}", vri, from.ToString("yyyy-MM-dd HH:mm:ss.f"), to.ToString("yyyy-MM-dd HH:mm:ss.f"), settings, uitvoerSettingsCsv);
            info.AppendLine();
            string analyseErrors = analyse.Errors;
            if (analyseErrors.Length > 0)
            {
                info.AppendLine();
                info.AppendLine("Analyse errors:");
                info.AppendLine(analyseErrors);
            }
            string analyseVlcAllMatchInfo = analyse.VlcAllMatchInfo;
            if (analyseVlcAllMatchInfo.Length > 0)
            {
                info.AppendLine();
                info.AppendLine("Data-VLC match info:");
                info.AppendLine(analyseVlcAllMatchInfo);
            }

            result.Info = info.ToString();

            result.Executed = true;

            sw.Stop();
            result.ProcessTime = sw.Elapsed;

            return result;
        }

        public static VveFuncResult PerformWaitAnalyseDpf(string rootdir,
                                                          string kmlConfigDir,
                                                          DateTime from,
                                                          DateTime to,
                                                          Dictionary<string, byte[]> configuraties,
                                                          AnalyseWachttijdSettings settings,
                                                          string vriSelection="")
        {
            VveFuncResult result = new VveFuncResult();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //controle invoer
            if (!Directory.Exists(rootdir) ||
                !Directory.Exists(kmlConfigDir) ||
                to <= from ||
                configuraties == null ||
                settings == null ||
                vriSelection == null)
            {
                result.Executed = false;
                result.Errors = "Input incorrect";
                return result;
            }

            StringBuilder info = new StringBuilder();
            StringBuilder infoDataVlcMatch = new StringBuilder();

            //input info
            info.AppendFormat("VRI: {0}\r\nVan: {1}\r\nTot: {2}\r\n\r\nSettings analyse:\r\n{3}", vriSelection==""?"alle":vriSelection, from.ToString("yyyy-MM-dd HH:mm:ss.f"), to.ToString("yyyy-MM-dd HH:mm:ss.f"), settings);
            info.AppendLine();
            info.AppendLine();

            Kml kml = new Kml();
            if (!kml.LoadLastFile(kmlConfigDir))
            {
                //error
                result.Executed = false;
                result.Errors = String.Format("Fout bij lezen KML configuratiebestand {0}:\r\n{1}\r\n", kml.FileName, kml.ErrorText);
                return result;
            }
            else if (kml.FietsVerweglussen.Length == 0)
            {
                //error
                result.Executed = false;
                result.Errors = String.Format("Geen KML verweglussen aanwezig.\r\n");
                return result;
            }
            else //valid en data
            {
                info.AppendLine(String.Format("Correct ingelezen KML configuratiebestand {0}.", kml.FileName));
                info.AppendFormat("");
            }

            //omzetten KML informatie naar GeoData voor VVE
            GeoData geoData = new GeoData(kml);

            //lijst maken met VRI's en geschikte meetpunten
            info.AppendLine();
            info.AppendLine("Bepalen meetpunten:");
            StringBuilder sbMeetpuntenDebug = new StringBuilder();
            int statsMeetpuntenAantalTotaal = 0;
            int statsMeetpuntenAantalMetDrukknop = 0;
            int statsMeetpuntenAantalZonderDrukknop = 0;
            int statsMeetpuntenAantalZonderLocatie = 0;
            Dictionary<string, List<WachttijdFietsMeetpunt>> meetpuntenMetGeoData = new Dictionary<string, List<WachttijdFietsMeetpunt>>(); //key=VRI name         
            foreach (KeyValuePair<string, byte[]> kvp in configuraties)
            {
                string vri = kvp.Key;

                //alleen de geselecteerde vri indien opgegeven
                if (vriSelection!="" && vri != vriSelection)
                {
                    continue;
                }

                //configuratie inlezen
                VLogCfg vc = new VLogCfg();
                try
                {
                    byte[] dataVlogCfg = kvp.Value;
                    if (!vc.ReadConfig(dataVlogCfg))
                    {
                        info.AppendLine(String.Format("VRI {0}: VLog configuratie: formaat incorrect. {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), vc.ErrorText));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    info.AppendLine(String.Format("VRI {0}: VLog configuratie: fout bij inlezen: {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), ex.Message));
                    continue;
                }

                //meetpunten bepalen op basis van vlog config
                ResultMeetpuntenWachttijdFiets resMeetpunten = bepaalWachttijdFietsMeetpunten(vc);
                if (!resMeetpunten.Succes)
                {
                    info.AppendLine(String.Format("VRI {0}: Bepalen meetpunten met vlog configuratie fout: {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), resMeetpunten.ErrorText));
                    continue;
                }
                if (resMeetpunten.Meetpunten.Length == 0)
                {
                    sbMeetpuntenDebug.AppendLine(String.Format("VRI {0}: Overgeslagen: geen meetpunten", VLogArchiefDenHaag.VriNameToFullKName(vri)));
                    continue;
                }
                statsMeetpuntenAantalTotaal += resMeetpunten.Aantal;
                statsMeetpuntenAantalMetDrukknop += resMeetpunten.AantalKopVerwegDruk;
                statsMeetpuntenAantalZonderDrukknop += resMeetpunten.AantalKopVerwegGeenDruk;

                //zoeken naar bijbehorende locatie stopstreep
                GeoDataVri gdv = null;
                if (geoData.Vris.ContainsKey(vri)) gdv = geoData.Vris[vri];
                foreach (WachttijdFietsMeetpunt wfm in resMeetpunten.Meetpunten)
                {
                    bool found = false;

                    if (gdv != null)
                    {
                        //op basis van verweglus
                      /*  foreach (GeoDataLus verweglus in gdv.FietsVerweglussen)
                        {
                            if (verweglus.Name == wfm.Verweglus.name)
                            {
                                //verweglus gevonden
                                wfm.Location = verweglus.LocationStopstreep;
                                wfm.Heading = verweglus.Heading;
                                if (!meetpuntenMetGeoData.ContainsKey(vri)) meetpuntenMetGeoData.Add(vri, new List<WachttijdFietsMeetpunt>());
                                meetpuntenMetGeoData[vri].Add(wfm);

                                found = true;
                                continue;
                            }
                        }*/

                        //op basis van de koplus
                        foreach (GeoDataLus koplus in gdv.FietsKoplussen)
                        {
                            if (koplus.Name == wfm.Koplus.name)
                            {
                                //koplus gevonden
                                wfm.Location = koplus.LocationStopstreep;
                                wfm.Heading = koplus.Heading;
                                if (!meetpuntenMetGeoData.ContainsKey(vri)) meetpuntenMetGeoData.Add(vri, new List<WachttijdFietsMeetpunt>());
                                meetpuntenMetGeoData[vri].Add(wfm);

                                found = true;
                                continue;
                            }
                        }
                    }
                    if (!found)
                    {
                        sbMeetpuntenDebug.AppendLine(String.Format("VRI {0}: Geen locatie bekend van meetpunt:\r\n{1}", VLogArchiefDenHaag.VriNameToFullKName(vri), "   "+wfm.ToString().Replace("\n","\n   ")));
                        statsMeetpuntenAantalZonderLocatie++;
                        continue;
                    }
                }

                if(vriSelection!= "")
                {
                    //geselecteerde VRI gevonden: niet verder zoeken
                    break;
                }
            }

            //statistieken gevonden meetpunten
            info.AppendLine(String.Format("In totaal {0} meetpunten gevonden, waarvan {1} met drukknop en {2} zonder drukknop.", statsMeetpuntenAantalTotaal, statsMeetpuntenAantalMetDrukknop, statsMeetpuntenAantalZonderDrukknop));
            info.AppendLine(String.Format("Van {0} meetpunten is geen locatie gevonden.", statsMeetpuntenAantalZonderLocatie));
            info.Append(sbMeetpuntenDebug);

            //analyse uitvoeren voor iedere VRI met meetpunten
            info.AppendLine();
            info.AppendLine("Analyse:");
            foreach (KeyValuePair<string, List<WachttijdFietsMeetpunt>> kvp in meetpuntenMetGeoData)
            {
                string vri = kvp.Key;
                List<WachttijdFietsMeetpunt> meetpunten = kvp.Value;

                //begin- en eindpunt zoeken in de data
                TimeRefLocation dataBegin = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, from, VLogArchiefDenHaag.findMode.before);
                TimeRefLocation dataEnd = VLogArchiefDenHaag.FindTimeRefLocation(rootdir, vri, to, VLogArchiefDenHaag.findMode.after);

                //configuratie inlezen
                VLogCfg vc = new VLogCfg();
                try
                {
                    byte[] dataVlogCfg = configuraties[vri];
                    if (!vc.ReadConfig(dataVlogCfg))
                    {
                        info.AppendLine(String.Format("VRI {0}: VLog configuratie 2e keer inlezen: formaat incorrect. {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), vc.ErrorText));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    info.AppendLine(String.Format("VRI {0}: VLog configuratie 2e keer inlezen: fout bij inlezen: {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), ex.Message));
                    continue;
                }

                //analyse instellen
                AnalyseWachttijden analyse = new AnalyseWachttijden();
                analyse.VLogConfig = vc;
                analyse.Settings = settings;
                analyse.Meetpunten = meetpunten.ToArray();
                analyse.VriName = vri;

                //v-log data doorlopen en analyse uitvoeren
                VLogArchiefDenHaag.ProcesData(rootdir, vri, dataBegin, dataEnd, VLogArchiefDenHaag.procesMode.includeToTimeRef, analyse.Process);
                analyse.End();

                //DPF resultaten ophalen
                DpfData dpfResult = analyse.DpfResult;
                if (dpfResult==null)
                {
                    //error
                    info.AppendLine(String.Format("VRI {0}: Generatie DPF fout", VLogArchiefDenHaag.VriNameToFullKName(vri)));
                    continue;
                }
                if (dpfResult.DebugInfo.Length > 0) info.AppendFormat("VRI {0}: Generatie DPF fout: {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), dpfResult.DebugInfo);
                dpfResult.Name = String.Format("{0} WachttijdFiets {1} tot {2} DPF", VLogArchiefDenHaag.VriNameToFullKName(vri), from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));
                result.DpfResults.Add(dpfResult);

                //debug info van analyse overnemen
                string analyseErrors = analyse.Errors;
                if (analyseErrors.Length > 0)
                {
                    info.AppendLine(String.Format("VRI {0}: Analyse uitgevoerd met errors:\r\n{1}", VLogArchiefDenHaag.VriNameToFullKName(vri), analyseErrors));                    
                }
                else
                {
                    info.AppendLine(String.Format("VRI {0}: Analyse uitgevoerd zonder errors", VLogArchiefDenHaag.VriNameToFullKName(vri)));
                }

                string analyseVlcNoMatchInfo = analyse.VlcNoMatchInfo;
                if (analyseVlcNoMatchInfo.Length > 0)
                {
                    infoDataVlcMatch.AppendFormat("VRI {0}: {1}", VLogArchiefDenHaag.VriNameToFullKName(vri), analyseVlcNoMatchInfo);
                }
            }

            result.Info = info.ToString();
            if (infoDataVlcMatch.Length > 0)
            {
                result.Info += string.Format("\r\n{0}", infoDataVlcMatch);
            }

            result.Executed = true;

            sw.Stop();
            result.ProcessTime = sw.Elapsed;

            return result;
        }

        public static VveFuncResult PerformSaveDpfToFile(VveFuncResult result,
                                                         string resultdir)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //opslaan naar bestanden
            foreach (DpfData dd in result.DpfResults)
            {
                string dpfFile = String.Format("{0}\\{1}.csv", resultdir, dd.Name);
                try
                {
                    File.WriteAllText(dpfFile, dd.AllRecords());
                }
                catch (Exception ex)
                {
                    result.Info += String.Format("Fout bij wegschrijven DPF resultaat naar bestand: {0}", ex.Message);
                    continue;
                }
            }

            sw.Stop();
            result.ProcessTime += sw.Elapsed;

            return result;
        }

        public static VveFuncResult PerformPostDpfToHttp(VveFuncResult result, PostSettings ps)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //verzenden met http POST

            //dpfData verzamelen
            DpfData allDpf = new DpfData();
            bool first = true;
            foreach (DpfData dd in result.DpfResults)
            {
                if (first)
                {
                    allDpf.Header = dd.Header;
                    first = false;
                }
                allDpf.Add(dd);
            }

            //dpfData verzenden
            HttpPostDpfData poster = new HttpPostDpfData();
            string postErrorDir = "PostLog";
            if (!Directory.Exists(postErrorDir)) Directory.CreateDirectory(postErrorDir);
            poster.PostData(allDpf, ps, postErrorDir);
            result.Info += poster.DebugLog;
            
            sw.Stop();
            result.ProcessTime += sw.Elapsed;

            return result;
        }

        /// <summary>
        /// Op basis van VLog config bepalen welke meetpunten geschikt zijn voor het wachttijd algoritme.
        /// Geldige meetpunten zijn:
        /// - aanwezigheid van koplus + verweglus + drukknop bij een fiets signaalgroep
        /// - aanwezigheid van koplus + verweglus bij een fiets signaalgroep
        /// </summary>
        /// <param name="vc"></param>
        /// <returns></returns>
        private static ResultMeetpuntenWachttijdFiets bepaalWachttijdFietsMeetpunten(VLogCfg vc)
        {
            ResultMeetpuntenWachttijdFiets result = new ResultMeetpuntenWachttijdFiets();
            
            //configuratie doornemen en bepalen meetpunten
            Dictionary<String, WachttijdFietsMeetpunt> meetpuntList = new Dictionary<string, WachttijdFietsMeetpunt>(); //key=FC name
            for (int i = 0; i < vc.Fc.Length; i++)
            {
                if (vc.Fc[i].type == 4)
                {
                    //fiets
                    if (!meetpuntList.ContainsKey(vc.Fc[i].name))
                    {
                        WachttijdFietsMeetpunt wfd = new WachttijdFietsMeetpunt();
                        wfd.Fc = vc.Fc[i];

                        //koplus zoeken
                        for (int j = 0; j < vc.Dp.Length; j++)
                        {
                            if (vc.Dp[j].name == wfd.Fc.name + "1" ||
                               vc.Dp[j].name == wfd.Fc.name + "_1")
                            {
                                wfd.Koplus = vc.Dp[j];
                                break;
                            }
                        }

                        //verweglus zoeken
                        for (int j = 0; j < vc.Dp.Length; j++)
                        {
                            if (vc.Dp[j].name == wfd.Fc.name + "2" ||
                               vc.Dp[j].name == wfd.Fc.name + "_2")
                            {
                                wfd.Verweglus = vc.Dp[j];
                                break;
                            }
                        }

                        //drukknop zoeken
                        for (int j = 0; j < vc.Dp.Length; j++)
                        {
                            if (vc.Dp[j].name == "K" + wfd.Fc.name + "1" ||
                                vc.Dp[j].name == "K" + wfd.Fc.name + "_1" ||
                                vc.Dp[j].name == "k" + wfd.Fc.name + "1" ||
                                vc.Dp[j].name == "k" + wfd.Fc.name + "_1")
                            {
                                wfd.Drukknop = vc.Dp[j];
                                break;
                            }
                        }

                        if (/*wfd.Drukknop != null &&*/
                            wfd.Koplus != null &&
                            wfd.Verweglus != null)
                        {
                            //fietsrichting met koplus, verweglus. Drukknop is optioneel.
                            meetpuntList.Add(wfd.Fc.name, wfd);
                        }
                    }
                }/*
                else if(vc.Fc[i].name.StartsWith("2"))
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("VRI {0} FC {1} niet als fiets opgegeven",vc.Sys, vc.Fc[i].name));
                }*/
            }
            result.Meetpunten = meetpuntList.Values.ToArray();
            result.Succes = true;

            return result;
        }

        private static ResultMeetpuntenTellen bepaalTelMeetpunten(VLogCfg vlogCfg, GeoDataVri geoDataVri)
        {
            if (vlogCfg == null || geoDataVri == null)
            {
               // addErrorText(string.Format("Één van de parameters is NULL"));
                return null;
            }
            if (!vlogCfg.Valid || vlogCfg.Dp == null)
            {
                //addErrorText(string.Format("V-Log configuratie is niet geldig"));
                return null;
            }

            ResultMeetpuntenTellen result = new ResultMeetpuntenTellen();

            List<TelMeetpunt> meetpuntList = new List<TelMeetpunt>();
            string errorText = "";

            //alle tellussen verzamelen
            List<GeoDataLus> verwegEnTellussen = new List<GeoDataLus>();
            verwegEnTellussen.AddRange(geoDataVri.FietsVerweglussen);
            verwegEnTellussen.AddRange(geoDataVri.FietsTellussen);
            //binnen vlog configuratie de lus opzoeken 
            foreach (GeoDataLus gdLus in verwegEnTellussen)
            {
                VLogCfgItem vlogCfgItem = null;
                bool error = false;
                foreach (VLogCfgItem vci in vlogCfg.Dp)
                {
                    if (vci.name == gdLus.Name)
                    {
                        //vlog index gevonden
                        if (vlogCfgItem != null)
                        {
                            //meerdere detectoren in vlog configuratie met dezelfde naam
                            errorText += string.Format("Detector \"{0}\" komt meer dan één keer voor in de V-Log configuratie. ", gdLus.Name);
                            error = true;
                            break;
                        }
                        vlogCfgItem = vci;
                    }
                }
                if (vlogCfgItem == null)
                {
                    //detector niet gevonden
                    errorText += string.Format("Detector \"{0}\" komt niet voor in de V-Log configuratie. ", gdLus.Name);
                    error = true;
                }

                if (!error && vlogCfgItem != null)
                {
                    //toevoegen detector
                    TelMeetpunt meetpunt = new TelMeetpunt()
                    {
                        Lus = vlogCfgItem,
                        Heading = gdLus.Heading,
                        Location = gdLus.LocationLus
                    };
                    meetpuntList.Add(meetpunt);
                }
            }

            result.Meetpunten = meetpuntList.ToArray();
            result.ErrorText = errorText;
            result.Succes = true;

            return result;
        }
    }   

    public class PostSettings
    {
        public string URL = "";
        public string Username = "";
        public string Password = "";
        public int MaxPostSize = 1000 * 1024; //1000kB default
    }

}
