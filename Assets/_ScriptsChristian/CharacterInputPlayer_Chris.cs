using System;
using UnityEngine;
using Cinemachine.Examples;
using UnityEngine.Serialization;

[AddComponentMenu("")] // Don't display in add component menu
public class CharacterInputPlayer_Chris : MonoBehaviour
{
	[Header("Input")]
    public KeyCode jumpKeyboard = KeyCode.Space;
    public KeyCode fireShooting = KeyCode.X;
    public float jumpFactor;
 	private float ySpeed;
    public float groundTolerance = 0.2f;

	public float factorGravity = 1f;
	public float moveSpeed = 1f;


	public GameObject particulasCarga;
	
		
	public GameObject bulletBase; // Prefab de la bala base
	public GameObject bullet2;    // Prefab de la bala 2
	public GameObject bullet3;    // Prefab de la bala 3

	public Transform firePoint;   // Punto de origen del disparo
	
	public float bulletSpeed_1 = 5f; 
	public float bulletSpeed_2 = 10f; 
	public float bulletSpeed_3 = 15f; 

	

  
    bool isSprinting = false;
    Animator anim;
    private Vector3 inputVec;
    float velocity;
    bool headingLeft = false;
    Quaternion targetRot;
    Quaternion previousTargetRot;
    Rigidbody rigbody;
    
    Vector3 jumpDirection = Vector3.down;
    bool GrounCollided = false;

    Vector3 velocityV = Vector3.zero;

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

	private Material VFX_brilloCarga;


	Vector3 PositionPlayer = Vector3.zero;
	
	

	private bool isJumping = false;	
	private bool isJumpingUp = false;
	
	float jumpAxisY = 0.0f;
	public float limitJumping = 5.0f;

	GameObject bullet = null;
	private float _counterFireShooting = 0.0f;


		
	private Color chargerColor_1 = new Color(0, 191, 74, 255) / 255f *3f;
	private Color chargerColor_2 = Color.red * 2f;	
	private Material colorCargaMat;

	void Start ()
	{
		
	    anim = GetComponent<Animator>();
	    rigbody = GetComponent<Rigidbody>();
	    targetRot = transform.rotation;        
	    
	    //anim.SetBool("Jumping", true);

	    VFX_brilloCarga = transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;

	    colorCargaMat =particulasCarga.GetComponent<ParticleSystemRenderer>().sharedMaterial;

	    colorCargaMat.SetColor("_Color", chargerColor_1);

	    inputVec = new Vector3(0,0,0);

	    
	}

	void RotatePlayer()
	{
		if ((inputVec.x < 0f && !headingLeft) || (inputVec.x > 0f && headingLeft))
		{
						
			if (inputVec.x < 0f)
			{
				targetRot = Quaternion.Euler(0, 270, 0);
			}

			if (inputVec.x > 0f)
			{
				targetRot = Quaternion.Euler(0, 90, 0);
			}
			headingLeft = !headingLeft;
		}
		
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 20f);
		
	}

	void MovePlayer()
	{
		float speed = Mathf.Abs(inputVec.x);
		speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
		if (_objectCollided == ObjectCollided.Wall)
			speed = 0;

		if (!isWallCollided)
			transform.position += new Vector3(inputVec.x * moveSpeed,0,0) *Time.deltaTime;
		
        anim.SetFloat("Speed", speed);
		
	}

	void Update()
	{		
		inputVec.x = Input.GetAxis("Horizontal");
		inputVec.y = Input.GetAxis("Vertical");

		MovePlayer();
		RotatePlayer();
		

		float signo = Mathf.Sign(transform.forward.x); 
		

		if (Input.GetKey(fireShooting))  //ideal seria un valor continuo para que sea suave
		{
			anim.SetLayerWeight(1, 1.0f);
			_counterFireShooting += Time.deltaTime;


			if (Input.GetKeyDown(fireShooting))
			{				
				bullet = Instantiate(bulletBase, firePoint.position, Quaternion.Euler(0, 90 * signo , 0));  
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				bullet.GetComponent<Rigidbody>().linearVelocity = Vector3.right * (firePoint.right.z * bulletSpeed_1);

			}

			if(_counterFireShooting > 0.8f)
			{
				VFX_brilloCarga.SetFloat("_Carga", 1.0f);
				particulasCarga.SetActive(true);
			}

			if(_counterFireShooting > 4)
			{				
				colorCargaMat.SetColor("_Color", chargerColor_2);
			}
				
				
		}
		else
			anim.SetLayerWeight(1, 0.0f);

		if( Input.GetKeyUp(fireShooting))
		{
			Debug.Log(_counterFireShooting);
		
			if (_counterFireShooting >= 2f && _counterFireShooting < 4)
			{				

				bullet = Instantiate(bullet2, firePoint.position, Quaternion.Euler(0, 90 *signo, 0));
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				
				bullet.GetComponent<Rigidbody>().linearVelocity = Vector3.right * (firePoint.right.z * bulletSpeed_2);
				particulasCarga.SetActive(false);
			}		
			else if ( _counterFireShooting >= 4 )
			{
						
				bullet = Instantiate(bullet3, firePoint.position, Quaternion.Euler(0, 90 * signo, 0));
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				
				bullet.GetComponent<Rigidbody>().linearVelocity = Vector3.right * (firePoint.right.z * bulletSpeed_3); //buscar forma sin rigidbody para balas porsiaca
				particulasCarga.SetActive(false);				
			}

			_counterFireShooting = 0.0f;
			VFX_brilloCarga.SetFloat("_Carga", 0.0f);
			 colorCargaMat.SetColor("_Color", chargerColor_1);
			particulasCarga.SetActive(false);
		}



		//************************* MOVIMIENTO //////////////////////////// 

		Vector3 posSuelo = Vector3.zero;
		//bool saltando = false;
		if(isGrounded)
		{
			posSuelo = transform.localPosition;
			anim.SetBool("Air", false);
		}
		else // caso suelo se caiga? o camines al borde y caigas
		{			
			anim.SetBool("Air", true);
		}

		if( inputVec.y > 0) // puedo saltar mientras caigo, si pongo isGrounded ya no pero el salto lo hace a medias >:( ?? o no?
		{
			anim.SetTrigger("Jump");
			anim.SetBool("Air", true);
			anim.SetBool("IdleWalk", false);				
			if(transform.localPosition.y <  (posSuelo  + new Vector3(0,1.5f,0)).y  )	//convertir altura en variable luego, funciona bien pero en cima vibra un poco	
				transform.localPosition = Vector3.SmoothDamp(transform.localPosition, transform.localPosition + new Vector3(0,1.5f,0) , ref velocityV, jumpFactor* Time.deltaTime );
		}
		else
		{
			anim.SetBool("IdleWalk", true);

			/*
			speed = Mathf.Abs(inputPlayer.x);
			speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
				
        	anim.SetFloat("Speed", speed);	*/
		}


		

	}
	
	void FixedUpdate ()
	{

		GroundedCheck();

		


/*
		if( Input.GetKeyUp(jumpKeyboard))
		{
			 GetComponent<Rigidbody>().useGravity = true;
		}*/




		/*
	    if (Input.GetKeyUp(jumpKeyboard))
	    {
	    	Debug.Log("saltando");
		    GetComponent<Rigidbody>().useGravity = true;
		    if (!isGrounded)
		    {
			    anim.SetBool("Air", true);
		    }
		    else
		    {
			    anim.SetBool("Air", false);
		    }
	    }
	

	    if (isGrounded && !isJumpingUp && Input.GetKeyDown(jumpKeyboard))
	    {
		    isJumpingUp = true;
		    jumpAxisY = transform.localPosition.y;
		    GetComponent<Rigidbody>().useGravity = false;
		    anim.SetBool("IdleWalk", false); 
		    anim.SetTrigger("Jump");
	    }
	    if ( !isGrounded && isJumpingUp && Input.GetKey(jumpKeyboard))
	    {
		    if (transform.localPosition.y <= jumpAxisY + limitJumping)
		    {
		    	Debug.Log("ojoo");
			    //transform.localPosition += Vector3.up * Time.deltaTime * -Physics.gravity.y * jumpVelocity;
			    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, transform.localPosition + new Vector3(0,1.5f,0) , ref velocityV, jumpVelocity* Time.deltaTime );
		    }
		    else
		    {
			    GetComponent<Rigidbody>().useGravity = true;
			    anim.SetBool("Air", true);
		    }

	    }*/
		

	}

	private bool isGrounded = false;

    public void GroundedCheck()
    {
    	 isGrounded = Physics.Raycast(
		    transform.position + new Vector3(0,0.05f,0),
		    jumpDirection,
		    groundTolerance
	    );
    	  print("isGrounded: " + isGrounded);


    	 // isJumpingUp= !isGrounded;


    }

/*
    public void GroundedCheck()
    {
	    bool wasGrounded = isGrounded;
	    isGrounded = Physics.Raycast(
		    transform.position + new Vector3(0,0.05f,0),
		    jumpDirection,
		    groundTolerance
	    );

	    //Debug.DrawRay(transform.position +new Vector3(0,0.05f,0), jumpDirection * groundTolerance, Color.yellow); 

	    print("isGrounded: " + isGrounded);
	    if (isGrounded)
	    {
		    if (!wasGrounded)
		    {
			    isJumpingUp = false;
		    }
		    else
		    {
			    anim.SetBool("Air", false);
			    anim.SetBool("IdleWalk", true);    
		    }
	    }
    }
*/

	



/*
	private void RotatePlayer(Vector2 inputPlayer)
	{
		// Check if direction changes
		if ((inputPlayer.x < 0f && !headingLeft) || (inputPlayer.x > 0f && headingLeft))
		{
			previousTargetRot = targetRot;
			//print("Previous Rotation: " + previousTargetRot);
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
		//print("Rotation: " + transform.rotation);
	}

	private void MovePlayer(Vector2 inputPlayer)
	{
		speed = Mathf.Abs(inputPlayer.x);
		speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
		if (_objectCollided == ObjectCollided.Wall)
			speed = 0;

		if (!isWallCollided)
		{
			//rigbody.linearVelocity = new Vector3(inputPlayer.x * factorMovement, rigbody.linearVelocity.y, 0); 
			//rigbody.AddForce(transform.forward * factorMovement);
			transform.position += new Vector3(inputPlayer.x * factorMovement,0,0) *Time.deltaTime;
		}
        anim.SetFloat("Speed", speed);
    }
	   */


    private void OnCollisionEnter(Collision other)
    {
	    if (other.gameObject.CompareTag("Ground"))
	    {
		    // _objectCollided = ObjectCollided.Ground;
		    isGroundCollided = true;
			// anim.SetBool("Air", false);
			// anim.SetTrigger("Ground");
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

