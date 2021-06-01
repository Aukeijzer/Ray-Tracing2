using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
	public struct Ray
	{
		public Vector3 O;    //origin
		public Vector3 D;    //direction
		public float t; //distance
	}

	class scene
    {
		public sphere s1;
		public sphere s2;
		public sphere s3;

		public scene()
        {
			s1 = new sphere( new Vector3(2, 0, 5), 1f );
			s2 = new sphere( new Vector3(0, 0, 5), 1f );
			s3 = new sphere( new Vector3(-2, 0, 5), 1f );
		}
    }

	class camera
	{
		public Vector3 E;
		public float fov;
		public static Vector3 p0 = new Vector3(1, 1, 0);
		public static Vector3 p1 = new Vector3(-1, 1, 0);
		public static Vector3 p2 = new Vector3(1, -1, 0);

		public camera(Vector3 e, float fieldofview)
		{
			E = e;              //camera position
			fov = fieldofview;  //field of view
		}

		public static Vector3 GetP(float u, float v)
		{
			Vector3 P;
			P = p0 + u * (p1 - p0) + v * (p2 - p0);
			return P;
		}

		public Vector3 GetE() => E;
	}
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

	class MyApplication
	{
		// member variables
		public Surface screen, debugScreen, rtScreen;
		// initialize
		Surface map;
		public Vector3[] pixels;
		public void Init()
		{
			debugScreen = new Surface(512, 512);
			debugScreen.Clear(0x000000);
			rtScreen = new Surface(512, 512);
			rtScreen.Clear(0xffffff);
			pixels = new Vector3[512 * 512];
		}

		public Vector3 Intersection()
		{
			Vector3 intersect;
			//...
			return intersect; //returns a point
		}

		// tick: renders one frame
		public void Tick()
		{
			rtScreen.CopyTo(screen, 0, 0);
			debugScreen.CopyTo(screen, 512, 0);
			for (int y = 0; y < 512; y++)
			{
				for (int x = 0; x < 512; x++)
				{
					//
					float u = (float)x / 512;
					float v = (float)y / 512;


					Vector3 P = camera.GetP(u, v);

					Ray R;
					R.O = camera.GetE();
					R.D = Vector3.Normalize(P - R.O);
					R.t = 1e37f;
					//for (each prmitive in .. )
					{
						Vector3 t = Intersection();
						if (t < R.t)
						{
							//...
						}

					}
				}
				for (int x = 512; x < 1024; x++)
				{

				}
				/*for (int i=0;i<512*512;i++)
				{
					Vector3 hdr = pixels[i];
					rtScreen.pixels[i] = MixColor(f2intcolor(hdr.X), f2intcolor(hdr.Y), f2intcolor(hdr.Z));
				}*/

			}

			//returns the color in one int.
			static int MixColor(int red, int green, int blue)
			{
				return (red << 16) + (green << 8) + blue;
			}
			static int f2intcolor(float color)
			{
				return Math.Min((int)color * 256, 255);
			}
			int TX(float x)
			{
				x += 2;
				return (int)(x / 4 * screen.width);
			}
			int TY(float y)
			{
				y = -y;
				y += 2;
				return (int)(y / 4 * screen.height) * (screen.width / screen.height);
			}
			public void RenderGL()
			{
			}


		}
	}
}
