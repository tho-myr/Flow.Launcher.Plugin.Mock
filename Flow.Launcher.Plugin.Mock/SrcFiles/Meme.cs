using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Flow.Launcher.Plugin.Mock.Settings.Models;

namespace Flow.Launcher.Plugin.Mock.SrcFiles;

public class Meme {
    
    private static readonly string[] ImageExtensions = { "*.jpg", "*.jpeg", "*.png" };

    private readonly PluginInitContext _context;
    private readonly string _memeName;
    private readonly string _memePath;

    private Meme(string memePath, PluginInitContext context) {
        _memeName = Path.GetFileNameWithoutExtension(memePath).Replace(' ', '_');
        _memePath = memePath;
        _context = context;
    }
    
    public Result ToResult(Query query, string imageText, bool mockedText = true) {
        var possibleMockedImageText = mockedText ? MockingCaseConverter.Convert(imageText) : imageText;
        var subTitle = "";
        if (query.SecondSearch.Equals(_memeName)) {
            if (string.IsNullOrEmpty(imageText)) {
                subTitle = "type text to put on image or select result to copy image only ^-^";
            }
            else {
                subTitle = mockedText ? $"mocked: '{possibleMockedImageText}'" : $"normal: '{imageText}'";
            }
        } else {
            subTitle = "select result to use image for meme generation :3";
        }
        
        return new Result {
            Title = _memeName,
            SubTitle = subTitle,
            IcoPath = _memePath,
            ContextData = this,
            AutoCompleteText = query.ActionKeyword + " " + query.FirstSearch + " " + _memeName + " ",
            Action = _ => {
                if (!query.SecondSearch.Equals(_memeName)) {
                    _context.API.ChangeQuery(query.ActionKeyword + " " + query.FirstSearch + " " + _memeName + " ");
                    return false;
                }
                var image = ImageGenerator.CreateImage(_memePath, possibleMockedImageText);
                Clipboard.SetImage(image);
                _context.API.ShowMsg(
                    $"copied {_memeName} to clipboard",
                    possibleMockedImageText,
                    _memePath
                );
                return true;
            }
        };
    }
    
    public static List<Meme> LoadAllFromMemesFolder(PluginInitContext context, MemeFolder parentMemeFolder) {
        if (!Directory.Exists(parentMemeFolder.FolderPath)) {
            return new List<Meme>();
        }
        var memes = new List<Meme>();
        var memeFiles = ImageExtensions
            .SelectMany(ext => Directory.GetFiles(parentMemeFolder.FolderPath, ext, SearchOption.TopDirectoryOnly))
            .Where(file => !Regex.IsMatch(Path.GetFileName(file), @"_icon\..*$"));
        memes.AddRange(memeFiles.Select(memeFile => new Meme(memeFile, context)));
        return memes;
    }
}