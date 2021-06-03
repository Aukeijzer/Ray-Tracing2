using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;

namespace Template
{
	public class Surface
	{
		public int width, height;
		public int[] pixels;
		static bool fontReady = false;
		static Surface font;
		static int[] fontRedir;

		//Surface constructor.
		public Surface(int w, int h)
		{
			width = w;
			height = h;
			pixels = new int[w * h];
		}

		//Surface constructor using a file.
		public Surface(string fileName)
		{
			Bitmap bmp = new Bitmap(fileName);
			width = bmp.Width;
			height = bmp.Height;
			pixels = new int[width * height];
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			System.Runtime.InteropServices.Marshal.Copy(data.Scan0, pixels, 0, width * height);
			bmp.UnlockBits(data);
		}

		//Create an OpenGL texture.
		public int GenTexture()
		{
			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
			return id;
		}

		//Clear the surface.
		public void Clear(int c)
		{
			for (int s = width * height, p = 0; p < s; p++) pixels[p] = c;
		}

		//Copy the surface to another surface/
		public void CopyTo(Surface target, int x = 0, int y = 0)
		{
			int src = 0;
			int dst = 0;
			int srcwidth = width;
			int srcheight = height;
			int dstwidth = target.width;
			int dstheight = target.height;
			if ((srcwidth + x) > dstwidth) srcwidth = dstwidth - x;
			if ((srcheight + y) > dstheight) srcheight = dstheight - y;
			if (x < 0)
			{
				src -= x;
				srcwidth += x;
				x = 0;
			}
			if (y < 0)
			{
				src -= y * width;
				srcheight += y;
				y = 0;
			}
			if ((srcwidth > 0) && (srcheight > 0))
			{
				dst += x + dstwidth * y;
				for (int v = 0; v < srcheight; v++)
				{
					for (int u = 0; u < srcwidth; u++) target.pixels[dst + u] = pixels[src + u];
					dst += dstwidth;
					src += width;
				}
			}
		}

		//Draw a circle.
		public void DrawCircle(float x, float y, float radius) //http://csharphelper.com/blog/2019/03/use-sines-and-cosines-to-draw-circles-and-ellipses-in-c/
		{
			float num_theta = 100;
			List<PointF> points = new List<PointF>();
			float dtheta = (float)(2 * Math.PI / num_theta);
			float theta = 0;
			for (int i = 0; i < num_theta; i++)
			{
				float x_new = x+(float)(radius * Math.Cos(theta));
				float y_new = y+(float)(radius * Math.Sin(theta));
				points.Add(new PointF(x_new, y_new));
				theta += dtheta;
			}
			for (int q = 0; q<points.Count; q++)
            {
				if (q+1 == points.Count)
                {
					Line((int)points[q].X, (int)points[q].Y, (int)points[0].X, (int)points[0].Y, 0x0000ff);
				}
                else
                {
					Line((int)points[q].X, (int)points[q].Y, (int)points[q + 1].X, (int)points[q + 1].Y, 0x0000ff);
				}
            }
		}

		//Draw a box.
		public void Box(int x1, int y1, int x2, int y2, int c)
		{
			int dest = y1 * width;
			for (int y = y1; y <= y2; y++, dest += width)
			{
				pixels[dest + x1] = c;
				pixels[dest + x2] = c;
			}
			int dest1 = y1 * width;
			int dest2 = y2 * width;
			for (int x = x1; x <= x2; x++)
			{
				pixels[dest1 + x] = c;
				pixels[dest2 + x] = c;
			}
		}

		//Draw a solid bar.
		public void Bar(int x1, int y1, int x2, int y2, int c)
		{
			int dest = y1 * width;
			for (int y = y1; y <= y2; y++, dest += width) for (int x = x1; x <= x2; x++)
				{
					pixels[dest + x] = c;
				}
		}

		//Helper function for line clipping.
		int OUTCODE(int x, int y)
		{
			int xmin = 0, ymin = 0, xmax = width - 1, ymax = height - 1;
			return (((x) < xmin) ? 1 : (((x) > xmax) ? 2 : 0)) + (((y) < ymin) ? 4 : (((y) > ymax) ? 8 : 0));
		}

		//Draw a line, clipped to the window.
		public void Line(int x1, int y1, int x2, int y2, int c)
		{
			int xmin = 0, ymin = 0, xmax = width - 1, ymax = height - 1;
			int c0 = OUTCODE(x1, y1), c1 = OUTCODE(x2, y2);
			bool accept = false;
			while (true)
			{
				if (c0 == 0 && c1 == 0) { accept = true; break; }
				else if ((c0 & c1) > 0) break;
				else
				{
					int x = 0, y = 0;
					int co = (c0 > 0) ? c0 : c1;
					if ((co & 8) > 0) { x = x1 + (x2 - x1) * (ymax - y1) / (y2 - y1); y = ymax; }
					else if ((co & 4) > 0) { x = x1 + (x2 - x1) * (ymin - y1) / (y2 - y1); y = ymin; }
					else if ((co & 2) > 0) { y = y1 + (y2 - y1) * (xmax - x1) / (x2 - x1); x = xmax; }
					else if ((co & 1) > 0) { y = y1 + (y2 - y1) * (xmin - x1) / (x2 - x1); x = xmin; }
					if (co == c0) { x1 = x; y1 = y; c0 = OUTCODE(x1, y1); }
					else { x2 = x; y2 = y; c1 = OUTCODE(x2, y2); }
				}
			}
			if (!accept) return;
			if (Math.Abs(x2 - x1) >= Math.Abs(y2 - y1))
			{
				if (x2 < x1) { int h = x1; x1 = x2; x2 = h; h = y2; y2 = y1; y1 = h; }
				int l = x2 - x1;
				if (l == 0) return;
				int dy = ((y2 - y1) * 8192) / l;
				y1 *= 8192;
				for (int i = 0; i < l; i++)
				{
					pixels[x1++ + (y1 / 8192) * width] = c;
					y1 += dy;
				}
			}
			else
			{
				if (y2 < y1) { int h = x1; x1 = x2; x2 = h; h = y2; y2 = y1; y1 = h; }
				int l = y2 - y1;
				if (l == 0) return;
				int dx = ((x2 - x1) * 8192) / l;
				x1 *= 8192;
				for (int i = 0; i < l; i++)
				{
					pixels[x1 / 8192 + y1++ * width] = c;
					x1 += dx;
				}
			}
		}

		//Plot a single pixel.
		public void Plot(int x, int y, int c)
		{
			if ((x >= 0) && (y >= 0) && (x < width) && (y < height))
			{
				pixels[x + y * width] = c;
			}
		}

		//Print a string.
		public void Print(string t, int x, int y, int c)
		{
			if (!fontReady)
			{
				font = new Surface("../../assets/font.png");
				string ch = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+={}[];:<>,.?/\\ ";
				fontRedir = new int[256];
				for (int i = 0; i < 256; i++) fontRedir[i] = 0;
				for (int i = 0; i < ch.Length; i++)
				{
					int l = (int)ch[i];
					fontRedir[l & 255] = i;
				}
				fontReady = true;
			}
			for (int i = 0; i < t.Length; i++)
			{
				int f = fontRedir[(int)t[i] & 255];
				int dest = x + i * 12 + y * width;
				int src = f * 12;
				for (int v = 0; v < font.height; v++, src += font.width, dest += width) for (int u = 0; u < 12; u++)
					{
						if ((font.pixels[src + u] & 0xffffff) != 0) pixels[dest + u] = c;
					}
			}
		}
		
		//Puts what is viewed through the camera of the scene in the screen.
		public void drawScene(Scene scene)
        {
			Camera camera = scene.camera;

			Vector3 origin = camera.E;					//Where the camera is situated.
			Vector3 direction = camera.p0-origin;		//This is the direction of the ray in the topright corner of the screen.
			Vector3 u=(camera.p1-camera.p0)/width;		//For each pixel to right the new direction of the ray is the direction of the old one + u.
			Vector3 v=(camera.p2-camera.p0)/height;		//Same as vector u but from top to bottom.

			//For each x and y update the pixel to what is seen in the scene.
			int i = 0;
			Primitive[] primitives = new Primitive[scene.primitives.Count];
			scene.primitives.CopyTo(primitives);

			for (int y=0;y<height;y++) for (int x = 0; x < width; x++)
            {
				//Make a new ray given the origin, direction, u, v.
				Ray pixelray = new Ray(origin, direction + x * u + y * v);
				Intersect intersection = scene.calcIntersection(pixelray,0,scene.primitives);

				//Check if intersection is made.
				if (intersection.intersection_made)
				{
					//Removing the object the ray hit.
					Primitive obj = intersection.obj;
					scene.primitives.Remove(intersection.obj);
					
					//Update color based on the color of the object.
					Vector3 color = intersection.obj.rgbcolor;

					//Check if the point of intersection is illuminated by a lightsource.
					bool light = false;

					foreach (Light lights in scene.lightSources)
					{
						Vector3 lightD = lights.pos - intersection.point;
						Ray lightr = new Ray(intersection.point, lightD);
						if (scene.calcIntersection(lightr,0,scene.primitives).r.t>lightD.Length)
						{
								light = true;
						}
					}
					if (light)
                    {
						pixels[i] = Primitive.Vec2intcolor(color);
					}
                    else
                    {
						pixels[i] = 0x000000;
                    }
					//Re-adding the removed object.
					scene.primitives.Add(obj);
				}

				//If no intersection is found the pixel will be set to white.
				else pixels[i] = 0xffffff; //Ray is saved with infinite distance traveled (no intersection).
				i++;
            }
        }

		//Draws the debug screen, giving a topdown view of of the plane horizontal to the direction of the camera in the scene.
		public void drawDebug(Scene scene)
        {
			Camera camera = scene.camera;

			Vector3 origin = camera.E;						//Where the camera is situated.
			Vector3 direction = camera.C - origin;			//this is the direction of the ray through the middle of the screen.
			Vector3 u = (camera.p1 - camera.p0) / width;	//for each pixel to right the new direction of the ray is the direction of the old one + u
			
			//When x=0. on the debugscreen this will create the line from the camera to the topleftcorner. In the scene this represents the ray going through the middle left of the screenplane.
			for (int x = 0; x < width; x ++)
			{
				Ray pixelray = new Ray(origin, direction + (x-width/2) * u);
				Intersect intersection = scene.calcIntersection(pixelray, 0,scene.primitives);
				Ray r;

				if (intersection.intersection_made && intersection.obj is Sphere)
				{
					r = new Ray(intersection.r.O, intersection.r.D, intersection.r.t); //Ray is saved with the distance traveled (intersection).
					float x1 = width / 2;
					float y1 = height;
					float x2 = (width / 2) + r.D.X * r.t * width/2/10;
					float y2 = (height) - r.D.Z* r.t * height/10 ;
					Line((int)x1, (int)y1, (int)x2, (int)y2, 0x00ff00);
						
				}
				else
				{
					r = pixelray; //Ray is saved with infinite distance traveled (no intersection).
					float x1 = width/2;
					float y1 = height;
					float x2 = (width / 2) + r.D.X * 100 * width / 2 / 10;
					float y2 = (height) - r.D.Z * 100 * height / 10;
					Line((int)x1, (int)y1, (int)x2, (int)y2, 0xff0000);
				}
			}
			foreach (Primitive primitive in scene.primitives)
            {
                if (primitive is Sphere s)
                {
                    float center_x = (s.pos.X * width / 2 / 10 + width / 2);
                    float center_y = height - (s.pos.Z * height / 10);
                    DrawCircle(center_x, center_y, s.r * width / 2 / 10);
                }
            }
				
			float half_screenplane = (float)(Math.Tan(0.5f * camera.fov) * camera.d)*width/10/2;
			Line((int)(width/2 -half_screenplane), (int)(height -camera.d*height/10), (int)(width/2 +half_screenplane), (int)(height -camera.d * height / 10), 0x0000ff);
		}
	}


	public class Sprite
	{
        readonly Surface bitmap;
		static public Surface target;
        readonly int textureID;
		
		//Sprite constructor.
		public Sprite(string fileName)
		{
			bitmap = new Surface(fileName);
			textureID = bitmap.GenTexture();
		}

		//Draw a sprite with scaling.
		public void Draw(float x, float y, float scale = 1.0f)
		{
			GL.BindTexture(TextureTarget.Texture2D, textureID);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Begin(PrimitiveType.Quads);
			float u1 = (x * 2 - 0.5f * scale * bitmap.width) / target.width - 1;
			float v1 = 1 - (y * 2 - 0.5f * scale * bitmap.height) / target.height;
			float u2 = ((x + 0.5f * scale * bitmap.width) * 2) / target.width - 1;
			float v2 = 1 - ((y + 0.5f * scale * bitmap.height) * 2) / target.height;
			GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(u1, v2);
			GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(u2, v2);
			GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(u2, v1);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(u1, v1);
			GL.End();
			GL.Disable(EnableCap.Blend);
		}
	}
}
