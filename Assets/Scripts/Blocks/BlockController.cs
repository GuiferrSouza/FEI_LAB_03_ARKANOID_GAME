using System;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public int points = 50;

    //----------------------------------------------------------------------------------------

    #region BREAK

    [Header("Break")]
    public ParticleSystem breakEffect;
    public AudioClip breakSound;

    public static event Action<int> Destroyed;
    private bool isBroken;

    //----------------------------------------------------------------------------------------

    protected void Break()
    {
        if (isBroken) return;
        isBroken = true;

        // Break effect.
        if (breakEffect != null)
        {
            breakEffect.transform.SetParent(null);
            breakEffect.Play();

            var main = breakEffect.main;
            var duration = main.duration + main.startLifetime.constantMax;
            Destroy(breakEffect.gameObject, duration);
        }

        // Break sound.
        // Uses PlayClipAtPoint because the block is destroyed immediately after breaking.
        if (breakSound != null) GameController.PlaySound(breakSound, 1f);

        Destroyed?.Invoke(points);
        Destroy(gameObject);
    }

    #endregion BREAK

    //----------------------------------------------------------------------------------------

    #region EVENTS

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball")) Break();
    }

    #endregion EVENTS
}