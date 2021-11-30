using System.Collections.Generic;
using System.Linq;

namespace Scrabble {
    public class Board {
        public enum MoveType {
            Self,
            Opponent
        }

        public enum RelativePosition {
            Left,
            Right,
            Up,
            Down
        }

        public enum Square {
            Normal,
            Middle,
            DoubleLetter,
            TripleLetter,
            DoubleWord,
            TripleWord
        }

        public readonly List<int> Columns = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList();

        private readonly Dictionary<(int column, char row), char> Letters =
            new Dictionary<(int column, char row), char>();

        public readonly HashSet<char> PlacedLetters = new HashSet<char>();

        public readonly List<char> Rows = "ABCDEFGHIJKLMNO".ToCharArray().ToList();

        public readonly Dictionary<(int column, char row), Square> Squares =
            new Dictionary<(int column, char row), Square>();

        public Board() {
            LoadSquares();
        }

        private static void GetPremiumPositions(out List<(int, int)> doubleWords,
                                                out List<(int, int)> tripleWords,
                                                out List<(int, int)> doubleLetters,
                                                out List<(int, int)> tripleLetters) {
            // These co-ordinates are (columnNumber, rowIndex + 1)
            tripleWords = new[] {
                (1, 1),
                (8, 1),
                (15, 1),
                (1, 8),
                (15, 8),
                (1, 15),
                (8, 15),
                (15, 15)
            }.ToList();

            doubleLetters = new[] {
                (4, 1),
                (12, 1),
                (7, 3),
                (9, 3),
                (8, 4),
                (1, 4),
                (15, 4),
                (3, 7),
                (7, 7),
                (9, 7),
                (13, 7),
                (4, 8),
                (12, 8),
                (3, 9),
                (7, 9),
                (9, 9),
                (13, 9),
                (1, 12),
                (8, 12),
                (15, 12),
                (7, 13),
                (9, 13),
                (4, 15),
                (12, 15)
            }.ToList();

            tripleLetters = new[] {
                (6, 2),
                (10, 2),
                (2, 6),
                (6, 6),
                (10, 6),
                (14, 6),
                (2, 10),
                (6, 10),
                (10, 10),
                (14, 10),
                (6, 14),
                (10, 14)
            }.ToList();

            doubleWords = new[] {
                (2, 2),
                (3, 3),
                (4, 4),
                (5, 5),
                (14, 2),
                (13, 3),
                (12, 4),
                (11, 5),
                (5, 11),
                (4, 12),
                (3, 13),
                (2, 14),
                (11, 11),
                (12, 12),
                (13, 13),
                (14, 14)
            }.ToList();
        }

        private void LoadSquares() {
            if (Squares.Keys.Count >= 1) {
                return;
            }

            // Fill a List with normal squares. We add special ones later.
            int squareCount = Rows.Count * Columns.Count;
            List<Square> squareList = Enumerable.Repeat(Square.Normal, squareCount).ToList();

            // Add the middle.
            squareList[squareCount / 2] = Square.Middle;

            GetPremiumPositions(
                out List<(int, int)> doubleWords, out List<(int, int)> tripleWords, out List<(int, int)> doubleLetters,
                out List<(int, int)> tripleLetters
            );

            (int column, char row) CoordinateToSquare((int, int) co) {
                (int item1, int item2) = co;
                return (item1, Rows[item2 - 1]);
            }

            var listIndex = 0;
            foreach (char row in Rows) {
                foreach (int column in Columns) {
                    Squares[(column, row)] = squareList[listIndex];
                    listIndex++;
                }
            }

            foreach ((int, int) co in tripleWords) {
                Squares[CoordinateToSquare(co)] = Square.TripleWord;
            }

            foreach ((int, int) co in tripleLetters) {
                Squares[CoordinateToSquare(co)] = Square.TripleLetter;
            }

            foreach ((int, int) co in doubleWords) {
                Squares[CoordinateToSquare(co)] = Square.DoubleWord;
            }

            foreach ((int, int) co in doubleLetters) {
                Squares[CoordinateToSquare(co)] = Square.DoubleLetter;
            }
        }

        // Reading
        public Square GetSquare((int column, char row) pos) {
            LoadSquares(); // Load squares if required.
            return Squares[pos];
        }

        public char GetSquareContents((int column, char row) pos) {
            return Letters.Keys.FastContains(pos) ? Letters[pos] : ' ';
        }

        public bool IsEmpty((int column, char row) pos) {
            return !Letters.Keys.FastContains(pos);
        }

        // Get the squares surrounding the passed one and return a dictionary.
        public Dictionary<RelativePosition, (int column, char row)> GetSurroundingDict((int column, char row) pos) {
            (int, char) oneUp = pos.row != 'A' ? (pos.column, Rows[Rows.IndexOf(pos.row) - 1]) : (-1, 'X');
            (int, char) oneDown = pos.row != 'O' ? (pos.column, Rows[Rows.IndexOf(pos.row) + 1]) : (-1, 'X');
            (int, char) oneLeft = pos.column != 1 ? (pos.column - 1, pos.row) : (-1, 'X');
            (int, char) oneRight = pos.column != 15 ? (pos.column + 1, pos.row) : (-1, 'X');

            bool TestValid((int column, char row) poz) {
                return !poz.Equals((-1, 'X'));
            }

            var dict = new Dictionary<RelativePosition, (int column, char row)>();

            void AddIfValid((int column, char row) poz, RelativePosition rel) {
                if (TestValid(poz)) {
                    dict.Add(rel, poz);
                }
            }

            AddIfValid(oneUp, RelativePosition.Up);
            AddIfValid(oneDown, RelativePosition.Down);
            AddIfValid(oneLeft, RelativePosition.Left);
            AddIfValid(oneRight, RelativePosition.Right);

            return dict;
        }

        public List<(int column, char row)> GetSurrounding((int column, char row) pos) {
            (int, char) oneUp = pos.row != 'A' ? (pos.column, Rows[Rows.IndexOf(pos.row) - 1]) : (-1, 'X');
            (int, char) oneDown = pos.row != 'O' ? (pos.column, Rows[Rows.IndexOf(pos.row) + 1]) : (-1, 'X');
            (int, char) oneLeft = pos.column != 1 ? (pos.column - 1, pos.row) : (-1, 'X');
            (int, char) oneRight = pos.column != 15 ? (pos.column + 1, pos.row) : (-1, 'X');

            (int, char)[] boundingSquares = { oneUp, oneDown, oneLeft, oneRight };

            var surrounding = new List<(int column, char row)>();
            foreach ((int, char) bSquare in boundingSquares) {
                if (!bSquare.Equals((-1, 'X'))) {
                    surrounding.Add((bSquare.Item1, bSquare.Item2));
                }
            }

            return surrounding;
        }

        /*
        public List<List<(int column, char row)>> LoadAllRows() {

        }
        */
        public List<(int column, char row)> GetRow(char row) {
            var found = new List<(int column, char row)>();

            var foundNumber = 0;
            (int column, char row)[] sqs = Squares.Keys.ToArray();

            for (int i = 0, c = sqs.Length; i < c && foundNumber <= 14; i++) {
                (int column, char row) sq = sqs[i];
                if (sq.row != row) {
                    continue;
                }

                found.Add(sq);
                foundNumber++;
            }

            return found;
        }

        public bool RowIsEmpty(char row) {
            List<(int column, char row)> squares = GetRow(row);
            for (var i = 0; i < squares.Count; i++) {
                (int column, char row) square = squares[i];
                if (!IsEmpty(square)) {
                    return false;
                }
            }

            return true;
        }

        public List<(int column, char row)> GetColumn(int column) {
            var found = new List<(int column, char row)>();

            var foundNumber = 0;
            foreach ((int column, char row) sq in Squares.Keys) {
                if (foundNumber > 14) {
                    break;
                }

                if (sq.column != column) {
                    continue;
                }

                found.Add(sq);
                foundNumber++;
            }

            return found;
        }

        // Writing
        public void SetSquareContents((int column, char row) pos, char letter) {
            Letters[pos] = letter;
        }

        public void ExecuteMove(Brain.Move move, MoveType type) {
            // Update the board for a move.
            if (move.Equals(Brain.Move.Err)) {
                return;
            }

            List<(int column, char row)> affected = Game.Brain.AffectedSquares(move);
            for (var i = 0; i < affected.Count; i++) {
                PlacedLetters.Add(move.Word[i]);
                SetSquareContents(affected[i], move.Word[i]);
            }

            ReloadPremiums();

            if (type == MoveType.Self) {
                Game.OwnMoves.Add(move);
            } else {
                Game.OpponentMoves.Add(move);
            }
        }

        private void ReloadPremiums() {
            // Remove any premium squares that have been used.
            for (var i = 0; i < Squares.Keys.Count; i++) {
                (int column, char row) sq = Squares.Keys.ToList()[i];
                if (GetSquareContents(sq) != ' ') {
                    Squares[sq] = Square.Normal;
                }
            }
        }
    }
}