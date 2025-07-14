using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public float MoveSpeed = 1f;
    public float WaitTime = 2f; // 新增：等待时间

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 movePos;
    private float waitTimer = 0f; // 新增：等待计时器
    private bool isWaiting = false; // 新增：是否正在等待

    void Start()
    {
        startPos = StartPoint.position;
        endPos = EndPoint.position;

        movePos = startPos;

        Destroy(StartPoint.gameObject);
        Destroy(EndPoint.gameObject);
    }

    void Update()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= WaitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            return;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, movePos, MoveSpeed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, endPos) < 0.1f)
        {
            movePos = startPos;
            isWaiting = true;
        }
        else if (Vector2.Distance(transform.position, startPos) < 0.1f)
        {
            movePos = endPos;
            isWaiting = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.transform.SetParent(transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.transform.SetParent(null);
        }
    }
}