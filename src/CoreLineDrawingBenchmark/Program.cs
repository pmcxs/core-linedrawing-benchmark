using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;
using ImageSharpCustomDrawing;

namespace CoreLineDrawingBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            DrawLinesAndMeasure(imageSize: 1000, numberOfLines: 10, lineWidth: 1);
            DrawLinesAndMeasure(imageSize: 1000, numberOfLines: 100, lineWidth: 1);
            DrawLinesAndMeasure(imageSize: 1000, numberOfLines: 1000, lineWidth: 1);
            DrawLinesAndMeasure(imageSize:1000, numberOfLines:10, lineWidth:5);
            DrawLinesAndMeasure(imageSize:1000, numberOfLines:100, lineWidth:5);
            DrawLinesAndMeasure(imageSize:1000, numberOfLines:1000, lineWidth:5);
            DrawLinesAndMeasure(imageSize:1000, numberOfLines:10, lineWidth:10);
            DrawLinesAndMeasure(imageSize:1000, numberOfLines:100, lineWidth:10);
            DrawLinesAndMeasure(imageSize:1000, numberOfLines:1000, lineWidth:10);
        }

        private static void DrawLinesAndMeasure(int imageSize, int numberOfLines, int lineWidth)
        {
            var lines = new List<int[]>();
            var rnd = new Random(1);

            for (int i = 0; i < numberOfLines; i++)
            {
                int pixelX = rnd.Next(imageSize);
                int pixelY = rnd.Next(imageSize);
                lines.Add(new int[] { pixelX, pixelY });
            }

            SixLabors.Primitives.PointF[] imageSharpLines = lines
                .Select(r => new SixLabors.Primitives.PointF(r[0], r[1]))
                .ToArray();

            PointF[] systemDrawingLines = lines.Select(r => new PointF(r[0], r[1])).ToArray();

            var w = new Stopwatch();

            w.Start();
            using (var image = new Image<Rgba32>(imageSize, imageSize))
            {
                image.Mutate(x => x

                    .DrawLines(
                        Rgba32.Red,
                        lineWidth,
                        imageSharpLines));

                using (var stream = File.Create($"test-imageSharp-{imageSize}-{numberOfLines}-{lineWidth}.png"))
                {
                    image.SaveAsPng(stream);
                }
            }
            w.Stop();

            Console.WriteLine($"Took {w.ElapsedMilliseconds} ms with ImageSharp");

            w.Restart();
            using (var image = new Image<Rgba32>(imageSize, imageSize))
            {
                image.DrawLines(imageSharpLines, Rgba32.Red, lineWidth);

                using (var stream = File.Create($"test-custom-{imageSize}-{numberOfLines}-{lineWidth}.png"))
                {
                    image.SaveAsPng(stream);
                }
            }
            w.Stop();

            Console.WriteLine($"Took {w.ElapsedMilliseconds} ms with Custom");

            w.Restart();

            using (var destination = new Bitmap(imageSize, imageSize))
            using (var graphics = Graphics.FromImage(destination))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var pen = new System.Drawing.Pen(System.Drawing.Color.Red, lineWidth))
                {
                    pen.StartCap = LineCap.Flat;
                    pen.EndCap = LineCap.Flat;

                    graphics.DrawLines(pen, systemDrawingLines);
                }

                using (var stream = File.Create($"test-systemDrawing-{imageSize}-{numberOfLines}-{lineWidth}.png"))
                {
                    destination.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                }

                w.Stop();

                Console.WriteLine($"Took {w.ElapsedMilliseconds} ms with System.Drawing.Common");
            }
        }
    }
}