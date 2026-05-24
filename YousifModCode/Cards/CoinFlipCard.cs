using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using YousifMod.YousifModCode.Character;
using YousifMod.YousifModCode.Extensions;

namespace YousifMod.YousifModCode.Cards;

[Pool(typeof(YousifCharacterCardPool))]
public class CoinFlipCard() : YousifModCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string PortraitPath => "coin_flip_card.png".CardImagePath();
    public override string CustomPortraitPath => "coin_flip_card.png".BigCardImagePath();
    private const string EnergyMultiplierKey = "EnergyMultiplier";
    private const string MinCostKey = "MinCost";
    private const string MaxCostKey = "MaxCost";

    private const int EnergyMultiplier = 2;
    private const int BaseMinCost = 0;
    private const int BaseMaxCost = 2;

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(EnergyMultiplierKey, EnergyMultiplier),
        new DynamicVar(MinCostKey, BaseMinCost),
        new DynamicVar(MaxCostKey, BaseMaxCost)
    ];

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this || Pile?.Type != PileType.Hand)
        {
            return Task.CompletedTask;
        }

        EnergyCost.SetThisCombat(NextEnergyCost());
        NCard.FindOnTable(this)?.PlayRandomizeCostAnim();
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PlayerCmd.GainEnergy(play.Resources.EnergyValue * DynamicVars[EnergyMultiplierKey].IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars[MaxCostKey].UpgradeValueBy(1M);

    private int NextEnergyCost()
    {
        var minCost = DynamicVars[MinCostKey].IntValue;
        var maxCost = DynamicVars[MaxCostKey].IntValue;
        return Owner.RunState.Rng.CombatEnergyCosts.NextInt(minCost, maxCost + 1);
    }
}
