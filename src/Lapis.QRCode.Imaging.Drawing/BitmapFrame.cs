using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lapis.QRCode.Imaging.Drawing
{
    public class BitmapFrame : IRgb24BitmapFrame, IDisposable
    {
        public BitmapFrame(Bitmap bitmap)
        {
        	Stopwatch stopWatch = new Stopwatch();
        	stopWatch.Start();
        	
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            Bitmap = bitmap;
            
            stopWatch.Stop();
				// Get the elapsed time as a TimeSpan value.
				TimeSpan ts = stopWatch.Elapsed;

				// Format and display the TimeSpan value.
				string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
					ts.Hours, ts.Minutes, ts.Seconds,
					ts.Milliseconds / 10);
				Console.WriteLine("BitmapFrameTime " + elapsedTime);
        }

        public Bitmap Bitmap { get; }

        public int Height => Bitmap.Height;

        public int Width => Bitmap.Width;

        public int GetPixel(int x, int y)
        {
            return ColorHelper.ToIntRgb24(Bitmap.GetPixel(x, y));
        }

        public void SetPixel(int x, int y, int pixel)
        {
            Bitmap.SetPixel(x, y, ColorHelper.FromIntRgb24(pixel));
        }

        public int[] GetPixels()
        {
            return (
                from x in Enumerable.Range(0, Bitmap.Width)
                from y in Enumerable.Range(0, Bitmap.Height)
                select ColorHelper.ToIntRgb24(Bitmap.GetPixel(x, y))
            ).ToArray();
        }

        public void Save(Stream stream)
        {
            Bitmap.Save(stream, Bitmap.RawFormat ?? ImageFormat.Png);
        }

        #region IDisposable Support

        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Bitmap.Dispose();
                }
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
