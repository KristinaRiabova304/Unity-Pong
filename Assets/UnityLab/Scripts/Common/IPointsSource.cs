using System;

namespace UnityLab.Components
{
    /// <summary>
    /// Raises <see cref="PointsAwarded"/> whenever this object should add points to the score.
    /// Score Manager listens automatically, so gameplay components do not need a Score Manager reference.
    /// </summary>
    public interface IPointsSource
    {
        event Action<int> PointsAwarded;
    }
}
