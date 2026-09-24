using UnityEngine;

namespace UnityLab.Components
{
    /// <summary>
    /// Information sent when something touches an Obstacle.
    /// Game Controller reads this and decides what to do.
    /// </summary>
    public readonly struct ObstacleHit
    {
        public readonly Obstacle Obstacle;
        public readonly GameObject Target;
        public readonly ObstacleResponse Response;
        public readonly int DamageAmount;

        public ObstacleHit(Obstacle obstacle, GameObject target, ObstacleResponse response, int damageAmount)
        {
            Obstacle = obstacle;
            Target = target;
            Response = response;
            DamageAmount = damageAmount;
        }
    }
}
