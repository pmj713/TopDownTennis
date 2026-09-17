using System.Collections;
using UnityEngine;

public class RacketSwing : MonoBehaviour
{
    public float backswingTime = 0.08f;
    public float forwardSwingTime = 0.12f;
    public float recoveryTime = 0.15f;

    public Vector3 backswingRotationEuler = new Vector3(15f, -70f, 0f);
    public Vector3 followThroughRotationEuler = new Vector3(-20f, 65f, 0f);
    public Vector3 backswingLocalOffset = new Vector3(-0.15f, -0.05f, -0.15f);
    public Vector3 followThroughLocalOffset = new Vector3(0.2f, 0.1f, 0.3f);

    Quaternion restRotation;
    Vector3 restPosition;
    Coroutine active;

    void Awake()
    {
        restRotation = transform.localRotation;
        restPosition = transform.localPosition;
    }

    public void PlaySwing()
    {
        if (active != null) StopCoroutine(active);
        active = StartCoroutine(SwingRoutine());
    }

    IEnumerator SwingRoutine()
    {
        Quaternion backRot = restRotation * Quaternion.Euler(backswingRotationEuler);
        Quaternion throughRot = restRotation * Quaternion.Euler(followThroughRotationEuler);
        Vector3 backPos = restPosition + backswingLocalOffset;
        Vector3 throughPos = restPosition + followThroughLocalOffset;

        yield return Ease(restRotation, backRot, restPosition, backPos, backswingTime);
        yield return Ease(backRot, throughRot, backPos, throughPos, forwardSwingTime);
        yield return Ease(throughRot, restRotation, throughPos, restPosition, recoveryTime);

        transform.localRotation = restRotation;
        transform.localPosition = restPosition;
        active = null;
    }

    IEnumerator Ease(Quaternion fromRot, Quaternion toRot, Vector3 fromPos, Vector3 toPos, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            transform.localRotation = Quaternion.Slerp(fromRot, toRot, k);
            transform.localPosition = Vector3.Lerp(fromPos, toPos, k);
            yield return null;
        }
    }
}
