using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using SkiaSharp;

namespace Flow.Launcher.Plugin.Mock {
    public class Main : IPlugin {
        private PluginInitContext _context;

        public void Init(PluginInitContext context) {
            _context = context;
        }

        public List<Result> Query(Query query) {
            var results = new List<Result>();
            var mockedQuery = MockingCase(query.Search);

            if (string.IsNullOrEmpty(mockedQuery)) {
                results.Add(new Result {
                    Title = "please enter a query to mock",
                    SubTitle = "No qUeRy eNtErEd (\u2b2dω\u2b2d)",
                    IcoPath = "Images/icon.png"
                });
            } else {
                results.Add(new Result {
                    Title = "copy mocked text",
                    SubTitle = mockedQuery,
                    IcoPath = "Images/copy-text-icon.png",
                    Action = _ => {
                        Clipboard.SetText(MockingCase(mockedQuery));
                        return true;
                    }
                });

                results.Add(new Result {
                    Title = "copy mocking spongebob meme image",
                    SubTitle = mockedQuery,
                    IcoPath = "Images/icon.png",
                    Action = _ => {
                        string iconPath = Path.Combine(_context.CurrentPluginMetadata.PluginDirectory,
                            "Images/icon.png");
                        string imagePath = Path.Combine(_context.CurrentPluginMetadata.PluginDirectory,
                            "Images/mocking-spongebob-meme.png");
                        BitmapImage image = CreateMockedImage(imagePath, mockedQuery);
                        Clipboard.SetImage(image);
                        SaveImageToOutputDir(image);
                        _context.API.ShowMsg(
                            "copied mocking spongebob meme to clipboard", 
                            mockedQuery, 
                            iconPath
                        );
                        return true;
                    }
                });
            }

            return results;
        }

        private void SaveImageToOutputDir(BitmapImage image) {
            string outputPath = Path.Combine(_context.CurrentPluginMetadata.PluginDirectory,
                "Images/Output/mocking-spongebob-meme-output.png");

            // ensure the Output directory exists
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDir)) {
                Directory.CreateDirectory(outputDir);
            }

            // save image to disk
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
            // _context.API.ShowMsg("Creating image with text \"" + text + "\"");
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
                    // _context.API.ShowMsg("Image copied to clipboard with text \"" + text + "\"");
                    return bitmapImage;
                }
            }
        }
    }
}