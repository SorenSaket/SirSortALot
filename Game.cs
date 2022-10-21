using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Resources.Databases;
using Saket.Engine.Resources.Loaders;
using Saket.Engine.Components;
using SirSortALot.Components;

namespace SirSortALot
{
    internal class Game : Application
    {
        World world;

        SpriteRenderer spriteRenderer;

        Pipeline pipeline_update;
        Pipeline pipeline_render;

        Entity entity_camera;
        ResourceManager resources;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            
        }
        
        protected override void OnLoad()
        {
            // Initialize Resource Manager
            resources = new ResourceManager();
            resources.databases.Add(new DatabaseEmbedded(Assembly.GetExecutingAssembly()));
            resources.RegisterLoader(new LoaderShader());
            resources.RegisterLoader(new LoaderTexture());
            resources.RegisterLoader(new LoaderSheet());

            world = new World();

            {
                world.SetResource(KeyboardState);
                world.SetResource(MouseState);
                world.SetResource(new WindowInfo(base.Size.X, base.Size.Y));
                world.SetResource(new ConveyourField(32,32));
            }

            {
                float ppu = 256f;

                TextureGroups groups = new();

                // Game Texture
                var tex = resources.Load<Texture>("game");
                tex.LoadToGPU();
                var sheet = new Sheet(8, 8, 1);
                groups.Add(tex, sheet);
                
                var tex_level = resources.Load<Texture>("level");
                tex_level.LoadToGPU();
                var sheet_level = new Sheet(1, 1,16);
                groups.Add(tex_level, sheet_level);


                var tex_items = resources.Load<Texture>("items");
                tex_items.LoadToGPU();
                var sheet_items = new Sheet(8, 8, 1);
                groups.Add(tex_items, sheet_items);

                /*
                var texfont = resources.Load<Texture>("font");
                texfont.LoadToGPU();
                var sheetfont = resources.Load<Sheet>("font");
                groups.Add(texfont, sheetfont);*/

                world.SetResource(groups);
            }

            {
                Animations anims = new Animations();
                anims.animations.Add(new int[] {0,1,2,3,4,5});
                world.SetResource(anims);
            }

            entity_camera = world.CreateEntity();
            entity_camera.Add(new Transform2D());
            entity_camera.Add(new CameraOrthographic(24, 0.1f, 100f));

            spriteRenderer = new SpriteRenderer(1000, entity_camera, resources.Load<Shader>("sprite") );
            
            
      
            // Create level
            {
                var background = world.CreateEntity();
                background.Add(new Sprite(1, 0, int.MaxValue));
                background.Add(new Transform2D(8, 8, -1f));

                var trashcan = world.CreateEntity();
                trashcan.Add(new Sprite(0, 10, int.MaxValue));
                trashcan.Add(new Transform2D(14.5f, 14.5f, 0f));
                trashcan.Add(new Trashcan());
                trashcan.Add(new Collider2DBox(Vector2.One*0.2f));


                for (int i = 0; i < 13; i++)
                {
                    var teste = world.CreateEntity();
                    teste.Add(new Sprite(0, 2, int.MaxValue));
                    teste.Add(new Transform2D(+1.5f + i, 6.5f+8f));
                    teste.Add(new SpriteAnimator(0, 16f));
                    teste.Add(new Conveyor(Vector2.UnitX));
                }

                {
                    var spawner = world.CreateEntity();
                    spawner.Add(new Transform2D(0.5f, 14.5f ));
                    spawner.Add(new BoxSpawner(0.4f));
                }
            }
            var placePreview = world.CreateEntity();
            placePreview.Add(new Sprite(0, 6, int.MaxValue));
            placePreview.Add(new Transform2D(8, 8, 4));

            var player = world.CreateEntity();
            player.Add(new Sprite(0, 9, int.MaxValue));
            player.Add(new Transform2D(8, 8));
            player.Add(new Player(placePreview.EntityPointer));
            player.Add(new ConveyorMovable());
            player.Add(new Trashable());
            player.Add(new Collider2DBox(Vector2.One*0.9f));
            player.Add(new Velocity());


            // Update pipeline
            pipeline_update = new();
			Stage stage_update = new ();
            stage_update.Add(Camera.CameraSystem2D);
            stage_update.Add(Camera.CameraSystemOrthographic);
            stage_update.Add(Systems.Player);

            
            stage_update.Add(Systems.Spawner);
            stage_update.Add(Systems.ConveyorField);
            stage_update.Add(Systems.ConveyorMovable);
            stage_update.Add(Systems.Falling);
            stage_update.Add(Systems.Tashing);
            stage_update.Add(Systems.Velocity);
            stage_update.Add(Systems.Camera);


            pipeline_update.AddStage(stage_update);

			// Render pipeline
			pipeline_render = new();
			Stage stage_render = new ();
            stage_render.Add(spriteRenderer.SystemSpriteAnimation);
            stage_render.Add(spriteRenderer.SystemSpriteRenderer);
            pipeline_render.AddStage(stage_render);
		}

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;

			pipeline_update.Update(world, delta);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            GL.ClearColor(1f, 0, 0, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            pipeline_render.Update(world, delta);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {   
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}