using OpenTK.Windowing.GraphicsLibraryFramework;
using Saket.ECS;
using Saket.Engine;
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
        private static Query query_boxTransform = new Query().With<(Box, Transform2D)>();
        private static Query query_player = new Query().With<Player>();
        private static Query query_colliderTransform = new Query().With<(Collider2DBox, Transform2D)>();

        public static void Player(World world)
        {
            if (!world.TryGetResource(out KeyboardState keyboardState))
                throw new Exception("No keyboard!");

            var players = world.Query(query_player);
            var boxes = world.Query(query_boxTransform);

            foreach (var entity_player in players)
            {
                var transform_player = entity_player.Get<Transform2D>();
                var player = entity_player.Get<Player>();
                var collider_player = entity_player.Get<Collider2DBox>();
                var player_velocity = entity_player.Get<Velocity>();


                player.dashTimer -= world.Delta;

                // ---- Movement ----
                Vector2 movement = new Vector2();
                {
                    if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                        movement.X += 1;
                    if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                        movement.X -= 1;
                    if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                        movement.Y += 1;
                    if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                        movement.Y -= 1;

                    if (movement.LengthSquared() != 0)
                    {
                        movement = Vector2.Normalize(movement);
                        player.direction = Utils.RectNormal(movement);
                    }
                }


                // ---- Decide place position ---- 
                Vector2 placePosition = (transform_player.Position + player.direction).Round();

                Entity? target = null;
                { 
                    // If we are holding an item find nearst avaiable spot within range to place the item we are holding
                    if (player.IsHoldingItem)
                    { /*
                        foreach (var entity_box in boxes)
                        {
                            var transform_box = entity_box.Get<Transform2D>();
                            
                        }*/
                    }
                    else
                    {
                        // else find closest item within range to pick up
                        foreach (var entity_box in boxes)
                        {
                            var transform_box = entity_box.Get<Transform2D>();
                            if ((target != null && Vector2.Distance(transform_box.Position, transform_player.Position) <
                                    Vector2.Distance(target.Value.Get<Transform2D>().Position, transform_player.Position)) ||

                                    (target == null && Vector2.Distance(transform_box.Position, transform_player.Position) < 2.5f))
                            {
                                target = entity_box;
                                placePosition = transform_box.Position;
                            }
                        }
                    }
                }

                // ---- Pickup / Place ----
                if (keyboardState.IsKeyPressed(Keys.Space) || keyboardState.IsKeyPressed(Keys.Z))
                {
                    // Try to pick up
                    if (player.item == EntityPointer.Default)
                    {
                        if (target != null)
                        {
                            target.Value.Set(new ConveyorMovable(true));
                            target.Value.Set(new Trashable(false));
                            player.item = target.Value.EntityPointer;
                        }
                    }
                    // Try Release
                    else
                    {
                        bool IsAnyOtherBoxInPosition()
                        {
                            foreach (var entity_box in boxes)
                            {
                                if(entity_box.EntityPointer == player.item)
                                       continue;
                                var transform_box = entity_box.Get<Transform2D>();
                                if (transform_box.Position == placePosition)
                                    return true;
                            }
                            return false;
                        }
                        if(world.TryGetEntity(player.item, out var entity) && !IsAnyOtherBoxInPosition())
                        {
                            
                            var t = entity.Get<Transform2D>();
                            t.Position = placePosition;
                            t.Scale = Vector2.One;
                            entity.Set(t);
                            entity.Set(new ConveyorMovable(false));
                            entity.Set(new Trashable(true));
                            //entity.Value.Add(new Trashable());
                            player.item = EntityPointer.Default;
                           
                        }
                    }
                }


                // ---- Move place preview ----
                {
                    Entity entity_placer = world.GetEntity(player.placePreview);
                    var sprite_placer = entity_placer.Get<Sprite>();
                    var moveTowards_placer = entity_placer.Get<MoveTowards>();
                    var transform_placer = entity_placer.Get<Transform2D>();

                    if (target != null)
                    {
                        moveTowards_placer.Target = placePosition;
                        sprite_placer.color = uint.MaxValue;
                    }
                    else
                    {
                        sprite_placer.color = uint.MinValue;
                        transform_placer.Position = transform_player.Position;
                        moveTowards_placer.Target = transform_player.Position;
                        entity_placer.Set(transform_placer);
                    }
                    entity_placer.Set(sprite_placer);
                    entity_placer.Set(moveTowards_placer);

                }


                // ---- Move item ----
                if (player.item != EntityPointer.Default)
                {
                    if (world.TryGetEntity(player.item, out var entity))
                    {
                        var t = entity.Get<Transform2D>();
                        t.Position = placePosition;
                        t.Scale = Vector2.One * +0.7f;
                        entity.Set(t);
                    }
                }


                // ---- Apply position ---- 
                Vector2 targetVelocity = ((movement));


                // Dash
                if((keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.X)) && player.dashTimer <= 0)
                {
                    targetVelocity *= 16;
                    player.dashTimer = 1f;
                }

                // If player would collide with another object return to original position
                var colliders = world.Query(query_colliderTransform);
                foreach (var item in colliders)
                {
                    if (item.EntityPointer == entity_player.EntityPointer)
                        continue;
                    if (item.EntityPointer == player.item)
                        continue;

                    var collider = item.Get<Collider2DBox>();

                    if (collider.IsTrigger)
                        continue;

                    var transform_collider = item.Get<Transform2D>();

                    if(Collider2DBox.IntersectsWith(collider_player, transform_player, collider, transform_collider))
                    {
                        if(collider.Size.X == collider.Size.Y)
                        {
                            Vector2 relativePosition = (transform_collider.Position - player.lastPos);
                            targetVelocity -= (Vector2.Normalize(relativePosition) * 1.8f);
                        }
                        else
                        {
                            Vector2 relativePosition = (transform_collider.Position - player.lastPos) / (collider.Size/2f);
                            Vector2 normal = Utils.RectNormal(relativePosition);
                            targetVelocity -= normal * 1.8f;
                        }
                      
                    }
                }
                
                player.lastPos = transform_player.Position;

                /*
                 if (entity_player.Get<MoveTowards>().speed != 0 && targetVelocity.Length() > 0f)
                 {
                     entity_player.Set(new MoveTowards(transform_player.Position + Vector2.Normalize(targetVelocity) * 0.1f, 20));
                 }
                 else
                 {

                 }*/
                player_velocity += targetVelocity;


                /*
                if()
                entity_player.Set(new MoveTowards(Vector2.Zero, 0));
                    else
                {
                    entity_player.Set(new MoveTowards((transform_player.Position + player.direction * 0.33f).Round(), 20));
                }*/


                entity_player.Set(player_velocity);
                entity_player.Set(player);
            }

        }
    }
}
