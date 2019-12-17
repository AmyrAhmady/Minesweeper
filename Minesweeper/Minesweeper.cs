using Minesweeper.Core;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Minesweeper : Form
    {
        public Minesweeper()
        {
            InitializeComponent();
            var board = new Board(this, 9, 9, 10);
            board.SetupBoard();
            board.PlaceMines();
        }
    }
}
