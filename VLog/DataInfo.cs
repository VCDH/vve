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

namespace VVE.VLog
{
    public class DataInfo
    {
        public VLogInfo Info = null;
        public int CntDp = -1;
        public int CntFc = -1;
        public int CntIs = -1;
        public int CntUs = -1;

        /// <summary>
        /// Vergelijkt de eigenschappen van de V-Log data met de aangegeven gegevens.
        /// Parameters NULL of -1 is een onbekende waarde
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="version"></param>
        /// <param name="cntDp"></param>
        /// <param name="cntFc"></param>
        /// <param name="cntIs"></param>
        /// <param name="cntUs"></param>
        /// <returns></returns>
        public bool EqualTo(string sys, string version, int cntDp, int cntFc, int cntIs, int cntUs)
        {
            //vergelijken voor zover mogelijk
            bool equal = true;

            if (sys != null && sys.Length > 0 && Info != null && Info.VriId != sys) equal = false;
            if (version != null && version.Length > 0 && Info != null && Info.VLogVersie != version) equal = false;
            if (cntDp > 0 && CntDp > 0 && cntDp != CntDp) equal = false;
            if (cntFc > 0 && CntFc > 0 && cntFc != CntFc) equal = false;
            if (cntIs > 0 && CntIs > 0 && cntIs != CntIs) equal = false;
            if (cntUs > 0 && CntUs > 0 && cntUs != CntUs) equal = false;

            return equal;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            if (Info != null) result.AppendFormat("SYS={0} ", Info.VriId);
            if (Info != null) result.AppendFormat("Version={0} ", Info.VLogVersie);
            if (CntDp != -1) result.AppendFormat("DP={0} ", CntDp);
            if (CntFc != -1) result.AppendFormat("Fc={0} ", CntFc);
            if (CntIs != -1) result.AppendFormat("Is={0} ", CntIs);
            if (CntUs != -1) result.AppendFormat("Us={0} ", CntUs);

            return result.ToString().Trim();
        }
    }
}
