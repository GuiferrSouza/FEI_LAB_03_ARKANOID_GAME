using UnityEngine;
using UnityEngine.Events;

public class ParticleController : MonoBehaviour
{
    #region CALLBACK

    [Header("Callback")]
    public UnityEvent onFinished;

    //----------------------------------------------------------------------------------------

    private void Finished() => onFinished?.Invoke();

    #endregion CALLBACK

    //----------------------------------------------------------------------------------------

    #region EVENTS

    /// <summary>
    /// Stop Action property must be Callback to work.
    /// </summary>
    private void OnParticleSystemStopped() => Finished();

    #endregion EVENTS
}