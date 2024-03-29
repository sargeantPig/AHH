﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AHH.Extensions;
using AHH.Base;
using AHH.UI;
using AHH.User;
using AHH.AI;
using AHH.Interactable.Building;
using AHH.Interactable.Spells;
using AHH.Research;
using AHH.UI.Elements.Messages;
using AHH.Functions;
namespace AHH
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		Random rng;
        Player player;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Grid grid;
		Point gridSize = new Point((1920/64), (1080/64) - 3);
		Vector2[] points = new Vector2[100];
		Point tileSize;
		Overseer os;
		UiMaster uiMaster;
		Architech architech;
        Researcher researcher;
		Wizard wizard;
        int num = 0;
		StaticSprite backdrop;
		StaticSprite terrain;
		StaticSprite light;
        StaticSprite overlay_Screen;
        Texture2D[] screens;

        public const int UPS = 45; // Updates per second
        public const int FPS = 60;

        TimeSpan update;
        public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
			Resolution.Init(ref graphics);
			
			rng = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
			
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			tileSize.X = 64;//graphics.PreferredBackBufferWidth / (gridSize.X);
			tileSize.Y = 64;//graphics.PreferredBackBufferWidth / (gridSize.X);

			Resolution.SetVirtualResolution(1920, 1080);
			Resolution.SetResolution(1920, 1080, graphics.IsFullScreen);

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / FPS);
            base.Initialize();
		}

		protected override void LoadContent()
		{
            // Create a new SpriteBatch, which can be used to draw textures.
            Statistics.Load(@"Content/settings/save.dat");
            Message.DisplayTime = 5000;
			AiUnit.statusBarTexture = new Texture2D[] { Content.Load<Texture2D>(@"texture/ui/status_bar_bottom"), Content.Load<Texture2D>(@"texture/ui/status_bar_top")};
			Building.statusBarTexture = new Texture2D[] { Content.Load<Texture2D>(@"texture/ui/status_bar_bottom"), Content.Load<Texture2D>(@"texture/ui/status_bar_top") };
			BaseObject.DebugFont = Content.Load<SpriteFont>(@"fonts/debug");
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Texture2D t_r = new Texture2D(GraphicsDevice, 1, 1);
			Texture2D t_g = new Texture2D(GraphicsDevice, 1, 1);
			Texture2D t_b = new Texture2D(GraphicsDevice, 1, 1);
            
			Color[] red = new Color[1];
			Color[] green = new Color[1];
			Color[] blue = new Color[1];

			for (int i = 0; i < t_r.Height * t_g.Width; i++)
			{
				red[i] = new Color(255, 0, 0);
				green[i] = new Color(0, 255, 0);
				blue[i] = new Color(0, 0, 255);
			}

			t_r.SetData<Color>(red);
			t_g.SetData<Color>(green);
			t_b.SetData<Color>(blue);

			for (int i = 0; i < points.Count(); i++)
			{
				points[i] = new Vector2(rng.Next(0, 600), rng.Next(0, 600));
			}

			grid = new Grid(gridSize, new Vector2(0, 0), Content.Load<Texture2D>(@"texture/tile_n"), Content.Load<Texture2D>(@"texture/tile_h"), Content.Load<Texture2D>(@"texture/tile_c"), tileSize, @"Content/buildings/buildings.txt", @"Content/UI/ui_grid_menu.txt", Content);
            player = new Player(t_b, new Point(1900, 50), new Texture2D[] {Content.Load<Texture2D>(@"texture/ui/healthbar_bottom"), Content.Load<Texture2D>(@"texture/ui/healthbar_top")}, Content);
			os = new Overseer(Content, new Texture2D[] { t_r, t_g });
			architech = new Architech(Content, grid);
			uiMaster = new UiMaster(Content);
			wizard = new Wizard(Content);
            researcher = new Researcher(Content, rng);
			backdrop = new StaticSprite(new Vector2(0, 0), Content.Load<Texture2D>(@"texture/ui/backdrop2"), new Point(1920, 1080));
			terrain = new StaticSprite(new Vector2(0, -5), Content.Load<Texture2D>(@"texture/terrain"), new Point(1920, 1080 - (3 * 64)));
            light = new StaticSprite(Vector2.Zero, Content.Load<Texture2D>(@"texture/terrainLight"), new Point(1920, 1080 - (3 * 64)));
            screens = new Texture2D[3];

            screens[0] = Content.Load<Texture2D>(@"texture/menu_screen");
            screens[1] = Content.Load<Texture2D>(@"texture/ui/guide");
            screens[2] = Content.Load<Texture2D>(@"texture/end_screen");
            overlay_Screen = new StaticSprite(Vector2.Zero, screens[0], new Point(1920, 1080));

            update = new TimeSpan();
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
		protected override void Update(GameTime gameTime)
		{
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                uiMaster.NextAction == ButtonFunction.Quit)
            {
                Statistics.Save(@"Content/settings/save.dat");
                Exit();
            }

            update += gameTime.ElapsedGameTime;
            // TODO: Add your update logic here
            Options.Update(gameTime);
            player.Input.KB = Keyboard.GetState();
			player.Update(uiMaster, Mouse.GetState(), architech, gameTime);
            uiMaster.Update(player, gameTime);

            switch (player.Mode)
            {
                case Player_Modes.Tutorial:
                    overlay_Screen.Texture = screens[1];
                    break;
                case Player_Modes.MainMenu:
                    overlay_Screen.Texture = screens[0];
                    break;
                case Player_Modes.Building:
                case Player_Modes.Research:
                case Player_Modes.Spells:
                case Player_Modes.Tools:
                case Player_Modes.Pause:
                    break;
                case Player_Modes.End_Screen:
                    overlay_Screen.Texture = screens[2];
                    break;
                case Player_Modes.ES_Death:
                    overlay_Screen.Texture = screens[2];
                    break;
                case Player_Modes.ES_God:
                    overlay_Screen.Texture = screens[3];
                    break;
                case Player_Modes.ES_Passive:
                    overlay_Screen.Texture = screens[4];
                    break;
            }

            if (player.Mode != Player_Modes.Pause && player.Mode != Player_Modes.MainMenu && player.Mode != Player_Modes.Tutorial &&
                player.Mode != Player_Modes.ES_Passive && player.Mode != Player_Modes.ES_God && player.Mode != Player_Modes.ES_Death && player.Mode != Player_Modes.End_Screen)
            {
                grid.Update(player, architech, os);
                architech.Update(grid, uiMaster, os, player, gameTime, os.GetUnitRects());
                if (update.TotalMilliseconds > 1000f / UPS)
                {
                    update = new TimeSpan();
                    os.Update(gameTime, player.Cursor, architech, uiMaster, grid, player, rng);
                }
                wizard.Update(gameTime, architech, os, uiMaster, grid, player);
                researcher.Update(gameTime, os, architech, uiMaster, player, wizard, rng);
                player.Input.KBP = player.Input.KB;
                player.Cursor.prevState = player.Cursor.GetState;

                if (Options.GetTick)
                    player.UpdateEnergy();
            }

			base.Update(gameTime);  
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			//GraphicsDevice.Clear(Color.Black);

			Resolution.BeginDraw();

			spriteBatch.Begin(SpriteSortMode.Immediate,
								   BlendState.AlphaBlend,
								   SamplerState.PointClamp, null, null, null,
								   Resolution.getTransformationMatrix());
			backdrop.Draw(spriteBatch);

            if (player.Mode != Player_Modes.MainMenu && player.Mode != Player_Modes.Tutorial &&
                player.Mode != Player_Modes.ES_Passive && player.Mode != Player_Modes.ES_God &&
                player.Mode != Player_Modes.ES_Death && player.Mode != Player_Modes.End_Screen)
            {
                terrain.Draw(spriteBatch);
                grid.Draw(spriteBatch, player.SelectedBuilding);
                architech.Draw(spriteBatch, player, grid);
                os.Draw(spriteBatch, player);
                wizard.Draw(spriteBatch);
                light.Draw(spriteBatch);
            }

            else if (player.Mode == Player_Modes.MainMenu || player.Mode == Player_Modes.Tutorial || player.Mode == Player_Modes.ES_Death ||
                player.Mode == Player_Modes.ES_God || player.Mode == Player_Modes.ES_Passive || player.Mode == Player_Modes.End_Screen)
                overlay_Screen.Draw(spriteBatch);

			uiMaster.Draw(spriteBatch, player);
			player.Draw(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
