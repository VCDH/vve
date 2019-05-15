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
    /// Wachttijd resultaat voor een periode met alle FC's
    /// </summary>
    public class PeriodResultWachttijden
    {
        public bool DataComplete;
        public DateTime PeriodStart;
        public TimeSpan PeriodLength;
        public PeriodResultWachttijdFC[] Result = new PeriodResultWachttijdFC[0]; //per FC de resultaten
    }

    public class PeriodResultWachttijdFC
    {
        public string FcName = "";
        public WachttijdFietser[] Wachttijden = new WachttijdFietser[0];
        public bool LusError = false;

        public int AantalFietsers
        {
            get
            {
                return Wachttijden.Length;
            }
        }
        public double WachttijdGemiddeldeSec
        {
            get
            {
                return WachttijdGemiddelde.TotalMilliseconds / 1000D;
            }
        }
        public TimeSpan WachttijdGemiddelde
        {
            get
            {
                int aantal = AantalFietsers;
                if (aantal == 0) return new TimeSpan();

                TimeSpan totalTs = FietsVerliesTijd;
                return new TimeSpan(totalTs.Ticks / aantal);
            }
        }
        public TimeSpan WachttijdMax
        {
            get
            {
                int aantal = AantalFietsers;
                TimeSpan tsMax = new TimeSpan();
                for (int i = 0; i < aantal; i++)
                {
                    if (tsMax < Wachttijden[i].Wachttijd) tsMax = Wachttijden[i].Wachttijd;
                }
                return tsMax;
            }
        }
        public TimeSpan WachttijdMin
        {
            get
            {
                TimeSpan tsMin = new TimeSpan();
                int aantal = AantalFietsers;
                for (int i = 0; i < aantal; i++)
                {
                    if (i == 0 || tsMin > Wachttijden[i].Wachttijd) tsMin = Wachttijden[i].Wachttijd;
                }
                return tsMin;
            }
        }
        public double PercentageGroenAankomst
        {
            get
            {
                int aantal = AantalFietsers;
                if (aantal == 0) return -1;
                int aantalFietsersZonderWachttijd = 0;
                for (int i = 0; i < aantal; i++)
                {
                    if (Wachttijden[i].Wachttijd.Ticks == 0) aantalFietsersZonderWachttijd++;
                }
                return 100D * aantalFietsersZonderWachttijd / aantal;
            }
        }
        public TimeSpan FietsVerliesTijd
        {
            get
            {
                int aantal = AantalFietsers;
                TimeSpan totalTs = new TimeSpan();
                for (int i = 0; i < aantal; i++)
                {
                    totalTs += Wachttijden[i].Wachttijd;
                }
                return totalTs;
            }
        }
    }
}
