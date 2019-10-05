using DotNes.NES;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace DotNes
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont Font;
       
        Cartridge cart;
        Bus nes;


        private byte selectedPaleta = 0;

        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        Dictionary<ushort, string> mapAsm;


        protected override void Initialize()
        {
            
            nes = new Bus(GraphicsDevice);
            
            cart = new Cartridge("nestest.nes");
            if (!cart.ImageValid())
            {
                return;
            }
            nes.InsertCartridge(cart);

            mapAsm = nes.CPU.Disassemble(0x0000, 0xFFFF);
            nes.Reset();
            base.Initialize();
        }

        private string GetCurrentInstruction()
        {
            string v = mapAsm.FirstOrDefault(m => m.Key == nes.CPU.pc).Value;
            return v;
        }

        private string hex(ushort d, int size)
        {
            string format = "X" + size;
            return d.ToString(format);
        }
        void DrawRam(int x, int y, ushort nAddr, int nRows, int nColumns)
        {
            int nRamX = x, nRamY = y;
            for (int row = 0; row < nRows; row++)
            {
                string sOffset = "$" + hex(nAddr, 4) + ":";
                for (int col = 0; col < nColumns; col++)
                {
                    sOffset += " " + hex(nes.cpuRead(nAddr, true), 2);
                    nAddr += 1;
                }
                spriteBatch.DrawString(Font, sOffset, new Vector2(nRamX, nRamY), Color.White);
                nRamY += 15;
            }
        }

        void DrawCpu(int x, int y)
        {
            spriteBatch.DrawString(Font, "Status", new Vector2(x, y), Color.White);
            spriteBatch.DrawString(Font, "N", new Vector2(x + 60, y), ((nes.CPU.status & (byte)FLAGS6502.N) != 0 )? Color.Green : Color.Red);
            spriteBatch.DrawString(Font, "V", new Vector2(x + 90, y), ((nes.CPU.status & (byte)FLAGS6502.V) != 0 )? Color.Green : Color.Red);
            spriteBatch.DrawString(Font, "-", new Vector2(x + 120, y), ((nes.CPU.status & (byte)FLAGS6502.U) != 0 )? Color.Green : Color.Red);
            spriteBatch.DrawString(Font, "B", new Vector2(x + 150, y), ((nes.CPU.status & (byte)FLAGS6502.B) != 0 )? Color.Green : Color.Red);
            spriteBatch.DrawString(Font, "D", new Vector2(x + 180, y), ((nes.CPU.status & (byte)FLAGS6502.D) != 0 )? Color.Green : Color.Red);
            spriteBatch.DrawString(Font, "I", new Vector2(x + 210, y), ((nes.CPU.status & (byte)FLAGS6502.I) != 0 )? Color.Green : Color.Red);
            spriteBatch.DrawString(Font, "Z", new Vector2(x + 240, y), ((nes.CPU.status & (byte)FLAGS6502.Z) != 0 )? Color.Green : Color.Red);
            spriteBatch.DrawString(Font, "C", new Vector2(x + 270, y), ((nes.CPU.status & (byte)FLAGS6502.C) != 0 )? Color.Green : Color.Red);
            spriteBatch.DrawString(Font, "C", new Vector2(x + 300, y), ((nes.CPU.status & (byte)FLAGS6502.C) != 0 )? Color.Green : Color.Red);

            spriteBatch.DrawString(Font, "PC: " + hex(nes.CPU.pc, 4), new Vector2(x, y+20), Color.White);
            spriteBatch.DrawString(Font, "A: " + hex(nes.CPU.a, 2), new Vector2(x, y+40), Color.White);
            spriteBatch.DrawString(Font, "X: " + hex(nes.CPU.x, 2), new Vector2(x, y+60), Color.White);
            spriteBatch.DrawString(Font, "Y: " + hex(nes.CPU.y, 2), new Vector2(x, y+80), Color.White);
            spriteBatch.DrawString(Font, "STACK: " + hex(nes.CPU.stkp, 2), new Vector2(x, y+100), Color.White);


        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("Arial");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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


        bool run = false;
        bool run2 = false;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState newState = Keyboard.GetState();

            // Check to see whether the Spacebar is down.
            if (newState.IsKeyDown(Keys.Space))
            {
                run = !run;
            }

            if (newState.IsKeyDown(Keys.R))
            {
                run2 = !run2;
            }

            if (newState.IsKeyDown(Keys.P))
            {
                selectedPaleta++;
                (selectedPaleta) &= 0x07;
            }
                
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private double fResidualTime = 0;
        private double fElapsedTime = 0;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue);
            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;


           

            if (run)
            {
                fElapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds;

                if (fResidualTime > 0.0f)
                    fResidualTime -= fElapsedTime;
                else
                {
                    fResidualTime += (1.0f / 60.0f) - fElapsedTime;
                    do { nes.Clock(); } while (!nes.ppu.frame_complete);
                    nes.ppu.frame_complete = false;
                }


                spriteBatch.Begin();
                spriteBatch.Draw(nes.ppu.GetScreen(), new Rectangle(0, 0, 256, 240), Color.White);
                spriteBatch.DrawString(Font, frameRate.ToString(), new Vector2(0, 260), Color.White);
                //spriteBatch.DrawString(Font, GetCurrentInstruction(), new Vector2(0, 280), Color.White);


                spriteBatch.Draw(nes.ppu.GetPatternTable(0, selectedPaleta), new Rectangle(516, 348, 128, 128), Color.White);
                spriteBatch.Draw(nes.ppu.GetPatternTable(1, selectedPaleta), new Rectangle(648, 348, 128, 128), Color.White);



                DrawRam(260, 0, 0, 10, 20);
                DrawCpu(0, 300);
                spriteBatch.End();
            }
            

            base.Draw(gameTime);
        }
    }
}
