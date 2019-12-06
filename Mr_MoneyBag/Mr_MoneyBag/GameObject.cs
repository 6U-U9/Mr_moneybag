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
        public bool visible;
        public bool seen;
        public bool isblocked=true;
        public bool hasimagechange;
        public GameObject(Gameboard gameboard)
        {
            this.gameboard = gameboard;
        }
        virtual public void damaged(int n)
        {
            hp -= n;
            if (hp <= 0) dead();
        }
        virtual public void dead()
        { 
            freshboard(x, y, new Space(gameboard)); 
        }
        //更新对象地图
        virtual public void freshboard(int x, int y, GameObject gameobject)
        {
            gameboard.status[x, y] = gameobject;
            gameboard.status[x, y].hasimagechange = true;
        }

        virtual public Image getimage()
        { return null; }
        virtual public String GetImageName()
        { return null; }
        virtual public int distance(int x, int y)
        { return Math.Abs(this.x - x) + Math.Abs(this.y - y); }
    }
    class Space : GameObject
    {
        public Space(Gameboard gameboard) : base(gameboard)
        { this.isblocked = false; }
        public override Image getimage()
        {
            return Properties.Resources.space;
        }
        public override string GetImageName()
        {
            return "space";
        }
    }
    class MoveableObject : GameObject
    {
        public MoveableObject(Gameboard gameboard, int money, int x, int y) : base(gameboard)
        {
            this.hp = money;
            this.x = x;
            this.y = y;
        }
        public int attack;
        virtual public void moveto(int x, int y)
        {
            if (!moveable(x,y))
                return;
            var pastx = this.x;
            var pasty = this.y;
            this.x = x;
            this.y = y;
            freshboard(pastx, pasty, new Space(gameboard));
            freshboard(x, y, this);
        }
        virtual public void shoot(int dx, int dy)
        {
            var x = this.x + dx;
            var y = this.y + dy;
            while (gameboard.status[x, y] is Space)
            {
                if (x == gameboard.GetHeight() - 1 || x == 0 || y == gameboard.GetWidth() - 1 || y == 0)
                    break;
                x = x + dx;
                y = y + dy;
            }
            gameboard.status[x, y].damaged(attack);
            Console.WriteLine(x + "," + y + "damaged");
        }
        virtual public bool moveable(int x, int y)
        {
            if (x > gameboard.GetHeight() - 1 || x < 0 || y > gameboard.GetWidth() - 1 || y < 0)
                return false;
            if (gameboard.status[x, y].isblocked==false)
                return true;
            return false;
        }
    }
    class Player : MoveableObject
    {
        public int moneylimit;
        public Player(Gameboard gameboard,int money,int x,int y,int moneylimit): base(gameboard,money,x,y)
        {
            //this.image = Properties.Resources.player;
            this.moneylimit = moneylimit;
        }
        public override bool moveable(int x, int y)
        {
            if (x > gameboard.GetHeight() - 1 || x < 0 || y > gameboard.GetWidth() - 1 || y < 0)
                return false;
            if (gameboard.status[x, y] is Enemy) this.damaged(((Enemy)gameboard.status[x, y]).attack);
            return base.moveable(x, y);
        }
        override public void moveto(int x, int y)
        {
            if (!moveable(x, y))
                return;
            this.x = x;
            this.y = y;
            //判断是否到达Gate
            if (gameboard.status[this.x, this.y] is Gate)
            {
                gameboard.level += 1;
                gameboard.genlevel(gameboard.level);
            }
            //判断是否获得Money
            if (gameboard.status[this.x, this.y] is Money&&this.hp<this.moneylimit)
            {
                getmoney(((Money)gameboard.status[this.x, this.y]).hp);
                freshboard(this.x,this.y, new Space(gameboard));
            }
            //Console.WriteLine(this.hp);
        }
        public override void damaged(int n)
        {
            hp -= n;
            if (hp < 0) dead();
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
        public Enemy(Gameboard gameboard,int money, int x, int y,int attack=1): base(gameboard,money,x,y)
        {
            this.attack = attack;
        }
        public void move()
        { }
    }
    class Shop : GameObject
    {
        public int gain;
        public Shop(Gameboard gameboard,int money, int x, int y): base(gameboard)
        {
            this.hp = money;
            this.x = x;
            this.y = y;
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
            return base.getimage();
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
            return base.getimage();
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
            return base.getimage();
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
            return base.getimage();
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
            return base.getimage();
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
            return base.getimage();
        }
        public override string GetImageName()
        {
            return "MoneyLimit_Shop";
        }
    }
    class Wall : GameObject
    {
        public Wall(Gameboard gameboard,int x, int y,int hp=1 ): base(gameboard)
        {
            this.hp = hp;
            this.x = x;
            this.y = y;
        }
        public override void damaged(int n)
        {
            base.damaged(1);
        }
        public override Image getimage()
        {
            return Properties.Resources.wall;
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
        public Gate(Gameboard gameboard) : base(gameboard)
        { }
    }
    class Money : Space
    { 
        public Money(Gameboard gameboard,int money=1): base(gameboard)
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
