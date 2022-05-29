using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Multiplayer
{
    [CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObject/Player/PlayerScriptableObject", order = 1)]
    public class PlayerScritableObject : ScriptableObject
    {
        [Header("SHOOTING")]
        public Vector2 shootingVectorCorrection;
        public float shootingForce;

    }
}

