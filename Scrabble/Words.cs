using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scrabble {
    internal static class Words {
        public static ConcurrentDictionary<string, string> Definitions = new ConcurrentDictionary<string, string>();
        public static HashSet<string> WordSet = new HashSet<string>();
        private static bool _loaded;

        public static void LoadDictionaries() {
            if (_loaded) {
                return;
            }

            WordSet = new HashSet<string>(
                File.ReadAllLines("Words/Words.txt")
            );
            WordSet.RemoveRange(0, 2);

            List<string> defsList = File.ReadAllLines("Words/Definitions.txt").ToList();
            defsList.RemoveRange(0, 2);

            Parallel.For(
                0, defsList.Count, index => {
                    string line = defsList[index];
                    string[] separated = line.Split(null, 2);

                    if (separated.Length < 2) {
                        return;
                    }

                    string word = separated[0];
                    string def = separated[1];

                    var squareBracketRegex = new Regex(@"\[([^\]]+)\]");
                    def = squareBracketRegex.Replace(def, "");
                    def = def.Replace("\"", "");

                    Definitions.TryAdd(word, def);
                }
            );

            _loaded = true;
        }
    }
}