using System;
using System.Diagnostics;

namespace JuliaSetVisualization
{
	class Complex
	{
		private double real;
		private double imaginary;

		public double Real { get => real; set => real = value; }
		public double Imaginary { get => imaginary; set => imaginary = value; }


		public static readonly Complex Zero = new Complex(0.0, 0.0);
		public static readonly Complex One = new Complex(1.0, 0.0);

		public Complex(double real, double imaginary)
		{
			this.real = real;
			this.imaginary = imaginary;
		}

		public Complex() { }

		public Complex(Complex value)
		{
			this.real = value.real;
			this.imaginary = value.imaginary;
		}

		public static Complex FromPolarCoordinates(double magnitude, double phase)
		{
			return new Complex((magnitude * Math.Cos(phase)), (magnitude * Math.Sin(phase)));
		}

		public static Complex Add(Complex left, Complex right)
		{
			return left + right;
		}

		public static Complex Subtract(Complex left, Complex right)
		{
			return left - right;
		}

		public static Complex Multiply(Complex left, Complex right)
		{
			return left * right;
		}

		public static Complex Divide(Complex dividend, Complex divisor)
		{
			return dividend / divisor;
		}

		public static Complex operator -(Complex value)
		{
			return new Complex(-value.real, -value.imaginary);
		}

		public static Complex operator +(Complex left, Complex right)
		{
			return new Complex(left.real + right.real, left.imaginary + right.imaginary);

		}

		public static Complex operator -(Complex left, Complex right)
		{
			return new Complex(left.real - right.real, left.imaginary - right.imaginary);
		}

		public static Complex operator *(Complex left, Complex right)
		{
			// Multiplication:  (a + bi)(c + di) = (ac - bd) + (bc + ad)i

			double resultReal = (left.real * right.real) - (left.imaginary * right.imaginary);
			double resultImaginary = (left.imaginary * right.real) + (left.real * right.imaginary);

			return new Complex(resultReal, resultImaginary);
		}

		public static Complex operator /(Complex left, Complex right)
		{
			// Division : Smith's formula.
			double a = left.real;
			double b = left.imaginary;
			double c = right.real;
			double d = right.imaginary;

			if (Math.Abs(d) < Math.Abs(c))
			{
				double doc = d / c;
				return new Complex((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
			}
			else
			{
				double cod = c / d;
				return new Complex((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
			}
		}

		public static bool operator ==(Complex left, Complex right)
		{
			return left.real == right.real && left.imaginary == right.imaginary;


		}
		public static bool operator !=(Complex left, Complex right)
		{
			return left.real != right.real || left.imaginary != right.imaginary;
		}

		public static double Abs(Complex value)
		{

			if (Double.IsInfinity(value.real) || Double.IsInfinity(value.imaginary))
			{
				return Double.PositiveInfinity;
			}

			// |value| == sqrt(a^2 + b^2)
			// To dodge Overflow:
			// sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(1 + b^2/a^2)

			double c = Math.Abs(value.real);
			double d = Math.Abs(value.imaginary);

			if (c > d)
			{
				double r = d / c;
				return c * Math.Sqrt(1d + r * r);
			}
			else if (d == 0.0)
			{
				return c;
			}
			else
			{
				double r = c / d;
				return d * Math.Sqrt(1d + r * r);
			}
		}

		public static double Phi(Complex value)
		{
			if(value == Zero)
			{
				return Double.NaN;
			}

			return Math.Atan2(value.imaginary, value.real);
		}

		public static Complex Pow(Complex value, Complex power)
		{

			if (power == Zero)
			{
				return One;
			}

			if (value == Zero)
			{
				return Zero;
			}

			double a = value.real;
			double b = value.imaginary;
			double c = power.real;
			double d = power.imaginary;

			double abs = Complex.Abs(value);
			double phi = Complex.Phi(value);
			double newAbs = c * phi + d * Math.Log(abs);

			double t1 = Math.Pow(abs, c);
			double t2 = Math.Pow(Math.E, -d * phi);

			return new Complex(t1 * t2 * Math.Cos(newAbs), t1 * t2 * Math.Sin(newAbs));
		}

		public static Complex Pow(Complex value, double power)
		{
			return Complex.Pow(value, new Complex(power, 0));
		}

		public static Complex Conjugate(Complex value)
		{
			// The conjugate of x+i*y is x-i*y 

			return new Complex(value.real, -value.imaginary);
		}

		public static Complex Reciprocal(Complex value)
		{
			// The reciprocal of x+i*y is 1/(x+i*y)

			if ((value.real == 0) && (value.imaginary == 0))
			{
				return Zero;
			}

			return One / value;
		}

		public static string PolarForm(Complex value)
		{
			if(value == Zero || Double.IsInfinity(value.real) || Double.IsInfinity(value.imaginary) || Double.IsNaN(value.real) || Double.IsNaN(value.imaginary))
			{
				return "Undefined";
			}

			return "(" + Abs(value) + ") * (cos(" + Phi(value) + ") + i * sin(" + Phi(value) + ")";
		}

		public static string ExponentialForm(Complex value)
		{
			if (Double.IsInfinity(value.real) || Double.IsInfinity(value.imaginary) || Double.IsNaN(value.real) || Double.IsNaN(value.imaginary) || value == Zero)
			{
				return "Undefined";
			}

			return "(" + Abs(value) + ") * e ^ ((" + Phi(value) + ") * i)";
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Complex))
			{
				return false;
			}
			return this == ((Complex) obj);
		}

		public bool Equals(Complex value)
		{
			return real.Equals(value.real) && imaginary.Equals(value.imaginary);

		}

		public override Int32 GetHashCode()
		{
			Int32 n1 = 99999997;
			Int32 hashReal = real.GetHashCode() % n1;
			Int32 hashImaginary = imaginary.GetHashCode();
			Int32 hash = hashReal ^ hashImaginary;
			return hash;
		}

		public override string ToString()
		{
			string complexString = "";

			if (real == 0 && imaginary == 0)
			{
				return "0";
			}

			if (Double.IsNaN(real) && Double.IsNaN(imaginary))
			{
				return Double.NaN.ToString();
			}

			if (real != 0d)
			{
				complexString = "(" + real.ToString() + ")";
			}

			if(imaginary != 0d)
			{
				complexString += imaginary > 0d ? " + (" : " - (";
				complexString += Math.Abs(imaginary).ToString() + ") * i";
			}

			return complexString;
		}
	}
}
