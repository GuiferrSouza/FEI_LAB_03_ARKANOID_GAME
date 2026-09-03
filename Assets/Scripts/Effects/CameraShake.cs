using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float magnitude = 0.25f;
    public float duration = 0.15f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    //----------------------------------------------------------------------------------------

    public void Shake()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            var progress = elapsed / duration;
            var currentMagnitude = Mathf.Lerp(magnitude, 0f, progress);

            var offset = Random.insideUnitCircle * currentMagnitude;
            transform.localPosition = originalPosition + new Vector3(offset.x, offset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }

    //----------------------------------------------------------------------------------------

    #region EVENTS

    private void Awake() => originalPosition = transform.localPosition;

    #endregion EVENTS
}