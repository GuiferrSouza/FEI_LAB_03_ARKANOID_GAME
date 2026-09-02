using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float minHorizontalDirection = 0.25f;
    public float minVerticalDirection = 0.25f;

    private Rigidbody2D rb;
    private float lastHorizontalDirection = 1f;

    [Header("Lose Zone")]
    public BoxCollider2D loseZone;

    [Header("GameController")]
    public GameController gameController;

    //----------------------------------------------------------------------------------------

    #region RESET
    private Vector2 initialPosition;

    //----------------------------------------------------------------------------------------

    public void Reset()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = initialPosition;
    }

    #endregion RESET

    //----------------------------------------------------------------------------------------

    #region MOVEMENT

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

    #endregion MOVEMENT

    //----------------------------------------------------------------------------------------

    #region EVENTS

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = rb.position;
    }

    private void FixedUpdate()
    {
        NormalizeVelocity();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == loseZone) gameController.Defeat();
        if (collision.contactCount > 0)
        {
            var normal = collision.GetContact(0).normal;
            // Prevents the ball from getting stuck vertically near side walls.
            if (Mathf.Abs(normal.x) > 0.5f) lastHorizontalDirection = Mathf.Sign(normal.x);
        }

        NormalizeVelocity();
    }

    #endregion EVENTS
}