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
using System.Text.RegularExpressions;

namespace VVE
{
    /// <summary>
    /// Bevat alle VLog Configuratie data aanwezig in een map
    /// </summary>
    public class VLogCfgDir
    {
        public string Dir { get; private set; } = "";
        public Dictionary<string, VlogCfgVri> Vris = new Dictionary<string, VlogCfgVri>(); //<kruispuntnummer,configuraties>

        /// <summary>
        /// Inlezen van alle vlc files en sorteren per VRI nummer. Per VRI wordt de voorkeur configuratie opgeslagen als eerste item in de lijst.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>true=volledig ingelezen, false=error</returns>
        public bool ReadFiles(string dir)
        {
            Dir = dir.Trim();
            Vris.Clear();

            if (Directory.Exists(Dir))
            {
                try
                {
                    Dictionary<string, List<VlogCfgFile>> vris = new Dictionary<string, List<VlogCfgFile>>(); //<kruispuntnummer,configuraties>

                    //bestanden inlezen en sorteren naar VRI nummer
                    string[] files = Directory.GetFiles(dir, "*.vlc", SearchOption.TopDirectoryOnly);
                    Array.Sort(files);
                    foreach (string f in files)
                    {
                        string fn = Path.GetFileNameWithoutExtension(f);
                        //Regex regex = new Regex(@"^K\d\d\dcfg$"); //regex voor Kxxxcfg.vlc files
                        Regex regexValidVlc = new Regex(@"^K\d\d\dcfg.*$"); //regex voor Kxxxcfg*.vlc files
                        Match matchIsValidVlc = regexValidVlc.Match(fn);
                        if (matchIsValidVlc.Success)
                        {
                            //inlezen data en kruispuntnaam
                            string vriNr = fn.Substring(1, 3).TrimStart('0');
                            byte[] data = File.ReadAllBytes(f);

                            //vlog configuratie opslaan onder kruispuntnummer
                            VlogCfgFile vlc = new VlogCfgFile() { Name = fn, FileName = f, Data = data };
                            if (!vris.ContainsKey(vriNr)) vris.Add(vriNr, new List<VlogCfgFile>());
                            vris[vriNr].Add(vlc);
                        }
                    }

                    //per VRI bepalen welk bestand de voorkeur heeft
                    foreach (KeyValuePair<string, List<VlogCfgFile>> kvp in vris)
                    {
                        string vriNr = kvp.Key;
                        List<VlogCfgFile> vlcs = kvp.Value;

                        List<VlogCfgFile> sortedList = new List<VlogCfgFile>();
                        bool preferredFound = false;
                        foreach (VlogCfgFile vlc in vlcs)
                        {
                            Regex regexPreferredVlc = new Regex(@"^K\d\d\dcfg(_imflow)?$"); //regex voor Kxxxcfg.vlc files en Kxxxcfg_imflow.vlc
                            Match matchIsPreferredVlc = regexPreferredVlc.Match(vlc.Name);

                            if (matchIsPreferredVlc.Success && !preferredFound)
                            {
                                //voorkeur config altijd op index 0
                                sortedList.Insert(0, vlc);
                                preferredFound = true;
                            }
                            else
                            {
                                sortedList.Add(vlc);
                            }
                        }

                        Vris.Add(vriNr, new VlogCfgVri() { Configs = sortedList.ToArray() });
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Vris.Clear();
                    // MessageBox.Show("Fout bij inlezen configuratiebestanden: " + ex.Message);
                }
            }
            else
            {
            }
            return false;
        }

        public Dictionary<string, byte[]> GetDefaults()
        {
            Dictionary<string, byte[]> configuraties = new Dictionary<string, byte[]>();

            List<string> nrs = Vris.Keys.ToList();
            NumberComparer sorter = new NumberComparer();
            nrs.Sort(sorter);

            foreach (string nr in nrs)
            {
                if (Vris[nr].Configs.Length > 0)
                {
                    configuraties.Add(nr, Vris[nr].Configs[0].Data);
                }
            }

            return configuraties;
        }

        /// <summary>
        /// Configuratie van een specifieke VRI met eventueel een specifieke configuratie
        /// </summary>
        /// <param name="vri"></param>
        /// <param name="vlcName"></param>
        /// <returns></returns>
        public Dictionary<string, byte[]> GetSpecific(string vri, string vlcName)
        {
            Dictionary<string, byte[]> configuraties = new Dictionary<string, byte[]>();

            if(Vris.ContainsKey(vri))
            {
                if (vlcName == "") return null;
                else
                {
                    //specifieke config
                    foreach (VlogCfgFile vlc in Vris[vri].Configs)
                    {
                        if(vlc.Name==vlcName)
                        {
                            configuraties.Add(vri, vlc.Data);
                        }
                    }
                }
            }

            return configuraties;
        }

        private class NumberComparer : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                int int1 = int.Parse(a);
                int int2 = int.Parse(b);
                return int1.CompareTo(int2);
            }
        }
    }

    public class VlogCfgVri
    {
        public VlogCfgFile[] Configs = new VlogCfgFile[0];
    }

    public class VlogCfgFile
    {
        public string Name = ""; //bestandsnaam zonder extensie
        public string FileName = ""; //volledige bestandsnaam
        public byte[] Data = new byte[0];
    }
}
