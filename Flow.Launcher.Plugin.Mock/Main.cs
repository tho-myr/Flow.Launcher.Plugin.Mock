using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Mock.Settings;
using Flow.Launcher.Plugin.Mock.SrcFiles;

namespace Flow.Launcher.Plugin.Mock;

public class Main : IPlugin, IContextMenu, ISettingProvider {
    
    private PluginInitContext _context;
    private Settings.Settings _settings;
    private SettingsViewModel _settingViewModel;

    private string _iconPath;
    private string _copyTextIconPath;
    private string _outputDir;

    private List<Meme> _memes = new();

    private Result _emptyQueryResult;
    private Result _openOutputDirResult;
    
    internal static string CustomIconsDirectory = "CustomIcons";

    public void Init(PluginInitContext context) {
        _context = context;
        _settings = context.API.LoadSettingJsonStorage<Settings.Settings>();
        _settingViewModel = new SettingsViewModel(_settings);
        _iconPath = PluginFile.FullPath(PluginFile.IconPath, context);
        _copyTextIconPath = PluginFile.FullPath(PluginFile.CopyTextIconPath, context);
        _outputDir = PluginDir.FullPath(PluginDir.OutputDir, context);
        CustomIconsDirectory = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, CustomIconsDirectory);
        
        _emptyQueryResult = new Result {
            Title = "please enter a query to mock",
            SubTitle = "PlEaSe eNtEr a qUeRy tO MoCk (\u2b2dω\u2b2d)",
            IcoPath = _iconPath
        };
        _openOutputDirResult = new Result {
            Title = "open output directory 📂",
            SubTitle = "folder contains last image generated for each meme",
            IcoPath = _iconPath,
            Action = _ => {
                if (!Directory.Exists(_outputDir)) {
                    Directory.CreateDirectory(_outputDir);
                }
                _context.API.OpenDirectory(_outputDir);
                return true;
            }
        };

        _memes = Meme.LoadAllFromMemesFolder(_context);
    }
    
    public List<Result> LoadContextMenus(Result selectedResult) {
        var contextMenuResults = new List<Result> {
            _openOutputDirResult
        };

        if (selectedResult is not { ContextData: Meme meme }) return contextMenuResults;

        contextMenuResults.Insert(0, meme.ToResult(null, _context, true));
        contextMenuResults.Insert(1, meme.ToResult(null, _context, true, false));

        return contextMenuResults;
    }

    public List<Result> Query(Query query) {
        var results = new List<Result>();
        var mockedQuery = MockingCaseConverter.Convert(query.Search);

        if (string.IsNullOrEmpty(mockedQuery)) {
            results.Add(_emptyQueryResult);
        }
        else {
            results.Add(new Result {
                Title = "copy mocked text",
                SubTitle = mockedQuery,
                IcoPath = _copyTextIconPath,
                Score = _memes.Count * 100,
                Action = _ => {
                    Clipboard.SetText(mockedQuery);
                    _context.API.ShowMsg(
                        "copied mocked text to clipboard",
                        mockedQuery,
                        _iconPath
                    );
                    return true;
                }
            });

            results.AddRange(_memes.Select(meme => meme.ToResult(query.Search, _context)));
        }

        return results;
    }
    
    public Control CreateSettingPanel() {
        return new PluginSettings(_context, _settingViewModel);
    }
}