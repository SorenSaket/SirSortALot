﻿using OpenTK.Windowing.GraphicsLibraryFramework;
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
        private static Query query_boxTransform = new Query().With<(Box, Transform2D)>();
        private static Query query_player = new Query().With<Player>();
        private static Query query_colliderTransform = new Query().With<(Collider2DBox, Transform2D)>();

        public static void Player(World world)
        {
            if (!world.HasResource(out KeyboardState keyboardState))
                throw new Exception("No keyboard!");

            var players = world.Query(query_player);


            foreach (var entity_player in players)
            {
                var transform_player = entity_player.Get<Transform2D>();
                var player = entity_player.Get<Player>();
                var collider_player = entity_player.Get<Collider2DBox>();

                // ---- Movement ----
                Vector2 movement = new Vector2();
                {
                    if (keyboardState.IsKeyDown(Keys.D))
                        movement.X += 1;
                    if (keyboardState.IsKeyDown(Keys.A))
                        movement.X -= 1;
                    if (keyboardState.IsKeyDown(Keys.W))
                        movement.Y += 1;
                    if (keyboardState.IsKeyDown(Keys.S))
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
                {
                    // Iterate all boxes
                    var boxes = world.Query(query_boxTransform);
                }
                else
                {
                    // else find closest item within range to pick up
                    var boxes = world.Query(query_boxTransform);
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
                if (keyboardState.IsKeyPressed(Keys.Space))
                {
                    // Try to pick up
                    if (player.item == EntityPointer.Default)
                    {
                        if (target != null)
                        {
                            target.Value.Set(new ConveyorMovable(true));
                            player.item = target.Value.EntityPointer;
                        }
                    }
                    // Try Release
                    else
                    {
                        var entity = world.GetEntity(player.item);
                        if (entity != null)
                        {
                            var t = entity.Value.Get<Transform2D>();
                            t.Position = placePosition;
                            t.Scale = Vector2.One;
                            entity.Value.Set(t);
                            entity.Value.Set(new ConveyorMovable(false));
                            player.item = EntityPointer.Default;
                        }
                    }
                }


                // ---- Move place preview ----
                {
                    Entity entity_placer = world.GetEntity(player.placePreview).GetValueOrDefault();

                    {
                        var transform_placer = entity_placer.Get<Transform2D>();
                        transform_placer.Position = placePosition;
                        entity_placer.Set(transform_placer);
                    }
                }


                // ---- Move item ----
                if (player.item != EntityPointer.Default)
                {
                    var entity = world.GetEntity(player.item);
                    if (entity != null)
                    {
                        var t = entity.Value.Get<Transform2D>();
                        t.Position = placePosition;
                        t.Scale = Vector2.One * +0.7f;
                        entity.Value.Set(t);
                    }
                }


                // ---- Apply position ---- 
                Vector2 targetVelocity = ((movement) * 5f);

                // If player would collide with another object return to original position
                var colliders = world.Query(query_colliderTransform);
                foreach (var item in colliders)
                {
                    if (item.EntityPointer == entity_player.EntityPointer)
                        continue;
                    if (item.EntityPointer == player.item)
                        continue;

                    var collider = item.Get<Collider2DBox>();
                    var transform_collider = item.Get<Transform2D>();

                    if(Collider2DBox.IntersectsWith(collider_player, transform_player, collider, transform_collider))
                    {
                        Vector2 relativePosition = (transform_collider.Position-player.lastPos);

                        Vector2 normal = Utils.RectNormal(relativePosition);
                        targetVelocity -=( normal ) * 5f;
                    }
                }
                player.lastPos = transform_player.Position;
                entity_player.Set(new Velocity(targetVelocity));
                entity_player.Set(player);
            }

        }
    }
}
