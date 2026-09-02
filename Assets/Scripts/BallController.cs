using UnityEngine;

public class BallController : MonoBehaviour
{
    private AudioSource audioSource;

    //----------------------------------------------------------------------------------------

    #region MOVEMENT

    [Header("Movement")]
    private Rigidbody2D rb;
    public float speed = 8f;
    public float minHorizontalDirection = 0.25f;
    public float minVerticalDirection = 0.25f;
    private float lastHorizontalDirection = 1f;

    //----------------------------------------------------------------------------------------

    public void Launch()
    {
        var direction = new Vector2(Random.Range(-0.75f, 0.75f), 1f).normalized;
        if (Mathf.Abs(direction.x) < minHorizontalDirection)
            direction.x = Random.value < 0.5f ? -minHorizontalDirection : minHorizontalDirection;

        lastHorizontalDirection = Mathf.Sign(direction.x);
        rb.linearVelocity = direction.normalized * speed;
    }

    private void NormalizeVelocity()
    {
        if (rb.linearVelocity.sqrMagnitude <= 0f) return;

        var direction = rb.linearVelocity.normalized;
        // Stores the last valid horizontal direction.
        if (Mathf.Abs(direction.x) >= minHorizontalDirection)
            lastHorizontalDirection = Mathf.Sign(direction.x);

        // Prevents almost vertical movement.
        if (Mathf.Abs(direction.x) < minHorizontalDirection)
            direction.x = lastHorizontalDirection * minHorizontalDirection;

        // Prevents almost horizontal movement.
        if (Mathf.Abs(direction.y) < minVerticalDirection)
        {
            var verticalDirection = Mathf.Sign(direction.y);
            if (verticalDirection == 0f) verticalDirection = 1f;
            direction.y = verticalDirection * minVerticalDirection;
        }

        rb.linearVelocity = direction.normalized * speed;
    }

    private void UpdateHorizontalDirection(Collision2D collision)
    {
        var normal = collision.GetContact(0).normal;
        // Prevents the ball from getting stuck vertically near side walls.
        if (Mathf.Abs(normal.x) > 0.5f) lastHorizontalDirection = Mathf.Sign(normal.x);
    }

    #endregion MOVEMENT

    //----------------------------------------------------------------------------------------

    #region STOP/RESET
    private Vector2 initialPosition;

    //----------------------------------------------------------------------------------------

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void Reset()
    {
        Stop();
        rb.position = initialPosition;
    }

    #endregion STOP/RESET

    //----------------------------------------------------------------------------------------

    #region EXPLODE

    [Header("Explode")]
    public ParticleSystem explodeEffect;
    public AudioClip explodeSound;
    public BoxCollider2D loseZone;

    //----------------------------------------------------------------------------------------

    public void Explode()
    {
        Stop();
        explodeEffect.Play();
        audioSource.PlayOneShot(explodeSound);
    }

    #endregion EXPLODE

    //----------------------------------------------------------------------------------------

    #region EVENTS

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        initialPosition = rb.position;
    }

    private void FixedUpdate() => NormalizeVelocity();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == loseZone) Explode();
        if (collision.contactCount > 0) UpdateHorizontalDirection(collision);
        NormalizeVelocity();
    }

    #endregion EVENTS
}