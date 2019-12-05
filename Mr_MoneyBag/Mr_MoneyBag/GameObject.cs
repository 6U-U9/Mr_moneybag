using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr_MoneyBag
{
    abstract class GameObject
    {
        public Gameboard gameboard;
        public int x, y,hp;
        public Image[] image;
        public bool visible;
        public bool seen;
        public bool isblocked;
        virtual public void damaged(int n)
        {
            hp -= n;
            if (hp <= 0) dead();
        }
        virtual public void dead()
        { 
            freshboard(x, y, new Space()); 
        }
        //更新对象地图
        virtual public void freshboard(int x, int y,GameObject gameobject)
        {
            gameboard.status[x, y] = gameobject;
        }
        virtual public Image getimage()
        { return null; }
        virtual public int distance(int x, int y)
        { return Math.Abs(this.x - x) + Math.Abs(this.y - y); }
    }    
    class Space : GameObject
    {
        public override Image getimage()
        {
            return Properties.Resources.space;
        }
    }
    class MoveableObject : GameObject
    {
        public int attack;
        virtual public void moveto(int x, int y)
        {
            if (!moveable(x,y))
                return;
            var pastx = this.x;
            var pasty = this.y;
            this.x = x;
            this.y = y;
            freshboard(pastx, pasty, gameboard.status[x, y]);
        }
        virtual public void shoot(int dx, int dy)
        {
            var x = this.x + dx;
            var y = this.y + dy;
            while (gameboard.status[x, y] is Space)
            {
                x = this.x + dx;
                y = this.y + dy;
            }
            gameboard.status[x, y].damaged(attack);
        }
        virtual public bool moveable(int x, int y)
        {
            if (gameboard.status[x, y] is Space)
                return true;
            return false;
        }
    }
    class Player : MoveableObject
    {
        public Player(int money,int x,int y)
        {
            //this.image = Properties.Resources.player;
            this.hp = money;
            this.x = x;
            this.y = y;
        }
        public override bool moveable(int x, int y)
        {
            if (gameboard.status[x, y] is Enemy) this.damaged(((Enemy)gameboard.status[x, y]).attack);
            return base.moveable(x, y);
        }
        override public void moveto(int x, int y)
        {
            base.moveto(x, y);
            //判断是否到达Gate
            if (gameboard.status[this.x, this.y] is Gate)
            {
                gameboard.level += 1;
                gameboard.genlevel(gameboard.level);
            }
            //判断是否获得Money
            if (gameboard.status[this.x, this.y] is Money)
            {
                getmoney(((Money)gameboard.status[this.x, this.y]).hp);
            }
        }
        public void getmoney(int money)
        { this.hp += money; }
        public void moveup()
        {
            moveto(this.x, this.y - 1);
        }
        public void movedown()
        { moveto(this.x, this.y + 1); }
        public void moveleft()
        { moveto(this.x-1, this.y); }
        public void moveright()
        { moveto(this.x+1, this.y); }
        public void shootup()
        { shoot(0, -1); }
        public void shootdown()
        { shoot(0, 1); }
        public void shootleft()
        { shoot(-1, 0); }
        public void shootright()
        { shoot(1, 0); }
        public override Image getimage()
        {
            return Properties.Resources.player; 
        }
    }
    class Enemy : MoveableObject
    {
        private string status = "walk";
        public Enemy(int money, int x, int y,int attack=1)
        {
            this.hp = money;
            this.x = x;
            this.y = y;
            this.attack = attack;
        }
        public void move()
        { }
    }
    class Shop : GameObject
    {
        public Shop(int money, int x, int y)
        {
            this.hp = money;
            this.x = x;
            this.y = y;
        }
    }
    class Wall : GameObject
    {
        public Wall(int x, int y,int hp=1 )
        {
            this.hp = hp;
            this.x = x;
            this.y = y;
        }
        public override void damaged(int n)
        {
            base.damaged(1);
        }
    }
    class Gate : Space
    { }
    class Money : Space
    { 
        public Money(int money=1)
        {
            this.hp = money;
        }
    }
}
