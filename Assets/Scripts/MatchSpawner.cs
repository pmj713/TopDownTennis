using Unity.Netcode;
using UnityEngine;

// 온라인 게임 씬에 진입하면 호스트(서버)가 접속된 두 클라이언트에게 플레이어를 하나씩,
// 공을 하나 스폰한다. 호스트가 nearSide, 참가자가 반대편.
public class MatchSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject ballPrefab;
    public Vector3 nearSideSpawnPos = new Vector3(2f, 1f, -11.4f);
    public Vector3 farSideSpawnPos = new Vector3(-2f, 1f, 11.4f);
    public Vector3 ballSpawnPos = new Vector3(0f, 1f, 0f);

    void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        bool nearSide = true;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Vector3 pos = nearSide ? nearSideSpawnPos : farSideSpawnPos;
            var player = Instantiate(playerPrefab, pos, Quaternion.identity);
            var movement = player.GetComponent<NetworkPlayerMovement>();
            movement.nearSide.Value = nearSide;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            nearSide = !nearSide;
        }

        var ball = Instantiate(ballPrefab, ballSpawnPos, Quaternion.identity);
        ball.GetComponent<NetworkObject>().Spawn();
    }
}
