using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PaddleController : MonoBehaviour
{
    public BoxCollider2D leftWall;
    public BoxCollider2D rightWall;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    // Using a dedicated AudioSource for charging sound to avoid interrupting other sounds.
    private AudioSource audioSource;
    private Vector2 initialPosition;

    //----------------------------------------------------------------------------------------

    #region RESET

    public void Reset()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = initialPosition;

        isLaunched = false;
        movementDirection = 0f;

        chargeDirection = 0;
        chargeTime = 0f;

        audioSource.Stop();

        directionIndicatorFill.fillAmount = 0f;
        directionIndicator.gameObject.SetActive(false);
    }

    #endregion RESET

    //----------------------------------------------------------------------------------------

    #region MOVEMENT

    [Header("Movement")]
    public float movementSpeed = 8f;

    private float movementDirection;
    private bool isLaunched;

    //----------------------------------------------------------------------------------------

    private void UpdateMovement(Keyboard keyboard)
    {
        movementDirection = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) movementDirection = -1f;
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) movementDirection = 1f;

        // Normal movement takes control back after a launch.
        if (movementDirection != 0f)
        {
            isLaunched = false;
            rb.linearVelocity = Vector2.right * movementDirection * movementSpeed;
        }
        else if (!isLaunched) rb.linearVelocity = Vector2.zero;
    }

    #endregion MOVEMENT

    //----------------------------------------------------------------------------------------

    #region CHARGE

    [Header("Charging")]
    public float minLaunchSpeed = 4f;
    public float maxLaunchSpeed = 16f;
    public float maxChargeTime = 1.5f;

    public Transform directionIndicator;
    public Image directionIndicatorFill;
    public float indicatorDistance = 0.5f;

    private float halfWidth;
    private float chargeTime;
    private int chargeDirection;

    public AudioClip chargeSound;

    //----------------------------------------------------------------------------------------

    private void UpdateDirectionIndicator()
    {
        var charge = Mathf.Clamp01(chargeTime / maxChargeTime);

        // Positions the indicator on the selected side.
        directionIndicator.localPosition = new Vector3(chargeDirection * (halfWidth + indicatorDistance), 0f, 0f);
        directionIndicator.localRotation = Quaternion.Euler(0f, chargeDirection < 0 ? 180f : 0f, 0f);

        // Fills the indicator based on the current charge.
        directionIndicatorFill.fillAmount = charge;
    }

    private void StartCharge(int direction)
    {
        chargeDirection = direction;
        chargeTime = 0f;
        isLaunched = false;

        rb.linearVelocity = Vector2.zero;

        audioSource.Play();

        directionIndicator.gameObject.SetActive(true);
        UpdateDirectionIndicator();
    }

    private void UpdateCharge(Keyboard keyboard)
    {
        chargeTime = Mathf.Min(chargeTime + Time.deltaTime, maxChargeTime);

        // Changes direction without resetting the current charge.
        if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame) chargeDirection = -1;
        else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame) chargeDirection = 1;

        UpdateDirectionIndicator();

        // Launches only when Shift is released.
        var shiftPressed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        if (!shiftPressed) Launch();
    }

    private void Launch()
    {
        var charge = Mathf.Clamp01(chargeTime / maxChargeTime);
        var speed = Mathf.Lerp(minLaunchSpeed, maxLaunchSpeed, charge);
        rb.linearVelocity = Vector2.right * chargeDirection * speed;

        isLaunched = true;

        audioSource.Stop();

        chargeDirection = 0;
        chargeTime = 0f;

        directionIndicatorFill.fillAmount = 0f;
        directionIndicator.gameObject.SetActive(false);
    }

    #endregion CHARGE

    //----------------------------------------------------------------------------------------

    #region IMPACT

    [Header("Wall Impact")]
    public CameraShake cameraShake;
    public AudioClip wallCollisionSound;

    public float minShakeImpact = 6f;

    public static event Action WallImpact;

    //----------------------------------------------------------------------------------------

    private void ShakeCamera(float velocity)
    {
        var impactSpeed = Mathf.Abs(velocity);
        if (impactSpeed < minShakeImpact) return;

        cameraShake.Shake();
        GameController.PlaySound(wallCollisionSound);
        WallImpact?.Invoke();
    }

    #endregion IMPACT

    //----------------------------------------------------------------------------------------

    #region BALL

    [Header("Ball")]
    public GameObject ball;
    public AudioClip ballCollisionSound;

    #endregion BALL

    //----------------------------------------------------------------------------------------

    #region EVENTS

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        initialPosition = rb.position;

        halfWidth = spriteRenderer.sprite.bounds.extents.x;

        directionIndicatorFill.fillAmount = 0f;
        directionIndicator.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!GameController.GameStarted) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Continues an existing charge.
        if (chargeDirection != 0)
        {
            UpdateCharge(keyboard);
            return;
        }

        // Shift + direction starts charging.
        var shiftPressed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        if (shiftPressed)
        {
            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            {
                StartCharge(-1);
                return;
            }

            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            {
                StartCharge(1);
                return;
            }
        }

        // Regular movement.
        UpdateMovement(keyboard);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == leftWall || collision.collider == rightWall) ShakeCamera(collision.relativeVelocity.x);
        else if (collision.gameObject == ball) GameController.PlaySound(ballCollisionSound);
    }

    #endregion EVENTS
}