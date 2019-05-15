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
namespace VVE
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.btGetVLogData = new System.Windows.Forms.Button();
            this.dtpVanDatum = new System.Windows.Forms.DateTimePicker();
            this.dtpTotDatum = new System.Windows.Forms.DateTimePicker();
            this.dtpVanTijd = new System.Windows.Forms.DateTimePicker();
            this.dtpTotTijd = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbVris = new System.Windows.Forms.ComboBox();
            this.tbDirVLogArchief = new System.Windows.Forms.TextBox();
            this.tbDirConfiguraties = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbResultaat = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btAnalyseCSV = new System.Windows.Forms.Button();
            this.saveFileDialogVlgZip = new System.Windows.Forms.SaveFileDialog();
            this.cbInterval = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.nudTellenFilterDet = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.saveFileDialogCSV = new System.Windows.Forms.SaveFileDialog();
            this.btTestCfg = new System.Windows.Forms.Button();
            this.btAnalyseDPF = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.tbKmlConfiguratie = new System.Windows.Forms.TextBox();
            this.folderBrowserDialogDpfResult = new System.Windows.Forms.FolderBrowserDialog();
            this.btTestFindTimeRefs = new System.Windows.Forms.Button();
            this.btTestVLog = new System.Windows.Forms.Button();
            this.cbCsvFilterMaandag = new System.Windows.Forms.CheckBox();
            this.cbCsvFilterDinsdag = new System.Windows.Forms.CheckBox();
            this.cbCsvFilterWoensdag = new System.Windows.Forms.CheckBox();
            this.cbCsvFilterDonderdag = new System.Windows.Forms.CheckBox();
            this.cbCsvFilterVrijdag = new System.Windows.Forms.CheckBox();
            this.cbCsvFilterZaterdag = new System.Windows.Forms.CheckBox();
            this.cbCsvFilterZondag = new System.Windows.Forms.CheckBox();
            this.btOpenMetViewer = new System.Windows.Forms.Button();
            this.dtpPeriod1Van = new System.Windows.Forms.DateTimePicker();
            this.dtpPeriod1Tot = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.gbCsvWeekdagen = new System.Windows.Forms.GroupBox();
            this.gbCsvFilterPerioden = new System.Windows.Forms.GroupBox();
            this.cbCsvFilterPeriode2 = new System.Windows.Forms.CheckBox();
            this.dtpPeriod2Van = new System.Windows.Forms.DateTimePicker();
            this.cbCsvFilterPeriode1 = new System.Windows.Forms.CheckBox();
            this.dtpPeriod2Tot = new System.Windows.Forms.DateTimePicker();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cbTellenAlleenKoplussenCsv = new System.Windows.Forms.CheckBox();
            this.rbDagbestanden = new System.Windows.Forms.RadioButton();
            this.rbEenBestand = new System.Windows.Forms.RadioButton();
            this.gbVLogOpslaan = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.nudWachttijdBezettijdVerweglus = new System.Windows.Forms.NumericUpDown();
            this.nudWachttijdBezettijdKoplus = new System.Windows.Forms.NumericUpDown();
            this.nudWachttijdNormRijtijdVerweglus = new System.Windows.Forms.NumericUpDown();
            this.nudWachttijdMaxRijtijdVerweglus = new System.Windows.Forms.NumericUpDown();
            this.tcInstellingen = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cbWachttijdIndivFietsersCsv = new System.Windows.Forms.CheckBox();
            this.nudWachttijdIntrekkenAanvraag = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.tbDpfPostPass = new System.Windows.Forms.TextBox();
            this.tbDpfPostUser = new System.Windows.Forms.TextBox();
            this.tbDpfPostUrl = new System.Windows.Forms.TextBox();
            this.btPostNaarFietsViewer = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.nudDpfPostMaxSize = new System.Windows.Forms.NumericUpDown();
            this.cbDpfAnalyse = new System.Windows.Forms.ComboBox();
            this.cbDpfVriSelectie = new System.Windows.Forms.ComboBox();
            this.cbVlcFileNames = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.gbInstellingen = new System.Windows.Forms.GroupBox();
            this.tcAnalyses = new System.Windows.Forms.TabControl();
            this.tabCsv = new System.Windows.Forms.TabPage();
            this.lblCsvAnalyse = new System.Windows.Forms.Label();
            this.tabDpf = new System.Windows.Forms.TabPage();
            this.cbDpfAutoPost = new System.Windows.Forms.CheckBox();
            this.lblDpfPostLastDay = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.dtpAutoPostTime = new System.Windows.Forms.DateTimePicker();
            this.tabTools = new System.Windows.Forms.TabPage();
            this.autoPostTimer = new System.Windows.Forms.Timer(this.components);
            this.pbDenHaag = new System.Windows.Forms.PictureBox();
            this.pbClaassensSolutions = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudTellenFilterDet)).BeginInit();
            this.gbCsvWeekdagen.SuspendLayout();
            this.gbCsvFilterPerioden.SuspendLayout();
            this.gbVLogOpslaan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdBezettijdVerweglus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdBezettijdKoplus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdNormRijtijdVerweglus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdMaxRijtijdVerweglus)).BeginInit();
            this.tcInstellingen.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdIntrekkenAanvraag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDpfPostMaxSize)).BeginInit();
            this.gbInstellingen.SuspendLayout();
            this.tcAnalyses.SuspendLayout();
            this.tabCsv.SuspendLayout();
            this.tabDpf.SuspendLayout();
            this.tabTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDenHaag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClaassensSolutions)).BeginInit();
            this.SuspendLayout();
            // 
            // btGetVLogData
            // 
            this.btGetVLogData.Location = new System.Drawing.Point(7, 75);
            this.btGetVLogData.Margin = new System.Windows.Forms.Padding(4);
            this.btGetVLogData.Name = "btGetVLogData";
            this.btGetVLogData.Size = new System.Drawing.Size(277, 34);
            this.btGetVLogData.TabIndex = 3;
            this.btGetVLogData.Text = "V-Log data opslaan in ZIP bestand";
            this.btGetVLogData.UseVisualStyleBackColor = true;
            this.btGetVLogData.Click += new System.EventHandler(this.btGetVLogData_Click);
            // 
            // dtpVanDatum
            // 
            this.dtpVanDatum.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpVanDatum.Location = new System.Drawing.Point(93, 127);
            this.dtpVanDatum.Margin = new System.Windows.Forms.Padding(4);
            this.dtpVanDatum.Name = "dtpVanDatum";
            this.dtpVanDatum.Size = new System.Drawing.Size(153, 22);
            this.dtpVanDatum.TabIndex = 3;
            // 
            // dtpTotDatum
            // 
            this.dtpTotDatum.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTotDatum.Location = new System.Drawing.Point(93, 159);
            this.dtpTotDatum.Margin = new System.Windows.Forms.Padding(4);
            this.dtpTotDatum.Name = "dtpTotDatum";
            this.dtpTotDatum.Size = new System.Drawing.Size(153, 22);
            this.dtpTotDatum.TabIndex = 5;
            // 
            // dtpVanTijd
            // 
            this.dtpVanTijd.CustomFormat = "hh:mm";
            this.dtpVanTijd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpVanTijd.Location = new System.Drawing.Point(255, 127);
            this.dtpVanTijd.Margin = new System.Windows.Forms.Padding(4);
            this.dtpVanTijd.Name = "dtpVanTijd";
            this.dtpVanTijd.ShowUpDown = true;
            this.dtpVanTijd.Size = new System.Drawing.Size(129, 22);
            this.dtpVanTijd.TabIndex = 4;
            this.dtpVanTijd.Value = new System.DateTime(2018, 6, 13, 0, 0, 0, 0);
            // 
            // dtpTotTijd
            // 
            this.dtpTotTijd.CustomFormat = "hh:mm";
            this.dtpTotTijd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTotTijd.Location = new System.Drawing.Point(255, 159);
            this.dtpTotTijd.Margin = new System.Windows.Forms.Padding(4);
            this.dtpTotTijd.Name = "dtpTotTijd";
            this.dtpTotTijd.ShowUpDown = true;
            this.dtpTotTijd.Size = new System.Drawing.Size(129, 22);
            this.dtpTotTijd.TabIndex = 6;
            this.dtpTotTijd.Value = new System.DateTime(2018, 6, 13, 0, 0, 0, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 130);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Van:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 162);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tot:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(45, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(349, 31);
            this.label3.TabIndex = 3;
            this.label3.Text = "V-Log Verwerkings Eenheid";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 64);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "VRI:";
            // 
            // cbVris
            // 
            this.cbVris.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbVris.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbVris.FormattingEnabled = true;
            this.cbVris.Location = new System.Drawing.Point(93, 62);
            this.cbVris.Margin = new System.Windows.Forms.Padding(4);
            this.cbVris.Name = "cbVris";
            this.cbVris.Size = new System.Drawing.Size(292, 24);
            this.cbVris.TabIndex = 1;
            this.cbVris.SelectedIndexChanged += new System.EventHandler(this.cbVris_SelectedIndexChanged);
            // 
            // tbDirVLogArchief
            // 
            this.tbDirVLogArchief.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbDirVLogArchief.Location = new System.Drawing.Point(172, 612);
            this.tbDirVLogArchief.Margin = new System.Windows.Forms.Padding(4);
            this.tbDirVLogArchief.Name = "tbDirVLogArchief";
            this.tbDirVLogArchief.Size = new System.Drawing.Size(903, 22);
            this.tbDirVLogArchief.TabIndex = 12;
            // 
            // tbDirConfiguraties
            // 
            this.tbDirConfiguraties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbDirConfiguraties.Location = new System.Drawing.Point(172, 644);
            this.tbDirConfiguraties.Margin = new System.Windows.Forms.Padding(4);
            this.tbDirConfiguraties.Name = "tbDirConfiguraties";
            this.tbDirConfiguraties.Size = new System.Drawing.Size(903, 22);
            this.tbDirConfiguraties.TabIndex = 13;
            this.tbDirConfiguraties.TextChanged += new System.EventHandler(this.tbDirConfiguraties_TextChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 615);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "V-Log archief";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 647);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(151, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "Configuratiebestanden";
            // 
            // tbResultaat
            // 
            this.tbResultaat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResultaat.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbResultaat.Location = new System.Drawing.Point(93, 353);
            this.tbResultaat.Margin = new System.Windows.Forms.Padding(4);
            this.tbResultaat.MaxLength = 100000;
            this.tbResultaat.Multiline = true;
            this.tbResultaat.Name = "tbResultaat";
            this.tbResultaat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResultaat.Size = new System.Drawing.Size(1259, 251);
            this.tbResultaat.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 356);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Resultaat:";
            // 
            // btAnalyseCSV
            // 
            this.btAnalyseCSV.Location = new System.Drawing.Point(3, 242);
            this.btAnalyseCSV.Margin = new System.Windows.Forms.Padding(4);
            this.btAnalyseCSV.Name = "btAnalyseCSV";
            this.btAnalyseCSV.Size = new System.Drawing.Size(292, 30);
            this.btAnalyseCSV.TabIndex = 4;
            this.btAnalyseCSV.Text = "Uitvoeren naar bestand...";
            this.btAnalyseCSV.UseVisualStyleBackColor = true;
            this.btAnalyseCSV.Click += new System.EventHandler(this.btAnalyseCSV_Click);
            // 
            // saveFileDialogVlgZip
            // 
            this.saveFileDialogVlgZip.DefaultExt = "zip";
            this.saveFileDialogVlgZip.Filter = "ZIP files|*.zip";
            this.saveFileDialogVlgZip.Title = "VLog data opslaan";
            // 
            // cbInterval
            // 
            this.cbInterval.FormattingEnabled = true;
            this.cbInterval.Items.AddRange(new object[] {
            "1 min",
            "5 min",
            "15 min",
            "30 min",
            "1 uur",
            "1 dag"});
            this.cbInterval.Location = new System.Drawing.Point(136, 25);
            this.cbInterval.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbInterval.Name = "cbInterval";
            this.cbInterval.Size = new System.Drawing.Size(79, 24);
            this.cbInterval.TabIndex = 1;
            this.cbInterval.Text = "1 uur";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 27);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 17);
            this.label9.TabIndex = 2;
            this.label9.Text = "Interval analyses:";
            // 
            // nudTellenFilterDet
            // 
            this.nudTellenFilterDet.DecimalPlaces = 1;
            this.nudTellenFilterDet.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTellenFilterDet.Location = new System.Drawing.Point(165, 9);
            this.nudTellenFilterDet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudTellenFilterDet.Name = "nudTellenFilterDet";
            this.nudTellenFilterDet.Size = new System.Drawing.Size(79, 22);
            this.nudTellenFilterDet.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 11);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(157, 17);
            this.label10.TabIndex = 2;
            this.label10.Text = "Filter minimaal hoog [s]:";
            // 
            // saveFileDialogCSV
            // 
            this.saveFileDialogCSV.DefaultExt = "csv";
            this.saveFileDialogCSV.Filter = "CSV|*.csv";
            this.saveFileDialogCSV.Title = "Resultaat analyse opslaan";
            // 
            // btTestCfg
            // 
            this.btTestCfg.Location = new System.Drawing.Point(7, 9);
            this.btTestCfg.Margin = new System.Windows.Forms.Padding(4);
            this.btTestCfg.Name = "btTestCfg";
            this.btTestCfg.Size = new System.Drawing.Size(231, 31);
            this.btTestCfg.TabIndex = 1;
            this.btTestCfg.Text = "Test actuele configuraties";
            this.btTestCfg.UseVisualStyleBackColor = true;
            this.btTestCfg.Click += new System.EventHandler(this.btTestCfg_Click);
            // 
            // btAnalyseDPF
            // 
            this.btAnalyseDPF.Location = new System.Drawing.Point(7, 71);
            this.btAnalyseDPF.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btAnalyseDPF.Name = "btAnalyseDPF";
            this.btAnalyseDPF.Size = new System.Drawing.Size(195, 30);
            this.btAnalyseDPF.TabIndex = 3;
            this.btAnalyseDPF.Text = "Uitvoeren naar map...";
            this.btAnalyseDPF.UseVisualStyleBackColor = true;
            this.btAnalyseDPF.Click += new System.EventHandler(this.btAnalyseDPF_Click);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 679);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 17);
            this.label11.TabIndex = 2;
            this.label11.Text = "KML Configuratie";
            // 
            // tbKmlConfiguratie
            // 
            this.tbKmlConfiguratie.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbKmlConfiguratie.Location = new System.Drawing.Point(172, 676);
            this.tbKmlConfiguratie.Margin = new System.Windows.Forms.Padding(4);
            this.tbKmlConfiguratie.Name = "tbKmlConfiguratie";
            this.tbKmlConfiguratie.Size = new System.Drawing.Size(903, 22);
            this.tbKmlConfiguratie.TabIndex = 14;
            // 
            // btTestFindTimeRefs
            // 
            this.btTestFindTimeRefs.Location = new System.Drawing.Point(7, 48);
            this.btTestFindTimeRefs.Margin = new System.Windows.Forms.Padding(4);
            this.btTestFindTimeRefs.Name = "btTestFindTimeRefs";
            this.btTestFindTimeRefs.Size = new System.Drawing.Size(231, 28);
            this.btTestFindTimeRefs.TabIndex = 2;
            this.btTestFindTimeRefs.Text = "Test zoeken tijdreferenties";
            this.btTestFindTimeRefs.UseVisualStyleBackColor = true;
            this.btTestFindTimeRefs.Click += new System.EventHandler(this.btTestFindTimeRefs_Click);
            // 
            // btTestVLog
            // 
            this.btTestVLog.Location = new System.Drawing.Point(7, 84);
            this.btTestVLog.Margin = new System.Windows.Forms.Padding(4);
            this.btTestVLog.Name = "btTestVLog";
            this.btTestVLog.Size = new System.Drawing.Size(231, 28);
            this.btTestVLog.TabIndex = 3;
            this.btTestVLog.Text = "Test VLog parsing";
            this.btTestVLog.UseVisualStyleBackColor = true;
            this.btTestVLog.Click += new System.EventHandler(this.btTestVLog_Click);
            // 
            // cbCsvFilterMaandag
            // 
            this.cbCsvFilterMaandag.AutoSize = true;
            this.cbCsvFilterMaandag.Checked = true;
            this.cbCsvFilterMaandag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCsvFilterMaandag.Location = new System.Drawing.Point(7, 21);
            this.cbCsvFilterMaandag.Margin = new System.Windows.Forms.Padding(4);
            this.cbCsvFilterMaandag.Name = "cbCsvFilterMaandag";
            this.cbCsvFilterMaandag.Size = new System.Drawing.Size(89, 21);
            this.cbCsvFilterMaandag.TabIndex = 1;
            this.cbCsvFilterMaandag.Text = "maandag";
            this.cbCsvFilterMaandag.UseVisualStyleBackColor = true;
            // 
            // cbCsvFilterDinsdag
            // 
            this.cbCsvFilterDinsdag.AutoSize = true;
            this.cbCsvFilterDinsdag.Checked = true;
            this.cbCsvFilterDinsdag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCsvFilterDinsdag.Location = new System.Drawing.Point(7, 47);
            this.cbCsvFilterDinsdag.Margin = new System.Windows.Forms.Padding(4);
            this.cbCsvFilterDinsdag.Name = "cbCsvFilterDinsdag";
            this.cbCsvFilterDinsdag.Size = new System.Drawing.Size(80, 21);
            this.cbCsvFilterDinsdag.TabIndex = 2;
            this.cbCsvFilterDinsdag.Text = "dinsdag";
            this.cbCsvFilterDinsdag.UseVisualStyleBackColor = true;
            // 
            // cbCsvFilterWoensdag
            // 
            this.cbCsvFilterWoensdag.AutoSize = true;
            this.cbCsvFilterWoensdag.Checked = true;
            this.cbCsvFilterWoensdag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCsvFilterWoensdag.Location = new System.Drawing.Point(7, 73);
            this.cbCsvFilterWoensdag.Margin = new System.Windows.Forms.Padding(4);
            this.cbCsvFilterWoensdag.Name = "cbCsvFilterWoensdag";
            this.cbCsvFilterWoensdag.Size = new System.Drawing.Size(94, 21);
            this.cbCsvFilterWoensdag.TabIndex = 3;
            this.cbCsvFilterWoensdag.Text = "woensdag";
            this.cbCsvFilterWoensdag.UseVisualStyleBackColor = true;
            // 
            // cbCsvFilterDonderdag
            // 
            this.cbCsvFilterDonderdag.AutoSize = true;
            this.cbCsvFilterDonderdag.Checked = true;
            this.cbCsvFilterDonderdag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCsvFilterDonderdag.Location = new System.Drawing.Point(7, 98);
            this.cbCsvFilterDonderdag.Margin = new System.Windows.Forms.Padding(4);
            this.cbCsvFilterDonderdag.Name = "cbCsvFilterDonderdag";
            this.cbCsvFilterDonderdag.Size = new System.Drawing.Size(99, 21);
            this.cbCsvFilterDonderdag.TabIndex = 4;
            this.cbCsvFilterDonderdag.Text = "donderdag";
            this.cbCsvFilterDonderdag.UseVisualStyleBackColor = true;
            // 
            // cbCsvFilterVrijdag
            // 
            this.cbCsvFilterVrijdag.AutoSize = true;
            this.cbCsvFilterVrijdag.Checked = true;
            this.cbCsvFilterVrijdag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCsvFilterVrijdag.Location = new System.Drawing.Point(7, 124);
            this.cbCsvFilterVrijdag.Margin = new System.Windows.Forms.Padding(4);
            this.cbCsvFilterVrijdag.Name = "cbCsvFilterVrijdag";
            this.cbCsvFilterVrijdag.Size = new System.Drawing.Size(72, 21);
            this.cbCsvFilterVrijdag.TabIndex = 5;
            this.cbCsvFilterVrijdag.Text = "vrijdag";
            this.cbCsvFilterVrijdag.UseVisualStyleBackColor = true;
            // 
            // cbCsvFilterZaterdag
            // 
            this.cbCsvFilterZaterdag.AutoSize = true;
            this.cbCsvFilterZaterdag.Checked = true;
            this.cbCsvFilterZaterdag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCsvFilterZaterdag.Location = new System.Drawing.Point(7, 176);
            this.cbCsvFilterZaterdag.Margin = new System.Windows.Forms.Padding(4);
            this.cbCsvFilterZaterdag.Name = "cbCsvFilterZaterdag";
            this.cbCsvFilterZaterdag.Size = new System.Drawing.Size(86, 21);
            this.cbCsvFilterZaterdag.TabIndex = 7;
            this.cbCsvFilterZaterdag.Text = "zaterdag";
            this.cbCsvFilterZaterdag.UseVisualStyleBackColor = true;
            // 
            // cbCsvFilterZondag
            // 
            this.cbCsvFilterZondag.AutoSize = true;
            this.cbCsvFilterZondag.Checked = true;
            this.cbCsvFilterZondag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCsvFilterZondag.Location = new System.Drawing.Point(7, 150);
            this.cbCsvFilterZondag.Margin = new System.Windows.Forms.Padding(4);
            this.cbCsvFilterZondag.Name = "cbCsvFilterZondag";
            this.cbCsvFilterZondag.Size = new System.Drawing.Size(77, 21);
            this.cbCsvFilterZondag.TabIndex = 6;
            this.cbCsvFilterZondag.Text = "zondag";
            this.cbCsvFilterZondag.UseVisualStyleBackColor = true;
            // 
            // btOpenMetViewer
            // 
            this.btOpenMetViewer.Location = new System.Drawing.Point(93, 188);
            this.btOpenMetViewer.Margin = new System.Windows.Forms.Padding(4);
            this.btOpenMetViewer.Name = "btOpenMetViewer";
            this.btOpenMetViewer.Size = new System.Drawing.Size(292, 30);
            this.btOpenMetViewer.TabIndex = 7;
            this.btOpenMetViewer.Text = "Openen met viewer";
            this.btOpenMetViewer.UseVisualStyleBackColor = true;
            this.btOpenMetViewer.Click += new System.EventHandler(this.btOpenMetViewer_Click);
            // 
            // dtpPeriod1Van
            // 
            this.dtpPeriod1Van.CustomFormat = "hh:mm";
            this.dtpPeriod1Van.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpPeriod1Van.Location = new System.Drawing.Point(48, 44);
            this.dtpPeriod1Van.Margin = new System.Windows.Forms.Padding(4);
            this.dtpPeriod1Van.Name = "dtpPeriod1Van";
            this.dtpPeriod1Van.ShowUpDown = true;
            this.dtpPeriod1Van.Size = new System.Drawing.Size(88, 22);
            this.dtpPeriod1Van.TabIndex = 2;
            this.dtpPeriod1Van.Value = new System.DateTime(2018, 6, 13, 0, 0, 0, 0);
            // 
            // dtpPeriod1Tot
            // 
            this.dtpPeriod1Tot.CustomFormat = "hh:mm";
            this.dtpPeriod1Tot.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpPeriod1Tot.Location = new System.Drawing.Point(48, 69);
            this.dtpPeriod1Tot.Margin = new System.Windows.Forms.Padding(4);
            this.dtpPeriod1Tot.Name = "dtpPeriod1Tot";
            this.dtpPeriod1Tot.ShowUpDown = true;
            this.dtpPeriod1Tot.Size = new System.Drawing.Size(88, 22);
            this.dtpPeriod1Tot.TabIndex = 3;
            this.dtpPeriod1Tot.Value = new System.DateTime(2018, 6, 13, 0, 0, 0, 0);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 73);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(28, 17);
            this.label12.TabIndex = 2;
            this.label12.Text = "tot:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 48);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(35, 17);
            this.label13.TabIndex = 2;
            this.label13.Text = "van:";
            // 
            // gbCsvWeekdagen
            // 
            this.gbCsvWeekdagen.Controls.Add(this.cbCsvFilterMaandag);
            this.gbCsvWeekdagen.Controls.Add(this.cbCsvFilterDinsdag);
            this.gbCsvWeekdagen.Controls.Add(this.cbCsvFilterZondag);
            this.gbCsvWeekdagen.Controls.Add(this.cbCsvFilterWoensdag);
            this.gbCsvWeekdagen.Controls.Add(this.cbCsvFilterZaterdag);
            this.gbCsvWeekdagen.Controls.Add(this.cbCsvFilterDonderdag);
            this.gbCsvWeekdagen.Controls.Add(this.cbCsvFilterVrijdag);
            this.gbCsvWeekdagen.Location = new System.Drawing.Point(3, 33);
            this.gbCsvWeekdagen.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbCsvWeekdagen.Name = "gbCsvWeekdagen";
            this.gbCsvWeekdagen.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbCsvWeekdagen.Size = new System.Drawing.Size(140, 204);
            this.gbCsvWeekdagen.TabIndex = 2;
            this.gbCsvWeekdagen.TabStop = false;
            this.gbCsvWeekdagen.Text = "Filter weekdagen";
            // 
            // gbCsvFilterPerioden
            // 
            this.gbCsvFilterPerioden.Controls.Add(this.cbCsvFilterPeriode2);
            this.gbCsvFilterPerioden.Controls.Add(this.dtpPeriod2Van);
            this.gbCsvFilterPerioden.Controls.Add(this.cbCsvFilterPeriode1);
            this.gbCsvFilterPerioden.Controls.Add(this.dtpPeriod2Tot);
            this.gbCsvFilterPerioden.Controls.Add(this.dtpPeriod1Van);
            this.gbCsvFilterPerioden.Controls.Add(this.label15);
            this.gbCsvFilterPerioden.Controls.Add(this.dtpPeriod1Tot);
            this.gbCsvFilterPerioden.Controls.Add(this.label14);
            this.gbCsvFilterPerioden.Controls.Add(this.label12);
            this.gbCsvFilterPerioden.Controls.Add(this.label13);
            this.gbCsvFilterPerioden.Location = new System.Drawing.Point(151, 33);
            this.gbCsvFilterPerioden.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbCsvFilterPerioden.Name = "gbCsvFilterPerioden";
            this.gbCsvFilterPerioden.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbCsvFilterPerioden.Size = new System.Drawing.Size(145, 204);
            this.gbCsvFilterPerioden.TabIndex = 3;
            this.gbCsvFilterPerioden.TabStop = false;
            this.gbCsvFilterPerioden.Text = "Filter periode";
            // 
            // cbCsvFilterPeriode2
            // 
            this.cbCsvFilterPeriode2.AutoSize = true;
            this.cbCsvFilterPeriode2.Location = new System.Drawing.Point(5, 103);
            this.cbCsvFilterPeriode2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbCsvFilterPeriode2.Name = "cbCsvFilterPeriode2";
            this.cbCsvFilterPeriode2.Size = new System.Drawing.Size(65, 21);
            this.cbCsvFilterPeriode2.TabIndex = 4;
            this.cbCsvFilterPeriode2.Text = "Actief";
            this.cbCsvFilterPeriode2.UseVisualStyleBackColor = true;
            // 
            // dtpPeriod2Van
            // 
            this.dtpPeriod2Van.CustomFormat = "hh:mm";
            this.dtpPeriod2Van.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpPeriod2Van.Location = new System.Drawing.Point(48, 126);
            this.dtpPeriod2Van.Margin = new System.Windows.Forms.Padding(4);
            this.dtpPeriod2Van.Name = "dtpPeriod2Van";
            this.dtpPeriod2Van.ShowUpDown = true;
            this.dtpPeriod2Van.Size = new System.Drawing.Size(88, 22);
            this.dtpPeriod2Van.TabIndex = 5;
            this.dtpPeriod2Van.Value = new System.DateTime(2018, 6, 13, 0, 0, 0, 0);
            // 
            // cbCsvFilterPeriode1
            // 
            this.cbCsvFilterPeriode1.AutoSize = true;
            this.cbCsvFilterPeriode1.Location = new System.Drawing.Point(5, 21);
            this.cbCsvFilterPeriode1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbCsvFilterPeriode1.Name = "cbCsvFilterPeriode1";
            this.cbCsvFilterPeriode1.Size = new System.Drawing.Size(65, 21);
            this.cbCsvFilterPeriode1.TabIndex = 1;
            this.cbCsvFilterPeriode1.Text = "Actief";
            this.cbCsvFilterPeriode1.UseVisualStyleBackColor = true;
            // 
            // dtpPeriod2Tot
            // 
            this.dtpPeriod2Tot.CustomFormat = "hh:mm";
            this.dtpPeriod2Tot.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpPeriod2Tot.Location = new System.Drawing.Point(48, 151);
            this.dtpPeriod2Tot.Margin = new System.Windows.Forms.Padding(4);
            this.dtpPeriod2Tot.Name = "dtpPeriod2Tot";
            this.dtpPeriod2Tot.ShowUpDown = true;
            this.dtpPeriod2Tot.Size = new System.Drawing.Size(88, 22);
            this.dtpPeriod2Tot.TabIndex = 6;
            this.dtpPeriod2Tot.Value = new System.DateTime(2018, 6, 13, 0, 0, 0, 0);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 155);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(28, 17);
            this.label15.TabIndex = 2;
            this.label15.Text = "tot:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 130);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 17);
            this.label14.TabIndex = 2;
            this.label14.Text = "van:";
            // 
            // cbTellenAlleenKoplussenCsv
            // 
            this.cbTellenAlleenKoplussenCsv.AutoSize = true;
            this.cbTellenAlleenKoplussenCsv.Location = new System.Drawing.Point(5, 39);
            this.cbTellenAlleenKoplussenCsv.Margin = new System.Windows.Forms.Padding(4);
            this.cbTellenAlleenKoplussenCsv.Name = "cbTellenAlleenKoplussenCsv";
            this.cbTellenAlleenKoplussenCsv.Size = new System.Drawing.Size(220, 21);
            this.cbTellenAlleenKoplussenCsv.TabIndex = 2;
            this.cbTellenAlleenKoplussenCsv.Text = "Alleen koplussen (alleen CSV)";
            this.cbTellenAlleenKoplussenCsv.UseVisualStyleBackColor = true;
            // 
            // rbDagbestanden
            // 
            this.rbDagbestanden.AutoSize = true;
            this.rbDagbestanden.Checked = true;
            this.rbDagbestanden.Location = new System.Drawing.Point(5, 25);
            this.rbDagbestanden.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbDagbestanden.Name = "rbDagbestanden";
            this.rbDagbestanden.Size = new System.Drawing.Size(120, 21);
            this.rbDagbestanden.TabIndex = 1;
            this.rbDagbestanden.TabStop = true;
            this.rbDagbestanden.Text = "dagbestanden";
            this.rbDagbestanden.UseVisualStyleBackColor = true;
            // 
            // rbEenBestand
            // 
            this.rbEenBestand.AutoSize = true;
            this.rbEenBestand.Location = new System.Drawing.Point(5, 47);
            this.rbEenBestand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbEenBestand.Name = "rbEenBestand";
            this.rbEenBestand.Size = new System.Drawing.Size(108, 21);
            this.rbEenBestand.TabIndex = 2;
            this.rbEenBestand.Text = "één bestand";
            this.rbEenBestand.UseVisualStyleBackColor = true;
            // 
            // gbVLogOpslaan
            // 
            this.gbVLogOpslaan.Controls.Add(this.rbDagbestanden);
            this.gbVLogOpslaan.Controls.Add(this.rbEenBestand);
            this.gbVLogOpslaan.Controls.Add(this.btGetVLogData);
            this.gbVLogOpslaan.Location = new System.Drawing.Point(93, 226);
            this.gbVLogOpslaan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbVLogOpslaan.Name = "gbVLogOpslaan";
            this.gbVLogOpslaan.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbVLogOpslaan.Size = new System.Drawing.Size(292, 116);
            this.gbVLogOpslaan.TabIndex = 8;
            this.gbVLogOpslaan.TabStop = false;
            this.gbVLogOpslaan.Text = "V-Log opvragen";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 11);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(129, 17);
            this.label16.TabIndex = 17;
            this.label16.Text = "Bezettijd verweglus";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(5, 37);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(107, 17);
            this.label17.TabIndex = 17;
            this.label17.Text = "Bezettijd koplus";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(5, 62);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(146, 17);
            this.label18.TabIndex = 17;
            this.label18.Text = "Norm. rijtijd verweglus";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(5, 89);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(137, 17);
            this.label19.TabIndex = 17;
            this.label19.Text = "Max. rijtijd verweglus";
            // 
            // nudWachttijdBezettijdVerweglus
            // 
            this.nudWachttijdBezettijdVerweglus.DecimalPlaces = 1;
            this.nudWachttijdBezettijdVerweglus.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudWachttijdBezettijdVerweglus.Location = new System.Drawing.Point(159, 9);
            this.nudWachttijdBezettijdVerweglus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudWachttijdBezettijdVerweglus.Name = "nudWachttijdBezettijdVerweglus";
            this.nudWachttijdBezettijdVerweglus.Size = new System.Drawing.Size(79, 22);
            this.nudWachttijdBezettijdVerweglus.TabIndex = 1;
            // 
            // nudWachttijdBezettijdKoplus
            // 
            this.nudWachttijdBezettijdKoplus.DecimalPlaces = 1;
            this.nudWachttijdBezettijdKoplus.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudWachttijdBezettijdKoplus.Location = new System.Drawing.Point(159, 34);
            this.nudWachttijdBezettijdKoplus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudWachttijdBezettijdKoplus.Name = "nudWachttijdBezettijdKoplus";
            this.nudWachttijdBezettijdKoplus.Size = new System.Drawing.Size(79, 22);
            this.nudWachttijdBezettijdKoplus.TabIndex = 2;
            // 
            // nudWachttijdNormRijtijdVerweglus
            // 
            this.nudWachttijdNormRijtijdVerweglus.DecimalPlaces = 1;
            this.nudWachttijdNormRijtijdVerweglus.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudWachttijdNormRijtijdVerweglus.Location = new System.Drawing.Point(159, 60);
            this.nudWachttijdNormRijtijdVerweglus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudWachttijdNormRijtijdVerweglus.Name = "nudWachttijdNormRijtijdVerweglus";
            this.nudWachttijdNormRijtijdVerweglus.Size = new System.Drawing.Size(79, 22);
            this.nudWachttijdNormRijtijdVerweglus.TabIndex = 3;
            // 
            // nudWachttijdMaxRijtijdVerweglus
            // 
            this.nudWachttijdMaxRijtijdVerweglus.DecimalPlaces = 1;
            this.nudWachttijdMaxRijtijdVerweglus.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudWachttijdMaxRijtijdVerweglus.Location = new System.Drawing.Point(159, 86);
            this.nudWachttijdMaxRijtijdVerweglus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudWachttijdMaxRijtijdVerweglus.Name = "nudWachttijdMaxRijtijdVerweglus";
            this.nudWachttijdMaxRijtijdVerweglus.Size = new System.Drawing.Size(79, 22);
            this.nudWachttijdMaxRijtijdVerweglus.TabIndex = 4;
            // 
            // tcInstellingen
            // 
            this.tcInstellingen.Controls.Add(this.tabPage1);
            this.tcInstellingen.Controls.Add(this.tabPage2);
            this.tcInstellingen.Location = new System.Drawing.Point(5, 52);
            this.tcInstellingen.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tcInstellingen.Name = "tcInstellingen";
            this.tcInstellingen.SelectedIndex = 0;
            this.tcInstellingen.Size = new System.Drawing.Size(261, 236);
            this.tcInstellingen.TabIndex = 2;
            this.tcInstellingen.SelectedIndexChanged += new System.EventHandler(this.tcInstellingen_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbTellenAlleenKoplussenCsv);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.nudTellenFilterDet);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Size = new System.Drawing.Size(253, 207);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tellen";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cbWachttijdIndivFietsersCsv);
            this.tabPage2.Controls.Add(this.nudWachttijdIntrekkenAanvraag);
            this.tabPage2.Controls.Add(this.nudWachttijdMaxRijtijdVerweglus);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.nudWachttijdNormRijtijdVerweglus);
            this.tabPage2.Controls.Add(this.nudWachttijdBezettijdKoplus);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.nudWachttijdBezettijdVerweglus);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.label20);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage2.Size = new System.Drawing.Size(253, 207);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Wachttijd";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cbWachttijdIndivFietsersCsv
            // 
            this.cbWachttijdIndivFietsersCsv.Location = new System.Drawing.Point(5, 142);
            this.cbWachttijdIndivFietsersCsv.Margin = new System.Windows.Forms.Padding(4);
            this.cbWachttijdIndivFietsersCsv.Name = "cbWachttijdIndivFietsersCsv";
            this.cbWachttijdIndivFietsersCsv.Size = new System.Drawing.Size(241, 38);
            this.cbWachttijdIndivFietsersCsv.TabIndex = 6;
            this.cbWachttijdIndivFietsersCsv.Text = "Individuele fietsers in uitvoer (alleen CSV)";
            this.cbWachttijdIndivFietsersCsv.UseVisualStyleBackColor = true;
            // 
            // nudWachttijdIntrekkenAanvraag
            // 
            this.nudWachttijdIntrekkenAanvraag.DecimalPlaces = 1;
            this.nudWachttijdIntrekkenAanvraag.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudWachttijdIntrekkenAanvraag.Location = new System.Drawing.Point(159, 112);
            this.nudWachttijdIntrekkenAanvraag.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudWachttijdIntrekkenAanvraag.Name = "nudWachttijdIntrekkenAanvraag";
            this.nudWachttijdIntrekkenAanvraag.Size = new System.Drawing.Size(79, 22);
            this.nudWachttijdIntrekkenAanvraag.TabIndex = 5;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(5, 114);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(130, 17);
            this.label20.TabIndex = 17;
            this.label20.Text = "Intrekken aanvraag";
            // 
            // tbDpfPostPass
            // 
            this.tbDpfPostPass.Location = new System.Drawing.Point(128, 172);
            this.tbDpfPostPass.Margin = new System.Windows.Forms.Padding(4);
            this.tbDpfPostPass.Name = "tbDpfPostPass";
            this.tbDpfPostPass.Size = new System.Drawing.Size(272, 22);
            this.tbDpfPostPass.TabIndex = 6;
            // 
            // tbDpfPostUser
            // 
            this.tbDpfPostUser.Location = new System.Drawing.Point(128, 142);
            this.tbDpfPostUser.Margin = new System.Windows.Forms.Padding(4);
            this.tbDpfPostUser.Name = "tbDpfPostUser";
            this.tbDpfPostUser.Size = new System.Drawing.Size(272, 22);
            this.tbDpfPostUser.TabIndex = 5;
            // 
            // tbDpfPostUrl
            // 
            this.tbDpfPostUrl.Location = new System.Drawing.Point(128, 112);
            this.tbDpfPostUrl.Margin = new System.Windows.Forms.Padding(4);
            this.tbDpfPostUrl.Name = "tbDpfPostUrl";
            this.tbDpfPostUrl.Size = new System.Drawing.Size(272, 22);
            this.tbDpfPostUrl.TabIndex = 4;
            // 
            // btPostNaarFietsViewer
            // 
            this.btPostNaarFietsViewer.Location = new System.Drawing.Point(7, 228);
            this.btPostNaarFietsViewer.Margin = new System.Windows.Forms.Padding(4);
            this.btPostNaarFietsViewer.Name = "btPostNaarFietsViewer";
            this.btPostNaarFietsViewer.Size = new System.Drawing.Size(393, 30);
            this.btPostNaarFietsViewer.TabIndex = 8;
            this.btPostNaarFietsViewer.Text = "UIitvoeren en posten";
            this.btPostNaarFietsViewer.UseVisualStyleBackColor = true;
            this.btPostNaarFietsViewer.Click += new System.EventHandler(this.btPostNaarFietsViewer_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(7, 43);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(62, 17);
            this.label27.TabIndex = 2;
            this.label27.Text = "Analyse:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(7, 10);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(34, 17);
            this.label25.TabIndex = 2;
            this.label25.Text = "VRI:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(260, 202);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(24, 17);
            this.label26.TabIndex = 2;
            this.label26.Text = "kB";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(7, 202);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(112, 17);
            this.label24.TabIndex = 2;
            this.label24.Text = "Max. POST size:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(7, 175);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(90, 17);
            this.label23.TabIndex = 2;
            this.label23.Text = "Wachtwoord:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(7, 145);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(75, 17);
            this.label22.TabIndex = 2;
            this.label22.Text = "Gebruiker:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(7, 114);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(40, 17);
            this.label21.TabIndex = 2;
            this.label21.Text = "URL:";
            // 
            // nudDpfPostMaxSize
            // 
            this.nudDpfPostMaxSize.Location = new System.Drawing.Point(128, 199);
            this.nudDpfPostMaxSize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudDpfPostMaxSize.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudDpfPostMaxSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDpfPostMaxSize.Name = "nudDpfPostMaxSize";
            this.nudDpfPostMaxSize.Size = new System.Drawing.Size(125, 22);
            this.nudDpfPostMaxSize.TabIndex = 7;
            this.nudDpfPostMaxSize.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // cbDpfAnalyse
            // 
            this.cbDpfAnalyse.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbDpfAnalyse.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbDpfAnalyse.FormattingEnabled = true;
            this.cbDpfAnalyse.Items.AddRange(new object[] {
            "tellen",
            "wachttijd",
            "alle"});
            this.cbDpfAnalyse.Location = new System.Drawing.Point(77, 39);
            this.cbDpfAnalyse.Margin = new System.Windows.Forms.Padding(4);
            this.cbDpfAnalyse.Name = "cbDpfAnalyse";
            this.cbDpfAnalyse.Size = new System.Drawing.Size(124, 24);
            this.cbDpfAnalyse.TabIndex = 2;
            this.cbDpfAnalyse.Text = "alle";
            // 
            // cbDpfVriSelectie
            // 
            this.cbDpfVriSelectie.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbDpfVriSelectie.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbDpfVriSelectie.FormattingEnabled = true;
            this.cbDpfVriSelectie.Items.AddRange(new object[] {
            "geselecteerde",
            "alle"});
            this.cbDpfVriSelectie.Location = new System.Drawing.Point(77, 7);
            this.cbDpfVriSelectie.Margin = new System.Windows.Forms.Padding(4);
            this.cbDpfVriSelectie.Name = "cbDpfVriSelectie";
            this.cbDpfVriSelectie.Size = new System.Drawing.Size(124, 24);
            this.cbDpfVriSelectie.TabIndex = 1;
            // 
            // cbVlcFileNames
            // 
            this.cbVlcFileNames.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbVlcFileNames.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbVlcFileNames.FormattingEnabled = true;
            this.cbVlcFileNames.Location = new System.Drawing.Point(93, 94);
            this.cbVlcFileNames.Margin = new System.Windows.Forms.Padding(4);
            this.cbVlcFileNames.Name = "cbVlcFileNames";
            this.cbVlcFileNames.Size = new System.Drawing.Size(292, 24);
            this.cbVlcFileNames.TabIndex = 2;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(48, 97);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(38, 17);
            this.label28.TabIndex = 2;
            this.label28.Text = "VLC:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(7, 6);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(62, 17);
            this.label29.TabIndex = 2;
            this.label29.Text = "Analyse:";
            // 
            // gbInstellingen
            // 
            this.gbInstellingen.Controls.Add(this.label9);
            this.gbInstellingen.Controls.Add(this.cbInterval);
            this.gbInstellingen.Controls.Add(this.tcInstellingen);
            this.gbInstellingen.Location = new System.Drawing.Point(391, 53);
            this.gbInstellingen.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbInstellingen.Name = "gbInstellingen";
            this.gbInstellingen.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbInstellingen.Size = new System.Drawing.Size(273, 293);
            this.gbInstellingen.TabIndex = 9;
            this.gbInstellingen.TabStop = false;
            this.gbInstellingen.Text = "Instellingen";
            // 
            // tcAnalyses
            // 
            this.tcAnalyses.Controls.Add(this.tabCsv);
            this.tcAnalyses.Controls.Add(this.tabDpf);
            this.tcAnalyses.Controls.Add(this.tabTools);
            this.tcAnalyses.Location = new System.Drawing.Point(669, 36);
            this.tcAnalyses.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tcAnalyses.Name = "tcAnalyses";
            this.tcAnalyses.SelectedIndex = 0;
            this.tcAnalyses.Size = new System.Drawing.Size(685, 310);
            this.tcAnalyses.TabIndex = 10;
            // 
            // tabCsv
            // 
            this.tabCsv.Controls.Add(this.btAnalyseCSV);
            this.tabCsv.Controls.Add(this.lblCsvAnalyse);
            this.tabCsv.Controls.Add(this.label29);
            this.tabCsv.Controls.Add(this.gbCsvWeekdagen);
            this.tabCsv.Controls.Add(this.gbCsvFilterPerioden);
            this.tabCsv.Location = new System.Drawing.Point(4, 25);
            this.tabCsv.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabCsv.Name = "tabCsv";
            this.tabCsv.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabCsv.Size = new System.Drawing.Size(677, 281);
            this.tabCsv.TabIndex = 0;
            this.tabCsv.Text = "CSV";
            this.tabCsv.UseVisualStyleBackColor = true;
            // 
            // lblCsvAnalyse
            // 
            this.lblCsvAnalyse.AutoSize = true;
            this.lblCsvAnalyse.Location = new System.Drawing.Point(77, 6);
            this.lblCsvAnalyse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCsvAnalyse.Name = "lblCsvAnalyse";
            this.lblCsvAnalyse.Size = new System.Drawing.Size(13, 17);
            this.lblCsvAnalyse.TabIndex = 2;
            this.lblCsvAnalyse.Text = "-";
            // 
            // tabDpf
            // 
            this.tabDpf.Controls.Add(this.cbDpfAutoPost);
            this.tabDpf.Controls.Add(this.tbDpfPostPass);
            this.tabDpf.Controls.Add(this.label25);
            this.tabDpf.Controls.Add(this.tbDpfPostUser);
            this.tabDpf.Controls.Add(this.cbDpfVriSelectie);
            this.tabDpf.Controls.Add(this.tbDpfPostUrl);
            this.tabDpf.Controls.Add(this.cbDpfAnalyse);
            this.tabDpf.Controls.Add(this.btPostNaarFietsViewer);
            this.tabDpf.Controls.Add(this.nudDpfPostMaxSize);
            this.tabDpf.Controls.Add(this.btAnalyseDPF);
            this.tabDpf.Controls.Add(this.lblDpfPostLastDay);
            this.tabDpf.Controls.Add(this.label31);
            this.tabDpf.Controls.Add(this.label30);
            this.tabDpf.Controls.Add(this.label21);
            this.tabDpf.Controls.Add(this.label27);
            this.tabDpf.Controls.Add(this.label22);
            this.tabDpf.Controls.Add(this.label23);
            this.tabDpf.Controls.Add(this.label26);
            this.tabDpf.Controls.Add(this.label24);
            this.tabDpf.Controls.Add(this.dtpAutoPostTime);
            this.tabDpf.Location = new System.Drawing.Point(4, 25);
            this.tabDpf.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabDpf.Name = "tabDpf";
            this.tabDpf.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabDpf.Size = new System.Drawing.Size(677, 281);
            this.tabDpf.TabIndex = 1;
            this.tabDpf.Text = "DPF";
            this.tabDpf.UseVisualStyleBackColor = true;
            // 
            // cbDpfAutoPost
            // 
            this.cbDpfAutoPost.AutoSize = true;
            this.cbDpfAutoPost.Location = new System.Drawing.Point(408, 176);
            this.cbDpfAutoPost.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbDpfAutoPost.Name = "cbDpfAutoPost";
            this.cbDpfAutoPost.Size = new System.Drawing.Size(262, 21);
            this.cbDpfAutoPost.TabIndex = 9;
            this.cbDpfAutoPost.Text = "Automatisch posten voorgaande dag";
            this.cbDpfAutoPost.UseVisualStyleBackColor = true;
            this.cbDpfAutoPost.CheckedChanged += new System.EventHandler(this.cbAutoPost_CheckedChanged);
            // 
            // lblDpfPostLastDay
            // 
            this.lblDpfPostLastDay.AutoSize = true;
            this.lblDpfPostLastDay.Location = new System.Drawing.Point(548, 239);
            this.lblDpfPostLastDay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDpfPostLastDay.Name = "lblDpfPostLastDay";
            this.lblDpfPostLastDay.Size = new System.Drawing.Size(13, 17);
            this.lblDpfPostLastDay.TabIndex = 2;
            this.lblDpfPostLastDay.Text = "-";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(405, 239);
            this.label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(134, 17);
            this.label31.TabIndex = 2;
            this.label31.Text = "Laatst geposte dag:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(405, 208);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(57, 17);
            this.label30.TabIndex = 2;
            this.label30.Text = "Tijdstip:";
            // 
            // dtpAutoPostTime
            // 
            this.dtpAutoPostTime.CustomFormat = "HH:mm";
            this.dtpAutoPostTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAutoPostTime.Location = new System.Drawing.Point(469, 204);
            this.dtpAutoPostTime.Margin = new System.Windows.Forms.Padding(4);
            this.dtpAutoPostTime.Name = "dtpAutoPostTime";
            this.dtpAutoPostTime.ShowUpDown = true;
            this.dtpAutoPostTime.Size = new System.Drawing.Size(129, 22);
            this.dtpAutoPostTime.TabIndex = 10;
            this.dtpAutoPostTime.Value = new System.DateTime(2019, 1, 22, 1, 30, 0, 0);
            // 
            // tabTools
            // 
            this.tabTools.Controls.Add(this.btTestCfg);
            this.tabTools.Controls.Add(this.btTestVLog);
            this.tabTools.Controls.Add(this.btTestFindTimeRefs);
            this.tabTools.Location = new System.Drawing.Point(4, 25);
            this.tabTools.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabTools.Name = "tabTools";
            this.tabTools.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabTools.Size = new System.Drawing.Size(677, 281);
            this.tabTools.TabIndex = 2;
            this.tabTools.Text = "Tools";
            this.tabTools.UseVisualStyleBackColor = true;
            // 
            // autoPostTimer
            // 
            this.autoPostTimer.Interval = 2000;
            this.autoPostTimer.Tick += new System.EventHandler(this.minutesTimer_Tick);
            // 
            // pbDenHaag
            // 
            this.pbDenHaag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbDenHaag.Image = global::VVE.Properties.Resources.DHKlein;
            this.pbDenHaag.Location = new System.Drawing.Point(1195, 1);
            this.pbDenHaag.Margin = new System.Windows.Forms.Padding(4);
            this.pbDenHaag.Name = "pbDenHaag";
            this.pbDenHaag.Size = new System.Drawing.Size(159, 55);
            this.pbDenHaag.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbDenHaag.TabIndex = 16;
            this.pbDenHaag.TabStop = false;
            // 
            // pbClaassensSolutions
            // 
            this.pbClaassensSolutions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbClaassensSolutions.Image = global::VVE.Properties.Resources.CSKlein;
            this.pbClaassensSolutions.Location = new System.Drawing.Point(1124, 655);
            this.pbClaassensSolutions.Margin = new System.Windows.Forms.Padding(4);
            this.pbClaassensSolutions.Name = "pbClaassensSolutions";
            this.pbClaassensSolutions.Size = new System.Drawing.Size(243, 41);
            this.pbClaassensSolutions.TabIndex = 15;
            this.pbClaassensSolutions.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1371, 706);
            this.Controls.Add(this.pbDenHaag);
            this.Controls.Add(this.pbClaassensSolutions);
            this.Controls.Add(this.tcAnalyses);
            this.Controls.Add(this.gbInstellingen);
            this.Controls.Add(this.gbVLogOpslaan);
            this.Controls.Add(this.btOpenMetViewer);
            this.Controls.Add(this.tbResultaat);
            this.Controls.Add(this.tbKmlConfiguratie);
            this.Controls.Add(this.tbDirConfiguraties);
            this.Controls.Add(this.tbDirVLogArchief);
            this.Controls.Add(this.cbVlcFileNames);
            this.Controls.Add(this.cbVris);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpTotTijd);
            this.Controls.Add(this.dtpVanTijd);
            this.Controls.Add(this.dtpTotDatum);
            this.Controls.Add(this.dtpVanDatum);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1386, 742);
            this.Name = "Main";
            this.Text = "VVE versie 1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudTellenFilterDet)).EndInit();
            this.gbCsvWeekdagen.ResumeLayout(false);
            this.gbCsvWeekdagen.PerformLayout();
            this.gbCsvFilterPerioden.ResumeLayout(false);
            this.gbCsvFilterPerioden.PerformLayout();
            this.gbVLogOpslaan.ResumeLayout(false);
            this.gbVLogOpslaan.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdBezettijdVerweglus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdBezettijdKoplus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdNormRijtijdVerweglus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdMaxRijtijdVerweglus)).EndInit();
            this.tcInstellingen.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWachttijdIntrekkenAanvraag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDpfPostMaxSize)).EndInit();
            this.gbInstellingen.ResumeLayout(false);
            this.gbInstellingen.PerformLayout();
            this.tcAnalyses.ResumeLayout(false);
            this.tabCsv.ResumeLayout(false);
            this.tabCsv.PerformLayout();
            this.tabDpf.ResumeLayout(false);
            this.tabDpf.PerformLayout();
            this.tabTools.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDenHaag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClaassensSolutions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btGetVLogData;
        private System.Windows.Forms.DateTimePicker dtpVanDatum;
        private System.Windows.Forms.DateTimePicker dtpTotDatum;
        private System.Windows.Forms.DateTimePicker dtpVanTijd;
        private System.Windows.Forms.DateTimePicker dtpTotTijd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbVris;
        private System.Windows.Forms.TextBox tbDirVLogArchief;
        private System.Windows.Forms.TextBox tbDirConfiguraties;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbResultaat;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btAnalyseCSV;
        private System.Windows.Forms.SaveFileDialog saveFileDialogVlgZip;
        private System.Windows.Forms.ComboBox cbInterval;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nudTellenFilterDet;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.SaveFileDialog saveFileDialogCSV;
        private System.Windows.Forms.Button btTestCfg;
        private System.Windows.Forms.Button btAnalyseDPF;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbKmlConfiguratie;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogDpfResult;
        private System.Windows.Forms.Button btTestFindTimeRefs;
        private System.Windows.Forms.Button btTestVLog;
        private System.Windows.Forms.CheckBox cbCsvFilterMaandag;
        private System.Windows.Forms.CheckBox cbCsvFilterDinsdag;
        private System.Windows.Forms.CheckBox cbCsvFilterWoensdag;
        private System.Windows.Forms.CheckBox cbCsvFilterDonderdag;
        private System.Windows.Forms.CheckBox cbCsvFilterVrijdag;
        private System.Windows.Forms.CheckBox cbCsvFilterZaterdag;
        private System.Windows.Forms.CheckBox cbCsvFilterZondag;
        private System.Windows.Forms.Button btOpenMetViewer;
        private System.Windows.Forms.DateTimePicker dtpPeriod1Van;
        private System.Windows.Forms.DateTimePicker dtpPeriod1Tot;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox gbCsvWeekdagen;
        private System.Windows.Forms.GroupBox gbCsvFilterPerioden;
        private System.Windows.Forms.CheckBox cbCsvFilterPeriode1;
        private System.Windows.Forms.CheckBox cbCsvFilterPeriode2;
        private System.Windows.Forms.DateTimePicker dtpPeriod2Van;
        private System.Windows.Forms.DateTimePicker dtpPeriod2Tot;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cbTellenAlleenKoplussenCsv;
        private System.Windows.Forms.RadioButton rbDagbestanden;
        private System.Windows.Forms.RadioButton rbEenBestand;
        private System.Windows.Forms.GroupBox gbVLogOpslaan;
        private System.Windows.Forms.NumericUpDown nudWachttijdMaxRijtijdVerweglus;
        private System.Windows.Forms.NumericUpDown nudWachttijdNormRijtijdVerweglus;
        private System.Windows.Forms.NumericUpDown nudWachttijdBezettijdKoplus;
        private System.Windows.Forms.NumericUpDown nudWachttijdBezettijdVerweglus;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabControl tcInstellingen;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown nudWachttijdIntrekkenAanvraag;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.CheckBox cbWachttijdIndivFietsersCsv;
        private System.Windows.Forms.TextBox tbDpfPostPass;
        private System.Windows.Forms.TextBox tbDpfPostUser;
        private System.Windows.Forms.TextBox tbDpfPostUrl;
        private System.Windows.Forms.Button btPostNaarFietsViewer;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.NumericUpDown nudDpfPostMaxSize;
        private System.Windows.Forms.ComboBox cbDpfVriSelectie;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.ComboBox cbDpfAnalyse;
        private System.Windows.Forms.ComboBox cbVlcFileNames;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.GroupBox gbInstellingen;
        private System.Windows.Forms.TabControl tcAnalyses;
        private System.Windows.Forms.TabPage tabCsv;
        private System.Windows.Forms.TabPage tabDpf;
        private System.Windows.Forms.TabPage tabTools;
        private System.Windows.Forms.CheckBox cbDpfAutoPost;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.DateTimePicker dtpAutoPostTime;
        private System.Windows.Forms.Timer autoPostTimer;
        private System.Windows.Forms.Label lblDpfPostLastDay;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.PictureBox pbClaassensSolutions;
        private System.Windows.Forms.PictureBox pbDenHaag;
        private System.Windows.Forms.Label lblCsvAnalyse;
    }
}

