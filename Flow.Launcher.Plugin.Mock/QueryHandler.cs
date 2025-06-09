using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Mock.Settings.Models;
using Flow.Launcher.Plugin.Mock.SrcFiles;

namespace Flow.Launcher.Plugin.Mock;

public class QueryHandler {
    
    private static readonly Result NotFoundResult = new Result {
        Title = "no images found in folder :(",
        SubTitle = "please check your meme folder path in settings",
        IcoPath = "Images/no-results-icon.png"
    };

    private readonly PluginInitContext _context;
    private readonly SettingsViewModel _settingViewModel;

    public QueryHandler(PluginInitContext context, SettingsViewModel settingViewModel) {
        _context = context;
        _settingViewModel = settingViewModel;
    }

    public List<Result> Query(Query query) {
        var customMemeFolder = _settingViewModel.Settings.GetCustomMemeFolder(_context, query.FirstSearch);
        if (customMemeFolder != null) {
            var memes = Meme.LoadAllFromMemesFolder(_context, customMemeFolder);
            if (memes == null || memes.Count == 0) {
                return new List<Result> { NotFoundResult };
            }

            var thirdToEndSearch = query.SearchTerms.Length > 2 ? string.Join(' ', query.SearchTerms[2..]) : "";

            var imageGenResults = memes.Select(meme => meme.ToResult(query, thirdToEndSearch))
                        .Where(result => result.Title.Contains(query.SecondSearch))
                        .ToList();

            if (imageGenResults.Count != 1) return imageGenResults;
            
            if (imageGenResults.First().ContextData is Meme foundMeme && thirdToEndSearch != "") {
                imageGenResults.Add(foundMeme.ToResult(query, thirdToEndSearch, false));
            }
            return imageGenResults;
        }
        
        var results = new List<Result>();

        results.AddRange(_settingViewModel.Settings.GetAllMemeFolders().Select(memeFolder => new Result {
            Title = memeFolder.Keyword,
            SubTitle = memeFolder.Description ?? "",
            IcoPath = memeFolder.GetIconPath(_context),
            AutoCompleteText = query.ActionKeyword + " " + memeFolder.Keyword + " ",
            Action = _ => {
                _context.API.ChangeQuery(query.ActionKeyword + " " + memeFolder.Keyword + " ");
                return false;
            }
        }));

        return results.Where(result =>
            result.Title.Contains(query.Search, StringComparison.OrdinalIgnoreCase)
            || result.SubTitle.Contains(query.Search, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }
}