using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;
		// initialize
		Surface map;
		float[,] h;
		float[] vertexData;
		public void Init()
		{
			map = new Surface("../../assets/coin.png");
			h = new float[256, 256];
			for (int y = 0; y < 256; y++) for (int x = 0; x < 256; x++)
					h[x, y] = ((float)(map.pixels[x + y * 256] & 255)) / 256;
			vertexData = new float[255 * 255 * 2 * 3 * 3];
		}
		float[] y = { -0.5f, -0.5f, 0.5f, 0.5f };
		float[] x = { -0.5f, 0.5f, 0.5f, -0.5f };
		// tick: renders one frame
		public void Tick()
		{
			screen.Clear( 0 );
			//screen.Print( "hello world", 2, 2, 0xffffff );
			//screen.Line(2, 20, 160, 20, 0xff0000);

			for (int i=0; i<4;i++)
            {
				float rx= (float)(x[i] * Math.Cos(0.01) - y[i] * Math.Sin(0.01));
				float ry= (float)(x[i] * Math.Sin(0.01) + y[i] * Math.Cos(0.01));
				x[i] = rx; y[i] = ry;
			}
			for (int i=0; i<4;i++)
            {
				screen.Line(TX(x[i]), TY(y[i]), TX(x[(i + 1) % 4]), TY(y[(i + 1) %4]), 0xffffff);
            }
		}
		//returns the color in one int.
		static int MixColor(int red, int green, int blue)
		{
			return (red << 16) + (green << 8) + blue;
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
		int a = 0;
		public void RenderGL()
		{
			/*
			var M = Matrix4.CreatePerspectiveFieldOfView(1.6f, 1.3f, .1f, 1000);
			GL.LoadMatrix(ref M);
			GL.Translate(0, 0, -1);
			GL.Rotate(110, 1, 0, 0);
			GL.Rotate(a * 30 / 3.141593, 0, 0, 1);
			a++;
/*
			GL.Color3(1.0f, 0.0f, 0.0f);
			GL.Begin(PrimitiveType.Triangles);
			GL.Vertex3(-0.5f, -0.5f, 0);
			GL.Vertex3(0.5f, -0.5f, 0);
			GL.Vertex3(-0.5f, 0.5f, 0);
*/

			for (int y = 0; y < 256; y++) for (int x = 0; x < 256; x++)
				{
					float gx = ((float) x) / 256;
					float gy = ((float) y) / 256;
					GL.Color3(h[x, y], h[x, y], h[x, y]);
					GL.Begin(PrimitiveType.Triangles);
					GL.Vertex3(gx, gy, h[x, y]);
					GL.Vertex3(gx+(1.0/125), gy, h[x, y]);
					GL.Vertex3(gx, gy + (1.0 / 125), h[x, y]);
					GL.End();
					GL.Begin(PrimitiveType.Triangles);
					GL.Vertex3(gx + (1 / 125), gy + (1 / 125), h[x, y]);
					GL.Vertex3(gx, gy + (1 / 125), h[x, y]);
					GL.Vertex3(gx + (1 / 125), gy + (1 / 125), h[x, y]);
					GL.End();
	
				}



			GL.End();


			
		}


	}
}