using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class primitive
	{
		public Vector3 rgbcolor; //color in Vector3
	}
	class sphere : primitive
	{
		public Vector3 pos;
		public float r;
		public sphere(Vector3 position, float radius)
		{
			pos = position;
			r = radius;
		}
	}

	class plane : primitive
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
