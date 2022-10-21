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
    public class ConveyourField
    {
        public readonly int width;
        public readonly int height;
        public Vector2[] data;

        public ConveyourField(int width, int height)
        {
            data = new Vector2[width * height];
            this.width = width;
            this.height = height;
        }
    }


    internal static partial class Systems
    {
        private static Query query_conveyorTransform = new Query().With<(Conveyor, Transform2D)>();
        
        public static void ConveyorField(World world)
        {
            ConveyourField field = world.GetResource<ConveyourField>()!;
            if (field == null)
                return;

            // clear field
            for (int i = 0; i < field.data.Length; i++)
            {
                field.data[i] = Vector2.Zero;
            }

            var entities = world.Query(query_conveyorTransform);
            foreach (var entity in entities)
            {
                var tranform = entity.Get<Transform2D>();
                int x = (int) tranform.Position.X;
                int y = (int) tranform.Position.Y;

                // Only run widthin bounds
                if((x >= 0 && x < field.width) && (y >= 0 && y < field.height))
                {
                    var conveyor = entity.Get<Conveyor>();
                    int index = x + y * field.width;
                    field.data[index] += conveyor.direction;
                }
            }
        }
    }
}
