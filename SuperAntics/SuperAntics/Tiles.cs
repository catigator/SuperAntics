using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperAntics
{
    // General Tile class
    public class Tile
    {
        public Constants.Tiletype name { get; set; }
        public Texture2D TileTexture { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Tile() { }

        public Tile(int x, int y)
            : base()
        {
            this.X = x;
            this.Y = y;
            this.name = Constants.Tiletype.Tile;
        }
        public Tile(int x, int y, Texture2D mytexture)
            : base()
        {
            this.X = x;
            this.Y = y;
            this.TileTexture = mytexture;
        }
    }

    // Hollow tile
    public class Hollow : Tile
    {
        public Hollow(int x, int y, Texture2D mytexture)
            : base(x, y, mytexture)
        {
            this.name = Constants.Tiletype.Hollow;
            this.TileTexture = mytexture;
        }
    }

    public class PointJ
    {
        public int X { get; set; }
        public int Y { get; set; }

        public PointJ() { }

        public PointJ(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Creature
    public class Creature : Tile
    {
        private int hits;

        public int nextX { get; set; }
        public int nextY { get; set; }
        public int justHit { get; set; }
        public int stunned { get; set; }

        public int Hits
        {
            get
            {
                return hits;
            }
            set
            {
                hits = value;
                if (hits <= 0)
                {
                    // Do something in the future
                }
            }
        }

        public bool Alive
        {
            get { return (Hits >= 0); }
        }
    }

    // Player
    public class Player : Creature
    {
        public Player(PointJ p)
        {
            X = p.X;
            Y = p.Y;
            Hits = Constants.StartingHitPoints;
            stunned = 0;
            name = Constants.Tiletype.Player;
        }
    }

    // Monster
    public class Monster : Creature
    {
        public Constants.Direction lastDirection { get; set; }
        public Constants.Direction nextMove2 { get; set; }
        public Constants.AImode mode { get; set; }
        public Constants.XYmode xymode { get; set; }
        public int consecutiveMovesX { get; set; }
        public int consecutiveMovesY { get; set; }


        public Monster(PointJ p)
        {
            X = p.X;
            Y = p.Y;
            name = Constants.Tiletype.Monster;
            Hits = Constants.StartingHitPoints;
            mode = Constants.AImode.Angry;
            consecutiveMovesX = 0;
            consecutiveMovesY = 0;
            stunned = Constants.StartingStun;
            xymode = Constants.XYmode.None;
        }

        public Monster(PointJ p, Constants.AImode m)
        {

            X = p.X;
            Y = p.Y;
            name = Constants.Tiletype.Monster;
            Hits = Constants.StartingHitPoints;
            mode = m;
            consecutiveMovesX = 0;
            consecutiveMovesY = 0;
            stunned = Constants.StartingStun;
        }

    }
}
