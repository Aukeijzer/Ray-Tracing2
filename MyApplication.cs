using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
	//Class that initializes the program upon load.
	public class MyApplication
	{
		public Surface screen, debugScreen, rtScreen;
		public Camera camera;
		public List<Ray> pixelsRayAll = new List<Ray>();
        readonly Scene scene = new Scene(); //Creates a new scene upon load.
		
		public void Init()
		{
			debugScreen = new Surface(512, 512);
			debugScreen.Clear(0x000000);
			rtScreen = new Surface(512, 512);
			rtScreen.Clear(0xffffff);
		}

		//Function that renders one frame (called every frame in the OnRenderFrame() function).
		public void Tick()
		{
			rtScreen.drawScene(Scene);
			rtScreen.CopyTo(screen, 0, 0);
			debugScreen.drawDebug(Scene);
			debugScreen.CopyTo(screen, 512, 0);
		}
	}
}