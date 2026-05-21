using BaseLib.Abstracts;
using BaseLib.Extensions;
using YousifMod.YousifModCode.Extensions;

namespace YousifMod.YousifModCode.Relics;

/// <summary>
/// This is the base class for your mod's relics, which is set up to load the relic's images from your mod's resources.
/// When creating a relic, right click the Relics folder and create a new file with the Custom Relic template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
public abstract class YousifModRelic : CustomRelicModel
{
    protected virtual string IconFileName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    protected virtual string OutlineIconFileName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png";

    public override string PackedIconPath => IconFileName.RelicImagePath();

    protected override string PackedIconOutlinePath => OutlineIconFileName.RelicImagePath();

    protected override string BigIconPath => IconFileName.BigRelicImagePath();
}
