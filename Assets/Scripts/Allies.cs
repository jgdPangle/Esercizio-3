
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Allies : MonoBehaviour
{
    public float life, _life, lifeMax, lifeMin, armor, attackRange, damage, damageReceived;

    public bool canSpeak, dead;
    public int suspendendAttacK;

    public List<GameObject> target = new List<GameObject>();

    public Vector3 myPosition;

    public NPCs npcs;
    public Skeletons skeletons;
    public Orc orc;

    // Inizializzazione delle caratteristiche dell'alleato
    protected virtual void Start()
    {
        dead = false;
        canSpeak = true;
        suspendendAttacK = 0;
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
        if (!dead && canSpeak && target != null && !skeletons.defeatedSkeletons)
        {
            Debug.Log("Eccoli! Arrivano da + " + npcs.spawnPos);
        }
        else if (!dead && canSpeak && target != null && !orc.defeatedOrc)
        {
            Debug.Log("Fai attenzione e sii veloce o morirai!!!");
        }
        else if (skeletons.defeatedSkeletons && orc.defeatedOrc)
        {
            Debug.Log("Che bello abbiamo sconfitto tutti i nemici!");
        }
    }
}

