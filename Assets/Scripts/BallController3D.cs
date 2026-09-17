using UnityEngine;

public class BallController3D : MonoBehaviour
{
    public float halfCourtWidth = 4.115f;
    public float halfCourtLength = 11.885f;
    public float groundBounceHeightThreshold = 0.3f;

    public event System.Action<int> OnCrossedNet;
    public event System.Action<int, bool, int> OnBounced;
    public event System.Action OnHitNet;

    public int CurrentSide => currentSide;

    Rigidbody rb;
    int currentSide;
    int bounceCountOnCurrentSide;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentSide = SideOf(transform.position.z);
    }

    void FixedUpdate()
    {
        int side = SideOf(transform.position.z);
        if (side != currentSide)
        {
            currentSide = side;
            bounceCountOnCurrentSide = 0;
            OnCrossedNet?.Invoke(currentSide);
            Debug.Log($"[Ball] Crossed net onto side {currentSide}");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Net"))
        {
            OnHitNet?.Invoke();
            Debug.Log("[Ball] Hit the net");
            return;
        }

        if (transform.position.y > groundBounceHeightThreshold) return;

        bounceCountOnCurrentSide++;
        bool inBounds = Mathf.Abs(transform.position.x) <= halfCourtWidth
                         && Mathf.Abs(transform.position.z) <= halfCourtLength;

        OnBounced?.Invoke(currentSide, inBounds, bounceCountOnCurrentSide);
        Debug.Log($"[Ball] Bounced on side {currentSide} at {transform.position} — inBounds={inBounds}, bounceCount={bounceCountOnCurrentSide}");
    }

    int SideOf(float z) => z >= 0 ? 1 : -1;
}
