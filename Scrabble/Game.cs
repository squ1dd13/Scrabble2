using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Scrabble.Utility;

namespace Scrabble {
    public static class Game {
        public static readonly List<char> Letters = new List<char>();

        public static readonly List<Brain.Move> OwnMoves = new List<Brain.Move>();
        public static readonly List<Brain.Move> OpponentMoves = new List<Brain.Move>();

        public static Board Board = new Board();

        public static readonly Brain Brain = new Brain();
        public static int BlankCount = 0;

        public static void Save() {
            return;

/*
            using Stream stream = new FileStream("save.sav", FileMode.Create);
            using var writer = new StreamWriter(stream);

            // Save our letters and if we don't have 7 just pad.
            for (var i = 0; i < 7; i++) {
                writer.Write(i >= Letters.Count ? ' ' : Letters[i]);
            }

            // To save a move, we just write the word followed by the indexes of the first and last squares.
            foreach (Brain.Move move in OwnMoves) {
                var idx1 = Board.Squares.Keys.ToList().IndexOf(move.FirstLetterPos).ToString("00");
                var idx2 = Board.Squares.Keys.ToList().IndexOf(move.LastLetterPos).ToString("00");

                writer.Write($"{move.Word}{idx1}{idx2}");
            }

            writer.Write("/");
            foreach (Brain.Move move in OpponentMoves) {
                var idx1 = Board.Squares.Keys.ToList().IndexOf(move.FirstLetterPos).ToString("00");
                var idx2 = Board.Squares.Keys.ToList().IndexOf(move.LastLetterPos).ToString("00");

                writer.Write($"{move.Word}{idx1}{idx2}");
            }

            writer.Write('/');

            // Save the letters in order.
            const string rows = "ABCDEFGHIJKLMNO";
            for (var i = 0; i < 15; i++) {
                List<(int column, char row)> squares = Board.GetRow(rows[i]);
                for (var j = 0; j < squares.Count; j++) {
                    (int column, char row) square = squares[j];
                    writer.Write(Board.GetSquareContents(square));
                }
            }

            writer.Flush();
*/
        }

        public static void Load() {
            using Stream stream = new FileStream("save.sav", FileMode.Open);
            using var reader = new StreamReader(stream);

            Write("Loading letters... ", ConsoleColor.Red);
            // Read all 7 chars, even if they are spaces. We can remove those after.
            for (var i = 0; i < 7; i++) {
                Letters.Add((char)reader.Read());
            }

            Letters.RemoveAll(ch => ch == ' ');
            Write("Done!", ConsoleColor.DarkCyan);
            Console.WriteLine();

            Write("Loading moves (1/2)... ", ConsoleColor.Red);
            while (true) {
                var read = new List<char>();
                char ch;

                var wdB = new StringBuilder();
                while (char.IsLetter(ch = (char)reader.Read())) {
                    wdB.Append(ch);
                    read.Add(ch);
                }

                if (ch == '/') {
                    break;
                }

                var idxB = new StringBuilder();
                while (char.IsDigit(ch = (char)reader.Read())) {
                    idxB.Append(ch);
                    read.Add(ch);
                }

                char[] chs = idxB.ToString().ToCharArray();
                int.TryParse(string.Join("", chs.Take(2)), out int idx1);
                int.TryParse(string.Join("", chs.Skip(2).Take(2)), out int idx2);

                var move = new Brain.Move(
                    wdB.ToString(),
                    Board.Squares.Keys.ToArray()[idx1],
                    Board.Squares.Keys.ToArray()[idx2]
                );
                OwnMoves.Add(move);
                if (ch == '/') {
                    break;
                }

                if (read.FastContains('/')) {
                    break;
                }
            }

            Write("Done!", ConsoleColor.DarkCyan);

            Console.WriteLine();
            Write("Loading moves (2/2)... ", ConsoleColor.Red);
            while (true) {
                var read = new List<char>();
                char ch;

                var wdB = new StringBuilder();
                while (char.IsLetter(ch = (char)reader.Read())) {
                    wdB.Append(ch);
                    read.Add(ch);
                }

                if (ch == '/') {
                    break;
                }

                var idxB = new StringBuilder();
                while (char.IsDigit(ch = (char)reader.Read())) {
                    idxB.Append(ch);
                    read.Add(ch);
                }

                char[] chs = idxB.ToString().ToCharArray();
                int.TryParse(string.Join("", chs.Take(2)), out int idx1);
                int.TryParse(string.Join("", chs.Skip(2).Take(2)), out int idx2);

                var move = new Brain.Move(
                    wdB.ToString(),
                    Board.Squares.Keys.ToArray()[idx1],
                    Board.Squares.Keys.ToArray()[idx2]
                );
                OpponentMoves.Add(move);
                if (ch == '/') {
                    break;
                }

                if (read.FastContains('/')) {
                    break;
                }
            }

            Write("Done!", ConsoleColor.DarkCyan);
            Console.WriteLine();

            Write("Loading board... ", ConsoleColor.Red);

            const string rows = "ABCDEFGHIJKLMNO";
            for (var i = 0; i < 15; i++) {
                List<(int column, char row)> squares = Board.GetRow(rows[i]);

                foreach ((int column, char row) square in squares) {
                    char ch;
                    if ((ch = (char)reader.Read()) != ' ') {
                        Board.SetSquareContents(square, ch);
                    }
                }
            }

            Write("Done!", ConsoleColor.DarkCyan);
            Console.WriteLine();
        }
    }
}