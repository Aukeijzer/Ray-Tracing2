using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	public class primitive
	{
		public Vector3 rgbcolor;
		public bool reflective;

		public static int MixColor(int red, int green, int blue)
		{
			return (red << 16) + (green << 8) + blue;
		}
		public static int f2intcolor(float color)
		{
			return Math.Min((int)color * 256, 255);
		}
		public static int vec2intcolor(Vector3 color)
		{
			return MixColor(f2intcolor(color[0]), f2intcolor(color[1]), f2intcolor(color[2]));
		}
	}
	public class sphere : primitive
	{
		public Vector3 pos;
		public float r;
		public bool refl;
		public Vector3 rgb;
		public sphere(Vector3 position, float radius)
		{
			pos = position;
			r = radius;
			reflective = true;
		}
		public sphere(Vector3 position, float radius, Vector3 rgbcolor)
		{
			pos = position;
			r = radius;
			reflective = false;
			rgb = rgbcolor;
		}
	}

	public class plane : primitive
	{
		public Vector2 normal;
		public float t;
		public Vector3 rgb;
		public bool refl;

		public plane(Vector2 basenormal, float distance)
		{
			normal = basenormal;
			t = distance;
			reflective = true;
		}
		public plane(Vector2 basenormal, float distance, Vector3 rgbcolor)
		{
			normal = basenormal;
			t = distance;
			reflective = false;
			rgb = rgbcolor;
		}
	}

}
