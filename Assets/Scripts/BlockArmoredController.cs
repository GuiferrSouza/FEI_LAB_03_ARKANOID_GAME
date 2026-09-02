using UnityEngine;

public class BlockArmoredController : BlockController
{
    public AudioClip collisionSound;

    //----------------------------------------------------------------------------------------

    #region EVENTS

    private void OnEnable() => PaddleController.WallImpact += Break;
    private void OnDisable() => PaddleController.WallImpact -= Break;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        // Armored blocks are not destroyed by ball collisions.
        AudioSource.PlayClipAtPoint(collisionSound, transform.position, 1f);
    }

    #endregion EVENTS
}