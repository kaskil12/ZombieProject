using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Misc")]
    public bool Enabled;
    public GameObject[] Arms;
    public Animator GunAnim;
    private Camera Guncam;
    bool Shoot;
    bool Shooting;
    bool Reloading;
    public PlayerMovement playerMovement;
    bool Executed = false;
    [Header("Audio")]
    public AudioSource ShootAudio;
    [Header("GunSpecs")]
    public float RecoilSpeed;
    public float RecoilAmount;
    public float AmmoAmount;
    public float FullClip;
    public float ShootDelayAmount;
    public float ReloadDelayAmount;
    public float SwayMultiplier;
    public float SwaySmooth;
    public Vector3 GunOffsetScript;
    // Start is called before the first frame update
    void Start()
    {
        Shooting = false;
        Shoot = true;
        Reloading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Enabled){
            //Enable Parts If Gun Is Enabled
                foreach (GameObject Arms in Arms)
                {
                    Arms.SetActive(true); 
                }
            //GetParts
                playerMovement = GetComponentsInParent<PlayerMovement>()[0];
                Camera Guncam = GetComponentsInParent<Camera>()[0];
            //Set offset to gun from hand
                transform.localPosition = GunOffsetScript;
            //GunSway
                float mouseX = Input.GetAxisRaw("Mouse X") * SwayMultiplier;
                float mouseY = Input.GetAxisRaw("Mouse Y") * SwayMultiplier;
                Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
                Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
                Quaternion targetRotation = rotationX * rotationY;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, SwaySmooth * Time.deltaTime);
            //Shooting
                if(Input.GetMouseButtonDown(0) && !Reloading && Shoot && !Shooting){
                    CallGun();
                }
            //Reloading
                if(Input.GetKeyDown(KeyCode.R) && !Reloading && !Shooting){
                    StartCoroutine(Reload());
                }
            //Recoil
            // if(Shooting && !Executed){
            //     playerMovement.camX = Mathf.Lerp(playerMovement.camX - RecoilAmount, playerMovement.camX, Time.deltaTime * RecoilSpeed);
            //     Executed = true;
            // }
        }else{
            foreach (GameObject Arms in Arms)
            {
                Arms.SetActive(false); 
            }
        }
    }
    public void CallGun(){
        Shooting = true;
        Shoot = false;
        StartCoroutine(ShootDelay());
        GunAnim.SetTrigger("Shoot");
        ShootAudio.Play();
        // Camera Guncam = GetComponentsInParent<Camera>()[0];
        // if(Physics.Raycast(Guncam.transform.position, Guncam.transform.forward, out RaycastHit ShootHit, Mathf.Infinity)){
            
        // }
    }
    IEnumerator ShootDelay(){
        yield return new WaitForSeconds(ShootDelayAmount);
        Executed = false;
        Shooting = false;
        Shoot = true;
    }
    IEnumerator Reload(){
        GunAnim.SetTrigger("Reload");
        Reloading = true;
        yield return new WaitForSeconds(ReloadDelayAmount);
        AmmoAmount = FullClip;
        Reloading = false;
    }
    public void Equip(){

    }
}
