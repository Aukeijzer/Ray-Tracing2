using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

//The template provides you with a window which displays a 'linear frame buffer', i.e.
//a 1D array of pixels that represents the graphical contents of the window.

//Under the hood, this array is encapsulated in a 'Surface' object, and copied once per
//frame to an OpenGL texture, which is then used to texture 2 triangles that exactly
//cover the window. This is all handled automatically by the template code.

//Before drawing the two triangles, the template calls the Tick method in MyApplication,
//in which you are expected to modify the contents of the linear frame buffer.

//After (or instead of) rendering the triangles you can add your own OpenGL code.

//We will use both the pure pixel rendering as well as straight OpenGL code in the
//tutorial. After the tutorial you can throw away this template code, or modify it at
//will, or maybe it simply suits your needs.

namespace Template
{
	public class OpenTKApp : GameWindow
	{
		static int screenID;            //Unique integer identifier of the OpenGL texture.
		static MyApplication app;       //Instance of the application.
		static bool terminated = false; //Application terminates gracefully when this is true.
		static bool w_key = false;
		static bool a_key = false;
		static bool s_key = false;
		static bool d_key = false;
		static bool r_arrow = false;
		static bool l_arrow = false;
		static bool u_arrow = false;
		static bool d_arrow = false;

		protected override void OnLoad( EventArgs e )
		{
			//Called during application initialization.
			GL.Hint( HintTarget.PerspectiveCorrectionHint, HintMode.Nicest );
			ClientSize = new Size( 1024, 512 );
			app = new MyApplication();
			app.screen = new Surface( Width, Height );
			Sprite.target = app.screen;
			screenID = app.screen.GenTexture();
			app.Init();
		}

		protected override void OnUnload( EventArgs e )
		{
			//Called upon app close,
			GL.DeleteTextures( 1, ref screenID );
			Environment.Exit( 0 );      //Bypass wait for key on CTRL-F5.
		}

		protected override void OnResize( EventArgs e )
		{
			//Called upon window resize. Note: does not change the size of the pixel buffer.
			GL.Viewport( 0, 0, Width, Height );
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			GL.Ortho( -1.0, 1.0, -1.0, 1.0, 0.0, 4.0 );
		}

		protected override void OnUpdateFrame( FrameEventArgs e )
		{
			//Called once per frame; app logic.
			var keyboard = OpenTK.Input.Keyboard.GetState();
			if( keyboard[OpenTK.Input.Key.Escape] ) terminated = true;

			//Keys to be used for camera management (we could not get this part to function).
			if (keyboard[OpenTK.Input.Key.W]) w_key = true;
			if (keyboard[OpenTK.Input.Key.A]) a_key = true;
			if (keyboard[OpenTK.Input.Key.S]) s_key = true;
			if (keyboard[OpenTK.Input.Key.D]) d_key = true;
			if (keyboard[OpenTK.Input.Key.Right]) r_arrow = true;
			if (keyboard[OpenTK.Input.Key.Left]) l_arrow = true;
			if (keyboard[OpenTK.Input.Key.Up]) u_arrow = true;
			if (keyboard[OpenTK.Input.Key.Down]) d_arrow = true;
		}

		protected override void OnRenderFrame( FrameEventArgs e )
		{
			GL.ClearColor(0, 0, 0, 0);
			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.DepthTest);
			GL.Color3(1.0f, 1.0f, 1.0f);
			//Called once per frame; render.
			app.Tick();
			if( terminated )
			{
				Exit();
				return;
			}
			if (w_key)
            {
				//...
            }
			if (a_key)
			{
				//...
			}
			if (s_key)
			{
				//...
			}
			if (d_key)
			{
				//...
			}
			if (r_arrow)
			{
				//...
			}
			if (l_arrow)
			{
				//...
			}
			if (u_arrow)
			{
				//...
			}
			if (d_arrow)
			{
				//...
			}
			
			//Convert MyApplication.screen to OpenGL texture.
			GL.BindTexture( TextureTarget.Texture2D, screenID );
			GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
						   app.screen.width, app.screen.height, 0,
						   OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
						   PixelType.UnsignedByte, app.screen.pixels
						 );
			//Draw screen filling quad.
			GL.Begin( PrimitiveType.Quads );
			GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex2( -1.0f, -1.0f );
			GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex2( 1.0f, -1.0f );
			GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex2( 1.0f, 1.0f );
			GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex2( -1.0f, 1.0f );
			GL.End();
			
			
			//Prepare for generic OpenGL rendering.
			GL.Enable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Texture2D);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			GL.PushMatrix();
			app.RenderGL();
			GL.PopMatrix();

			//Tell OpenTK we're done rendering.
			SwapBuffers();
		}

		public static void Main( string[] args )
		{
			//Entry point.
			using( OpenTKApp app = new OpenTKApp() ) { app.Run( 30.0, 0.0 ); }
		}
	}
}