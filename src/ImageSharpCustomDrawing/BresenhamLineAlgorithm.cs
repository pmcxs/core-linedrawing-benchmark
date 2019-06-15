using System;
using System.Collections.Generic;
using System.Numerics;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace ImageSharpCustomDrawing
{
    public static class BresenhamLineAlgorithm
    {
        /// <summary>
        /// Draws a line between two points using Bresenham's algorithm with a thickness of 1 pixel
        /// </summary>
        /// <remarks>Algorithm from http://tech-algorithm.com/articles/drawing-line-using-bresenham-algorithm/</remarks>
        /// <param name="image"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public static void DrawLine(Span<Rgba32> image, float x1, float y1, float x2, float y2, Rgba32 color, int width, int height)
        {
            float w = x2 - x1;
            float h = y2 - y1;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            float longest = Math.Abs(w);
            float shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            float numerator = (int)longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                DrawingUtils.SetPixel(image, x1, y1, color, width, height);

                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x1 += dx1;
                    y1 += dy1;
                }
                else
                {
                    x1 += dx2;
                    y1 += dy2;
                }
            }
        }

        /// <summary>
        /// Using a variant of Bresenham algorithm obtains the various points of a line (without the first one) without any diagonal movement
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        public static IEnumerable<Point> GetNonDiagonalPointsOfLine(int x0, int y0, Vector2 normalVector, float targetDistance)
        {
            int originalX = x0;
            int originalY = y0;

            int x1 = (int)Math.Round(x0 + normalVector.X);
            int y1 = (int)Math.Round(y0 + normalVector.Y);

            int xDist = Math.Abs(x1 - x0);
            int yDist = -Math.Abs(y1 - y0);
            int xStep = x0 < x1 ? +1 : -1;
            int yStep = y0 < y1 ? +1 : -1;
            int error = xDist + yDist;

            //Add additional pixels while the distance is below the target one
            while (Math.Sqrt(Math.Pow(x0 - originalX, 2) + Math.Pow(y0 - originalY, 2)) < targetDistance)
            {
                float e2 = error << 1;

                if (e2 - yDist > xDist - e2)
                {
                    // horizontal step
                    error += yDist;
                    x0 += xStep;
                }
                else
                {
                    // vertical step
                    error += xDist;
                    y0 += yStep;
                }

                yield return new Point(x0, y0);
            }
        }
    }

}
