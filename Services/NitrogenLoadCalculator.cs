namespace divelog.Services
{
    public static class NitrogenLoadCalculator
    {
        public static string Calculate(double depth, int time)
        {
            //If-satser för att hitta korrekt kvävebelastning efter dyk

            if (depth <= 3 && time <= 58) return "A";
            if (depth <= 3 && time <= 100) return "B";
            if (depth <= 3 && time <= 156) return "C";
            if (depth <= 3 && time <= 240) return "D";
            if (depth <= 3 && time <= 410) return "E";
            if (depth <= 3 && time > 410) return "F";

            if (depth <= 4.5 && time <= 36) return "A";
            if (depth <= 4.5 && time <= 60) return "B";
            if (depth <= 4.5 && time <= 87) return "C";
            if (depth <= 4.5 && time <= 120) return "D";
            if (depth <= 4.5 && time <= 160) return "E";
            if (depth <= 4.5 && time <= 213) return "F";
            if (depth <= 4.5 && time <= 288) return "G";
            if (depth <= 4.5 && time <= 427) return "H";
            if (depth <= 4.5 && time > 427) return "I";

            if (depth <= 6 && time <= 26) return "A";
            if (depth <= 6 && time <= 43) return "B";
            if (depth <= 6 && time <= 61) return "C";
            if (depth <= 6 && time <= 81) return "D";
            if (depth <= 6 && time <= 104) return "E";
            if (depth <= 6 && time <= 131) return "F";
            if (depth <= 6 && time <= 162) return "G";
            if (depth <= 6 && time <= 200) return "H";
            if (depth <= 6 && time <= 250) return "I";
            if (depth <= 6 && time <= 319) return "J";
            if (depth <= 6 && time <= 436) return "K";
            if (depth <= 6 && time <= 1016) return "L";
            if (depth <= 6 && time > 1016) return "M";

            if (depth <= 7.5 && time <= 20) return "A";
            if (depth <= 7.5 && time <= 33) return "B";
            if (depth <= 7.5 && time <= 47) return "C";
            if (depth <= 7.5 && time <= 61) return "D";
            if (depth <= 7.5 && time <= 77) return "E";
            if (depth <= 7.5 && time <= 95) return "F";
            if (depth <= 7.5 && time <= 115) return "G";
            if (depth <= 7.5 && time <= 137) return "H";
            if (depth <= 7.5 && time <= 163) return "I";
            if (depth <= 7.5 && time <= 193) return "J";
            if (depth <= 7.5 && time <= 230) return "K";
            if (depth <= 7.5 && time <= 276) return "L";
            if (depth <= 7.5 && time <= 340) return "M";
            if (depth <= 7.5 && time <= 442) return "N";
            if (depth <= 7.5 && time <= 719) return "O";
            if (depth <= 7.5 && time <= 745) return "Z";

            if (depth <= 9 && time <= 17) return "A";
            if (depth <= 9 && time <= 27) return "B";
            if (depth <= 9 && time <= 38) return "C";
            if (depth <= 9 && time <= 49) return "D";
            if (depth <= 9 && time <= 62) return "E";
            if (depth <= 9 && time <= 75) return "F";
            if (depth <= 9 && time <= 90) return "G";
            if (depth <= 9 && time <= 105) return "H";
            if (depth <= 9 && time <= 123) return "I";
            if (depth <= 9 && time <= 142) return "J";
            if (depth <= 9 && time <= 164) return "K";
            if (depth <= 9 && time <= 189) return "L";
            if (depth <= 9 && time <= 218) return "M";
            if (depth <= 9 && time <= 253) return "N";
            if (depth <= 9 && time <= 297) return "O";
            if (depth <= 9 && time <= 356) return "Z";

            if (depth <= 10.5 && time <= 14) return "A";
            if (depth <= 10.5 && time <= 23) return "B";
            if (depth <= 10.5 && time <= 32) return "C";
            if (depth <= 10.5 && time <= 41) return "D";
            if (depth <= 10.5 && time <= 51) return "E";
            if (depth <= 10.5 && time <= 62) return "F";
            if (depth <= 10.5 && time <= 73) return "G";
            if (depth <= 10.5 && time <= 86) return "H";
            if (depth <= 10.5 && time <= 99) return "I";
            if (depth <= 10.5 && time <= 113) return "J";
            if (depth <= 10.5 && time <= 128) return "K";
            if (depth <= 10.5 && time <= 146) return "L";
            if (depth <= 10.5 && time <= 164) return "M";
            if (depth <= 10.5 && time <= 186) return "N";
            if (depth <= 10.5 && time <= 208) return "O";
            if (depth <= 10.5 && time > 208) return "Direktuppstigning överskriden!";

            if (depth <= 12 && time <= 12) return "A";
            if (depth <= 12 && time <= 19) return "B";
            if (depth <= 12 && time <= 27) return "C";
            if (depth <= 12 && time <= 35) return "D";
            if (depth <= 12 && time <= 44) return "E";
            if (depth <= 12 && time <= 53) return "F";
            if (depth <= 12 && time <= 62) return "G";
            if (depth <= 12 && time <= 72) return "H";
            if (depth <= 12 && time <= 83) return "I";
            if (depth <= 12 && time <= 94) return "J";
            if (depth <= 12 && time <= 106) return "K";
            if (depth <= 12 && time <= 119) return "L";
            if (depth <= 12 && time <= 133) return "M";
            if (depth <= 12 && time <= 148) return "N";
            if (depth <= 12 && time <= 151) return "O";
            if (depth <= 12 && time > 151) return "Direktuppstigning överskriden!";

            if (depth <= 13.5 && time <= 11) return "A";
            if (depth <= 13.5 && time <= 17) return "B";
            if (depth <= 13.5 && time <= 24) return "C";
            if (depth <= 13.5 && time <= 31) return "D";
            if (depth <= 13.5 && time <= 38) return "E";
            if (depth <= 13.5 && time <= 46) return "F";
            if (depth <= 13.5 && time <= 54) return "G";
            if (depth <= 13.5 && time <= 62) return "H";
            if (depth <= 13.5 && time <= 71) return "I";
            if (depth <= 13.5 && time <= 80) return "J";
            if (depth <= 13.5 && time <= 90) return "K";
            if (depth <= 13.5 && time <= 101) return "L";
            if (depth <= 13.5 && time <= 112) return "M";
            if (depth <= 13.5 && time > 112) return "Direktuppstigning överskriden!";

            if (depth <= 15 && time <= 9) return "A";
            if (depth <= 15 && time <= 15) return "B";
            if (depth <= 15 && time <= 21) return "C";
            if (depth <= 15 && time <= 27) return "D";
            if (depth <= 15 && time <= 34) return "E";
            if (depth <= 15 && time <= 41) return "F";
            if (depth <= 15 && time <= 48) return "G";
            if (depth <= 15 && time <= 55) return "H";
            if (depth <= 15 && time <= 62) return "I";
            if (depth <= 15 && time <= 70) return "J";
            if (depth <= 15 && time <= 79) return "K";
            if (depth <= 15 && time <= 85) return "L";
            if (depth <= 15 && time > 85) return "Direktuppstigning överskriden!";

            if (depth <= 16.5 && time <= 8) return "A";
            if (depth <= 16.5 && time <= 14) return "B";
            if (depth <= 16.5 && time <= 19) return "C";
            if (depth <= 16.5 && time <= 25) return "D";
            if (depth <= 16.5 && time <= 30) return "E";
            if (depth <= 16.5 && time <= 36) return "F";
            if (depth <= 16.5 && time <= 42) return "G";
            if (depth <= 16.5 && time <= 49) return "H";
            if (depth <= 16.5 && time <= 56) return "I";
            if (depth <= 16.5 && time <= 62) return "J";
            if (depth <= 16.5 && time <= 69) return "K";
            if (depth <= 16.5 && time > 69) return "Direktuppstigning överskriden!";

            if (depth <= 18 && time <= 7) return "A";
            if (depth <= 18 && time <= 12) return "B";
            if (depth <= 18 && time <= 17) return "C";
            if (depth <= 18 && time <= 22) return "D";
            if (depth <= 18 && time <= 27) return "E";
            if (depth <= 18 && time <= 33) return "F";
            if (depth <= 18 && time <= 38) return "G";
            if (depth <= 18 && time <= 44) return "H";
            if (depth <= 18 && time <= 50) return "I";
            if (depth <= 18 && time <= 56) return "J";
            if (depth <= 18 && time <= 59) return "K";
            if (depth <= 18 && time > 59) return "Direktuppstigning överskriden!";

            if (depth <= 21 && time <= 6) return "A";
            if (depth <= 21 && time <= 10) return "B";
            if (depth <= 21 && time <= 14) return "C";
            if (depth <= 21 && time <= 19) return "D";
            if (depth <= 21 && time <= 23) return "E";
            if (depth <= 21 && time <= 27) return "F";
            if (depth <= 21 && time <= 32) return "G";
            if (depth <= 21 && time <= 37) return "H";
            if (depth <= 21 && time <= 42) return "I";
            if (depth <= 21 && time <= 43) return "J";
            if (depth <= 21 && time > 43) return "Direktuppstigning överskriden!";

            if (depth <= 24 && time <= 5) return "A";
            if (depth <= 24 && time <= 9) return "B";
            if (depth <= 24 && time <= 12) return "C";
            if (depth <= 24 && time <= 16) return "D";
            if (depth <= 24 && time <= 20) return "E";
            if (depth <= 24 && time <= 24) return "F";
            if (depth <= 24 && time <= 27) return "G";
            if (depth <= 24 && time <= 32) return "H";
            if (depth <= 24 && time <= 33) return "I";
            if (depth <= 24 && time > 33) return "Direktuppstigning överskriden!";

            if (depth <= 27 && time <= 4) return "A";
            if (depth <= 27 && time <= 7) return "B";
            if (depth <= 27 && time <= 11) return "C";
            if (depth <= 27 && time <= 14) return "D";
            if (depth <= 27 && time <= 17) return "E";
            if (depth <= 27 && time <= 21) return "F";
            if (depth <= 27 && time <= 24) return "G";
            if (depth <= 27 && time <= 26) return "H";
            if (depth <= 27 && time > 26) return "Direktuppstigning överskriden!";

            if (depth <= 30 && time <= 4) return "A";
            if (depth <= 30 && time <= 6) return "B";
            if (depth <= 30 && time <= 9) return "C";
            if (depth <= 30 && time <= 12) return "D";
            if (depth <= 30 && time <= 15) return "E";
            if (depth <= 30 && time <= 18) return "F";
            if (depth <= 30 && time <= 21) return "G";
            if (depth <= 30 && time <= 22) return "H";
            if (depth <= 30 && time > 22) return "Direktuppstigning överskriden!";

            if (depth <= 33 && time <= 3) return "A";
            if (depth <= 33 && time <= 6) return "B";
            if (depth <= 33 && time <= 8) return "C";
            if (depth <= 33 && time <= 11) return "D";
            if (depth <= 33 && time <= 13) return "E";
            if (depth <= 33 && time <= 16) return "F";
            if (depth <= 33 && time <= 17) return "G";
            if (depth <= 33 && time > 17) return "Direktuppstigning överskriden!";

            if (depth <= 36 && time <= 3) return "A";
            if (depth <= 36 && time <= 5) return "B";
            if (depth <= 36 && time <= 7) return "C";
            if (depth <= 36 && time <= 10) return "D";
            if (depth <= 36 && time <= 12) return "E";
            if (depth <= 36 && time <= 15) return "F";
            if (depth <= 36 && time > 15) return "Direktuppstigning överskriden!";

            if (depth <= 39 && time <= 2) return "A";
            if (depth <= 39 && time <= 4) return "B";
            if (depth <= 39 && time <= 6) return "C";
            if (depth <= 39 && time <= 9) return "D";
            if (depth <= 39 && time <= 11) return "E";
            if (depth <= 39 && time <= 12) return "F";
            if (depth <= 39 && time > 12) return "Direktuppstigning överskriden!";

            if (depth <= 42 && time <= 2) return "A";
            if (depth <= 42 && time <= 4) return "B";
            if (depth <= 42 && time <= 6) return "C";
            if (depth <= 42 && time <= 8) return "D";
            if (depth <= 42 && time <= 10) return "E";
            if (depth <= 42 && time > 10) return "Direktuppstigning överskriden!";

            return "Direktuppstigning överskriden!";
        }
    }
}