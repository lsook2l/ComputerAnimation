using UnityEngine;

/// <summary>
/// 파티클 효과가 들어간 작은 장애물 프리팹을 중심 기준 풍차 형태로 자동 생성하고 회전시키는 스크립트입니다.
///
/// 구현 의도:
/// - 장애물 20개를 직접 손으로 배치하지 않고, 프리팹 1개를 기준으로 자동 생성합니다.
/// - 날개 개수, 날개당 장애물 개수, 간격, 시작 각도, 회전 속도를 Inspector에서 조절할 수 있습니다.
/// - 파티클은 시각 효과이고, 실제 충돌 판정은 fireClumpPrefab 안의 Circle Collider 2D가 담당합니다.
///
/// 사용 위치:
/// - Obstacle_WindmillDragon 오브젝트에 붙입니다.
/// - fireClumpPrefab에는 Circle Collider 2D, GameOverTrigger, 파티클 자식 오브젝트가 들어 있어야 합니다.
/// </summary>
public class WindmillObstacleGenerator : MonoBehaviour
{
    [Header("장애물 프리팹")]
    [Tooltip("Circle Collider 2D, GameOverTrigger, 파티클이 들어있는 작은 장애물 프리팹")]
    public GameObject fireClumpPrefab;

    [Header("풍차 배치 설정")]
    [Tooltip("풍차 날개 개수. 일반적인 풍차형 장애물은 4를 사용합니다.")]
    public int bladeCount = 4;

    [Tooltip("각 날개에 들어갈 장애물 개수")]
    public int clumpsPerBlade = 6;

    [Tooltip("중심에서 첫 번째 장애물까지의 거리")]
    public float startDistance = 7f;

    [Tooltip("장애물 사이 간격")]
    public float spacing = 7f;

    [Tooltip("풍차 시작 각도. 45도로 설정하면 대각선 방향 날개 형태가 됩니다.")]
    public float startAngle = 45f;

    [Header("회전 설정")]
    [Tooltip("체크하면 풍차 장애물이 계속 회전합니다.")]
    public bool rotate = true;

    [Tooltip("초당 회전 각도. 값이 클수록 빠르게 회전합니다.")]
    public float rotationSpeed = 10f;

    [Tooltip("체크하면 시계 방향, 해제하면 반시계 방향으로 회전합니다.")]
    public bool clockwise = true;

    [Header("생성 설정")]
    [Tooltip("게임 시작 시 자동으로 풍차 장애물을 생성합니다.")]
    public bool generateOnStart = true;

    [Tooltip("생성 전 기존 자식 오브젝트를 삭제합니다. 중복 생성을 방지하는 용도입니다.")]
    public bool clearChildrenBeforeGenerate = true;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateWindmill();
        }
    }

    private void Update()
    {
        if (!rotate)
        {
            return;
        }

        float direction = clockwise ? -1f : 1f;

        // 현재 오브젝트 자체를 회전시키면, 자식으로 생성된 모든 장애물이 중심 기준으로 함께 회전합니다.
        transform.Rotate(0f, 0f, rotationSpeed * direction * Time.deltaTime);
    }

    /// <summary>
    /// 중심 기준으로 여러 방향의 날개에 장애물 프리팹을 자동 배치합니다.
    /// ContextMenu를 사용했기 때문에 Inspector의 우클릭 메뉴에서도 수동 실행할 수 있습니다.
    /// </summary>
    [ContextMenu("Generate Windmill")]
    public void GenerateWindmill()
    {
        if (fireClumpPrefab == null)
        {
            Debug.LogError("Fire Clump Prefab이 연결되지 않았습니다.");
            return;
        }

        if (clearChildrenBeforeGenerate)
        {
            ClearChildren();
        }

        // bladeIndex: 몇 번째 날개인지
        for (int bladeIndex = 0; bladeIndex < bladeCount; bladeIndex++)
        {
            float angle = startAngle + (360f / bladeCount) * bladeIndex;
            float radian = angle * Mathf.Deg2Rad;

            // 현재 날개가 뻗어나갈 방향 벡터를 계산합니다.
            Vector3 bladeDirection = new Vector3(
                Mathf.Cos(radian),
                Mathf.Sin(radian),
                0f
            );

            // clumpIndex: 한 날개 안에서 몇 번째 장애물인지
            for (int clumpIndex = 0; clumpIndex < clumpsPerBlade; clumpIndex++)
            {
                float distance = startDistance + spacing * clumpIndex;
                Vector3 localPosition = bladeDirection * distance;

                // fireClumpPrefab을 현재 오브젝트의 자식으로 생성합니다.
                GameObject clump = Instantiate(fireClumpPrefab, transform);

                clump.name =
                    "Obstacle_FireClump_B" +
                    (bladeIndex + 1) +
                    "_" +
                    (clumpIndex + 1);

                // localPosition을 사용해야 부모인 Obstacle_WindmillDragon 기준으로 배치됩니다.
                clump.transform.localPosition = localPosition;
                clump.transform.localRotation = Quaternion.identity;
                clump.transform.localScale = Vector3.one;
            }
        }
    }

    /// <summary>
    /// 기존에 생성된 자식 장애물들을 삭제합니다.
    /// 에디터 실행 중과 게임 실행 중 삭제 방식이 달라서 DestroyImmediate와 Destroy를 구분합니다.
    /// </summary>
    [ContextMenu("Clear Children")]
    public void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}
