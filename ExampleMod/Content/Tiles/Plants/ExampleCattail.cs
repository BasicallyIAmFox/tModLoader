using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles.Plants
{
	public class ExampleCattail : ModCattail
	{
		private Asset<Texture2D> texture;

		public override void SetStaticDefaults() {
			// Makes Example Cattail grow on ExampleBlock
			GrowsOnTileId = new int[1] { ModContent.TileType<ExampleBlock>() };
			texture = ModContent.Request<Texture2D>("ExampleMod/Content/Tiles/Plants/ExampleCattail");
		}

		public override Asset<Texture2D> GetTexture() => texture;
	}
}
