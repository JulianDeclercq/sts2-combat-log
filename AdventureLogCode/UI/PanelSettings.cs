using System.Text.Json;
using Godot;

namespace AdventureLog.AdventureLogCode.UI;

internal static class PanelSettings
{
    private const string Dir = "user://AdventureLog";
    private const string Path = "user://AdventureLog/settings.json";

    public record Data(float OffsetLeft, float OffsetRight, float OffsetTop, float OffsetBottom);

    public static Data? Load()
    {
        if (!Godot.FileAccess.FileExists(Path)) return null;
        try
        {
            using var f = Godot.FileAccess.Open(Path, Godot.FileAccess.ModeFlags.Read);
            return f is null ? null : JsonSerializer.Deserialize<Data>(f.GetAsText());
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[AdventureLog] Load settings failed: {e.Message}");
            return null;
        }
    }

    public static void Save(Data data)
    {
        try
        {
            if (!DirAccess.DirExistsAbsolute(Dir))
                DirAccess.MakeDirRecursiveAbsolute(Dir);
            // Write to a tmp file then rename so a crash mid-write can't truncate settings.
            var realPath = ProjectSettings.GlobalizePath(Path);
            var tmpPath = realPath + ".tmp";
            System.IO.File.WriteAllText(tmpPath, JsonSerializer.Serialize(data));
            System.IO.File.Move(tmpPath, realPath, overwrite: true);
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[AdventureLog] Save settings failed: {e.Message}");
        }
    }
}
