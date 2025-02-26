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
    public float jumpVelocity = 7f;
    public float groundTolerance = 0.2f;
    public bool checkGroundForJump = true;
	public float factorGravity = 1f;
	public float factorMovement = 1f;

	public Material shBrightMesh;

	public GameObject goAuraCharger;
	// public GameObject chargerParticles;
	// public GameObject projectileBasic;
	// public GameObject projectileMedium;
	// public GameObject projectileHard;
	// public Transform firePoint; // Punto de salida de las balas
	// public float bulletSpeed = 10f; // Velocidad de la bala
	
	
	public GameObject bulletBase; // Prefab de la bala base
	public GameObject bullet2;    // Prefab de la bala 2
	public GameObject bullet3;    // Prefab de la bala 3

	public Transform firePoint;   // Punto de origen del disparo
	public float fireRate = 0.5f; // Tiempo entre disparos
	public float bulletSpeed_1 = 5f; 
	public float bulletSpeed_2 = 10f; 
	public float bulletSpeed_3 = 15f; 

	private float nextFireTime = 0f;
	private int currentBulletIndex = 0; // Índice de la bala actual
	private Rigidbody bulletRigidbody;

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

	private Material VFX_mesh; 
	
	// Use this for initialization
	void Start ()
	{
		_particleChargerColor = goAuraCharger.GetComponent<ParticleSystemRenderer>();
	    anim = GetComponent<Animator>();
	    rigbody = GetComponent<Rigidbody>();
	    targetRot = transform.rotation;        
	    
	    //anim.SetBool("Jumping", true);

	    VFX_mesh = transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;
	    // VFX_mesh.SetFloat("_Brillar", 1.0f);
	}
	GameObject bullet = null;
	private float _counterFireShooting = 0.0f;

	private bool _isShootingOn = false; 
	private bool _isShootingOff = false; 
		
	private Color chargerColor_1 = new Color(0, 191, 74, 255) / 255f *3f;
	private Color chargerColor_2 = Color.yellow * 1.2f;
	ParticleSystemRenderer _particleChargerColor = new();
	// Update is called once per frame
	void FixedUpdate ()
	{


		// goAuraCharger.GetComponent<ParticleSystemRenderer>().sharedMaterial.SetColor("_Color", chargerColor_2);
		// goAuraCharger.GetComponent<ParticleSystemRenderer>().sharedMaterial.SetColor("_EmissionColor", Color.red);

		//print("firePoint.right: "+firePoint.right.z);
		input.x = Input.GetAxis("Horizontal");

		RotatePlayer(input);
		MovePlayer(input);

		float signo = Mathf.Sign(transform.forward.x); 

		

		if (Input.GetKey(fireShooting))
			anim.SetLayerWeight(1, 1.0f);
		else
			anim.SetLayerWeight(1, 0.0f); 

		if (!_isShootingOn && Input.GetKeyDown(fireShooting))
		{

			_isShootingOn = true;
			_isShootingOff = false;
			_counterFireShooting = 0.0f;
			 VFX_mesh.SetFloat("_Carga", 1.0f);
			
			bulletBase.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(false);
		}
		if (!_isShootingOff && Input.GetKeyUp(fireShooting))
		{
			_isShootingOn = false;
			_isShootingOff = true;
			 VFX_mesh.SetFloat("_Carga", 0.0f);
			
			shBrightMesh.SetFloat("_Carga", 0.0f);

			
			
			if (_counterFireShooting < 60f)
			{
				// bulletBase.gameObject.SetActive(true);
				bullet = Instantiate(bulletBase, firePoint.position, Quaternion.Euler(0, 90 * signo , 0));  //
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				bullet.GetComponent<Rigidbody>().linearVelocity = Vector3.right * (firePoint.right.z * bulletSpeed_1);
				
			}
			else if (_counterFireShooting >= 60f && _counterFireShooting < 120f)
			{
				// bullet2.gameObject.SetActive(true);
				bullet = Instantiate(bullet2, firePoint.position, Quaternion.Euler(0, 90 *signo, 0));
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				
				bullet.GetComponent<Rigidbody>().linearVelocity = Vector3.right * (firePoint.right.z * bulletSpeed_2);

				

				goAuraCharger.SetActive(false);
			}
			else if (_counterFireShooting >= 120f)
			{
				// bullet3.gameObject.SetActive(true);
				bullet = Instantiate(bullet3, firePoint.position, Quaternion.Euler(0, 90 * signo, 0));
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				
				bullet.GetComponent<Rigidbody>().linearVelocity = Vector3.right * (firePoint.right.z * bulletSpeed_3);
				

				goAuraCharger.SetActive(false);
			}
		}

		if (_isShootingOn && !_isShootingOff)
		{
			_counterFireShooting += 1.0f;
			if (_counterFireShooting == 60f)
			{
				shBrightMesh.SetFloat("_Carga", 1.0f);
				goAuraCharger.SetActive(true);
				_particleChargerColor.material.SetColor("_Color", chargerColor_1);
			}
			else if (_counterFireShooting == 120f)
			{
				goAuraCharger.SetActive(true);
				_particleChargerColor.material.SetColor("_Color", chargerColor_2);
			}
		}
		//print("counterFireShooting: "+_counterFireShooting);


		GroundedCheck();


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
	    if (isJumpingUp && Input.GetKey(jumpKeyboard))
	    {
		    if (transform.localPosition.y <= jumpAxisY + limitJumping)
		    {
		    	Debug.Log("ojoo");
			    transform.localPosition += Vector3.up * Time.deltaTime * -Physics.gravity.y * jumpVelocity;
		    }
		    else
		    {
			    GetComponent<Rigidbody>().useGravity = true;
			    anim.SetBool("Air", true);
		    }

	    }
		

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
			//rigbody.linearVelocity = new Vector3(inputPlayer.x * factorMovement, rigbody.linearVelocity.y, 0); 
			//rigbody.AddForce(transform.forward * factorMovement);
			transform.position += new Vector3(inputPlayer.x * factorMovement,0,0) *0.01f; // *timedeltatime?
		}
        anim.SetFloat("Speed", speed);
    }
	Vector3 PositionPlayer = Vector3.zero;
	[SerializeField] private float jumpForce = 10f; // Fuerza máxima del salto
	[SerializeField] private float jumpTime = 0.3f; // Tiempo máximo de salto
	private float jumpTimer;
	private bool isJumping = false;
	
	private bool isJumpingUp = false;
	private bool isFallingDown = false;
	float jumpAxisY = 0.0f;
	public float limitJumping = 5.0f;
   

    private void Update()
    {		    

		
	   

    }


    private bool isGrounded = false;
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

