using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{


    [StructLayout(LayoutKind.Sequential)]
    public struct Player
    {
        public Vector2 lastPos;
        public Vector2 direction;
        public ECSPointer placePreview;
        public ECSPointer item;
        public float dashTimer;

        public bool IsHoldingItem => item != ECSPointer.Default;

        public Player(ECSPointer placePreview) 
        {
            dashTimer = 0;
            lastPos = Vector2.Zero;
            direction = Vector2.Zero;
            this.placePreview = placePreview;
            item = ECSPointer.Default;
        }
    }
}
