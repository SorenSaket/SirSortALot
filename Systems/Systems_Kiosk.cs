
using OpenTK.Windowing.GraphicsLibraryFramework;
using Saket.ECS;
using Saket.Engine;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal static partial class Systems
    {
        private readonly static float range_kiosk = 3.5f;

        private static Query query_kisok = new Query().With<(Kiosk, Transform2D)>();

        public static void Kiosk(World world)
        {
            if (!world.HasResource(out ResourceKiosk resourceKiosk))
                return;

            var entities = world.Query(query_kisok);
           
            var players = world.Query(query_player);

            foreach (var entity in entities)
            {
                var kiosk = entity.Get<Kiosk>();
                var transform_kiosk = entity.Get<Transform2D>();

                // Update the sprites to match orders
                for (int i = 0; i < 3; i++)
                {
                    var entity_sprite = world.GetEntity(kiosk[i]).GetValueOrDefault();
                    var sprite_sprite = entity_sprite.Get<Sprite>();

                    if(i >= resourceKiosk.orders.Count)
                    {
                        sprite_sprite.color = uint.MinValue;
                    }
                    else
                    {
                        sprite_sprite.spr = resourceKiosk.orders[i].ID;
                        sprite_sprite.color = uint.MaxValue;
                    }

                    entity_sprite.Set(sprite_sprite);
                }

                // If a player is clone with a requied item on their hand take the item.
                foreach (var entity_player in players)
                {
                    var player = entity_player.Get<Player>();
                    var transform_player = entity_player.Get<Transform2D>();

                    if (Vector2.Distance(transform_player.Position, transform_kiosk.Position) <= range_kiosk)
                    {
                        if (player.IsHoldingItem)
                        {
                            var entity_item = world.GetEntity(player.item).GetValueOrDefault();
                            var item = entity_item.Get<Box>();

                            if (resourceKiosk.orders.FirstOrFalse(x =>x.ID == item.id, out var order))
                            {
                                resourceKiosk.orders.Remove(order);
                                entity_item.Destroy();
                                player.item = EntityPointer.Default;
                            }
                        }
                        entity_player.Set(player);
                    }
                }

                
                entity.Set(kiosk);
            }

        }
    }
}
