using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using YousifMod.YousifModCode.Character;
using YousifMod.YousifModCode.Extensions;
using YousifMod.YousifModCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YousifMod.YousifModCode.Cards;

[Pool(typeof(YousifCharacterCardPool))]
public class StockMarketCard() :
    CustomCardModel(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("GoldPerTurn", 3M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StockMarketPower>()
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StockMarketPower>(Owner.Creature, DynamicVars["GoldPerTurn"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["GoldPerTurn"].UpgradeValueBy(2M);
}
