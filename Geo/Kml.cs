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
using System.Xml;
using System.IO;

namespace VVE
{
    public class Kml
    {
        private StringBuilder errors = new StringBuilder();
        public string ErrorText
        {
            get
            {
                return errors.ToString();
            }
        }

        public string FileName = "";
        public KmlEntry[] FietsVerweglussen = new KmlEntry[0];
        public KmlEntry[] FietsKoplussen = new KmlEntry[0];
        public KmlEntry[] FietsTellussen = new KmlEntry[0];

        private void clear()
        {
            errors.Clear();
            FietsVerweglussen = new KmlEntry[0];
            FietsKoplussen = new KmlEntry[0];
            FileName = "";
        }

        /// <summary>
        /// Inlezen KML bestand.
        /// Eisen:
        /// - locaties in folders Fietsverweglussen en Fietskoplussen
        /// - locatie bevat naam in formaat: [VRI naam]{spatie][detector]
        /// - locatie bevat lijn met minimaal 2 coordinaten.
        /// </summary>
        /// <param name="filename"></param>
        public bool Load(string filename)
        {
            clear();
            FileName = filename;

            Dictionary<string, KmlEntry> fietsVerweglussen = new Dictionary<string, KmlEntry>();
            Dictionary<string, KmlEntry> fietsKoplussen = new Dictionary<string, KmlEntry>();
            Dictionary<string, KmlEntry> fietsTellussen = new Dictionary<string, KmlEntry>();

            try
            {
                //inlezen
                byte[] kmlData = File.ReadAllBytes(filename);

                using (MemoryStream ms = new MemoryStream(kmlData))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ms);

                    XmlElement Document = doc.DocumentElement["Document"];
                    if (Document != null)
                    {
                        foreach (XmlNode n in Document)
                        {
                            if (n.Name == "Folder")
                            {
                                //XmlElement Folder = Document["Folder"];
                                XmlNode Folder = n;
                                if (Folder != null)
                                {
                                    Dictionary<string, KmlEntry> activeFolder = null;

                                    foreach (XmlNode folderNode in Folder)
                                    {
                                        string folderNaam = "";
                                        if (folderNode.Name == "name")
                                        {
                                            folderNaam = folderNode.InnerText.Trim();
                                            switch (folderNaam)
                                            {
                                                case "Fietsverweglussen":
                                                    activeFolder = fietsVerweglussen;
                                                    break;
                                                case "Fietskoplussen":
                                                    activeFolder = fietsKoplussen;
                                                    break;
                                                case "Fietstellussen":
                                                    activeFolder = fietsTellussen;
                                                    break;
                                            }
                                        }

                                        if (folderNode.Name == "Placemark")
                                        {
                                            string name = "";
                                            XmlNode xmlName = folderNode["name"];
                                            if (xmlName != null) name = xmlName.InnerText.Trim();

                                            string coordinates = "";
                                            XmlNode xmlLineString = folderNode["LineString"];
                                            if (xmlLineString != null)
                                            {
                                                XmlNode xmlCoord = xmlLineString["coordinates"];
                                                if (xmlCoord != null) coordinates = xmlCoord.InnerText.Trim();
                                            }

                                            if (name.Length > 0 && coordinates.Length > 0)
                                            {
                                                if (coordinates.Split(' ').Length > 2) errors.AppendLine("Info: " + name + " bevat meer dan 2 coordinaten");

                                                KmlEntry entry = KmlEntry.FromKml(name, coordinates);
                                                if (entry != null)
                                                {
                                                    if (activeFolder != null)
                                                    {
                                                        if (!activeFolder.ContainsKey(name))
                                                        {
                                                            activeFolder.Add(name, entry);
                                                        }
                                                        else
                                                        {
                                                            errors.AppendLine("Naam " + name + " wordt meerdere keren gebruikt");
                                                        }
                                                    }
                                                    else errors.AppendLine("Geen folder behorend bij " + folderNode.InnerText);
                                                }
                                                if (entry == null) errors.AppendLine("Fout bij " + folderNode.InnerText);
                                            }
                                            else errors.AppendLine("Fout bij " + folderNode.InnerText);
                                        }
                                    }
                                }
                                else errors.AppendLine("Geen Folder entry");
                            }
                        }
                    }
                    else errors.AppendLine("Geen Document entry");
                }
                return true;
            }
            catch (Exception ex)
            {
                errors.AppendLine("Exception: " + ex.Message);
                return false;
            }
            finally
            {
                FietsVerweglussen = fietsVerweglussen.Values.ToArray();
                FietsKoplussen = fietsKoplussen.Values.ToArray();
                FietsTellussen = fietsTellussen.Values.ToArray();
            }

            //sorteren op Kruispunt en detectornaam
            /*  List<string> regelsVerwegLussen = new List<string>(fietsVerwegLussen.Values);
              regelsVerwegLussen.Sort();

              List<string> regelsKopLussen = new List<string>(fietsKoplussen.Values);
              regelsKopLussen.Sort();
  */
            /*  StringBuilder result = new StringBuilder();
              result.AppendLine("//krp;detector;lat;lon;heading");
              foreach (string regel in regelsVerwegLussen) result.AppendLine(regel);

              foreach (string regel in regelsKopLussen) result.AppendLine("//Koplus: " + regel);
              */
        }

        public bool LoadLastFile(string dir)
        {
            clear();

            if (Directory.Exists(dir))
            {
                try
                {
                    //files zoeken
                    string[] files = Directory.GetFiles(dir, "*.kml", SearchOption.TopDirectoryOnly);

                    //files sorteren
                    Array.Sort(files);

                    //laatste file lezen
                    if (files.Length > 0)
                    {
                        string lastFile = files[files.Length - 1];
                        return Load(lastFile);
                    }
                    else
                    {
                        errors.AppendLine("Geen KML bestanden gevonden");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errors.AppendLine("Exception: " + ex.Message);
                    return false;
                }
            }
            else
            {
                errors.AppendLine("Directory bestaat niet");
                return false;
            }
        }
    }

    public class KmlEntry
    {
        public string VriName = "";  //zoals ingelezen, dus met voorloop 'K' en '0'-en indien deze aanwezig is
        public string Detector = ""; //zoals ingelezen, dus met voorloop 'd' indien deze aanwezig is
        public WgsLocation[] Coordinaten = new WgsLocation[0];

        /// <summary>
        /// Inlezen volgens formaat:
        /// - name = [VRInaam][spatie][detectornaam]
        /// - coordinates: minimaal 2 locaties
        /// </summary>
        /// <param name="kmlName"></param>
        /// <param name="kmlCoordinates"></param>
        /// <returns></returns>
        public static KmlEntry FromKml(string kmlName, string kmlCoordinates)
        {
            KmlEntry result = new KmlEntry();

            string[] nameParts = kmlName.Split(' ');
            if (nameParts.Length != 2) return null;
            result.VriName = nameParts[0].Trim();
            result.Detector = nameParts[1].Trim();

            string[] locations = kmlCoordinates.Split(' ');
            if (locations.Length < 2) return null;

            result.Coordinaten = new WgsLocation[locations.Length];

            for (int i = 0; i < locations.Length; i++)
            {
                string[] loc = locations[i].Split(',');
                if (loc.Length != 3) return null;
                double lon, lat;
                if (!double.TryParse(loc[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lon)) return null;
                if (!double.TryParse(loc[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lat)) return null;
                result.Coordinaten[i] = new WgsLocation() { Latitude = lat, Longitude = lon };
            }

            //    return string.Format("{0};{1};{2};{3};{4}", krp, detector, lat1.ToString("F6", System.Globalization.CultureInfo.InvariantCulture), lon1.ToString("F6", System.Globalization.CultureInfo.InvariantCulture), heading);
            return result;
        }       
    }

    public struct WgsLocation
    {
        public double Latitude, Longitude;

        public override string ToString()
        {
            return string.Format("Lat={0} Lon={1}", Latitude, Longitude);
        }
    }
}
