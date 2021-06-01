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
			D = direction.Normalized();
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
		public Vector3 p3;  //screen corner

		public camera(Vector3 e, float fieldofview, Vector3 viewdirection)
		{
			E = e;              //camera position
			fov = fieldofview;  //field of view
			V = viewdirection;  //camera direction
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
		//s list of all primitives in the scene
		public List<primitive> primitives = new List<primitive>();
		public List<light> lightSources = new List<light>();
		public camera camera;
		public scene()
		{
			sphere s1 = new sphere(new Vector3(2, 0, 5), 1f);
			sphere s2 = new sphere(new Vector3(0, 0, 5), 1f);
			sphere s3 = new sphere(new Vector3(-2, 0, 5), 1f);
			light l1 = new light(new Vector3(9, 9, 1), 1f);
			light l2 = new light(new Vector3(-9, 6, 2), 0.6f);
			lightSources.Add(l1);
			lightSources.Add(l2);
			primitives.Add(s1);
			primitives.Add(s2);
			primitives.Add(s3);
		}
		//for a given ray gives the nearest intersection in a scene with a primitive. returns this as a intersection object.
		public intersect calcIntersection(ray r)
		{
			//if this is true then an intersection is found
			bool intersection=false;
			//the object with the nearest intersection
			primitive obj=new primitive();
			Vector3 D = r.D;
			Vector3 O = r.O;
			//t is calculated as the distance between the origin of the ray and the intersection
			float t=1e37f;
			//calculating all possible intersections for all spheres in the scene
			foreach (sphere s in primitives)
			{
				Vector3 SO = new Vector3(O[0] - s.pos[0], O[1] - s.pos[1], O[2] - s.pos[2]);
				float rad = s.r;
				float a = Vector3.Dot(D, D);
				float b = Vector3.Dot(D, SO);
				float c = Vector3.Dot(SO, SO) - rad * rad;
				float d = b * b - 4 * a * c;
				//if d<0 then the sphere has no intersection with the ray
				if (d >= 0)
                {
					intersection = true;
					t = -(b + (float)Math.Sqrt(d)) / (2 * a);
					//checking if there is any closer intersection
					if (t < r.t)
                    {
						r.t = t;
						obj = s;
                    }
                }					
			}
			//calculating all possible intersections for all planes in the scene
			foreach (plane p in primitives)
            {
				Vector3 N = p.normal;
				float d = p.t;
				float ndotd = Vector3.Dot(N, D);
				//if ndot==0 then the ray is parallel to the plane, thus we check if the origin is in the plane (which means N*O+d==0) and if it is t is set to 0.
				if (ndotd == 0 && Vector3.Dot(N, O) + d == 0)
                {
					intersection = true;
					t = 0;
                }
				//if ndot!=0 calculate the intersection the regular way
                else
                {
					t = (ndotd + d) / Vector3.Dot(N, O);
                }
				//checking if there is any closer intersection
				if (t < r.t)
                {
					r.t = t;
					obj = p;
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
		public Vector3 point; //the point of intersection
		public primitive obj; //the primitive with which is intersected
		public ray r; //the ray which caused the intersection
		public bool intersection; //if this is true then an intersection is made. (when a ray has no intersection it still creates an intersection object)
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
		public Vector3 reflection(ray r, Vector3 surface)
		{
			return (r.D - 2 * Vector3.Dot(r.D, surface) * surface);
		}
	}
}