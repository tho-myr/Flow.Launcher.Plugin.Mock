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
    
    private string Query { get; set; }

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
    
    public Result ToResult(string query, PluginInitContext context, bool contextMenuItem = false, bool mockedText = true) {
        Query = query ?? Query;
        var shownText = mockedText ? MockingCaseConverter.Convert(Query) : Query;
        return new Result {
            Title = contextMenuItem ? mockedText ? "copy with mocked query" : "copy with unmodified query" : $"copy {_memeName} image",
            SubTitle = shownText,
            IcoPath = _memeIconPath,
            Score = contextMenuItem ? 0 : _score,
            ContextData = this,
            Action = _ => {
                var image = Generate(shownText);
                Clipboard.SetImage(image);
                context.API.ShowMsg(
                    $"copied {_memeName} to clipboard",
                    shownText,
                    _memeIconPath
                );
                return true;
            }
        };
    }
    
    private BitmapImage Generate(string text) {
        return ImageGenerator.CreateImage(_memePath, _memeOutputPath, text);
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

        const int scoreMultiplier = 50;
        var score = memeFiles.Count() * scoreMultiplier;
        for (var i = 0; i < memeFiles.Count(); i++) {
            memes.Add(new Meme(memeFiles.ElementAt(i), score, context));
            score -= scoreMultiplier;
        }

        return memes;
    }
}