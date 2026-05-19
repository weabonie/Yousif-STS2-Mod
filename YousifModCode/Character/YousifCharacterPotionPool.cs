using BaseLib.Abstracts;
using Godot;
using YousifMod.YousifModCode.Extensions;

namespace YousifMod.YousifModCode.Character;

public abstract class YousifCharacterPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => YousifCharacter.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}