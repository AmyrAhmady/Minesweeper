using System;
using System.Linq;
using System.Windows.Forms;

namespace Minesweeper.Core
{
    public class Board
    {
        public Minesweeper Minesweeper;
        public int Width;
        public int Height;
        public int NumMines;
        public Cell[,] Cells;

        public Board(Minesweeper minesweeper, int width, int height, int mines)
        {
            Minesweeper = minesweeper;
            Width = width;
            Height = height;
            NumMines = mines;
            Cells = new Cell[width, height];
        }

        public void SetupBoard()
        {
            for (var x = 1; x <= Width; x++)
            {
                for (var y = 1; y <= Height; y++)
                {
                    var c = new Cell
                    {
                        XLoc = x - 1,
                        YLoc = y - 1,
                        CellState = CellState.Closed,
                        CellType = CellType.Regular,
                        CellSize = 50,
                        Board = this
                    };
                    c.SetupDesign();
                    c.MouseDown += Cell_MouseClick;

                    Cells[x - 1, y - 1] = c;
                    Minesweeper.Controls.Add(c);
                }
            }
        }

        public void PlaceMines()
        {
            var minesPlaced = 0;
            var random = new Random();

            while (minesPlaced < NumMines)
            {
                int x = random.Next(0, Width);
                int y = random.Next(0, Height);

                if (!Cells[x, y].IsMine())
                {
                    Cells[x, y].CellType = CellType.Mine;
                    minesPlaced += 1;
                }
            }

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var c = Cells[x, y];
                    c.UpdateDisplay();
                    c.NumMines = c.GetNeighborCells().Where(n => n.IsMine()).Count();
                }
            }
        }

        private void Cell_MouseClick(object sender, MouseEventArgs e)
        {
            var cell = (Cell)sender;

            if (cell.CellState == CellState.Opened)
                return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    cell.OnClick();
                    break;

                case MouseButtons.Right:
                    cell.OnFlag();
                    break;

                default:
                    return;
            }

            CheckForWin();
        }

        private void CheckForWin()
        {
            //var correctMines = 0;
            //var incorrectMines = 0;
            var openedCells = 0;

            /*for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var c = Cells[x, y];
                    if (c.CellType == CellType.Flagged)
                        incorrectMines += 1;
                    if (c.CellType == CellType.FlaggedMine)
                        correctMines += 1;
                }
            }

            if (correctMines == NumMines && incorrectMines == 0)
            {
                MessageBox.Show("You won!");
                RestartGame();
            }*/

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var c = Cells[x, y];
                    if (c.CellState == CellState.Opened)
                        openedCells++;
                }
            }

            if (openedCells == (Cells.Length - NumMines))
            {
                MessageBox.Show("You won!");
                RestartGame();
            }
        }

        public void RestartGame()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var c = Cells[x, y];
                    Minesweeper.Controls.Remove(c);
                }
            }

            SetupBoard();
            PlaceMines();
        }
    }
}
