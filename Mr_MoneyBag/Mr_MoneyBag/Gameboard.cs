using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Mr_MoneyBag
{
    class GameBoard
    {
        public const int width=39, height=39;
        static int default_cof = 24, default_nrg = 20, default_rnd = 20;
        static double default_st = 5.9;
        static int[,] default_sa = { { 1, 3 }, { 1, 3 }, { 1, 3 }, { 1, 3 }, { 1, 3 }, { 1, 3 } }; // coinonfloor, newredgen, rednoticedist, sight, damage, moneylimit, 

        static Random rnd = new Random();
        public int level=1; //当前关卡
        public double shopnoticedist = 1.1;
        public int coinsonfloor = 24, newredgen = 20, rednoticedist = 20,  initplayermoneylimit = 30;
        public double sight = 5.9; //视野
        public int[,] shop_amount = new int[,] { { 1, 3 }, { 1, 3 }, { 1, 3 }, { 1, 3 }, { 1, 3 }, { 1, 3 } }; // coinonfloor, newredgen, rednoticedist, sight, damage, moneylimit, 

        public GameObject[,] status = new GameObject[height, width];

        public const int initial_x = 20;
        public const int initial_y = 20;
        public Player player;
        
        public List<Enemy> enemies = new List<Enemy>();
        public List<Bullet> bullets = new List<Bullet>();
        public List<ValueTuple<string, Type>> noticelist = new List<ValueTuple<string, Type>>();
        public int turn = 0;
        public bool is_newlevel = false;
        public bool is_playerdead = false;

        public GameBoard()
        {
            level = 1;
            player= new Player(this, initplayermoneylimit, initial_x, initial_y, initplayermoneylimit);
            GenLevel(level);
            is_playerdead = false;
        }
        public void Restart()
        {
            level = 1;
            turn = 0;
            player = new Player(this, initplayermoneylimit, initial_x, initial_y, initplayermoneylimit);
            coinsonfloor = default_cof; newredgen = default_nrg; rednoticedist = default_rnd;
            sight = default_st;
            shop_amount = default_sa.Clone() as int[,];
            status = new GameObject[height, width];
            enemies = new List<Enemy>();
            bullets = new List<Bullet>();
            noticelist = new List<ValueTuple<string, Type>>();
            GenLevel(level);
        }

        public void IfNextLevel()
        {
            //判断是否到达Gate
            if (!(status[player.x, player.y] is Gate))
                return;
            if (player.is_moving)
                return;
            level = level + 1;
            turn = 0;
            status = new GameObject[height, width];
            enemies = new List<Enemy>();
            bullets = new List<Bullet>();
            player.InitPosition(initial_x,initial_y);
            GenLevel(level);
            is_newlevel = true;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public void FreshBullets()
        {
            if (bullets.Count > 0)
                for (int i=0; i< bullets.Count;i++ )
                    bullets[i].Move();
        }
        public void NextTurn()
        {
            turn += 1;
            if (turn % newredgen==0)
            {
                SpawnEnemy();
            }
            
            foreach (Enemy enemy in enemies)
            {if (turn % 2 == 0)
                    enemy.Move();
                else
                    enemy.ReadytoAction();
            }

            Console.WriteLine("Money: " + player.hp + " Limit " + player.moneylimit + " CoinsOnFloor " + coinsonfloor + " NewRedGen " + newredgen + " RedNoticeDist " + rednoticedist + " sight " + sight + " damage " + player.attack);

            // check shops and notice
            DisplayShopNotice(player.x, player.y);
        }

        public void DisplayShopNotice(int x, int y) // check the status around x and y and notice the nearest shop
        {
            int xst = x - (int)shopnoticedist;
            if (xst < 0) xst = 0;
            int xed = x + (int)shopnoticedist;
            if (xed > height - 1) xed = height - 1;
            int yst = y - (int)shopnoticedist;
            if (yst < 0) yst = 0;
            int yed = y + (int)shopnoticedist;
            if (yed > width - 1) yed = width - 1;

            double mindist = shopnoticedist;
            int minx = 0, miny = 0;

            for (int i = xst; i <= xed; i++)
            {
                for (int j = yst; j <= yed; j++)
                {
                    if (!(status[i, j] is Shop)) continue;
                    double temp = status[i, j].Distance(x, y);
                    if (temp < mindist)
                    {
                        Console.WriteLine("d: " + temp + " from " + i + "," + j + " to " + x + "," + y);
                        mindist = temp;
                        minx = i; miny = j;
                    }
                }
            }
            //Console.WriteLine("aaa");
            if (mindist >= shopnoticedist) return;
            //Console.WriteLine("bbb");

            try
            {
                ((Shop)(status[minx, miny])).notice();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Cannot convert to shop!", e);
            }

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



                if (status[x, y] is Space && player.Distance(x, y) > sight)
                {
                    enemies.Add(new Enemy(this, rnd.Next(0, level + 1), x, y));
                    success = true;
                    Console.WriteLine("Enemy Spawn at " + x + ", " + y);
                }
                
            }
            DisplayEnemy();
            AddNotice("New Enemy Spawn!", typeof(Enemy));
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

        public void DisplayEnemy()
        {
            Console.Write("Current Enemies: ");
            foreach (Enemy enemy in enemies)
            {
                Console.Write("(" + enemy.x + ", " + enemy.y + ") ");
                
            }
            Console.WriteLine();
        }

        public void GenLevel(int lv)
        {
            GenBasicMap();
            AddStair();
            AddShop(lv);
            AddMoney();
        }

        public void AddStair()
        {
            bool success = false;
            while (!success)
            {
                int x = rnd.Next(0, height);
                int y = rnd.Next(0, width);

                if (status[x, y] is Space && player.x != x && player.y != y)
                {
                    status[x, y] = new Gate(this, x, y);
                    success = true;
                }

            }
        }

        public void AddShop(int lv)
        {
            for (int i = 0; i < shop_amount.Length/2; i++)
            {
                int amount = rnd.Next(shop_amount[i, 0], shop_amount[i, 1]);
                for (int j = 0; j < amount; j++)
                {
                    bool success = false;
                    while (!success)
                    {
                        int x = rnd.Next(1, height - 1);
                        int y = rnd.Next(1, width - 1);
                        int max_health = Math.Min(level * 2, 6);
                        int min_health = Math.Min(level, 3);

                        if (status[x, y].GetType() == typeof(Space) && player.x != x && player.y != y &&
                            !(status[x - 1, y].isblocked && status[x + 1, y].isblocked) &&
                            !(status[x, y - 1].isblocked && status[x, y + 1].isblocked)
                            )
                        {
                            switch (i)
                            {
                                case 0:
                                    status[x, y] = new CoinOnFloor_Shop(this, rnd.Next(min_health, max_health), x, y);
                                    break;
                                case 1:
                                    status[x, y] = new NewRedGen_Shop(this, rnd.Next(min_health, max_health), x, y);
                                    break;
                                case 2:
                                    status[x, y] = new RedNoticeDist_Shop(this, rnd.Next(min_health, max_health), x, y);
                                    break;
                                case 3:
                                    status[x, y] = new Sight_Shop(this, rnd.Next(min_health, max_health), x, y);
                                    break;
                                case 4:
                                    status[x, y] = new Damage_Shop(this, rnd.Next(min_health, max_health), x, y);
                                    break;
                                case 5:
                                    status[x, y] = new MoneyLimit_Shop(this, rnd.Next(min_health, max_health), x, y);
                                    break;

                            }
                            success = true;
                        }

                    }
                }

            }
        }

        public void AddMoney()
        {
            for (int i = 0; i < coinsonfloor; i++)
            {
                bool success = false;
                while (!success)
                {
                    int x = rnd.Next(0, height);
                    int y = rnd.Next(0, width);

                    if (status[x, y].GetType() == typeof(Space) && player.x != x && player.y != y)
                    {
                        status[x, y] = new Money(this, x, y);
                        success = true;
                    }

                }
            }
        }


        public void GenBasicMap()
        {

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 || j == 0 || i == height - 1 || j == width - 1)
                        status[i, j] = new UnbreakableWall(this, i, j);
                    else
                        status[i, j] = new Wall(this, i, j);
                }
            }

            bool[,] vis = new bool[height, width];
            GenBasicMapHelper(vis, initial_x, initial_y, 0);
            //status[initial_x, initial_y] = board.player;
        }


        private void GenBasicMapHelper(bool[,] vis, int x, int y, int step)
        {

            if (x > width - 2 || x < 1 || y > height - 2 || y < 1) return;
            
            if (vis[y, x]) return;
            if (status[y, x].GetType() == typeof(Space)) return;

            vis[y, x] = true;
            //Console.WriteLine(x + " -xy- " + y);
            status[y, x] = new Space(this, y, x);

            //int[] rndorder = order.OrderBy(t => rnd.Next()).ToArray();
            //int next = rnd.Next(0, 4);
            //GenBasicMapHelper(board, vis, x + dir[next, 0], y + dir[next, 1], step + 1);
            int[,] dir = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
            int[,] parameter = new int[,] { { 4, 0 }, { 5, 0 }, { 255, 1 }, { 505, 1 } };

            int prev = -1;
            for (int i = 0; i < 4; i++)
            {

                if (rnd.Next(0, (step / parameter[i, 0]) + parameter[i, 1]) == 0)
                {
                    int next = rnd.Next(0, 4);
                    if (i == 0) prev = next;
                    else if (next == prev)
                    {
                        while (next == prev)
                            next = rnd.Next(0, 4);
                    }
                    GenBasicMapHelper(vis, x + dir[next, 0], y + dir[next, 1], step + 1);
                }
            }
        }

        public void AddNotice(string s, Type type)
        {
            noticelist.Add((s, type));
            //noticelist.ForEach(Console.WriteLine);
        }

    }
}
