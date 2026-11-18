using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Allies : MonoBehaviour
{
    public float life, lifeMax, lifeMin, armor, attackRange, damage, damageReceived;

    public bool canSpeak, dead;

    public List<GameObject> target = new List<GameObject>();

    public NPCs npcs;
    public Orc orc;

    // Inizializzazione delle caratteristiche dell'alleato
    protected virtual void Start()
    {
        dead = false;
        canSpeak = true;
        life = lifeMax;

        // Avvia la coroutine di attacco
        StartAttack();
    }

    // Gestisce il danno ricevuto dall'alleato
    public void TakeDamage(float damageReceived)
    {
        life -= damageReceived;
        Debug.Log("Il tuo alleato ha subito " + damageReceived + " danni, la sua vita attuale è: " + life);
    }

    // Aggiunge un nuovo nemico alla lista dei bersagli
    public void AddTarget(GameObject newTarget)
    {
        target.Add(newTarget);
    }

    // Coroutine che gestisce gli attacchi dell'alleato
    IEnumerator Attack()
    {
        while (target != null)
        {
            foreach (GameObject t in target)
            {
                // Controlla che il bersaglio esista e sia entro la portata di attacco
                if (t != null && Vector3.Distance(t.transform.position, transform.position) <= attackRange)
                {
                    // Infligge danno al nemico
                    t.GetComponent<Enemies>().TakeDamage(damage);
                    Debug.Log($"Il tuo alleato {gameObject.name} ha colpito {t.name}!");
                }
            }

            // Attende 3 secondi prima del controllo successivo
            yield return new WaitForSeconds(3f);
        }
    }

    // Avvia la coroutine di attacco
    public void StartAttack()
    {
        StartCoroutine(Attack());
    }

    // Gestisce i messaggi pronunciati dall'alleato
    public void CanTalk()
    {
        if (dead || !canSpeak || npcs == null) return;

        // Controlla se tutti gli scheletri sono stati sconfitti
        bool allSkeletonsDefeated = npcs.skeletonDefeatedCount >= npcs.skeletonsToSpawn;

        if (!allSkeletonsDefeated)
        {
            string direction = "";

            switch (npcs.spawnPos)
            {
                case 1:
                    direction = "davanti";
                    break;

                case 2:
                    direction = "dietro";
                    break;

                case 3:
                    direction = "a destra";
                    break;

                case 4:
                    direction = "a sinistra";
                    break;
            }

            Debug.Log("Nano: Eccoli! Arrivano da " + direction + "!");
        }
        else if (npcs.orcSpawned && orc != null && !orc.defeatedOrc)
        {
            Debug.Log("Nano: Fai attenzione e sii veloce o morirai!!!");
        }
        else if (npcs.orcSpawned && orc != null && orc.defeatedOrc)
        {
            Debug.Log("Nano: Che bello abbiamo sconfitto tutti i nemici!");
        }
    }
}

