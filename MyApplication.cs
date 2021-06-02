using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen, debugScreen, rtScreen;
		public camera camera;
		public bool debug=true; //determines if the debugscreen will be shown
		// initialize
		Surface map;
		scene scene = new scene();
		public Vector3[] pixels;
		public void Init()
		{

			debugScreen = new Surface(512, 512);
			debugScreen.Clear(0x000000);
			rtScreen = new Surface(512, 512);
			rtScreen.Clear(0xffffff);
			pixels = new Vector3[512 * 512];


			Surface map; float[,] h;

			map = new Surface("../../assets/checkered_field.png");
			h = new float[696, 696];
			for (int y = 0; y < 696; y++) for (int x = 0; x < 696; x++) h[x, y] = ((float)(map.pixels[x + y * 696] & 695)) / 696;

		}



		// tick: renders one frame
		public void Tick()
		{
			rtScreen.drawscene(scene);
			rtScreen.CopyTo(screen, 0, 0);
			if (debug)
            {
				debugScreen.CopyTo(screen, 512, 0);
			}

	/*for (int i=0;i<512*512;i++)
	{
		Vector3 hdr = pixels[i];
		rtScreen.pixels[i] = MixColor(f2intcolor(hdr.X), f2intcolor(hdr.Y), f2intcolor(hdr.Z));
	}*/

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