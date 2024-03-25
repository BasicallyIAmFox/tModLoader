using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace tModLoader.SourceGenerator.NPCShopDatabase;

public sealed class VanillaSetupShopMethodWalker : CSharpSyntaxWalker
{
	private readonly ImmutableArray<Model.Shop>.Builder shopBuilder = ImmutableArray.CreateBuilder<Model.Shop>();
	private Model.Shop.Builder currentShop;
	private Stack<Model.Shop.Condition> currentConditions;

	public ImmutableArray<Model.Shop> Shops => shopBuilder.ToImmutableArray();

	public void VisitShopsSwitchStatement(SwitchStatementSyntax node)
	{
		foreach (var section in node.Sections) {
			if (section.Labels is not [
				CaseSwitchLabelSyntax {
					Value: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.Value: int shopId
					}
				}
			])
				continue;

			// Travelling Merchant and Bartender shops are NOT sane
			if (shopId is 19 or 21)
				continue;

			currentShop = new Model.Shop.Builder(shopId);
			currentConditions = [];

			foreach (var sectionStatement in section.Statements) {
				sectionStatement.Accept(this);
			}

			shopBuilder.Add(currentShop.ToShop());
		}
	}

	public override void VisitForStatement(ForStatementSyntax node)
	{
		switch (node) {
			case {
				Statement: var statement,
				Declaration: VariableDeclarationSyntax {
					Variables:
					[
						{
							Identifier.ValueText: var id,
							Initializer: EqualsValueClauseSyntax {
								Value: LiteralExpressionSyntax {
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token.ValueText: var constantValue1
								}
							}
						}
					]
				},
				Condition: BinaryExpressionSyntax {
					RawKind: (int)SyntaxKind.LessThanExpression or (int)SyntaxKind.LessThanOrEqualExpression,
					Left: IdentifierNameSyntax {
						Identifier.ValueText: var id2
					},
					Right: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: var constantValue2
					}
				} conditionExpressionSyntax,
				Incrementors:
				[
					PostfixUnaryExpressionSyntax {
						RawKind: (int)SyntaxKind.PostIncrementExpression,
						Operand: IdentifierNameSyntax {
							Identifier.ValueText: var id3
						}
					}
				]
			}:
				Debug.Assert(statement is BlockSyntax);

				if (id != id2 || id2 != id3)
					break;

				// This loop iterates through player inventory
				if (constantValue1 is "0" && constantValue2 is "54" or "58") {
					foreach (var blockStatementSyntax in statement.DescendantNodes()) {
						if (blockStatementSyntax is not IfStatementSyntax ifStatementSyntax)
							continue;

						if (ifStatementSyntax.Condition is not BinaryExpressionSyntax {
							RawKind: (int)SyntaxKind.EqualsExpression,
							Left: MemberAccessExpressionSyntax {
								Expression: ElementAccessExpressionSyntax {
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
										}
										or MemberAccessExpressionSyntax {
											Expression: IdentifierNameSyntax {
												Identifier.ValueText: "Main"
											},
											Name.Identifier.ValueText: "LocalPlayer"
										},
										Name.Identifier.ValueText: "inventory"
									},
									ArgumentList.Arguments:
									[
										{
											Expression: IdentifierNameSyntax {
												Identifier.ValueText: var id4
											}
										}
									]
								},
								Name.Identifier.ValueText: "type"
							},
							Right: LiteralExpressionSyntax {
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token.ValueText: var itemIdLiteral
							}
						})
							continue;

						if (id != id4)
							continue;

						if (!int.TryParse(itemIdLiteral, out int itemId))
							continue;

						currentConditions.Push(new(Model.Shop.Condition.ConditionType.PlayerCarriesItem, Data1: itemId));

						ifStatementSyntax.Statement.Accept(this);

						currentConditions.Pop();
						break;
					}
				}
				else if (int.TryParse(constantValue1, out int fromItemId) && int.TryParse(constantValue2, out int toItemId)) {
					bool useLessThanOrEqual = conditionExpressionSyntax.IsKind(SyntaxKind.LessThanOrEqualExpression);

					if (useLessThanOrEqual) {
						for (int i = fromItemId; i <= toItemId; i++)
							currentShop.AddItem(new Model.Shop.Item(i, currentConditions.ToImmutableArray()));
					}
					else {
						for (int i = fromItemId; i < toItemId; i++)
							currentShop.AddItem(new Model.Shop.Item(i, currentConditions.ToImmutableArray()));
					}
				}

				break;
		}
	}

	public override void VisitIfStatement(IfStatementSyntax node)
	{
		if (!ConditionMatcher.TryTransform(node.Condition, out var matches))
			return;

		foreach (var match in matches)
			currentConditions.Push(match.Condition);

		node.Statement.Accept(this);

		for (int i = 0; i < matches.Count; i++)
			currentConditions.Pop();

		if (node.Else != null) {
			int count = 0;

			foreach (var match in matches) {
				if (match.TryInvert()) {
					currentConditions.Push(match.Condition);
					count++;
				}
			}

			node.Else.Statement.Accept(this);

			for (int i = 0; i < count; i++)
				currentConditions.Pop();
		}
	}

	public override void VisitSwitchStatement(SwitchStatementSyntax node)
	{
		var evalExpression = node.Expression;

		foreach (var section in node.Sections) {
			foreach (var label in section.Labels) {
				if (label is CaseSwitchLabelSyntax {
					Value: LiteralExpressionSyntax {
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token.ValueText: var constantIntStr
					} constantValueExpr
				}) {
					if (!int.TryParse(constantIntStr, out _))
						continue;

					var expr = SyntaxFactory.BinaryExpression(
						SyntaxKind.EqualsExpression,
						evalExpression,
						constantValueExpr
						);

					if (!ConditionMatcher.TryTransform(expr, out var matches))
						continue;

					foreach (var match in matches)
						currentConditions.Push(match.Condition);

					section.Accept(this);

					for (int i = 0; i < matches.Count; i++)
						currentConditions.Pop();
				}
			}
		}
	}

	public override void VisitWhileStatement(WhileStatementSyntax node)
	{
		if (node.Condition is BinaryExpressionSyntax {
			RawKind: (int)SyntaxKind.LogicalAndExpression,
			Left: BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanOrEqualExpression,
				Left: IdentifierNameSyntax {
					Identifier.ValueText: var varId
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: int upperLimitIdInclusive
				}
			},
			Right: BinaryExpressionSyntax {
				RawKind: (int)SyntaxKind.LessThanExpression,
				Left: IdentifierNameSyntax {
					Identifier.ValueText: "num"
				},
				Right: LiteralExpressionSyntax {
					RawKind: (int)SyntaxKind.NumericLiteralExpression,
					Token.Value: 39
				}
			}
		}) {
			var varDeclNode = node.Parent.DescendantNodes()
				.OfType<LocalDeclarationStatementSyntax>()
				.Where(x => x.Declaration.Variables is [ var variable ] && variable.Identifier.ValueText.Equals(varId))
				.First();

			if (varDeclNode is {
				Declaration.Variables:
				[
					{
						Initializer: EqualsValueClauseSyntax {
							Value: LiteralExpressionSyntax {
								Token.Value: int lowerLimitIdInclusive
							}
						}
					}
				]
			}) {
				int i = lowerLimitIdInclusive;
				while (i <= upperLimitIdInclusive) {
					currentShop.AddItem(new Model.Shop.Item(i, currentConditions.ToImmutableArray()));
					i++;
				}
			}
		}
	}

	public override void VisitInvocationExpression(InvocationExpressionSyntax node)
	{
		if (node is {
				Expression: MemberAccessExpressionSyntax {
					RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
					Expression: ElementAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.ValueText: "array"
						},
						ArgumentList.Arguments:
						[
							{
								Expression: IdentifierNameSyntax {
									Identifier.ValueText: "num"
								}
								or PostfixUnaryExpressionSyntax {
									RawKind: (int)SyntaxKind.PostIncrementExpression,
									Operand: IdentifierNameSyntax {
										Identifier.ValueText: "num"
									}
								}
							}
						]
					},
					Name.Identifier.ValueText: "SetDefaults"
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
			}) {
			currentShop.AddItem(new Model.Shop.Item(itemId, currentConditions.ToImmutableArray()));
		}
	}
}
