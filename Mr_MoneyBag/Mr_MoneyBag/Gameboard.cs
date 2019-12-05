using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Mr_MoneyBag
{
    class Gameboard
    {
        const int width=99, height=99;
        public int level;
        public int turn = 0;
        public GameObject[,] status = new Space[height, width];
        public GameObject Player = new Player(3, 50, 50);
        public Gameboard()
        {
            level = 1;
            genlevel(level); 
        }
        public void genlevel(int level)
        { 
        }

        public int GetSize()
        {
            return width * height + 1;
        }
    }
}
