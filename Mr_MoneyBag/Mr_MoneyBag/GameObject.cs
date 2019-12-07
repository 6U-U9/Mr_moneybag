using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr_MoneyBag
{
    class GameObject
    {
        public Gameboard gameboard;
        public int x, y,hp;
        public Image[] image;
        public bool seen;
        public bool isblocked=true;
        public bool NearlySeen;
        public GameObject(Gameboard gameboard,int x,int y)
        {
            this.gameboard = gameboard;
            this.x = x;
            this.y = y;
        }
        virtual public void damaged(int n)
        {
            hp -= n;
            if (hp <= 0) dead();
        }
        virtual public void dead()
        { 
            freshboard(x, y, new Space(gameboard,x,y,this.seen,this.NearlySeen));
        }
        //更新对象地图
        virtual public void freshboard(int x, int y, GameObject gameobject)
        {
            gameboard.status[x, y] = gameobject;
        }

        virtual public Image getimage()
        { return null; }
        virtual public String GetImageName()
        { return null; }
        virtual public double distance(int x, int y)
        { return Math.Sqrt((this.x - x)*(this.x - x) + (this.y - y)*(this.y - y)); }
    }
    class Space : GameObject
    {
        public Space(Gameboard gameboard, int x, int y,bool seen=false, bool NearlySeen=false) : base(gameboard,x,y)
        { this.isblocked = false;
            this.seen = seen;
            this.NearlySeen = NearlySeen;
        }
        public override Image getimage()
        {
            return Properties.Resources.Space;
        }
        public override string GetImageName()
        {
            return "space";
        }
    }
    class MoveableObject : GameObject
    {
        public MoveableObject(Gameboard gameboard, int money, int x, int y) : base(gameboard,x,y)
        {
            this.hp = money;
        }
        public int attack;
        virtual public void moveto(int x, int y)
        {
            if (moveable(x,y) <= 0)
                return;
            var pastx = this.x;
            var pasty = this.y;
            this.x = x;
            this.y = y;
            freshboard(pastx, pasty, new Space(gameboard,pastx,pasty));
            freshboard(x, y, this);
        }
        virtual public void shoot(int dx, int dy)
        {
            int cnt = 1;
            var x = this.x + dx;
            var y = this.y + dy;
            while (gameboard.status[x, y] is Space && !gameboard.HasEnemy(x, y) && cnt <= gameboard.shootrange)
            {
                if (x == gameboard.GetHeight() - 1 || x == 0 || y == gameboard.GetWidth() - 1 || y == 0)
                    break;
                x = x + dx;
                y = y + dy;
                cnt += 1;
            }
            if (gameboard.HasEnemy(x, y))
                gameboard.GetEnemy(x, y).damaged(attack);
            else
                gameboard.status[x, y].damaged(attack);
            Console.WriteLine(x + "," + y + "damaged");
        }
        virtual public int moveable(int x, int y)
        {
            if (x > gameboard.GetHeight() - 1 || x < 0 || y > gameboard.GetWidth() - 1 || y < 0)
                return 0;
            if (gameboard.status[x, y].isblocked)
                return 0;
            if (gameboard.HasEnemy(x, y))
                return -1;
            return 1;
        }
    }
    class Player : MoveableObject
    {
        public int moneylimit;
        public Player(Gameboard gameboard,int money,int x,int y,int moneylimit,int attack=1): base(gameboard,money,x,y)
        {
            //this.image = Properties.Resources.player;
            this.moneylimit = moneylimit;
            this.attack = attack;
        }
        /*public override int moveable(int x, int y)
        {
            if (x > gameboard.GetHeight() - 1 || x < 0 || y > gameboard.GetWidth() - 1 || y < 0)
                return false;
            if (gameboard.status[x, y] is Enemy) this.damaged(((Enemy)gameboard.status[x, y]).attack);
            return base.moveable(x, y);
        }*/
        override public void moveto(int x, int y)
        {
            int canmove = moveable(x, y);
            if (canmove == 0) return;
            if (canmove == -1) { gameboard.IncreaseTimer(); return; }
            
            Console.WriteLine("Player: " + x + ", " + y);

            this.x = x;
            this.y = y;
            
            //判断是否到达Gate
            if (gameboard.status[this.x, this.y] is Gate)
            {
                gameboard.level += 1;
                Level.GenRandomLevel(gameboard, gameboard.level);
            }
            //判断是否获得Money
            if (gameboard.status[this.x, this.y] is Money&&this.hp<this.moneylimit)
            {
                getmoney(((Money)gameboard.status[this.x, this.y]).hp);
                freshboard(this.x,this.y, new Space(gameboard, this.x, this.y,true));
            }
            gameboard.IncreaseTimer();

            foreach (GameObject gameObject in gameboard.status)
            {
                if (gameObject.distance(this.x, this.y) < gameboard.sight)
                    gameObject.seen = true;
                else if (gameObject.distance(this.x, this.y) < gameboard.sight + 1)
                    gameObject.NearlySeen = true;
            }
        }
        public override void damaged(int n)
        {
            hp -= n;
            if (hp < 0) dead();
        }

        public override void dead()
        {
            Console.WriteLine("Player Dead!");
            base.dead();
            gameboard.restart();

        }
        public void getmoney(int money)
        { this.hp += money; }
        public void moveup()
        {moveto(this.x - 1, this.y); }
        public void movedown()
        { moveto(this.x+ 1, this.y ); }
        public void moveleft()
        { moveto(this.x, this.y-1); }
        public void moveright()
        { moveto(this.x, this.y+1); }
        public void shootup()
        { if (hp > 0) { this.hp--; shoot(-1, 0); } }
        public void shootdown()
        { if (hp > 0) { this.hp--; shoot(1, 0); } }
        public void shootleft()
        {if (hp > 0) { this.hp--; shoot(0, -1); } }
        public void shootright()
        {if (hp > 0) { this.hp--; shoot(0, 1); } }
        public override Image getimage()
        {
            return Properties.Resources.player; 
        }
        public override string GetImageName()
        {
            return "player";
        }
    }
    class Enemy : MoveableObject
    {
        private string status = "walk";
        static int[,] dir = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
        public Enemy(Gameboard gameboard,int money, int x, int y,int attack=1): base(gameboard,money,x,y)
        {
            this.hp = money;
            this.attack = attack;
            this.isblocked = true;
        }
        public void move()
        {
            if (this.distance(gameboard.player.x, gameboard.player.y) > gameboard.rednoticedist) return;
            TryAttack();

            Node go = DistanceUtility.GetNextStep(gameboard.player, this, gameboard);
            Console.WriteLine("Enemy" + x + "," + y + " to " + go.x + " -next- " + go.y);
            this.x = go.x;
            this.y = go.y;

        }

        public void TryAttack()
        {
            for(int i = 0; i < 4; i++)
            {
                int nx = x + dir[i, 0];
                int ny = y + dir[i, 1];
                if ((nx > 0 && nx < gameboard.GetWidth() - 1 && ny > 0 && ny < gameboard.GetHeight() - 1) 
                    && gameboard.player.x == nx && gameboard.player.y == ny)
                {
                    gameboard.player.damaged(attack);
                    Console.WriteLine("Attacked Player");
                }
            }
        }

        public override void dead()
        {
            base.dead();
            gameboard.enemies.Remove(this);
        }

        public override Image getimage()
        {
            switch(hp)
            {
                case 1:
                    return Properties.Resources.enemy_1;
                case 2:
                    return Properties.Resources.enemy_2;
                case 3:
                    return Properties.Resources.enemy_3;
            }
            return Properties.Resources.enemy_1;

        }
        public override string GetImageName()
        {
            switch (hp)
            {
                case 1:
                    return "enemy_1";
                case 2:
                    return "enemy_2";
                case 3:
                    return "enemy_3";
            }
            return "enemy_1";
        }
    }
    class Shop : GameObject
    {
        public int gain;
        public Shop(Gameboard gameboard,int money, int x, int y): base(gameboard,x,y)
        {
            this.hp = money;
            this.x = x;
            this.y = y;
            this.isblocked = true;
        }

        public override Image getimage()
        {
            return Properties.Resources.shop_heart;
        }
        public override void damaged(int n)
        {
            //Console.WriteLine("shop get money remain "+hp);
            base.damaged(1);
        }
    }
    class CoinOnFloor_Shop : Shop
    {
        public CoinOnFloor_Shop(Gameboard gameboard, int money, int x, int y,int gain=6) : base(gameboard,money,x,y)
        {
            this.gain = gain;
        }
        public override void dead()
        {
            gameboard.coinsonfloor += gain;
            base.dead();
        }
        public override Image getimage()
        {
            return Properties.Resources.shop_coinonfloor;
        }
        public override string GetImageName()
        {
            return "CoinOnFloor_Shop";
        }
    }
    class NewRedGen_Shop : Shop
    {
        public NewRedGen_Shop(Gameboard gameboard, int money, int x, int y,int gain=10) : base(gameboard, money, x, y)
        {
            this.gain = gain;
        }
        public override void dead()
        {
            gameboard.newredgen += gain;
            base.dead();
        }
        public override Image getimage()
        {
            return Properties.Resources.shop_newredgen;
        }
        public override string GetImageName()
        {
            return "NewRedGen_Shop";
        }
    }
    class RedNoticeDist_Shop : Shop
    {
        public RedNoticeDist_Shop(Gameboard gameboard, int money, int x, int y,int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
        }
        public override void dead()
        {
            gameboard.rednoticedist += gain;
            base.dead();
        }
        public override Image getimage()
        {
            return Properties.Resources.shop_rednoticedist;
        }
        public override string GetImageName()
        {
            return "RedNoticeDist_Shop";
        }
    }
    class Sight_Shop : Shop
    {
        public Sight_Shop(Gameboard gameboard, int money, int x, int y,int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
        }
        public override void dead()
        {
            gameboard.sight += gain;
            base.dead();
        }
        public override Image getimage()
        {
            return Properties.Resources.shop_sight;
        }
        public override string GetImageName()
        {
            return "Sight_Shop";
        }
    }
    class Damage_Shop : Shop
    {
        public Damage_Shop(Gameboard gameboard, int money, int x, int y, int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
        }
        public override void dead()
        {
            gameboard.player.attack+= gain;
            base.dead();
        }
        public override Image getimage()
        {
            return Properties.Resources.shop_damage;
        }
        public override string GetImageName()
        {
            return "Damage_Shop";
        }
    }
    class MoneyLimit_Shop : Shop
    {
        public MoneyLimit_Shop(Gameboard gameboard, int money, int x, int y, int gain=2) : base(gameboard, money, x, y)
        {
            this.gain = gain;
        }
        public override void dead()
        {
            gameboard.player.moneylimit += gain;
            base.dead();
        }
        public override Image getimage()
        {
            return Properties.Resources.shop_heart;
        }
        public override string GetImageName()
        {
            return "MoneyLimit_Shop";
        }
    }
    class Wall : GameObject
    {
        public Wall(Gameboard gameboard,int x, int y,int hp=1 ): base(gameboard,x,y)
        {
            this.hp = hp;
            this.isblocked = true;
        }
        public override void damaged(int n)
        {
            base.damaged(1);
        }
        public override Image getimage()
        {
            return Properties.Resources.Wall;
        }
        public override string GetImageName()
        {
            return "wall";
        }
    }
    class UnbreakableWall : Wall
    {
        public UnbreakableWall(Gameboard gameboard, int x, int y, int hp = 1) : base(gameboard,x,y,hp)
        { }
        public override void damaged(int n)
        { }
    }
    class Gate : Space
    {
        public Gate(Gameboard gameboard,int x,int y) : base(gameboard,x,y)
        { }
    }
    class Money : Space
    { 
        public Money(Gameboard gameboard,int x,int y,int money=1): base(gameboard,x,y)
        {
            this.hp = money;
        }

        public override Image getimage()
        {
            return Properties.Resources.money;
        }
        public override string GetImageName()
        {
            return "money";
        }
    }
}
