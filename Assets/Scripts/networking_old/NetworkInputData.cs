using UnityEngine;
using Fusion;

namespace networking
{
    public struct NetworkInputData : INetworkInput
    {
        public const byte MOUSEBUTTON1 = 0x01;

        public Vector2 direction;
    }
}

