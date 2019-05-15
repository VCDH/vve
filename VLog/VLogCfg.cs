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

namespace VVE.VLog
{
    public class VLogCfg
    {
        private enum State { waitForHeader, readingBody, ended };

        public bool Valid = false;
        public string ErrorText = "";
        public string VLogVersie = "";
        public string Sys = "";
        public VLogCfgItem[] Dp = new VLogCfgItem[0];
        public VLogCfgItem[] Is = new VLogCfgItem[0];
        public VLogCfgItem[] Fc = new VLogCfgItem[0];
        public VLogCfgItem[] Us = new VLogCfgItem[0];

        private void reset()
        {
            Valid = false;
            VLogVersie = "";
            Sys = "";
            Dp = new VLogCfgItem[0];
            Is = new VLogCfgItem[0];
            Fc = new VLogCfgItem[0];
            Us = new VLogCfgItem[0];
        }

        /// <summary>
        /// Leest V-Log Configuratiebestand in
        /// </summary>
        /// <param name="data"></param>
        /// <returns>True=correct ingelezen, False=fout bij inlezen</returns>
        public bool ReadConfig(byte[] data)
        {
            State status = State.waitForHeader;

            reset();
            bool error = false;

            List<VLogCfgItem> DpItems = new List<VLogCfgItem>();
            List<VLogCfgItem> IsItems = new List<VLogCfgItem>();
            List<VLogCfgItem> FcItems = new List<VLogCfgItem>();
            List<VLogCfgItem> UsItems = new List<VLogCfgItem>();

            try
            {
                using (var memoryStream = new MemoryStream(data))
                {
                    using (StreamReader sr = new StreamReader(memoryStream))
                    {
                        string line = sr.ReadLine();
                        int lineCnt = 1;
                        while (line != null && !error)
                        {
                            line = trimComments(line.Trim());
                            switch (status)
                            {
                                case State.waitForHeader:
                                    if (isStartLine(line))
                                    {
                                        string vvTemp = readVLogVersie(line);
                                        if (vvTemp == null) error = true;
                                        else
                                        {
                                            VLogVersie = vvTemp;
                                            status = State.readingBody;
                                        }
                                    }
                                    break;
                                case State.readingBody:
                                    if (isEndLine(line))
                                    {
                                        status = State.ended;
                                    }
                                    else
                                    {
                                        string[] itemParts = line.Split(',');
                                        if (itemParts.Length == 4) //DP/IS/FC/US regel
                                        {
                                            //item
                                            VLogCfgItem vci = new VLogCfgItem();
                                            error |= !int.TryParse(itemParts[1], out vci.index);
                                            vci.name = itemParts[2].Trim('"');
                                            error |= !int.TryParse(itemParts[3], out vci.type);

                                            if (itemParts[0] == "DP" && !error)
                                            {
                                                //DP item
                                                if (vci.index != DpItems.Count) error = true; //controle op verwachtte oplopende index waarde
                                                else DpItems.Add(vci);
                                            }
                                            if (itemParts[0] == "IS" && !error)
                                            {
                                                //IS item
                                                if (vci.index != IsItems.Count) error = true; //controle op verwachtte oplopende index waarde
                                                else IsItems.Add(vci);
                                            }
                                            if (itemParts[0] == "FC" && !error)
                                            {
                                                //FC item
                                                if (vci.index != FcItems.Count) error = true; //controle op verwachtte oplopende index waarde
                                                else FcItems.Add(vci);
                                            }
                                            if (itemParts[0] == "US" && !error)
                                            {
                                                //US item
                                                if (vci.index != UsItems.Count) error = true; //controle op verwachtte oplopende index waarde
                                                else UsItems.Add(vci);
                                            }
                                        }
                                        else if (itemParts.Length == 2) //SYS regel
                                        {
                                            if(itemParts[0].Trim()=="SYS") Sys = itemParts[1].Trim('"');
                                        }
                                    }
                                    break;
                                case State.ended:
                                    break;
                            }

                            if(error) ErrorText = string.Format("Line {0} incorrect: \"{1}\"", lineCnt, line);

                            line = sr.ReadLine();
                            lineCnt++;
                        }
                    }
                }
            }
            catch
            {
                error = true;
            }

            if (!error && status == State.ended)
            {
                Dp = DpItems.ToArray();
                Is = IsItems.ToArray();
                Fc = FcItems.ToArray();
                Us = UsItems.ToArray();
                Valid = true;
                return true;
            }
            else
            {
                reset();
                return false;
            }
        }

        private bool isStartLine(string line)
        {
            line = line.TrimStart();
            return line.StartsWith("**** VLOGCFG");
        }

        private string readVLogVersie(string line)
        {
            string[] parts = line.Split('/');
            if (parts.Length == 3)
            {
                string part2 = parts[1].Trim();
                if (part2.StartsWith("versie "))
                {
                    return part2.Substring(7, part2.Length - 7);
                }
                else return null;
            }
            else return null;
        }

        private bool isEndLine(string line)
        {
            line = line.TrimStart();
            return line.StartsWith("**** EINDE VLOGCFG ****");
        }

        /// <summary>
        /// Verwijderd /* */ comments op één regel, en // comments
        /// </summary>
        /// <param name="line"></param>
        /// <returns>regel zonder comments</returns>
        private string trimComments(string line)
        {
            if (line == null || line.Length == 0) return line;

            //commentaar met '//' uitfilteren
            int index = line.IndexOf("//");
            if (index != -1) line = line.Substring(0, index);

            //commentaar met /*   */  uitfilteren
            bool ready = false;
            while (!ready)
            {
                int indexStart = line.IndexOf("/*");
                if (indexStart != -1)
                {
                    int indexEnd = line.IndexOf("*/", indexStart + 2);
                    if (indexEnd != -1)
                    {
                        line = line.Substring(0, indexStart) + line.Substring(indexEnd + 2);
                    }
                    else ready = true;
                }
                else ready = true;
            }

            return line;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            if (!Valid) result.AppendFormat("Invalid");
            else
            {
                result.AppendFormat("SYS={0} ", Sys);
                result.AppendFormat("Version={0} ", VLogVersie);
                result.AppendFormat("DP={0} ", Dp.Length);
                result.AppendFormat("Fc={0} ", Fc.Length);
                result.AppendFormat("Is={0} ", Is.Length);
                result.AppendFormat("Us={0}", Us.Length);
            }

            return result.ToString().Trim();
        }
    }

    public class VLogCfgItem
    {
        public int index = 0;
        public string name = "";
        public int type = 0;
    }
}
