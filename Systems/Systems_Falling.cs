using OpenTK.Windowing.GraphicsLibraryFramework;
using Saket.ECS;
using Saket.Engine;
using SirSortALot.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal static partial class Systems
    {
        private static Query query_fallingTransform = new Query().With<(Falling, Transform2D)>();

        public static void Falling(World world)
        {
            const float turnSpeed = 6f;

            var entities = world.Query(query_fallingTransform);

            foreach (var entity in entities)
            {
                var transform = entity.Get<Transform2D>();
                var falling = entity.Get<Falling>();

                falling.timer += world.Delta;

                if (falling.timer >= 1f)
                {
                    entity.Destroy();
                    continue;
                }

                transform.rx = falling.baseTransform.rx + falling.timer * turnSpeed;
                transform.Scale = falling.baseTransform.Scale * (1f - falling.timer);

                entity.Set(transform);
                entity.Set(falling);
            }
        }
    }
}
