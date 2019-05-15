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

namespace VVE
{
    public class VveFuncResult
    {
        public bool Executed = false; //analyse uitgevoerd
        public string Errors = ""; //opgetreden fouten indien analyse niet uitgevoerd kon worden (ExecutionOk==False)
        public TimeSpan ProcessTime = new TimeSpan(); //verstreken tijd gedurende analyse
        public bool ResultsSet = false;
        public string Results = ""; //het resultaat
        public string Info = ""; //info over uitgevoerde analyse
        public List<DpfData> DpfResults = new List<DpfData>();

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();

            res.AppendLine("Uitgevoerd: " + (Executed ? "Ja" : "Nee"));
            if (ProcessTime.Ticks > 0) res.AppendFormat("Processed in {0} seconds.\r\n", (ProcessTime.TotalMilliseconds / 1000D).ToString("F2"));

            res.AppendLine();

            if (Info.Length > 0)
            {
                res.AppendFormat("Info:\r\n-----\r\n{0}\r\n", Info.TrimEnd(new char[] { '\r', '\n' }));
                res.AppendLine();
            }
            if (Errors.Length > 0)
            {
                res.AppendFormat("Errors:\r\n-------\r\n{0}\r\n", Errors.TrimEnd(new char[] { '\r', '\n' }));
                res.AppendLine();
            }
            if (ResultsSet)
            {
                res.AppendFormat("Resultaat:\r\n----------\r\n");
                if (Results.Length == 0)
                {
                    res.AppendLine("- geen resultaat -");
                    res.AppendLine();
                }
                else
                {
                    res.AppendLine(Results.TrimEnd(new char[] { '\r', '\n' }));
                    res.AppendLine();
                }
            }

            return res.ToString().TrimEnd(new char[] { '\r', '\n' });
        }
    }
}
