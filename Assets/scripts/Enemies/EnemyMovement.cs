using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float damage;
    [SerializeField] private float visionDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;
    [SerializeField] private LayerMask attackable;
    [SerializeField] private float timer;
    [SerializeField] private bool attackPossible;
    private float distanceToTarget;
    NavMeshAgent agent;


    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;
        timer = 0;
        attackPossible = true;
    }
    private void Update()
    {
        if (!attackPossible)
        {
            timer += Time.deltaTime;
        }
        
        if (timer > attackCooldown)
        {
            attackPossible = true;
            timer = 0;
        }

        distanceToTarget = Vector3.Distance(transform.position, target.position);
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        if (distanceToTarget < attackDistance && attackPossible)
        {
            Attack();
        }
        else if (distanceToTarget > attackDistance && distanceToTarget < visionDistance) 
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    private void Idle()
    {
        agent.isStopped = true;
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    private void Attack()
    {
        agent.isStopped = true;
        Quaternion direction = Quaternion.RotateTowards(transform.rotation, target.rotation, 20);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackDistance, attackable))
        {
            if (hit.collider.CompareTag("Player"))
            {
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                damageable?.Damage(damage);
            }
        }
        attackPossible = false;
    }
}