using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using YousifMod.YousifModCode.Extensions;

namespace YousifMod.YousifModCode.Powers;

/// <summary>
/// Power applied by StockMarketCard. Accumulates gold every turn and obtains it on victory.
/// </summary>
public class StockMarketPower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int AccumulatedGold { get; set; } = 0;

    public override LocString Description
    {
        get
        {
            var desc = base.Description;
            desc.Add("AccumulatedGold", (decimal)AccumulatedGold);
            return desc;
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player.Creature != Owner)
            return Task.CompletedTask;

        AccumulatedGold += Amount;
        return Task.CompletedTask;
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner.IsPlayer && Owner.Player != null && AccumulatedGold > 0)
        {
            await PlayerCmd.GainGold(AccumulatedGold, Owner.Player, false);
        }
    }
}
