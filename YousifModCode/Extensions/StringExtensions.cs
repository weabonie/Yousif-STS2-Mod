using Godot;

namespace YousifMod.YousifModCode.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    private static string ResourcePath(params string[] parts)
    {
        return string.Join("/", parts.Select(part => part.Replace('\\', '/').Trim('/')));
    }

    public static string ImagePath(this string path)
    {
        return ResourcePath(MainFile.ResPath, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "card_portraits", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find card image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "card_portraits", "strike_card.png");
    }

    public static string BigCardImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "card_portraits", "big", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find big card image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "card_portraits", "big", "strike_card.png");
    }

    public static string PowerImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "powers", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find power image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "powers", "power.png");
    }

    public static string BigPowerImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "powers", "big", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find big power image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "powers", "big", "power.png");
    }

    public static string RelicImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "relics", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find relic image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "relics", "relic.png");
    }

    public static string BigRelicImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "relics", "big", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find big relic image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "relics", "big", "relic.png");
    }

    public static string CharacterUiPath(this string path)
    {
        return ResourcePath(MainFile.ResPath, "images", "charui", path);
    }

    public static string CharacterScenePath(this string path)
    {
        return ResourcePath(MainFile.ResPath, "scenes", path);
    }
}

