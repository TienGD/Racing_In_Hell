using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    Rigidbody2D rb;
    public float minSize = 0.5f;
    public float maxSize = 2f;
    public float minSpeed = 50f;
    public float maxSpeed = 150f;
    public float maxspin = 10f;

    // Thêm biến để giới hạn tốc độ tối đa
    public float maxVelocityLimit = 7f;

    public GameObject bounceEffectPrefab;

    void Start()
    {
        // Xử lý random kích thước
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        rb = GetComponent<Rigidbody2D>();

        // Xử lý lực ban đầu
        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        Vector2 randomDirection = Random.insideUnitCircle;
        rb.AddForce(randomDirection * randomSpeed);

        // Xử lý momen xoắn
        float randomTorque = Random.Range(-maxspin, maxspin);
        rb.AddTorque(randomTorque);
    }

    // FixedUpdate được gọi mỗi khung hình vật lý (thích hợp để xử lý Rigidbody)
    void FixedUpdate()
    {
        // Kiểm tra nếu tốc độ hiện tại vượt quá giới hạn
        if (rb.linearVelocity.magnitude > maxVelocityLimit)
        {
            // Thiết lập vận tốc về mức giới hạn nhưng vẫn giữ nguyên hướng di chuyển
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocityLimit;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bounceEffectPrefab != null)
        {
            Vector2 contactPoint = collision.GetContact(0).point;
            GameObject bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);
            Destroy(bounceEffect, 1f);
        }
    }
}