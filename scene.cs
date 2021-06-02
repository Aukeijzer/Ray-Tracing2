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
		public ray(Vector3 origin, Vector3 direction,float distance)
		{
			O = origin;
			D = direction.Normalized();
			t = distance;
		}
	}
	//The camera class is used to let surfaces know which
	public class camera
	{
		public Vector3 E;   //camera position
		public Vector3 V;   //camera direction
		public Vector3 C;   //screen center
		public Vector3 p0;  //screen corner (top left)
		public Vector3 p1;  //screen corner (top right)
		public Vector3 p2;  //screen corner (bottom left)
		public Vector3 p3;  //screen corner (bottem right)
		public float d;		//distance E to screenplane

		public camera(Vector3 e, int fieldofview, Vector3 viewdirection)
		{
			E = e;              //camera position
			float normalPeopleUseRadians = (float)((fieldofview * Math.PI) / 180);
			float d = (float)(1f / Math.Tan(normalPeopleUseRadians)); //distance from E to screen plane (1f is half the screen plane).
			V = viewdirection.Normalized();  //camera direction (0,0,1)
			C = E + d * V;
			p0 = C + new Vector3(-1, 1, 0);
			p1 = C + new Vector3(1, 1, 0);
			p2 = C + new Vector3(-1, -1, 0);
			p3 = C + new Vector3(1, -1,0);
		}   //transform camera by multiplying p0, p1, p2, and E with the camera matrix.

	}
	//this scene stores all lightsources and primitives and is used to calculate intersections between rays and primitives. 
	public class scene
	{
		public List<primitive> primitives = new List<primitive>(); //a list of all primitives in the scene
		public List<light> lightSources = new List<light>(); //a list of all lightsources in the scene
		public camera camera; //the camera, mainly used as membervariable to pass on to methods using the scene object
		int maxreflect;
		//TODO: make less stupid constructor
		public scene()
		{
			camera = new camera(new Vector3(0, 0, 0), 120, new Vector3(0, 0, 1));
			maxreflect = 3;
			sphere s1 = new sphere(new Vector3(0, -2, 5), 1f, new Vector3(1,0,0));
			sphere s2 = new sphere(new Vector3(0, 0, 5), 1f, new Vector3(0, 1, 0));
			sphere s3 = new sphere(new Vector3(0, 2, 5), 1f, new Vector3(0, 0, 1));
			plane p1=new plane(new Vector3(0,0,1))
			light l1 = new light(new Vector3(9, 9, 1), 1f);
			light l2 = new light(new Vector3(-9, 6, 2), 0.6f);
			lightSources.Add(l1);
			lightSources.Add(l2);
			primitives.Add(s1);
			primitives.Add(s2);
			primitives.Add(s3);
		}
		//for a given ray gives the nearest intersection in a scene with a primitive. returns this as a intersection object.
		public intersect calcIntersection(ray r,int i)
		{
			//if this is true then the ray met a primitive
			bool intersection=false;
			//the object with the nearest intersection
			primitive obj=new primitive();
			Vector3 D = r.D;
			Vector3 O = r.O;
			//t is calculated as the distance between the origin of the ray and the intersection
			float t=1e37f;
			foreach (primitive P in primitives)
			{
				//calculating intersections for all spheres in the scene
				if (P is sphere)
				{
					sphere s = (sphere)P;
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
					}
				}
				//calculating intersections for all planes in the scene
				if (P is plane)
                {
					plane p = (plane)P;
					Vector3 N = p.normal;
					float d = p.t;
					float ndotd = Vector3.Dot(N, D);
					//if ndot==0 then the ray is parallel to the plane
					if (ndotd == 0)
					{
						//check if the origin is in the plane(which means N* O+d == 0) and if it is t is set to 0.
						if (Vector3.Dot(N, O) + d == 0)
                        {
							intersection = true;
							t = 0;
						}
					}
					//if ndot!=0 calculate the intersection the regular way
					else
					{
						intersection = true;
						t = (ndotd + d) / Vector3.Dot(N, O);
					}
				}
				if (t < r.t)
				{
					r.t = t;
					obj = P;
				}
			}
			intersect intersect = new intersect(r);
			Vector3 point = r.O + r.t * r.D;
			//if the primitive is reflective the ray will reflect and land somewhere else, thus caclIntersection will be call recursively to find the intersection of that ray.
			if (obj.reflective)
			{
				//if the ray has reflected maxreflect amount of times and hits a reflective object it will be assumed the ray didnt meet a non-reflective primitive.
				if (i > maxreflect)
				{
					//calculating the normal to the surface the ray reflects upon
					Vector3 normal = new Vector3();
					if (obj is sphere) normal = (point - ((sphere)obj).pos).Normalized();
					if (obj is plane) normal = ((plane)obj).normal.Normalized();

					//calculating the direction of the reflected ray
					Vector3 D2 = r.D - 2 * Vector3.Dot(r.D, normal) * normal;
					ray r2 = new ray(point, D2);
					//calling calcIntersection to see where the new ray will end up
					intersect = calcIntersection(r2, i + 1);
				}
			}
            else
            {
				//if the ray met a primitive who isnt reflective it will
				if (intersection) { intersect = new intersect(point, obj, r); }
			}
			return intersect;
		}
	}
	//the class intersection is used to store information about intersections of rays and to handle logic with intersections.
	public class intersect
    {
		public Vector3 point; //this is the point where the ray lands
		public primitive obj; //the primitive with which is intersected
		public ray r; //the ray which caused the intersection
		public bool intersection_made; //if this is true then the ray has met an non-reflective object (when a ray doesnt meet any primitives still creates an intersection object)
		//if an intersection is made between a ray and primitive this constructormethod will store all relevant information in the membervariables.
		public intersect(Vector3 point,primitive obj, ray r)
	    {
			this.point = point;
			this.obj = obj;
			this.r = r;
			this.intersection_made = true;
        }
		//if no intersection is made for a ray this constructor is used.
		public intersect(ray r)
        {
			this.r = r;
			this.intersection_made = false;
        }
	}
}