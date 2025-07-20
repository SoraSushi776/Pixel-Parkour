using UnityEngine;

namespace LevelObjects
{
    public class MovePlatform : MonoBehaviour
    {
        public Transform startPoint;
        public Transform endPoint;
        public float moveSpeed = 1f;
        public float waitTime = 2f;

        private Vector2 _startPos;
        private Vector2 _endPos;
        private Vector2 _movePos;
        private float _waitTimer;
        private bool _isWaiting;

        void Start()
        {
            _startPos = startPoint.position;
            _endPos = endPoint.position;

            _movePos = _startPos;

            Destroy(startPoint.gameObject);
            Destroy(endPoint.gameObject);
        }

        void Update()
        {
            if (_isWaiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= waitTime)
                {
                    _isWaiting = false;
                    _waitTimer = 0f;
                }
                return;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, _movePos, moveSpeed * Time.deltaTime);
            }

            if (Vector2.Distance(transform.position, _endPos) < 0.01f)
            {
                _movePos = _startPos;
                _isWaiting = true;
            }
            else if (Vector2.Distance(transform.position, _startPos) < 0.01f)
            {
                _movePos = _endPos;
                _isWaiting = true;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(transform);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(null);
            }
        }
    }
}