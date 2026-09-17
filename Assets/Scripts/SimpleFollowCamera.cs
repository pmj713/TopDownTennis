using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    public Transform target;
    public Transform lookTarget;
    public Vector3 offset = new Vector3(0f, 2.2f, -4.5f);
    public float followLerp = 8f;
    public float lookAtBlend = 0.35f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);

        Vector3 lookPoint = target.position + Vector3.up * 1.2f;
        if (lookTarget)
        {
            lookPoint = Vector3.Lerp(lookPoint, lookTarget.position, lookAtBlend);
        }

        Vector3 lookDir = lookPoint - transform.position;
        if (lookDir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), followLerp * Time.deltaTime);
    }
}
