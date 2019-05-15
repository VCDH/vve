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
    /// Definieerd periode binnen een dag
    /// </summary>
    public class TimePeriod
    {
        public TimeSpan From;
        public TimeSpan To;

        /// <summary>
        /// Stuurt terug of het tijdstip binnen de periode valt
        /// </summary>
        /// <param name="tod"></param>
        /// <returns>true indien binnen periode</returns>
        public bool Between(TimeSpan tod)
        {
            //periode van 10:00 tot 12:00: de tijdstippen 10 uur en 11 uur worden als geldig gezien, 12 uur niet.
            //periode van 00:00 tot 09:00: uur 0 t/m 8 wordt als geldig gezien
            //periode van 18:00 tot 00:00: uur 18 t/m 23 wordt als geldig gezien
            //periode van 18:00 tot 10:00: uur 18 t/m 23 en 0 t/m 9 worden als geldig gezien
            //periode van 00:00 tot 00:00: alle uren zijn geldig
            //periode van 16:00 tot 16:00: alle uren zijn geldig

            if (From < To)
            {
                //b.v. 9:00 tot 12:00
                if (tod >= From && tod < To) return true;
            }
            if (From >= To)
            {
                if (To.Ticks == 0)
                {
                    //b.v. 18:00 - 00:00
                    //b.v. 00:00 - 00:00

                    if (tod >= From) return true;
                }
                else
                {
                    //b.v. 18:00 - 10:00
                    //b.v. 16:00 - 16:00

                    if (tod >= From || tod < To) return true;
                }
            }
            return false;
        }
    }
}
