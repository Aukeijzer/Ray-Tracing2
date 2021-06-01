using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	struct ray
	{
		public Vector3 O;  //origin
		public Vector3 D;  //direction
		public float t;    //distance
	}

	class camera
	{
		public Vector3 E;	//camera position
		public float fov;	//fov
		public Vector3 V;	//camera direction
		public Vector3 C;	//screen center
		public Vector3 p0;	//screen corner
		public Vector3 p1;	//screen corner
		public Vector3 p2;	//screen corner

		public camera(Vector3 e, float fieldofview, Vector3 viewdirection)
		{
			E = e;              //camera position
			fov = fieldofview;  //field of view
			V = viewdirection;
			C = E + fov * V;
			p0 = C + new Vector3(-1, 1, 0);
			p1 = C + new Vector3(1, 1, 0);
			p2 = C + new Vector3(-1, -1, 0);
		}	//transform camera by multiplying p0, p1, p2, and E with the camera matrix.

		public Vector3 GetP(float u, float v)
		{
			Vector3 P;
			P = p0 + u * (p1 - p0) + v * (p2 - p0);
			return P;
		}

		class scene
		{
			public sphere s1;
			public sphere s2;
			public sphere s3;
			public List<sphere> sphereList = new List<sphere>();
			public scene()
			{
				s1 = new sphere(new Vector3(2, 0, 5), 1f);
				s2 = new sphere(new Vector3(0, 0, 5), 1f);
				s3 = new sphere(new Vector3(-2, 0, 5), 1f);
				sphereList.Add(s1);
				sphereList.Add(s2);
				sphereList.Add(s3);
			}

			public Vector3 Intersection(ray r)
			{
				Vector3 intersect;

				foreach (sphere s in sphereList)
				{
					float a = r.D[0] * r.D[0] + r.D[1] * r.D[1] + r.D[2] * r.D[2];
					float b = 
				}

				return intersect;
			}
		}
	}
}

