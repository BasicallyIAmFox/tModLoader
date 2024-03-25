using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace tModLoader.SourceGenerator.NPCShopDatabase;

public sealed record class Model
{
	public readonly record struct Shop(int Id, EquatableArray<Shop.Item> Items)
	{
		public readonly record struct Item(int Id, EquatableArray<Condition> Conditions)
		{
			public sealed class Builder(int id)
			{
				private readonly List<Condition> conditions = [];

				public int Id { get; } = id;
				public IEnumerable<Condition> Conditions => conditions;

				public void AddConditions(params Condition[] conditions)
				{
					this.conditions.AddRange(conditions);
				}

				public Item ToItem() => new Item(Id, Conditions.ToImmutableArray());
			}

			public int Id { get; } = Id;
			public EquatableArray<Condition> Conditions { get; } = Conditions;
		}

		public readonly record struct Condition(Condition.ConditionType Type, int? Data1 = null, int? Data2 = null)
		{
			public enum ConditionType : uint
			{
				NearWater = 0,
				NearLava = 1,
				NearHoney = 2,
				NearShimmer = 3,

				TimeDay = 4,
				TimeNight = ~TimeDay,

				InDungeon = 5,
				InCorrupt = 6,
				InHallow = 7,
				InMeteor = 8,
				InJungle = 9,
				InSnow = 10,
				InCrimson = 11,
				InWaterCandle = 12,
				InPeaceCandle = 13,
				InTowerSolar = 14,
				InTowerVortex = 15,
				InTowerNebula = 16,
				InTowerStardust = 17,
				InDesert = 18,
				InGlowshroom = 19,
				InUndergroundDesert = 20,
				InSkyHeight = 21,
				InOverworldHeight = 22,
				InDirtLayerHeight = 23,
				InRockLayerHeight = 24,
				InUnderworldHeight = 25,
				InBeach = 26,
				InRain = 27,
				InSandstorm = 28,
				InOldOnesArmy = 29,
				InGranite = 30,
				InMarble = 31,
				InHive = 32,
				InGemCave = 33,
				InLihzhardTemple = 34,
				InGraveyard = 35,
				InAether = 36,
				InShoppingZoneForest = 37,
				InBelowSurface = 38,
				InEvilBiome = 39,
				NotInEvilBiome = ~InEvilBiome,
				NotInHallowBiome = ~InHallow,
				NotInGraveyard = ~InGraveyard,
				NotInUnderworld = ~InUnderworldHeight,

				InClassicMode = ~InExpertMode,
				InExpertMode = 40,
				InMasterMode = 41,
				InJourneyMode = 42,

				Hardmode = 43,
				PreHardmode = ~Hardmode,
				SmashedShadowOrb = 44,
				CrimsonWorld = 45,
				CorruptWorld = ~CrimsonWorld,

				DrunkWorld = 46,
				RemixWorld = 47,
				NotTheBeesWorld = 48,
				ForTheWorthyWorld = 49,
				TenthAnniversaryWorld = 50,
				DontStarveWorld = 51,
				NoTrapsWorld = 52,
				ZenithWorld = 53,
				NotDrunkWorld = ~DrunkWorld,
				NotRemixWorld = ~RemixWorld,
				NotNotTheBeesWorld = ~NotTheBeesWorld,
				NotForTheWorthyWorld = ~ForTheWorthyWorld,
				NotTenthAnniversaryWorld = ~TenthAnniversaryWorld,
				NotDontStarveWorld = ~DontStarveWorld,
				NotNoTrapsWorld = ~NoTrapsWorld,
				NotZenithWorld = ~ZenithWorld,

				Christmas = 54,
				Halloween = 55,
				BloodMoon = 56,
				NotBloodMoon = ~BloodMoon,
				Eclipse = 57,
				NotEclipse = ~Eclipse,
				EclipseOrBloodMoon = 58,
				NotEclipseAndNotBloodMoon = ~EclipseOrBloodMoon,
				Thunderstorm = 59,
				BirthdayParty = 60,
				LanternNight = 61,
				HappyWindyDay = 62,

				DownedKingSlime = 63,
				DownedEyeOfCthulhu = 64,
				DownedEowOrBoc = 65,
				DownedEaterOfWorlds = 66,
				DownedBrainOfCthulhu = 67,
				DownedQueenBee = 68,
				DownedDeerclops = 69,
				DownedSkeletron = 70,
				DownedQueenSlime = 71,
				DownedEarlygameBoss = 72,
				DownedMechBossAny = 73,
				DownedTwins = 74,
				DownedDestroyer = 75,
				DownedSkeletronPrime = 76,
				DownedMechBossAll = 77,
				DownedPlantera = 78,
				DownedEmpressOfLight = 79,
				DownedDukeFishron = 80,
				DownedGolem = 81,
				DownedMourningWood = 82,
				DownedPumpking = 83,
				DownedEverscream = 84,
				DownedSantaNK1 = 85,
				DownedIceQueen = 86,
				DownedCultist = 87,
				DownedMoonLord = 88,
				DownedClown = 89,
				DownedGoblinArmy = 90,
				DownedPirates = 91,
				DownedMartians = 92,
				DownedFrostLegion = 93,
				DownedSolarPillar = 94,
				DownedVortexPillar = 95,
				DownedNebulaPillar = 96,
				DownedStardustPillar = 97,
				DownedOldOnesArmyAny = 98,
				DownedOldOnesArmyT1 = 99,
				DownedOldOnesArmyT2 = 100,
				DownedOldOnesArmyT3 = 101,
				NotDownedKingSlime = ~DownedKingSlime,
				NotDownedEyeOfCthulhu = ~DownedEyeOfCthulhu,
				NotDownedEowOrBoc = ~DownedEowOrBoc,
				NotDownedEaterOfWorlds = ~DownedEaterOfWorlds,
				NotDownedBrainOfCthulhu = ~DownedBrainOfCthulhu,
				NotDownedQueenBee = ~DownedQueenBee,
				NotDownedDeerclops = ~DownedDeerclops,
				NotDownedSkeletron = ~DownedSkeletron,
				NotDownedQueenSlime = ~DownedQueenSlime,
				NotDownedEarlygameBoss = ~DownedEarlygameBoss,
				NotDownedMechBossAny = ~DownedMechBossAny,
				NotDownedTwins = ~DownedTwins,
				NotDownedDestroyer = ~DownedDestroyer,
				NotDownedSkeletronPrime = ~DownedSkeletronPrime,
				NotDownedMechBossAll = ~DownedMechBossAll,
				NotDownedPlantera = ~DownedPlantera,
				NotDownedEmpressOfLight = ~DownedEmpressOfLight,
				NotDownedDukeFishron = ~DownedDukeFishron,
				NotDownedGolem = ~DownedGolem,
				NotDownedMourningWood = ~DownedMourningWood,
				NotDownedPumpking = ~DownedPumpking,
				NotDownedEverscream = ~DownedEverscream,
				NotDownedSantaNK1 = ~DownedSantaNK1,
				NotDownedIceQueen = ~DownedIceQueen,
				NotDownedCultist = ~DownedCultist,
				NotDownedMoonLord = ~DownedMoonLord,
				NotDownedClown = ~DownedClown,
				NotDownedGoblinArmy = ~DownedGoblinArmy,
				NotDownedPirates = ~DownedPirates,
				NotDownedMartians = ~DownedMartians,
				NotDownedFrostLegion = ~DownedFrostLegion,
				NotDownedSolarPillar = ~DownedSolarPillar,
				NotDownedVortexPillar = ~DownedVortexPillar,
				NotDownedNebulaPillar = ~DownedNebulaPillar,
				NotDownedStardustPillar = ~DownedStardustPillar,
				NotDownedOldOnesArmyAny = ~DownedOldOnesArmyAny,
				NotDownedOldOnesArmyT1 = ~DownedOldOnesArmyT1,
				NotDownedOldOnesArmyT2 = ~DownedOldOnesArmyT2,
				NotDownedOldOnesArmyT3 = ~DownedOldOnesArmyT3,

				BloodMoonOrHardmode = 102,
				NightOrEclipse = 103,

				Multiplayer = 104,
				HappyEnough = 105,
				HappyEnoughToSellPylons = 106,
				AnotherTownNPCNearby = 107,
				IsNpcShimmered = 108,

				MoonPhaseFull = 109,
				MoonPhaseWaningGibbous = 110,
				MoonPhaseThirdQuarter = 111,
				MoonPhaseWaningCrescent = 112,
				MoonPhaseNew = 113,
				MoonPhaseWaxingCrescent = 114,
				MoonPhaseFirstQuarter = 115,
				MoonPhaseWaxingGibbous = 116,
				MoonPhasesQuarter0 = 117,
				MoonPhasesQuarter1 = 118,
				MoonPhasesQuarter2 = 119,
				MoonPhasesQuarter3 = 120,
				MoonPhasesHalf0 = 121,
				MoonPhasesHalf1 = ~MoonPhasesHalf0,
				MoonPhasesEven = 122,
				MoonPhasesOdd = ~MoonPhasesEven,
				MoonPhasesNearNew = 123,
				MoonPhasesEvenQuarters = 124,
				MoonPhasesOddQuarters = ~MoonPhasesEvenQuarters,
				MoonPhases04 = 125,
				MoonPhases15 = 126,
				MoonPhases26 = 127,
				MoonPhases37 = 128,

				DownedB2B3HM = 129,
				WorldGenSilver = 130,
				WorldGenTungsten = ~WorldGenSilver,
				NightAfterEvilOrHardmode = 132,
				MoonPhasesHalf0OrPreHardmode = 133,
				HardmodeOrFTW = 134,
				InBeach2 = 135,
				AtLeastXHealth = 136,
				AtLeastXMana = 137,
				PlatinumCoin = 138,
				StyleMoon = 139,
				OnTeam = 140,
				NightDayFullMoon = 141,
				DaytimeNotFullMoon = ~NightDayFullMoon,
				NoAteLoaf = 142,
				Periodically1 = 143,
				Periodically2 = ~Periodically1,
				BestiaryFull = 144,

				PlayerCarriesItem = 145,
				GolfScoreOver = 146,
				NpcIsPresent = 147,
				AnglerQuestsFinishedOver = 148,
				BestiaryFilledPercent = 149,
			}

			public ConditionType Type { get; } = Type;
			public int? Data1 { get; } = Data1;
			public int? Data2 { get; } = Data2;
		}

		public sealed class Builder(int id)
		{
			private readonly List<Item> items = [];

			public int Id { get; } = id;
			public IEnumerable<Item> Items => items;

			public void AddItem(Item item)
			{
				items.Add(item);
			}

			public void AddItems(params Item[] items)
			{
				this.items.AddRange(items);
			}

			public Shop ToShop() => new Shop(Id, Items.ToImmutableArray());
		}

		public int Id { get; } = Id;
		public EquatableArray<Item> Items { get; } = Items;
	}
}
