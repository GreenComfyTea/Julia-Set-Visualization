using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JuliaSetVisualization
{
	class Selection
	{
		private Rectangle rectangle;
		private Point position;

		private double aspectRatio = 16d / 9d;

		public Rectangle Rectangle { get => rectangle; set => rectangle = value; }
		public Point Position { get => position; set => position = value; }
		public double AspectRatio { get => aspectRatio; set => aspectRatio = value; }

		public Selection()
		{
			Init();
			Update();
		}

		public Selection(double x1, double y1, double x2, double y2)
		{
			Init();
			Update(x1, y1, x2, y2);
		}

		public Selection(Point first, Point second)
		{
			Init();
			Update(first.X, first.Y, second.X, second.Y);
		}

		public Selection(Point first, double x2, double y2)
		{
			Init();
			Update(first.X, first.Y, x2, y2);
		}

		public Selection(double x1, double y1, Point second)
		{
			Init();
			Update(x1, y1, second.X, second.Y);
		}

		private void Init()
		{
			rectangle = new Rectangle();
			rectangle.Stroke = Brushes.White;
		}

		public void Update(Point first, Point second)
		{
			Update(first.X, first.Y, second.X, second.Y);
		}

		public void Update(double x1 = 0d, double y1 = 0d, double x2 = 0d, double y2 = 0d)
		{
			rectangle.Width = Math.Abs(x1 - x2);
			rectangle.Height = Math.Abs(y1 - y2); 

			position = new Point(x1 <= x2 ? x1 : x2, y1 <= y2 ? y1 : y2);

			if (aspectRatio > 1d)
			{
				rectangle.Height = (rectangle.Width / aspectRatio);
			}
			else
			{
				rectangle.Width = (rectangle.Height * aspectRatio);
			}
		}
	}
}
