using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using YousifMod.YousifModCode.Character;
using YousifMod.YousifModCode.Extensions;

namespace YousifMod.YousifModCode.Cards;

[Pool(typeof(YousifCharacterCardPool))]
public class StrikeYousif() :
    YousifModCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6M, ValueProp.Move)
    ];
    
    public override string PortraitPath => "strike_yousif.png".CardImagePath();
    public override string CustomPortraitPath => "strike_yousif.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3M);
}
