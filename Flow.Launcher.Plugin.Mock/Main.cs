using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void Init(PluginInitContext context) {
            _context = context;
            _iconPath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _iconPath);
            _copyTextIconPath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _copyTextIconPath);
            _spongebobMemeImagePath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _spongebobMemeImagePath);
            _outputDir = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _outputDir);
            _spongebobMemeImageOutputPath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _outputDir, _spongebobMemeImageOutputPath);
        }

        public List<Result> Query(Query query) {
            var results = new List<Result>();
            var mockedQuery = MockingCase(query.Search);

            if (string.IsNullOrEmpty(mockedQuery)) {
                results.Add(new Result {
                    Title = "please enter a query to mock",
                    SubTitle = "PlEaSe eNtEr a qUeRy tO MoCk (\u2b2dω\u2b2d)",
                    IcoPath = _iconPath
                });
            } else {
                results.Add(new Result {
                    Title = "copy mocked text",
                    SubTitle = mockedQuery,
                    IcoPath = _copyTextIconPath,
                    Action = _ => {
                        Clipboard.SetText(MockingCase(mockedQuery));
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
                        BitmapImage image = CreateMockedImage(_spongebobMemeImagePath, mockedQuery);
                        Clipboard.SetImage(image);
                        SaveImageToOutputDir(image, _spongebobMemeImageOutputPath);
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

        public List<Result> LoadContextMenus(Result selectedResult) {
            return new List<Result> {
                new Result {
                    Title = "open output directory 📂",
                    SubTitle = "containing last image generated",
                    IcoPath = _iconPath,
                    Action = _ => {
                        _context.API.OpenDirectory(_outputDir);
                        return true;
                    }
                }
            };
        }

        private void SaveImageToOutputDir(BitmapImage image, string outputPath) {
            if (!Directory.Exists(_outputDir)) {
                Directory.CreateDirectory(_outputDir);
            }

            using (var fileStream = new FileStream(outputPath, FileMode.Create)) {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }

        private string MockingCase(string input) {
            var result = new char[input.Length];
            for (int i = 0; i < input.Length; i++) {
                result[i] = i % 2 == 0 ? char.ToUpper(input[i]) : char.ToLower(input[i]);
            }

            return new string(result);
        }

        private BitmapImage CreateMockedImage(string inputImagePath, string text) {
            using (var input = File.OpenRead(inputImagePath))
            using (var image = SKBitmap.Decode(input))
            using (var canvas = new SKCanvas(image)) {
                var font = new SKFont {
                    Size = 80,
                    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                };

                var paint = new SKPaint {
                    Color = SKColors.White,
                    IsAntialias = true
                };

                var paintOutline = new SKPaint {
                    Color = SKColors.Black,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 6
                };


                var point = new SKPoint(image.Width / 2, image.Height - 60);
                canvas.DrawText(text, point, SKTextAlign.Center, font, paintOutline);
                canvas.DrawText(text, point, SKTextAlign.Center, font, paint);

                using (var output = new MemoryStream()) {
                    image.Encode(output, SKEncodedImageFormat.Png, 100);
                    output.Seek(0, SeekOrigin.Begin);
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = output;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
        }
    }
}