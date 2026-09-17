using System.Collections;
using UnityEngine;

public class RacketSwing : MonoBehaviour
{
    public float swingDuration = 0.3f;
    public float swingAngle = 100f;

    Quaternion restRotation;
    Coroutine active;

    void Awake()
    {
        restRotation = transform.localRotation;
    }

    public void PlaySwing()
    {
        if (active != null) StopCoroutine(active);
        active = StartCoroutine(SwingRoutine());
    }

    IEnumerator SwingRoutine()
    {
        float half = swingDuration * 0.5f;
        Quaternion swungRotation = restRotation * Quaternion.Euler(-swingAngle, 0f, 0f);

        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(restRotation, swungRotation, t / half);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(swungRotation, restRotation, t / half);
            yield return null;
        }

        transform.localRotation = restRotation;
        active = null;
    }
}
