using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using YousifMod.YousifModCode.Character;

namespace YousifMod.YousifModCode.Relics;

[Pool(typeof(YousifCharacterRelicPool))]
public class YousifAiRelic : YousifModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(6M)
    ];

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner.Creature.IsDead)
            return;

        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}
