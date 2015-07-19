using System;
using System.Collections.Generic;

namespace KeyConvert.Convert
{
    /// <summary>
    /// Contains conversion dictionaries for keys
    /// </summary>
    public class KeyDictionaries
    {
        public static readonly Dictionary<String, String> CamelotDictionary = new Dictionary<string, string>
        {
            // map beatport notation to camelot
            {"A#maj", "6B"},
            {"A#min", "3A"},
            {"Amaj", "11B"},
            {"Amin", "8A"},
            {"Bmaj", "1B"},
            {"Bmin", "10A"},
            {"C#maj", "3B"},
            {"C#min", "12A"},
            {"Cmaj", "8B"},
            {"Cmin", "5A"},
            {"D#maj", "5B"},
            {"D#min", "2A"},
            {"Dmaj", "10B"},
            {"Dmin", "7A"},
            {"Emaj", "12B"},
            {"Emin", "9A"},
            {"F#maj", "2B"},
            {"F#min", "11A"},
            {"Fmaj", "7B"},
            {"Fmin", "4A"},
            {"G#maj", "4B"},
            {"G#min", "1A"},
            {"Gmaj", "9B"},
            {"Gmin", "6A"},

            //map open key (traktor) to camelot
            {"1m", "8A"},
            {"1d", "8B"},
            {"2m", "9A"},
            {"2d", "9B"},
            {"3m", "10A"},
            {"3d", "10B"},
            {"4m", "11A"},
            {"4d", "11B"},
            {"5m", "12A"},
            {"5d", "12B"},
            {"6m", "1A"},
            {"6d", "1B"},
            {"7m", "2A"},
            {"7d", "2B"},
            {"8m", "3A"},
            {"8d", "3B"},
            {"9m", "4A"},
            {"9d", "4B"},
            {"10m", "5A"},
            {"10d", "5B"},
            {"11m", "6A"},
            {"11d", "6B"},
            {"12m", "7A"},
            {"12d", "7B"}
        };
    }
}
