using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    /*
    [SerializeField] private KeyCode[] moveRightKey = { KeyCode.D, KeyCode.RightArrow };
    [SerializeField] private KeyCode[] moveLeftKey = { KeyCode.A, KeyCode.LeftArrow };
    [SerializeField] private KeyCode[] jumpKey = { KeyCode.Space, KeyCode.W, KeyCode.UpArrow};
    */

    public static Player Instance;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float detectRadius = 1.1f;

    static internal Rigidbody2D Rigidbody;
    static internal Transform Transform;
    static internal Animator Animator;

    public UnityEvent OnCharacterStartRunning = new UnityEvent();
    public UnityEvent OnCharacterStartIdle = new UnityEvent();
    public UnityEvent OnCharacterJump = new UnityEvent();
    public UnityEvent OnCharacterLand = new UnityEvent();

    private bool isLandOnGround
    {
        get
        {
            return Physics2D.Raycast(Transform.position, Vector2.down, detectRadius, groundLayer);
        }
    }

    private bool isMoving
    {
        get
        {
            return Mathf.Abs(Rigidbody.linearVelocity.x) > 0.1f;
        }
    }

    private bool wasLandOnGround = false;
    private bool wasMoving = false;

    // 初始化游戏和玩家实例
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Transform = GetComponent<Transform>();
        Animator = GetComponent<Animator>();

        Instance = this;
        OnCharacterStartRunning = new UnityEvent();
        OnCharacterStartIdle = new UnityEvent();
        OnCharacterJump = new UnityEvent();
        OnCharacterLand = new UnityEvent();

        AddSwitchAnimatorEvent();
    }

    // 重置实例
    void OnDestroy()
    {
        Instance = null;
    }

    public void AddSpeed(float newSpeed, float holdingTime)
    {
        StartCoroutine(AddSpeedCoroutine(newSpeed, holdingTime));
    }

    public void Cheat()
    {
        Debug.Log("作弊模式已开启");

        moveSpeed = 10f;
        jumpPower = 6f;
    }

    void Update()
    {
        // 处理输入
        HandleInputAndMove();
        HandleInputAndJump();

        CheckLanding();
        CheckMoving();

        EditorDebug();
    }

    void HandleInputAndMove()
    {
        // 处理转向
        if (Input.GetAxis("Horizontal") > 0)
        {
            Transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            Transform.localScale = new Vector3(-1, 1, 1);
        }

        // 处理移动
        float horizontalInput = Input.GetAxis("Horizontal");
        Rigidbody.linearVelocity = new Vector2(horizontalInput * moveSpeed, Rigidbody.linearVelocity.y);
    }

    void HandleInputAndJump()
    {
        // 处理跳跃和跳跃事件
        if (Input.GetButtonDown("Jump") && isLandOnGround)
        {
            Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, jumpPower);
            OnCharacterJump.Invoke();
        }
    }

    void AddSwitchAnimatorEvent()
    {
        OnCharacterStartRunning.AddListener(SwitchAnimatorToRunning);
        OnCharacterStartIdle.AddListener(SwitchAnimatorToIdle);
    }

    void SwitchAnimatorToRunning()
    {
        Animator.SetBool("IsRunning", true);
    }

    void SwitchAnimatorToIdle()
    {
        Animator.SetBool("IsRunning", false);
    }

    void CheckLanding()
    {
        bool currentIsLandOnGround = isLandOnGround;
        if (!wasLandOnGround && currentIsLandOnGround)
        {
            OnCharacterLand.Invoke();
        }
        wasLandOnGround = currentIsLandOnGround;
    }

    void CheckMoving()
    {
        bool currentIsMoving = isMoving;
        if (!wasMoving && currentIsMoving)
        {
            OnCharacterStartRunning.Invoke();
        }
        else if (wasMoving && !currentIsMoving)
        {
            OnCharacterStartIdle.Invoke();
        }
        wasMoving = currentIsMoving;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.down * detectRadius);
    }

    IEnumerator AddSpeedCoroutine(float newSpeed, float holdingTime)
    {
        float originalSpeed = moveSpeed;
        moveSpeed = newSpeed;

        if (holdingTime != 0)
        {
            yield return new WaitForSeconds(holdingTime);
            moveSpeed = originalSpeed;
        }
    }

    void EditorDebug()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}