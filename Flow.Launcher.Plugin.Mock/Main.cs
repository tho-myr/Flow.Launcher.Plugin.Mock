using System.Collections.Generic;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Mock.Settings.Interface;
using Flow.Launcher.Plugin.Mock.Settings.Models;

namespace Flow.Launcher.Plugin.Mock;

public class Main : IPlugin, IContextMenu, ISettingProvider {
    
    private PluginInitContext _context;
    private Settings.Models.Settings _settings;
    private SettingsViewModel _settingViewModel;
    private QueryHandler _queryHandler;

    public void Init(PluginInitContext context) {
        _context = context;
        _settings = context.API.LoadSettingJsonStorage<Settings.Models.Settings>();
        _settingViewModel = new SettingsViewModel(_settings);
        _queryHandler = new QueryHandler(context, _settingViewModel);
    }
    
    public List<Result> LoadContextMenus(Result selectedResult) {
        // TODO: Implement context menu loading logic.
        return new List<Result>();
    }

    public List<Result> Query(Query query) {
        return _queryHandler.Query(query);
    }
    
    public Control CreateSettingPanel() {
        return new PluginSettings(_context, _settingViewModel);
    }
}