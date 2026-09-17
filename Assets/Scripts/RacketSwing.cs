using System.Collections;
using UnityEngine;

public class RacketSwing : MonoBehaviour
{
    public float backswingTime = 0.08f;
    public float forwardSwingTime = 0.12f;
    public float recoveryTime = 0.15f;

    // 오른쪽에서 왼쪽으로 휘두르는 포핸드 동작 (오른손 기준, +X가 오른쪽)
    public Vector3 backswingRotationEuler = new Vector3(15f, 70f, 0f);
    public Vector3 followThroughRotationEuler = new Vector3(-20f, -65f, 0f);
    public Vector3 backswingLocalOffset = new Vector3(0.15f, -0.05f, -0.15f);
    public Vector3 followThroughLocalOffset = new Vector3(-0.2f, 0.1f, 0.3f);

    Quaternion restRotation;
    Vector3 restPosition;
    Coroutine active;

    void Awake()
    {
        restRotation = transform.localRotation;
        restPosition = transform.localPosition;
    }

    // mirrored=false: 포핸드 (오른쪽 -> 왼쪽). mirrored=true: 백핸드 (왼쪽 -> 오른쪽).
    public void PlaySwing(bool mirrored = false)
    {
        if (active != null) StopCoroutine(active);
        active = StartCoroutine(SwingRoutine(mirrored));
    }

    IEnumerator SwingRoutine(bool mirrored)
    {
        float sign = mirrored ? -1f : 1f;

        Vector3 backRotEuler = backswingRotationEuler;
        Vector3 throughRotEuler = followThroughRotationEuler;
        backRotEuler.y *= sign;
        throughRotEuler.y *= sign;

        Vector3 backOffset = backswingLocalOffset;
        Vector3 throughOffset = followThroughLocalOffset;
        backOffset.x *= sign;
        throughOffset.x *= sign;

        // 부모(플레이어) 기준 좌표축으로 회전을 적용 (라켓 자체를 X축으로 돌려놔도
        // 스윙이 항상 같은 좌우/앞뒤 평면에서 일어나도록 함)
        Quaternion backRot = Quaternion.Euler(backRotEuler) * restRotation;
        Quaternion throughRot = Quaternion.Euler(throughRotEuler) * restRotation;
        Vector3 backPos = restPosition + backOffset;
        Vector3 throughPos = restPosition + throughOffset;

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
