using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCs : MonoBehaviour
{
    public Player player;
    Dwarf dwarf;

    public List<GameObject> Allies;
    public List<GameObject> Enemies;

    public GameObject entities;

    public int skeletonCount, skeletonDefeatedCount, skeletonsToSpawn;

    [SerializeField]
    float spawnDistance = 10f;

    public float spawnPos;
    Vector3[] directions;

    public static bool gameEnded = false;

    void Start()
    {
        gameEnded = false;
        Time.timeScale = 1f;

        // Inizializzazione dei contatori degli scheletri
        skeletonsToSpawn = 15;
        skeletonCount = 15;
        skeletonDefeatedCount = 0;

        // Generazione del Nano e degli scheletri
        InstantiateNano();
        InstanSkeletons();
    }

    // Istanzia il Nano
    public void InstantiateNano()
    {
        GameObject dwarfObject = Instantiate(Allies[0], entities.transform);
        dwarf = dwarfObject.GetComponent<Dwarf>();

        dwarf.SetTargetMovement(player); // Imposta il riferimento al giocatore per il movimento del Nano
        dwarf.StartHealing();
    }

    // Avvia la coroutine per generare gli scheletri
    public void InstanSkeletons()
    {
        StartCoroutine(InstantiateSkeletons());
    }

    IEnumerator InstantiateSkeletons()
    {
        // Definisce le quattro direzioni di generazione rispetto al giocatore
        directions = new Vector3[]{-player.transform.forward, player.transform.forward, -player.transform.right, player.transform.right};

        for (int i = 0; i < skeletonsToSpawn; i++)
        {
            // Seleziona casualmente una direzione
            int directionIndex = Random.Range(0, directions.Length);
            Vector3 direction = directions[directionIndex];

            // Calcola la posizione di generazione dello scheletro in base alla direzione selezionata e alla distanza dal player
            Vector3 spawnPosition = player.transform.position + direction * spawnDistance;

            // Istanzia lo scheletro
            Skeletons skeleton = Instantiate(Enemies[0], spawnPosition, Quaternion.identity, entities.transform).GetComponent<Skeletons>();

            // Imposta i target dello scheletro
            skeleton.SetTargetMovement(player);
            skeleton.SetNPCs(this);
            skeleton.SetAllyTarget(dwarf.gameObject);

            // Aggiunge lo scheletro alla lista dei bersagli del Nano
            dwarf.AddTarget(skeleton.gameObject);

            // Comunica dalla quale direzione arrivano i nemici
            if (directionIndex == 0)
            {
                spawnPos = 1;
                Debug.Log("I nemici arrivano da davanti!");
            }
            else if (directionIndex == 1)
            {
                spawnPos = 2;
                Debug.Log("I nemici arrivano da dietro!");
            }
            else if (directionIndex == 2)
            {
                spawnPos = 3;
                Debug.Log("I nemici arrivano da destra!");
            }
            else if (directionIndex == 3)
            {
                spawnPos = 4;
                Debug.Log("I nemici arrivano da sinistra!");
            }

            yield return new WaitForSeconds(3f);
        }
    }

    // Istanzia l'Orco 
    public void InstantiateOrco()
    {
        GameObject orco = Instantiate(Enemies[1], entities.transform);

        Orc orcScript = orco.GetComponent<Orc>();

        // Imposta i target dell'Orco
        orcScript.SetTargetMovement(player);
        orcScript.SetAllyTarget(dwarf.gameObject);
        orcScript.SetNPCs(this);

        // Aggiunge l'Orco ai bersagli del Nano
        dwarf.AddTarget(orco);
    }

    // Gestisce la fine della partita
    public void EndGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        Debug.Log("THE END!");
    }
}

