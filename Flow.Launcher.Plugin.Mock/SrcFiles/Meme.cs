using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Flow.Launcher.Plugin.Mock.SrcFiles;

public class Meme {
    
    private readonly string _memeName;
    private readonly string _memePath;
    private readonly string _memeOutputPath;
    private readonly string _memeIconPath;
    private readonly int _score;

    private Meme(string memePath, int score, PluginInitContext context) {
        _memeName = Path.GetFileNameWithoutExtension(memePath).Replace('-', ' ');
        _memePath = memePath;
        _memeOutputPath = Path.Combine(
            context.CurrentPluginMetadata.PluginDirectory, PluginDir.OutputDir,
            Path.GetFileNameWithoutExtension(memePath) + "-output" + Path.GetExtension(memePath)
        );
        _memeIconPath = Path.Combine(
            context.CurrentPluginMetadata.PluginDirectory, PluginDir.MemesDir,
            Path.GetFileNameWithoutExtension(memePath) + "-icon" + Path.GetExtension(memePath)
        );
        _score = score;
    }
    
    public Result ToResult(string text, PluginInitContext context) {
        return new Result {
            Title = $"copy {_memeName} image",
            SubTitle = text,
            IcoPath = _memeIconPath,
            Score = _score,
            Action = _ => {
                var image = Generate(text);
                Clipboard.SetImage(image);
                context.API.ShowMsg(
                    $"copied {_memeName} to clipboard",
                    text,
                    _memeIconPath
                );
                return true;
            }
        };
    }
    
    private BitmapImage Generate(string text) {
        return ImageGenerator.CreateMockedImage(_memePath, _memeOutputPath, MockingCaseConverter.Convert(text));
    }
    
    public static List<Meme> LoadAllFromMemesFolder(PluginInitContext context) {
        var memesDir = PluginDir.FullPath(PluginDir.MemesDir, context);
        var iconPath = PluginFile.FullPath(PluginFile.IconPath, context);
        
        var memes = new List<Meme>();
        if (!Directory.Exists(memesDir)) {
            Directory.CreateDirectory(memesDir);
            context.API.ShowMsg(
                "mock plugin meme folder not found 😖 initialization for image generation failed",
                "image generation will not be available",
                iconPath
            );
        }
    
        var memeFiles = Directory.GetFiles(memesDir, "*.png")
            .Where(file => !file.EndsWith("-icon.png"))
            .OrderBy(file => {
                if (file.Contains("mocking-spongebob")) return 0;
                if (file.Contains("mocking-patrick")) return 1;
                if (file.Contains("cat-and-woman")) return 2;
                return 3;
            });

        int score = memeFiles.Count() * 8;
        for (int i = 0; i < memeFiles.Count(); i++) {
            memes.Add(new Meme(memeFiles.ElementAt(i), score, context));
            score -= 8;
        }

        return memes;
    }
}