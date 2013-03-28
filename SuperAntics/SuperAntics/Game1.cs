using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SuperAntics
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Textures        
        private Texture2D mAnt;
        private Texture2D mHollow;
        private Texture2D mLogo;
        private Texture2D mMessageBox;
        private Texture2D mBlack;
        private Texture2D mEnemy;

        //Game objects
        private SpriteFont mText;
        private Player mplayer;
        private Tile[,] mTiles;
        private IList<Monster> monsters;

        // Time counters
        private float mTime = 0.3F;
        private float eTime = 0.3F;
        private float iTime = 0.3F;

        // Enemy and player Speeds
        private float mSpeed = 0;
        private float eSlow = 0.21F; // enemy slowness
        private float mSlow = 0.15F; // player slowness
        private float rSlow = 0.2F; // reference slowness

        // Used as random numbers
        private int mInt = 1;
        private int mInt2 = 2;

        // Enemy variables 
        private int xAttackers = 0;
        private int yAttackers = 0;
        private int nbrOfMonsters = Constants.NumberOfMonsters;


        //Keyboard Input
        private bool mPausePressed = false;

        //Color to tint the background when the game is paused
        private Color mPauseColor = Color.MediumSlateBlue;

        //Game data tracking for the current score
        private int mScore = 0;

        //Game data tracking the total time survived
        private float mElapsed = 0;

        // Random number generator
        private Random mRandom = new Random();

        //Define the various states the game can be in
        private enum GameState
        {
            Intro,
            Playing,
            Paused,
            GameOver
        }//end enum GameState

        //Stores the current state of the game
        private GameState mCurrentState = GameState.Intro;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Initialize the graphics, the game and the text objects
            InitializeGraphics();
            InitializeGame();
            InitializeText();
        }

        //Initialize the SpriteBatch and textures
        protected void InitializeGraphics()
        {
            mBlack = Content.Load<Texture2D>("Images/Ground");
            mAnt = Content.Load<Texture2D>("Images/Ant1");
            mHollow = Content.Load<Texture2D>("Images/Hollow");
            mMessageBox = Content.Load<Texture2D>("Images/MessageBox");
            mLogo = Content.Load<Texture2D>("Images/logo");
            mEnemy = Content.Load<Texture2D>("Images/Ant3");
        }//end protected void InitializeGraphics()

        //Initialize the fonts to be used
        protected void InitializeText()
        {
            mText = Content.Load<SpriteFont>("myFont");
        }//end protected void InitializeText()

        //Initialize the Game
        protected void InitializeGame()
        {
            //Initialize the game data

            mplayer = new Player(new PointJ(Constants.DungeonWidth / 2, Constants.DungeonHeight / 2));

            mTiles = new Tile[Constants.DungeonWidth, Constants.DungeonHeight];
            mTiles[mplayer.X, mplayer.Y] = new Hollow(mplayer.X, mplayer.Y, mHollow);
            CreateGroundTiles();
            mElapsed = 0;
            mScore = 0;

            // Monsters
            monsters = new List<Monster>();
            for (int i = 0; i < Constants.NumberOfMonsters; i++)
            {
                Monster monsterr = new Monster(GetValidRandomPoint());
                mTiles[monsterr.X, monsterr.Y] = new Hollow(mplayer.X, mplayer.Y, mHollow);
                monsters.Add(monsterr);
            }

        }//end protected void InitializeGame()


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            //elapsed = time since Update was called last
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;


            //Get the current state of the keyboard
            KeyboardState aKeyboard = Keyboard.GetState();

            //If the Escape key has been pressed, then exit the game
            if (aKeyboard.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }//end if (aKeyboard.IsKeyDown(Keys.Escape) == true)

            //Update the game objects
            UpdateGameObjects(elapsed, aKeyboard);

            base.Update(gameTime);
        }

        private void CreateGroundTiles()
        {
            for (int i = 0; i < Constants.DungeonHeight; i++)
            {
                for (int j = 0; j < Constants.DungeonWidth; j++)
                {
                    mTiles[j, i] = new Tile(i, j, mBlack);
                }
            }
            mTiles[mplayer.X, mplayer.Y] = new Hollow(mplayer.X, mplayer.Y, mHollow);
        }


        //Update the game objects
        protected void UpdateGameObjects(float elapsed, KeyboardState theKeyboard)
        {
            if (mCurrentState == GameState.Playing)
            {

                mTime += elapsed;
                eTime += elapsed;

                // Move enemies if enough time has passed since last move
                if (eTime > eSlow)
                {

                    eTime = 0F;
                    EnemyMoves();
                    FinalMoveE();
                    mScore += 1;
                    CheckCollisions();
                }

                if (mCurrentState == GameState.Playing)
                {



                    mElapsed += elapsed;
                    //Move player to the left
                    if (theKeyboard.IsKeyDown(Keys.Left) == true)
                    {
                        // Check if enough time has passed
                        if (mTime > mSlow)
                        {
                            mTime = 0F;
                            Move(mplayer, -1, 0);
                        }
                    }

                    //Move player to the right
                    if (theKeyboard.IsKeyDown(Keys.Right) == true)
                    {
                        // Check if enough time has passed
                        if (mTime > mSlow)
                        {
                            mTime = 0F;
                            Move(mplayer, 1, 0);
                        }
                    }

                    // Move player up
                    if (theKeyboard.IsKeyDown(Keys.Up) == true)
                    {
                        // Check if enough time has passed
                        if (mTime > mSlow)
                        {
                            mTime = 0F;

                            Move(mplayer, 0, -1);
                        }
                    }


                    //Move player down
                    if (theKeyboard.IsKeyDown(Keys.Down) == true)
                    {
                        // Check if enough time has passed
                        if (mTime > mSlow)
                        {


                            mTime = 0F;
                            Move(mplayer, 0, 1);
                        }
                    }

                    // Check collisions after movement
                    if (mTime == 0F)
                    {
                        CheckCollisions();
                    }
                }
            }

            // Check if player wants to pause the game
            if (theKeyboard.IsKeyDown(Keys.P) == true)
            {
                if (mCurrentState == GameState.Paused | mCurrentState == GameState.Playing)
                {
                    if (mPausePressed == false)
                    {
                        mPausePressed = true;
                        if (mCurrentState == GameState.Paused)
                        {
                            mCurrentState = GameState.Playing;
                        }
                        else
                        {
                            mCurrentState = GameState.Paused;
                        }
                    }
                }
            }
            else
            {
                mPausePressed = false;
            }

            // Start game if Space is pressed
            if (theKeyboard.IsKeyDown(Keys.Space) == true)
            {
                if (mCurrentState == GameState.Intro)
                {

                    iTime += elapsed;
                    if (iTime > rSlow)
                    {
                        // Create level
                        mTiles = new Tile[Constants.DungeonWidth, Constants.DungeonHeight];
                        CreateGroundTiles();

                        // Create monsters
                        monsters = new List<Monster>();
                        for (int i = 0; i < nbrOfMonsters; i++)
                        {
                            Monster monsterr = new Monster(GetValidRandomPoint());
                            mTiles[monsterr.X, monsterr.Y] = new Hollow(mplayer.X, mplayer.Y, mHollow);
                            monsters.Add(monsterr);
                        }
                        // Set Game Speed
                        eSlow = 0.042F * (5 - mSpeed);
                        mSlow = 0.03F * (5 - mSpeed);
                        iTime = 0F;
                        mCurrentState = GameState.Playing;
                    }
                }
                else if (mCurrentState == GameState.GameOver)
                {
                    mCurrentState = GameState.Intro;
                    InitializeGame();
                    mCurrentState = GameState.Intro;
                }
            }

            if (mCurrentState == GameState.Intro)
            {
                iTime += elapsed;

                // Decrease the number of monsters

                if (theKeyboard.IsKeyDown(Keys.Left) == true)
                {
                    if (iTime > rSlow && nbrOfMonsters > 0)
                    {
                        iTime = 0F;
                        nbrOfMonsters--;
                    }
                }

                // Increase the number of monsters

                if (theKeyboard.IsKeyDown(Keys.Right) == true)
                {
                    if (iTime > rSlow && nbrOfMonsters < 30)
                    {
                        iTime = 0F;
                        nbrOfMonsters++;                    
                    }
                }

                // Decrease the Speed

                if (theKeyboard.IsKeyDown(Keys.Down) == true)
                {
                    if (iTime > rSlow)
                    {
                        iTime = 0F;
                        if (mSpeed >= -3)
                        {
                            mSpeed -= 1;
                        }
                    }
                }

                // Increase the Speed

                if (theKeyboard.IsKeyDown(Keys.Up) == true)
                {
                    if (iTime > rSlow)
                    {
                        iTime = 0F;
                        if (mSpeed <= 3)
                        {
                            mSpeed += 1;
                        }
                    }
                }
            }



        }//end protected void UpdateGameObjects(float elapsed, KeyboardState theKeyboard)

        // Move enemies according to their AI mode
        protected void EnemyMoves()
        {
            ChangeAttacks();
            monsters.ToList().ForEach(m =>
            {
                switch (m.mode)
                {
                    case Constants.AImode.Angry:
                        MoveToPoint(m, mplayer.X, mplayer.Y);
                        break;
                    case Constants.AImode.Ignorant:
                        // Stay in place
                        break;
                }

            });
        }

        // Try to move creature xD steps in x direction and yD steps in y direction. Dig if encountering ground, move if encountering a hollow tile
        protected void Move(Creature creature, int xD, int yD)
        {
            if (creature.stunned == 0)
            {
                if ((creature.X + xD >= 0 && creature.Y + yD >= 0) && (creature.X + xD < Constants.DungeonWidth && creature.Y + yD < Constants.DungeonHeight))
                {
                    if (mTiles[creature.X + xD, creature.Y + yD].name == Constants.Tiletype.Hollow)
                    {
                        creature.X += xD;
                        creature.Y += yD;
                    }
                    else
                    {
                        mTiles[creature.X + xD, creature.Y + yD] = new Hollow(creature.X + xD, creature.Y + yD, mHollow);
                    }
                }
            }
            else
            {
                creature.stunned -= 1;
            }
        }//end protected void Move

        // Set enemies NEXT move
        protected void MoveE(Creature creature, int xD, int yD)
        {
            creature.nextX = xD;
            creature.nextY = yD;

            if (!(creature.X + xD >= 0 && creature.X + xD < Constants.DungeonWidth))
                creature.nextX = 0;

            if (!(creature.Y + yD >= 0 && creature.Y + yD < Constants.DungeonHeight))
                creature.nextY = 0;

            if (mTiles[creature.X + creature.nextX, creature.Y + creature.nextY].name == Constants.Tiletype.Hollow)
            {
                creature.justHit = 0;
            }
            else
            {
                creature.justHit = 1;
            }
        }//end protected void MoveE

        // Move all enemies if they are not stunned
        protected void FinalMoveE()
        {
            monsters.ToList().ForEach(creature =>
            {
                if (creature.stunned == 0)
                {
                    if (creature.justHit == 1)
                    {
                        if (mTiles[creature.X + creature.nextX, creature.Y + creature.nextY].name != Constants.Tiletype.Hollow)
                        {
                            mTiles[creature.X + creature.nextX, creature.Y + creature.nextY] = new Hollow(creature.X + creature.nextX, creature.Y + creature.nextY, mHollow);
                        }
                    }
                    else if (mTiles[creature.X + creature.nextX, creature.Y + creature.nextY].name == Constants.Tiletype.Hollow)
                    {
                        creature.X += creature.nextX;
                        creature.Y += creature.nextY;
                    }
                }
                else
                {
                    creature.stunned -= 1;
                }
            });

        }//end protected void Move

        // Make monster follow player
        protected void MoveToPoint(Monster creature, int xD, int yD)
        {
            mInt = RandomNumber(1, 2);
            mInt2 = RandomNumber(0, 7); // 1 in 8 chance of moving randomly despite having a hollow way


            // Check that enemy is within the level bounds and not in the same position as player
            if (creature.X <= Constants.DungeonWidth && creature.Y <= Constants.DungeonHeight && !(creature.X == xD && creature.Y == yD))
            {
                if (creature.nextMove2 != Constants.Direction.Unknown)
                {
                    switch(creature.nextMove2)
                    {
                        case Constants.Direction.N:
                            MoveE(creature, 0,-1);
                            break;
                        case Constants.Direction.S:
                            MoveE(creature, 0, 1);
                            break;
                        case Constants.Direction.E:
                            MoveE(creature, 1, 0);
                            break;
                        case Constants.Direction.W:
                            MoveE(creature, -1, 0);
                            break;
                    }
                    creature.nextMove2 = Constants.Direction.Unknown;
                }
                else
                {

                
                    // Move towards player, taking a hollow way if possible
                    if (creature.X != xD && creature.Y != yD)
                    {
                        switch (HollowWay(creature, xD, yD))
                        {
                            case Constants.HollowXY.X:
                                MoveE(creature, DiffInt(creature.X, xD), 0);
                                if (mInt2 == 3)
                                {
                                    MoveE(creature, 0, DiffInt(creature.Y, yD));
                                    if (DiffInt(creature.Y, yD) == 1)
                                        creature.nextMove2 = Constants.Direction.S;
                                    else
                                        creature.nextMove2 = Constants.Direction.N;
                                }
                                break;
                            case Constants.HollowXY.Y:
                                MoveE(creature, 0, DiffInt(creature.Y, yD));
                                if (mInt2 == 3) // move in different direction sometimes
                                {
                                    MoveE(creature, DiffInt(creature.X, xD), 0);
                                    if (DiffInt(creature.Y, yD) == 1)
                                        creature.nextMove2 = Constants.Direction.E;
                                    else
                                        creature.nextMove2 = Constants.Direction.W;
                                }
                                break;
                            // If both the x and y directions are 
                            case Constants.HollowXY.XY:
                                // Follow player in random direction if in XYmode None
                                if (creature.xymode == Constants.XYmode.None)
                                {
                                    switch (mInt)
                                    {
                                        case (0):
                                            MoveE(creature, 0, DiffInt(creature.Y, yD));
                                            break;
                                        case (1):
                                            MoveE(creature, DiffInt(creature.X, xD), 0);
                                            break;
                                    }                                
                                }
                                // Follow player in x direction if in XYmode X
                                else if (creature.xymode == Constants.XYmode.X || creature.Y == yD)
                                {
                                    MoveE(creature, DiffInt(creature.X, xD), 0);
                                }
                                // Follow player in y direction if in XYmode Y
                                else if (creature.xymode == Constants.XYmode.Y || creature.X == xD)
                                {
                                    MoveE(creature, 0, DiffInt(creature.Y, yD));                                
                                }
                                break;
                            case Constants.HollowXY.None:
                                //Follow player in random direction if there is no hollow way
                                if (mInt == 1)
                                {
                                    MoveE(creature, DiffInt(creature.X, xD), 0);
                                }
                                else
                                {
                                    MoveE(creature, 0, DiffInt(creature.Y, yD));
                                }
                                break;
                        }
                    }
                    else if (creature.Y != yD) // See if Y position equals the player's
                    {
                        MoveE(creature, 0, DiffInt(creature.Y, yD)); // kommer inte hit blaargh!
                    }
                    else if (creature.X != xD) // See if X position equals the player's
                    {
                        MoveE(creature, DiffInt(creature.X, xD), 0);
                    }

                    else
                    {
                        switch (mInt) // decide randomly which direction to prioritize
                        {
                            case 1:
                                if (creature.Y != yD)
                                {
                                    MoveE(creature, 0, DiffInt(creature.Y, yD)); 
                                }
                                MoveE(creature, DiffInt(creature.X, xD), 0);
                                break;
                            case 2:
                                if (creature.X != xD)
                                {
                                    MoveE(creature, DiffInt(creature.X, xD), 0); 
                                }
                                MoveE(creature, 0, DiffInt(creature.Y, yD));
                                break;
                        }
                    }
            }
        }

        }

        // check what directions are hollow when walking towards xD and yD
        protected Constants.HollowXY HollowWay(Monster creature, int xD, int yD)
        {


            if ((mTiles[creature.X + DiffInt(creature.X, xD), creature.Y].name == Constants.Tiletype.Hollow) && (mTiles[creature.X, creature.Y + DiffInt(creature.Y, yD)].name == Constants.Tiletype.Hollow))
            {
                creature.consecutiveMovesX += 1;
                creature.consecutiveMovesY += 1;
                return Constants.HollowXY.XY;
            }
            else if (mTiles[creature.X + DiffInt(creature.X, xD), creature.Y].name == Constants.Tiletype.Hollow)
            {
                return Constants.HollowXY.X;
            }
            else if (mTiles[creature.X, creature.Y + DiffInt(creature.Y, yD)].name == Constants.Tiletype.Hollow)
            {
                return Constants.HollowXY.Y;
            }
            else
            {
                creature.consecutiveMovesX = 0;
                creature.consecutiveMovesY = 0;
                return Constants.HollowXY.None;
            }
        }

        protected int DiffInt(int a, int b)
        {
            if (a < b)
                return 1;
            else if (a > b)
                return -1;
            else
                return 0;
        }

        protected void CheckCollisions()
        {
            monsters.ToList().ForEach(m =>
            {
                if (m.stunned == 0)
                {
                    if (mplayer.X == m.X && mplayer.Y == m.Y)
                    {
                        mplayer.Hits -= 1;
                        m.stunned = Constants.StunDuration;
                    }
                }
            });

            if (mplayer.Hits < 1)
            {
                mCurrentState = GameState.GameOver;
            }
        }

        public void ChangeAttacks()
        {
            monsters.ToList().ForEach(m =>
            {
                if (m.consecutiveMovesX > Constants.ConMovesLimit && m.consecutiveMovesX > Constants.ConMovesLimit)
                {
                    switch (m.xymode)
                    {
                        case Constants.XYmode.None:
                            if (xAttackers < yAttackers)
                            {
                                m.xymode = Constants.XYmode.X;
                                xAttackers += 1;
                                m.consecutiveMovesX = 0;
                                m.consecutiveMovesY = 0;
                            }
                            else if (xAttackers > yAttackers)
                            {
                                m.xymode = Constants.XYmode.Y;
                                yAttackers += 1;
                                m.consecutiveMovesX = 0;
                                m.consecutiveMovesY = 0;
                            }
                            else
                            {
                                switch (RandomNumber(0, 1))
                                {
                                    case 0:
                                        m.xymode = Constants.XYmode.X;
                                        m.consecutiveMovesX = 0;
                                        m.consecutiveMovesY = 0;
                                        break;
                                    case 1:
                                        m.xymode = Constants.XYmode.Y;
                                        m.consecutiveMovesX = 0;
                                        m.consecutiveMovesY = 0;
                                        break;
                                }
                            }
                            break;

                        case Constants.XYmode.X:
                            if (m.X == mplayer.X)
                            {
                                m.xymode = Constants.XYmode.None;
                                m.consecutiveMovesX = 0;
                                m.consecutiveMovesY = 0;
                            }
                            break;

                        case Constants.XYmode.Y:
                            if (m.Y == mplayer.Y)
                            {
                                m.xymode = Constants.XYmode.None;
                                m.consecutiveMovesX = 0;
                                m.consecutiveMovesY = 0;
                            }
                            break;
                    }
                }
            });
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            if (mCurrentState == GameState.Intro)
            {
                spriteBatch.Draw(mLogo, new Rectangle(32 * 16 - 8 * 38, 5 * 16, 16 * 38, 16 * 19), BackgroundColor);
            }
            else
            {
                DrawLevel(spriteBatch);

                DrawCreatures(spriteBatch);

                if (mCurrentState == GameState.Paused | mCurrentState == GameState.GameOver)
                {
                    spriteBatch.Draw(mMessageBox, new Rectangle(300, 200, 500, 300), Color.Red);
                }
            }

            DrawText(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Determine the current color to "tint" the background
        protected Color BackgroundColor
        {
            get
            {
                Color mColor = Color.White;
                if (mCurrentState == GameState.Paused)
                {
                    mColor = mPauseColor;
                }

                return mColor;
            }
        }//end BackgroundColor


        // Draw the ground
        protected void DrawLevel(SpriteBatch theBatch)
        {
            for (int i = 0; i < Constants.DungeonHeight; i++)
            {
                for (int j = 0; j < Constants.DungeonWidth; j++)
                {
                    spriteBatch.Draw(mTiles[j, i].TileTexture, new Rectangle(16 * j, 16 * i, 16, 16), BackgroundColor);
                }
            }
        }//end protected void DrawCreatures(SpriteBatch theBatch)

        //Draw the creatures to be displayed
        protected void DrawCreatures(SpriteBatch theBatch)
        {
            //spriteBatch.Draw(mHollow, new Rectangle(32 * mplayer.X, 32 * mplayer.Y, 32, 32), BackgroundColor);
            spriteBatch.Draw(mAnt, new Rectangle(16 * mplayer.X, 16 * mplayer.Y, 16, 16), BackgroundColor);

            // draw monsters
            monsters.ToList().ForEach(m =>
            {
                spriteBatch.Draw(mEnemy, new Rectangle(16 * m.X, 16 * m.Y, 16, 16), BackgroundColor);
            });
        }//end protected void DrawCreatures(SpriteBatch theBatch)

        //Draw the text to be displayed
        protected void DrawText(SpriteBatch theBatch)
        {
            if (mCurrentState == GameState.Intro)
            {
                spriteBatch.DrawString(mText, "Press 'Space' to begin!", new Vector2(400, 500), Color.White);
                spriteBatch.DrawString(mText, "Use arrow keys Left & Right to change the number of monsters, and Up & Down to change the game speed", new Vector2(90, 550), Color.White);
                spriteBatch.DrawString(mText, "Monsters: " + nbrOfMonsters.ToString(), new Vector2(440, 600), Color.White);
                spriteBatch.DrawString(mText, "Speed: " + (mSpeed + 5).ToString(), new Vector2(440, 650), Color.White);
            }
            else
            {
                spriteBatch.Draw(mLogo, new Rectangle(50 * 16, 50, 5 * 38, 5 * 19), BackgroundColor);
                spriteBatch.DrawString(mText, "Monsters: " + (monsters.Count).ToString(), new Vector2(50 * 16, 210), Color.Blue);
                spriteBatch.DrawString(mText, "Score: " + mScore.ToString(), new Vector2(50 * 16, 230), Color.Blue);
                spriteBatch.DrawString(mText, "Lives: " + (mplayer.Hits).ToString(), new Vector2(50 * 16, 250), Color.Blue);

                // Following code writes the current AI mode for all enemies, used for testing
                //int a = 0;
                //monsters.ToList().ForEach(m =>
                //{
                //    a++;
                //    spriteBatch.DrawString(mText, "XY Mode: " + (m.xymode).ToString(), new Vector2(50 * 16, 270 + 20 * a), Color.Blue);

                //    spriteBatch.DrawString(mText, "Cons X: " + (m.consecutiveMovesX).ToString() + ", Cons Y: " + (m.consecutiveMovesY).ToString(), new Vector2(50 * 16, 290 + 20 * Constants.NumberOfMonsters + 20 * a), Color.Blue);

                //});

                if (mCurrentState == GameState.GameOver)
                {
                    spriteBatch.DrawString(mText, "Game Over", new Vector2(500, 280), Color.White);
                    spriteBatch.DrawString(mText, "You survived " + mElapsed.ToString() + " seconds and " + mScore.ToString() + " steps", new Vector2(390, 315), Color.White);
                    spriteBatch.DrawString(mText, "Press 'Space' to play again, 'Esc' to exit.", new Vector2(390, 350), Color.White);
                }//end if (mCurrentState == GameState.GameOver)

                if (mCurrentState == GameState.Paused)
                {
                    spriteBatch.DrawString(mText, "Paused!", new Vector2(510, 300), Color.White);
                    spriteBatch.DrawString(mText, "Press 'P' to un-pause the game.", new Vector2(420, 350), Color.White);
                }//end if (mCurrentState == GameState.Paused)
            }//end if (mCurrentState == GameState.Intro)
        }//end protected void DrawText()

        //Generate a random number between (and including) passed in min and max values
  
        protected int RandomNumber(int min, int max)
        {
            return mRandom.Next(min, max + 1);
        }//end protected int RandomNumber(int min, int max)



        public PointJ GetValidRandomPoint()
        {

            PointJ p = new PointJ(mRandom.Next(Constants.DungeonWidth), mRandom.Next(Constants.DungeonHeight));
            return p;
        } //end public PointJ GetValidRandomPoint()
    }



    public static class Constants
    {
        public readonly static int DungeonHeight = 48; // 16 * 48 = 768
        public readonly static int DungeonWidth = 48; // 16 * 32 = 1024
        public readonly static int NumberOfSwords = 10;
        public readonly static int MonsterDamage = 2;
        public readonly static int NumberOfMonsters = 5;
        public readonly static int StartingHitPoints = 2;
        public readonly static int StartingStun = 13;
        public readonly static int StunDuration = 5;
        public readonly static int ConMovesLimit = 5;

        public enum Direction { N, S, E, W, Unknown };
        public enum AImode { Angry, Ignorant };
        public enum XYmode { X, Y, None };
        public enum HollowXY { X, Y, XY, None };
        public enum Tiletype { Tile, Ground, Hollow, Player, Monster };
    }
}
