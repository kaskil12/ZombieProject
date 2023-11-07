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
    public AudioSource ZombieGrowl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerDetection();
        //Sound Effects
        if(!ZombieGrowl.isPlaying){
            ZombieGrowl.Play();
        }
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
