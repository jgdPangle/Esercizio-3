
using UnityEngine;

public class Player : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    public Camera playerCamera;
    public GameObject target;
    public GameObject targetAxis;

    public float speed, sensibilityH, sensibilityV;
    private float rotateHorizontal, rotateVertical;

    public float lifeMax, lifeMin;

    [SerializeField]
    float tCamera;


    public float attackRange = 5.5f;
    public float attackAngle = 45f;
    public float attackDamage = 10f;

    // Proprietà che gestisce il valore della vita tra Min e Max
    public float life
    {
        get
        {
            return _life;
        }
        set
        {
            if (value > lifeMax)
            {
                _life = lifeMax;
            }
            else if (value < lifeMin)
            {
                _life = lifeMin;
            }
            else
            {
                _life = value;
            }

            // Termina la partita quando il giocatore raggiunge la vita minima
            if (_life <= lifeMin && !NPCs.gameEnded)
            {
                FindFirstObjectByType<NPCs>().EndGame();
            }
        }
    }

    public float _life;

    Vector3 startPosition;
    Vector3 targetPosition;

    float distance;
    float movementProgress;
    bool isMoving = false;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    // Inizializzazione dei parametri del giocatore
    void Start()
    {
        Time.timeScale = 1f;

        lifeMin = 0;
        lifeMax = 300;
        life = lifeMax;
        speed = 4f;
        sensibilityH = 2f;

        Debug.Log("START! Vita: " + life);
        Debug.Log("Attacco = E");
        Debug.Log("Schivata = Space");
    }

    // Aggiornamento delle azioni del giocatore
    void Update()
    {
        if (NPCs.gameEnded) return;

        Movement();
        CameraMove();
        HandleDodge();
        HandleAttack();
    }

    // Gestisce il movimento del giocatore e il suo orientamento
    void Movement()
    {
        Vector2 movimento = inputActions.Player.Move.ReadValue<Vector2>();

        // Movimento normale del giocatore, quando non sta schivando
        if (!isMoving)
        {
            transform.position += transform.forward * movimento.y * speed * Time.deltaTime;
            transform.position += transform.right * movimento.x * speed * Time.deltaTime;
        }
        // Durante la schivata, lo spostamento viene gestito tramite interpolazione
        else
        {
            movementProgress += speed * Time.deltaTime / distance;
            transform.position = Vector3.Lerp(startPosition, targetPosition, movementProgress);

            if (movementProgress >= 1)
            {
                isMoving = false;
            }
        }

        // Allinea la direzione del giocatore con quella della telecamera
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0f;
        transform.forward = Vector3.Slerp(transform.forward, cameraForward, 0.05f);
    }

    public void CameraMove()
    {
        // Sposta la telecamera verso il target con interpolazione e smoothing
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, target.transform.position, Mathf.SmoothStep(0.0f, 1.0f, tCamera));

        // Calcola la rotazione della telecamera in base al movimento del mouse
        rotateHorizontal += inputActions.Player.Look.ReadValue<Vector2>().x * sensibilityH;
        rotateVertical -= inputActions.Player.Look.ReadValue<Vector2>().y * sensibilityV;

        rotateVertical = Mathf.Clamp(rotateVertical, -20f, 60f); // Limita la rotazione verticale tra -20 e 60 gradi

        targetAxis.transform.eulerAngles = new Vector3(rotateVertical, rotateHorizontal, 0.0f);

        playerCamera.transform.LookAt(transform.position);         // Fa in modo che la telecamera guardi sempre il giocatore
    }

    // Controlla l'input per attivare la schivata
    private void HandleDodge()
    {
        if (inputActions.Player.Schivata.WasPressedThisFrame())
        {
            Dodge(2f);
        }
    }

    void Dodge(float distanceDodge)
    {
        startPosition = transform.position;


        Vector3 dodgeDirection = playerCamera.transform.forward; // Usa la direzione della telecamera per la schivata

        dodgeDirection.y = 0;

        dodgeDirection.Normalize();

        targetPosition = startPosition + dodgeDirection * distanceDodge; // Imposta la distanza per la schivata

        movementProgress = 0;
        isMoving = true;
    }

    private void HandleAttack()
    {
        if (inputActions.Player.Attacco.WasPressedThisFrame())
        {
            Attack();
        }
    }

    void Attack()
    {
        Enemies[] enemies = FindObjectsByType<Enemies>(FindObjectsSortMode.None); // Ottiene tutti i nemici presenti nella scena

        foreach (Enemies enemy in enemies)
        {
            Vector3 direction = (enemy.transform.position - transform.position).normalized; // Calcola la direzione verso il nemico

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            float angle = Vector3.Angle(transform.forward, direction);

            // Verifica che il nemico sia entro la portata e l'angolo di attacco
            if (distance <= attackRange && angle <= attackAngle)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log("Nemico colpito! Danno: " + attackDamage);
            }
        }
    }

    // Gestisce il danno ricevuto dal giocatore
    public void TakeDamage(float damageReceived)
    {
        life -= damageReceived;
        Debug.Log("Danno ricevuto: " + damageReceived + ". Vita rimanente: " + life);
    }
}
