using System;
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
		int num = 0;
		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			//graphics.IsFullScreen = true;
			Resolution.Init(ref graphics);
			
			rng = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
			
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
			tileSize.X = 64;//graphics.PreferredBackBufferWidth / (gridSize.X);
			tileSize.Y = 64;//graphics.PreferredBackBufferWidth / (gridSize.X);

			Resolution.SetVirtualResolution(1920, 1080);
			Resolution.SetResolution(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height, graphics.IsFullScreen);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
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

			grid = new Grid(gridSize, new Vector2(0, 0), t_r, t_b, t_g, tileSize, @"Content/buildings/buildings.txt", @"Content/UI/ui_grid_menu.txt", Content);
            player = new Player(t_b);
			os = new Overseer(Content);
			architech = new Architech(Content);
			uiMaster = new UiMaster(Content);
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
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

            // TODO: Add your update logic here
            player.Input.KB = Keyboard.GetState();

			player.Update(uiMaster, Mouse.GetState());
			uiMaster.Update(player);
			
			grid.Update(player);
			architech.Update(grid, uiMaster, player);
			os.Update(gameTime, player.Cursor, architech, grid, rng);

            player.Input.KBP = player.Input.KB;
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

			grid.Draw(spriteBatch, player.SelectedBuilding);
			architech.Draw(spriteBatch, player, grid);
			os.Draw(spriteBatch);
			uiMaster.Draw(spriteBatch, player);
			player.Draw(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
