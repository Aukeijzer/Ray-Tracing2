using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	public struct ray
	{
		public Vector3 O;  //origin
		public Vector3 D;  //direction
		public float t;    //distance
		public ray(Vector3 origin, Vector3 direction)
        {
			O = origin;
			D = direction;
			t = 1e37f;
        }
	}

	public class camera
	{
		public Vector3 E;   //camera position
		public float fov;   //fov
		public Vector3 V;   //camera direction
		public Vector3 C;   //screen center
		public Vector3 p0;  //screen corner
		public Vector3 p1;  //screen corner
		public Vector3 p2;  //screen corner
		public Vector3 p3; //screen corner

		public camera(Vector3 e, float fieldofview, Vector3 viewdirection)
		{
			E = e;              //camera position
			fov = fieldofview;  //field of view
			V = viewdirection; //camera direction
			C = E + fov * V;
			p0 = C + new Vector3(-1, 1, 0);
			p1 = C + new Vector3(1, 1, 0);
			p2 = C + new Vector3(-1, -1, 0);
			p3 = C + new Vector3(1, -1,0);
		}   //transform camera by multiplying p0, p1, p2, and E with the camera matrix.

		public Vector3 GetP(float u, float v)
		{
			Vector3 P;
			P = p0 + u * (p1 - p0) + v * (p2 - p0);
			return P;
		}
	}
	public class scene
	{
		public sphere s1;
		public sphere s2;
		public sphere s3;
		public List<primitive> primitives = new List<primitive>();
		public camera camera;
		public scene()
		{
			s1 = new sphere(new Vector3(2, 0, 5), 1f);
			s2 = new sphere(new Vector3(0, 0, 5), 1f);
			s3 = new sphere(new Vector3(-2, 0, 5), 1f);
			primitives.Add(s1);
			primitives.Add(s2);
			primitives.Add(s3);
		}
		public intersect calcIntersection(ray r)
		{
			bool intersection=false;
			primitive obj=new primitive();
			foreach (sphere s in primitives)
			{
				Vector3 D = r.D;
				Vector3 SO = new Vector3(r.O[0] - s.pos[0], r.O[1] - s.pos[1], r.O[2] - s.pos[2]);
				float rad = s.r;
				float a = Vector3.Dot(D, D);
				float b = Vector3.Dot(D, SO);
				float c = Vector3.Dot(SO, SO) - rad * rad;
				float d = b * b - 4 * a * c;
				float t;
				if (d >= 0)
                {
					intersection = true;
					t = -(b + (float)Math.Sqrt(d)) / (2 * a);
					if (t < r.t)
                    {
						r.t = t;
						obj = s;
                    }
                }					
			}
			Vector3 point = r.O + r.t * r.D;
			intersect intersect;
			if (intersection) { intersect = new intersect(point, obj, r); }
            else { intersect = new intersect(r); }
			return intersect;
		}
	}
	public class intersect
    {
		public Vector3 point;
		public primitive obj;
		public ray r;
		public bool intersection;
		public intersect(Vector3 point,primitive obj, ray r)
	    {
			this.point = point;
			this.obj = obj;
			this.r = r;
			this.intersection = true;
        }
		public intersect(ray r)
        {
			this.r = r;
			this.intersection = false;
        }
    }

}

