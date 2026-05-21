using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using YousifMod.YousifModCode.Character;

namespace YousifMod.YousifModCode.Relics;

[Pool(typeof(YousifCharacterRelicPool))]
public class YousifAiRelic : YousifModRelic
{
    private const int TurnInterval = 4;
    private const int MaxCardsToAutoplay = 13;
    private const string ImageFileName = "yousifai_relic.png";
    private const string OutlineImageFileName = "yousifai_relic_outline.png";

    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override string IconFileName => ImageFileName;
    protected override string OutlineIconFileName => OutlineImageFileName;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override async Task BeforePlayPhaseStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;

        var combatState = player.Creature.CombatState;
        if (combatState == null || combatState.RoundNumber % TurnInterval != 0) return;

        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        await AutoPlayCards(choiceContext, player, combatState);
    }

    private async Task AutoPlayCards(PlayerChoiceContext choiceContext, Player player, CombatState combatState)
    {
        for (var cardsPlayed = 0; cardsPlayed < MaxCardsToAutoplay; ++cardsPlayed)
        {
            if (CombatManager.Instance.IsOverOrEnding || CombatManager.Instance.IsPlayerReadyToEndTurn(player))
            {
                return;
            }

            var card = PileType.Hand.GetPile(Owner).Cards.FirstOrDefault(card => card.CanPlay());
            if (card == null) return;

            var target = GetTarget(card, combatState);
            await card.SpendResources();
            await CardCmd.AutoPlay(choiceContext, card, target, skipXCapture: true);
        }
    }

    private Creature? GetTarget(CardModel card, CombatState combatState)
    {
        return card.TargetType switch
        {
            TargetType.AnyEnemy => combatState.HittableEnemies.FirstOrDefault(),
            TargetType.AnyPlayer => Owner.Creature,
            TargetType.AnyAlly => GetRandomAlly(combatState),
            _ => null
        };
    }

    private Creature? GetRandomAlly(CombatState combatState)
    {
        var allies = combatState.Allies
            .Where(creature => creature != null && creature.IsAlive && creature.IsPlayer && creature != Owner.Creature)
            .ToList();

        return allies.Count == 0 ? null : Owner.RunState.Rng.CombatTargets.NextItem(allies);
    }
}
