using UnityEngine;
using System.Collections;

public class Dwarf : Allies
{
    [SerializeField]
    float speed, distance, minimumDistance;

    GameObject targetMovement;
    Player player;

    // Inizializzazione delle caratteristiche del Nano
    protected override void Start()
    {
        lifeMax = 200;
        lifeMin = 0;
        attackRange = 6f;
        damage = 5;
        minimumDistance = 1.5f;

        base.Start(); // Richiama lo Start della classe base Allies
    }

    void Update()
    {
        if (NPCs.gameEnded) return;

        DwarfMove();
        DwarfDefeated();
    }

    // Gestisce il movimento del Nano verso il giocatore
    public void DwarfMove()
    {
        if (targetMovement == null) return;

        distance = Vector3.Distance(transform.position, targetMovement.transform.position); // Calcola la distanza dal giocatore

        if (distance > minimumDistance) // Se la distanza è maggiore della distanza minima, il Nano si avvicina al giocatore
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetMovement.transform.position,
                speed * Time.deltaTime
            );
        }
    }

    // Imposta il giocatore da seguire e la velocità del Nano
    public void SetTargetMovement(Player targetMov)
    {
        player = targetMov;
        targetMovement = targetMov.gameObject;
        speed = player.speed;
    }

    // Coroutine che gestisce la cura del giocatore
    IEnumerator Healing()
    {
        // Attende un frame prima di iniziare la cura
        yield return null;

        while (player.life > player.lifeMin)
        {
            // Cura il giocatore se la sua vita è inferiore a 150
            if (player.life < 150)
            {
                player.life += 10;
                Debug.Log("Sei stato curato di 10 punti vita, la tua vita attuale è: " + player.life);
            }

            // Attende 3 secondi prima della cura successiva
            yield return new WaitForSeconds(3f);
        }
    }

    // Avvia la coroutine di cura
    public void StartHealing()
    {
        StartCoroutine(Healing());
    }

    // Controlla se il Nano è stato sconfitto
    public void DwarfDefeated()
    {
        if (life <= 0 && !dead)
        {
            dead = true;
            Destroy(gameObject);
            Debug.Log("Il nano è stato sconfitto!");
        }
    }
}

