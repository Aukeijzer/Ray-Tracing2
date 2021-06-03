using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	//Struct for the rays. WE always use .Normalized() to keep the given direction at length 1, so t will always be it's length.
	public struct ray
	{
		public Vector3 O;  //The ray its origin.
		public Vector3 D;  //The ray its direction.
		public float t;    //The distance it will or has traveled.

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

	//The camera class is used to set the screenplane, it's corners, the FOV, viewdirection and camera position.
	public class camera
	{
		public Vector3 E;   //Camera position.
		public Vector3 V;   //Camera direction (0,0,1) ussually.
		public Vector3 C;   //Screen center.
		public Vector3 p0;  //Screen corner (top left).
		public Vector3 p1;  //Screen corner (top right).
		public Vector3 p2;  //Screen corner (bottom left).
		public Vector3 p3;  //Screen corner (bottem right).
		public float d;     //Distance E to screenplane.
		public float fov;   //Field of view (degrees).

		//Transform the camera by multiplying p0, p1, p2, and E with the camera matrix.
		public camera(Vector3 e, int fieldofview, Vector3 viewdirection)
		{
			E = e;
			fov = (float)((fieldofview * Math.PI) / 180);
			d = (float)(1f / Math.Tan(fov/2));
			V = viewdirection.Normalized();
			C = E + d * V;
			p0 = C + new Vector3(-1, 1, 0);
			p1 = C + new Vector3(1, 1, 0);
			p2 = C + new Vector3(-1, -1, 0);
			p3 = C + new Vector3(1, -1,0);
		}   
	}

	//The scene stores all lightsources and primitives and is used to calculate intersections between rays and primitives. 
	public class scene
	{
		public List<primitive> primitives = new List<primitive>();	//A list of all primitives in the scene.
		public List<light> lightSources = new List<light>();		//A list of all lightsources in the scene
		public camera camera;										//The camera, mainly used as membervariable to pass on to methods using the scene object
		int maxReflect;												//Integer used to show how many reflections are allowed between reflective primitives.
		
		//Scene constructor function.
		public scene()
		{
			camera = new camera(new Vector3(0, 0, 0), 120, new Vector3(0, 0, 1));
			
			sphere s1 = new sphere(new Vector3(-3f, 0, 6f), 1f, new Vector3(1,0,0));
			sphere s2 = new sphere(new Vector3(0, 0, 6f), 1f);
			sphere s3 = new sphere(new Vector3(3f, 0, 6f), 1f, new Vector3(0, 0, 1));
			plane p1 = new plane(new Vector3(0, 1, 0), -2);
			light l1 = new light(new Vector3(9, 9, 1), 1f);
			light l2 = new light(new Vector3(-9, 6, 2), 0.6f);

			lightSources.Add(l1);
			lightSources.Add(l2);
			primitives.Add(s1);
			primitives.Add(s2);
			primitives.Add(s3);
			primitives.Add(p1);
			maxReflect = 10000;
		}

		//For a given ray gives the nearest intersection in a scene with a primitive. Returns this as n intersection object.
		public intersect calcIntersection(ray r,int i)
		{
			//If true, then the ray met a primitive.
			bool intersection=false;

			//The object with the nearest intersection.
			primitive obj=new primitive();

			Vector3 D = r.D;
			Vector3 O = r.O;

			//t is calculated as the distance between the origin of the ray and the intersection.
			float t=1e37f;

			foreach (primitive P in primitives)
			{
				//Calculating intersections for all spheres in the scene.
				if (P is sphere)
				{
					sphere s = (sphere)P;
					Vector3 SO = new Vector3(O[0] - s.pos[0], O[1] - s.pos[1], O[2] - s.pos[2]);
					float rad = s.r;
					float a = Vector3.Dot(D, D);
					float b = 2*Vector3.Dot(D, SO);
					float c = Vector3.Dot(SO, SO) - rad * rad;
					float d = b * b - 4 * a * c;

					//If d<0 then the sphere has no intersection with the ray.
					if (d >= 0)
					{
						//Normally t=t1, however in the edge case where te camera is inside the sphere, t=t2.
						float t1= -(b + (float)Math.Sqrt(d)) / (2 * a);
						if (t1 >= 0)
                        {
							intersection = true;
							t = t1;
						}
                        else
                        {
							float t2= -(b - (float)Math.Sqrt(d)) / (2 * a);
							if (t2 >= 0)
                            {
								intersection = true;
								t = t2;
                            }
						}
					}
				}

				//Calculating intersections for all planes in the scene.
				if (P is plane)
                {
					plane p = (plane)P;
					Vector3 N = p.normal;
					float d = p.t;
					float ndotd = Vector3.Dot(N, D);

					//If ndot==0 then the ray is parallel to the plane.
					if (ndotd == 0)
					{
						//Check if the origin is in the plane (which means N*O+d == 0). If it is, t is set to 0.
						if (Vector3.Dot(N, O) == 0)
                        {
							intersection = true;
							t = 0;
						}
					}

					//If ndot!=0 calculate the intersection the regular way.
					else
					{
						float t1 = -(Vector3.Dot(N,O) + d) / ndotd;
						if (t1 >= 0)
                        {
							intersection = true;
							t = t1;
						}
					}
				}
				if (t < r.t && t > 0)
				{
					r.t = t;
					obj = P;
				}
			}
			intersect intersect = new intersect(r);
			Vector3 point = r.O + r.t * r.D;

			//If the primitive is reflective the ray will reflect and land somewhere else, thus caclIntersection will be call recursively to find the intersection of that ray.
			if (obj.reflective)
			{
				//If the ray has reflected maxReflect amount of times and hits a reflective object it will be assumed the ray didnt meet a non-reflective primitive.
				if (i < maxReflect)
				{
					//Calculating the normal to the surface the ray reflects upon.
					Vector3 normal = new Vector3();
					if (obj is sphere) normal = (point - ((sphere)obj).pos).Normalized();
					if (obj is plane) normal = ((plane)obj).normal.Normalized();

					//Calculating the direction of the reflected ray.
					Vector3 D2 = r.D - 2 * Vector3.Dot(r.D, normal) * normal;
					ray r2 = new ray(point, D2);

					//Calling calcIntersection to see where the new ray will end up.
					intersect = calcIntersection(r2, i + 1);
				}
			}
            else
            {
				//Creates an intersect object that stores the point of intersection, the object it insects with, and the intersecting ray.
				if (intersection) { 
					intersect = new intersect(point, obj, r); 
				}
			}
			return intersect;
		}
	}

	//The class intersect is used to store information about intersections of rays and to handle logic with intersections.
	public class intersect
    {
		public Vector3 point;					//The the point where the ray lands.
		public primitive obj;					//The primitive with which is intersected.
		public ray r;							//The ray which caused the intersection.
		public bool intersection_made;			//If this is true then the ray has met an non-reflective object (when a ray doesnt meet any primitives it will still create an intersect object).

		//If an intersection is made between a ray and primitive this constructor method will store all relevant information in the membervariables.
		public intersect(Vector3 point,primitive obj, ray r)
	    {
			//If the color is zero, obj is just an empty intitialized obj.
			if (obj.rgbcolor!=new Vector3(0, 0, 0))
            {
				this.point = point;
				this.obj = obj;
				this.r = r;
				this.intersection_made = true;
			}
            else
            {
				this.r = r;
				this.intersection_made = false;
            }
        }

		//If no intersection is made for a ray this constructor is used instead.
		public intersect(ray r)
        {
			this.r = r;
			this.intersection_made = false;
        }
	}
}