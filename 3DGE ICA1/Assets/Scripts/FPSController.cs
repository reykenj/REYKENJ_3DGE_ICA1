using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

public class FPSController : MonoBehaviour
{

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float speed_multiplier = 1.0f;
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float JumpHeight = 1.0f;

    [SerializeField] private float NormHeight = 1.0f;
    [SerializeField] private float OriginalFOV = 1.0f;


    [SerializeField] private float bobFrequency = 1.0f;
    [SerializeField] private float bobAmplitude = 1.0f;
    public GameObject WeaponHolder;
    [SerializeField] GameObject TheWorld;

    private float originalCameraY;
    private float timer;
    private float bobbingOffset;

    private CharacterController characterController;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction dropAction;
    private bool dropActionUp = true;
    private Vector2 mouseDelta;
    private Vector3 move;
    private Vector3 jumpVelocity;
    private float xRotation = 0f; // To track the camera's vertical rotation
                                  // Use for camera shake
    private InputAction shootAction;
    private float shakeDuration = 0.5f; // duration of the shake
    private float shakeMagnitude = 0.2f; // strength of the shake
    private float shakeTimeRemaining = 0.0f; // time remaining since shake starts
    private Vector3 originalPosition; // stores original position to return to
    private Vector3 shakeOffset; // stores the offset during shake
    private bool _sliding = false;
    private bool SingleShotCheck = false;

    private RaycastHit slopeHit;
    //[SerializeField] private float maxSlopeAngle = 45.0f;
    [SerializeField] private float SlopeMultiplier = 10.0f;
    //private Vector3 SlideForward;



    [Header("Weapon List")]
    public Weapon[] weapons;
    // For weapon switching
    private InputAction switchWeaponAction;
    private InputAction scopeAction;
    private InputAction reloadAction;
    private int currentWeaponIndex = 0;
    [HideInInspector] public Weapon currentWeapon;

    public static System.Action<int, int, int> OnAmmoCountChanged;

    [Header("Pick Up")]
    [SerializeField] private float pickUpRange;
    [SerializeField] private LayerMask itemLayer;
    // For item pick up
    private InputAction pickUpAction;

    public void InvokeAmmoCountChanged()
    {
        OnAmmoCountChanged?.Invoke(currentWeapon.ammoCount,
        currentWeapon.weaponData.maxAmmo, currentWeapon.magazineCount);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = weapons[currentWeaponIndex];
        // locks cursor to middle and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = GetComponent<CharacterController>();
        OriginalFOV = Camera.main.fieldOfView;
        NormHeight = characterController.height;
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        crouchAction = playerInput.actions["Crouch"];
        shootAction = playerInput.actions["Shoot"];
        pickUpAction = playerInput.actions["PickUp"];
        switchWeaponAction = playerInput.actions["SwitchWeapon"];
        scopeAction = playerInput.actions["Scope"];
        reloadAction = playerInput.actions["Reload"];
        dropAction = playerInput.actions["Drop"];

        // Bobbing initial parameter
        originalCameraY = Camera.main.transform.localPosition.y;
        timer = 0f;
        bobbingOffset = 0f;

        // for camera shake
        originalPosition = Camera.main.transform.localPosition;

        InvokeAmmoCountChanged();
    }

    // Update is called once per frame
    void Update()
    {
        //Camera.main.fieldOfView = 1;
        Vector2 input = moveAction.ReadValue<Vector2>();
        Jump();
        if (sprintAction.IsPressed() && !crouchAction.IsPressed())
        {
            speed_multiplier = Mathf.Lerp(speed_multiplier, 5.0f, 5 * Time.deltaTime);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, OriginalFOV * 2, 5 * Time.deltaTime);
        }
        else if (sprintAction.IsPressed() && speed_multiplier >= 4.5f && !_sliding)
        {
            _sliding = true;
            move = transform.forward;
            speed_multiplier = speed_multiplier * 2f;
        }
        else if(!_sliding)
        {
            Crouch();
            speed_multiplier = Mathf.Lerp(speed_multiplier, 1.0f, 5 * Time.deltaTime);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, OriginalFOV, 5 * Time.deltaTime);
        }


        jumpVelocity.y += gravity * Time.deltaTime;
        if (!_sliding)
        {
            //Debug.Log("should be");
            move = transform.right * input.x + transform.forward * input.y;
            Vector3 SlopeDirection = new Vector3(0, 0, 0);
            if (OnSlope())
            {
                SlopeDirection.y = Mathf.Abs(gravity);
                SlopeDirection = GetSlopeMoveDirection(SlopeDirection);
                Debug.Log(Mathf.Sin(Vector3.Angle(Vector3.up, slopeHit.normal) * Mathf.Deg2Rad));
                SlopeDirection *= Mathf.Sin(Vector3.Angle(Vector3.up, slopeHit.normal) * Mathf.Deg2Rad) * SlopeMultiplier;
                move = GetSlopeMoveDirection(move);
            }
            Vector3 finalMove = jumpVelocity + move * speed * speed_multiplier - SlopeDirection;
            characterController.Move(finalMove * Time.deltaTime);
        }
        else
        {
            Sliding();
        }


        Scope();
        if (!currentWeapon.Shooting)
        {
            Shoot();
            PickUp();


            if (!currentWeapon.Reloading)
            {
                SwitchWeapon();
                Reload();
                Drop();
            }
        }
        
    }

    private void LateUpdate()
    {
        Look();
        HandleCameraBob();
        HandleCameraShake();
        Camera.main.transform.localPosition = originalPosition + shakeOffset + new Vector3(0, bobbingOffset, 0);
    }

    void Look()
    {
        mouseDelta = lookAction.ReadValue<Vector2>();
        float mouseX = mouseDelta.x * sensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * sensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }


    void Jump()
    {
        if (jumpAction.IsPressed() && characterController.isGrounded)
        {
            jumpVelocity.y = Mathf.Sqrt(-2 * gravity * JumpHeight);
        }
        if (characterController.isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2.0f;
        }
    }
    bool Crouch()
    {
        //Debug.Log("CROUCH");
        if (crouchAction.IsPressed())
        {
            characterController.height = Mathf.Lerp(characterController.height, NormHeight / 2, 5 * Time.deltaTime);
            //characterController.center = new Vector3(characterController.center.x, (characterController.height - NormHeight), characterController.center.z);
            return true;
        }
        else
        {
            characterController.height = Mathf.Lerp(characterController.height, NormHeight, 5 * Time.deltaTime);
            return false;
        }

    }

    public void Shoot()
    {
        //Debug.Log(!SingleShotCheck || !currentWeapon.weaponData.SingleShot);
        if (shootAction.IsPressed() && !currentWeapon.Reloading)
        {
            if ((!SingleShotCheck || !currentWeapon.weaponData.SingleShot))
            {
                SingleShotCheck = true;
                shakeTimeRemaining = shakeDuration;
                shakeMagnitude = 0.05f;
                // Update the code to shoot
                currentWeapon.StartToShoot(this);
                //InvokeAmmoCountChanged();
            }
        }
        else
        {
            SingleShotCheck = false;
        }
    }
    public void Scope()
    {
        bool Scoping = scopeAction.IsPressed();
        currentWeapon.LookIntoScope(Scoping && !currentWeapon.Reloading);
    }

    private void PickUp()
    {
        //Debug.Log("Checking lol1");
        if (pickUpAction.IsPressed())
        {
            //Debug.Log("Checking lol2");
            Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(ray, out RaycastHit hit,
            pickUpRange, itemLayer))
            {
                Item item = hit.collider.GetComponent<Item>();
                //Debug.Log("Checking lol");
                if (item != null)
                {
                    item.Use(this);
                    //Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    private void Reload()
    {
        //Debug.Log("Checking lol1");
        if (reloadAction.IsPressed())
        {
            currentWeapon.StartReloading(this);
        }
    }

    private void Drop()
    {
        if (dropAction.IsPressed())
        {
            if (dropActionUp)
            {
                dropActionUp = false;
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i] != null && weapons[i] != currentWeapon)
                    {
                        currentWeapon.gameObject.layer = 7;
                        foreach (Transform child in currentWeapon.transform)
                        {
                            child.gameObject.layer = 7;
                        }
                        currentWeapon.transform.parent = TheWorld.transform;
                        Rigidbody rigidbody = currentWeapon.gameObject.GetComponent<Rigidbody>();
                        rigidbody.useGravity = true;
                        rigidbody.isKinematic = false;
                        currentWeapon.gameObject.GetComponent<IKWeaponGrab>().enabled = false;
                        currentWeapon.DropUI.SetActive(true);
                        currentWeapon.enabled = false;

                        weapons[currentWeaponIndex] = null;
                        currentWeapon = weapons[i];
                        currentWeaponIndex = i;
                        currentWeapon.gameObject.SetActive(true);
                        InvokeAmmoCountChanged();
                        break;
                    }
                }
            }
            //currentWeapon.StartReloading(this);
        }
        else
        {
            dropActionUp = true;
        }
    }

    private void SwitchWeapon()
    {

        Vector2 scroll = switchWeaponAction.ReadValue<Vector2>();
        int Move = 1;
        if (scroll.y > 0)
        {
            Move = -1;
        }
        //Debug.Log("Move: " + Move);
        if (scroll.y != 0)
        {
            for (int i = 1; i < weapons.Length + 1; i++)
            {
                //if (currentWeaponIndex == 0 )
                //int PotentialWeaponIndex = Math.Abs((currentWeaponIndex + Move * i)) % weapons.Length;
                int PotentialWeaponIndex = (currentWeaponIndex + Move * i);
                if (PotentialWeaponIndex < 0)
                {
                    PotentialWeaponIndex = weapons.Length + PotentialWeaponIndex;
                }
                PotentialWeaponIndex %= weapons.Length;
                //Debug.Log("Potential: " + PotentialWeaponIndex);
                if (weapons[PotentialWeaponIndex] != null)
                {
                    currentWeapon.gameObject.SetActive(false);
                    currentWeaponIndex = PotentialWeaponIndex;
                    currentWeapon = weapons[currentWeaponIndex];
                    currentWeapon.gameObject.SetActive(true);
                    InvokeAmmoCountChanged();
                    break;
                }
            }
        }

    }
    void Sliding()
    {
        if (crouchAction.IsPressed())
        {
            speed_multiplier = Mathf.Lerp(speed_multiplier, 1.0f, 2f * Time.deltaTime);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, OriginalFOV * 2, 2f * Time.deltaTime);

            characterController.height = Mathf.Lerp(characterController.height, NormHeight / 2, 10 * Time.deltaTime);
            Vector3 finalMove = jumpVelocity + move * speed * speed_multiplier;
            characterController.Move(finalMove * Time.deltaTime);
            bobbingOffset = Mathf.Lerp(bobbingOffset, -0.5F, 10 * Time.deltaTime);
            if (speed_multiplier <= 1.1f)
            {
                _sliding = false;
            }
        }
        else
        {
            _sliding = false;
        }
    }

    private void HandleCameraBob()
    {
        if (!characterController.isGrounded) return;
        Transform cameraTransform = Camera.main.transform;
        bool isMoving = move.magnitude > 0;
        if (isMoving && !_sliding)
        {
            // Increment the bobbing timer as player is moving
            timer += Time.deltaTime * bobFrequency * speed_multiplier;
            // Calculate the new Y position for camera bobbing using a sine wave
            bobbingOffset = Mathf.Sin(timer) * bobAmplitude;
        }
        else
        {
            bobbingOffset = Mathf.Lerp(bobbingOffset, 0, Time.deltaTime);
        }
        //// Add the originalCameraY with the bobbingOffset value as new Y position value
        //cameraTransform.localPosition = new
        //Vector3(cameraTransform.localPosition.x,
        //originalCameraY + bobbingOffset,
        //cameraTransform.localPosition.z);
    }
    private void HandleCameraShake()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeOffset = UnityEngine.Random.insideUnitSphere * shakeMagnitude;
            shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            // Ensure we don't accumulate small positional errors over time
            shakeOffset = Vector3.zero;
        }
    }

    IEnumerator PerformShake(float duration)
    {
        // Shake using position
        Vector3 startPosition = Camera.main.transform.position;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Camera.main.transform.localPosition =
            new Vector3(
            startPosition.x + UnityEngine.Random.Range(-1, 1),
            startPosition.y + UnityEngine.Random.Range(-1, 1),
            startPosition.z + UnityEngine.Random.Range(-1, 1)
            );
            yield return null;
        }
        transform.position = startPosition;

    }


    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, NormHeight * 0.5f + 0.4f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < characterController.slopeLimit && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
