using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    public float speed = 8f;
    public float boundY = 3.9f;
    public Key upKey = Key.W;
    public Key downKey = Key.S;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float move = 0f;
        if (keyboard[upKey].isPressed) move += 1f;
        if (keyboard[downKey].isPressed) move -= 1f;

        float newY = Mathf.Clamp(rb.position.y + move * speed * Time.fixedDeltaTime, -boundY, boundY);
        rb.MovePosition(new Vector2(rb.position.x, newY));
    }
}
