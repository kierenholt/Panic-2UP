using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Panic
{
    public static class extensions
    {


        public static int mod(this int x, int m)
        {
            return (x % m + m) % m;
        }

        public static char toLowerAlpha(this int n)
        {
            return (char)('a' + n);
        }
        
        public static void processRGB(this byte[] imgdata, int index, Color TargetColor)
        {
            //double saturation = (imgdata[index] + imgdata[index + 1] + imgdata[index + 2]) / 3.0 / 255.0;
            //imgdata[index] = (byte)(saturation * TargetColor.B);
            //imgdata[index + 1] = (byte)(saturation * TargetColor.G);
            //imgdata[index + 2] = (byte)(saturation * TargetColor.R);

            imgdata[index] = (byte)(imgdata[index] + (byte)((1 - imgdata[index] / 255.0) * TargetColor.B));
            imgdata[index + 1] = (byte)(imgdata[index + 1] + (byte)((1 - imgdata[index + 1] / 255.0) * TargetColor.G));
            imgdata[index + 2] = (byte)(imgdata[index + 2] + (byte)((1 - imgdata[index + 2] / 255.0) * TargetColor.R)); 
        }


        public static List<WriteableBitmap> colorized(this List<WriteableBitmap> list, Color targetColor)
        {
            return list.Select(wbmp => wbmp.colorized(targetColor)).ToList();
        }

        public static WriteableBitmap colorized(this WriteableBitmap wbitmap, Color TargetColor)
        {
            // draw using byte array
            int width = wbitmap.PixelWidth, height = wbitmap.PixelHeight , bytesperpixel = 4;
            int stride = width * bytesperpixel;
            byte[] imgdata = wbitmap.ToByteArray();

            // draw a gradient from red to green from top to bottom (R00 -> ff; Gff -> 00)
            // draw a gradient of alpha from left to right
            // Blue constant at 00
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // BGRA
                    imgdata.processRGB(row * stride + col * 4, TargetColor); 
                }
            }
            return new WriteableBitmap(BitmapSource.Create(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, imgdata, stride));
        }

        public static List<WriteableBitmap> toFrames(this Bitmap image, int startX, int startY, int frameWidth, int frameHeight, int numFrames)
        {
            List<WriteableBitmap> retVal = new List<WriteableBitmap>();

            for (int frame = 0; frame < numFrames; frame++)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    image.Clone(new RectangleF(startX + frameWidth * frame, startY, frameWidth, frameHeight), PixelFormat.Format32bppArgb).Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    retVal.Add( new WriteableBitmap(bitmapImage) );
                }
            }
            return retVal;
        }

        public static List<WriteableBitmap> ToFrame(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return new List<WriteableBitmap>() { new WriteableBitmap(bitmapImage) };
            }
        }

        public static Bitmap tile(this Bitmap bitmap, int HTiles, int VTiles)
        {
            Bitmap dst = new Bitmap(bitmap.Width * HTiles, bitmap.Height * VTiles); 
            using (Graphics g = Graphics.FromImage(dst)) 
            {
                for (int i = 0; i < HTiles; i++)
                    for (int j = 0; j < VTiles; j++)
                    {
                        g.DrawImage(bitmap, new System.Drawing.Point(i * bitmap.Width, j * bitmap.Height)); 
                    }
            }
            return dst;
        }


        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random rng)
        {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}
