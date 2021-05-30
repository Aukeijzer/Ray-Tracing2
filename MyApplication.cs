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
		int VBO;
		float[,] h;
		float[] vertexData;
		public void Init()
		{
			map = new Surface("../../assets/coin.png");
			h = new float[256, 256];
			for (int y = 0; y < 256; y++) for (int x = 0; x < 256; x++)
					h[x, y] = (((float)(map.pixels[x + y * 256] & 255)) / 256)*-0.5f;
			vertexData = new float[256 * 256 * 2 * 3 * 3];
			for (int x = 0; x < 256; x++) for (int y = 0; y < 256; y++)
				{
					int i = (x + y * 256) * 18;
					float gx = ((float)x) / 256 - 0.5f;
					float gy = ((float)y) / 256 - 0.5f;
					float[] cum = {
						gx,gy,h[x, y],
						(float)(gx + (1.0 / 256)),gy,h[x, y],
						gx, (float)(gy + (1.0 / 256)),h[x, y],

						(float)(gx + (1.0 / 256)), (float)(gy + (1.0 / 256)),h[x, y],
						gx,(float)(gx + (1.0 / 256)),h[x, y],
						(float)(gx + (1.0 / 256)),(float)(gx + (1.0 / 256)),h[x, y]
					};
					Array.Copy(cum, 0, vertexData, i, 18);
				}

			VBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
			GL.BufferData<float>(BufferTarget.ArrayBuffer,
								vertexData.Length * 4,
								vertexData,
								BufferUsageHint.StaticDraw
								);
			GL.EnableClientState(ArrayCap.VertexArray); GL.VertexPointer(3, VertexPointerType.Float, 12, 0);
			GL.VertexPointer(3, VertexPointerType.Float, 12, 0);

		}
		float[] y = { -0.5f, -0.5f, 0.5f, 0.5f };
		float[] x = { -0.5f, 0.5f, 0.5f, -0.5f };
		// tick: renders one frame
		public void Tick()
		{
			screen.Clear( 0 );
			screen.Print( "fps: 30", 2, 2, 0xffffff );
			//screen.Line(2, 20, 160, 20, 0xff0000);

			for (int i=0; i<4;i++)
            {
				float rx= (float)(x[i] * Math.Cos(0.1) - y[i] * Math.Sin(0.1));
				float ry= (float)(x[i] * Math.Sin(0.1) + y[i] * Math.Cos(0.1));
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
			*/
			/*
			GL.Color3(1.0f, 0.0f, 0.0f);
			GL.Begin(PrimitiveType.Triangles);
			GL.Vertex3(-0.5f, -0.5f, 0);
			GL.Vertex3(0.5f, -0.5f, 0);
			GL.Vertex3(-0.5f, 0.5f, 0);
			*/
			/*
						for (int y = 0; y < 256; y++) for (int x = 0; x < 256; x++)
							{
								float gx = ((float) x) / 256-0.5f;
								float gy = ((float) y) / 256-0.5f;
								GL.Color3(h[x, y], h[x, y], h[x, y]);
								GL.Begin(PrimitiveType.Triangles);
								GL.Vertex3(0,gx, gy);
								GL.Vertex3(0,gx+(1.0/128), gy);
								GL.Vertex3(0,gx, gy + (1.0 / 128));
								GL.End();
								GL.Begin(PrimitiveType.Triangles);
								GL.Vertex3(0,gx + (1 / 125), gy + (1 / 125));
								GL.Vertex3(0,gx, gy + (1 / 125));
								GL.Vertex3(0,gx + (1 / 125), gy + (1 / 125));
								GL.End();

							}
			*/
			//GL.Color3(1.0f, 0.0f, 0.0f);
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBO); GL.DrawArrays(PrimitiveType.Triangles, 0, 255 * 255 * 2 * 3);
			GL.DrawArrays(PrimitiveType.Triangles, 0, 255 * 255 * 2 * 3);
			GL.End();


			
		}


	}
}