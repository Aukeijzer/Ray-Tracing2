using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
	public class MyApplication
	{
		// member variables
		public Surface screen, debugScreen, rtScreen;
		public camera camera;
		public bool debug = true; //determines if the debugscreen will be shown
								  // initialize
		Surface map;
		scene scene = new scene();
		//public Vector3[] pixelsRay;
		public List<ray> pixelsRayAll = new List<ray>();
		public void Init()
		{
			debugScreen = new Surface(512, 512);
			debugScreen.Clear(0x000000);
			rtScreen = new Surface(512, 512);
			rtScreen.Clear(0xffffff);
			//pixelsRay = new Vector3[512 * 512];


			/*
			Surface map; float[,] h;

			map = new Surface("../../assets/checkered_field.png");
			h = new float[696, 696];
			for (int y = 0; y < 696; y++) for (int x = 0; x < 696; x++) h[x, y] = ((float)(map.pixels[x + y * 696] & 695)) / 696;*/

		}
		// tick: renders one frame
		public void Tick()
		{
			rtScreen.drawscene(scene);
			rtScreen.CopyTo(screen, 0, 0);
			if (debug)
			{
				debugScreen.drawdebug(scene);
				debugScreen.CopyTo(screen, 512, 0);
			}
		}
		public void RenderGL()
		{

		}
	}
}