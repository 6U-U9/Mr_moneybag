using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mr_MoneyBag
{
    public partial class Form1 : Form
    {
        public const double blocksize = 30;
        public int x, y;
        public PictureBox[,] map = new PictureBox[30, 18];
        Gameboard gameboard = new Gameboard();
        private bool is_space_down = false;
        public Form1()
        {
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    map[i, j].Image = gameboard.status[i, j].getimage();
                }
            InitializeComponent();
        }

        protected override bool ProcessDialogKey(Keys keycode)
        {
            switch (keycode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return false;
                default:
                    return base.ProcessDialogKey(keycode);
            }
        }
    }
}
