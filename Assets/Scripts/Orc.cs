using UnityEngine;
using System.Collections;

public class Orc : Enemies
{
    [SerializeField]
    float speed, distance, minimumDistance;

    public bool defeatedOrc;

    NPCs npcs;

    // Inizializzazione delle caratteristiche dell'Orco
    protected override void Start()
    {
        armor = 40;
        lifeMax = 150;
        lifeMin = 0;
        attackRange = 2.5f;
        damage = 20;
        minimumDistance = 2f;
        speed = 2f;

        defeatedOrc = false;

        // Richiama lo Start della classe base Enemies
        base.Start();

        // Imposta la vita iniziale sommando vita massima e armatura
        life = lifeMax + armor;

        Debug.Log("ORCO APPARSO! Vita: " + life + " | Armatura: " + armor);
    }

    void Update()
    {
        // Controllo che il gioco non sia terminato prima di eseguire le azioni dell'Orco
        if (NPCs.gameEnded) return;

        OrcoMove();
        LostArmor();
        DefeatedOrc();
    }

    // Gestisce il movimento dell'Orco verso il giocatore
    public void OrcoMove()
    {
        // Impedisce il movimento se manca il bersaglio o l'Orco è incapacitato
        if (target == null || incapacitated) return;

        // Calcola la distanza dal bersaglio
        distance = Vector3.Distance(transform.position, target.transform.position);

        // Avvicina l'Orco al bersaglio fino alla distanza minima
        if (distance > minimumDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.transform.position,
                speed * Time.deltaTime
            );
        }
    }

    // Imposta il giocatore come bersaglio dell'Orco
    public void SetTargetMovement(Player targetMovement)
    {
        target = targetMovement.gameObject;
    }

    // Gestisce la distruzione dell'armatura e il passaggio alla seconda fase
    public void LostArmor()
    {
        if (life <= armor && armor > 0)
        {
            armor = 0;
            life = lifeMax;

            Debug.Log("ARMATURA DISTRUTTA! Seconda fase! Vita ripristinata: " + life);
        }
    }

    // Coroutine che gestisce l'attacco in carica dell'Orco: sovrascrive il metodo Attack della classe base Enemies
    protected override IEnumerator Attack()
    {
        while (target != null)
        {
            GameObject currentTarget = target;

            Player playerTarget = currentTarget.GetComponent<Player>();
            Allies allyTarget = currentTarget.GetComponent<Allies>();

            // Controlla se il bersaglio è entro la portata di attacco
            if (Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange)
            {
                if (!incapacitated)
                {
                    // Calcola la direzione della carica verso il bersaglio
                    Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
                    direction.y = 0;

                    float elapsedTime = 0f;

                    // Esegue la carica per 0,3 secondi
                    while (elapsedTime < 0.3f)
                    {
                        transform.position += direction * 20f * Time.deltaTime;
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    // Infligge danno al giocatore o all'alleato
                    if (playerTarget != null)
                    {
                        playerTarget.TakeDamage(damage);
                    }
                    else if (allyTarget != null)
                    {
                        allyTarget.TakeDamage(damage);
                    }

                    // Blocca temporaneamente l'Orco dopo l'attacco
                    incapacitated = true;
                    yield return new WaitForSeconds(5f);
                    incapacitated = false;
                }
            }

            // Attende 0.5 secondi prima del controllo successivo
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Assegna il riferimento allo script NPCs
    public void SetNPCs(NPCs npcManager)
    {
        npcs = npcManager;
    }

    // Gestisce la sconfitta dell'Orco e la fine della partita
    public void DefeatedOrc()
    {
        if (life <= lifeMin && !dead)
        {
            defeatedOrc = true;
            dead = true;

            Debug.Log("ORCO SCONFITTO! HAI VINTO!");

            // Il Nano comunica la vittoria prima della distruzione dell'Orco
            npcs.dwarf.CanTalk();

            Destroy(gameObject);

            npcs.EndGame();
        }
    }
}

