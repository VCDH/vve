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
    public class ResultMeetpuntenWachttijdFiets
    {
        public bool Succes = false;
        public string ErrorText = "";
        public WachttijdFietsMeetpunt[] Meetpunten = new WachttijdFietsMeetpunt[0];

        public int Aantal
        {
            get
            {
                return Meetpunten.Length;
            }
        }

        public int AantalKopVerwegDruk
        {
            get
            {
                int result = 0;
                foreach (WachttijdFietsMeetpunt wfm in Meetpunten)
                {
                    if (wfm.Drukknop != null) result++;
                }
                return result;
            }
        }

        public int AantalKopVerwegGeenDruk
        {
            get
            {
                int result = 0;
                foreach (WachttijdFietsMeetpunt wfm in Meetpunten)
                {
                    if (wfm.Drukknop == null) result++;
                }
                return result;
            }
        }
    }
}
