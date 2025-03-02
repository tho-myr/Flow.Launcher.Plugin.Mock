using System.IO;

namespace Flow.Launcher.Plugin.Mock.SrcFiles;

public static class PluginFile
{
    public const string IconPath = @"Images\icon.png";
    public const string CopyTextIconPath = @"Images\copy-text-icon.png";

    public static string FullPath(string directory, PluginInitContext context)
    {
        return Path.Combine(context.CurrentPluginMetadata.PluginDirectory, directory);
    }
}