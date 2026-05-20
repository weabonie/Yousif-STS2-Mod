using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using YousifMod.YousifModCode.Character;
using YousifMod.YousifModCode.Extensions;

namespace YousifMod.YousifModCode.Cards;

[Pool(typeof(YousifCharacterCardPool))]
public class GluttonCard() :
    CustomCardModel(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int BaseDamage = 3;
    private const string IncreaseKey = "Increase";

    private int _currentDamage = BaseDamage;
    private int _increasedDamage;

    [SavedProperty]
    private int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    private int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move),
        new IntVar(IncreaseKey, 5M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    ];

    public override string PortraitPath => "glutton_card.png".CardImagePath();
    public override string CustomPortraitPath => "glutton_card.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        bool shouldTriggerFatal = play.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        AttackCommand attack = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (!shouldTriggerFatal || !attack.Results.Any(r => r.WasTargetKilled))
            return;

        int extraDamage = DynamicVars[IncreaseKey].IntValue;
        BuffFromPlay(extraDamage);

        if (DeckVersion is GluttonCard deckVersion)
            deckVersion.BuffFromPlay(extraDamage);
    }

    protected override void OnUpgrade() => DynamicVars[IncreaseKey].UpgradeValueBy(2M);

    protected override void AfterDowngraded() => UpdateDamage();

    private void BuffFromPlay(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage() => CurrentDamage = BaseDamage + IncreasedDamage;
}
