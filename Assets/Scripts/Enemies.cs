using System.Collections;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float life, lifeMax, lifeMin, _life, armor, attackRange, damage, damageReceived;

    public bool dead, incapacitated;
    public GameObject target;

    public GameObject alliesTarget;

    public Vector3 myPosition;

    protected virtual void Start()
    {
        dead = false;
        incapacitated = false;
        life = lifeMax;

        // Avvia la coroutine di attacco
        StartAttack();
    }

    // Gestisce il danno ricevuto dal nemico
    public void TakeDamage(float damageReceived)
    {
        life -= damageReceived;
    }

    // Assegna il Nano come possibile bersaglio
    public void SetAllyTarget(GameObject ally)
    {
        alliesTarget = ally;
    }

    // Coroutine che gestisce la scelta del bersaglio e l'attacco
    protected virtual IEnumerator Attack()
    {
        while (target != null)
        {
            // Il bersaglio iniziale è il giocatore
            GameObject currentTarget = target;

            // Controlla se è presente anche l'alleato
            if (alliesTarget != null)
            {
                // Calcola la distanza del nemico dal giocatore e dall'alleato
                float playerDistance = Vector3.Distance(transform.position, target.transform.position);
                float allyDistance = Vector3.Distance(transform.position, alliesTarget.transform.position);

                // Se l'alleato è più vicino del player diventa il bersaglio
                if (allyDistance < playerDistance)
                {
                    currentTarget = alliesTarget;
                }
            }

            // Controlla se il bersaglio si trova entro la portata di attacco
            if (Vector3.Distance(currentTarget.transform.position, transform.position) <= attackRange)
            {
                if (!incapacitated)
                {
                    // Recupera il componente del bersaglio
                    Player playerTarget = currentTarget.GetComponent<Player>();
                    Allies ally = currentTarget.GetComponent<Allies>();

                    // Infligge danno al giocatore o all'alleato
                    if (playerTarget != null)
                    {
                        playerTarget.TakeDamage(damage);
                    }
                    else if (ally != null)
                    {
                        ally.TakeDamage(damage);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(5f);
                    incapacitated = false;
                }
            }
            // Intervallo tra un controllo e il successivo
            yield return new WaitForSeconds(3f);
        }
    }

    // Avvia la coroutine di attacco
    public virtual void StartAttack()
    {
        StartCoroutine(Attack());
    }
}

