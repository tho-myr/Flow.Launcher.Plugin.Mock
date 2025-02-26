using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using SkiaSharp;

namespace Flow.Launcher.Plugin.Mock {
    public class Main : IPlugin, IContextMenu {
        private PluginInitContext _context;

        private string _iconPath = "Images\\icon.png";
        private string _copyTextIconPath = "Images\\copy-text-icon.png";
        private string _outputDir = "Images\\Output";

        private string _spongebobMemeImagePath = "Images\\mocking-spongebob-meme.png";
        private string _spongebobMemeImageOutputPath = "mocking-spongebob-meme-output.png";

        private Result _emptyQueryResult;
        private Result _openOutputDirResult;

        public void Init(PluginInitContext context) {
            _context = context;
            _iconPath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _iconPath);
            _copyTextIconPath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _copyTextIconPath);
            _spongebobMemeImagePath =
                Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _spongebobMemeImagePath);
            _outputDir = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _outputDir);
            _spongebobMemeImageOutputPath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _outputDir,
                _spongebobMemeImageOutputPath);
            
            _emptyQueryResult = new Result {
                Title = "please enter a query to mock",
                SubTitle = "PlEaSe eNtEr a qUeRy tO MoCk (\u2b2dω\u2b2d)",
                IcoPath = _iconPath
            };
            _openOutputDirResult = new Result {
                Title = "open output directory 📂",
                SubTitle = "containing last image generated",
                IcoPath = _iconPath,
                Action = _ => {
                    _context.API.OpenDirectory(_outputDir);
                    return true;
                }
            };
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

                results.Add(new Result {
                    Title = "copy mocking spongebob meme image",
                    SubTitle = mockedQuery,
                    IcoPath = _iconPath,
                    Action = _ => {
                        BitmapImage image = ImageGenerator.CreateMockedImage(
                            _spongebobMemeImagePath,
                            _spongebobMemeImageOutputPath,
                            mockedQuery
                        );
                        Clipboard.SetImage(image);
                        _context.API.ShowMsg(
                            "copied mocking spongebob meme to clipboard",
                            mockedQuery,
                            _iconPath
                        );
                        return true;
                    }
                });
            }

            return results;
        }
    }
}