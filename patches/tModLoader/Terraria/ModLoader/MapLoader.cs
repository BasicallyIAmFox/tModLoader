using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;

namespace Terraria.ModLoader;

//todo: further documentation
internal static class MapLoader
{
	internal static bool initialized = false;
	internal static readonly Dictionary<ushort, IList<MapEntry>> tileEntries = [];
	internal static readonly Dictionary<ushort, IList<MapEntry>> wallEntries = [];
	internal static readonly Dictionary<ushort, ushort> entryToTile = [];
	internal static readonly Dictionary<ushort, ushort> entryToWall = [];
	internal static readonly Dictionary<ModCactus, ushort> cactusToEntry = [];

	internal static readonly Dictionary<ushort, Func<string, int, int, string>> nameFuncs = [];

	internal static int modTileOptions(ushort type)
	{
		return tileEntries[type].Count;
	}

	internal static int modWallOptions(ushort type)
	{
		return wallEntries[type].Count;
	}
	//make Terraria.Map.MapHelper.colorLookup internal
	//add internal modPosition field to Terraria.Map.MapHelper
	//near end of Terraria.Map.MapHelper.Initialize set modPosition to num11 + 1
	//in Terraria.Map.MapHelper.SaveMap add mod-type-check to darkness check
	internal static void FinishSetup()
	{
		if (Main.dedServ) {
			return;
		}

		Array.Resize(ref MapHelper.tileLookup, TileLoader.TileCount);
		Array.Resize(ref MapHelper.wallLookup, WallLoader.WallCount);

		var colors = new List<Color>();
		var names = new List<LocalizedText>();

		foreach (ushort type in tileEntries.Keys) {
			MapHelper.tileLookup[type] = (ushort)(MapHelper.modPosition + colors.Count);
			foreach (MapEntry entry in tileEntries[type]) {
				ushort mapType = (ushort)(MapHelper.modPosition + colors.Count);
				entryToTile[mapType] = type;
				nameFuncs[mapType] = entry.getName;
				colors.Add(entry.color);
				if (entry.name != null) {
					names.Add(entry.name);
				}
				else {
					throw new Exception("How did this happen?");
					//names.Add(Language.GetText(entry.translation.Key));
				}
			}
		}

		foreach (ushort type in wallEntries.Keys) {
			MapHelper.wallLookup[type] = (ushort)(MapHelper.modPosition + colors.Count);
			foreach (MapEntry entry in wallEntries[type]) {
				ushort mapType = (ushort)(MapHelper.modPosition + colors.Count);
				entryToWall[mapType] = type;
				nameFuncs[mapType] = entry.getName;
				colors.Add(entry.color);
				if (entry.name != null) {
					names.Add(entry.name);
				}
				else {
					throw new Exception("How did this happen?");
					//names.Add(Language.GetText(entry.translation.Key));
				}
			}
		}

		{
			var modCactiArray = ModContent.GetContent<ModCactus>().ToArray();
			int oldLength = MapHelper.tileLookup.Length;

			Array.Resize(ref MapHelper.tileLookup, oldLength + modCactiArray.Length);

			for (int type = 0; type < modCactiArray.Length; type++) {
				var cacti = modCactiArray[type];

				ushort mapType = (ushort)(MapHelper.modPosition + colors.Count);
				var color = cacti.MapColor ?? default;

				MapHelper.tileLookup[oldLength + type] = mapType;

				entryToTile[mapType] = (ushort)(oldLength + type);
				nameFuncs[mapType] = (_, _, _) => Lang.GetMapObjectName(TileID.Cactus);
				cactusToEntry[cacti] = mapType;

				colors.Add(color);
				names.Add(Lang._itemNameCache[ItemID.Cactus] /* TODO: Is Lang.GetMapObjectName available at this stage? */);
			}
		}

		Array.Resize(ref MapHelper.colorLookup, MapHelper.modPosition + colors.Count);
		Lang._mapLegendCache.Resize(MapHelper.modPosition + names.Count);
		for (int k = 0; k < colors.Count; k++) {
			MapHelper.colorLookup[MapHelper.modPosition + k] = colors[k];
			Lang._mapLegendCache[MapHelper.modPosition + k] = names[k];
		}
		initialized = true;
	}

	internal static void UnloadModMap()
	{
		tileEntries.Clear();
		wallEntries.Clear();
		if (Main.dedServ) {
			return;
		}
		nameFuncs.Clear();
		entryToTile.Clear();
		entryToWall.Clear();
		cactusToEntry.Clear();
		Array.Resize(ref MapHelper.tileLookup, TileID.Count);
		Array.Resize(ref MapHelper.wallLookup, WallID.Count);
		Array.Resize(ref MapHelper.colorLookup, MapHelper.modPosition);
		Lang._mapLegendCache.Resize(MapHelper.modPosition);
		initialized = false;
	}
	//at end of Terraria.Map.MapHelper.CreateMapTile before returning call
	//  MapLoader.ModMapOption(ref num16, i, j);
	internal static void ModMapOption(ref ushort mapType, int i, int j)
	{
		if (mapType == MapHelper.TileToLookup(TileID.Cactus, 0)) {
			var tile = Main.tile[i, j];

			WorldGen.GetCactusType(i, j, tile.TileFrameX, tile.TileFrameY, out int sandType);

			var cactus = PlantLoader.Get<ModCactus>(TileID.Cactus, sandType);
			if (cactus != null) {
				if (cactus.MapColor is null) {
					mapType = (ushort)MapHelper.TileToLookup(TileID.Cactus, 0);
				}
				else {
					mapType = cactusToEntry[cactus];
				}

				return;
			}
		}

		if (entryToTile.TryGetValue(mapType, out ushort tileId)) {
			ModTile tile = TileLoader.GetTile(tileId);
			ushort option = tile.GetMapOption(i, j);
			if (option < 0 || option >= modTileOptions(tile.Type)) {
				throw new ArgumentOutOfRangeException("Bad map option for tile " + tile.Name + " from mod " + tile.Mod.Name);
			}
			mapType += option;
		}
		else if (entryToWall.TryGetValue(mapType, out ushort wallId)) {
			ModWall wall = WallLoader.GetWall(wallId);
			ushort option = wall.GetMapOption(i, j);
			if (option < 0 || option >= modWallOptions(wall.Type)) {
				throw new ArgumentOutOfRangeException("Bad map option for wall " + wall.Name + " from mod " + wall.Mod.Name);
			}
			mapType += option;
		}
	}
}
