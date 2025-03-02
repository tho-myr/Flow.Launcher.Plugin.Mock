using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Flow.Launcher.Plugin.Mock.SrcFiles;

namespace Flow.Launcher.Plugin.Mock;

public class Main : IPlugin, IContextMenu {
    private PluginInitContext _context;

    private string _iconPath;
    private string _copyTextIconPath;
    private string _outputDir;
    
    private List<Meme> _memes = new List<Meme>();

    private Result _emptyQueryResult;
    private Result _openOutputDirResult;

    public void Init(PluginInitContext context) {
        _context = context;
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
        return new List<Result> {
            _openOutputDirResult
        };
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

            results.AddRange(_memes.Select(meme => meme.ToResult(mockedQuery, _context)));
        }

        return results;
    }
}