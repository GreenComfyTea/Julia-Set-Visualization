using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ld4_Luznovs_Mandelbrot_Set
{
	class MyWriteableBitmap
	{
		private WriteableBitmap writeableBitmap;

		public MyWriteableBitmap(BitmapSource source)
		{
			writeableBitmap = new WriteableBitmap(source);
		}

		public MyWriteableBitmap(int pixelWidth, int pixelHeight, double dpiX, double dpiY, PixelFormat pixelFormat, BitmapPalette palette)
		{
			writeableBitmap = new WriteableBitmap(pixelWidth, pixelHeight, dpiX, dpiY, pixelFormat, palette);
		}

		public WriteableBitmap WriteableBitmap { get => writeableBitmap; set => writeableBitmap = value; }

		public void SetPixel(int x, int y, byte red, byte green, byte blue)
		{
			try
			{
				// Reserve the back buffer for updates.
				writeableBitmap.Lock();

				unsafe
				{
					// Get a pointer to the back buffer.
					int pBackBuffer = (int)writeableBitmap.BackBuffer;

					// Find the address of the pixel to draw.
					pBackBuffer += x * writeableBitmap.BackBufferStride;
					pBackBuffer += y * 4;

					// Compute the pixel's color.
					int color_data = red << 16; // R
					color_data |= green << 8;   // G
					color_data |= blue << 0;   // B

					// Assign the color data to the pixel.
					*((int*)pBackBuffer) = color_data;
				}

				// Specify the area of the bitmap that changed.
				writeableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
			}
			finally
			{
				// Release the back buffer and make it available for display.
				writeableBitmap.Unlock();
			}
		}

		public void SetPixel(int x, int y, Color color)
		{
			SetPixel(x, y, color.R, color.G, color.B);
		}
	}
}
