using UnityEngine;

public class Skeletons : Enemies
{
    [SerializeField]
    float speed, distance, minimumDistance;
    public bool defeatedSkeletons;

    NPCs npcs;

    // Inizializzazione delle caratteristiche dello scheletro
    protected override void Start()
    {
        lifeMax = 100;
        lifeMin = 0;
        attackRange = 5f;
        damage = 5;
        speed = 8f;
        minimumDistance = 4f;
        defeatedSkeletons = false;

        // Richiama lo Start della classe base Enemies
        base.Start();
    }

    void Update()
    {
        // Controllo che il gioco non sia terminato prima di eseguire le azioni degli scheletri
        if (NPCs.gameEnded) return;

        SkeletonsMove();
        SkeletonsDefeated();
    }

    // Gestisce il movimento dello scheletro verso il giocatore
    public void SkeletonsMove()
    {
        //controllo che il bersaglio esista prima di calcolare la distanza e muoversi
        if (target == null) return;

        // Calcola la distanza dal bersaglio
        distance = Vector3.Distance(transform.position, target.transform.position);

        // Avvicina lo scheletro fino alla distanza minima
        if (distance > minimumDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.transform.position,
                speed * Time.deltaTime
            );
        }
    }

    // Imposta il giocatore come bersaglio dello scheletro
    public void SetTargetMovement(Player targetMovement)
    {
        target = targetMovement.gameObject;
    }

    // Assegna il riferimento allo script NPCs
    public void SetNPCs(NPCs npcManager)
    {
        npcs = npcManager;
    }

    // Controlla se lo scheletro è stato sconfitto
    public void SkeletonsDefeated()
    {
        if (life <= 0 && !dead)
        {
            defeatedSkeletons = true;
            dead = true;

            // Aggiorna i contatori degli scheletri
            npcs.skeletonCount--;
            npcs.skeletonDefeatedCount++;

            Destroy(gameObject);

            Debug.Log("Scheletro eliminato. Scheletri rimanenti: " + npcs.skeletonCount);

            // Istanzia l'Orco quando tutti gli scheletri sono stati sconfitti
            if (npcs.skeletonDefeatedCount == npcs.skeletonsToSpawn)
            {
                npcs.InstantiateOrco();
            }
        }
    }
}

