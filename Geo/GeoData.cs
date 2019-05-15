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
    /// Bevat per VRI de locatie en rijrichting van de koplussen en verweglussen.
    /// </summary>
    public class GeoData
    {
        public Dictionary<string, GeoDataVri> Vris = new Dictionary<string, GeoDataVri>();

        public GeoData()
        {

        }

        /// <summary>
        /// Converteerd de KML gegevens naar GeoData
        /// </summary>
        /// <param name="kmlData"></param>
        public GeoData(Kml kmlData)
        {
            if (kmlData == null) throw new ArgumentNullException("kmlData");
            if (kmlData.FietsVerweglussen == null) throw new ArgumentNullException("kmlData.FietsVerweglussen");
            if (kmlData.FietsKoplussen == null) throw new ArgumentNullException("kmlData.FietsKoplussen");
            if (kmlData.FietsTellussen == null) throw new ArgumentNullException("kmlData.FietsTellussen");

            //verweglussen doorlopen
            foreach (KmlEntry ke in kmlData.FietsVerweglussen)
            {
                if (ke.Coordinaten.Length >= 2)
                {
                    string vriName = ke.VriName.TrimStart(new char[] { 'K', 'k', '0' });
                    if (!Vris.ContainsKey(vriName))
                    {
                        Vris.Add(vriName, new GeoDataVri() { Name = vriName });
                    }
                    Vris[vriName].FietsVerweglussen.Add(new GeoDataLus()
                    {
                        Name = ke.Detector.TrimStart('d'),
                        LocationLus = ke.Coordinaten[1],
                        Heading = heading(ke.Coordinaten[1], ke.Coordinaten[0]),
                        LocationStopstreep = ke.Coordinaten[0]
                    });
                }
            }
            //koplussen doorlopen
            foreach (KmlEntry ke in kmlData.FietsKoplussen)
            {
                if (ke.Coordinaten.Length >= 2)
                {
                    string vriName = ke.VriName.TrimStart(new char[] { 'K', 'k', '0' });
                    if (!Vris.ContainsKey(vriName))
                    {
                        Vris.Add(vriName, new GeoDataVri() { Name = vriName });
                    }
                    Vris[vriName].FietsKoplussen.Add(new GeoDataLus()
                    {
                        Name = ke.Detector.TrimStart('d'),
                        LocationLus = ke.Coordinaten[1],
                        Heading = heading(ke.Coordinaten[1], ke.Coordinaten[0]),
                        LocationStopstreep = ke.Coordinaten[0]
                    });
                }
            }
            //tellussen doorlopen
            foreach (KmlEntry ke in kmlData.FietsTellussen)
            {
                if (ke.Coordinaten.Length >= 2)
                {
                    string vriName = ke.VriName.TrimStart(new char[] { 'K', 'k', '0' });
                    if (!Vris.ContainsKey(vriName))
                    {
                        Vris.Add(vriName, new GeoDataVri() { Name = vriName });
                    }
                    Vris[vriName].FietsTellussen.Add(new GeoDataLus()
                    {
                        Name = ke.Detector.TrimStart('d'),
                        LocationLus = ke.Coordinaten[1],
                        Heading = heading(ke.Coordinaten[1], ke.Coordinaten[0]),
                        LocationStopstreep = ke.Coordinaten[0]
                    });
                }
            }
        }

        private int heading(WgsLocation van, WgsLocation naar)
        {
            return (int)(bearing(van.Latitude, van.Longitude, naar.Latitude, naar.Longitude) + 0.5);
        }

        private double bearing(double lat1, double lon1, double lat2, double lon2)
        {
            double Y = Math.Sin(deg2Rad(lon2 - lon1)) * Math.Cos(deg2Rad(lat2));
            double X = Math.Cos(deg2Rad(lat1)) * Math.Sin(deg2Rad(lat2)) - Math.Sin(deg2Rad(lat1)) * Math.Cos(deg2Rad(lat2)) * Math.Cos(deg2Rad(lon2 - lon1));
            double bearing = rad2Deg(Math.Atan2(Y, X));
            if (bearing < 0) bearing = 360 + bearing;
            return bearing;
        }

        private double deg2Rad(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        private double rad2Deg(double radians)
        {
            return (radians / Math.PI) * 180;
        }        
    }

    public class GeoDataVri
    {
        public string Name = ""; //VRI naam
        public List<GeoDataLus> FietsKoplussen = new List<GeoDataLus>();
        public List<GeoDataLus> FietsVerweglussen = new List<GeoDataLus>();
        public List<GeoDataLus> FietsTellussen = new List<GeoDataLus>();

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("VRI {0}\r\n", Name);
            result.AppendLine("Koplussen");
            foreach (GeoDataLus d in FietsKoplussen)
            {
                result.AppendFormat("  {0}\r\n", d);
            }
            result.AppendLine("Verweglussen");
            foreach (GeoDataLus d in FietsVerweglussen)
            {
                result.AppendFormat("  {0}\r\n", d);
            }
            result.AppendLine("Tellussen");
            foreach (GeoDataLus d in FietsTellussen)
            {
                result.AppendFormat("  {0}\r\n", d);
            }

            return result.ToString();
        }        
    }

    public class GeoDataLus
    {
        public string Name = ""; //detectornaam zonder voorloop 'd', overeenkomend met de regeling
        public WgsLocation LocationLus;
        public int Heading = 0;
        public WgsLocation LocationStopstreep;        

        public override string ToString()
        {
            return string.Format("  {0}: {1}, {2}, {3}\r\n", Name, LocationLus.Latitude.ToString("F4"), LocationLus.Longitude.ToString("F4"), Heading);
        }
    }
}
