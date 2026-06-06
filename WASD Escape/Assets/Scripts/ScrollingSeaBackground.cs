using UnityEngine;

/// <summary>
/// 3장의 바다 배경 이미지를 이어 붙여 무한 스크롤처럼 보이게 만드는 스크립트입니다.
///
/// 담당 기능:
/// 1. sea1, sea2, sea3를 가로로 이어 배치
/// 2. 배경을 왼쪽 또는 오른쪽으로 천천히 이동
/// 3. 화면 밖으로 나간 이미지를 반대편 끝으로 이동시켜 빈 공간이 생기지 않게 처리
///
/// 사용 위치:
/// - 배경 관리용 빈 오브젝트에 붙입니다.
/// - sea1, sea2, sea3에는 SpriteRenderer가 있어야 합니다.
///
/// 주의:
/// - Unity에서 스크립트 파일명과 클래스명이 달라서 문제가 생기면,
///   파일명을 InfiniteSeaBackground.cs로 바꾸거나 class 이름을 파일명에 맞게 변경해야 합니다.
/// </summary>
public class InfiniteSeaBackground : MonoBehaviour
{
    [Header("이어 붙일 바다 배경 3장")]
    public Transform sea1;
    public Transform sea2;
    public Transform sea3;

    [Header("스크롤 설정")]
    [Tooltip("배경이 이동하는 속도")]
    public float speed = 0.2f;

    [Tooltip("체크하면 오른쪽으로, 해제하면 왼쪽으로 이동")]
    public bool moveRight = false;

    private Transform[] seas;
    private float width;

    private void Start()
    {
        seas = new Transform[] { sea1, sea2, sea3 };

        // 가운데 배경인 sea2의 SpriteRenderer 크기를 기준으로 이미지 한 장의 폭을 계산합니다.
        width = sea2.GetComponent<SpriteRenderer>().bounds.size.x;

        // sea2를 기준으로 왼쪽과 오른쪽에 sea1, sea3를 자동 배치합니다.
        Vector3 center = sea2.position;

        sea1.position = new Vector3(center.x - width, center.y, center.z);
        sea2.position = center;
        sea3.position = new Vector3(center.x + width, center.y, center.z);
    }

    private void Update()
    {
        float dir = moveRight ? 1f : -1f;
        Vector3 move = Vector3.right * dir * speed * Time.deltaTime;

        // 세 배경을 같은 방향으로 이동시킵니다.
        foreach (Transform sea in seas)
        {
            sea.position += move;
        }

        if (!moveRight)
        {
            foreach (Transform sea in seas)
            {
                // 왼쪽으로 이동 중일 때, 가장 왼쪽 배경이 화면 밖으로 완전히 나가면
                // 현재 가장 오른쪽 배경의 오른쪽 끝에 다시 붙입니다.
                if (sea.position.x + width < GetLeftEdge())
                {
                    sea.position = new Vector3(GetRightMostX() + width, sea.position.y, sea.position.z);
                }
            }
        }
        else
        {
            foreach (Transform sea in seas)
            {
                // 오른쪽으로 이동 중일 때, 가장 오른쪽 배경이 화면 밖으로 완전히 나가면
                // 현재 가장 왼쪽 배경의 왼쪽 끝에 다시 붙입니다.
                if (sea.position.x - width > GetRightEdge())
                {
                    sea.position = new Vector3(GetLeftMostX() - width, sea.position.y, sea.position.z);
                }
            }
        }
    }

    /// <summary>
    /// 현재 카메라 화면의 왼쪽 끝 월드 좌표를 반환합니다.
    /// </summary>
    private float GetLeftEdge()
    {
        Camera cam = Camera.main;
        float halfWidth = cam.orthographicSize * cam.aspect;
        return cam.transform.position.x - halfWidth;
    }

    /// <summary>
    /// 현재 카메라 화면의 오른쪽 끝 월드 좌표를 반환합니다.
    /// </summary>
    private float GetRightEdge()
    {
        Camera cam = Camera.main;
        float halfWidth = cam.orthographicSize * cam.aspect;
        return cam.transform.position.x + halfWidth;
    }

    /// <summary>
    /// 세 배경 중 가장 왼쪽에 있는 배경의 X 좌표를 반환합니다.
    /// </summary>
    private float GetLeftMostX()
    {
        float min = seas[0].position.x;

        foreach (Transform sea in seas)
        {
            if (sea.position.x < min)
            {
                min = sea.position.x;
            }
        }

        return min;
    }

    /// <summary>
    /// 세 배경 중 가장 오른쪽에 있는 배경의 X 좌표를 반환합니다.
    /// </summary>
    private float GetRightMostX()
    {
        float max = seas[0].position.x;

        foreach (Transform sea in seas)
        {
            if (sea.position.x > max)
            {
                max = sea.position.x;
            }
        }

        return max;
    }
}
