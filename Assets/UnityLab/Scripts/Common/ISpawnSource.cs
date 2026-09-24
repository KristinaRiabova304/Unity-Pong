using System;
using UnityEngine;

namespace UnityLab.Components
{
    /// <summary>
    /// Raises <see cref="Spawned"/> whenever a new GameObject is created at runtime.
    /// Score Manager and Target Counter listen so they can track newly spawned objects.
    /// </summary>
    public interface ISpawnSource
    {
        event Action<GameObject> Spawned;
    }
}
