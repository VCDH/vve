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
    public class VLogDataCfgMatch
    {
        public enum PeriodStatus { inactive, activeMatch, activeNoMatch }

        DateTime from = new DateTime();
        DateTime lastKnownTime = new DateTime();
        PeriodStatus status = PeriodStatus.inactive;
        List<MatchPeriod> periods = new List<MatchPeriod>();

        /// <summary>
        /// Start periode. Indien de lopende overeenkomt met de nieuwe match, dan blijft de huidige periode gehandhaafd
        /// </summary>
        /// <param name="match"></param>
        /// <param name="time"></param>
        public void StartPeriod(bool match, DateTime time)
        {
            lastKnownTime = time;
            switch (status)
            {
                case PeriodStatus.inactive:
                    from = time;
                    status = match ? PeriodStatus.activeMatch : PeriodStatus.activeNoMatch;
                    break;
                case PeriodStatus.activeMatch:
                    if (match) break; //was al match, periodes lopen in elkaar over tot één geheel
                    else //no match
                    {
                        saveNewPeriod(true, time);
                        status = PeriodStatus.activeNoMatch;
                    }
                    break;
                case PeriodStatus.activeNoMatch:
                    if (match)
                    {
                        saveNewPeriod(false, time);
                        status = PeriodStatus.activeMatch;
                    }
                    else break; //was al no match, periodes lopen in elkaar over tot één geheel
                    break;
            }
        }

        private void saveNewPeriod(bool match, DateTime time)
        {
            if (from == time)
            {
                //periode van duur nul: niet opslaan

                //checken of de match
                int aantal = periods.Count;
                if (aantal==0)
                {
                    //nog geen periode actief, dus verder gaan met bestaande
                }
                else
                {
                    //doorgaan met vorige periode
                    from = periods[aantal - 1].From;
                    periods.RemoveAt(aantal - 1);
                }
            }
            else
            {
                //match periode opslaan
                periods.Add(new MatchPeriod() { From = from, To = time, Match = match });

                //nieuwe periode starten
                from = time;
            }
        }

        public void UpdateLastKnowTime(DateTime time)
        {
            lastKnownTime = time;
        }

        /// <summary>
        /// beeindig huidige periode
        /// </summary>
        /// <param name="time"></param>
        public void End(DateTime time)
        {
            
            switch (status)
            {
                case PeriodStatus.inactive:
                    //niets af te sluiten
                    break;
                case PeriodStatus.activeMatch:
                    //match periode opslaan
                    periods.Add(new MatchPeriod() { From = from, To = lastKnownTime, Match = true });
                    status = PeriodStatus.inactive;
                    break;
                case PeriodStatus.activeNoMatch:
                    //match periode opslaan
                    periods.Add(new MatchPeriod() { From = from, To = lastKnownTime, Match = false });
                    status = PeriodStatus.inactive;
                    break;
            }
        }

        public string GetText(bool onlyNoMatchPeriods=false)
        {
            StringBuilder sb = new StringBuilder();

            foreach (MatchPeriod p in periods)
            {
                if (onlyNoMatchPeriods && p.Match) continue;
                sb.AppendFormat("V-Log data-config {0} van {1:} tot {2:}\r\n", p.Match ? "match" : "GEEN match", p.From.ToString("yyyy-MM-dd HH:mm:ss.f"), p.To.ToString("yyyy-MM-dd HH:mm:ss.f"));
            }
            if (status == PeriodStatus.activeMatch)
            {
                if(!onlyNoMatchPeriods)
                {
                    sb.AppendFormat("V-Log data-config match van {0:} tot -\r\n",  from.ToString("yyyy-MM-dd HH:mm:ss.f"));
                }
            }
            else if(status==PeriodStatus.activeNoMatch)
            {
                sb.AppendFormat("V-Log data-config GEEN match van {0:} tot -\r\n", from.ToString("yyyy-MM-dd HH:mm:ss.f"));
            }

            return sb.ToString();
        }

        private class MatchPeriod
        {
            public DateTime From;
            public DateTime To;
            public bool Match;
        }
    }
}
