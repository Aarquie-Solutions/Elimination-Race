using System;
using UnityEngine;

namespace ZombieElimination
{
    public interface IJump
    {
        void Jump(Vector3 destination, float jumpHeight, float jumpDuration, Action onComplete = null);
    }
}
