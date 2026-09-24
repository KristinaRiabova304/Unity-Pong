namespace UnityLab.Components
{
    /// <summary>
    /// What Game Controller should do when the player touches an Obstacle.
    /// GameOver shows the lose panel. Respawn reloads the scene. Damage removes health.
    /// The Obstacle only reports this value; it never applies the result itself.
    /// </summary>
    public enum ObstacleResponse
    {
        GameOver,
        Respawn,
        Damage
    }
}
