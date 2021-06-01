using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class camera
	{

		public Vector3 E; //camera position
		public float fov; //fov
		public Vector3 V; //camera direction
		public Vector3 p0 = new Vector3(1, 1, 0);
		public Vector3 p1 = new Vector3(-1, 1, 0);
		public Vector3 p2 = new Vector3(1, -1, 0);

		public camera(Vector3 e, float fieldofview)
		{
			E = e;              //camera position
			fov = fieldofview;  //field of view

		}

		public Vector3 GetP(float u, float v)
		{
			Vector3 P;
			P = p0 + u * (p1 - p0) + v * (p2 - p0);
			return P;
		}

	}
}
