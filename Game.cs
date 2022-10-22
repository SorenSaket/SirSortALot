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
using System.Diagnostics;
using System.Runtime.InteropServices;
using NAudio.Wave;
using System.Threading;

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
            /*
            var audioFile = new AudioFileReader("Assets/Ludum Dare 38 - Track 5.wav");
            var outputDevice = new WaveOutEvent();
           
            outputDevice.Init(audioFile);
            outputDevice.Volume = 1f;
            outputDevice.Play();*/
           


            // Initialize Resource Manager
            {
                resources = new ResourceManager();

                resources.databases.Add(new DatabaseEmbedded(Assembly.GetExecutingAssembly()));
                resources.RegisterLoader(new LoaderShader());
                resources.RegisterLoader(new LoaderTexture());
                resources.RegisterLoader(new LoaderSheet());
            }

            ResourceKiosk kiosk = new();
            world = new World();

            // Msic resources
            {
                world.SetResource(KeyboardState);
                world.SetResource(MouseState);
                world.SetResource(new WindowInfo(base.Size.X, base.Size.Y));
                world.SetResource(new ConveyourField(32,32));
                world.SetResource(kiosk);
            }

            // ---- Texture Loading
            {
                float ppu = 256f;

                TextureGroups groups = new();

                // Game Texture
                var tex = resources.Load<Texture>("game");
                tex.LoadToGPU();
                tex.UnloadFromCPU();
                var sheet = new Sheet(8, 8, 1);
                groups.Add(tex, sheet);
                
                var tex_level = resources.Load<Texture>("level");
                tex_level.LoadToGPU();
                tex_level.UnloadFromCPU();
                var sheet_level = new Sheet(1, 1, 32);
                groups.Add(tex_level, sheet_level);


                var tex_items = resources.Load<Texture>("items");
                tex_items.LoadToGPU();
                tex_items.UnloadFromCPU();
                var sheet_items = new Sheet(8, 8, 1);
                groups.Add(tex_items, sheet_items);

                
                var tex_font = resources.Load<Texture>("font");
                tex_font.LoadToGPU();
                tex_font.UnloadFromCPU();
                var sheet_font = new Sheet(10, 10, 1);
                groups.Add(tex_font, sheet_font);

                world.SetResource(groups);
            }

            // ---- Animations ---- 
            {
                Animations anims = new Animations();
                anims.animations.Add(new int[] {0,1,2,3,4,5});
                world.SetResource(anims);
            }

            entity_camera = world.CreateEntity();
            entity_camera.Add(new Transform2D());
            entity_camera.Add(new CameraOrthographic(32, 0.1f, 100f));

            spriteRenderer = new SpriteRenderer(1000, entity_camera, resources.Load<Shader>("sprite") );
           

            // Create level
            {
                var background = world.CreateEntity();
                background.Add(new Sprite(1, 0, int.MaxValue));
                background.Add(new Transform2D(16.5f, 16.5f, -1f));

                var Wall = world.CreateEntity();
                Wall.Add(new Transform2D(16, 32f - 1f, -1f));
                Wall.Add(new Collider2DBox(new Vector2(32,3)));

                var trashcan = world.CreateEntity();
                trashcan.Add(new Sprite(0, 10, int.MaxValue));
                trashcan.Add(new Transform2D(27f, 27f, 0f));
                trashcan.Add(new Trashcan());
                trashcan.Add(new Collider2DBox(Vector2.One*0.2f, true));


                for (int i = 0; i < 24; i++)
                {
                    var teste = world.CreateEntity();
                    teste.Add(new Sprite(0, 2, int.MaxValue));
                    teste.Add(new Transform2D(3f + i, 27f));
                    teste.Add(new SpriteAnimator(0, 16f));
                    teste.Add(new Conveyor(Vector2.UnitX));
                }

                {
                    var spawner = world.CreateEntity();
                    spawner.Add(new Transform2D(3f, 27f));
                    spawner.Add(new BoxSpawner(0.4f));
                }

                // Spawn kiosk
                {
                    Vector2 position = new Vector2(16, 2);
                    float spacing = 0.5f;


                    Span<EntityPointer> pointers = stackalloc EntityPointer[3];

                    for (int i = 0; i < pointers.Length; i++)
                    {
                        var entity_item = world.CreateEntity();
                        entity_item.Add(new Transform2D(position.X- ((pointers.Length + spacing * i) / 2f) +i + spacing*i, position.Y));
                        entity_item.Add(new Sprite(2,0,int.MaxValue));
                        pointers[i] = entity_item.EntityPointer;
                    }

                    var entity_kiosk = world.CreateEntity();
                    entity_kiosk.Add(new Kiosk(pointers));
                    entity_kiosk.Add(new Transform2D(position.X, position.Y));
                }
            }
            
            var placePreview = world.CreateEntity();
            placePreview.Add(new Sprite(0, 6, int.MaxValue));
            placePreview.Add(new Transform2D(8, 8, 4));
            placePreview.Add(new MoveTowards(Vector2.Zero, 16f));

            var player = world.CreateEntity();
            player.Add(new Sprite(0, 9, int.MaxValue));
            player.Add(new Transform2D(8, 8,5));
            player.Add(new Player(placePreview.EntityPointer));
            player.Add(new ConveyorMovable());
            //player.Add(new Trashable());
            player.Add(new Collider2DBox(Vector2.One*0.9f));
            player.Add(new Velocity());
            player.Add(new MoveTowards(Vector2.Zero, 0));
            player.Add(new Friction(8f));


            // Update pipeline
            pipeline_update = new();
			Stage stage_update = new ();

            stage_update.Add(kiosk.Update);

            stage_update.Add(Camera.CameraSystem2D);
            stage_update.Add(Camera.CameraSystemOrthographic);
            
            stage_update.Add(Systems.Player);

            stage_update.Add(Systems.Spawner);
            stage_update.Add(Systems.Kiosk);
            stage_update.Add(Systems.ConveyorField);
            stage_update.Add(Systems.ConveyorMovable);


            stage_update.Add(Systems.Tashing);

            stage_update.Add(Systems.MoveTowards);

            stage_update.Add(Systems.Friction);
            stage_update.Add(Systems.Velocity);

            stage_update.Add(Systems.Camera);
            stage_update.Add(Systems.Falling);

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