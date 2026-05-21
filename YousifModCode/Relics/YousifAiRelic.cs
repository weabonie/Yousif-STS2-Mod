using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using YousifMod.YousifModCode.Character;
using ICardSelector = MegaCrit.Sts2.Core.TestSupport.ICardSelector;

namespace YousifMod.YousifModCode.Relics;

[Pool(typeof(YousifCharacterRelicPool))]
public class YousifAiRelic : YousifModRelic
{
  public override RelicRarity Rarity => RelicRarity.Starter;

  protected override IEnumerable<DynamicVar> CanonicalVars => [new("EnergyVar", 1)];

  public override async Task BeforePlayPhaseStartLate(PlayerChoiceContext choiceContext, Player player)
  {
    CombatState combatState;
    if (player != Owner)
    {
      combatState = (CombatState) null;
    }
    combatState = player.Creature.CombatState;
    if (combatState.RoundNumber % 4 == 0)
    {
      Flash();
      bool flag;
      using (CardSelectCmd.PushSelector((ICardSelector)new VakuuCardSelector()))
      {
        int cardsPlayed = 0;
        while (cardsPlayed < 13 && !CombatManager.Instance.IsOverOrEnding &&
               !CombatManager.Instance.IsPlayerReadyToEndTurn(player))
        {
          CardModel card = PileType.Hand.GetPile(Owner).Cards
            .FirstOrDefault<CardModel>((Func<CardModel, bool>)(c => c.CanPlay()));
          if (card != null)
          {
            Creature target = GetTarget(card, combatState);
            (int, int) valueTuple = await card.SpendResources();
            await CardCmd.AutoPlay(choiceContext, card, target, skipXCapture: true);
            ++cardsPlayed;
            card = (CardModel)null;
            target = (Creature)null;
          }
          else
            break;
        }
        flag = cardsPlayed >= 13;
        if (cardsPlayed == 0)
        {
          combatState = (CombatState) null;
          return;
        }
      }
    }
  }

  private Creature? GetTarget(CardModel card, CombatState combatState)
  {
    Rng combatTargets = Owner.RunState.Rng.CombatTargets;
    Creature target;
    switch (card.TargetType)
    {
      case TargetType.AnyEnemy:
        target = combatState.HittableEnemies.FirstOrDefault<Creature>();
        break;
      case TargetType.AnyPlayer:
        target = Owner.Creature;
        break;
      case TargetType.AnyAlly:
        target = combatTargets.NextItem<Creature>(combatState.Allies.Where<Creature>((Func<Creature, bool>) (c => c != null && c.IsAlive && c.IsPlayer && c != Owner.Creature)));
        break;
      default:
        target = (Creature)null;
        break;
    }
    return target;
  }
}