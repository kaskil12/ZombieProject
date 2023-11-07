using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour
{
    float Z_Health;
    public float DetectRange;
    public LayerMask PlayerMask;
    public NavMeshAgent ZombieAi;
    public Animator ZombieAnim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerDetection();
    }
    public void TakeDamage(float damage){
        Z_Health -= damage;
    }
    public void PlayerDetection(){
        if(Physics.CheckSphere(transform.position, DetectRange, PlayerMask)){
            ZombieAnim.SetTrigger("Walk");
            ZombieAi.SetDestination(PlayerMovement.PlayerPos);
        }
    }
}
