using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YousifMod.YousifModCode.Character;

namespace YousifMod.YousifModCode.Cards;

[Pool(typeof(YousifCharacterCardPool))]
public class CommonAttackCard() : PlaceholderAttackCard(1, CardRarity.Common, 7M, 3M);

[Pool(typeof(YousifCharacterCardPool))]
public class CommonDefendCard() : PlaceholderDefendCard(1, CardRarity.Common, 6M, 3M);

[Pool(typeof(YousifCharacterCardPool))]
public class CommonDebuffCard() : PlaceholderVulnerableCard(2, CardRarity.Common, 6M, 2M, 2M, 1M);

[Pool(typeof(YousifCharacterCardPool))]
public class UncommonAttackCard() : PlaceholderAttackCard(2, CardRarity.Uncommon, 12M, 4M);

[Pool(typeof(YousifCharacterCardPool))]
public class UncommonDefendCard() : PlaceholderDefendCard(1, CardRarity.Uncommon, 8M, 3M);

[Pool(typeof(YousifCharacterCardPool))]
public class UncommonDebuffCard() : PlaceholderVulnerableCard(2, CardRarity.Uncommon, 9M, 3M, 3M, 1M);

[Pool(typeof(YousifCharacterCardPool))]
public class RareAttackCard() : PlaceholderAttackCard(2, CardRarity.Rare, 18M, 5M);

[Pool(typeof(YousifCharacterCardPool))]
public class RareDefendCard() : PlaceholderDefendCard(2, CardRarity.Rare, 16M, 5M);

[Pool(typeof(YousifCharacterCardPool))]
public class RareDebuffCard() : PlaceholderVulnerableCard(3, CardRarity.Rare, 14M, 4M, 4M, 1M);

public abstract class PlaceholderAttackCard(
    int cost,
    CardRarity rarity,
    decimal damage,
    decimal damageUpgrade) : YousifModCard(cost, CardType.Attack, rarity, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(damage, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(damageUpgrade);
}

public abstract class PlaceholderDefendCard(
    int cost,
    CardRarity rarity,
    decimal block,
    decimal blockUpgrade) : YousifModCard(cost, CardType.Skill, rarity, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(block, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(blockUpgrade);
}

public abstract class PlaceholderVulnerableCard(
    int cost,
    CardRarity rarity,
    decimal damage,
    decimal vulnerable,
    decimal damageUpgrade,
    decimal vulnerableUpgrade) : YousifModCard(cost, CardType.Attack, rarity, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(damage, ValueProp.Move),
        new PowerVar<VulnerablePower>(vulnerable)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        AttackCommand attack = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);

        await PowerCmd.Apply<VulnerablePower>(play.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(damageUpgrade);
        DynamicVars.Vulnerable.UpgradeValueBy(vulnerableUpgrade);
    }
}
