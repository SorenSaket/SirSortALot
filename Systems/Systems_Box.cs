using Saket.ECS;
using Saket.Engine;
using SirSortALot.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal static partial class Systems
    {
        

        private static Query query_ConveyormovableTransform = new Query().With<(ConveyorMovable, Transform2D)>();
        
        public static void ConveyorMovable(World world)
        {
            ConveyourField field = world.GetResource<ConveyourField>()!;
            if (field == null)
                return;

            var entities = world.Query(query_ConveyormovableTransform);
            foreach (var entity in entities)
            {
                var movable = entity.Get<ConveyorMovable>();

                if (movable.Frozen != 0)
                    continue;

                var tranform = entity.Get<Transform2D>();

                int x = (int)tranform.Position.X;
                int y = (int)tranform.Position.Y;

                // Only run widthin bounds
                if ((x >= 0 && x < field.width) && (y >= 0 && y < field.height))
                {
                    int index = x + y * field.width;
                    tranform.Position += field.data[index] * world.Delta;

                    entity.Set(tranform);
                }
            }
        }
    }
}