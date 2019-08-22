using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JuliaSetVisualization
{
	class JuliaSet
	{
		public WriteableBitmap WriteableBmp { get; set; }

		public JuliaSet(int width, int height, Complex complex, int iterationCount, double realMin, double realMax, double imaginaryMin, double imaginaryMax)
		{
			WriteableBmp = Create(width, height, complex, iterationCount, realMin, realMax, imaginaryMin, imaginaryMax);
		}

		private WriteableBitmap Create(int width, int height, Complex complex, int iterationCount, double realMin, double realMax, double imaginaryMin, double imaginaryMax)
		{
			double radius = (1 + Math.Sqrt(1 + 4 * Complex.Abs(complex))) / 2;

			double realStep = Math.Abs(realMax - realMin) / width;
			double imaginaryStep = Math.Abs(imaginaryMax - imaginaryMin) / height;

			Complex[,] z = new Complex[width, height];

			int[,] iterations = new int[width, height];

			WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Rgb24, null);
			int stride = writeableBitmap.Format.BitsPerPixel / 8;
			byte[] colors = new byte[width * height * stride];

			int[] nums = Enumerable.Range(0, height).ToArray();
			Parallel.ForEach(nums, y =>
			{
				for (int x = 0; x < width; x++)
				{
					z[x, y] = new Complex(realMin + x * realStep, imaginaryMin + y * imaginaryStep);
					Complex newZ = new Complex(z[x, y]);

					int iteration = 0;
					for (; iteration < iterationCount; iteration++)
					{
						newZ = Complex.Pow(newZ, new Complex(2d, 0d));
						newZ += complex;

						if (radius > 0d && Complex.Abs(newZ) > radius)
						{
							break;
						}
					}

					double value = (iteration + 1 - Math.Log(Math.Log(Complex.Abs(newZ))) / Math.Log(2)) / (double) iterationCount;

					Color color = Colors.Black;
					if (iteration != iterationCount)
					{
						color = HsvToRgb(value * 360d, 1d, value < 1d ? 1d : 0d);
					}

					colors[(x + y * width) * stride] = color.R;
					colors[(x + y * width) * stride + 1] = color.G;
					colors[(x + y * width) * stride + 2] = color.B;
				}
			});

			writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), colors, stride * width, 0, 0);
			writeableBitmap.Freeze();
			return writeableBitmap;
		}

		private static Color HsvToRgb(double h, double S, double V)
		{
			byte v = Convert.ToByte(V * 255);

			double H = h;
			while (H < 0)
			{
				H += 360;
			};
			while (H >= 360)
			{
				H -= 360;
			};

			if (V <= 0)
			{
				return Color.FromRgb(0, 0, 0);
			}
			else if (S <= 0)
			{
				return Color.FromRgb(v, v, v);
			}
			else
			{
				double hf = H / 60.0;
				int i = (int) Math.Floor(hf);
				double f = hf - i;

				byte pv = Convert.ToByte(V * (1 - S) * 255);
				byte qv = Convert.ToByte(V * (1 - S * f) * 255);
				byte tv = Convert.ToByte(V * (1 - S * (1 - f)) * 255);
				
				switch (i)
				{
					case 0:		return Color.FromRgb(v, tv, pv);
					case 1:		return Color.FromRgb(qv, v, pv);
					case 2:		return Color.FromRgb(pv, v, tv);
					case 3:		return Color.FromRgb(pv, qv, v);
					case 4:		return Color.FromRgb(tv, pv, v);
					case 5:		return Color.FromRgb(v, pv, qv);
					case 6:		return Color.FromRgb(v, tv, pv);
					case -1:	return Color.FromRgb(v, pv, qv);
					default:	return Color.FromRgb(v, v, v);
				}
			}
		}
	}
}
