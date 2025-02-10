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
		Ground,
		Wall
	}

	private ObjectCollided _objectCollided;

	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	    rigbody = GetComponent<Rigidbody>();
	    targetRot = transform.rotation;        
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
		// set speed to horizontal inputs
		speed = Mathf.Abs(inputPlayer.x);
		// if (!WallCollided)
		// {
		// speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
		print(previousTargetRot + " :Previous Current: " + targetRot);
		var tempSpeed = speed;
		print("speed: " + speed);
		if (_objectCollided == ObjectCollided.Wall)
		{
			speed = 0;
			print("speedTargetWall: " + speed);
		}
		else
		{
			print("_T_ _objectCollided: " + (_objectCollided == ObjectCollided.Wall));
			print("_T_ previousTargetRot: " + (targetRot == previousTargetRot));
		}

		if (targetRot.eulerAngles.z != previousTargetRot.eulerAngles.z)
		{
			speed = tempSpeed;
		}
		
		anim.SetFloat("Speed", speed);
		// }
	}
	
    private void Update()
    {
        // Jump
	    if (isGrounded() && (Input.GetKeyDown(jumpJoystick) || Input.GetKeyDown(jumpKeyboard)))
	    {
		    rigbody.AddForce(new Vector3(0, jumpVelocity, 0), ForceMode.Impulse);
	    }
	}


    public bool isGrounded()
    {
	    if (checkGroundForJump)
	    {
		    return Physics.Raycast(
			    transform.position,
			    jumpDirection,
			    groundTolerance
		    );
	    }
        else
            return true;
    }


    private void OnCollisionEnter(Collision other)
    {
	    if (other.gameObject.CompareTag("Ground"))
	    {
		    _objectCollided = ObjectCollided.Ground;
		    // jumpDirection = Vector3.down;
		    print("coll Ground");
	    }
	    else if (other.gameObject.CompareTag("Wall"))
	    {
		    _objectCollided = ObjectCollided.Wall;
		    // jumpDirection = Vector3.right;
		    print("coll Wall");
	    }
    }
}

