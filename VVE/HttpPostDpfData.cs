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
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace VVE
{
    /// <summary>
    /// Verzenden Dpfdata via HTTP Post
    /// </summary>
    public class HttpPostDpfData
    {
        private bool error = false;
        public bool Errors
        {
            get { return error; }
        }

        private StringBuilder debugLog = new StringBuilder();
        public string DebugLog
        {
            get
            {
                return debugLog.ToString();
            }
        }

        public void PostData(DpfData data, PostSettings ps, string failedDataDir)
        {
            if (data == null || ps == null) throw new ArgumentNullException();
            
            //splitsen in delen
            byte[][] dataParts = data.Split(ps.MaxPostSize);

            for (int i = 0; i < dataParts.Length; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                byte[] dp = dataParts[i];
                string dpLength = (dp != null) ? dp.Length.ToString() : "-";

                JsonRespons respons = Post(ps.URL, dp, ps.Username, ps.Password);

                sw.Stop();

                if (respons == null)
                {
                    //fout
                    //opslaan data
                    string fileName = string.Format("{2}\\{1} DPF Post Part {0} Failed.csv", i, DateTime.Now.ToString("yyyy-MM-dd HHmmss"), failedDataDir);
                    try
                    {
                        File.WriteAllBytes(fileName, dp);
                        debugLog.AppendFormat("POST Part {0} failed after {1} s.: {2} bytes saved to {3}\r\n", i, sw.Elapsed.TotalSeconds.ToString("F2"), dpLength, fileName);
                    }
                    catch (Exception ex)
                    {
                        debugLog.AppendFormat("POST Part {0} failed after {1} s.: {2} bytes could not be saved to {3}: {4}\r\n", i, sw.Elapsed.TotalSeconds.ToString("F2"), dpLength, fileName, ex.Message);
                    }
                }
                else
                {
                    //succes
                    debugLog.AppendFormat("POST Part {0} succes in {1} s.: {2} bytes, FietsViewerRespons.processId={3}\r\n", i, sw.Elapsed.TotalSeconds.ToString("F2"), dpLength, respons.FietsViewerRespons.ProcessId);
                }
            }
        }

        private JsonRespons Post(string URL, byte[] data, string username = "", string password = "")
        {
            //true=ok, false=fout opgetreden en al een foutmelding doorgegeven
            if (URL == null)
            {
                debugLog.AppendLine("Failed: URL is NULL");
                error = true;
                return null;
            }
            if (data == null)
            {
                debugLog.AppendLine("Failed: data is NULL");
                error = true;
                return null;
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Timeout = 5000; //standaard is 100 seconden timeout

                if (username != "") request.Credentials = new NetworkCredential(username, password);
                //request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded"; //"application/json"
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                int responseCode = (int)response.StatusCode;
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responsData = reader.ReadToEnd(); // Read the content
                reader.Close();
                response.Close();

                JsonRespons res = null;
                try
                {
                    res = DeserializeFromString<JsonRespons>(responsData);
                }
                catch
                {
                    debugLog.AppendLine(string.Format("Failed: URL {0} Response kon niet geinterpreteerd worden: {1}", URL, responsData));
                    return null;
                }

                if (responseCode >= 200 && responseCode < 300)
                {
                    return res;
                }
                else
                {
                    //In de praktijk komt hij hier niet, aangezien errors in de exception belanden.
                    //debugLog.AppendLine(string.Format("Failed: URL {0} Response {1}:{2}", URL, responseCode, response.StatusDescription));
                    debugLog.AppendLine(string.Format("Failed: URL {0} Response: {1}", URL, res.FietsViewerRespons.ToString()));
                    return null;
                }
            }
            catch (Exception ex)
            {                
                if (ex is WebException)
                {
                    debugLog.AppendLine(string.Format("Failed: URL {0} Response: {1}", URL, ex.Message));
                }
                else debugLog.AppendLine(string.Format("ERROR: URL {0} Exception: {1}", URL, ex.Message));
                return null;
            }
        }

        public T DeserializeFromString<T>(string json)
        {
             T instance = Activator.CreateInstance<T>();
             using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
             {
                 DataContractJsonSerializer serializer = new DataContractJsonSerializer((typeof(T)));
                 return (T)serializer.ReadObject(ms);
             }
        }
    }

    [DataContract]
    public class JsonRespons
    {
        [DataMember(IsRequired = true, Name = "FietsViewerRespons", Order = 1)]
        public FietsViewerResp FietsViewerRespons = new FietsViewerResp();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("StatusCode={0}, StatusText={1}", FietsViewerRespons.StatusCode, FietsViewerRespons.StatusText);
            if (FietsViewerRespons.StatusDesc!=null && FietsViewerRespons.StatusDesc.Length > 0) sb.AppendFormat(", StatusDesc={0}", FietsViewerRespons.StatusDesc);
            if (FietsViewerRespons.ProcessId != null) sb.AppendFormat(", ProcessId={0}", FietsViewerRespons.ProcessId);
            if (FietsViewerRespons.md5!=null && FietsViewerRespons.md5.Length > 0) sb.AppendFormat(", MD5={0}", FietsViewerRespons.md5);

            return sb.ToString();
        }
    }

    [DataContract]
    public class FietsViewerResp
    {
        [DataMember(IsRequired = true, Name = "statusCode", Order = 1)]
        public int StatusCode = 0;
        [DataMember(IsRequired = true, Name = "statusText", Order = 2)]
        public string StatusText = "";
        [DataMember(IsRequired = false, Name = "statusDesc", Order = 3)]
        public string StatusDesc = "";
        [DataMember(IsRequired = false, Name = "processId", Order = 4)]
        public int? ProcessId = null;
        [DataMember(IsRequired = false, Name = "md5", Order = 5)]
        public string md5 = "";
    }
}
