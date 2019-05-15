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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO.Compression;
using VVE.VLog;

namespace VVE
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            //instellingen
            loadAllSettings();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveAllSettings();

            //tijdelijke V-Log bestanden opruimen uit temp directory
            string[] filesVlg = Directory.GetFiles(Path.GetTempPath(), "*.vlg", SearchOption.TopDirectoryOnly);
            foreach (string f in filesVlg)
            {
                try
                {
                    File.Delete(f);
                }
                catch { }
            }

            //tijdelijke V-Log bestanden opruimen uit temp directory
            string[] filesVlc = Directory.GetFiles(Path.GetTempPath(), "*.vlc", SearchOption.TopDirectoryOnly);
            foreach (string f in filesVlc)
            {
                try
                {
                    File.Delete(f);
                }
                catch { }
            }
        }

        VLogCfgDir configuraties = new VLogCfgDir();
        private void ladenConfiguratieBestanden()
        {
            string vlogcfgDir = tbDirConfiguraties.Text.Trim();

            configuraties.ReadFiles(vlogcfgDir);
        }

        private void tbDirConfiguraties_TextChanged(object sender, EventArgs e)
        {
            ladenConfiguratieBestanden();

            updateKruispuntNummers();
        }

        private void saveAllSettings()
        {
            //mappen
            Properties.Settings.Default.VLogDataDir = tbDirVLogArchief.Text;
            Properties.Settings.Default.VLogCfgDir = tbDirConfiguraties.Text;
            Properties.Settings.Default.KmlConfigDir = tbKmlConfiguratie.Text;

            //VRI, VLC en periode selectie
            Properties.Settings.Default.VRI = cbVris.Text;
            Properties.Settings.Default.VLC = cbVlcFileNames.Text;
            Properties.Settings.Default.PeriodeVan = dtpVanDatum.Value.Date.AddMinutes(dtpVanTijd.Value.TimeOfDay.TotalMinutes).Ticks;
            Properties.Settings.Default.PeriodeTot = dtpTotDatum.Value.Date.AddMinutes(dtpTotTijd.Value.TimeOfDay.TotalMinutes).Ticks;

            //V-Log naar ZIP bestand
            Properties.Settings.Default.VLogBestandEenheid = rbDagbestanden.Checked ? "dag" : "een";

            //Instellingen
            Properties.Settings.Default.InstellingIntervalMinuten = GetIntervalMinutes();

            Properties.Settings.Default.InstellingTellenMinHoog = nudTellenFilterDet.Value;
            Properties.Settings.Default.InstellingTellenAlleenKoplussenCsv = cbTellenAlleenKoplussenCsv.Checked;

            Properties.Settings.Default.InstellingWachttijdBezettijdVerweglus = nudWachttijdBezettijdVerweglus.Value;
            Properties.Settings.Default.InstellingWachttijdBezettijdKoplus = nudWachttijdBezettijdKoplus.Value;
            Properties.Settings.Default.InstellingWachttijdNormRijtijdVerweglus = nudWachttijdNormRijtijdVerweglus.Value;
            Properties.Settings.Default.InstellingWachttijdMaxRijtijdVerweglus = nudWachttijdMaxRijtijdVerweglus.Value;
            Properties.Settings.Default.InstellingWachttijdIntrekkenAanvraag = nudWachttijdIntrekkenAanvraag.Value;
            Properties.Settings.Default.InstellingWachttijdIndivFietsersCsv = cbWachttijdIndivFietsersCsv.Checked;

            Properties.Settings.Default.InstellingTabIndex = tcInstellingen.SelectedIndex;

            //Analyse CSV
            Properties.Settings.Default.CsvFilterMa = cbCsvFilterMaandag.Checked;
            Properties.Settings.Default.CsvFilterDi = cbCsvFilterDinsdag.Checked;
            Properties.Settings.Default.CsvFilterWo = cbCsvFilterWoensdag.Checked;
            Properties.Settings.Default.CsvFilterDo = cbCsvFilterDonderdag.Checked;
            Properties.Settings.Default.CsvFilterVr = cbCsvFilterVrijdag.Checked;
            Properties.Settings.Default.CsvFilterZa = cbCsvFilterZaterdag.Checked;
            Properties.Settings.Default.CsvFilterZo = cbCsvFilterZondag.Checked;
            Properties.Settings.Default.CsvFilterPer1Actief = cbCsvFilterPeriode1.Checked;
            Properties.Settings.Default.CsvFilterPer1Van = dtpPeriod1Van.Value.Ticks;
            Properties.Settings.Default.CsvFilterPer1Tot = dtpPeriod1Tot.Value.Ticks;
            Properties.Settings.Default.CsvFilterPer2Actief = cbCsvFilterPeriode2.Checked;
            Properties.Settings.Default.CsvFilterPer2Van = dtpPeriod2Van.Value.Ticks;
            Properties.Settings.Default.CsvFilterPer2Tot = dtpPeriod2Tot.Value.Ticks;

            //Analyse DPF
            Properties.Settings.Default.DpfVriSelectie = cbDpfVriSelectie.Text;
            Properties.Settings.Default.DpfAnalyse = cbDpfAnalyse.Text;

            Properties.Settings.Default.DpfPostUrl = tbDpfPostUrl.Text;
            Properties.Settings.Default.DpfPostUser = tbDpfPostUser.Text;
            Properties.Settings.Default.DpfPostPass = tbDpfPostPass.Text;
            Properties.Settings.Default.DpfPostMaxSizekB = nudDpfPostMaxSize.Value;

            Properties.Settings.Default.DpfAutoPostDtTicks = dtpAutoPostTime.Value.Ticks;
            Properties.Settings.Default.DpfAutoPostEnabled = cbDpfAutoPost.Checked;

            //Analyse
            Properties.Settings.Default.AnalyseTabIndex = tcAnalyses.SelectedIndex;

            Properties.Settings.Default.Save();
        }

        private void loadAllSettings()
        {
            //mappen
            tbDirVLogArchief.Text = Properties.Settings.Default.VLogDataDir;
            tbDirConfiguraties.Text = Properties.Settings.Default.VLogCfgDir;
            tbKmlConfiguratie.Text = Properties.Settings.Default.KmlConfigDir;

            //VRI, VLC en periode selectie
            DateTime vanDatum = new DateTime(Properties.Settings.Default.PeriodeVan);
            if (vanDatum.Ticks != 0)
            {
                dtpVanDatum.Value = vanDatum.Date;
                dtpVanTijd.Value = vanDatum;
            }
            DateTime totDatum = new DateTime(Properties.Settings.Default.PeriodeTot);
            if (totDatum.Ticks != 0)
            {
                dtpTotDatum.Value = totDatum.Date;
                dtpTotTijd.Value = totDatum;
            }

            //V-Log naar ZIP bestand
            if (Properties.Settings.Default.VLogBestandEenheid == "dag") rbDagbestanden.Checked = true;
            else rbEenBestand.Checked = true;

            //Instellingen
            SetIntervalMinutes(Properties.Settings.Default.InstellingIntervalMinuten);

            nudTellenFilterDet.Value = Properties.Settings.Default.InstellingTellenMinHoog;
            cbTellenAlleenKoplussenCsv.Checked = Properties.Settings.Default.InstellingTellenAlleenKoplussenCsv;

            nudWachttijdBezettijdVerweglus.Value = Properties.Settings.Default.InstellingWachttijdBezettijdVerweglus;
            nudWachttijdBezettijdKoplus.Value = Properties.Settings.Default.InstellingWachttijdBezettijdKoplus;
            nudWachttijdNormRijtijdVerweglus.Value = Properties.Settings.Default.InstellingWachttijdNormRijtijdVerweglus;
            nudWachttijdMaxRijtijdVerweglus.Value = Properties.Settings.Default.InstellingWachttijdMaxRijtijdVerweglus;
            nudWachttijdIntrekkenAanvraag.Value = Properties.Settings.Default.InstellingWachttijdIntrekkenAanvraag;
            cbWachttijdIndivFietsersCsv.Checked = Properties.Settings.Default.InstellingWachttijdIndivFietsersCsv;

            tcInstellingen.SelectedIndex = Properties.Settings.Default.InstellingTabIndex;
            updateSelectedCsvAnalyse();

            //Analyse CSV
            cbCsvFilterMaandag.Checked = Properties.Settings.Default.CsvFilterMa;
            cbCsvFilterDinsdag.Checked = Properties.Settings.Default.CsvFilterDi;
            cbCsvFilterWoensdag.Checked = Properties.Settings.Default.CsvFilterWo;
            cbCsvFilterDonderdag.Checked = Properties.Settings.Default.CsvFilterDo;
            cbCsvFilterVrijdag.Checked = Properties.Settings.Default.CsvFilterVr;
            cbCsvFilterZaterdag.Checked = Properties.Settings.Default.CsvFilterZa;
            cbCsvFilterZondag.Checked = Properties.Settings.Default.CsvFilterZo;
            cbCsvFilterPeriode1.Checked = Properties.Settings.Default.CsvFilterPer1Actief;
            if (Properties.Settings.Default.CsvFilterPer1Van != 0) dtpPeriod1Van.Value = new DateTime(Properties.Settings.Default.CsvFilterPer1Van);
            if (Properties.Settings.Default.CsvFilterPer1Tot != 0) dtpPeriod1Tot.Value = new DateTime(Properties.Settings.Default.CsvFilterPer1Tot);
            cbCsvFilterPeriode2.Checked = Properties.Settings.Default.CsvFilterPer2Actief;
            if (Properties.Settings.Default.CsvFilterPer2Van != 0) dtpPeriod2Van.Value = new DateTime(Properties.Settings.Default.CsvFilterPer2Van);
            if (Properties.Settings.Default.CsvFilterPer2Tot != 0) dtpPeriod2Tot.Value = new DateTime(Properties.Settings.Default.CsvFilterPer2Tot);

            //Analyse DPF
            cbDpfVriSelectie.Text = Properties.Settings.Default.DpfVriSelectie;
            cbDpfAnalyse.Text = Properties.Settings.Default.DpfAnalyse;

            tbDpfPostUrl.Text = Properties.Settings.Default.DpfPostUrl;
            tbDpfPostUser.Text = Properties.Settings.Default.DpfPostUser;
            tbDpfPostPass.Text = Properties.Settings.Default.DpfPostPass;
            nudDpfPostMaxSize.Value = Properties.Settings.Default.DpfPostMaxSizekB;
            if (Properties.Settings.Default.DpfAutoPostDtTicks != 0) dtpAutoPostTime.Value = new DateTime(Properties.Settings.Default.DpfAutoPostDtTicks);
            
            //Analyse
            tcAnalyses.SelectedIndex = Properties.Settings.Default.AnalyseTabIndex;

            ladenConfiguratieBestanden();
            updateKruispuntNummers();

            cbVris.Text = Properties.Settings.Default.VRI;
            cbVlcFileNames.Text = Properties.Settings.Default.VLC;

            cbDpfAutoPost.Checked = Properties.Settings.Default.DpfAutoPostEnabled;
        }

        private void updateKruispuntNummers()
        {
            //bepalen kruispuntnummers
            List<string> nrs = configuraties.Vris.Keys.ToList();
            NumberComparer sorter = new NumberComparer();
            nrs.Sort(sorter);

            //updaten combobox
            cbVris.BeginUpdate();
            cbVris.Items.Clear();
            foreach (string nr in nrs) cbVris.Items.Add(nr);
            cbVris.EndUpdate();
        }

        private void updateVlcFileNames()
        {
            string vriNr = cbVris.Text;

            //updaten combobox
            cbVlcFileNames.BeginUpdate();
            cbVlcFileNames.Items.Clear();
            cbVlcFileNames.Text = "";

            if (vriNr.Length != 0)
            {
                if (configuraties.Vris.ContainsKey(vriNr))
                {
                    VlogCfgVri vlcs = configuraties.Vris[vriNr];
                    foreach (VlogCfgFile vlc in vlcs.Configs) cbVlcFileNames.Items.Add(vlc.Name);
                    if (vlcs.Configs.Length > 0) cbVlcFileNames.Text = vlcs.Configs[0].Name;
                }
            }
            cbVlcFileNames.EndUpdate();
        }

        public class NumberComparer : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                int int1 = int.Parse(a);
                int int2 = int.Parse(b);
                return int1.CompareTo(int2);
            }
        }

        private void btGetVLogData_Click(object sender, EventArgs e)
        {
            string vri = cbVris.Text;
            string vlcName = cbVlcFileNames.Text;

            StringBuilder result = new StringBuilder();
            result.AppendLine("Get VLog data");
            result.AppendLine("-------------");

            bool error = false;

            //check aanwezigheid kruispunt
            if (!error && !configuraties.Vris.ContainsKey(vri))
            {
                result.AppendFormat("Kruispunt {0} niet aanwezig.\r\n", vri);
                error = true;
            }

            //bepalen vlog configuratie(s): alleen van de geselecteerde VRI en specifieke configuratie
            Dictionary<string, byte[]> configs = configuraties.GetSpecific(vri, vlcName);
            if (!error && (configs == null || configs.Count == 0))
            {
                result.AppendFormat("Geen VLog configuratie {0} aanwezig.\r\n", vlcName);
                error = true;
            }

            //check aanwezigheid V-Log data map
            string vlgRoot = "";
            if (!error)
            {
                //kruispunt aanwezig
                //bestanden doorzoeken
                vlgRoot = tbDirVLogArchief.Text.Trim();
                if (!Directory.Exists(vlgRoot))
                {
                    result.AppendFormat("Map {0} bestaat niet.\r\n", vlgRoot);
                    error = true;
                }
            }

            //bepalen gewenste tijdstip van-tot
            DateTime from = new DateTime();
            DateTime to = new DateTime();
            if (!error)
            {
                from = dtpVanDatum.Value.Date.AddMinutes(dtpVanTijd.Value.TimeOfDay.TotalMinutes);
                to = dtpTotDatum.Value.Date.AddMinutes(dtpTotTijd.Value.TimeOfDay.TotalMinutes);
                if ((to - from).Ticks <= 0)
                {
                    result.AppendFormat("Einddatum gelijk aan of voor begindatum.\r\n");
                    error = true;
                }
            }

            //opslag in dagbestanden of een totaalbestand
            bool dagbestanden = rbDagbestanden.Checked;

            //bepalen filenaam voor opslag
            if (!error)
            {
                saveFileDialogVlgZip.FileName = String.Format("K{0} VLog {1} tot {2}.zip", VLogArchiefDenHaag.VriNameToFullNumber(vri), from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));
                DialogResult res = saveFileDialogVlgZip.ShowDialog();
                if (res != DialogResult.OK)
                {
                    result.AppendFormat("Geen uitvoerbestand opgegeven.\r\n");
                    error = true;
                }
            }

            //uitvoeren V-Log data verzamelen
            if (!error)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                VveFuncResult resVve = VveMainFunctions.GetAndSaveVLogData(vlgRoot, vri, from, to, saveFileDialogVlgZip.FileName, configs, dagbestanden);
                result.AppendLine(resVve.ToString());

                sw.Stop();
                result.AppendFormat("Processed in {0} seconds.\r\n", (sw.ElapsedMilliseconds / 1000M).ToString("F3"));
            }

            tbResultaat.Text = result.ToString();

            GC.Collect();
        }

        private void btOpenMetViewer_Click(object sender, EventArgs e)
        {
            //'van' dag data bij elkaar zoeken en openen met viewer
            StringBuilder result = new StringBuilder();
            result.AppendLine("Open data met viewer");
            result.AppendLine("--------------------");

            string vri = cbVris.Text;
            string vlcName = cbVlcFileNames.Text;

            bool error = false;

            byte[] configuratie = new byte[0];

            //check aanwezigheid kruispunt
            if (!error && !configuraties.Vris.ContainsKey(vri))
            {
                result.AppendFormat("Kruispunt {0} niet aanwezig.\r\n", vri);
                error = true;
            }

            //bepalen vlog configuratie(s): alleen van de geselecteerde VRI en specifieke configuratie
            Dictionary<string, byte[]> configs = configuraties.GetSpecific(vri, vlcName);
            if (!error && (configs == null || configs.Count == 0))
            {
                result.AppendFormat("Geen VLog configuratie {0} aanwezig.\r\n", vlcName);
                error = true;
            }

            //check aanwezigheid V-Log data map
            string vlgRoot = "";
            if (!error)
            {
                //kruispunt aanwezig
                //bestanden doorzoeken
                vlgRoot = tbDirVLogArchief.Text.Trim();
                if (!Directory.Exists(vlgRoot))
                {
                    result.AppendFormat("Map {0} bestaat niet.\r\n", vlgRoot);
                    error = true;
                }
            }

            //bepalen gewenste tijdstip van-tot
            DateTime from = new DateTime();
            DateTime to = new DateTime();
            if (!error)
            {
                from = dtpVanDatum.Value.Date.AddMinutes(dtpVanTijd.Value.TimeOfDay.TotalMinutes);
                to = dtpTotDatum.Value.Date.AddMinutes(dtpTotTijd.Value.TimeOfDay.TotalMinutes);
                if ((to - from).Ticks <= 0)
                {
                    result.AppendFormat("Einddatum gelijk aan of voor begindatum.\r\n");
                    error = true;
                }
            }

            //uitvoeren V-Log data verzamelen
            if (!error)
            {     
                Stopwatch sw = new Stopwatch();
                sw.Start();

                VveFuncResult resVve = VveMainFunctions.SaveVLogDataForViewer(vlgRoot, vri, from, to, configs);
                result.AppendLine(resVve.ToString());
                sw.Stop();
                result.AppendFormat("Processed in {0} seconds.\r\n", (sw.ElapsedMilliseconds / 1000M).ToString("F3"));
            }

            tbResultaat.Text = result.ToString();

            GC.Collect();
        }

        private int GetIntervalMinutes()
        {
            switch (cbInterval.Text)
            {
                case "1 min":
                    return 1;
                case "5 min":
                    return 5;
                case "15 min":
                    return 15;
                case "30 min":
                    return 30;
                case "1 uur":
                    return 60;
                case "1 dag":
                    return 1 * 24 * 60;
                default:
                    return 60; //default uur
            }
        }

        private void SetIntervalMinutes(int min)
        {
            string waarde = "";
            switch (min)
            {
                case 1:
                    waarde = "1 min";
                    break;
                case 5:
                    waarde = "5 min";
                    break;
                case 15:
                    waarde = "15 min";
                    break;
                case 30:
                    waarde = "30 min";
                    break;
                case 60:
                    waarde = "1 uur";
                    break;
                case (1 * 24 * 60):
                    waarde = "1 dag";
                    break;
                default:
                    waarde = "1 uur"; //default uur
                    break;
            }
            cbInterval.Text = waarde;
        }

        private void btTestCfg_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("Analyse all V-Log configuration files");
            result.AppendLine("-------------------------------------");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (KeyValuePair<string, byte[]> kvp in configuraties.GetDefaults())
            {
                //check of het configuratiebestand correct is
                VLogCfg vc = new VLogCfg();
                if (vc.ReadConfig(kvp.Value)) result.AppendFormat("Configuratie VRI {0}: OK\r\n", kvp.Key);
                else
                {
                    result.AppendFormat("Configuratie VRI {0}: formaat incorrect. {1}\r\n", kvp.Key, vc.ErrorText);
                }
                if (vc.Is.Length > 127) result.AppendFormat("Configuratie VRI {0}: aantal IS = {1}\r\n", kvp.Key, vc.Is.Length);
                if (vc.Us.Length > 127) result.AppendFormat("Configuratie VRI {0}: aantal US = {1}\r\n", kvp.Key, vc.Us.Length);
                //result.AppendFormat("Configuratie VRI {0}: aantal FC = {1}\r\n", kvp.Key, vc.Fc.Length);
            }

            sw.Stop();
            result.AppendFormat("Processed in {0} seconds.\r\n", (sw.ElapsedMilliseconds / 1000M).ToString("F3"));
            tbResultaat.Text = result.ToString();
        }

        private void btTestFindTimeRefs_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("Test find timerefs");
            result.AppendLine("------------------");

            string rootTestData = @"..\..\2018-09-03 Testdata archief zoekalgoritme\";

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // DateTime from = dtpVanDatum.Value.Date.AddMinutes(dtpVanTijd.Value.TimeOfDay.TotalMinutes);
            //  result.AppendLine( VLogArchiefDenHaag.TestFindTimeRefLocations(@"D:\Projecten\2018-05-04 Den Haag VVE\vlog_data test\1"));

            result.AppendLine("Test dataset 1");
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"1", "57", new DateTime(2018, 05, 10, 10, 00, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"1", "57", new DateTime(2018, 05, 10, 10, 04, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"1", "57", new DateTime(2018, 05, 10, 10, 05, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"1", "57", new DateTime(2018, 05, 10, 14, 10, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"1", "57", new DateTime(2018, 05, 10, 17, 00, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"1", "57", new DateTime(2018, 05, 10, 5, 30, 00), 2).ToString());

            result.AppendLine();
            result.AppendLine("Test dataset 2");
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"2", "57", new DateTime(2018, 05, 10, 10, 00, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"2", "57", new DateTime(2018, 05, 10, 10, 10, 00), 2).ToString());

            result.AppendLine();
            result.AppendLine("Test dataset 3");
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"3", "57", new DateTime(2018, 05, 10, 10, 00, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"3", "57", new DateTime(2018, 05, 10, 10, 31, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"3", "57", new DateTime(2018, 05, 10, 9, 00, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"3", "57", new DateTime(2018, 05, 10, 6, 13, 00), 2).ToString());

            result.AppendLine();
            result.AppendLine("Test dataset 4");
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"4", "57", new DateTime(2018, 05, 10, 10, 00, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"4", "57", new DateTime(2018, 05, 10, 9, 31, 00), 2).ToString());
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"4", "57", new DateTime(2018, 05, 10, 11, 00, 00), 2).ToString());

            result.AppendLine();
            result.AppendLine("Test dataset 5");
            result.AppendLine(VLogArchiefDenHaag.FindTimeRefs(rootTestData + @"5", "57", new DateTime(2018, 05, 10, 10, 00, 00), 2).ToString());

            sw.Stop();
            result.AppendFormat("Processed in {0} seconds.\r\n", (sw.ElapsedMilliseconds / 1000M).ToString("F3"));

            tbResultaat.Text = result.ToString();

            GC.Collect();
        }

        private void btTestVLog_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("Test V-Log parsing");
            result.AppendLine("------------------");


            Stopwatch sw = new Stopwatch();
            sw.Start();

            // DateTime from = dtpVanDatum.Value.Date.AddMinutes(dtpVanTijd.Value.TimeOfDay.TotalMinutes);
            //  result.AppendLine( VLogArchiefDenHaag.TestFindTimeRefLocations(@"D:\Projecten\2018-05-04 Den Haag VVE\vlog_data test\1"));

            result.AppendLine(Test.TestMessages());

            sw.Stop();
            result.AppendFormat("Processed in {0} seconds.\r\n", (sw.ElapsedMilliseconds / 1000M).ToString("F3"));

            tbResultaat.Text = result.ToString();

            GC.Collect();
        }

        private void btAnalyseCSV_Click(object sender, EventArgs e)
        {
            string vri = cbVris.Text;
            string vlcName = cbVlcFileNames.Text;

            //bepalen analyse
            bool analyseTellen = false;
            bool analyseWachttijd = false;
            string analyseTekst = lblCsvAnalyse.Text;
            switch (analyseTekst)
            {
                case "tellen":
                    analyseTellen = true;
                    break;
                case "wachttijd":
                    analyseWachttijd = true;
                    break;
            }

            StringBuilder result = new StringBuilder();
            result.AppendFormat("Analyse {0} CSV\r\n", analyseTekst);
            result.Append("------------");
            for (int i = 0; i < analyseTekst.Length; i++) result.Append("-");
            result.AppendLine();

            bool error = false;

            //check aanwezigheid kruispunt
            if (!error && !configuraties.Vris.ContainsKey(vri))
            {
                result.AppendFormat("Kruispunt {0} niet aanwezig.\r\n", vri);
                error = true;
            }

            //bepalen vlog configuratie(s): alleen van de geselecteerde VRI en specifieke configuratie
            Dictionary<string, byte[]> configs = configuraties.GetSpecific(vri, vlcName);
            if (!error && (configs == null || configs.Count == 0))
            {
                result.AppendFormat("Geen VLog configuratie {0} aanwezig.\r\n", vlcName);
                error = true;
            }

            string vlgRoot = "";
            if (!error)
            {
                //kruispunt aanwezig
                //bestanden doorzoeken
                vlgRoot = tbDirVLogArchief.Text.Trim();
                if (!Directory.Exists(vlgRoot))
                {
                    result.AppendFormat("V-Log data map {0} bestaat niet.\r\n", vlgRoot);
                    error = true;
                }
            }

            //periode
            DateTime from = new DateTime();
            DateTime to = new DateTime();
            if (!error)
            {
                from = dtpVanDatum.Value.Date.AddMinutes(dtpVanTijd.Value.TimeOfDay.TotalMinutes);
                to = dtpTotDatum.Value.Date.AddMinutes(dtpTotTijd.Value.TimeOfDay.TotalMinutes);
                if ((to - from).Ticks <= 0)
                {
                    result.AppendFormat("Einddatum ligt voor of op de begindatum.\r\n");
                    error = true;
                }
            }

            //filter voor csv uitvoer
            CsvFilter filter = new CsvFilter();
            if (!error)
            {
                filter = getCsvFilter();

                if (!filter.Min1WerkdagGeselecteerd)
                {
                    result.AppendFormat("Geen weekdagen geselecteerd.\r\n");
                    error = true;
                }
            }

            //vragen naar filenaam voor resultaten
            string resultFilename = "";
            string vriNameLong = VLogArchiefDenHaag.VriNameToFullNumber(vri);
            if (!error)
            {
                saveFileDialogCSV.FileName = String.Format("K{0} {1} {2} tot {3}.csv", vriNameLong, analyseTekst, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));
                DialogResult res = saveFileDialogCSV.ShowDialog();
                if (res == DialogResult.OK)
                {
                    resultFilename = saveFileDialogCSV.FileName;
                }
                else
                {
                    resultFilename = ""; //analyse wel uitvoeren
                }
            }

            //----------- uitvoeren analyses -----------      
            if (!error)
            {
                VveFuncResult resVve = new VveFuncResult();
                if (analyseTellen)
                {
                    //Analyse settings
                    AnalyseTellenSettings ais = TellenSettings();

                    //Uitvoer settings
                    bool alleenKoplussen = cbTellenAlleenKoplussenCsv.Checked;
                    AnalyseTellenUitvoerSettingsCsv aus = new AnalyseTellenUitvoerSettingsCsv()
                    {
                        Filter = filter,
                        AlleenKoplussen = alleenKoplussen
                    };

                    //Analyse uitvoeren
                    resVve = VveMainFunctions.PerformCountAnalyseCsv(vlgRoot, vri, from, to, resultFilename, configs, ais, aus);
                }
                if (analyseWachttijd)
                {
                    //Analyse settings
                    AnalyseWachttijdSettings aws = WachttijdSettings();

                    //Uitvoer settings
                    bool indivFietsersInUitvoer = cbWachttijdIndivFietsersCsv.Checked;
                    AnalyseWachttijdUitvoerSettingsCsv aus = new AnalyseWachttijdUitvoerSettingsCsv()
                    {
                        Filter = filter,
                        IndividueleFietsers = indivFietsersInUitvoer
                    };

                    //Analyse uitvoeren
                    resVve = VveMainFunctions.PerformWaitAnalyseCsvFile(vlgRoot, vri, from, to, resultFilename, configs, aws, aus);
                }

                //Resultaat weergeven
                result.AppendLine(resVve.ToString());
            }

            tbResultaat.Text = result.ToString();

            GC.Collect();
        }

        private void btAnalyseDPF_Click(object sender, EventArgs e)
        {
            string vri = "";
            string vlcName = cbVlcFileNames.Text;

            //bepalen VRI
            bool alleVris = false;
            switch (cbDpfVriSelectie.Text)
            {
                case "geselecteerde":
                    vri = cbVris.Text;
                    break;
                case "alle":
                    alleVris = true;
                    break;
            }

            //bepalen analyse
            bool analyseTellen = false;
            bool analyseWachttijd = false;
            string analyseTekst = cbDpfAnalyse.Text;
            switch (analyseTekst)
            {
                case "tellen":
                    analyseTellen = true;
                    break;
                case "wachttijd":
                    analyseWachttijd = true;
                    break;
                case "alle":
                    analyseTellen = true;
                    analyseWachttijd = true;
                    break;
            }

            StringBuilder result = new StringBuilder();
            result.AppendFormat("Analyse {0} DPF naar map\r\n", analyseTekst);
            result.Append("---------------------");
            for (int i = 0; i < analyseTekst.Length; i++) result.Append("-");
            result.AppendLine();

            bool error = false;

            //----------- input -----------

            //bepalen vlog configuratie(s): alle default configuraties opvragen of alleen van de geselecteerde VRI en specifieke configuratie
            Dictionary<string, byte[]> configs = alleVris ? configuraties.GetDefaults() : configuraties.GetSpecific(vri, vlcName);
            if (!error && (configs == null || configs.Count == 0))
            {
                result.AppendLine("Geen VLog configuratie(s) aanwezig");
                error = true;
            }

            //KML directory
            string kmlConfigDir = "";
            if (!error)
            {
                kmlConfigDir = tbKmlConfiguratie.Text.Trim();
                if (!Directory.Exists(kmlConfigDir))
                {
                    result.AppendFormat("KML map {0} bestaat niet.\r\n", kmlConfigDir);
                    error = true;
                }
            }

            //V-Log data directory
            string vlgRoot = "";
            if (!error)
            {
                //kruispunt aanwezig
                //bestanden doorzoeken
                vlgRoot = tbDirVLogArchief.Text.Trim();
                if (!Directory.Exists(vlgRoot))
                {
                    result.AppendFormat("V-Log data map {0} bestaat niet.\r\n", vlgRoot);
                    error = true;
                }
            }

            //periode
            DateTime from = new DateTime();
            DateTime to = new DateTime();
            if (!error)
            {
                from = dtpVanDatum.Value.Date.AddMinutes(dtpVanTijd.Value.TimeOfDay.TotalMinutes);
                to = dtpTotDatum.Value.Date.AddMinutes(dtpTotTijd.Value.TimeOfDay.TotalMinutes);
                if ((to - from).Ticks <= 0)
                {
                    result.AppendFormat("Einddatum ligt voor of op de begindatum.\r\n");
                    error = true;
                }
            }

            //vragen naar directory voor resultaten
            string resultDir = "";
            DialogResult res = folderBrowserDialogDpfResult.ShowDialog();
            if (res == DialogResult.OK)
            {
                resultDir = folderBrowserDialogDpfResult.SelectedPath;
            }
            else
            {
                //geen locatie om resultaten op te slaan
                resultDir = "";
                result.AppendFormat("Geen map opgegeven voor het opslaan van de resultaten.\r\n");
                error = true;
            }

            //----------- uitvoeren analyses -----------
            if (!error)
            {
                if (analyseTellen)
                {
                    result.AppendLine("Tellen naar DPF");
                    result.AppendLine("---------------");

                    //Analyse settings
                    AnalyseTellenSettings ais = TellenSettings();

                    //Analyse uitvoeren
                    VveFuncResult resVve = VveMainFunctions.PerformCountAnalyseDpf(vlgRoot, kmlConfigDir, from, to, configs, ais, alleVris ? "" : vri);
                    resVve = VveMainFunctions.PerformSaveDpfToFile(resVve, resultDir);

                    //Resultaat weergeven
                    result.AppendLine(resVve.ToString());
                    result.AppendLine();
                }
                if (analyseWachttijd)
                {
                    result.AppendLine("Wachttijd naar DPF");
                    result.AppendLine("------------------");

                    //Analyse settings
                    AnalyseWachttijdSettings aws = WachttijdSettings();

                    //Analyse uitvoeren
                    VveFuncResult resVve = VveMainFunctions.PerformWaitAnalyseDpf(vlgRoot, kmlConfigDir, from, to, configs, aws, alleVris ? "" : vri);
                    resVve = VveMainFunctions.PerformSaveDpfToFile(resVve, resultDir);

                    //Resultaat weergeven
                    result.AppendLine(resVve.ToString());
                    result.AppendLine();
                }
            }

            //----------- respons weergeven -----------

            tbResultaat.Text = result.ToString();

            GC.Collect();
        }

        private void btPostNaarFietsViewer_Click(object sender, EventArgs e)
        {
            //----------- respons weergeven -----------
            tbResultaat.Text = DpfPost();

        }

        private string DpfPost(DateTime? specificDay=null)
        {
            string vri = "";
            string vlcName = cbVlcFileNames.Text;

            //bepalen VRI
            bool alleVris = false;
            switch (cbDpfVriSelectie.Text)
            {
                case "geselecteerde":
                    vri = cbVris.Text;
                    break;
                case "alle":
                    alleVris = true;
                    break;
            }

            //bepalen analyse
            bool analyseTellen = false;
            bool analyseWachttijd = false;
            switch (cbDpfAnalyse.Text)
            {
                case "tellen":
                    analyseTellen = true;
                    break;
                case "wachttijd":
                    analyseWachttijd = true;
                    break;
                case "alle":
                    analyseTellen = true;
                    analyseWachttijd = true;
                    break;
            }

            StringBuilder result = new StringBuilder();
            result.AppendLine("Posten DPF");
            result.AppendLine("----------");
            result.AppendLine("Start: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            result.AppendLine();

            bool error = false;

            //----------- input -----------

            //bepalen vlog configuratie(s): alle default configuraties opvragen of alleen van de geselecteerde VRI en specifieke configuratie
            Dictionary<string, byte[]> configs = alleVris ? configuraties.GetDefaults() : configuraties.GetSpecific(vri, vlcName);
            if (!error && (configs == null || configs.Count == 0))
            {
                result.AppendLine("Geen VLog configuratie(s) aanwezig");
                error = true;
            }

            //KML directory
            string kmlConfigDir = "";
            kmlConfigDir = tbKmlConfiguratie.Text.Trim();
            if (!Directory.Exists(kmlConfigDir))
            {
                result.AppendFormat("KML map {0} bestaat niet.\r\n", kmlConfigDir);
                error = true;
            }

            //V-Log data directory
            string vlgRoot = "";
            //kruispunt aanwezig
            //bestanden doorzoeken
            vlgRoot = tbDirVLogArchief.Text.Trim();
            if (!Directory.Exists(vlgRoot))
            {
                result.AppendFormat("V-Log data map {0} bestaat niet.\r\n", vlgRoot);
                error = true;
            }

            //periode
            DateTime from = new DateTime();
            DateTime to = new DateTime();
            if (specificDay == null)
            {
                from = dtpVanDatum.Value.Date.AddMinutes(dtpVanTijd.Value.TimeOfDay.TotalMinutes);
                to = dtpTotDatum.Value.Date.AddMinutes(dtpTotTijd.Value.TimeOfDay.TotalMinutes);
                if ((to - from).Ticks <= 0)
                {
                    result.AppendFormat("Einddatum ligt voor begindatum.\r\n");
                    error = true;
                }
            }
            else
            {
                //één specifieke dag
                from = (DateTime)specificDay;
                to = ((DateTime)specificDay).AddDays(1);
            }

            //----------- uitvoeren algoritme -----------
            if (!error)
            {
                PostSettings ps = FietsViewerPostSettings();

                if (analyseTellen)
                {
                    result.AppendLine("Tellen naar DPF");
                    result.AppendLine("---------------");
                    AnalyseTellenSettings ais = TellenSettings();

                    VveFuncResult resVve = VveMainFunctions.PerformCountAnalyseDpf(vlgRoot, kmlConfigDir, from, to, configs, ais, alleVris ? "" : vri);
                    VveFuncResult resVveAfterPost = VveMainFunctions.PerformPostDpfToHttp(resVve, ps);
                    result.AppendLine(resVveAfterPost.ToString());
                    result.AppendLine();
                }
                if (analyseWachttijd)
                {
                    result.AppendLine("Wachttijd naar DPF");
                    result.AppendLine("------------------");
                    AnalyseWachttijdSettings aws = WachttijdSettings();

                    VveFuncResult resVve = VveMainFunctions.PerformWaitAnalyseDpf(vlgRoot, kmlConfigDir, from, to, configs, aws, alleVris ? "" : vri);
                    VveFuncResult resVveAfterPost = VveMainFunctions.PerformPostDpfToHttp(resVve, ps);
                    result.AppendLine(resVveAfterPost.ToString());
                    result.AppendLine();
                }
            }

            GC.Collect();
            
            return result.ToString();
        }

        /// <summary>
        /// Bepalen tellen algoritme settings
        /// </summary>
        /// <returns></returns>
        public AnalyseTellenSettings TellenSettings()
        {
            //analyse settings algemeen
            int intervalMin = GetIntervalMinutes();

            //filter minimale hoogtijd
            int detFilterMs = (int)(nudTellenFilterDet.Value * 1000);

            AnalyseTellenSettings aws = new AnalyseTellenSettings()
            {
                IntervalMin = intervalMin,
                DetFilterMs= detFilterMs
            };

            return aws;
        }

        /// <summary>
        /// Bepalen wachttijd algoritme settings
        /// </summary>
        /// <returns></returns>
        public AnalyseWachttijdSettings WachttijdSettings()
        {
            //analyse settings algemeen
            int intervalMin = GetIntervalMinutes();
            
            //analyse settings wachttijd
            int bezettijdVerweglus = (int)(nudWachttijdBezettijdVerweglus.Value * 1000);
            int bezettijdKoplus = (int)(nudWachttijdBezettijdKoplus.Value * 1000);
            int normRijtijdVerweglus = (int)(nudWachttijdNormRijtijdVerweglus.Value * 1000);
            int maxRijtijdVerweglus = (int)(nudWachttijdMaxRijtijdVerweglus.Value * 1000);
            int intrekkenAanvraag = (int)(nudWachttijdIntrekkenAanvraag.Value * 1000);

            AnalyseWachttijdSettings aws = new AnalyseWachttijdSettings()
            {
                IntervalMin = intervalMin,
                BezettijdVerweglus = bezettijdVerweglus,
                BezettijdKoplus = bezettijdKoplus,
                NormRijtijdVerweglus = normRijtijdVerweglus,
                MaxRijtijdVerweglus = maxRijtijdVerweglus,
                IntrekkenAanvraag = intrekkenAanvraag
            };

            return aws;
        }

        public CsvFilter getCsvFilter()
        {
            //filter weekdagen
            bool[] weekdagen = new bool[7]; //index 0 = maandag
            weekdagen[0] = cbCsvFilterMaandag.Checked;
            weekdagen[1] = cbCsvFilterDinsdag.Checked;
            weekdagen[2] = cbCsvFilterWoensdag.Checked;
            weekdagen[3] = cbCsvFilterDonderdag.Checked;
            weekdagen[4] = cbCsvFilterVrijdag.Checked;
            weekdagen[5] = cbCsvFilterZaterdag.Checked;
            weekdagen[6] = cbCsvFilterZondag.Checked;

            //filter perioden
            List<TimePeriod> filterPeriodenList = new List<TimePeriod>();
            if (cbCsvFilterPeriode1.Checked)
            {
                TimePeriod fp = new TimePeriod();
                fp.From = dtpPeriod1Van.Value.TimeOfDay;
                fp.To = dtpPeriod1Tot.Value.TimeOfDay;
                filterPeriodenList.Add(fp);
            }
            if (cbCsvFilterPeriode2.Checked)
            {
                TimePeriod fp = new TimePeriod();
                fp.From = dtpPeriod2Van.Value.TimeOfDay;
                fp.To = dtpPeriod2Tot.Value.TimeOfDay;
                filterPeriodenList.Add(fp);
            }
            TimePeriod[] filterPerioden = filterPeriodenList.ToArray();

            CsvFilter result = new CsvFilter() { FilterPerioden = filterPerioden, Weekdagen = weekdagen };

            return result;
        }

        public PostSettings FietsViewerPostSettings()
        {
            string url = tbDpfPostUrl.Text;
            string user = tbDpfPostUser.Text;
            string pass = tbDpfPostPass.Text;
            int maxSize = (int)(nudDpfPostMaxSize.Value)*1024;

            PostSettings ps = new PostSettings()
            {
                URL = url,
                Username = user,
                Password = pass,
                MaxPostSize = maxSize
            };

            return ps;
        }

        private void cbVris_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateVlcFileNames();
        }

        private void Main_Load(object sender, EventArgs e)
        {
#if DEBUG
            btTestFindTimeRefs.Visible = true;
            btTestVLog.Visible = true;
#else
            
            btTestFindTimeRefs.Visible = false;
            btTestVLog.Visible = false;
#endif
        }

        private void cbAutoPost_CheckedChanged(object sender, EventArgs e)
        {
            if(cbDpfAutoPost.Checked)
            {
                DisableAllControlsExclDpfAutoPost();
                lastTimeTick = DateTime.Now; //vanaf het moment van activering kijken of het tijdstip verstreken is
                autoPostTimer.Enabled = true;
            }
            else
            {
                EnableAllControlsInclDpfAutoPost();
                autoPostTimer.Enabled = false;
            }
        }

        private void DisableAllControlsExclDpfAutoPost()
        {
            //VRI
            cbVris.Enabled = false;
            cbVlcFileNames.Enabled = false;
            dtpVanDatum.Enabled = false;
            dtpVanTijd.Enabled = false;
            dtpTotDatum.Enabled = false;
            dtpTotTijd.Enabled = false;
            btOpenMetViewer.Enabled = false;
            gbVLogOpslaan.Enabled = false;

            //instellingen
            cbInterval.Enabled = false;
            nudTellenFilterDet.Enabled = false;
            cbTellenAlleenKoplussenCsv.Enabled = false;
            nudWachttijdBezettijdVerweglus.Enabled = false;
            nudWachttijdBezettijdKoplus.Enabled = false;
            nudWachttijdNormRijtijdVerweglus.Enabled = false;
            nudWachttijdMaxRijtijdVerweglus.Enabled = false;
            nudWachttijdIntrekkenAanvraag.Enabled = false;
            cbWachttijdIndivFietsersCsv.Enabled = false;

            //CSV
            gbCsvWeekdagen.Enabled = false;
            gbCsvFilterPerioden.Enabled = false;
            btAnalyseCSV.Enabled = false;

            //DPF
            cbDpfVriSelectie.Enabled = false;
            cbDpfAnalyse.Enabled = false;
            btAnalyseDPF.Enabled = false;
            tbDpfPostUrl.Enabled = false;
            tbDpfPostUser.Enabled = false;
            tbDpfPostPass.Enabled = false;
            nudDpfPostMaxSize.Enabled = false;
            btPostNaarFietsViewer.Enabled = false;
            dtpAutoPostTime.Enabled = false;

            //Tools
            btTestCfg.Enabled = false;

            //Dirs
            tbDirVLogArchief.Enabled = false;
            tbDirConfiguraties.Enabled = false;
            tbKmlConfiguratie.Enabled = false;
        }

        private void EnableAllControlsInclDpfAutoPost()
        {
            //VRI
            cbVris.Enabled = true;
            cbVlcFileNames.Enabled = true;
            dtpVanDatum.Enabled = true;
            dtpVanTijd.Enabled = true;
            dtpTotDatum.Enabled = true;
            dtpTotTijd.Enabled = true;
            btOpenMetViewer.Enabled = true;
            gbVLogOpslaan.Enabled = true;

            //instellingen
            cbInterval.Enabled = true;
            nudTellenFilterDet.Enabled = true;
            cbTellenAlleenKoplussenCsv.Enabled = true;
            nudWachttijdBezettijdVerweglus.Enabled = true;
            nudWachttijdBezettijdKoplus.Enabled = true;
            nudWachttijdNormRijtijdVerweglus.Enabled = true;
            nudWachttijdMaxRijtijdVerweglus.Enabled = true;
            nudWachttijdIntrekkenAanvraag.Enabled = true;
            cbWachttijdIndivFietsersCsv.Enabled = true;

            //CSV
            gbCsvWeekdagen.Enabled = true;
            gbCsvFilterPerioden.Enabled = true;
            btAnalyseCSV.Enabled = true;

            //DPF
            cbDpfVriSelectie.Enabled = true;
            cbDpfAnalyse.Enabled = true;
            btAnalyseDPF.Enabled = true;
            tbDpfPostUrl.Enabled = true;
            tbDpfPostUser.Enabled = true;
            tbDpfPostPass.Enabled = true;
            nudDpfPostMaxSize.Enabled = true;
            btPostNaarFietsViewer.Enabled = true;
            dtpAutoPostTime.Enabled = true;

            //Tools
            btTestCfg.Enabled = true;

            //Dirs
            tbDirVLogArchief.Enabled = true;
            tbDirConfiguraties.Enabled = true;
            tbKmlConfiguratie.Enabled = true;
        }

        private DateTime lastTimeTick = new DateTime();
        private DateTime lastProcessedDay = new DateTime();
        private void minutesTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (lastTimeTick.Ticks != 0)
            {
                if (now.TimeOfDay >= dtpAutoPostTime.Value.TimeOfDay &&
                    lastTimeTick.TimeOfDay < dtpAutoPostTime.Value.TimeOfDay)
                {
                    //overgang = post uitvoeren als voorgaande dag nog niet geprocessed is

                    DateTime previousDay = now.Date.AddDays(-1); //voorgaande dag bepalen
                   // if (lastProcessedDay < previousDay)
                    {
                        //V-Log configuraties updaten
                        ladenConfiguratieBestanden();
                        updateKruispuntNummers();

                        //analyse uitvoeren en resultaten posten
                        string result = DpfPost(previousDay);
                        tbResultaat.Text = result;

                        //resultaat opslaan in log directory
                        string logDir = "PostLog";
                        if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                        //opslaan data
                        string fileName = string.Format("{0}\\{1} Log Auto DPF Post.txt", logDir, DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                        try
                        {
                            File.WriteAllText(fileName, result);
                        }
                        catch (Exception ex)
                        {
                            tbResultaat.Text = string.Format("Log auto DPF post could not be saved to {0}: {1}\r\n", fileName, ex.Message) + result;
                        }
                        
                        lastProcessedDay = previousDay;
                        lblDpfPostLastDay.Text = previousDay.ToString("yyyy-MM-dd");
                    }
                }
            }
            else
            {
                //na starten applicatie nog niet eerder gerund
            }
            lastTimeTick = now;
        }

        private void tcInstellingen_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateSelectedCsvAnalyse();
        }

        private void updateSelectedCsvAnalyse()
        {
            int tabIdx = tcInstellingen.SelectedIndex;
            switch (tabIdx)
            {
                case 0:
                    lblCsvAnalyse.Text = "tellen";
                    break;
                case 1:
                    lblCsvAnalyse.Text = "wachttijd";
                    break;
                default:
                    lblCsvAnalyse.Text = "-";
                    break;
            }
        }

        /* private void testVLogCfgRead()
         {
             string filebase = tbDirConfiguraties.Text;
             string[] files = Directory.GetFiles(filebase);

             foreach (string f in files)
             {
                 byte[] dataVlogCfg = File.ReadAllBytes(f);
                 VLogCfg vc = new VLogCfg();
                 vc.ReadConfig(dataVlogCfg);
             }
         }*/
    }
}
