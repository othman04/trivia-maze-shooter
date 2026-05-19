using UnityEngine;
using UnityEngine.AI;

public class FollowAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    GameObject ObejctToFollow;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ObejctToFollow = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, ObejctToFollow.transform.position);
        Vector3 dir = ObejctToFollow.transform.position - transform.position;
        dir.y = 0;
        float angle = Vector3.Angle(transform.forward, dir);

        if (distance < 5)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            if (angle > 5f)
            {
                animator.SetInteger("C", 0);
                animator.SetInteger("F", 1);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
            }
            else
            {
                animator.SetInteger("F", 0);
                animator.SetInteger("C", 0);
            }
        }
        else if (distance >= 5 && distance < 13)
        {
            agent.isStopped = false;
            agent.speed = 3;
            agent.SetDestination(ObejctToFollow.transform.position);
            animator.SetInteger("F", 0);
            animator.SetInteger("C", 1);
        }
        else if (distance >= 13)
        {
            agent.isStopped = false;
            agent.speed = 6;
            agent.SetDestination(ObejctToFollow.transform.position);
            animator.SetInteger("F", 0);
            animator.SetInteger("C", 2);
        }
    }
}