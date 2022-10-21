using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Velocity
    {
        public Vector2 value;

        public Velocity(Vector2 value)
        {
            this.value = value; 
        }

        public static implicit operator Velocity(Vector2 v) => new Velocity(v);
        public static implicit operator Vector2(Velocity v) => v.value;
    }
}
