using System;
using UnityEngine;
using Cinemachine.Examples;
using UnityEngine.Serialization;
using UnityEngine.Animations.Rigging;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")] // Don't display in add component menu
public class CharacterInputPlayer_Chris : MonoBehaviour
{
	[Header("Input")]
    public KeyCode jumpKeyboard = KeyCode.Space;
    public KeyCode fireShooting = KeyCode.X;
    public float jumpSpeed; 
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

	
  
    private Animator anim;
    private Vector3 inputVec;
    private float velocity;
    private bool headingLeft = false;
    private Quaternion targetRot;
    private Quaternion previousTargetRot;
    
    
    private Vector3 jumpDirection = Vector3.down;
    private bool deslizar = false;
   

    private Vector3 velocityV = Vector3.zero;


	private bool isWallCollided = false;
	private bool isGrounded = true;

	private Material VFX_brilloCarga;


	private Vector3 PositionPlayer = Vector3.zero;
	
	public float jumpHeight = 1.5f;  // mejor dejarlo en 1 y jmSpeed en 5 da mejores resultados...

	private GameObject bullet = null;
	private float _counterFireShooting = 0.0f;


		
	private Color chargerColor_1 = new Color(0, 191, 74, 255) / 255f *3f;
	private Color chargerColor_2 = Color.red * 2f;	
	private Material colorCargaMat;

	private CharacterController characterController;

	private float ySpeed;
	private Impacto impactoScript;
	private ChainIKConstraint IK_R;
	private ChainIKConstraint IK_L;

	private AudioSource SonidosPlayer;
	public List<AudioClip> sonidos; 

	void Start ()
	{
		
	    anim = GetComponent<Animator>();	    
	    targetRot = transform.rotation;        
	    
	   

	    VFX_brilloCarga = transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;

	    colorCargaMat =particulasCarga.GetComponent<ParticleSystemRenderer>().sharedMaterial;

	    colorCargaMat.SetColor("_Color", chargerColor_1);

	    inputVec = new Vector3(0,0,0);

	    characterController = GetComponent<CharacterController>();
	    impactoScript = GetComponent<Impacto>();

	    IK_R = transform.GetChild(5).GetChild(0).GetComponent<ChainIKConstraint>(); // cuidado con la posicion, no creo que sea buena practica pero ¯\_(ツ)_/¯ 
	    IK_L = transform.GetChild(5).GetChild(1).GetComponent<ChainIKConstraint>();

	    IK_R.weight =0; 
		IK_L.weight =0;

		SonidosPlayer = GetComponent<AudioSource>();

	    
	}

	public bool getOrientation()
	{
		return headingLeft;
	}

	void RotatePlayer()
	{
		
		if(!anim.GetBool("Wall") && !anim.GetBool("Air")) // no rotar si estoy deslizandome
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
				headingLeft = transform.forward.x < 0; //confirmar caso se desliza
			}
			
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 20f);
		}		
	
		
	}

	void MovePlayer(Vector3 vector)
	{
		if(!impactoScript.getVivo()) 
		{
			float speed = Mathf.Abs(inputVec.x);
			speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
			if (isWallCollided) //Funciona?
			{			
				//speed = Mathf.SmoothDamp(anim.GetFloat("Speed"),0, ref velocity, 0.1f);
				speed = Mathf.Lerp(anim.GetFloat("Speed"),0,Mathf.Abs(inputVec.x)); // mejorar luego?
			}

			//if (!isWallCollided)
			//{
				//transform.position += new Vector3(inputVec.x * moveSpeed,0,0) *Time.deltaTime;
				characterController.Move(vector);
			//}		
			
	        anim.SetFloat("Speed", speed);
		}
			
		
	}


	void Disparar()
	{
		float signo = Mathf.Sign(transform.forward.x); 
		
		if (isWallCollided && anim.GetBool("Air"))
			signo = -signo;

		if (Input.GetKey(fireShooting))  //ideal seria un valor continuo para que sea suave
		{
			anim.SetLayerWeight(1, 1.0f);
			if(signo >0)
			{
				IK_R.weight =1;
				IK_L.weight =0; 
			}
			else
			{
				IK_R.weight =0;
				IK_L.weight =1;
			}
			
			

			_counterFireShooting += Time.deltaTime;


			if (Input.GetKeyDown(fireShooting))
			{				
				bullet = Instantiate(bulletBase, firePoint.position, Quaternion.Euler(0, 90 * signo , 0));  
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward *  bulletSpeed_1; //Vector3.right * (firePoint.right.z * bulletSpeed_1);

				SonidosPlayer.resource = sonidos[0];
				if(!SonidosPlayer.isPlaying)
					SonidosPlayer.Play();

			}

			if(_counterFireShooting > 0.8f)
			{
				VFX_brilloCarga.SetFloat("_Carga", 1.0f);
				particulasCarga.SetActive(true);

						
				if(!SonidosPlayer.isPlaying)
				{
					SonidosPlayer.resource = sonidos[3];	
					SonidosPlayer.Play();
				}
			}

			if(_counterFireShooting > 2f)
			{				
				colorCargaMat.SetColor("_Color", chargerColor_2);
				SonidosPlayer.resource = sonidos[4];
				SonidosPlayer.loop = true;
				if(!SonidosPlayer.isPlaying)
					SonidosPlayer.Play();
			}
				
				
		}
		else
		{
			anim.SetLayerWeight(1, 0.0f);
			IK_R.weight =0; 
			IK_L.weight =0;
		}

		if( Input.GetKeyUp(fireShooting))
		{
			//Debug.Log(_counterFireShooting);
		
			if (_counterFireShooting >= 1f && _counterFireShooting < 2)
			{				

				bullet = Instantiate(bullet2, firePoint.position, Quaternion.Euler(0, 90 *signo, 0));
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				
				bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward  * bulletSpeed_2;
				particulasCarga.SetActive(false);

				SonidosPlayer.resource = sonidos[1];
				if(!SonidosPlayer.isPlaying)
					SonidosPlayer.Play();
			}		
			else if ( _counterFireShooting >= 2 )
			{
						
				bullet = Instantiate(bullet3, firePoint.position, Quaternion.Euler(0, 90 * signo, 0));
				bullet.gameObject.SetActive(true);
				bullet.transform.position = firePoint.position;
				
				bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward *  bulletSpeed_3; //buscar forma sin rigidbody para balas porsiaca
				particulasCarga.SetActive(false);

				SonidosPlayer.loop = false;
				SonidosPlayer.resource = sonidos[2];
				if(!SonidosPlayer.isPlaying)
					SonidosPlayer.Play();				
			}

			_counterFireShooting = 0.0f;
			VFX_brilloCarga.SetFloat("_Carga", 0.0f);
			 colorCargaMat.SetColor("_Color", chargerColor_1);
			particulasCarga.SetActive(false);
		}

	}


	
	void Update()
	{		
		inputVec.x = Input.GetAxis("Horizontal");
		inputVec.y = Input.GetAxis("Vertical");

		Vector3 moveVector = new Vector3 ( inputVec.x * moveSpeed,jumpHeight*inputVec.y* moveSpeed ,0) ;		
		
		
		ySpeed += Physics.gravity.y * Time.deltaTime;

		deslizar = false;
		anim.SetBool("Left", false);
		
		if(deslizar && headingLeft) // creo que ni funciona
		{
			anim.SetBool("Left", true);
			//transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 270, 0), Time.deltaTime * 20f);//??
		}

		if(isGrounded)
		{
			anim.SetBool("Air", false);
			anim.SetBool("IdleWalk", true);
			ySpeed = 0;
			if(inputVec.y > 0) 
			{
				ySpeed = jumpSpeed;
				anim.SetTrigger("Jump");
				anim.SetBool("Air", true);
				anim.SetBool("IdleWalk", false);
			}
			else 
			{
				ySpeed = 0;
				//anim.SetBool("IdleWalk", true);
				//anim.SetBool("Air", false);
			}
			if(!isWallCollided)
			{
				anim.SetBool("Wall", false);
				anim.SetBool("Air", false);
			}

		}
		else
		{

			anim.SetBool("Air", true);
			anim.SetBool("IdleWalk", false);
			if(isWallCollided)
			{
				
				anim.SetBool("Air", true);
				anim.SetBool("Wall", true);			
				
				moveVector.y =  -Mathf.Abs(inputVec.x);
				deslizar= true;
				if (Mathf.Abs(moveVector.x) < 1f) //si dejo de moverme hacia adelante chocando con pared en aire
				{
					moveVector.x = -(transform.forward.x) * /*Mathf.Abs(inputVec.x) **/ 5.0f; //mover un poco en direccion opuesta a la pared, funciona?
					deslizar = false;
					ySpeed = jumpSpeed/jumpHeight;

				}

				if(inputVec.y > 0) 
				{
					moveVector.x = -(transform.forward.x) /** Mathf.Abs(inputVec.x)*/ * 20.0f;// funciona como deberia?
					ySpeed = jumpSpeed;
					anim.SetTrigger("Jump");
					anim.SetBool("Air", true);
					anim.SetBool("IdleWalk", false);
					deslizar = false;
				}
				
					
			} 

			else
			{
				anim.SetBool("Wall", false);
				anim.SetBool("Air", true);
				if (Mathf.Abs(moveVector.x) < 1f) //si dejo de moverme hacia adelante chocando con pared en aire
				{
					deslizar = false;
					//moveVector.y = ySpeed * jumpHeight;
					//moveVector.y = 0;//0.01f; // caida libre?
					

				}

			}
		}

				
		if(!deslizar)//!isWallCollided)
			moveVector.y = ySpeed * jumpHeight;
		MovePlayer(moveVector* Time.deltaTime);		
		RotatePlayer();
		Disparar();		

	
	
	}

	public void timelineAnimFin() // hmmmm
	{
		anim.SetBool("Fin",false);
	}
	
	void FixedUpdate ()
	{

		GroundedCheck();
		WallCheck();
	}

	public List<AudioClip> GetAudios()
	{
		return sonidos;
	}
	
    public void GroundedCheck() // mascara para que no sale encima de balas o enemigos?
    {
    	LayerMask layerMask = LayerMask.GetMask("Piso", "PisoDestruible"); // evitar que salte sobre proyectiles y cosas que no sean suelo ,a ver
    	isGrounded = Physics.Raycast(
		    transform.position, 
		    jumpDirection,
		    groundTolerance,
		    layerMask
	    );
    	//print("isGrounded: " + isGrounded);
    	//Debug.DrawRay(transform.position , jumpDirection * groundTolerance, Color.yellow); 

    }

    public void WallCheck()
    {
    	Vector3 centro = transform.position + new Vector3(0,0.5f,0);
    	RaycastHit hit;
    	LayerMask layerMask = LayerMask.GetMask("Piso", "PisoDestruible"); // evitar que salte sobre proyectiles y cosas que no sean suelo ,a ver
    	float distancia = 0.5f;
    	isWallCollided = Physics.Raycast(
		    centro, 
		    transform.forward,
		   	out hit,
		    distancia,
		    layerMask
	    );

    	if ( isWallCollided && !hit.transform.gameObject.CompareTag("Wall") )
    		isWallCollided = false;

    	//Debug.Log("hay pared?: " + isWallCollided);
    	//Debug.DrawRay(centro , transform.forward * distancia, Color.yellow); 
    }


}

