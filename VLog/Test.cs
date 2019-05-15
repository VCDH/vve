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

namespace VVE.VLog
{
    public static class Test
    {
        public static string TestMessages()
        {
            StringBuilder result = new StringBuilder();

            //tijdreferentie
            result.AppendLine("Tijdreferentieberichten");
            result.AppendLine("-----------------------");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("012018052808050000"), "normaal");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("012018121123571500"), "normaal");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("010018052808050000"), "geen eeuw aanduiding");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("012018052808050090"), "tienden in bericht");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("012018052A08050000"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("012018052k08050000"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("012018052A0805000"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("012018052A080000"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("01"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeTijdRef("070000190000000000"), "geen tijdreferentiebericht");
            result.AppendLine();

            //info
            result.AppendLine("Informatie berichten");
            result.AppendLine("--------------------");
            result.AppendFormat("{1}: {0}\r\n", decodeInfo("040200003132392020202020202020202020202020202020"), "normaal VRI 129, 2.0.0");
            result.AppendFormat("{1}: {0}\r\n", decodeInfo("040C223F3132392020202020202020202020202020202020"), "normaal VRI 129, 12.34.63");
            result.AppendFormat("{1}: {0}\r\n", decodeInfo("040200005652493132333920427572676572776567202021"), "normaal VRI1239 Burgerweg  !, 2.0.0");
            result.AppendFormat("{1}: {0}\r\n", decodeInfo("0402000031323920202020202020202020202020202020"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeInfo("04"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeInfo("070200003132392020202020202020202020202020202020"), "onjuist berichttype");
            result.AppendFormat("{1}: {0}\r\n", decodeInfo("040200003132K92020202020202020202020202020202020"), "onjuist karakter");
            result.AppendLine();

            //delta
            result.AppendLine("Delta uit berichten");
            result.AppendLine("-------------------");
            result.AppendFormat("{1}: {0}\r\n", decodeDelta("0500002600016600000001000000000000000000000000"), "normaal delta 0");
            result.AppendFormat("{1}: {0}\r\n", decodeDelta("07BB1012000000"), "normaal delta 2993");
            result.AppendFormat("{1}: {0}\r\n", decodeDelta("0912300E007062007062063063007063062007007063007007"), "normaal delta 291");
            result.AppendFormat("{1}: {0}\r\n", decodeDelta("0B00103A0803C00000000480"), "normaal delta 1");
            result.AppendFormat("{1}: {0}\r\n", decodeDelta("0B00103A0803C0000000048"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeDelta("0B00"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeDelta("040200003132392020202020202020202020202020202020"), "onjuist type");
            result.AppendLine();

            //status WPS
            result.AppendLine("Status WPS");
            result.AppendLine("----------");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusWPS("1300000240"), "normaal 2 stuks [0]=4 [1]=0");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusWPS("130000034020"), "normaal 3 stuks [0]=4 [1]=0 [2]=2");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusWPS("13000000"), "onjuist aantal van 0 elementen");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusWPS("130000!240"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusWPS("13000002402"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusWPS("1300000340"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusWPS("1300"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusWPS("0600000240"), "onjuiste type");
            result.AppendLine();

            //status detectie
            result.AppendLine("Status detectie");
            result.AppendLine("---------------");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusDet("05000006000167"), "normaal 6 stuks [3]=1 [4]=6 [5]=7");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusDet("050000FF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEFFFFFFFFFFFFFFFE0"), "normaal 255 stuks");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusDet("05000000"), "normaal 0 stuks");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusDet("050000060001!7"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusDet("0500000600016"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusDet("0500000600016701"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusDet("0500"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusDet("06000006000167"), "onjuiste type");
            result.AppendLine();

            //status FC intern
            result.AppendLine("Status FC intern");
            result.AppendLine("----------------");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcInt("0900001A007007046007007063007063007007063007007063063007007007007007007007007063063063"), "normaal 26 FC's");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcInt("09000005007007046807FFF0"), "normaal 5 FC's");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcInt("09000000"), "normaal 0 FC's");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcInt("0900000500700704H0070070"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcInt("09000005007007046007007"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcInt("09000005007007046007"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcInt("09"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcInt("050000050070070460070070"), "onjuist type");
            result.AppendLine();

            //status FC extern
            result.AppendLine("Status FC extern");
            result.AppendLine("----------------");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcExt("0D0000151000000000000000000110"), "normaal 26 FC's");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcExt("0D000005765430"), "normaal 5 FC's");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcExt("0D000000"), "normaal 0 FC's");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcExt("0D00000507060J0403"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcExt("0D000005070605043"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcExt("0D00000507060403"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcExt("0D"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusFcExt("050000050706050403"), "onjuist type");
            result.AppendLine();

            //status IS
            result.AppendLine("Status IS");
            result.AppendLine("---------");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("070000150A0F00"), "normaal 21 IS");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("0700001500F000"), "normaal 21 IS");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("0700008500100000100100000000000020000002D8"), "normaal 135 IS");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("0700000400100000100100000000000020000002D80010000010010000000000002000FF00"), "normaal 260 IS");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("0700000400100000100100000000000020000002D80010000010010000000000002000FF0012"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("07000000"), "normaal 0 IS");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("0700001500)000"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("0700001500F00"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("0700001500F0"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("07"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusIs("0500001500F000"), "onjuist type");
            result.AppendLine();

            //status US
            result.AppendLine("Status US GUS");
            result.AppendLine("-------------");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B0000150A0F00"), "normaal 21 US");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B00001500F000"), "normaal 21 US");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B00008500100000100100000000000020000002D8"), "normaal 135 US");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B00000400100000100100000000000020000002D80010000010010000000000002000FF00"), "normaal 260 US");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B00000400100000100100000000000020000002D80010000010010000000000002000FF0012"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B000000"), "normaal 0 US");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B00001500)000"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B00001500F00"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B00001500F0"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0B"), "onjuiste lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeStatusUsGus("0500001500F000"), "onjuist type");
            result.AppendLine();

            //wijziging wps
            result.AppendLine("Wijziging WPS");
            result.AppendLine("-------------");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingWPS("14132105"), "normaal [0]=5");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingWPS("141323051223"), "normaal [0]=5 [1]=2 [2]=3");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingWPS("141320"), "normaal 0 stuks");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingWPS("14132G05"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingWPS("1413210512"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingWPS("141321051"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingWPS("14"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingWPS("05132105"), "onjuist type");
            result.AppendLine();

            //wijziging detectie
            result.AppendLine("Wijziging detectie");
            result.AppendLine("------------------");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingDet("06A0810B01"), "normaal [0]=index 11, value 1");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingDet("06A08F0B010AFF0100020103010B010AFF010002010B010AFF0100020103010B01"), "normaal");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingDet("06A080"), "normaal 0 stuks");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingDet("06A0810B0G"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingDet("06A0810B0"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingDet("06A0810B010AFF"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingDet("06A0"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingDet("05A0810B01"), "onjuist type");
            result.AppendLine();

            //wijziging FC intern
            result.AppendLine("Wijziging FC intern");
            result.AppendLine("-------------------");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcInt("0A0163010062030062090063"), "normaal [0]=index 1, value 98 [1]=index 3, value 98 [2]=index 9, value 99");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcInt("0A016F01006203006209006308006207006206006305006204006202006300FEDC0A0062AB0063F000620E00620D0063"), "normaal");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcInt("0A0080"), "normaal 0 stuks");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcInt("0A008108006G"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcInt("0A016301006203006209063"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcInt("0A0162010062030062090063"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcInt("0A01"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcInt("0B0163010062030062090063"), "onjuist type");
            result.AppendLine();

            //wijziging FC extern
            result.AppendLine("Wijziging FC extern");
            result.AppendLine("-------------------");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcExt("0E0033000213031401"), "normaal [0]=2, [19]=3, [20]=1");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcExt("0E003200021303"), "normaal");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcExt("0E0030"), "normaal 0 stuks");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcExt("0E00330002130314i1"), "onjuist karakter");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcExt("0E003300021303140"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcExt("0E003300021303"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcExt("0E01"), "onjuist lengte");
            result.AppendFormat("{1}: {0}\r\n", decodeWijzigingFcExt("050033000213031401"), "onjuist type");
            result.AppendLine();

            return result.ToString();
        }

        private static string decodeTijdRef(string msg)
        {
            DateTime res = Parser.DecodeTijdRef(msg);
            if (res.Ticks == 0) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, res.ToString("yyyy-MM-dd HH:mm:ss.f"));
        }

        private static string decodeInfo(string msg)
        {
            VLogInfo res = Parser.DecodeInfo(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, res.ToString());
        }

        private static string decodeDelta(string msg)
        {
            int res = Parser.DecodeDelta(msg);
            if (res == -1) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, res.ToString());
        }

        private static string decodeStatusWPS(string msg)
        {
            int[] res = Parser.DecodeStatusWPS(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeStatusDet(string msg)
        {
            int[] res = Parser.DecodeStatusDet(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeStatusFcInt(string msg)
        {
            int[] res = Parser.DecodeStatusFcInt(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeStatusFcExt(string msg)
        {
            int[] res = Parser.DecodeStatusFcExt(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeStatusIs(string msg)
        {
            bool[] res = Parser.DecodeStatusIs(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeStatusUsGus(string msg)
        {
            bool[] res = Parser.DecodeStatusUsGus(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeWijzigingWPS(string msg)
        {
            Change[] res = Parser.DecodeWijzigingWPS(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeWijzigingDet(string msg)
        {
            Change[] res = Parser.DecodeWijzigingDet(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeWijzigingFcInt(string msg)
        {
            Change[] res = Parser.DecodeWijzigingFcInt(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        private static string decodeWijzigingFcExt(string msg)
        {
            Change[] res = Parser.DecodeWijzigingFcExt(msg);
            if (res == null) return string.Format("{0} - ongeldig bericht", msg);
            else return string.Format("{0} - {1}", msg, arrayToString(res));
        }

        /*private static string intArrayToString(int[] data)
        {
            if (data == null) return "NULL";
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                result.AppendFormat("[{0}]={1} ",i,data[i]);
            }

            return result.ToString().Trim();
        }*/

        private static string arrayToString<T>(T[] data)
        {
            if (data == null) return "NULL";
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                result.AppendFormat("[{0}]={1} ", i, data[i]);
            }

            return result.ToString().Trim();
        }
    }
}
