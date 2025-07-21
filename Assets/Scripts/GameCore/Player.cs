using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace GameCore
{
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
    
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private float deathY = -5f;

        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip gameOverSound;
        [SerializeField] private AudioClip gameWinSound;
        
        [SerializeField] private int levelTotalSeconds = 180;
        
        private Vector2 _playerInitialPosition;
        
        private static Rigidbody2D _rigidbody;
        private static Transform _transform;
        private static Animator _animator;
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        
        public static float LevelTime;

        public static UnityEvent OnCharacterStartRunning = new UnityEvent();
        public static UnityEvent OnCharacterStartIdle = new UnityEvent();
        public static UnityEvent OnCharacterJump = new UnityEvent();
        public static UnityEvent OnCharacterLand = new UnityEvent();
        public static UnityEvent OnGameOver = new UnityEvent();
        public static UnityEvent OnGameWin = new UnityEvent();

        private bool IsLandOnGround => Physics2D.Raycast(_transform.position, Vector2.down, detectRadius, groundLayer);

        private static bool IsMoving => Mathf.Abs(_rigidbody.linearVelocity.x) > 0.1f;

        private bool _wasLandOnGround;
        private bool _wasMoving;

        // 初始化游戏和玩家实例
        void Awake()
        {
            LevelManager.InitializeLevel();
            
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = GetComponent<Transform>();
            _animator = GetComponent<Animator>();

            Instance = this;
            OnCharacterStartRunning = new UnityEvent();
            OnCharacterStartIdle = new UnityEvent();
            OnCharacterJump = new UnityEvent();
            OnCharacterLand = new UnityEvent();
            
            LevelTime = levelTotalSeconds;
            
            _playerInitialPosition = _transform.position;
            
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

            AddSwitchAnimatorEvent();
        }

        // 重置实例
        void OnDestroy()
        {
            Instance = null;
        }
    
        void Update()
        {
            if (LevelManager.CurrentLevelState == LevelManager.LevelState.Playing)
            {
                HandleInputAndMove();
                HandleInputAndJump();
                
                UpdateTimer();
            }

            CheckLanding();
            CheckMoving();
            CheckGameStatus();

            EditorDebug();
        }

        void CheckGameStatus()
        {
            if  (LevelManager.CurrentLevelState == LevelManager.LevelState.Playing && _transform.position.y < deathY)
            {
                LevelManager.CurrentLevelState = LevelManager.LevelState.Waiting;
                Death(false);
            }

            if (LevelManager.CurrentLevelState == LevelManager.LevelState.GameOver)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
        
        void UpdateTimer()
        {
            LevelTime -= Time.deltaTime;
            if (LevelTime <= 0)
            {
                LevelTime = 0;
                LevelManager.CurrentLevelState = LevelManager.LevelState.Waiting;
                GameOver();
            }
        }

        public static void PlayAudioClip(AudioClip audioClip)
        {
            GameObject audioGameObject = new GameObject("Audio " + audioClip.name);
            audioGameObject.AddComponent<AudioSource>().clip = audioClip;
            audioGameObject.GetComponent<AudioSource>().Play();
            Destroy(audioGameObject, audioClip.length);
        }

        public static void StopPlayerAudioSource()
        {
            AudioSource[] audioSources = Instance.GetComponents<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Stop();
            }
        }
        
        public static void StartPlayerAudioSource()
        {
            AudioSource[] audioSources = Instance.GetComponents<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Play();
            }
        }

        public void Death(bool haveAnimation = true)
        {
            cinemachineCamera.Follow = null;
            LevelManager.CurrentLevelState = LevelManager.LevelState.Waiting;

            if (haveAnimation)
            {
                _rigidbody.simulated = false;
                _transform.DOLocalMoveY(1f, 0.3f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    _transform.DOLocalMoveY(-10f, 1.5f).SetEase(Ease.OutSine);
                });
            }
                
            
            StopPlayerAudioSource();
            PlayAudioClip(deathSound);
            
            if (LevelManager.CanRevive)
            {
                LevelManager.CurrentLife--;
                Invoke(nameof(Respawn), deathSound.length + 1f);
            }
            else
            {
                Invoke(nameof(GameOver), deathSound.length + 1f);
            }
        }

        void Respawn()
        {
            if (LevelManager.LatestCheckPoint != null)
            {
                Vector3 position = new Vector3(LevelManager.LatestCheckPoint.position.x, LevelManager.LatestCheckPoint.position.y + 1f, LevelManager.LatestCheckPoint.position.z);
                _transform.position = position;
            }
            else
            {
                _transform.position = _playerInitialPosition;
            }
            
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.simulated = true;
            cinemachineCamera.Follow = _transform;
            
            LevelManager.CurrentLevelState = LevelManager.LevelState.Playing;
            
            StartPlayerAudioSource();
        }

        void ResetRigidbody()
        {
            _rigidbody.simulated = true;
            _rigidbody.linearVelocity = Vector2.zero;
        }

        void GameOver()
        {
            OnGameOver?.Invoke();
            
            StopPlayerAudioSource();
            LevelManager.CurrentLevelState = LevelManager.LevelState.GameOver;
            PlayAudioClip(gameOverSound);
        }

        public void GameWin()
        {
            OnGameWin?.Invoke();
            
            ResetRigidbody();
            
            StopPlayerAudioSource();
            LevelManager.CurrentLevelState = LevelManager.LevelState.Win;
            PlayAudioClip(gameWinSound);
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

        void HandleInputAndMove()
        {
            // 处理转向
            if (Input.GetAxis("Horizontal") > 0)
            {
                _transform.localScale = new Vector3(1, 1, 1);
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                _transform.localScale = new Vector3(-1, 1, 1);
            }

            // 处理移动
            float horizontalInput = Input.GetAxis("Horizontal");
            bool canMove = true;
            
            // 检测前方是否有障碍物
            float rayDistance = 0.5f;
            Vector2 rayOrigin = (Vector2)_transform.position + new Vector2(0, -0.4f);
            LayerMask obstacleLayer = LayerMask.GetMask("Ground");

            if (horizontalInput > 0)
            {
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayDistance, obstacleLayer);
                if (hit.collider)
                {
                    canMove = false;
                }
            }
            else if (horizontalInput < 0)
            {
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, rayDistance, obstacleLayer);
                if (hit.collider)
                {
                    canMove = false;
                }
            }

            if (canMove)
            {
                _rigidbody.linearVelocity = new Vector2(horizontalInput * moveSpeed, _rigidbody.linearVelocity.y);
            }
        }

        void HandleInputAndJump()
        {
            // 处理跳跃和跳跃事件
            if (Input.GetButtonDown("Jump") && IsLandOnGround)
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpPower);
                OnCharacterJump.Invoke();
                
                PlayAudioClip(jumpSound);
            }
        }

        void AddSwitchAnimatorEvent()
        {
            OnCharacterStartRunning.AddListener(SwitchAnimatorToRunning);
            OnCharacterStartIdle.AddListener(SwitchAnimatorToIdle);
        }

        void SwitchAnimatorToRunning()
        {
            _animator.SetBool(IsRunning, true);
        }

        void SwitchAnimatorToIdle()
        {
            _animator.SetBool(IsRunning, false);
        }

        void CheckLanding()
        {
            bool currentIsLandOnGround = IsLandOnGround;
            if (!_wasLandOnGround && currentIsLandOnGround)
            {
                OnCharacterLand.Invoke();
            }
            _wasLandOnGround = currentIsLandOnGround;
        }

        void CheckMoving()
        {
            bool currentIsMoving = IsMoving;
            if (!_wasMoving && currentIsMoving)
            {
                OnCharacterStartRunning.Invoke();
            }
            else if (_wasMoving && !currentIsMoving)
            {
                OnCharacterStartIdle.Invoke();
            }
            _wasMoving = currentIsMoving;
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
}