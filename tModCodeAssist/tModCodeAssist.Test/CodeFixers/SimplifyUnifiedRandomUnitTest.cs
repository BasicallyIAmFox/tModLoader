﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = tModCodeAssist.Test.Verifier.Analyzer<tModCodeAssist.SimplifyUnifiedRandom.SimplifyUnifiedRandomAnalyzer>.CodeFixer<tModCodeAssist.SimplifyUnifiedRandom.SimplifyUnifiedRandomCodeFixProvider>;

namespace tModCodeAssist.Test.CodeFixers;

[TestClass]
public sealed class SimplifyUnifiedRandomUnitTest
{
	[TestMethod]
	public async Task Test_Equality()
	{
		await VerifyCS.Run(
			"""
			using Terraria;

			_ = [|Main.rand.Next(5) == 0|];
			""",
			"""
			using Terraria;
			
			_ = Main.rand.NextBool(5);
			""");
	}

	[TestMethod]
	public async Task Test_Inequality()
	{
		await VerifyCS.Run(
			"""
			using Terraria;

			_ = [|Main.rand.Next(5) != 0|];
			""",
			"""
			using Terraria;
			
			_ = !Main.rand.NextBool(5);
			""");
	}

	[TestMethod]
	public async Task Test_SwappedOperands()
	{
		await VerifyCS.Run(
			"""
			using Terraria;

			_ = [|0 == Main.rand.Next(5)|];
			""",
			"""
			using Terraria;
			
			_ = Main.rand.NextBool(5);
			""");
	}
}
