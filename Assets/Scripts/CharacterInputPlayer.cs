using System;
using UnityEngine;
using Cinemachine.Examples;

[AddComponentMenu("")] // Don't display in add component menu
public class CharacterInputPlayer : MonoBehaviour
{
	[Header("Input")]
    public KeyCode sprintJoystick = KeyCode.JoystickButton2;
    public KeyCode jumpJoystick = KeyCode.JoystickButton0;
    public KeyCode sprintKeyboard = KeyCode.LeftShift;
    public KeyCode jumpKeyboard = KeyCode.Space;
    public float jumpVelocity = 7f;
    public float groundTolerance = 0.2f;
    public bool checkGroundForJump = true;
	public float factorGravity = 1f;
	public float factorMovement = 1f;

    float speed = 0f;
    bool isSprinting = false;
    Animator anim;
    Vector2 input;
    float velocity;
    bool headingLeft = false;
    Quaternion targetRot;
    Quaternion previousTargetRot;
    Rigidbody rigbody;
    
    Vector3 jumpDirection = Vector3.down;
    bool GrounCollided = false;

    private enum ObjectCollided
	{
		Shooter,
		Ground,
		Wall,
		None
	}

	private bool isGroundCollided = false;
	private bool isWallCollided = false;

	private ObjectCollided _objectCollided;

	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	    rigbody = GetComponent<Rigidbody>();
	    targetRot = transform.rotation;        
	    anim.SetBool("Jumping", true);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		input.x = Input.GetAxis("Horizontal");

		RotatePlayer(input);
		MovePlayer(input);



		//    // set sprinting
		// if ((Input.GetKeyDown(sprintJoystick) || Input.GetKeyDown(sprintKeyboard))&& input != Vector2.zero) isSprinting = true;
		// if ((Input.GetKeyUp(sprintJoystick) || Input.GetKeyUp(sprintKeyboard))|| input == Vector2.zero) isSprinting = false;
		//    anim.SetBool("isSprinting", isSprinting);
	}

	private void RotatePlayer(Vector2 inputPlayer)
	{
		// Check if direction changes
		if ((inputPlayer.x < 0f && !headingLeft) || (inputPlayer.x > 0f && headingLeft))
		{
			previousTargetRot = targetRot;
			print("Previous Rotation: " + previousTargetRot);
			if (inputPlayer.x < 0f)
			{
				targetRot = Quaternion.Euler(0, 270, 0);
			}

			if (inputPlayer.x > 0f)
			{
				targetRot = Quaternion.Euler(0, 90, 0);
			}
			headingLeft = !headingLeft;
		}
		// Rotate player if direction changes
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 20f);
		print("Rotation: " + transform.rotation);
	}

	private void MovePlayer(Vector2 inputPlayer)
	{
		speed = Mathf.Abs(inputPlayer.x);
		speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
		if (_objectCollided == ObjectCollided.Wall)
			speed = 0;

		if (!isWallCollided)
		{
			rigbody.linearVelocity = new Vector3(inputPlayer.x * factorMovement, rigbody.linearVelocity.y, 0);
		}
        anim.SetFloat("Speed", speed);
    }
	Vector3 PositionPlayer = Vector3.zero;
	[SerializeField] private float jumpForce = 10f; // Fuerza máxima del salto
	[SerializeField] private float jumpTime = 0.3f; // Tiempo máximo de salto
	private float jumpTimer;
	private bool isJumping = false;
    private void Update()
    {
	    GroundedCheck();
	    if (isGrounded && (Input.GetKeyDown(jumpJoystick) || Input.GetKeyDown(jumpKeyboard)))
	    {
		    rigbody.AddForce(new Vector3(0, jumpVelocity, 0), ForceMode.Acceleration);
		    anim.SetTrigger("Jump");
		    // anim.SetBool("Jumping", true);
	    }
	    if (!isGrounded)
	    {
		    // anim.SetBool("Jumping", false);
		    anim.SetBool("Air", true);
		    rigbody.AddForce(Vector3.down * (factorGravity * Physics.gravity.y), ForceMode.Acceleration);
	    }
    }


    private bool isGrounded = false;
    public void GroundedCheck()
    {
	    isGrounded = Physics.Raycast(
		    transform.position,
		    jumpDirection,
		    groundTolerance
	    );
	    print("isGrounded: " + isGrounded);
	    // if (isGrounded)
	    // {
		   //  anim.SetTrigger("Ground");
	    // }
    }


    private void OnCollisionEnter(Collision other)
    {
	    if (other.gameObject.CompareTag("Ground"))
	    {
		    // _objectCollided = ObjectCollided.Ground;
		    isGroundCollided = true;
			anim.SetBool("Air", false);
			anim.SetTrigger("Ground");
			// anim.SetBool("Jumping", false);
            // jumpDirection = Vector3.down;
            print("coll Ground");
	    }
	    else if (other.gameObject.CompareTag("Wall"))
	    {
		    isWallCollided = true;
		    // _objectCollided = ObjectCollided.Wall;
		    // jumpDirection = Vector3.right;
		    print("coll Wall");
	    }
		else
		{
            _objectCollided = ObjectCollided.None;
            print("None wallground");
        }
    }

    private void OnCollisionExit(Collision other)
    {
	    if (other.gameObject.CompareTag("Ground"))
	    {
		    // _objectCollided = ObjectCollided.Ground;
		    isGroundCollided = false;
		    // jumpDirection = Vector3.down;
		    print("coll Ground");
	    }
	    else if (other.gameObject.CompareTag("Wall"))
	    {
		    isWallCollided = false;
		    // _objectCollided = ObjectCollided.Wall;
		    // jumpDirection = Vector3.right;
		    print("coll Wall");
	    }
    }
}

