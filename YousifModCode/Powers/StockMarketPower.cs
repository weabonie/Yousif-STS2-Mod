using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Threading.Tasks;
using YousifMod.YousifModCode.Extensions;

namespace YousifMod.YousifModCode.Powers;

/// <summary>
/// Power applied by StockMarketCard.
/// </summary>
public class StockMarketPower : CustomPowerModel
{
    public override string CustomPackedIconPath => "stock_market.png".PowerImagePath();
    public override string CustomBigIconPath => "stock_market.png".BigPowerImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private decimal _accumulatedGold;

    [SavedProperty]
    public decimal AccumulatedGold
    {
        get => _accumulatedGold;
        set
        {
            AssertMutable();
            _accumulatedGold = value;
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side)
            return;

        Flash();
        AccumulatedGold += Amount;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (AccumulatedGold > 0 && Owner.Player != null)
        {
            Flash();
            room.AddExtraReward(Owner.Player, new GoldReward((int)AccumulatedGold, Owner.Player));
            AccumulatedGold = 0;
        }
    }
}
