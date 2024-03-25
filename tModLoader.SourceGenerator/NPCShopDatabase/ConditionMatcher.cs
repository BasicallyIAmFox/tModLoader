using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace tModLoader.SourceGenerator.NPCShopDatabase;

public static class ConditionMatcher
{
	public sealed record class ConditionMatch(Model.Shop.Condition Condition)
	{
		public Model.Shop.Condition Condition { get; private set; } = Condition;

		public bool TryInvert()
		{
			if (!Enum.IsDefined(typeof(Model.Shop.Condition.ConditionType), ~Condition.Type))
				return false;

			Condition = new(~Condition.Type);
			return true;
		}
	}

	public static bool TryTransform(ExpressionSyntax node, out List<ConditionMatch> matches)
	{
		matches = [];

		if (TryTransformInternal(node, out var match3)) {
			matches.Add(match3);
			return true;
		}

		while (node.IsKind(SyntaxKind.LogicalAndExpression)) {
			var transformed = (BinaryExpressionSyntax)node;

			if (TryTransform(transformed.Left, out var matches1) && TryTransform(transformed.Right, out var matches2)) {
				matches.AddRange(matches1);
				matches.AddRange(matches2);
				return true;
			}

			matches.Clear();
			return false;
		}

		return false;
	}

	private static bool TryTransformInternal(ExpressionSyntax node, out ConditionMatch match)
	{
		match = default;

		bool shouldBeInverted = false;

		if (node.IsKind(SyntaxKind.LogicalNotExpression)) {
			node = ((PrefixUnaryExpressionSyntax)node).Operand;
			shouldBeInverted = true;

			while (node.IsKind(SyntaxKind.ParenthesizedExpression)) {
				node = ((ParenthesizedExpressionSyntax)node).Expression;
			}
		}

		switch (node) {
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "dayTime"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.TimeDay));
				break;
			case PrefixUnaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalNotExpression,
				Operand: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "dayTime"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.TimeNight));
				break;

			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneDungeon"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InDungeon));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneCorrupt"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InCorrupt));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneHallow"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InHallow));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneMeteor"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InMeteor));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneJungle"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InJungle));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneSnow"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InSnow));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneCrimson"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InCrimson));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneWaterCandle"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InWaterCandle));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZonePeaceCandle"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InPeaceCandle));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneTowerSolar"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InTowerSolar));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneTowerVortex"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InTowerVortex));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneTowerNebula"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InTowerNebula));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneTowerStardust"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InTowerStardust));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneDesert"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InDesert));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneGlowshroom"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InGlowshroom));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneUndergroundDesert"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InUndergroundDesert));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneSkyHeight"
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanExpression,
				Left: CastExpressionSyntax {
					Expression: ParenthesizedExpressionSyntax {
						Expression: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.DivideExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: MemberAccessExpressionSyntax {
									Expression: ElementAccessExpressionSyntax {
										Expression: MemberAccessExpressionSyntax {
											Expression: IdentifierNameSyntax {
												Identifier.ValueText: "Main"
											},
											Name.Identifier.ValueText: "player"
										},
										ArgumentList.Arguments:
										[
											{
												Expression: MemberAccessExpressionSyntax {
													Expression: IdentifierNameSyntax {
														Identifier.ValueText: "Main"
													},
													Name.Identifier.ValueText: "myPlayer"
												}
											}
										]
									},
									Name.Identifier.ValueText: "position"
								},
								Name.Identifier.ValueText: "Y"
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.Value: 16f
							}
						}
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.MultiplyExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "worldSurface"
					},
					Right: LiteralExpressionSyntax {
						Token.Value: 0.3499999940395355
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InSkyHeight));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneOverworldHeight"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InOverworldHeight));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneDirtLayerHeight"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InDirtLayerHeight));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneRockLayerHeight"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InRockLayerHeight));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneUnderworldHeight"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InUnderworldHeight));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneBeach"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InBeach));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneRain"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InRain));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneSandstorm"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InSandstorm));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneOldOneArmy"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InOldOnesArmy));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneGranite"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InGranite));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneMarble"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InMarble));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneHive"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InHive));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneGemCave"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InGemCave));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneLihzhardTemple"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InLihzhardTemple));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneGraveyard"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InGraveyard));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ZoneShimmer"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InAether));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ShoppingZone_Forest"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InShoppingZoneForest));
				break;
			case MemberAccessExpressionSyntax {
				Expression: ElementAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "player"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "myPlayer"
							}
						}
					]
				}
				or MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "LocalPlayer"
				},
				Name.Identifier.ValueText: "ShoppingZone_BelowSurface"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InBelowSurface));
				break;

			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "hardMode"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.Hardmode));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "WorldGen"
				},
				Name.Identifier.ValueText: "shadowOrbSmashed"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.SmashedShadowOrb));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "WorldGen"
				},
				Name.Identifier.ValueText: "crimson"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.CrimsonWorld));
				break;

			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "drunkWorld"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DrunkWorld));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "remixWorld"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.RemixWorld));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "notTheBeesWorld"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.NotTheBeesWorld));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "getGoodWorld"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.ForTheWorthyWorld));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "tenthAnniversaryWorld"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.TenthAnniversaryWorld));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "dontStarveWorld"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DontStarveWorld));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "noTrapsWorld"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.NoTrapsWorld));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalAndExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "remixWorld"
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "getGoodWorld"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.ZenithWorld));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: PrefixUnaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalNotExpression,
					Operand: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "remixWorld"
					}
				},
				Right: PrefixUnaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalNotExpression,
					Operand: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "getGoodWorld"
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.NotZenithWorld));
				break;

			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "xMas"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.Christmas));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "halloween"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.Halloween));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "bloodMoon"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.BloodMoon));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "eclipse"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.Eclipse));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "bloodMoon"
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "eclipse"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "eclipse"
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "bloodMoon"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.EclipseOrBloodMoon));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalAndExpression,
				Left: PrefixUnaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalNotExpression,
					Operand: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "remixWorld"
					}
				},
				Right: PrefixUnaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalNotExpression,
					Operand: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "getGoodWorld"
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.NotEclipseAndNotBloodMoon));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "IsItStorming"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.Thunderstorm));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "BirthdayParty"
				},
				Name.Identifier.ValueText: "PartyIsUp"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.BirthdayParty));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "LanternNight"
				},
				Name.Identifier.ValueText: "LanternsUp"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.LanternNight));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "Main"
				},
				Name.Identifier.ValueText: "IsItAHappyWindyDay"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.HappyWindyDay));
				break;

			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedSlimeKing"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedKingSlime));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedBoss1"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedEyeOfCthulhu));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedBoss2"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedEowOrBoc));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedQueenBee"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedQueenBee));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedBoss3"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedSkeletron));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedDeerclops"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedDeerclops));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedQueenSlime"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedQueenSlime));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalOrExpression,
					Left: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalOrExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.LogicalOrExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "downedBoss1"
							},
							Right: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "NPC"
								},
								Name.Identifier.ValueText: "downedBoss2"
							}
						},
						Right: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "NPC"
							},
							Name.Identifier.ValueText: "downedBoss3"
						}
					},
					Right: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "NPC"
						},
						Name.Identifier.ValueText: "downedQueenBee"
					}
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "hardMode"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedEarlygameBoss));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedMechBossAny"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedMechBossAny));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedMechBoss2"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedTwins));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedMechBoss1"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedDestroyer));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedMechBoss3"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedSkeletronPrime));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalOrExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "downedMechBoss1"
					},
					Right: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "NPC"
						},
						Name.Identifier.ValueText: "downedMechBoss2"
					}
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "NPC"
					},
					Name.Identifier.ValueText: "downedMechBoss3"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedMechBossAll));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedPlantBoss"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedPlantera));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedEmpressOfLight"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedEmpressOfLight));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedFishron"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedDukeFishron));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedGolemBoss"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedGolem));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedHalloweenTree"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedMourningWood));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedHalloweenKing"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedPumpking));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedChristmasTree"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedEverscream));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedChristmasSantank"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedSantaNK1));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedChristmasIceQueen"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedIceQueen));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedAncientCultist"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedCultist));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedMoonlord"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedMoonLord));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedClown"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedClown));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedGoblins"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedGoblinArmy));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedPirates"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedPirates));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedMartians"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedMartians));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedFrost"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedFrostLegion));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedTowerSolar"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedSolarPillar));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedTowerVortex"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedVortexPillar));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedTowerNebula"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedNebulaPillar));
				break;
			case MemberAccessExpressionSyntax {
				Expression: IdentifierNameSyntax {
					Identifier.ValueText: "NPC"
				},
				Name.Identifier.ValueText: "downedTowerStardust"
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedStardustPillar));
				break;

			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "bloodMoon"
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "hardMode"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.BloodMoonOrHardmode));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: PrefixUnaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalNotExpression,
					Operand: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "dayTime"
					}
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "eclipse"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.BloodMoonOrHardmode));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "netMode"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "1"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.Multiplayer));
				break;

			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "0"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhaseFull));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "1"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhaseWaningGibbous));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "2"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhaseThirdQuarter));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "3"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhaseWaningCrescent));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "4"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhaseNew));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "5"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhaseWaxingCrescent));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "6"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhaseFirstQuarter));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "7"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhaseWaxingGibbous));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "0"
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "1"
					}
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.DivideExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "0"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanOrEqualExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "1"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesQuarter0));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "3"
					}
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.DivideExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "1"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanOrEqualExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "3"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesQuarter1));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "4"
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "5"
					}
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.DivideExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "2"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanOrEqualExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "5"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesQuarter2));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "6"
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "7"
					}
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.DivideExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "3"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanOrEqualExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "7"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesQuarter3));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.DivideExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "4"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "0"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "4"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesHalf0));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.DivideExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "4"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "1"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.GreaterThanOrEqualExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "moonPhase"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "4"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesHalf1));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.ModuloExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "0"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.NotEqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.ModuloExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "1"
				}
			}:
			case ParenthesizedExpressionSyntax {
				Expression: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalOrExpression,
					Left: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalOrExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.LogicalOrExpression,
							Left: BinaryExpressionSyntax {
								RawKind: (int)SyntaxKind.EqualsExpression,
								Left: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "moonPhase"
								},
								Right: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token.ValueText: "0"
								}
							},
							Right: BinaryExpressionSyntax {
								RawKind: (int)SyntaxKind.EqualsExpression,
								Left: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "moonPhase"
								},
								Right: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token.ValueText: "2"
								}
							}
						},
						Right: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "moonPhase"
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.ValueText: "4"
							}
						}
					},
					Right: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.EqualsExpression,
						Left: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "moonPhase"
						},
						Right: LiteralExpressionSyntax {
							RawKind: (int)SyntaxKind.NumericLiteralExpression,
							Token.ValueText: "6"
						}
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesEven));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.NotEqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.ModuloExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "0"
				}
			}:
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.ModuloExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "2"
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "1"
				}
			}:
			case ParenthesizedExpressionSyntax {
				Expression: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalOrExpression,
					Left: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalOrExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.LogicalOrExpression,
							Left: BinaryExpressionSyntax {
								RawKind: (int)SyntaxKind.EqualsExpression,
								Left: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "moonPhase"
								},
								Right: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token.ValueText: "1"
								}
							},
							Right: BinaryExpressionSyntax {
								RawKind: (int)SyntaxKind.EqualsExpression,
								Left: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "moonPhase"
								},
								Right: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token.ValueText: "3"
								}
							}
						},
						Right: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "moonPhase"
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.ValueText: "5"
							}
						}
					},
					Right: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.EqualsExpression,
						Left: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "moonPhase"
						},
						Right: LiteralExpressionSyntax {
							RawKind: (int)SyntaxKind.NumericLiteralExpression,
							Token.ValueText: "7"
						}
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesOdd));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalAndExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.GreaterThanOrEqualExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "3"
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LessThanOrEqualExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "5"
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesNearNew));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalOrExpression,
					Left: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalOrExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "moonPhase"
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.ValueText: "0"
							}
						},
						Right: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "moonPhase"
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.ValueText: "1"
							}
						}
					},
					Right: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.EqualsExpression,
						Left: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "moonPhase"
						},
						Right: LiteralExpressionSyntax {
							RawKind: (int)SyntaxKind.NumericLiteralExpression,
							Token.ValueText: "4"
						}
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "5"
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesEvenQuarters));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalOrExpression,
					Left: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalOrExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "moonPhase"
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.ValueText: "2"
							}
						},
						Right: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "moonPhase"
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.ValueText: "3"
							}
						}
					},
					Right: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.EqualsExpression,
						Left: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "moonPhase"
						},
						Right: LiteralExpressionSyntax {
							RawKind: (int)SyntaxKind.NumericLiteralExpression,
							Token.ValueText: "6"
						}
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: "7"
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesOddQuarters));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.ModuloExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.Value: 4
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: 0
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhases04));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.ModuloExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.Value: 4
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: 1
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhases15));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.ModuloExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.Value: 4
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: 2
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhases26));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.ModuloExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.Value: 4
					}
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: 3
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhases37));
				break;

			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalOrExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "NPC"
						},
						Name.Identifier.ValueText: "downedBoss2"
					},
					Right: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "NPC"
						},
						Name.Identifier.ValueText: "downedBoss3"
					}
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "hardMode"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.DownedB2B3HM));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "WorldGen"
						},
						Name.Identifier.ValueText: "SavedOreTiers"
					},
					Name.Identifier.ValueText: "Silver"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.ValueText: "168"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.WorldGenSilver));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: ParenthesizedExpressionSyntax {
					Expression: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalAndExpression,
						Left: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "NPC"
							},
							Name.Identifier.ValueText: "downedBoss2"
						},
						Right: PrefixUnaryExpressionSyntax {
							RawKind: (int)SyntaxKind.LogicalNotExpression,
							Operand: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "dayTime"
							}
						}
					}
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "hardMode"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.NightAfterEvilOrHardmode));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalAndExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.GreaterThanOrEqualExpression,
					Left: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "moonPhase"
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.Value: 4
					}
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "hardMode"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.MoonPhasesHalf1AndHardmode));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Main"
					},
					Name.Identifier.ValueText: "hardMode"
				},
				Right: PrefixUnaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LogicalNotExpression,
					Operand: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "getGoodWorld"
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.HardmodeOrFTW));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalAndExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LessThanExpression,
					Left: CastExpressionSyntax {
						Expression: ParenthesizedExpressionSyntax {
							Expression: BinaryExpressionSyntax {
								RawKind: (int)SyntaxKind.DivideExpression,
								Left: MemberAccessExpressionSyntax {
									Expression: MemberAccessExpressionSyntax {
										Expression: IdentifierNameSyntax {
											Identifier.ValueText: "Main"
										},
										Name.Identifier.ValueText: "screenPosition"
									},
									Name.Identifier.ValueText: "Y"
								},
								Right: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression
								}
							}
						}
					},
					Right: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.AddExpression,
						Left: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "worldSurface"
						},
						Right: LiteralExpressionSyntax {
							RawKind: (int)SyntaxKind.NumericLiteralExpression
						}
					}
				},
				Right: ParenthesizedExpressionSyntax {
					Expression: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalOrExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.LessThanExpression,
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression
							}
						},
						Right: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.GreaterThanExpression,
							Right: BinaryExpressionSyntax {
								RawKind: (int)SyntaxKind.SubtractExpression,
								Left: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "maxTilesX"
								},
								Right: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression
								}
							}
						}
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.InBeach2));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "LocalPlayer"
					},
					Name.Identifier.ValueText: "ConsumedLifeCrystals"
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Player"
					},
					Name.Identifier.ValueText: "LifeCrystalMax"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.AtLeastXHealth));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.EqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "LocalPlayer"
					},
					Name.Identifier.ValueText: "ConsumedManaCrystals"
				},
				Right: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "Player"
					},
					Name.Identifier.ValueText: "ManaCrystalMax"
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.AtLeastXMana));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.GreaterThanOrEqualExpression,
				Left: IdentifierNameSyntax,
				Right: LiteralExpressionSyntax {
					Token.Value: 1000000
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.PlatinumCoin));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: ParenthesizedExpressionSyntax {
					Expression: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalAndExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: BinaryExpressionSyntax {
								RawKind: (int)SyntaxKind.ModuloExpression,
								Left: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "moonPhase"
								},
								Right: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token.Value: 2
								}
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.Value: 0
							}
						},
						Right: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "dayTime"
						}
					}
				},
				Right: ParenthesizedExpressionSyntax {
					Expression: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.LogicalAndExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: BinaryExpressionSyntax {
								RawKind: (int)SyntaxKind.ModuloExpression,
								Left: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "moonPhase"
								},
								Right: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token.Value: 2
								}
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.Value: 1
							}
						},
						Right: PrefixUnaryExpressionSyntax {
							RawKind: (int)SyntaxKind.LogicalNotExpression,
							Operand: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "dayTime"
							}
						}
					}
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.StyleMoon));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.NotEqualsExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: ElementAccessExpressionSyntax {
						Expression: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "player"
						},
						ArgumentList.Arguments:
						[
							{
								Expression: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "myPlayer"
								}
							}
						]
					},
					Name.Identifier.ValueText: "team"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: 0
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.OnTeam));
				break;
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanOrEqualExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.MultiplyExpression,
					Left: BinaryExpressionSyntax {
						RawKind: (int)SyntaxKind.MultiplyExpression,
						Left: BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.ModuloExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "time"
							},
							Right: LiteralExpressionSyntax {
								Token.Value: 60.0
							}
						},
						Right: LiteralExpressionSyntax {
							Token.Value: 60.0
						}
					},
					Right: LiteralExpressionSyntax {
						Token.Value: 6.0
					}
				},
				Right: LiteralExpressionSyntax {
					Token.Value: 10800.0
				}
			}:
				match = new(new(Model.Shop.Condition.ConditionType.Periodically1));
				break;

			case InvocationExpressionSyntax {
				Expression: MemberAccessExpressionSyntax {
					Expression: ElementAccessExpressionSyntax {
						Expression: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "player"
						},
						ArgumentList.Arguments:
						[
							{
								Expression: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "myPlayer"
								}
							}
						]
					},
					Name.Identifier.ValueText: "HasItem"
				},
				ArgumentList.Arguments:
				[
					{
						Expression: LiteralExpressionSyntax {
							RawKind: (int)SyntaxKind.NumericLiteralExpression,
							Token.Value: int itemId
						}
					}
				]
			}: {
				match = new(new(Model.Shop.Condition.ConditionType.PlayerCarriesItem, Data1: itemId));
				break;
			}
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalOrExpression,
				Left: InvocationExpressionSyntax {
					Expression: MemberAccessExpressionSyntax {
						Expression: ElementAccessExpressionSyntax {
							Expression: MemberAccessExpressionSyntax {
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "player"
							},
							ArgumentList.Arguments:
							[
								{
									Expression: MemberAccessExpressionSyntax {
										Expression: IdentifierNameSyntax {
											Identifier.ValueText: "Main"
										},
										Name.Identifier.ValueText: "myPlayer"
									}
								}
							]
						},
						Name.Identifier.ValueText: "HasItem"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.Value: int itemId1
							}
						}
					]
				},
				Right: InvocationExpressionSyntax {
				Expression: MemberAccessExpressionSyntax {
					Expression: ElementAccessExpressionSyntax {
						Expression: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
									Identifier.ValueText: "Main"
								},
								Name.Identifier.ValueText: "player"
							},
							ArgumentList.Arguments:
							[
								{
									Expression: MemberAccessExpressionSyntax {
										Expression: IdentifierNameSyntax {
												Identifier.ValueText: "Main"
										},
										Name.Identifier.ValueText: "myPlayer"
									}
								}
							]
						},
						Name.Identifier.ValueText: "HasItem"
					},
					ArgumentList.Arguments:
					[
						{
							Expression: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.Value: int itemId2
							}
						}
					]
				}
			}: {
				match = new(new(Model.Shop.Condition.ConditionType.PlayerCarriesItem, Data1: itemId1, Data2: itemId2));
				break;
			}
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LogicalAndExpression,
				Left: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LessThanExpression,
					Left: IdentifierNameSyntax,
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.Value: 38
					}
				},
				Right: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.GreaterThanExpression or (int)SyntaxKind.GreaterThanOrEqualExpression,
					Left: IdentifierNameSyntax {
						Identifier.ValueText: ['g', 'o', 'l', 'f', 'e', 'r', 'S', 'c', 'o', 'r', 'e', 'A', 'c', 'c', 'u', 'm', 'u', 'l', 'a', 't', 'e', 'd', ..]
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.Value: int golfScoreAtleast
					}
				}
			}: {
				match = new(new(Model.Shop.Condition.ConditionType.GolfScoreOver, Data1: golfScoreAtleast));
				break;
			}
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.GreaterThanExpression or (int)SyntaxKind.GreaterThanOrEqualExpression,
				Left: IdentifierNameSyntax {
					Identifier.ValueText: ['g', 'o', 'l', 'f', 'e', 'r', 'S', 'c', 'o', 'r', 'e', 'A', 'c', 'c', 'u', 'm', 'u', 'l', 'a', 't', 'e', 'd', .. ]
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: int golfScoreAtleast
				}
			}: {
				match = new(new(Model.Shop.Condition.ConditionType.GolfScoreOver, Data1: golfScoreAtleast));
				break;
			}
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.GreaterThanExpression or (int)SyntaxKind.GreaterThanOrEqualExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: ElementAccessExpressionSyntax {
						Expression: MemberAccessExpressionSyntax {
							Expression: IdentifierNameSyntax {
								Identifier.ValueText: "Main"
							},
							Name.Identifier.ValueText: "player"
						},
						ArgumentList.Arguments:
						[
							{
								Expression: MemberAccessExpressionSyntax {
									Expression: IdentifierNameSyntax {
										Identifier.ValueText: "Main"
									},
									Name.Identifier.ValueText: "myPlayer"
								}
							}
						]
					}
					or MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "Main"
						},
						Name.Identifier.ValueText: "LocalPlayer"
					},
					Name.Identifier.ValueText: "golferScoreAccumulated"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: int golfScoreAtleast
				}
			}: {
				match = new(new(Model.Shop.Condition.ConditionType.GolfScoreOver, Data1: golfScoreAtleast));
				break;
			}
			case InvocationExpressionSyntax {
				Expression: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "NPC"
					},
					Name.Identifier.ValueText: "AnyNPCs"
				},
				ArgumentList.Arguments:
				[
					{
						Expression: LiteralExpressionSyntax {
							RawKind: (int)SyntaxKind.NumericLiteralExpression,
							Token.Value: int npcId
						}
					}
				]
			}: {
				match = new(new(Model.Shop.Condition.ConditionType.NpcIsPresent, Data1: npcId));
				break;
			}
			case BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.GreaterThanOrEqualExpression,
				Left: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.ValueText: "bestiaryProgressReport"
					},
					Name.Identifier.ValueText: "CompletionPercent"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: float percent
				}
			}: {
				match = percent < 1f
					? new(new(Model.Shop.Condition.ConditionType.BestiaryFilledPercent, Data1: (int)(percent * 100f)))
					: new(new(Model.Shop.Condition.ConditionType.BestiaryFull));
				break;
			}
		}

		if (shouldBeInverted) {
			match?.TryInvert();
		}

		return match != default && Enum.IsDefined(typeof(Model.Shop.Condition.ConditionType), match.Condition.Type);
	}
}
