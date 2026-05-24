using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using YousifMod.YousifModCode.Character;
using YousifMod.YousifModCode.Extensions;
using YousifMod.YousifModCode.Powers;

namespace YousifMod.YousifModCode.Cards;

[Pool(typeof(YousifCharacterCardPool))]
public class StockMarketCard() : YousifModCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private const string GoldPerTurnKey = "GoldPerTurn";
    private const int BaseGoldPerTurn = 3;

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(GoldPerTurnKey, BaseGoldPerTurn)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StockMarketPower>(Owner.Creature, DynamicVars[GoldPerTurnKey].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars[GoldPerTurnKey].UpgradeValueBy(2M);
}
