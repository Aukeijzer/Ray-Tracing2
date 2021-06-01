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
		public Vector3 rgbcolor; //color in Vector3

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
		public sphere(Vector3 position, float radius)
		{
			pos = position;
			r = radius;
		}
	}

	public class plane : primitive
	{
		public Vector2 normal;
		public float t;

		public plane(Vector2 basenormal, float distance)
		{
			normal = basenormal;
			t = distance;
		}
	}

}
