using UnityEngine;

/// <summary>
/// Interface to control an entity by a player
/// </summary>
public interface IPuppet
{
    /// <summary>
    /// Moves puppet
    /// </summary>
    /// <param name="moveToward">Magnitude should be <= 1 </param>
    public void Move(Vector2 moveDelta);

    /// <summary>
    /// Turn puppet character towards specified direction
    /// </summary>
    /// <param name="faceToward"></param>
    /// <returns>Returns angle away from target after turning</returns>
    public float FaceToward(Vector2 faceToward);

    /// <summary>
    /// Shoots in the current facing direction
    /// </summary>
    public void Shoot();

    /// <summary>
    /// Dashes in the current facing direction
    /// </summary>
    public void Dash();

    /// <summary>
    /// Disables any controls and activity
    /// </summary>
    public void Suspend();

    /// <summary>
    /// Re-enables any controls and activity
    /// </summary>
    public void Resume();

    /// <summary>
    /// Destroys the object
    /// </summary>
    public void Destroy();
}