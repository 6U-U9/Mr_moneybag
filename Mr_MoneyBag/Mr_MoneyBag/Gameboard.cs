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
        public const int width=39, height=39;
        readonly int default_cof = 24, default_nrg = 20, default_rnd = 20;
        readonly int default_sr = 3;
        readonly double default_st = 5.9;
        readonly int[] default_sa = { 2, 2, 2, 2, 2, 2 }; // coinonfloor, newredgen, rednoticedist, sight, damage, moneylimit, 


        static Random rnd = new Random();
        public int level;
        public int turn = 0;
        public int coinsonfloor = 24, newredgen = 20, rednoticedist = 20,  InitPlayerMoneyLimit = 5;
        public int shootrange = 3;
        public double sight = 5.9;
        public int[] shop_amount = new int[] { 2, 2, 2, 2, 2, 2 }; // coinonfloor, newredgen, rednoticedist, sight, damage, moneylimit, 
        public GameObject[,] status = new GameObject[height, width];
        public const int initial_x = 20;
        public const int initial_y = 20;
        public Player player;
        public int timer = 0;
        public List<Enemy> enemies = new List<Enemy>();

        public Gameboard()
        {
            level = 3;
            player= new Player(this, InitPlayerMoneyLimit, initial_x, initial_y, InitPlayerMoneyLimit);
            
            Level.GenRandomLevel(this, level);
        }
        public void restart()
        {
            level = 3;
            player = new Player(this, InitPlayerMoneyLimit, initial_x, initial_y, InitPlayerMoneyLimit);
            coinsonfloor = default_cof; newredgen = default_nrg; rednoticedist = default_rnd; shootrange = default_sr;
            sight = default_st;
            default_sa.CopyTo(shop_amount, 0);
            enemies = new List<Enemy>();
            timer = 0;
            status = new GameObject[height, width];
            Level.GenRandomLevel(this, level);
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public void IncreaseTimer()
        {
            timer += 1;
            if (timer == newredgen)
            {
                timer = 0;
                SpawnEnemy();
            }
            if(timer % 2 == 0)
            foreach (Enemy enemy in enemies)
            {
                enemy.move();
            }
            Console.WriteLine("Money: " + player.hp + " Limit " + player.moneylimit + " CoinsOnFloor " + coinsonfloor + " NewRedGen " + newredgen + " RedNoticeDist " + rednoticedist + " sight " + sight + " damage " + player.attack);

        }

        public void SpawnEnemy()
        {
            //if (enemies.Count >= 3) return;
            bool success = false;
            while (!success)
            {
                int x = rnd.Next(1, height - 1);
                int y = rnd.Next(1, width - 1);

                bool duplicate = false;
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.x == x && enemy.y == y)
                    { 
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate) continue;
                if (status[x, y] is Space && player.x != x && player.y != y)
                {
                    enemies.Add(new Enemy(this, rnd.Next(0, level + 1), x, y));
                    success = true;
                    Console.WriteLine("Enemy Spawn at " + x + ", " + y);
                }
                
            }
            
        }

        public bool HasEnemy(int x, int y)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.x == x && enemy.y == y)
                {
                    return true;
                }
            }
            return false;
        }

        public Enemy GetEnemy(int x, int y)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.x == x && enemy.y == y)
                {
                    return enemy;
                }
            }
            return null;
        }
    }
}
