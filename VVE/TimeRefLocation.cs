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
    /// <summary>
    /// Bevat een verwijzing naar een V-Log bestand met daarin de gespecificeerde tijdreferentie
    /// </summary>
    public class TimeRefLocation
    {
        public bool Valid = false;
        public DateTime DtFile = new DateTime(); //bestand op basis van datum en uur
        public DateTime DtTimeRefVLog = new DateTime(); //betreffende tijdreferentie

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0} ", Valid ? "Valid:" : "Invalid");
            if (DtFile.Ticks != 0) sb.AppendFormat("File: {0} ", DtFile.ToString("yyyy-MM-dd HH"));
            if (DtTimeRefVLog.Ticks != 0) sb.AppendFormat("TimeRefFound: {0} ", DtTimeRefVLog.ToString("yyyy-MM-dd HH:mm:ss.f"));

            return sb.ToString();
        }
    }

    /// <summary>
    /// Bevat twee verwijzingen naar tijdreferenties: voor(of op) en na(of op) een aangegeven tijdstip
    /// </summary>
    public class TimeRefFindResult
    {
        public TimeRefLocation TrlBefore = new TimeRefLocation();
        public TimeRefLocation TrlAfter = new TimeRefLocation();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Before={0}\r\n", TrlBefore);
            sb.AppendFormat("After={0}\r\n", TrlAfter);

            return sb.ToString();
        }
    }
}
