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
using System.IO.Compression;

namespace VVE.VLog
{
    public static class Parser
    {
        public static DateTime DecodeTijdRef(string msg)
        {
            //decodeer het V-Log tijdreferentiebericht
            // - .Ticks>0  : geldige waarde
            // - .Ticks==0 : onjuist bericht

            //voorbeeld v-log tijdreferentiebericht: 010018052808050000
            // 01
            // 0018 jaar 2018
            // 05   maand
            // 28   dag
            // 08   uren
            // 05   minuten
            // 00   seconden
            // 0    tienden
            // 0    dummy

            DateTime result = new DateTime();

            //checks
            if (!msgOk(msg, 18, "01")) return result;

            //data verwerken
            int[] waarden = new int[18];
            for (int i = 0; i < 18; i++)
            {
                waarden[i] = hex2int(msg[i]);
                if (waarden[i] >= 10) return result; //alleen 0-9 zijn geldige waarden
            }

            int jaar = waarden[2] * 1000 + waarden[3] * 100 + waarden[4] * 10 + waarden[5];
            int maand = waarden[6] * 10 + waarden[7];
            int dag = waarden[8] * 10 + waarden[9];
            int uur = waarden[10] * 10 + waarden[11];
            int minuut = waarden[12] * 10 + waarden[13];
            int sec = waarden[14] * 10 + waarden[15];
            int tienden = waarden[16];
            tienden = 0; //tienden zijn niet betrouwbaar

            //Oude Peek automaten geven 20xx niet door, als correctie 2000 optellen
            if (jaar < 100) jaar += 2000;

            //situatie Vialis FR90/FR94 automaten die 24:00:00 uitsturen i.p.v. 0:00:00
            if (uur == 24 && minuut == 0 && sec == 0)
            {
                try
                {
                    result = new DateTime(jaar, maand, dag, 0, minuut, sec, tienden * 100, DateTimeKind.Unspecified);
                    result = result.AddDays(1);
                }
                catch
                {
                    //datum/tijd incorrect
                    return new DateTime();
                }
            }
            else
            {
                //normale situatie
                try
                {
                    result = new DateTime(jaar, maand, dag, uur, minuut, sec, tienden * 100, DateTimeKind.Unspecified);
                }
                catch
                {
                    //datum/tijd incorrect
                    return new DateTime();
                }
            }

            return result;
        }

        public static VLogInfo DecodeInfo(string msg)
        {
            //decodeer informatie bericht: berichttype 0x04
            //  !NULL : geldige waarde -> aantal detectoren met actuele status
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "040200003132392020202020202020202020202020202020"
            //  04: type
            //  020000: versie
            //  3132392020202020202020202020202020202020: vri_id / sys

            //checks
            if (!msgOk(msg, 48, "04")) return null;

            VLogInfo result = new VLogInfo();

            result.VLogVersie = string.Format("{0}.{1}.{2}", hex2int(msg[2], msg[3]), hex2int(msg[4], msg[5]), hex2int(msg[6], msg[7]));
            StringBuilder vriId = new StringBuilder();
            for (int i = 8; i < 48; i += 2) vriId.Append((char)hex2int(msg[i], msg[i + 1]));
            result.VriId = vriId.ToString().Trim();

            return result;
        }

        public static int DecodeDelta(string msg)
        {
            //decodeer de delta uit het bericht
            //  -1  : onjuist bericht
            //  >=0 : geldige waarde
            int result = -1;

            //checks
            if (!msgOk(msg, 6)) return result;

            string type = msg.Substring(0, 2);
            switch (type)
            {
                case "05": //status detectoren
                case "06": //wijziging detectoren
                case "07": //status overige ingangen GUS
                case "08": //wijziging overige ingangen GUS
                case "09": //status signaalgroepen intern
                case "0A": //wijziging signaalgroepen intern
                case "0B": //status overige uitgangen GUS
                case "0C": //wijziging overige uitgangen GUS
                case "0D": //status signaalgroep extern WUS
                case "0E": //wijzigings signaalgroep extern WUS
                case "0F": //status overige uitgangen WUS
                case "10": //wijzigings overige uitgangen WUS
                case "11": //status gewenst programma
                case "12": //wijzigings gewenste programma
                case "13": //status werkelijke programma
                case "14": //wijzigings werkelijke programma
                case "17": //status thermometer
                case "18": //wijzigings thermometer
                case "1A": //Snelheid detectie
                case "1C": //Selectieve detectie (KAR)
                case "1E": //Selectieve detectie
                case "20": //Status instructievariabelen
                case "22": //Aanvullend openbaar vervoer en hulpdienst informatie
                    result = (hex2int(msg[2]) << 8) + (hex2int(msg[3]) << 4) + hex2int(msg[4]);
                    break;
                default:
                    break;
            }

            return result;
        }

        public static int[] DecodeStatusWPS(string msg)
        {
            //decodeer status bericht WPS: berichttype 0x13
            //  !NULL : geldige waarde -> statussen met actuele status
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "1300000240"
            //  13: type
            //  000: delta
            //  0: dummy
            //  02: aantal WPS elementen (altijd 2 of 3)
            //  4 0: data (2x 1 chars)
            //  x: eventueel dummy bij oneven aantal

            //checks
            if (!msgOk(msg, 8, "13")) return null;

            //aantal WPS items bepalen
            int aantal = (hex2int(msg[6]) << 4) + hex2int(msg[7]);
            if (aantal != 2 && aantal != 3) return null; //formaat onjuist: altijd 2 of 3 elementen verwacht
            if (msg.Length != 8 + (((aantal + 1) / 2) * 2)) return null; //lengte onjuist

            int[] result = new int[aantal];
            for (int i = 0; i < aantal; i++)
            {
                result[i] = hex2int(msg[8 + i]);
            }

            return result;
        }

        public static int[] DecodeStatusDet(string msg)
        {
            //decodeer status bericht detectie: berichttype 0x05
            //  !NULL : geldige waarde -> aantal detectoren met actuele status
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "050000641100000001111110000000000000000000000000000000000000000000000001000000000000000000000000000000000000"
            //  05: type
            //  000: delta
            //  0: dummy
            //  64: aantal detectoren (100 stuks)
            //  1100000001111110000000000000000000000000000000000000000000000001000000000000000000000000000000000000: data (100x 1 chars)

            //checks
            if (!msgOk(msg, 8, "05")) return null;

            //aantal detectoren bepalen
            int aantal = (hex2int(msg[6]) << 4) + hex2int(msg[7]);
            if (msg.Length != 8 + (((aantal + 1) / 2) * 2)) return null; //lengte onjuist

            int[] result = new int[aantal];
            for (int i = 0; i < aantal; i++)
            {
                result[i] = hex2int(msg[8 + i]);
            }

            return result;
        }

        public static int[] DecodeStatusFcInt(string msg)
        {
            //decodeer status bericht interne signaalgroepen: berichttype 0x09
            //  !NULL : geldige waarde -> aantal signaalgroepen met actuele status
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "0900001A007007046007007063007063007007063007007063063007007007007007007007007063063063"
            //  09: type
            //  000: delta
            //  0: dummy
            //  1A: aantal signaalgroepen (26 stuks)
            //  007007046007007063007063007007063007007063063007007007007007007007007063063063: data (26x 3 chars)
            //  x: dummy bij oneven aantal signaalgroepen

            //checks
            if (!msgOk(msg, 8, "09")) return null;

            //aantal signaalgroepen bepalen
            int aantal = hex2int(msg[6], msg[7]);
            if (msg.Length != 8 + ((((aantal * 3) + 1) / 2) * 2)) return null; //lengte onjuist

            int[] result = new int[aantal];
            for (int i = 0; i < aantal; i++)
            {
                int idx = 8 + i * 3;
                result[i] = hex2int(msg[idx], msg[idx + 1], msg[idx + 2]);
            }

            return result;
        }

        public static int[] DecodeStatusFcExt(string msg)
        {
            //decodeer status bericht externe signaalgroepen: berichttype 0x0D
            //  !NULL : geldige waarde -> aantal signaalgroepen met actuele status
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "0D0000151000000000000000000110"
            //  0D: type
            //  000: delta
            //  0: dummy
            //  15: aantal signaalgroepen (21 stuks)
            //  100000000000000000011: data (21x 1 char)
            //  0: dummy bij oneven aantal signaalgroepen

            //checks
            if (!msgOk(msg, 8, "0D")) return null;

            //aantal signaalgroepen bepalen
            int aantal = hex2int(msg[6], msg[7]);
            if (msg.Length != 8 + (((aantal + 1) / 2) * 2)) return null; //lengte onjuist

            int[] result = new int[aantal];
            for (int i = 0; i < aantal; i++)
            {
                result[i] = hex2int(msg[8 + i]);
            }

            return result;
        }

        public static bool[] DecodeStatusIs(string msg)
        {
            //decodeer status bericht ingangssignalen: berichttype 0x07
            //  !NULL : geldige waarde -> aantal ingangssignalen met actuele status
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "07000015000000"
            //  07: type
            //  000: delta
            //  0: dummy
            //  15: aantal ingangssignalen (21 stuks)
            //  000000: data (21x 1 bit chars)
            //  x: eventueel dummy bits ingangssignalen

            //checks
            if (!msgOk(msg, 8, "07")) return null;

            //aantal ingangssignalen bepalen
            int aantal = hex2int(msg[6], msg[7]);
            int verwachtteLengteBericht = 8 + ((aantal + 7) / 8) * 2;
            int verschilLengte = msg.Length - verwachtteLengteBericht;
            if (verschilLengte == 0)
            {
                //lengte OK
            }
            else if (verschilLengte > 0 && verschilLengte % (256 / 4) == 0)
            {
                //veelvoud van 256 aanwezig
                aantal = aantal + verschilLengte * 4;
            }
            else return null; //lengte onjuist

            bool[] result = new bool[aantal];
            int data = 0;
            for (int i = 0; i < aantal; i++)
            {
                int bit = 3 - (i % 4);
                if (bit == 3)
                {
                    int idx = 8 + (i / 4);
                    data = hex2int(msg[idx]);
                }
                result[i] = ((data >> bit) & 0x01) == 0x01;
            }

            return result;
        }

        public static bool[] DecodeStatusUsGus(string msg)
        {
            //decodeer status bericht uitgangssignalen: berichttype 0x0B
            //  !NULL : geldige waarde -> aantal uitgangssignalen met actuele status
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "0B000013800240"
            //  0B: type
            //  000: delta
            //  0: dummy
            //  13: aantal uitgangssignalen (19 stuks)
            //  800240: data (19 x 1 bit chars)
            //  x: eventueel dummy bits uitgangssignalen

            //checks
            if (!msgOk(msg, 8, "0B")) return null;

            //aantal uitgangssignalen bepalen
            int aantal = hex2int(msg[6], msg[7]);
            int verwachtteLengteBericht = 8 + ((aantal + 7) / 8) * 2;
            int verschilLengte = msg.Length - verwachtteLengteBericht;
            if (verschilLengte == 0)
            {
                //lengte OK
            }
            else if (verschilLengte > 0 && verschilLengte % (256 / 4) == 0)
            {
                //veelvoud van 256 aanwezig
                aantal = aantal + verschilLengte * 4;
            }
            else return null; //lengte onjuist

            bool[] result = new bool[aantal];
            int data = 0;
            for (int i = 0; i < aantal; i++)
            {
                int bit = 3 - (i % 4);
                if (bit == 3)
                {
                    int idx = 8 + (i / 4);
                    data = hex2int(msg[idx]);
                }
                result[i] = ((data >> bit) & 0x01) == 0x01;
            }

            return result;
        }

        public static Change[] DecodeWijzigingWPS(string msg)
        {
            //decodeer wijzigings bericht detectie: berichttype 0x14
            //  !NULL : geldige waarde -> bevat alleen de wijzigingen door combinaties van index en nieuwe waarde
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "1408C102"
            //  14: type
            //  08C: delta
            //  1: aantal wijzigingen (2 stuks)
            //  02: data (1x 2 chars) index 0=2

            //checks
            if (!msgOk(msg, 6, "14")) return null;

            //aantal wijzigingen bepalen
            int aantal = hex2int(msg[5]);
            if (msg.Length != 6 + (aantal * 2)) return null; //lengte onjuist

            Change[] result = new Change[aantal];
            for (int i = 0; i < aantal; i++)
            {
                result[i] = new Change();
                result[i].index = hex2int(msg[6 + i * 2]);
                result[i].value = hex2int(msg[7 + i * 2]);
            }

            return result;
        }

        public static Change[] DecodeWijzigingDet(string msg)
        {
            //decodeer wijzigings bericht detectie: berichttype 0x06
            //  !NULL : geldige waarde -> bevat alleen de wijzigingen door combinaties van index en nieuwe waarde
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "0636C202001401"
            //  06: type
            //  36C: delta
            //  2: aantal wijzigingen (2 stuks)
            //  0200 1401: data (2x 4 chars) index 2=0 (niet bezet), index 20=1(bezet)

            //checks
            if (!msgOk(msg, 6, "06")) return null;

            //aantal wijzigingen bepalen
            int aantal = hex2int(msg[5]);
            if (msg.Length != 6 + (aantal * 4)) return null; //lengte onjuist

            Change[] result = new Change[aantal];
            for (int i = 0; i < aantal; i++)
            {
                result[i] = new Change();
                result[i].index = (hex2int(msg[6 + i * 4]) << 4) + hex2int(msg[7 + i * 4]);
                result[i].value = hex2int(msg[9 + i * 4]);
            }

            return result;
        }

        public static Change[] DecodeWijzigingFcInt(string msg)
        {
            //decodeer wijzigings bericht interne signaalgroepen: berichttype 0x0A
            //  !NULL : geldige waarde -> bevat alleen de wijzigingen door combinaties van index en nieuwe waarde
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "0A0162010062090063"
            //  0A: type
            //  016: delta
            //  2: aantal wijzigingen (2 stuks)
            //  010062 090063: data (2x 6 chars: 2 index + 1 dummy + 3 value) index 1=98 (aanvraag, primair, vastgroen), index 9=99(aanvraag, primair, wachtgroen)

            //checks
            if (!msgOk(msg, 6, "0A")) return null;

            //aantal wijzigingen bepalen
            int aantal = hex2int(msg[5]);
            if (msg.Length != 6 + (aantal * 6)) return null; //lengte onjuist

            Change[] result = new Change[aantal];
            for (int i = 0; i < aantal; i++)
            {
                result[i] = new Change();
                result[i].index = (hex2int(msg[6 + i * 6]) << 4) + hex2int(msg[7 + i * 6]);
                result[i].value = (hex2int(msg[9 + i * 6]) << 8) + (hex2int(msg[10 + i * 6]) << 4) + hex2int(msg[11 + i * 6]);
            }

            return result;
        }

        public static Change[] DecodeWijzigingFcExt(string msg)
        {
            //decodeer wijzigings bericht externe signaalgroepen: berichttype 0x0E
            //  !NULL : geldige waarde -> bevat alleen de wijzigingen door combinaties van index en nieuwe waarde
            //  NULL  : onjuist bericht

            //voorbeeld bericht: "0E0033000213031401"
            //  0A: type
            //  003: delta
            //  3: aantal wijzigingen (3 stuks)
            //  0002 1303 1401": data (3x 4 chars)  [0]=2 [19]=3 [20]=1

            //checks
            if (!msgOk(msg, 6, "0E")) return null;

            //aantal wijzigingen bepalen
            int aantal = hex2int(msg[5]);
            if (msg.Length != 6 + (aantal * 4)) return null; //lengte onjuist

            Change[] result = new Change[aantal];
            for (int i = 0; i < aantal; i++)
            {
                result[i] = new Change();
                result[i].index = (hex2int(msg[6 + i * 4]) << 4) + hex2int(msg[7 + i * 4]);
                result[i].value = hex2int(msg[9 + i * 4]);
            }

            return result;
        }

        private static int hex2int(char ch)
        {
            if (ch >= '0' && ch <= '9')
                return ch - '0';
            if (ch >= 'A' && ch <= 'F')
                return ch - 'A' + 10;
            if (ch >= 'a' && ch <= 'f')
                return ch - 'a' + 10;
            return -1;
        }

        private static int hex2int(char msCh, char lsCh)
        {
            return (hex2int(msCh) << 4) + hex2int(lsCh);
        }

        private static int hex2int(char msCh, char Ch, char lsCh)
        {
            return (hex2int(msCh) << 8) + (hex2int(Ch) << 4) + hex2int(lsCh);
        }

        /// <summary>
        /// Check of het bericht voldoet aan een standaard V-Log bericht met minimum lengte, even aantal ASCII karakters en verwachtte type nummer
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="minimumLength"></param>
        /// <param name="msgType"></param>
        /// <returns></returns>
        private static bool msgOk(string msg, int minimumLength, string msgType = null)
        {
            //input check
            if (msg == null) return false;

            if (msg.Length < minimumLength) return false; //lengte onjuist
            if (msgType != null)
            {
                if (msg.Length < msgType.Length) return false; //lengte onjuist
                if (msg.Substring(0, msgType.Length) != msgType) return false; //berichttype onjuist
            }
            if (!IsHexAscii(msg)) return false; //geen ascii hex
            return true;
        }

        public static bool IsHexAscii(string msg)
        {
            if (msg == null) return false;
            if (msg.Length % 2 == 1) return false;//oneven aantal chars
            foreach (char c in msg)
            {
                if ((c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'F') ||
                    (c >= 'a' && c <= 'f'))
                {
                    //ok
                }
                else return false;
            }
            return true;
        }

        public static string RetainOnlyHex(string msg)
        {
            int l = msg.Length;
            char[] result = new char[l];
            for (int i = 0; i < l; i++)
            {
                char c = msg[i];
                if ((c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'F') ||
                    (c >= 'a' && c <= 'f'))
                {
                    result[i] = c;
                }
                else
                {
                    result[i] = '?';
                }
            }

            return new string(result);
        }

        public enum FcState { groen, geel, rood, unknown }
        public enum FcExternState { rood = 0, groen = 1, geel = 2, witKnipperen = 3, gedoofd = 4, geelKnipperen = 5 }

        public static FcState FcExternToFc(int fcExtern)
        {
            switch (fcExtern)
            {
                case (int)FcExternState.rood:
                    return FcState.rood;
                case (int)FcExternState.groen:
                case (int)FcExternState.witKnipperen:
                case (int)FcExternState.gedoofd:
                case (int)FcExternState.geelKnipperen:
                    return FcState.groen;
                case (int)FcExternState.geel:
                    return FcState.geel;
                default:
                    return FcState.unknown;
            }
        }

        public static FcState FcInternToFc(int fcIntern)
        {
            switch (fcIntern & 0x001F)
            {
                case 0:
                case 7:
                    return FcState.rood;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return FcState.groen;
                case 6:
                    return FcState.geel;
                default:
                    return FcState.unknown;
            }
        }

        public static bool FcExternSequenceOk(int[] fcsOld, int[] fcsNew,int wps0=5)
        {
            if (fcsOld == null || fcsNew == null) throw new ArgumentException("FcExternSequenceOk() NULL argument");
            if (fcsOld.Length != fcsNew.Length) return false; //lengte verschillend, dus volgorde per definitie fout

            bool result = true;
            for (int i = 0; i < fcsOld.Length; i++)
            {
                result &= FcExternSequenceOk(fcsOld[i], fcsNew[i], wps0);
            }
            return result;
        }

        public static bool FcExternSequenceOk(int fcOld, int fcNew, int wps0 = 5)
        {
            FcState oldValue = FcExternToFc(fcOld);
            FcState newValue = FcExternToFc(fcNew);
            return FcExternSequenceOk(oldValue, newValue, wps0);
        }

        public static bool FcExternSequenceOk(FcState oldValue, FcState newValue, int wps0 = 5)
        {
            bool result = true;
            if (wps0 != 5) return true; //bij niet regelen, altijd OK
            switch (oldValue)
            {
                case FcState.groen:
                    if (newValue != FcState.groen && newValue != FcState.geel) result = false; //alleen groen blijven of naar geel gaan is OK
                    break;
                case FcState.geel:
                    if (newValue != FcState.geel && newValue != FcState.rood) result = false; //alleen geel blijven of naar rood gaan is OK 
                    break;
                case FcState.rood:
                    if (newValue != FcState.rood && newValue != FcState.groen) result = false; //alleen rood blijven of naar groen gaan is OK
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }
    }

    public class Change
    {
        public int index = 0;
        public int value = 0;

        public override string ToString()
        {
            return string.Format("[{0}]={1}", index, value);
        }
    }

    public class VLogInfo
    {
        public string VriId = "";
        public string VLogVersie = "";

        public override string ToString()
        {
            return string.Format("VriID={0} V-Log Versie={1}", VriId, VLogVersie);
        }
    }
}
