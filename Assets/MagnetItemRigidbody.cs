using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MagnetItemRigidbody : MonoBehaviour
{
    public float magnetRange = 5f;       // 吸引開始距離
    public float attractionForce = 10f;  // 吸引力
    public float maxSpeed = 5f;          // 最大速度制限

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // 空中を漂うアイテム想定
        rb.linearDamping = 2f;          // 抵抗（滑らかさ調整）

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < magnetRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // 吸引力を加える（距離によって増加させても良い）
            rb.AddForce(direction * attractionForce);

            // 最大速度制限
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }
}
