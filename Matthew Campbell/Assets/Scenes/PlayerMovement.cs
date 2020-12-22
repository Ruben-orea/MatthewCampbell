using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

	public CharacterController controller;
	public Transform groundCheck, crouchCheck, interactuarCheck;
    public Camera cameraMain;
    //public Animator animatorPistol, animatorCamera;
    //public Text interactuarText;
    

    //private GameObject saveMachine, interactuarTextObj;
    private AudioSource aS;

    public float speed = 10f, fov, x, z;
	public float gravity = -21f;
	public float jumpHeight = 2.5f;

    private float xScale = 1, yScale = 1, zScale = 1;

    public float groundDistance = 0.4f, staminaRunning, crouchDistance = 0.4f, interactuarDistance;
    public LayerMask groundMask, crouchMask, saveMask;

	Vector3 velocity;
    Vector3 heightCamera = new Vector3(1, 1, 1);

    bool isCrouch, isTired;

	void Start ()
    {
        //saveMachine = GameObject.FindGameObjectWithTag("SaveMachine");
        //aS = saveMachine.GetComponent<AudioSource>();
        //interactuarTextObj = interactuarText.gameObject;
	}

	void Update ()
	{
        cameraMain.fieldOfView = fov;
        transform.localScale = heightCamera;

        CheckGround();
        Movement();
        InteractuarSaveMachine();

        if(isTired)
        {
            staminaRunning += Time.deltaTime;

            if (staminaRunning >= 0)
            {
                staminaRunning = 0;
                isTired = false;
            }

        }

		if(CheckGround() && velocity.y < 0)
		{
			velocity.y = -2f;
		}

		x = Input.GetAxis("Horizontal");
		z = Input.GetAxis("Vertical");

		Vector3 move = transform.right * x + transform.forward * z;
		controller.Move(move * speed * Time.deltaTime);
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);

        if(x != 0 || z != 0)
        {
            //animatorCamera.SetBool("isMoving", true);
            //animatorCamera.speed = speed / 3.5f;
        }

        else
        {
            //animatorCamera.SetBool("isMoving", false);
            //animatorCamera.speed = 1;
        }

	}

    void InteractuarSaveMachine()
    {
        if(SaveMachine())
        {
            //interactuarText.enabled = true;

            if(Input.GetKeyDown(KeyCode.E))
            {
                aS.Play();
                //interactuarTextObj.SetActive(false);
            }
        }
        else
        {
           //interactuarText.enabled = false;
            //interactuarTextObj.SetActive(true);
        }
    }

    bool SaveMachine()
    {
        bool isTouchingSaveMachine = Physics.CheckBox(interactuarCheck.position, new Vector3(0.4f, 0.45f, 0.8f), Quaternion.identity, saveMask);
        return isTouchingSaveMachine;
    }

    bool CheckGround()
    {
       bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
       return isGrounded;
    }
    

	bool CheckCrouch()
	{
		bool isCrouched = Physics.CheckSphere(crouchCheck.position, crouchDistance, crouchMask);
		return isCrouched;
	}

    void Movement()
    {
        //Crouch.
        if(Input.GetKey(KeyCode.LeftControl) && !isCrouch)
        {
            xScale = Mathf.Lerp(xScale, 0.5f, 4 * Time.deltaTime);
            yScale = Mathf.Lerp(yScale, 0.5f, 4 * Time.deltaTime);
            zScale = Mathf.Lerp(zScale, 0.5f, 4 * Time.deltaTime);

            heightCamera = new Vector3(xScale, yScale, zScale);

            speed = 5;

            isCrouch = true;
        }


		else if(CheckCrouch())
		{
			heightCamera = new Vector3(xScale, yScale, zScale);

			speed = 5;

			isCrouch = true;
		}

        else
        {
            xScale = Mathf.Lerp(xScale, 1, 4 * Time.deltaTime);
            yScale = Mathf.Lerp(yScale, 1, 4 * Time.deltaTime);
            zScale = Mathf.Lerp(zScale, 1, 4 * Time.deltaTime);


            heightCamera = new Vector3(xScale, yScale, zScale);
        }

        //Jumping.
        if (Input.GetButtonDown("Jump") && CheckGround() && !isCrouch)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);          
        }

        //Run Increase FOV, Speed, Stamina.
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouch && staminaRunning >= 0 && staminaRunning <= 5 && CheckGround() && z > 0)
        {
            speed = Mathf.Lerp(speed, 16, 3 * Time.deltaTime);
            fov = Mathf.Lerp(fov, 75, 3 * Time.deltaTime);
            staminaRunning += Time.deltaTime;
            //animatorPistol.SetBool("isRunning", true);
        }

        else if(staminaRunning >= 5)
        {
            speed = Mathf.Lerp(speed, 10, 3 * Time.deltaTime);
            fov = Mathf.Lerp(fov, 60, 3 * Time.deltaTime);
            isCrouch = false;
            //animatorPistol.SetBool("isRunning", false);

            isTired = true;
            staminaRunning = -3f;
        }

        else
        {
            speed = Mathf.Lerp(speed, 10, 3 * Time.deltaTime);
            fov = Mathf.Lerp(fov, 60, 3 * Time.deltaTime);
            isCrouch = false;
            //animatorPistol.SetBool("isRunning", false);

            if (staminaRunning > 0)
            {
                staminaRunning = Mathf.Lerp(staminaRunning, 0, 0.5f * Time.deltaTime);
            }
        }
    }
}
