
using OpenTK.Windowing.GraphicsLibraryFramework;
using Saket.ECS;
using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal static partial class Systems
    {
        private static Query query_cameraTransform = new Query().With<CameraOrthographic>().With<Transform2D>();

        public static void Camera(World world)
        {
            Vector2 min =  new Vector2(-12.5f - 8 + 4, -17.5f -8+2);
            Vector2 max = new Vector2(-4.5f-8-4, 0.5f - 8-2);


            var players = world.Query(query_player);
            if (players.Count <= 0)
                return; 


            Vector2 avgPos = Vector2.Zero;

            foreach (var entity in players)
            {
                avgPos -= entity.Get<Transform2D>().Position;
            }
            avgPos /= players.Count;

            var cameras = world.Query(query_cameraTransform);
            
            foreach (var entity in cameras)
            {
                var transform = entity.Get<Transform2D>();

                transform.Position = Vector2.Clamp( Vector2.Lerp(transform.Position, avgPos, world.Delta), min, max);

                entity.Set(transform);

                //Debug.WriteLine(entity.Get<CameraOrthographic>().WorldToScreenPoint(avgPos.ToVector3XY()));
            }

        }
    }
}
