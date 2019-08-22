using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; 
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JuliaSetVisualization
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly double normalTime = 0.0000000005d;

		private Complex complex;
		private int iterationCount;

		private double xMin;
		private double xMax;
		private double yMin;
		private double yMax;

		private bool isJuliaSetCreated;

		private Point first;
		private Point second;

		private Selection selection;

		bool withBounds = false;

		public MainWindow()
		{
			InitializeComponent();

			Loaded += delegate
			{
				selection = new Selection();
				selection.AspectRatio = canvas.ActualWidth / canvas.ActualHeight;
			};

			RenderOptions.SetBitmapScalingMode(canvasImage, BitmapScalingMode.NearestNeighbor);
			RenderOptions.SetEdgeMode(canvasImage, EdgeMode.Aliased);

			isJuliaSetCreated = false;

			xMin = Double.NaN;
			xMax = Double.NaN;
			yMin = Double.NaN;
			yMax = Double.NaN;

			iterationCountTextBox.Text = GetOptimalIterationCount();
		}

		private string GetOptimalIterationCount()
		{
			return Convert.ToInt32(1d / (normalTime * Environment.ProcessorCount * Width * Height)).ToString();
		}

		private void ResetBounds()
		{
			double radius = (1 + Math.Sqrt(1d + 4d * Complex.Abs(complex))) / 2d;

			double x = radius * selection.AspectRatio;

			xMin = -x;
			yMin = -radius;
			xMax = x;
			yMax = radius;
		}

		private void Start(object sender, RoutedEventArgs e)
		{
			withBounds = false;
			ProcessAsync();
		}

		private void StartWithBounds(object sender, RoutedEventArgs e)
		{
			withBounds = true;
			ProcessAsync();
		}

		private async void ProcessAsync()
		{
			try
			{
				ReadInput();
				if (withBounds)
				{
					ReadBounds();
				}
				else
				{
					ResetBounds();
				}
			}
			catch (Exception)
			{
				ConsoleBox.Text = "Error!";
				return;
			}

			Stopwatch stopwatch = Stopwatch.StartNew();

			isJuliaSetCreated = false;

			StartButton.IsEnabled = false;
			StartWithBoundsButton.IsEnabled = false;
			ConsoleBox.Text = "Processing...";

			canvasImage.Source = (await Task.Run(() => new JuliaSet((int)canvas.ActualWidth, (int)canvas.ActualHeight, complex, iterationCount, xMin, xMax, yMin, yMax))).WriteableBmp;

			if (canvas.Children.Contains(selection.Rectangle))
			{
				canvas.Children.Remove(selection.Rectangle);
			}

			StartButton.IsEnabled = true;
			StartWithBoundsButton.IsEnabled = true;
			isJuliaSetCreated = true;

			stopwatch.Stop();
			ConsoleBox.Text = "Done! Elapsed Time: " + stopwatch.Elapsed;
		}

		private void ReadInput()
		{
			bool iterationsSuccess = int.TryParse(iterationCountTextBox.Text, out int iterationCount);
			bool realSuccess = double.TryParse(realTextBox.Text.Replace(',', '.'), out double real);
			bool imaginarySuccess = double.TryParse(imaginaryTextBox.Text.Replace(',', '.'), out double imaginary);

			if (!iterationsSuccess || !realSuccess || !imaginarySuccess)
			{
				throw new Exception();

			}

			this.complex = new Complex(real, imaginary);
			this.iterationCount = iterationCount;
		}

		private void ReadBounds()
		{
			bool xMinSuccess = double.TryParse(xMinTextBox.Text.Replace(',', '.'), out double xMin);
			bool xMaxSuccess = double.TryParse(xMaxTextBox.Text.Replace(',', '.'), out double xMax);
			bool yMinSuccess = double.TryParse(yMinTextBox.Text.Replace(',', '.'), out double yMin);
			bool yMaxSuccess = double.TryParse(yMaxTextBox.Text.Replace(',', '.'), out double yMax);

			if (!xMinSuccess || !xMaxSuccess || !yMinSuccess || !yMaxSuccess || xMax < xMin || yMax < yMin)
			{
				throw new Exception();
			}

			this.xMin = xMin;
			this.xMax = xMax;
			this.yMin = yMin;
			this.yMax = yMax;
		}

		private void SetBounds()
		{
			double xMinTemp = 0d;
			double xMaxTemp = 0d;
			double yMinTemp = 0d;
			double yMaxTemp = 0d;

			try
			{
				xMinTemp = AffineTransformation(selection.Position.X, 0, canvas.ActualWidth, xMin, xMax);
				xMaxTemp = AffineTransformation(selection.Position.X + selection.Rectangle.Width, 0, canvas.ActualWidth, xMin, xMax);
				yMinTemp = AffineTransformation(selection.Position.Y, 0, canvas.ActualHeight, yMin, yMax);
				yMaxTemp = AffineTransformation(selection.Position.Y + selection.Rectangle.Height, 0, canvas.ActualHeight, yMin, yMax);
			}
			catch(Exception)
			{
				ConsoleBox.Text = "Error!";
				return;
			}

			xMinTextBox.Text = xMinTemp.ToString();
			xMaxTextBox.Text = xMaxTemp.ToString();
			yMinTextBox.Text = yMinTemp.ToString();
			yMaxTextBox.Text = yMaxTemp.ToString();
		}

		private void MousePressed(object sender, MouseButtonEventArgs e)
		{
			if (isJuliaSetCreated)
			{
				first = e.GetPosition(canvas);
				if (canvas.Children.Contains(selection.Rectangle))
				{
					canvas.Children.Remove(selection.Rectangle);
				}
				canvas.Children.Add(selection.Rectangle);
				canvas.CaptureMouse();
			}
		}

		private void MouseReleased(object sender, MouseButtonEventArgs e)
		{
			if (isJuliaSetCreated && canvas.IsMouseCaptured)
			{
				canvas.ReleaseMouseCapture();
				second = e.GetPosition(canvas);
				selection.Update(first, second);
				SetBounds();
			}
		}

		private void MouseMoved(object sender, MouseEventArgs e)
		{
			if (isJuliaSetCreated && canvas.IsMouseCaptured)
			{
				second = e.GetPosition(canvas);
				selection.Update(first, second);

				Canvas.SetLeft(selection.Rectangle, selection.Position.X);
				Canvas.SetTop(selection.Rectangle, selection.Position.Y);
			}
		}

		private double AffineTransformation(double value, double oldMin, double oldMax, double newMin, double newMax)
		{
			if (value < oldMin || value > oldMax || oldMax == oldMin)
			{
				throw new Exception("Incorrect arguments");
			}

			return ((value - oldMin) * ((newMax - newMin) / (oldMax - oldMin))) + newMin;
		}
	}
}
