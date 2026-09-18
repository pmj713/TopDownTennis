using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// TennisPlayerController의 온라인 버전. 이동은 각자 자신의 클라이언트가 소유(owner-authoritative)하며,
// NetworkTransform(Owner 모드)이 위치를 상대에게 동기화한다.
public class NetworkPlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float halfCourtWidth = 4.115f;
    public float halfCourtLength = 11.885f;

    // 서버가 스폰 시 지정: 이 플레이어가 코트의 가까운 쪽(-Z)인지 먼 쪽(+Z)인지.
    public NetworkVariable<bool> nearSide = new NetworkVariable<bool>(true);

    public Key upKey = Key.W;
    public Key downKey = Key.S;
    public Key leftKey = Key.A;
    public Key rightKey = Key.D;

    void Update()
    {
        if (!IsOwner) return;

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
        pos.z = nearSide.Value
            ? Mathf.Clamp(pos.z, -halfCourtLength + margin, -margin)
            : Mathf.Clamp(pos.z, margin, halfCourtLength - margin);

        transform.position = pos;
    }
}
