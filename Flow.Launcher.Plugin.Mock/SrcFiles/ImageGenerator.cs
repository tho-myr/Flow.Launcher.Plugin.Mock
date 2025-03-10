﻿using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using SkiaSharp;

namespace Flow.Launcher.Plugin.Mock.SrcFiles;

public static class ImageGenerator {
    
    private const string FontFamily = "Segoe UI Emoji";
    
    public static BitmapImage CreateImage(string inputImagePath, string outputImagePath, string text) {
        using (var input = File.OpenRead(inputImagePath))
        using (var image = SKBitmap.Decode(input))
        using (var canvas = new SKCanvas(image)) {
            int fontSize = image.Height / 13; // Set font size to 7.69% of the image height (hehe 69 :3)
            int textMarginBottom = image.Height / 25; // Set text margin bottom to 4% of the image height
            int strokeWidth = fontSize / 5; // Set stroke width to 10% of the font size
            
            var font = new SKFont {
                Size = fontSize,
                Typeface = SKTypeface.FromFamilyName(FontFamily, SKFontStyle.Bold)
            };

            var paint = new SKPaint {
                Color = SKColors.White,
                IsAntialias = true
            };

            var paintOutline = new SKPaint {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth
            };

            var lines = SplitImageTextIntoLines(text, font, image);

            // Draw each line of text
            var yOffset = image.Height - textMarginBottom - (lines.Count - 1) * font.Size;
            foreach (var line in lines) {
                var point = new SKPoint(image.Width / 2, yOffset);
                canvas.DrawText(line, point, SKTextAlign.Center, font, paintOutline);
                canvas.DrawText(line, point, SKTextAlign.Center, font, paint);
                yOffset += font.Size;
            }

            using (var output = new MemoryStream()) {
                image.Encode(output, SKEncodedImageFormat.Png, 100);
                output.Seek(0, SeekOrigin.Begin);
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = output;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                SaveImageToOutputDir(bitmapImage, outputImagePath);
                return bitmapImage;
            }
        }
    }

    private static List<string> SplitImageTextIntoLines(string text, SKFont font, SKBitmap image) {
        var lines = new List<string>();
        var words = text.Split(' ');
        var currentLine = new List<string>();

        foreach (var word in words) {
            currentLine.Add(word);
            var currentText = string.Join(" ", currentLine);
            var textWidth = font.MeasureText(currentText);

            if (textWidth > image.Width) {
                currentLine.RemoveAt(currentLine.Count - 1);
                lines.Add(string.Join(" ", currentLine));
                currentLine.Clear();
                currentLine.Add(word);
            }
        }

        if (currentLine.Count > 0) {
            lines.Add(string.Join(" ", currentLine));
        }

        return lines;
    }

    private static void SaveImageToOutputDir(BitmapImage image, string outputPath) {
        string outputDir = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(outputDir)) {
            Directory.CreateDirectory(outputDir);
        }

        using (var fileStream = new FileStream(outputPath, FileMode.Create)) {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(fileStream);
        }
    }
}