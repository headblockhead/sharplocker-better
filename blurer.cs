using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SharpLocker
{
    public static class blurer
    {
        public static void BoxBlur(this WriteableBitmap bmp, int range)
        {
            if ((range & 1) == 0)
            {
                throw new InvalidOperationException("Range must be odd!");
            }

            bmp.BoxBlurHorizontal(range);
            bmp.BoxBlurVertical(range);
        }

        public static void BoxBlurHorizontal(this WriteableBitmap bmp, int range)
        {
            int[] pixels = bmp.Pixels;
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int halfRange = range / 2;
            int index = 0;
            int[] newColors = new int[w];

            for (int y = 0; y < h; y++)
            {
                int hits = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                for (int x = -halfRange; x < w; x++)
                {
                    int oldPixel = x - halfRange - 1;
                    if (oldPixel >= 0)
                    {
                        int col = pixels[index + oldPixel];
                        if (col != 0)
                        {
                            r -= ((byte)(col >> 16));
                            g -= ((byte)(col >> 8));
                            b -= ((byte)col);
                        }
                        hits--;
                    }

                    int newPixel = x + halfRange;
                    if (newPixel < w)
                    {
                        int col = pixels[index + newPixel];
                        if (col != 0)
                        {
                            r += ((byte)(col >> 16));
                            g += ((byte)(col >> 8));
                            b += ((byte)col);
                        }
                        hits++;
                    }

                    if (x >= 0)
                    {
                        int color =
                            (255 << 24)
                            | ((byte)(r / hits) << 16)
                            | ((byte)(g / hits) << 8)
                            | ((byte)(b / hits));

                        newColors[x] = color;
                    }
                }

                for (int x = 0; x < w; x++)
                {
                    pixels[index + x] = newColors[x];
                }

                index += w;
            }
        }

        public static void BoxBlurVertical(this WriteableBitmap bmp, int range)
        {
            int[] pixels = bmp.Pixels;
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int halfRange = range / 2;

            int[] newColors = new int[h];
            int oldPixelOffset = -(halfRange + 1) * w;
            int newPixelOffset = (halfRange) * w;

            for (int x = 0; x < w; x++)
            {
                int hits = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                int index = -halfRange * w + x;
                for (int y = -halfRange; y < h; y++)
                {
                    int oldPixel = y - halfRange - 1;
                    if (oldPixel >= 0)
                    {
                        int col = pixels[index + oldPixelOffset];
                        if (col != 0)
                        {
                            r -= ((byte)(col >> 16));
                            g -= ((byte)(col >> 8));
                            b -= ((byte)col);
                        }
                        hits--;
                    }

                    int newPixel = y + halfRange;
                    if (newPixel < h)
                    {
                        int col = pixels[index + newPixelOffset];
                        if (col != 0)
                        {
                            r += ((byte)(col >> 16));
                            g += ((byte)(col >> 8));
                            b += ((byte)col);
                        }
                        hits++;
                    }

                    if (y >= 0)
                    {
                        int color =
                            (255 << 24)
                            | ((byte)(r / hits) << 16)
                            | ((byte)(g / hits) << 8)
                            | ((byte)(b / hits));

                        newColors[y] = color;
                    }

                    index += w;
                }

                for (int y = 0; y < h; y++)
                {
                    pixels[y * w + x] = newColors[y];
                }
            }
        }
        private static int[] GetPixels(this WriteableBitmap bmp)
        {
            int[] pixels = new int[bmp.PixelHeight * bmp.PixelWidth * bmp.Format.BitsPerPixel / 8];

            bmp.CopyPixels(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight),
            pixels, bmp.PixelWidth * bmp.Format.BitsPerPixel / 8, 0);

            return pixels;
        }

        private static void SetPixels(this WriteableBitmap bmp, int[] pixels)
        {
            bmp.WritePixels(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight),
            pixels, bmp.PixelWidth * bmp.Format.BitsPerPixel / 8, 0);
        }
    }
}