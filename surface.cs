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
		// surface constructor
		public Surface(int w, int h)
		{
			width = w;
			height = h;
			pixels = new int[w * h];
		}
		// surface constructor using a file
		public Surface(string fileName)
		{
			Bitmap bmp = new Bitmap(fileName);
			width = bmp.Width;
			height = bmp.Height;
			pixels = new int[width * height];
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			IntPtr ptr = data.Scan0;
			System.Runtime.InteropServices.Marshal.Copy(data.Scan0, pixels, 0, width * height);
			bmp.UnlockBits(data);
		}
		// create an OpenGL texture
		public int GenTexture()
		{
			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
			return id;
		}
		// clear the surface
		public void Clear(int c)
		{
			for (int s = width * height, p = 0; p < s; p++) pixels[p] = c;
		}
		// copy the surface to another surface
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
		// draw a rectangle

		public static void DrawCircle(float x, float y, float radius, Color4 c) //https://stackoverflow.com/questions/46130413/opentk-draw-transparent-circle
		{
			GL.Enable(EnableCap.Blend);
			GL.Begin(PrimitiveType.TriangleFan);
			GL.Color4(c);

			GL.Vertex2(x, y);
			for (int i = 0; i < 360; i++)
			{
				GL.Vertex2(x + Math.Cos(i) * radius, y + Math.Sin(i) * radius);
			}

			GL.End();
			GL.Disable(EnableCap.Blend);
		}

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
		// draw a solid bar
		public void Bar(int x1, int y1, int x2, int y2, int c)
		{
			int dest = y1 * width;
			for (int y = y1; y <= y2; y++, dest += width) for (int x = x1; x <= x2; x++)
				{
					pixels[dest + x] = c;
				}
		}
		// helper function for line clipping
		int OUTCODE(int x, int y)
		{
			int xmin = 0, ymin = 0, xmax = width - 1, ymax = height - 1;
			return (((x) < xmin) ? 1 : (((x) > xmax) ? 2 : 0)) + (((y) < ymin) ? 4 : (((y) > ymax) ? 8 : 0));
		}
		// draw a line, clipped to the window
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
		// plot a single pixel
		public void Plot(int x, int y, int c)
		{
			if ((x >= 0) && (y >= 0) && (x < width) && (y < height))
			{
				pixels[x + y * width] = c;
			}
		}
		// print a string
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
		int TX(float x)
		{
			x++;
			return (int) (x / 2 * width);
		}
		int TY(float y)
        {
			y = -y;
			y++;
			return (int) (y / 2 * height);
        }
		//puts what is viewed through the camera of the scene in the screen.
		public void drawscene(scene scene)
        {
			camera camera = scene.camera;
			Vector3 origin = camera.E; //where the camera is situated
			Vector3 direction = camera.p0-origin; //this is the direction of the ray in the topright corner of the screen
			Vector3 u=(camera.p1-camera.p0)/width; //for each pixel to right the new direction of the ray is the direction of the old one + u
			Vector3 v=(camera.p2-camera.p0)/height; //same as vector u but from top to bottom
			//for each x and y update the pixel to what is seen in the scene.
			int i = 0;
			for (int y=0;y<height;y++) for (int x = 0; x < width; x++)
            {
				//make a new ray given the origin,direction,u,v
				ray pixelray = new ray(origin, direction + x * u + y * v);

					

				intersect intersection = scene.calcIntersection(pixelray,0);
				//check if intersection is made
				if (intersection.intersection_made)
				{
					//Vector3 tempVect = new Vector3(intersection.point - intersection.r.O);
						
					//pixelsRayAll.Add(new ray(pixelray.O, pixelray.D, (float)Math.Sqrt(Math.Pow(tempVect.X,2) + (Math.Pow(tempVect.Y, 2)) + (Math.Pow(tempVect.Z, 2))))); //ray is saved with the distance traveled (intersection)

					//update color based on the color of the object
					Vector3 color = intersection.obj.rgbcolor;
					pixels[i] = primitive.vec2intcolor(color);
						
					//check if the point of intersection is illuminated by a lightsource
					foreach (light lightray in scene.lightSources)
					{
						ray temp = new ray(intersection.point, lightray.pos - intersection.point);
						if (scene.calcIntersection(temp,0).intersection_made == true)
						{
							color = new Vector3(0, 0, 0);
						}
					}

				}
				// if no intersection is found the pixel will be set to white
				else pixels[i] = 0xffffff; //ray is saved with infinite distance traveled (no intersection)
				i++;
            }
        }
		//draws the debug screen, giving a topdown view of of the plane horizontal to the direction of the camera in the scene.
		public void drawdebug(scene scene)
        {
			camera camera = scene.camera;
			Vector3 origin = camera.E; //where the camera is situated
			Vector3 direction = camera.C - origin; //this is the direction of the ray through the middle of the screen
			Vector3 u = (camera.p1 - camera.p0) / width; //for each pixel to right the new direction of the ray is the direction of the old one + u
			//when x=0 on the debugscreen this will create the line from the camera to the toprightcorner. In the scene this represents the ray going through the middle left of the screenplane.
			for (int x = 0; x < width; x += 2)
			{
				ray pixelray = new ray(origin, direction + (x-width/2) * u);
				if (x == 512)
                {
					int cum = 69;
                }
				intersect intersection = scene.calcIntersection(pixelray, 0);
				ray r;
				if (intersection.intersection_made)
				{
					Vector3 tempVect = new Vector3(intersection.point - intersection.r.O);
					r = new ray(pixelray.O, pixelray.D, tempVect.Length); //ray is saved with the distance traveled (intersection)
					float x1 = width / 2;
					float y1 = height;
					float full_length = 10*new Vector2(((float)x)/width-0.5f, 1).Length;
					float x2 = (width / 2) + (x - width / 2) * (r.t / full_length);
					float y2 = height*(1-(r.t / full_length));
					Line((int)x1, (int)y1, (int)x2, (int)y2, 0x00ff00); //groen
				}
				else
				{
					r = pixelray; //ray is saved with infinite distance traveled (no intersection)
					float x1 = width/2;
					float y1 = height;
					float x2 = x;
					float y2 = 0;
					Line((int)x1, (int)y1, (int)x2, (int)y2, 0xff0000); //rood
				}
			}
			foreach (primitive primitive in scene.primitives)
            {
				if (primitive is sphere)
                {
					sphere s = (sphere)primitive;
					//float center_x = (s.pos.X -origin.X)*;
					float center_y = (s.pos.Z * -(512f / 10f) + 512);
					//DrawCircle(center_x, center_y, s.r, new Color4(255, 0, 0, 100)); //... werkt dit? nee
				}
			}
				
			float half_screenplane = (float)(Math.Tan(0.5f * camera.fov) * camera.d);

			Line((int)(256 -half_screenplane), (int)(512 -camera.d), (int)(256 +half_screenplane), (int)(512 -camera.d), 255); //draws screenplane (255 = blue)
		}
	}



	public class Sprite
	{
		Surface bitmap;
		static public Surface target;
		int textureID;
		// sprite constructor
		public Sprite(string fileName)
		{
			bitmap = new Surface(fileName);
			textureID = bitmap.GenTexture();
		}
		// draw a sprite with scaling
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
