using UnityEngine;

/// <summary>
/// Main Camera가 플레이어를 따라가도록 하는 스크립트입니다.
///
/// 사용 위치:
/// - Main Camera 오브젝트에 붙입니다.
/// - Target에 Dragon_Player를 연결합니다.
/// - 카메라는 이 스크립트로 따라가게 합니다.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("따라갈 대상")]
    [Tooltip("카메라가 따라갈 플레이어 Transform")]
    public Transform target;

    [Header("카메라 위치 보정")]
    [Tooltip("플레이어 위치에서 카메라를 얼마나 떨어뜨릴지 설정합니다. 2D에서는 Z 값을 -10으로 둡니다.")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("부드럽게 따라가기 사용")]
    [Tooltip("체크하면 카메라가 부드럽게 따라가고, 해제하면 플레이어 위치에 즉시 맞춰집니다.")]
    public bool useSmoothFollow = false;

    [Header("부드러운 이동 속도")]
    [Tooltip("useSmoothFollow가 켜져 있을 때 카메라가 따라가는 속도")]
    public float smoothSpeed = 8f;

    private void LateUpdate()
    {
        // 대상이 연결되지 않았으면 카메라 이동을 하지 않습니다.
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position + offset;

        if (useSmoothFollow)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.position = targetPosition;
        }
    }
}
