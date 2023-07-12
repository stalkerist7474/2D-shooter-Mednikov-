using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour
{
    public static Player LocalPlayer;
    public int Money;
    public Vector2 MovementInput;

    [SerializeField] public float _speed;
    [SerializeField] private float _timeSmoothMove = 0.1f;
    [SerializeField] private int _maxHeath;
    [SerializeField] private int _rewardCoin;
    [SerializeField] public Transform _shootpoint;
    [SerializeField] private Bullet _bulletTemplate;

    private Vector3 _currenPosition;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private SpriteRenderer _spriteRenderer;
    private NetworkMatch networkMatch;
    private int _currentHeath;
    private GameObject _gameUI;

    [SyncVar] public string matchID;

    public event UnityAction<int, int> HealthChanged;
    public event UnityAction<int> MoneyChanged;

    private void Awake()
    {
        _gameUI = GameObject.FindGameObjectWithTag("GameUI");
    }

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        networkMatch = GetComponent<NetworkMatch>();

        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.color = RandomColor();

        _currentHeath = _maxHeath;



        if (isLocalPlayer)
        {


            LocalPlayer = this;

        }
        else
        {
            Mainmenu.Instance.SpawnPlayerUIPrefab(this);
        }
    }

    public void HostGame()
    {
        string ID = Mainmenu.GetID();
        CmdHostGame(ID);
    }
    [Command]
    public void CmdHostGame(string ID)
    {
        matchID = ID;
        if (Mainmenu.Instance.HostGame(ID, gameObject))
        {
            Debug.Log("лобби создано");
            networkMatch.matchId = ID.ToGuid();
            TargetHostGame(true, ID);
        }
        else
        {
            Debug.Log("ошибка в создании лобби");
            TargetHostGame(false, ID);
        }
    }

    [TargetRpc]

    void TargetHostGame(bool success, string ID)
    {
        matchID = ID;
        Debug.Log($"ID {matchID} == {ID}");
        Mainmenu.Instance.HostSuccess(success, ID);
    }
    //===========================================================================================================//
    //===========================================================================================================//
    //===========================================================================================================//
    public void JoinGame(string inputID)
    {

        CmdJoinGame(inputID);
    }
    [Command]
    public void CmdJoinGame(string ID)
    {
        matchID = ID;
        Debug.Log(Mainmenu.Instance.JoinGame(ID, gameObject));
        if (Mainmenu.Instance.JoinGame(ID, gameObject))
        {
            Debug.Log("успешное подключение к лобби");
            networkMatch.matchId = ID.ToGuid();
            TargetJoinGame(true, ID);
        }
        else
        {
            Debug.Log("не удалось подключиться к лобби");
            TargetJoinGame(false, ID);
        }
    }

    [TargetRpc]

    void TargetJoinGame(bool success, string ID)
    {
        matchID = ID;
        Debug.Log($"ID {matchID} == {ID}");
        Mainmenu.Instance.JoinSuccess(success, ID);
    }
    //===========================================================================================================//
    //===========================================================================================================//
    //===========================================================================================================//

    public void BeginGame()
    {

        CmdBeginGame();
    }
    [Command]
    public void CmdBeginGame()
    {
        Mainmenu.Instance.BeginGame(matchID);
        Debug.Log($"игра началась");
    }


    public void StartGame()
    {
        TargetBeginGame();
    }

    [TargetRpc]

    void TargetBeginGame()
    {
        Debug.Log($"ID {matchID} Start");
        Player[] players = FindObjectsOfType<Player>();

        for (int i = 0; i < players.Length; i++)
        {
            DontDestroyOnLoad(players[i]);
        }

        Mainmenu.Instance.InGame = true;
        transform.localScale = new Vector3(1, 1, 1);
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);

    }

    [Server]
    public void PlayerCountUpdated(int playerCount)
    {
        TargetPlayerCountUpdated(playerCount);
    }

    [TargetRpc]
    void TargetPlayerCountUpdated(int playerCount)
    {
        if (playerCount > 1)
        {
            Mainmenu.Instance.SetBeginButtonActive(true);
        }
        else
        {
            Mainmenu.Instance.SetBeginButtonActive(false);
        }
    }





    private void FixedUpdate()
    {


        if (isLocalPlayer)
        {


            _rigidbody2D.velocity = Vector3.zero;

            if (!MovementInput.Equals(new Vector2(0, 0)))
            {

                _smoothedMovementInput = Vector2.SmoothDamp(_smoothedMovementInput, MovementInput, ref _movementInputSmoothVelocity, _timeSmoothMove);


                transform.rotation = Quaternion.LookRotation(Vector3.forward, MovementInput);


                _rigidbody2D.velocity = _smoothedMovementInput * _speed;
            }

        }



    }



    private void OnMove(InputValue inputValue)
    {
        if (isLocalPlayer)
        {

            MovementInput = inputValue.Get<Vector2>();
            Debug.Log("Move");
            Debug.Log(MovementInput);
        }

    }

    [Command]
    private void OnFire()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Fire");
            Shoot();

        }
    }

    class mRandom
    {
        public static System.Random random = new System.Random();
        public static double GetRandomNumber(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
    Color RandomColor()
    {
        float r = (float)mRandom.GetRandomNumber(0, 1f);
        float g = (float)mRandom.GetRandomNumber(0, 1f);
        float b = (float)mRandom.GetRandomNumber(0, 1f);
        float a = 0.7f;
        return new Color(r, g, b, a);
    }

    [TargetRpc]
    private void Shoot()
    {
        Instantiate(_bulletTemplate, _shootpoint.position, transform.rotation);
        //NetworkServer.Spawn(bullet.gameObject);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            Money += _rewardCoin;
            MoneyChanged?.Invoke(Money);
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHeath -= damage;
        HealthChanged?.Invoke(_currentHeath, _maxHeath);

        if (_currentHeath <= 0)
        {

            Destroy(gameObject);
        }
    }



}
