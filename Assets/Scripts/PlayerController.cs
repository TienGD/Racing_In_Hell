using System.IO; // THÊM THƯ VIỆN NÀY ĐỂ XỬ LÝ FILE
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Tạo một class để chứa dữ liệu cần lưu
[System.Serializable]
public class SaveData
{
    public float highScore;
}

public class PlayerController : MonoBehaviour
{
    #region VARIABLE DECLARATION AREA
    public float thrustForce = 1f;
    Rigidbody2D rb;
    public float maxSpeed = 5f;
    public GameObject boosterFlame;
    public GameObject boosterFlame2;
    private float elapsedTime = 0f;
    private float score = 0f;
    private float highScore = 0f;
    public float scoreMultiplier = 10f;
    public UIDocument uIDocument;
    private Label scoreText;
    private Label highScoreText;
    public GameObject ExplosionEffect;
    private Button RestartButton;
    public GameObject Obstacle;
    public GameObject Border;



    // ĐƯỜNG DẪN FILE TẠI Ổ D
    private string directoryPath = @"D:\UnityMaterial\GameData";
    private string fileName = "save.json";
    private string fullPath;

    #endregion

    #region START METHOD

    private void Awake()
    {
        var root = uIDocument.rootVisualElement;
        rb = GetComponent<Rigidbody2D>();
        scoreText = root.Q<Label>("ScoreLabel");
        highScoreText = root.Q<Label>("HighScoreLabel");
        RestartButton = root.Q<Button>("RestartButton");
    }
    #endregion

    #region AWAKE METHOD
    void Start()
    {
        fullPath = Path.Combine(directoryPath, fileName);
        RestartButton.style.display = DisplayStyle.None;
        RestartButton.clicked += ReloadScence;

        // LOAD DỮ LIỆU TỪ FILE JSON
        LoadGameData();
        highScoreText.text = "High Score: " + highScore;
    }
    #endregion

    #region UPDATE METHOD

    void Update()
    {
        if (rb != null)
        {
            UpdateScore();
            MovePlayer();
        }
    }

    #endregion

    #region ONDISABLE METHOD
    private void OnDisable()
    {
        Destroy(gameObject);
        Destroy(rb);
        Destroy(Obstacle);

    }
    #endregion

    #region ONDESTROY METHOD
    private void OnDestroy()
    {
        Border.SetActive(false);
        Destroy(Border);
        Debug.Log("OnDestroy called.");
    }
    #endregion

    void UpdateScore()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score;
    }

    void MovePlayer()
    {
        // xử lý nhấn chuột trái 
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = ((Vector2)mousePos - (Vector2)transform.position).normalized;
            transform.up = direction;
            rb.AddForce(direction * thrustForce);
        }

        // giới hạn tốc độ
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // xử lý bật tắt lửa 
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlame.SetActive(true);
            boosterFlame2.SetActive(true);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlame.SetActive(false);
            boosterFlame2.SetActive(false);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(ExplosionEffect, transform.position, transform.rotation);

        // KIỂM TRA VÀ LƯU JSON KHI CÓ ĐIỂM CAO MỚI
        if (score > highScore)
        {
            highScore = score;
            SaveGameData(); // Gọi hàm lưu xuống ổ D
        }

        RestartButton.style.display = DisplayStyle.Flex;
        highScoreText.text = "High Score: " + highScore;

        gameObject.SetActive(false);
        Obstacle.SetActive(false);

    }

    #region XỬ LÝ JSON
    // --- HÀM XỬ LÝ JSON ---

    void SaveGameData()
    {
        // 1. Kiểm tra và tạo thư mục nếu chưa tồn tại
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 2. Chuẩn bị dữ liệu
        SaveData data = new SaveData();
        data.highScore = highScore;

        // 3. Chuyển sang chuỗi JSON
        string json = JsonUtility.ToJson(data, true);

        // 4. Ghi file
        File.WriteAllText(fullPath, json);
        Debug.Log("Đã lưu HighScore vào JSON tại: " + fullPath);
    }

    void LoadGameData()
    {
        if (File.Exists(fullPath))
        {
            // 1. Đọc nội dung file
            string json = File.ReadAllText(fullPath);
            // 2. Giải mã JSON về lại class
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            // 3. Gán giá trị
            highScore = data.highScore;
        }
        else
        {
            highScore = 0;
            Debug.Log("Không tìm thấy file save, bắt đầu với điểm 0.");
        }
    }
    #endregion

    void ReloadScence()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}