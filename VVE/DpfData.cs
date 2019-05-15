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
    public class DpfData
    {
        public string Name = "";
        public string Header = "";
        public List<string> Records = new List<string>();
        public string DebugInfo = "";

        public string AllRecords()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine(Header);
            foreach (string r in Records) result.AppendLine(r);

            return result.ToString();
        }

        public void Add(DpfData data)
        {
            Records.AddRange(data.Records);
        }

        public byte[][] Split(int maxSizeBytes)
        {
            List<byte[]> result = new List<byte[]>();

            StringBuilder part = new StringBuilder();
            bool headerWritten = false;
            foreach (string r in Records)
            {
                if (!headerWritten)
                {
                    //minimaal header + 1 record
                    part.AppendLine(Header);
                    part.AppendLine(r);
                    headerWritten = true;
                }
                else
                {
                    if ((part.Length + r.Length) > maxSizeBytes)
                    {
                        //splitsen
                        string data = part.ToString();
                        byte[] dataBytes = Encoding.ASCII.GetBytes(data);
                        result.Add(dataBytes);

                        //nieuw deel aanmaken
                        part.Clear();
                        headerWritten = false;
                    }
                    else
                    {
                        part.AppendLine(r);
                    }
                }
            }

            //laatste data opslaan
            if(headerWritten)
            {
                string data = part.ToString();
                byte[] dataBytes = Encoding.ASCII.GetBytes(data);
                result.Add(dataBytes);
            }

            return result.ToArray();
        }
    }
}
