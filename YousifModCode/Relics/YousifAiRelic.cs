using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using YousifMod.YousifModCode.Character;

namespace YousifMod.YousifModCode.Relics;

[Pool(typeof(YousifCharacterRelicPool))]
public class YousifAiRelic : YousifModRelic
{
    private const int TurnsThreshold = 4;
    private const int MaxCardsToAutoplay = 13;
    private const string TurnsKey = "Turns";
    private const string ImageFileName = "yousifai_relic.png";
    private const string OutlineImageFileName = "yousifai_relic_outline.png";

    private bool _isActivating;
    private int _turnsSeen;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    protected override string IconFileName => ImageFileName;
    protected override string OutlineIconFileName => OutlineImageFileName;

    public override int DisplayAmount => IsActivating ? DynamicVars[TurnsKey].IntValue : TurnsSeen;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new DynamicVar(TurnsKey, TurnsThreshold)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int TurnsSeen
    {
        get => _turnsSeen;
        set
        {
            AssertMutable();
            _turnsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task BeforePlayPhaseStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;

        var combatState = player.Creature.CombatState;
        if (combatState == null) return;

        var threshold = DynamicVars[TurnsKey].IntValue;
        if (TurnsSeen < threshold) return;

        TurnsSeen = 0;
        Status = RelicStatus.Normal;
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        await AutoPlayCards(choiceContext, player, combatState);
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Creature.Side)
        {
            IncrementTurnCounter();
        }

        return Task.CompletedTask;
    }

    private void IncrementTurnCounter()
    {
        var threshold = DynamicVars[TurnsKey].IntValue;
        if (TurnsSeen < threshold)
        {
            TurnsSeen += 1;
        }

        Status = TurnsSeen >= threshold - 1 ? RelicStatus.Active : RelicStatus.Normal;
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
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

            // var target = GetTarget(card, combatState);
            await card.SpendResources();
            await CardCmd.AutoPlay(choiceContext, card, null);
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
