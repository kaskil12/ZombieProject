using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class PlayerMovement : MonoBehaviour
{
    [Header("Camera Movement")]
    public float Sensitivity;
    public float camX;
    [Header("Rigidbody Movement")]
    public Rigidbody rb;
    public float speed;
    public bool Looks;
    public bool Sliding;
    public bool SlidAble;
    
    [Header("Jumping")]
    public GameObject JumpObject;
    public float jumprad;
    public float JumpPower;
    bool IsGrounded;
    public bool Jumpable = true;
    [Header("PlayerComponents")]
    public GameObject Unequip;
    public float Health = 100;
    public bool Walking;
    public bool Running;
    public float CapsuleHeight;
    public float WalkSpeed;
    public float RunningSpeed;
    public GameObject HipPosition;
    public bool Aiming;
    public float HandLerpSpeed;
    public static Vector3 PlayerPos;
    public GameObject AimingPos;
    //bobbing
    public float speedCurve;
    //HeadBobbing
    public Transform cameraTransform;
    public float bobbingSpeed = 1.0f; 
    public float bobbingAmount = 0.05f;
    public float midpoint = 1.0f;
    private float timer = 0.0f;

    //NotePad/Inventory List
    public GameObject NotePad;
    public TMP_Text BandageText;
    public TMP_Text ExtraAmmoText;
    public float ExtraAmmoNotepad;
    public float BandageNotePad;
    bool NotepadActive;
    public GameObject NotePadClosedPos;
    public GameObject NotePadOpenPos;
    public float OpenNoteSpeed;
    
    [Header("Weapons")]
    public GameObject HandObject;
    public int currentWeapon = 0;
    public GameObject[] weaponSlots;
    public Camera MyCamera;
    [Header("UI")]
    public GameObject PlayerUI;
    public bool SettingsActive;
    public GameObject SettingsObject;
    public Slider SensetivitySlider;
    public Slider VolumeSlider;
    public Toggle fullscreenToggle;
    [Header("Gameplay")]
    public GameObject DeadPosition;
    public string tagName = "SpawnPoint"; // Tag for your SpawnPoints
    public GameObject[] SpawnPositions;
    private bool hasAppliedForce = false;
    

    
    // Start is called before the first frame update
    void Start()
    {
        NotepadActive = false;
        Aiming = false;
        SlidAble = true;
        Jumpable = true;
        SettingsActive = false;
        Health = 100;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Looks = true;
        speed = 50;
        MyCamera = GetComponentInChildren<Camera>();
        SettingsObject.SetActive(false);
        Application.targetFrameRate = 144;
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.magnitude > 2){
            Walking = true;
        }else{
            Walking = false;
        }
      Look();
      Jumping();
      Movement();
      if(!IsGrounded)
      {
        speed = 3;
      }
      if(Input.GetKeyDown(KeyCode.E)){
      Pickup();
      }
      if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = 0;
            ActivateCurrentGun();
            weaponSlots[currentWeapon].GetComponent<GunScript>().Equip();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = 1;
            ActivateCurrentGun();
            weaponSlots[currentWeapon].GetComponent<GunScript>().Equip();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = 2;
            ActivateCurrentGun();
            weaponSlots[currentWeapon].GetComponent<GunScript>().Equip();
        }
        if(Input.GetKeyDown(KeyCode.G) && weaponSlots[currentWeapon] != null){
            DropGun();
        }
        DisableUnusedGun();
        //Settings
            if(Input.GetKeyDown(KeyCode.Escape)){
                SettingsActive = !SettingsActive;
            }
            if(SettingsActive){
                SettingsObject.SetActive(true);
                Looks = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }else{
                SettingsObject.SetActive(false);
                if(!NotepadActive){
                    Looks = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            AudioListener.volume = VolumeSlider.value;
            Sensitivity = SensetivitySlider.value;
        //Sprinting
            if(Input.GetKey(KeyCode.LeftShift) && IsGrounded){
                Running = true;
                speed = RunningSpeed;
            }else if(IsGrounded){
                speed = WalkSpeed;
                Running = false;
            }
        //Aiming
            if(Input.GetMouseButtonDown(1)){
                Aiming = !Aiming;
            }
            if (Aiming)
            {
                // Lerping towards AimingPos
                HandObject.transform.position = Vector3.Lerp(HandObject.transform.position, AimingPos.transform.position, HandLerpSpeed * Time.deltaTime);
                HandObject.transform.rotation = Quaternion.Lerp(HandObject.transform.rotation, AimingPos.transform.rotation, HandLerpSpeed * Time.deltaTime);
            }
            else
            {
                // Lerping towards HipPosition
                HandObject.transform.position = Vector3.Lerp(HandObject.transform.position, HipPosition.transform.position, HandLerpSpeed * Time.deltaTime);
                HandObject.transform.rotation = Quaternion.Lerp(HandObject.transform.rotation, HipPosition.transform.rotation, HandLerpSpeed * Time.deltaTime);
            }
        //Update Vector3 variable
            PlayerPos = gameObject.transform.position;
        //Headbobbing
        HeadBob();
        //NotePad
        if(Input.GetKeyDown(KeyCode.I)){
            NotepadActive = !NotepadActive;
        }
        if(NotepadActive){
            NotePadVoid();
            NotePad.SetActive(true);
            if(Vector3.Distance(NotePad.transform.position, NotePadOpenPos.transform.position) > 0.1f){
            NotePad.transform.position = Vector3.Lerp(NotePad.transform.position, NotePadOpenPos.transform.position, OpenNoteSpeed * Time.deltaTime);
            }
            Looks = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }else{
            if(NotePad.active){
                if(Vector3.Distance(NotePad.transform.position, NotePadClosedPos.transform.position) > 0.1f){
                NotePad.transform.position = Vector3.Lerp(NotePad.transform.position, NotePadClosedPos.transform.position, OpenNoteSpeed * Time.deltaTime);
                }else{
                    NotePad.SetActive(false);
                }
                
            }
        }

    }

    void NotePadVoid(){
        ExtraAmmoText.text = "Extra Ammo: " + ExtraAmmoNotepad.ToString();
        BandageText.text = "Bandage's: " + BandageNotePad.ToString();
         if (Input.GetMouseButtonDown(0))
        {
            Ray ray = MyCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast and check if it hits something
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null)
                {
                        if (hit.collider.gameObject.tag == "AddAmmo")
                    {
                        ExtraAmmoPlus();
                    }
                    if (hit.collider.gameObject.tag == "SubtractAmmo")
                    {
                        ExtraAmmoSubtract();
                    }
                    if (hit.collider.gameObject.tag == "AddBandage")
                    {
                        BandagePlus();
                    }
                    if (hit.collider.gameObject.tag == "SubtractBandage")
                    {
                        BandageSubtract();
                    }
                }
            }
            
                
            
        }
    }
    public void BandagePlus(){
        BandageNotePad += 1;
    }
    public void BandageSubtract(){
        BandageNotePad -= 1;
    }
    public void ExtraAmmoPlus(){
        ExtraAmmoNotepad += 1;
    }
    public void ExtraAmmoSubtract(){
        ExtraAmmoNotepad -= 1;
    }

    //HeadBob
    void HeadBob(){
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeed * Time.deltaTime;

            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            totalAxes = Mathf.Clamp(totalAxes, 0, 1);

            translateChange = totalAxes * translateChange;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, midpoint + translateChange, cameraTransform.localPosition.z);
        }
        else
        {
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, midpoint, cameraTransform.localPosition.z);
        }
    }
    
    public void DisableUnusedGun(){
        if (weaponSlots.Length > 0 && currentWeapon >= 0 && currentWeapon < weaponSlots.Length)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (i != currentWeapon)
                {
                    if(weaponSlots[i] != null){
                    weaponSlots[i].SetActive(false);
                    }
                }
            }
        }
    }

    
    public void GunSlot1(){
        currentWeapon = 0;
    }
    
    public void GunSlot2(){
        currentWeapon = 1;
    }
    
    public void GunSlot3(){
        currentWeapon = 2;
    }
    
    public void ActivateCurrentGun(){
        weaponSlots[currentWeapon].SetActive(true);
    }
    
    public void DropGun(){
        weaponSlots[currentWeapon].transform.SetParent(null);
        weaponSlots[currentWeapon].GetComponent<GunScript>().Enabled = false;
        weaponSlots[currentWeapon].transform.position = Unequip.transform.position;
        weaponSlots[currentWeapon].transform.rotation = Unequip.transform.rotation;
        weaponSlots[currentWeapon] = null;
    }
    
    
    void Pickup()
    {
        if (Physics.Raycast(MyCamera.transform.position, MyCamera.transform.forward, out RaycastHit PickupHit, 5))
        {
            if (PickupHit.transform.CompareTag("Gun"))
            {
                int nextAvailableSlot = -1;
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    if (weaponSlots[i] == null)
                    {
                        nextAvailableSlot = i;
                        break;
                    }
                }
                if (nextAvailableSlot != -1)
                {
                    weaponSlots[nextAvailableSlot] = PickupHit.transform.gameObject;
                    weaponSlots[nextAvailableSlot].transform.position = HandObject.transform.position;
                    weaponSlots[nextAvailableSlot].transform.rotation = HandObject.transform.rotation;
                    weaponSlots[nextAvailableSlot].transform.parent = HandObject.transform;
                    weaponSlots[nextAvailableSlot].GetComponent<GunScript>().Enabled = true;
                }
                else
                {
                    Debug.Log("All weapon slots are full!");
                }
            }
        }
    }
    
    public void TakeDamage(float Damage){
        Health -= Damage;
    }
    void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = (moveX * transform.right + moveZ * transform.forward).normalized;
        Vector3 move = moveDirection * speed * Time.deltaTime;
        rb.AddForce(move, ForceMode.VelocityChange);
    }

    void Look(){
        if (Looks)
        {
            GetComponentsInChildren<Camera>();
            float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
            transform.Rotate(transform.up * mouseX);

            camX -= mouseY;
            camX = Mathf.Clamp(camX, -70, 70);
            GetComponentsInChildren<Camera>()[0].transform.localRotation = Quaternion.Euler(camX, 0, 0);
        }
    }
    void Jumping(){
        IsGrounded = false;
        foreach(Collider i in Physics.OverlapSphere(JumpObject.transform.position, jumprad)){
            if(i.transform.tag != "Player"){
                IsGrounded = true;
                break;
            }
        }
        if(IsGrounded){
            if(Input.GetKeyDown(KeyCode.Space) && Jumpable == true){
                StartCoroutine(JumpDelay());
                rb.AddForce(transform.up * JumpPower, ForceMode.VelocityChange);
            }
        }
        if(!Sliding){
        rb.drag = IsGrounded ? 15 : 0.1f;
        }
    }
    IEnumerator JumpDelay(){
        Jumpable = false;
        yield return new WaitForSeconds(0.1f);
        Jumpable = true;
    }
    public void ToggleFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }
}