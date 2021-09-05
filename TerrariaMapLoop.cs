using Terraria.GameInput;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace TerrariaMapLoop
{
	public class TerrariaMapLoop : Mod
	{
		public TerrariaMapLoop() : base()
		{

		}
		public override void Load()
		{
			On.Terraria.Main.ClampScreenPositionToWorld += Main_ClampScreenPositionToWorld; //nothing
			On.Terraria.Player.BordersMovement += Player_BordersMovement;
			On.Terraria.Player.Update += Player_Update;
			On.Terraria.GameContent.Drawing.TileDrawing.Draw += TileDrawing_Draw;
			base.Load();
		}

		private void TileDrawing_Draw(On.Terraria.GameContent.Drawing.TileDrawing.orig_Draw orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
		{
			orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
			int maxX = (int)Main.rightWorld / 16;
			for (int x = 0; x < 41; x++)
			{
				for (int y = 0; y < Main.tile.GetLength(1); y++)
				{
					int theX = maxX - (41 * 2) + x;
					//int theX = (41 * 2) + x;
					//Main.
					Main.tile[x, y] = new Tile(Main.tile[theX, y]);
					if (Main.tile[theX, y] != null)
					{
						Main.tile[x, y].IsActive = Main.tile[theX, y].IsActive;
					}
				}
			}
			for (int x = 0; x < 41; x++)
			{
				for (int y = 0; y < Main.tile.GetLength(1); y++)
				{
					int theX = x+41;
					//int theX = (41 * 2) + x;
					//Main.
					Main.tile[maxX - 41 + x, y] = new Tile(Main.tile[theX, y]);
					if (Main.tile[theX, y] != null)
					{
						Main.tile[maxX - 41 + x, y].IsActive = Main.tile[theX, y].IsActive;
					}
				}
			}
			

			/*Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
			Vector2 zero = new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange);
			int firstTileX;
			int lastTileX;
			int firstTileY;
			int lastTileY;
			GetScreenDrawArea(unscaledPosition, zero + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition), out firstTileX, out lastTileX, out firstTileY, out lastTileY);
			TileDrawInfo tileDrawInfo = ((ThreadLocal<TileDrawInfo>)typeof(Terraria.GameContent.Drawing.TileDrawing).GetProperty("_currentTileDrawInfo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self)).Value;
			Tile tile = Main.tile[600, 600];*/
			//self.drawSi

		}

		private void Player_Update(On.Terraria.Player.orig_Update orig, Player self, int i)
		{
			orig(self, i);
			if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.K))
			{
				self.position.X = 1500;
				self.position.Y -= 600;
			}
		}

		private void Player_BordersMovement(On.Terraria.Player.orig_BordersMovement orig, Terraria.Player self)
		{
			if (self.position.X < Main.leftWorld + 640f + 16f && self.velocity.X < 0)
			{
				Main.cameraX = 0f;
				self.position.X = Main.rightWorld - 41f*16f;
				//self.velocity.X = 0f;
			}
			if (self.position.X + (float)self.width > Main.rightWorld - 40f * 16f && self.velocity.X > 0)
			{
				Main.cameraX = 0f;
				self.position.X = Main.leftWorld + 41f*16f;
				//self.velocity.X = 0f;
			}
			if (self.position.Y < Main.topWorld + 640f + 16f)
			{
				self.position.Y = Main.topWorld + 640f + 16f;
				if ((double)self.velocity.Y < 0.11)
				{
					self.velocity.Y = 0.11f;
				}
				self.gravDir = 1f;
				AchievementsHelper.HandleSpecialEvent(self, 11);
			}
			if (self.position.Y > Main.bottomWorld - 640f - 32f - (float)self.height)
			{
				self.position.Y = Main.bottomWorld - 640f - 32f - (float)self.height;
				self.velocity.Y = 0f;
			}
			if (self.position.Y > Main.bottomWorld - 640f - 150f - (float)self.height)
			{
				AchievementsHelper.HandleSpecialEvent(self, 10);
			}
		}

		private void Main_ClampScreenPositionToWorld(On.Terraria.Main.orig_ClampScreenPositionToWorld orig)
		{

		}

		private void GetScreenDrawArea(Vector2 screenPosition, Vector2 offSet, out int firstTileX, out int lastTileX, out int firstTileY, out int lastTileY)
		{
			firstTileX = (int)((screenPosition.X - offSet.X) / 16f - 1f);
			lastTileX = (int)((screenPosition.X + (float)Main.screenWidth + offSet.X) / 16f) + 2;
			firstTileY = (int)((screenPosition.Y - offSet.Y) / 16f - 1f);
			lastTileY = (int)((screenPosition.Y + (float)Main.screenHeight + offSet.Y) / 16f) + 5;
			if (firstTileX < 4)
			{
				firstTileX = 4;
			}
			if (lastTileX > Main.maxTilesX - 4)
			{
				lastTileX = Main.maxTilesX - 4;
			}
			if (firstTileY < 4)
			{
				firstTileY = 4;
			}
			if (lastTileY > Main.maxTilesY - 4)
			{
				lastTileY = Main.maxTilesY - 4;
			}
			if (Main.sectionManager.FrameSectionsLeft > 0)
			{
				TimeLogger.DetailedDrawReset();
				WorldGen.SectionTileFrameWithCheck(firstTileX, firstTileY, lastTileX, lastTileY);
				TimeLogger.DetailedDrawTime(5);
			}
		}
	}
}