using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Random;
using YousifMod.YousifModCode.Character;
using YousifMod.YousifModCode.Extensions;

namespace YousifMod.YousifModCode.Cards;

[Pool(typeof(YousifCharacterCardPool))]
public class Chopped() : YousifModCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override string PortraitPath => "chopped_card.png".CardImagePath();
    public override string CustomPortraitPath => "chopped_card.png".BigCardImagePath();

    private bool _isReplaying;

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3M, ValueProp.Move),
        new IntVar("DamageMax", 4M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        int minDmg = (int)DynamicVars.Damage.BaseValue;
        int maxDmg = (int)DynamicVars["DamageMax"].BaseValue;

        for (int i = 0; i < 2; i++)
        {
            int baseDmg = Rng.Chaotic.NextInt(minDmg, maxDmg + 1);
            await DamageCmd.Attack(baseDmg)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        if (play.IsAutoPlay && !_isReplaying)
        {
            _isReplaying = true;
            try
            {
                await CardCmd.AutoPlay(choiceContext, this, play.Target, AutoPlayType.Default, false, false);
            }
            finally
            {
                _isReplaying = false;
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["DamageMax"].UpgradeValueBy(2M);
    }
}
