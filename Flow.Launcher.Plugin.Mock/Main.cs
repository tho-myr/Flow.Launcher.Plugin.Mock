using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Mock.Settings.Interface;
using Flow.Launcher.Plugin.Mock.Settings.Models;
using Flow.Launcher.Plugin.Mock.SrcFiles;

namespace Flow.Launcher.Plugin.Mock;

public class Main : IPlugin, IContextMenu, ISettingProvider {
    
    private PluginInitContext _context;
    private Settings.Models.Settings _settings;
    private SettingsViewModel _settingViewModel;

    private string _iconPath;
    private string _copyTextIconPath;
    private string _outputDir;

    private List<Meme> _memes = new();

    private Result _emptyQueryResult;
    private Result _openOutputDirResult;
    
    public void Init(PluginInitContext context) {
        _context = context;
        _settings = context.API.LoadSettingJsonStorage<Settings.Models.Settings>();
        _settingViewModel = new SettingsViewModel(_settings);
        _iconPath = PluginFile.FullPath(PluginFile.IconPath, context);
        _copyTextIconPath = PluginFile.FullPath(PluginFile.CopyTextIconPath, context);
        _outputDir = PluginDir.FullPath(PluginDir.OutputDir, context);
        
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
        var defaultCollection = new Result {
            Title = "mocked",
            SubTitle = "default collection for mocking images and mocking text",
            IcoPath = _iconPath,
            Action = _ => {
                if (!Directory.Exists(_outputDir)) {
                    Directory.CreateDirectory(_outputDir);
                }
                _context.API.OpenDirectory(_outputDir);
                return true;
            }
        };
        results.Add(defaultCollection);
        
        results.AddRange(_settingViewModel.Settings.CustomMemeFolders.Select(customMemeFolder => new Result {
            Title = customMemeFolder.Keyword,
            SubTitle = "custom meme folder :3",
            IcoPath = customMemeFolder.GetIconPath(),
            Action = _ => {
                if (!Directory.Exists(customMemeFolder.FolderPath)) {
                    _context.API.ShowMsg("Invalid Path :(", "Path could not be resolved or doesn't exist anymore 😣", _iconPath);
                    return false;
                }

                _context.API.OpenDirectory(customMemeFolder.FolderPath);
                return true;
            }
        }));
        return results;
    }
    
    public Control CreateSettingPanel() {
        return new PluginSettings(_context, _settingViewModel);
    }
}