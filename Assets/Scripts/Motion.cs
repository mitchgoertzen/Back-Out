using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Motion : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;

    [SerializeField] private Text fpsText;

    [SerializeField] private Transform weaponParent;

    [SerializeField] private Weapon equippedWeapon;

    private bool isSprinting = false;
    private bool isJumping = false;
    private bool isAiming = false;

    private CharacterController controller;

    private float baseFOV;
    private float sprintFOVModifier = 1.05f;
    private float movementCounter;
    private float idleCounter;
    private float sprintBoost = 2f;
    private float directionY;
    private float deltaTime;

    private RaycastHit[] rayResults;

    private Vector3 weaponParentOrigin;
    private Vector3 targetWeaponPosition;

    // Start is called before the first frame update
    void Start()
    {
        transform.localRotation = Quaternion.identity;
        cam.transform.localRotation = Quaternion.identity;
        controller = GetComponent<CharacterController>();
        baseFOV = cam.fieldOfView;
        weaponParentOrigin = weaponParent.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Camera.main.GetComponent<Look>().isPaused())
        {
            float currSpeed = speed;
            float hmove = Input.GetAxisRaw("Horizontal");
            float vmove = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(hmove * .5f, 0, vmove);
            direction = Camera.main.transform.TransformDirection(direction);
            direction.y = 0.0f;
            direction.Normalize();

            if (!isAiming && isSprinting && vmove > 0)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 8f);
                currSpeed = speed * sprintBoost;
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV, Time.deltaTime * 16f);
            }

            if (controller.isGrounded)
            {
                controller.stepOffset = 0.3f;
                controller.slopeLimit = 45;
                if (hmove == 0 && vmove == 0)
                {
                    HeadBob(idleCounter, .005f, .005f);
                    idleCounter += Time.deltaTime;
                    if (idleCounter == 2 * Mathf.PI)
                        idleCounter = 0;
                    weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponPosition, Time.deltaTime * 2f);

                }
                else
                {
                    HeadBob(movementCounter, currSpeed * 0.0008f, currSpeed * 0.0009f);
                    movementCounter += Time.deltaTime * currSpeed * .4f;
                    if (movementCounter == 2 * Mathf.PI)
                        movementCounter = 0;
                    weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponPosition, Time.deltaTime * 10f);

                }
            }

            directionY -= gravity * Time.deltaTime;
            direction.y = directionY;
            controller.Move(direction * currSpeed * Time.deltaTime);
        }

        
    }

    void Update()
    {
        if (!Camera.main.GetComponent<Look>().isPaused())
        {
            isAiming = equippedWeapon.GetIsAiming();
            KeyPress_Space();
            KeyPress_Shift();

            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = Mathf.Ceil(fps).ToString();
        }
        else
        {
            fpsText.text = "";
        }
    }

    void HeadBob(float z, float xIntensity, float yIntensity)
    {
        targetWeaponPosition = weaponParentOrigin + new Vector3(Mathf.Cos(z) * xIntensity, Mathf.Sin(z * 2) * yIntensity, 0);
    }

    void KeyPress_Space()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            if(isAiming)
                directionY = jumpForce *.9f;
            else
                directionY = jumpForce;

            controller.slopeLimit = 90;
            controller.stepOffset = 0f;

        }
    }

    void KeyPress_Shift()
    {
        if (!isAiming && controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                isSprinting = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isSprinting = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isSprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        }
    }

    public float GetSpeed()
    {
        return speed;
    }

}

