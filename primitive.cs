using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	//Masterclass for the primitives.
	public class Primitive
	{
		public Vector3 rgbcolor;
		public bool reflective=false;

		public static int MixColor(int red, int green, int blue)
		{
			return (red << 16) + (green << 8) + blue;
		}
		public static int F2intcolor(float color)
		{
			return Math.Min((int)(color * 256f), 255);
		}
		public static int Vec2intcolor(Vector3 color)
		{
			return MixColor(F2intcolor(color[0]), F2intcolor(color[1]), F2intcolor(color[2]));
		}
	}

	//Subclass for spheres, with its own constructor functions. When gives an rgb value, it is considered non-reflective.
	public class Sphere : Primitive
	{
		public Vector3 pos;
		public float r;

		public Sphere(Vector3 position, float radius)
		{
			pos = position;
			r = radius;
			reflective = true;
		}
		public Sphere(Vector3 position, float radius, Vector3 rgb)
		{
			pos = position;
			r = radius;
			reflective = false;
			rgbcolor = rgb;
		}
	}

	//Subclass for planes, with its own constructor functions. When gives an rgb value, it is considered non-reflective.
	public class Plane : Primitive
	{
		public Vector3 normal;
		public float t;

		public Plane(Vector3 basenormal, float distance)
		{
			normal = basenormal;
			t = distance;
			reflective = true;
		}
		public Plane(Vector3 basenormal, float distance, Vector3 rgb)
		{
			normal = basenormal;
			t = distance;
			reflective = false;
			rgbcolor = rgb;
		}
	}
}
