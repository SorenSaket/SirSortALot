using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Kiosk
    {
        // I would really like to use fixed size arrays here
        // but fixed cannot be used with custom structs for some reason

        // Marhsal is not an option either because it would have to use a managed array
        // (components must be fully unmanaged)

        public ECSPointer sprite1 = ECSPointer.Default;
        public ECSPointer sprite2 = ECSPointer.Default;
        public ECSPointer sprite3 = ECSPointer.Default;

        public ECSPointer this[int i]
        {
            get {
                switch (i)
                {
                    case 0:
                        return sprite1;
                    case 1:
                        return sprite2;
                    case 2:
                        return sprite3;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                switch (i)
                {
                    case 0:
                        sprite1 = value;
                        break;
                    case 1:
                        sprite2 = value;
                        break;
                    case 2:
                        sprite3 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Kiosk(Span<ECSPointer> sprites) 
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                this[i] = sprites[i];
            }
        }
    }
}