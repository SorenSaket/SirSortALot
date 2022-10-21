using OpenTK.Windowing.GraphicsLibraryFramework;
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
        private static Query query_tashcanTransform = new Query().With<(Trashcan, Transform2D, Collider2DBox)>();
        private static Query query_tashableTransform = new Query().With<(Trashable, Transform2D, Collider2DBox)>();

        public static void Tashing(World world)
        {
            if (!world.HasResource(out KeyboardState keyboardState))
                return;

            var tashcans = world.Query(query_tashcanTransform);
            var tashable = world.Query(query_tashableTransform);

            //
            foreach (var can in tashcans)
            {
                var can_transform = can.Get<Transform2D>();
                var can_box = can.Get<Collider2DBox>();

                foreach (var trash in tashable)
                {
                    var trash_transform = trash.Get<Transform2D>();
                    var trash_box = trash.Get<Collider2DBox>();

                    if(Collider2DBox.IntersectsWith(can_box, can_transform, trash_box, trash_transform))
                    {
                        if(!trash.Has<Falling>())
                            trash.Add(new Falling(trash_transform));

                        trash_transform.Position = can_transform.Position;
                        trash.Set(trash_transform);
                    }
                }
            }
        }
    }
}
