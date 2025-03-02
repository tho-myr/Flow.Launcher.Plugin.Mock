using System.IO;

namespace Flow.Launcher.Plugin.Mock.SrcFiles;

public static class PluginDir
{
    public const string MemesDir = @"Images\Memes";
    public const string OutputDir = @"Images\Output";

    public static string FullPath(string directory, PluginInitContext context)
    {
        return Path.Combine(context.CurrentPluginMetadata.PluginDirectory, directory);
    }
}