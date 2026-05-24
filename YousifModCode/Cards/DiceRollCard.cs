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
public class DiceRollCard() : YousifModCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override string PortraitPath => "dice_roll_card.png".CardImagePath();
    public override string CustomPortraitPath => "dice_roll_card.png".BigCardImagePath();
    private const int MinDamage = 7;
    private const int MaxDamage = 15;

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(MaxDamage, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        var damage = play.IsAutoPlay ? MaxDamage : Owner.RunState.Rng.CombatTargets.NextInt(MinDamage, MaxDamage);

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
