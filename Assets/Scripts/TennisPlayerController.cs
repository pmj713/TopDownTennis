using UnityEngine;
using UnityEngine.InputSystem;

public class TennisPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float halfCourtWidth = 4.115f;
    public float halfCourtLength = 11.885f;
    public bool nearSide = true;

    public Key upKey = Key.W;
    public Key downKey = Key.S;
    public Key leftKey = Key.A;
    public Key rightKey = Key.D;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        Vector3 move = Vector3.zero;
        if (kb[upKey].isPressed) move.z += 1f;
        if (kb[downKey].isPressed) move.z -= 1f;
        if (kb[leftKey].isPressed) move.x -= 1f;
        if (kb[rightKey].isPressed) move.x += 1f;

        if (move.sqrMagnitude > 1f) move.Normalize();

        Vector3 pos = transform.position + move * moveSpeed * Time.deltaTime;

        const float margin = 0.4f;
        pos.x = Mathf.Clamp(pos.x, -halfCourtWidth + margin, halfCourtWidth - margin);
        pos.z = nearSide
            ? Mathf.Clamp(pos.z, -halfCourtLength + margin, -margin)
            : Mathf.Clamp(pos.z, margin, halfCourtLength - margin);

        transform.position = pos;
    }
}
