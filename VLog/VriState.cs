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
    public class VriState
    {
        public bool Valid = false;
        public DateTime TimeRef = new DateTime();
        public int Delta = 0;
        public VLogInfo Info = null;
        public int[] Detectors = null;
        public int[] FcExtern = null;
        public int[] Wps = null;
        public bool[] OverigeIs = null;
        public bool[] OverigeUsGus = null;

        public DateTime Time
        {
            get
            {
                return TimeRef.AddMilliseconds(Delta * 100);
            }
        }

        public VriState Clone()
        {
            VriState result = new VriState() { Valid = this.Valid, TimeRef = this.TimeRef, Delta = this.Delta, Info = this.Info };

            if (this.Detectors == null) result.Detectors = null;
            else
            {
                result.Detectors = new int[this.Detectors.Length];
                for (int i = 0; i < this.Detectors.Length; i++) result.Detectors[i] = this.Detectors[i];
            }

            if (this.FcExtern == null) result.FcExtern = null;
            else
            {
                result.FcExtern = new int[this.FcExtern.Length];
                for (int i = 0; i < this.FcExtern.Length; i++) result.FcExtern[i] = this.FcExtern[i];
            }

            if (this.Wps == null) result.Wps = null;
            else
            {
                result.Wps = new int[this.Wps.Length];
                for (int i = 0; i < this.Wps.Length; i++) result.Wps[i] = this.Wps[i];
            }

            if (this.OverigeIs == null) result.OverigeIs = null;
            else
            {
                result.OverigeIs = new bool[this.OverigeIs.Length];
                for (int i = 0; i < this.OverigeIs.Length; i++) result.OverigeIs[i] = this.OverigeIs[i];
            }

            if (this.OverigeUsGus == null) result.OverigeUsGus = null;
            else
            {
                result.OverigeUsGus = new bool[this.OverigeUsGus.Length];
                for (int i = 0; i < this.OverigeUsGus.Length; i++) result.OverigeUsGus[i] = this.OverigeUsGus[i];
            }

            return result;
        }

        public DataInfo DataInfo
        {
            get
            {
                DataInfo result = new DataInfo();

                result.Info = Info;
                if (Detectors == null) result.CntDp = -1;
                else result.CntDp = Detectors.Length;
                if (FcExtern == null) result.CntFc = -1;
                else result.CntFc = FcExtern.Length;
                if (OverigeIs == null) result.CntIs = -1;
                else result.CntIs = OverigeIs.Length;
                if (OverigeUsGus == null) result.CntUs = -1;
                else result.CntUs = OverigeUsGus.Length;

                return result;
            }
        }
        
        public Parser.FcState FcStateFromFcExternal(int idx)
        {
            return Parser.FcExternToFc(FcExtern[idx]);           
        }

        public Parser.FcState FcStateFromFcExternalWps(int idx)
        {
            if (Wps != null)
            {
                if (Wps.Length > 1)
                {
                    if (Wps[0] != 5) return Parser.FcState.groen;
                }
            }
            return Parser.FcExternToFc(FcExtern[idx]);
        }
    }
}
