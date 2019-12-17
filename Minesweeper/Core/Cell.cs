using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper.Core
{
    public enum CellType
    {
        Regular,
        Mine,
        Flagged,
        FlaggedMine
    }

    public enum CellState
    {
        Opened,
        Closed
    }

    public class Cell : Button
    {
        public int XLoc;
        public int YLoc;
        public int CellSize;
        public CellState CellState;
        public CellType CellType;
        public int NumMines;
        public Board Board;


        public void SetupDesign()
        {
            Location = new Point(XLoc * CellSize, YLoc * CellSize);
            Size = new Size(CellSize, CellSize);
            UseVisualStyleBackColor = false;
            Font = new Font("Verdana", 15.75F, FontStyle.Bold);
        }

        public bool IsMine()
        {
            return CellType == CellType.Mine ||
                CellType == CellType.FlaggedMine;
        }

        public void OnFlag()
        {
            switch (CellType)
            {
                case CellType.Regular:
                    CellType = CellType.Flagged;
                    break;
                case CellType.Mine:
                    CellType = CellType.FlaggedMine;
                    break;
                case CellType.Flagged:
                    CellType = CellType.Regular;
                    break;
                case CellType.FlaggedMine:
                    CellType = CellType.Mine;
                    break;
                default:
                    throw new Exception("Error in cell types");
            }

            UpdateDisplay();
        }

        public void OnClick(bool recursiveCall = false)
        {
            if (recursiveCall)
            {
                if (CellType != CellType.Regular || CellState != CellState.Closed)
                    return;
            }

            if (CellType == CellType.Mine)
            {
                CellState = CellState.Opened;
                UpdateDisplay();

                for (var x = 0; x < Board.Width; x++)
                {
                    for (var y = 0; y < Board.Height; y++)
                    {
                        Board.Cells[x, y].CellState = CellState.Opened;
                        Board.Cells[x, y].UpdateDisplay();
                    }
                }

                MessageBox.Show("You clicked on a mine, Game Over!");

                Board.RestartGame();
                return;
            }

            if (CellType == CellType.Regular)
            {
                CellState = CellState.Opened;
                UpdateDisplay();
            }

            if (NumMines == 0)
            {
                var neighbors = GetNeighborCells();
                foreach (var n in neighbors)
                    n.OnClick(true);
            }
        }

        public List<Cell> GetNeighborCells()
        {
            var neighbors = new List<Cell>();

            for (var x = -1; x < 2; x++)
            {
                for (var y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (XLoc + x < 0 || XLoc + x >= Board.Width || YLoc + y < 0 || YLoc + y >= Board.Height)
                        continue;

                    neighbors.Add(Board.Cells[XLoc + x, YLoc + y]);
                }
            }

            return neighbors;
        }

        public void UpdateDisplay()
        {

            if (CellType == CellType.Flagged || CellType == CellType.FlaggedMine)
            {
                BackColor = Color.Gray;
                ForeColor = Color.White;
                Text = "🏴‍";
                return;
            }

            if (CellState == CellState.Closed)
            {
                BackColor = SystemColors.ButtonFace;
                Text = string.Empty;
                return;
            }

            if (CellType == CellType.Mine)
            {
                BackColor = Color.DarkRed;
                ForeColor = Color.White;
                Text = "💣";
            }

            if (CellType == CellType.Regular)
            {
                BackColor = Color.LightGray;
                ForeColor = GetCellColour();
                Text = NumMines > 0 ? string.Format("{0}", NumMines) : string.Empty;
            }
        }

        private Color GetCellColour()
        {
            switch (NumMines)
            {
                case 1:
                    return ColorTranslator.FromHtml("0x0000FE");
                case 2:
                    return ColorTranslator.FromHtml("0x186900");
                case 3:
                    return ColorTranslator.FromHtml("0xAE0107");
                case 4:
                    return ColorTranslator.FromHtml("0x000177");
                case 5:
                    return ColorTranslator.FromHtml("0x8D0107");
                case 6:
                    return ColorTranslator.FromHtml("0x007A7C");
                case 7:
                    return ColorTranslator.FromHtml("0x902E90");
                case 8:
                    return ColorTranslator.FromHtml("0x000000");
                default:
                    return ColorTranslator.FromHtml("0xffffff");
            }
        }
    }
}