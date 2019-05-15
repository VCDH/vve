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
using VVE.VLog;

namespace VVE
{
    public class CollectVLogData
    {
        public enum FileStructure { DagBestanden, Eenbestand}

        private FileCollection _dataFiles = new FileCollection();
        private string _vri = "";
        private StringBuilder _collectedVLogData = new StringBuilder(); //data van actuele dag
        private DateTime _firstVLogDataDt; //eerst ontvangen datum
        private DateTime _lastVLogDataDt; //actuele dag
        private FileStructure _fileStructure= FileStructure.DagBestanden;
        private DateTime _beginVLogDt;
        private DateTime _endVLogDt;

        /// <summary>
        /// Verzamelen van V-Log data in dagbestanden
        /// </summary>
        /// <param name="rootDir">archief map Den Haag structuur</param>
        /// <param name="vri">kruispuntnummer</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public FileCollection Collect(string rootDir, string vri, TimeRefLocation begin, TimeRefLocation end, FileStructure fileStructure)
        {
            //verzamelen van V-Log data in dagbestanden
            //  - data vanaf begin opslaan tot end
            //  - V-Log data opslaan in dagbestanden

            _dataFiles = new FileCollection();
            _vri = vri;
            _collectedVLogData = new StringBuilder();
            _lastVLogDataDt = new DateTime();
            _fileStructure = fileStructure;
            _beginVLogDt = begin.DtTimeRefVLog;
            _endVLogDt = end.DtTimeRefVLog;

            //doorlopen V-Log data en doorgeven 
            VLogArchiefDenHaag.ProcesData(rootDir, vri, begin, end, VLogArchiefDenHaag.procesMode.excludeToTimeRef, receiveData);

            //laatste data opslaan
            endData();

            return _dataFiles;
        }

        private void receiveData(string vlogMsg)
        {
            //ontvangt V-Log data

            if (vlogMsg.Length > 2 && vlogMsg.Substring(0, 2) == "01")
            {
                //tijdreferentie bericht
                DateTime tijdstip = VLog.Parser.DecodeTijdRef(vlogMsg);
                if (tijdstip.Ticks > 0)
                {
                    //geldige tijd ontvangen
                    if (_lastVLogDataDt.Ticks == 0)
                    {
                        //eerste data
                        _lastVLogDataDt = tijdstip;
                        _firstVLogDataDt = tijdstip;
                    }
                    else
                    {
                        //vervolg data
                        if (_fileStructure == FileStructure.DagBestanden)
                        {
                            //bepalen of een nieuwe dag is aangebroken en dus een nieuw data bestand aangemaakt moet worden
                            if (tijdstip.Date > _lastVLogDataDt.Date)
                            {
                                //nieuwe dag aangebroken

                                //opslaan data in dagbestand
                                string datum = _lastVLogDataDt.ToString("yyyy-MM-dd");
                                SingleFile df = new SingleFile();
                                df.Data = Encoding.ASCII.GetBytes(_collectedVLogData.ToString());
                                df.FileName = generateFilename();
                                _dataFiles.Files.Add(df);

                                //nieuw dagbestand prepareren
                                _collectedVLogData.Clear();
                            }
                            else
                            {
                                //zelfde dag
                            }
                        }

                        _lastVLogDataDt = tijdstip;
                    }
                }
            }

            _collectedVLogData.AppendLine(vlogMsg); //toevoegen VLog bericht
        }

        private void endData()
        {
            //laatste data is ontvangen: opslaan data in dagbestand
            if (_lastVLogDataDt.Ticks == 0 || _collectedVLogData.Length == 0) return; //geen data om op te slaan
                        
            SingleFile df = new SingleFile();
            df.Data = Encoding.ASCII.GetBytes(_collectedVLogData.ToString());
            df.FileName = generateFilename();
            _dataFiles.Files.Add(df);
        }

        private string generateFilename()
        {
            switch (_fileStructure)
            {
                case FileStructure.DagBestanden:
                    return string.Format("K{0} {1}.vlg", VLogArchiefDenHaag.VriNameToFullNumber(_vri), _lastVLogDataDt.ToString("yyyy-MM-dd")); //"K025 2018-06-15.vlg";        
                case FileStructure.Eenbestand:
                    return string.Format("K{0} {1} - {2}.vlg",
                                          VLogArchiefDenHaag.VriNameToFullNumber(_vri),
                                          _firstVLogDataDt.ToString("yyyy-MM-dd_HHmm"),
                                          _lastVLogDataDt.ToString("yyyy-MM-dd_HHmm")); //"K025 2018-06-15_1300 - 2018-06-15_1800.vlg";   
            }
            return "";
        }
    }

    public class FileCollection
    {
        //bevat meerdere bestanden
        public List<SingleFile> Files = new List<SingleFile>();
    }

    public class SingleFile
    {
        //bevat bestandsnaam en data
        public string FileName;
        public byte[] Data;
    }
}
