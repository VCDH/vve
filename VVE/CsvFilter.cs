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
    public class CsvFilter
    {
        public bool[] Weekdagen = null;
        public TimePeriod[] FilterPerioden = null;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Weekdagen != null)
            {
                sb.Append("Weekdagen: ");
                if (Weekdagen.Length != 7) sb.Append("ERROR " + Weekdagen.Length.ToString() + " weekdagen");
                else
                {
                    string[] dagen = new string[] { "ma", "di", "wo", "do", "vr", "za", "zo" };
                    bool first = true;
                    for (int i = 0; i < 7; i++)
                    {
                        if (Weekdagen[i])
                        {
                            if (!first) sb.Append(", ");
                            sb.Append(dagen[i]);
                            first = false;
                        }
                    }
                    if (first)
                    {
                        sb.Append(" - geen -");
                    }
                }
                sb.AppendLine();
            }
            if (FilterPerioden != null)
            {
                sb.Append("Dagperioden: ");
                bool first = true;
                foreach (TimePeriod tp in FilterPerioden)
                {
                    if (!first) sb.Append(", ");
                    sb.AppendFormat("{0:hh}:{0:mm}-{1:hh}:{1:mm}", tp.From, tp.To);
                    first = false;
                }
                if (first)
                {
                    sb.Append(" - geen filter actief -");
                }
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd(new char[] { '\r', '\n' });
        }

        public bool Min1WerkdagGeselecteerd
        {
            get
            {
                if (Weekdagen == null || Weekdagen.Length != 7) return true;
                if (!Weekdagen[0] &&
                    !Weekdagen[1] &&
                    !Weekdagen[2] &&
                    !Weekdagen[3] &&
                    !Weekdagen[4] &&
                    !Weekdagen[5] &&
                    !Weekdagen[6])
                {
                    return false;
                }
                else return true;
            }
        }       

    /// <summary>
    /// Bepaalt of een tijdstip binnen het filter valt.
    /// </summary>
    /// <param name="dt"></param>
    /// <returns>true=valt binnen filter</returns>
    public bool ValidInFilter(DateTime dt)
        {
            return checkDayOfWeek(dt) && checkFilterPerioden(dt);
        }

        /// <summary>
        /// Stuurt terug of de weekdag geselecteerd is
        /// </summary>
        /// <param name="weekdagen"></param>
        /// <param name="dt"></param>
        /// <returns>true=actief</returns>
        private bool checkDayOfWeek(DateTime dt)
        {
            if (Weekdagen == null || Weekdagen.Length != 7) return true;

            int dow = (int)dt.DayOfWeek; //zondag=0, maandag=1
            dow--; //zondag=-1, maandag=0
            if (dow == -1) dow += 7; //zondag=6, maandag=0

            return Weekdagen[dow];
        }

        /// <summary>
        /// Stuurt terug of het tijdstip binnen de actieve perioden valt
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="perioden"></param>
        /// <returns>true=actief</returns>
        private bool checkFilterPerioden(DateTime dt)
        {
            //periode van 10:00 tot 12:00: de tijdstippen 10 uur en 11 uur worden als geldig gezien, 12 uur niet.
            //periode van 00:00 tot 09:00: uur 0 t/m 8 wordt als geldig gezien
            //periode van 18:00 tot 00:00: uur 18 t/m 23 wordt als geldig gezien
            //periode van 18:00 tot 10:00: uur 18 t/m 23 en 0 t/m 9 worden als geldig gezien
            //periode van 00:00 tot 00:00: alle uren zijn geldig
            //periode van 16:00 tot 16:00: alle uren zijn geldig
            if (FilterPerioden == null || FilterPerioden.Length == 0) return true;

            TimeSpan tod = dt.TimeOfDay;

            foreach (TimePeriod fp in FilterPerioden)
            {
                if (fp.Between(tod)) return true;
            }
            return false;
        }

        
    }
}
