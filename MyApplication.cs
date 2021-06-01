using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
    struct ray
    {
		Vector3 O;	//origin
		Vector3 D;	//direction
		float t;	//distance
    }

	class camera
	{
		public Vector3 E;
		public float fov;
		public Vector3 p0 = new Vector3(1, 1, 0);
		public Vector3 p1 = new Vector3(-1, 1, 0);
		public Vector3 p2 = new Vector3(1, -1, 0);

		public camera(Vector3 e, float fieldofview)
		{
			E = e;				//camera position
			fov = fieldofview;  //field of view

		}

		public Vector3 GetP(float u, float v)
        {
			Vector3 P;
			P = p0 + u * (p1 - p0) + v * (p2 - p0);
			return P;
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

					/*
					Vector3 P = camera.GetP(u, v);

					Ray R;
					R.O = camera.GetEye();
					R.D = Vector3.Normalize(P - R.O);
					R.t = 1e37f;
					for (each prmitive in .. )
                    {
						t = Intersection();
						if (t < R.t)
                        {
							...
                        }
					*/
                    }
                }
				for (int x=512; x < 1024; x++)
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
			x+=2;
			return (int)(x / 4 * screen.width);
		}
		int TY(float y)
		{
			y = -y;
			y+=2;
			return (int)(y / 4 * screen.height)*(screen.width/screen.height);
		}
		public void RenderGL()
		{
		}


	}
}