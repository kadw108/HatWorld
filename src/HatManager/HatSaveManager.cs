using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HatWorld
{
    public class HatSaveManager
    {
        // map of scav id : physical type of worn hat (saved as type.Namespace + . + type.Name) to keep track of hats throughout games
        public static Dictionary<EntityID, string> hats = new Dictionary<EntityID, string>();

        public static void AddHooks()
        {
            // make sure hats of which creature wears which hat persists between games - requires saving hats to file
            On.SaveState.SaveToString += SaveState_SaveToString;
            On.SaveState.LoadGame += SaveState_LoadGame;
        }

        private static void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState self, string str, RainWorldGame game)
        {
            orig(self, str, game);

            string results = SearchForSavePair(str, "HATWORLD", "<svB>", "<svA>");
            Debug.Log("HatWorld: load hats " + results);
            var persistData = DataFromString(results);
            hats = persistData;
        }

        private static string SaveState_SaveToString(On.SaveState.orig_SaveToString orig, SaveState self)
        {
            StringBuilder sb = new StringBuilder();

            string customData = DataToString(hats);

            sb.Append("HATWORLD<svB>");
            sb.Append(customData ?? "");
            sb.Append("<svA>");

            Debug.Log("HatWorld: save hats " + customData);

            return orig(self) + sb.ToString();
        }

        /* from SlugBase.CustomSaveState */

        internal static Dictionary<EntityID, string> DataFromString(string dataString)
        {
            var data = new Dictionary<EntityID, string>();
            if (string.IsNullOrEmpty(dataString)) return data;
            string[] entries = dataString.Split(',');
            for (int i = 0; i < entries.Length; i++)
            {
                string entry = entries[i];
                int split = entry.IndexOf(':');
                if (split == -1) continue;

                string key = Unescape(entry.Substring(0, split));
                EntityID actualKey = EntityID.FromString(key);
                string value = Unescape(entry.Substring(split + 1));

                data[actualKey] = value;
            }
            return data;
        }

        private static string Unescape(string value)
        {
            StringBuilder sb = new StringBuilder(value.Length);
            int i = 0;
            bool escape = false;
            while (i < value.Length)
            {
                char c = value[i];
                if (escape)
                {
                    escape = false;
                    switch (c)
                    {
                        case '\\': sb.Append('\\'); break;
                        case 'L': sb.Append('<'); break;
                        case 'G': sb.Append('>'); break;
                        case 'C': sb.Append(':'); break;
                        case 'c': sb.Append(','); break;
                        default: sb.Append(c); break;
                    }
                }
                else
                {
                    if (c == '\\') escape = true;
                    else sb.Append(c);
                }
                i++;
            }
            return sb.ToString();
        }

        private static string SearchForSavePair(string input, string key, string separator, string cap)
        {
            int start = input.IndexOf(key + separator);
            if (start == -1) return null;
            start += key.Length + separator.Length;
            int end = input.IndexOf(cap, start);
            if (end == -1) return input.Substring(start);
            return input.Substring(start, end - start);
        }

        internal static string DataToString(Dictionary<EntityID, string> data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pair in data)
            {
                sb.Append(Escape(pair.Key.ToString()));
                sb.Append(':');
                sb.Append(Escape(pair.Value));
                sb.Append(',');
            }
            return sb.ToString();
        }

        private static string Escape(string value)
        {
            value = value.Replace("\\", "\\\\");
            value = value.Replace("<", "\\L");
            value = value.Replace(">", "\\G");
            value = value.Replace(":", "\\C");
            value = value.Replace(",", "\\c");
            return value;
        }
    }
}
