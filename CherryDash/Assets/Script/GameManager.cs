using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int score = 0;
    public int Score => score;

    public void AddScore(int amount = 1)
    {
        if (isGameOver) return;

        score += amount;
        Debug.Log($"Score: {score}");
        scoreText.text = $"Score: {score}";
    }


    void Awake()
    {
        Instance = this;
        hp = maxHp;
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (bgmClip != null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    [SerializeField]
    GameObject itemPrefab;

    [SerializeField]
    GameObject obstaclePrefab;

    [SerializeField]
    float itemSpawnInterval = 2f;

    [SerializeField]
    Vector2 itemSpawnYRange = new Vector2(-2f, 2f);

    [SerializeField]
    public float scrollSpeed = 10f;

    [SerializeField]
    Camera targetCamera;

    [SerializeField]
    Transform itemParent;

    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    TextMeshProUGUI distanceText;

    [SerializeField]
    TextMeshProUGUI hpText;

    [SerializeField]
    int itemsPerObstacle = 10;

    [Header("Speed Scale")]
    [SerializeField]
    float speedIncreaseRate = 0.5f;

    [SerializeField]
    float maxScrollSpeed = 30f;

    [Header("BGM")]
    [SerializeField]
    AudioClip bgmClip;

    AudioSource bgmSource;

    float spawnTimer;
    int itemSpawnCount;
    float distance;

    [SerializeField]
    int maxHp = 3;
    int hp;
    bool isGameOver = false;

    [SerializeField]
    PlayerController playerController;

    [Header("Platform Settings")]
    public GameObject platformPrefab;
    [SerializeField] float platformSpawnChance = 0.1f;
    [SerializeField] int bonusScoreAmount = 5;
    [SerializeField] int bonusItemCount = 3;
    [SerializeField] float bonusItemSpacing = 1.5f;

    private Platform currentPlatform = null;

    [Header("Game Over UI")]
    [SerializeField]
    GameObject gameOverPanel;

    void Update()
    {
        if (isGameOver) return;

        scrollSpeed = Mathf.Min(scrollSpeed + speedIncreaseRate * Time.deltaTime, maxScrollSpeed);
        //Debug.Log($"Scroll Speed: {scrollSpeed}");

        distance += scrollSpeed * Time.deltaTime;
        if (distanceText != null)
            distanceText.text = $"Distance: {(int)distance}m";

        spawnTimer += Time.deltaTime;
        if (spawnTimer < itemSpawnInterval)
            return;

        spawnTimer = 0f;

        itemSpawnCount++;

        if (currentPlatform == null)
        {
            if (Random.value < platformSpawnChance)
            {
                SpawnPlatform();
            }
            else if (itemSpawnCount >= itemsPerObstacle)
            {
                itemSpawnCount = 0;
                SpawnObstacle();
            }
            else
            {
                SpawnItem();
            }
        }
        else
        {
            SpawnItem();
        }
    }

    public void TakeDamage()
    {
        if (isGameOver) return;

        hp--;
        Debug.Log($"HP: {hp}");
        UpdateHpText();

        if (hp <= 0)
            GameOver();
    }

    void UpdateHpText()
    {
        if (hpText != null)
            hpText.text = $"HP: {hp}";
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    void SpawnItem()
    {
        float spawnX = GetCameraRightX();
        float spawnY = Random.Range(itemSpawnYRange.x, itemSpawnYRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity, itemParent);

        ItemMover mover = item.GetComponent<ItemMover>();
        if (mover == null)
            mover = item.AddComponent<ItemMover>();

        mover.targetCamera = targetCamera != null ? targetCamera : Camera.main;
    }

    void SpawnObstacle()
    {
        float spawnX = GetCameraRightX();

        float[] possibleY = new float[] { -1.0f, -0f, 1f };
        float spawnY = possibleY[Random.Range(0, possibleY.Length)];

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, itemParent);

        ItemMover mover = obstacle.GetComponent<ItemMover>();
        if (mover == null)
            mover = obstacle.AddComponent<ItemMover>();

        mover.targetCamera = targetCamera != null ? targetCamera : Camera.main;
        mover.isObstacle = true;
    }

    void SpawnPlatform()
    {
        if (platformPrefab == null) return;

        float spawnX = GetCameraRightX();
        Vector3 spawnPosition = new Vector3(spawnX, 0.6f, 0f);

        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity, itemParent);

        Platform platformScript = platform.GetComponent<Platform>();
        if (platformScript != null)
            platformScript.targetCamera = targetCamera != null ? targetCamera : Camera.main;

        currentPlatform = platformScript;

        for (int i = 0; i < bonusItemCount; i++)
        {
            Vector3 itemPos = spawnPosition + new Vector3(i * bonusItemSpacing, -1.0f, 0f);
            GameObject item = Instantiate(itemPrefab, itemPos, Quaternion.identity, itemParent);

            Collider2D col = item.GetComponent<Collider2D>();
            if (col != null)
                col.isTrigger = true;

            ItemMover mover = item.GetComponent<ItemMover>();
            if (mover == null)
                mover = item.AddComponent<ItemMover>();

            mover.targetCamera = targetCamera != null ? targetCamera : Camera.main;
            mover.bonusScore = bonusScoreAmount;
            mover.enabled = false;

            if (platformScript != null)
                platformScript.AddBonusItem(item);
        }
    }

    public void OnPlatformDestroyed()
    {
        currentPlatform = null;
    }

    float GetCameraRightX()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return 10f;

        float distance = Mathf.Abs(cam.transform.position.z);
        return cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, distance)).x;
    }
}
