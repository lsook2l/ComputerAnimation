using UnityEngine;

/// <summary>
/// 드래곤 플레이어의 이동, 키 배정, 이동 애니메이션을 제어하는 스크립트입니다.
///
/// 핵심 기능:
/// 1. 게임 시작 시 바로 움직이지 않고, 키 부여 버튼을 눌렀을 때만 키를 배정합니다.
/// 2. 상하좌우 4방향 중 2개는 Player1의 WASD, 나머지 2개는 Player2의 방향키로 랜덤 배정합니다.
/// 3. 키를 한 번 누르면 해당 방향으로 계속 이동합니다.
/// 4. 장애물 충돌로 재시도할 때는 기존 키 배정을 유지합니다.
/// 5. ESC 재시작 또는 클리어 후 다시하기는 키 배정을 초기화하고 다시 부여받게 합니다.
///
/// 사용 위치:
/// - Dragon_Player 오브젝트에 붙입니다.
/// - Dragon_Player에는 Rigidbody2D, Collider2D, Animator가 있어야 합니다.
/// - Dragon_Player의 Tag는 Player로 설정합니다.
/// </summary>
public class DragonPlayerMove : MonoBehaviour
{
    public static DragonPlayerMove Instance { get; private set; }

    [Header("이동 설정")]
    [Tooltip("캐릭터 이동 속도")]
    public float moveSpeed = 3f;

    [Header("플레이어 이름")]
    [Tooltip("Player1 이름. 키 부여 UI에서 입력받아 저장됩니다.")]
    public string player1Name = "Player1";

    [Tooltip("Player2 이름. 키 부여 UI에서 입력받아 저장됩니다.")]
    public string player2Name = "Player2";

    // 씬 리셋 후에도 플레이어 이름을 유지하기 위한 static 저장 변수입니다.
    private static string savedPlayer1Name;
    private static string savedPlayer2Name;

    [Header("키 배정 결과")]
    [Tooltip("Player1에게 배정된 WASD 키 2개")]
    public string player1AssignedKeys;

    [Tooltip("Player2에게 배정된 방향키 2개")]
    public string player2AssignedKeys;

    [Header("Animator State 이름")]
    [Tooltip("아래쪽 이동 애니메이션 State 이름")]
    public string walkDownStateName = "Dragon_Walk_Down";

    [Tooltip("위쪽 이동 애니메이션 State 이름")]
    public string walkUpStateName = "Dragon_Walk_Up";

    [Tooltip("왼쪽 이동 애니메이션 State 이름")]
    public string walkLeftStateName = "Dragon_Walk_Left";

    [Tooltip("오른쪽 이동 애니메이션 State 이름")]
    public string walkRightStateName = "Dragon_Walk_Right";

    private Rigidbody2D rb;
    private Animator animator;

    // 실제 방향별로 배정된 키입니다.
    private KeyCode upKey;
    private KeyCode leftKey;
    private KeyCode downKey;
    private KeyCode rightKey;

    // 현재 이동 방향입니다. 키를 한 번 누르면 이 값이 유지되어 계속 이동합니다.
    private Vector2 moveDirection = Vector2.zero;

    // 키를 한 번이라도 눌러 이동이 시작되었는지 나타냅니다.
    private bool isMoving = false;

    // 키 부여 버튼을 눌러 키 배정이 완료되었는지 나타냅니다.
    private bool hasAssignedKeys = false;

    // 장애물 충돌 시 즉시 이동을 멈추기 위한 상태값입니다.
    private bool canMove = true;

    public bool HasAssignedKeys => hasAssignedKeys;

    // 씬이 다시 로드되어도 기존 키를 유지하기 위한 static 변수들입니다.
    private static bool hasSavedKeys = false;
    private static bool useSavedKeysOnNextLoad = false;

    private static KeyCode savedUpKey;
    private static KeyCode savedLeftKey;
    private static KeyCode savedDownKey;
    private static KeyCode savedRightKey;

    private static string savedPlayer1AssignedKeys;
    private static string savedPlayer2AssignedKeys;

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 장애물 충돌 후 재시도하는 경우에는 기존 키와 이름을 그대로 불러옵니다.
        if (useSavedKeysOnNextLoad && hasSavedKeys)
        {
            LoadSavedKeys();
        }
        else
        {
            hasAssignedKeys = false;
            player1AssignedKeys = "";
            player2AssignedKeys = "";
        }

        // 게임 시작 직후에는 캐릭터가 자동으로 걷는 애니메이션을 재생하지 않도록 Animator를 꺼둡니다.
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    private void Update()
    {
        // 키가 아직 배정되지 않았거나 충돌로 이동이 막힌 상태라면 입력을 받지 않습니다.
        if (!hasAssignedKeys || !canMove)
        {
            return;
        }

        CheckDirectionInput();
    }

    private void FixedUpdate()
    {
        // 키 배정 전, 이동 전, 충돌 직후에는 위치를 이동시키지 않습니다.
        if (!hasAssignedKeys || !canMove || !isMoving)
        {
            return;
        }

        if (rb != null)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            transform.position += (Vector3)(moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// 키 부여 버튼을 눌렀을 때 호출합니다.
    /// 입력받은 플레이어 이름을 저장하고, 이동 키를 새로 랜덤 배정합니다.
    /// </summary>
    public void AssignNewRandomKeysFromButton(string newPlayer1Name, string newPlayer2Name)
    {
        player1Name = newPlayer1Name;
        player2Name = newPlayer2Name;

        RandomizeMoveKeys();
        SaveCurrentKeys();

        hasAssignedKeys = true;
        canMove = true;
        isMoving = false;
        moveDirection = Vector2.zero;

        // 키를 누르기 전까지 걷기 애니메이션이 자동 재생되지 않도록 유지합니다.
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    /// <summary>
    /// 화면 UI에 표시할 키 배정 결과 문자열을 반환합니다.
    /// </summary>
    public string GetAssignedKeyResultText()
    {
        if (!hasAssignedKeys)
        {
            return "Player1: -\nPlayer2: -";
        }

        return player1Name + ": " + player1AssignedKeys + "\n" +
               player2Name + ": " + player2AssignedKeys;
    }

    /// <summary>
    /// 장애물 충돌 후 씬을 다시 불러올 때 기존 키를 유지하도록 설정합니다.
    /// </summary>
    public static void KeepCurrentKeysOnNextLoad()
    {
        useSavedKeysOnNextLoad = true;
    }

    /// <summary>
    /// ESC 메뉴 재시작 또는 클리어 후 다시하기를 눌렀을 때 키를 새로 부여받도록 초기화합니다.
    /// </summary>
    public static void NewRandomKeysOnNextLoad()
    {
        hasSavedKeys = false;
        useSavedKeysOnNextLoad = false;
    }

    /// <summary>
    /// 현재 배정된 키와 플레이어 이름을 static 변수에 저장합니다.
    /// 씬이 다시 로드되어도 같은 키 배정을 유지하기 위해 사용합니다.
    /// </summary>
    private void SaveCurrentKeys()
    {
        savedUpKey = upKey;
        savedLeftKey = leftKey;
        savedDownKey = downKey;
        savedRightKey = rightKey;

        savedPlayer1AssignedKeys = player1AssignedKeys;
        savedPlayer2AssignedKeys = player2AssignedKeys;

        savedPlayer1Name = player1Name;
        savedPlayer2Name = player2Name;

        hasSavedKeys = true;
        useSavedKeysOnNextLoad = true;
    }

    /// <summary>
    /// 저장되어 있던 키와 플레이어 이름을 불러옵니다.
    /// 장애물 충돌 후 같은 키로 재도전할 때 사용합니다.
    /// </summary>
    private void LoadSavedKeys()
    {
        upKey = savedUpKey;
        leftKey = savedLeftKey;
        downKey = savedDownKey;
        rightKey = savedRightKey;

        player1AssignedKeys = savedPlayer1AssignedKeys;
        player2AssignedKeys = savedPlayer2AssignedKeys;

        player1Name = savedPlayer1Name;
        player2Name = savedPlayer2Name;

        hasAssignedKeys = true;
        useSavedKeysOnNextLoad = true;
    }

    /// <summary>
    /// 상하좌우 4방향 중 2개는 Player1의 WASD,
    /// 나머지 2개는 Player2의 방향키로 랜덤 배정합니다.
    /// </summary>
    private void RandomizeMoveKeys()
    {
        int[] directions = { 0, 1, 2, 3 };
        // 0 = 아래, 1 = 위, 2 = 왼쪽, 3 = 오른쪽

        // Fisher-Yates 방식으로 방향 배열을 랜덤 섞기합니다.
        for (int i = 0; i < directions.Length; i++)
        {
            int randomIndex = Random.Range(i, directions.Length);

            int temp = directions[i];
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        player1AssignedKeys = "";
        player2AssignedKeys = "";

        // 랜덤으로 섞인 방향 중 앞의 2개는 Player1에게 배정합니다.
        AssignKey(directions[0], true);
        AssignKey(directions[1], true);

        // 나머지 2개는 Player2에게 배정합니다.
        AssignKey(directions[2], false);
        AssignKey(directions[3], false);

        player1AssignedKeys = player1AssignedKeys.TrimEnd(',', ' ');
        player2AssignedKeys = player2AssignedKeys.TrimEnd(',', ' ');
    }

    /// <summary>
    /// direction 값에 해당하는 실제 KeyCode를 플레이어별로 배정합니다.
    /// </summary>
    private void AssignKey(int direction, bool isPlayer1)
    {
        KeyCode assignedKey;
        string keyText;

        if (isPlayer1)
        {
            assignedKey = GetPlayer1KeyCode(direction);
            keyText = GetPlayer1KeyText(direction);
            player1AssignedKeys += keyText + ", ";
        }
        else
        {
            assignedKey = GetPlayer2KeyCode(direction);
            keyText = GetPlayer2KeyText(direction);
            player2AssignedKeys += keyText + ", ";
        }

        switch (direction)
        {
            case 0:
                downKey = assignedKey;
                break;
            case 1:
                upKey = assignedKey;
                break;
            case 2:
                leftKey = assignedKey;
                break;
            case 3:
                rightKey = assignedKey;
                break;
        }
    }

    private KeyCode GetPlayer1KeyCode(int direction)
    {
        switch (direction)
        {
            case 0: return KeyCode.S;
            case 1: return KeyCode.W;
            case 2: return KeyCode.A;
            case 3: return KeyCode.D;
            default: return KeyCode.None;
        }
    }

    private KeyCode GetPlayer2KeyCode(int direction)
    {
        switch (direction)
        {
            case 0: return KeyCode.DownArrow;
            case 1: return KeyCode.UpArrow;
            case 2: return KeyCode.LeftArrow;
            case 3: return KeyCode.RightArrow;
            default: return KeyCode.None;
        }
    }

    private string GetPlayer1KeyText(int direction)
    {
        switch (direction)
        {
            case 0: return "S";
            case 1: return "W";
            case 2: return "A";
            case 3: return "D";
            default: return "";
        }
    }

    private string GetPlayer2KeyText(int direction)
    {
        switch (direction)
        {
            case 0: return "↓";
            case 1: return "↑";
            case 2: return "←";
            case 3: return "→";
            default: return "";
        }
    }

    /// <summary>
    /// 배정된 키가 눌렸는지 확인하고, 눌린 키에 맞는 이동 방향과 애니메이션을 적용합니다.
    /// </summary>
    private void CheckDirectionInput()
    {
        if (Input.GetKeyDown(upKey))
        {
            SetDirection(Vector2.up, walkUpStateName);
        }
        else if (Input.GetKeyDown(leftKey))
        {
            SetDirection(Vector2.left, walkLeftStateName);
        }
        else if (Input.GetKeyDown(downKey))
        {
            SetDirection(Vector2.down, walkDownStateName);
        }
        else if (Input.GetKeyDown(rightKey))
        {
            SetDirection(Vector2.right, walkRightStateName);
        }
    }

    /// <summary>
    /// 이동 방향을 설정하고, 해당 방향의 걷기 애니메이션을 재생합니다.
    /// </summary>
    private void SetDirection(Vector2 newMoveDirection, string animationStateName)
    {
        moveDirection = newMoveDirection;
        isMoving = true;

        if (animator != null)
        {
            if (!animator.enabled)
            {
                animator.enabled = true;
            }

            animator.Play(animationStateName, 0, 0f);
        }
    }

    /// <summary>
    /// 장애물에 닿았을 때 플레이어 이동과 애니메이션을 즉시 멈춥니다.
    /// 리셋 대기 시간 동안 플레이어가 계속 움직이는 문제를 방지합니다.
    /// </summary>
    public void StopMovementImmediately()
    {
        canMove = false;
        isMoving = false;
        moveDirection = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (animator != null)
        {
            animator.enabled = false;
        }
    }
}
