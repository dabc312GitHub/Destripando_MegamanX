using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [SerializeField] public int id = 0;
    private float puntosDeVida;
    private float ataqueA;
    private float ataqueB;
    private float ataqueCol;
    private float velocidadMov;
    private Animator animator;
    private Material mat;

    private float contadorEfectoDamage = 0.5f;
    private bool efectoDamage = false;
    private Vector3 velocity = Vector3.zero;
    private Vector3 vectorMov = Vector3.zero;
    private Collider col;
    private Explotar explotaScript;
    private bool bienMuerto = false;

    private enum TipoEnemigo
    {
        spiky,
        crusher,
        ball,
        gunVolt,
        bee,
        bombbeen,
        jamminger,
        roadAt
    }

    private TipoEnemigo tipoEnemigo;

    void Start()
    {
        
        switch(id) //redundante?, deberia usar herencia? o interfaz? ya me olvide que es una interfaz
        {
            case 0: 
                tipoEnemigo = TipoEnemigo.spiky;
                IniSpiky();
                break;
            case 1:
                tipoEnemigo = TipoEnemigo.crusher;
                break;
            case 2:
                tipoEnemigo = TipoEnemigo.ball;
                break;
            case 3:
                tipoEnemigo = TipoEnemigo.gunVolt;
                break;
            case 4:
                tipoEnemigo = TipoEnemigo.bee;
                break;
            case 5:
                 tipoEnemigo = TipoEnemigo.bombbeen;
                 break;
            case 6:
                 tipoEnemigo = TipoEnemigo.jamminger;
                 break;
           default:
                tipoEnemigo = TipoEnemigo.roadAt;
                break;
        }
        animator = GetComponent<Animator>();
        mat = transform.GetChild(0).GetComponent<Renderer>().material;
        col = GetComponent<Collider>();
        explotaScript = GetComponent<Explotar>();
    }

    void Update()
    {
        if(efectoDamage)
        {
            contadorEfectoDamage -=Time.deltaTime;
            if(contadorEfectoDamage <= 0)
            {
                mat.SetFloat("_Damage",0);
                efectoDamage = false;
            }
        }

        Debug.Log(vectorMov.x);
        
        if(tipoEnemigo == TipoEnemigo.spiky)
        {
            if( !bienMuerto && puntosDeVida==0 && vectorMov.x > -0.05f ) //recien quieto, puntos de vida 0 porque al inicio vectorx es zero
            {
                //recien explotar solo Spiky fuera de evento de anim porque se mueve un poquito muertito
                explotaScript.Explota();
                bienMuerto = true;
            }

            if(isWallCollided) // el parche...
            {
                vectorMov.x = Mathf.SmoothDamp( vectorMov.x, 0, ref velocity.x , 0.1f);
                //Debug.Log(vectorMov.x);
            }
            else
            {
                if( puntosDeVida == 0)
                {
                    //vectorMov = Vector3.SmoothDamp(vectorMov, Vector3.zero, ref velocity, 0.3f);
                    vectorMov.x = Mathf.SmoothDamp( vectorMov.x, 0, ref velocity.x , 0.8f);
                    //vectorMov.x = Mathf.Lerp( vectorMov.x, 0, 0.1f);                    
                   
                }
                else
                    vectorMov.x = -1 *velocidadMov;

                if(!isGrounded)    
                    vectorMov.y += Physics.gravity.y * Time.deltaTime * 0.1f;
            }
         
         if(!bienMuerto)  
            Movimiento(vectorMov); //Spiky tambien se mueve en una rampa a ver como la hago sin RigidBody :P, bueno lo logré maso :P

        }
    }

    void FixedUpdate()
    {
        GroundedCheck();
        if(!isWallCollided) // solo una vez?
            WallCheck();
    } 

    void Movimiento(Vector3 vectorMov) 
    {
        transform.position += vectorMov * Time.deltaTime;
    }

    private float groundTolerance =0.1f;
    private bool isGrounded = false;
    private bool isWallCollided = false;


    void GroundedCheck()
    {
        isGrounded = Physics.Raycast(
            transform.position, 
            Vector3.down,
            groundTolerance
        );
        //print("isGrounded: " + isGrounded);
       // Debug.DrawRay(transform.position , Vector3.down * groundTolerance, Color.yellow); 

    }

    public void WallCheck()
    {
        Vector3 centro = transform.position + new Vector3(0,0.5f,0);
        RaycastHit hit;
        float distancia = 0.5f;
        isWallCollided = Physics.Raycast(
            centro, 
            new Vector3(-1,0,0),
            out hit,
            distancia
        );

        if ( isWallCollided && !hit.transform.gameObject.CompareTag("Wall") )
            isWallCollided = false;

        //Debug.DrawRay(centro , new Vector3(-1,0,0) * distancia, Color.yellow); 
    }

    public int getId()
    {
        return id;
    }

    public float getAtaqueCol()
    {
        return ataqueCol;
    }

    void Muerte()
    {
        if(col)
            col.enabled = false;
        puntosDeVida = 0;

        if( tipoEnemigo == TipoEnemigo.spiky)
        {
            animator.SetTrigger("Muerte");
            efectoDamage = false; //si muere deja de brillar?
            mat.SetFloat("_Damage",0);           
        }
           
        
    }

    void Damage(float d)
    {
        if(!efectoDamage)
            contadorEfectoDamage = 0.5f;

        efectoDamage = true;

        puntosDeVida -=d;        
        mat.SetFloat("_Damage",1);

        if(puntosDeVida <=0)
            Muerte();
    }

     void IniSpiky()
    {
        puntosDeVida = 2;
        ataqueCol = 2;
        ataqueA = 0;
        ataqueB = 0;
        velocidadMov = 2.5f; //digamos

        //velocidadMov = 0; //debug
        //puntosDeVida = 10;
    }

    void IniCrusher()
    {
        puntosDeVida = 4;
        ataqueCol = 4;
        ataqueA = 0;
        ataqueB = 0;
        velocidadMov = 2;
    }

    void IniBall()
    {
        puntosDeVida = 4;
        ataqueCol = 1;
        ataqueA = 0;
        ataqueB = 0;
        velocidadMov =1;
    }

    void IniGunVolt()
    {
        puntosDeVida = 16;
        ataqueCol = 3;
        ataqueA = 2; // misiles
        ataqueB = 2; // plasma o lo que fuere
        velocidadMov = 0;
    }

    void IniBee()
    {
        puntosDeVida = 32;
        ataqueCol = 4;
        ataqueA = 1; // ametralladora
        ataqueB = 2; // misiles
        velocidadMov = 2;
    }

    void IniBombBeen()
    {
        puntosDeVida = 2;
        ataqueCol = 2;
        ataqueA = 1; // mina
        ataqueB = 0;
        velocidadMov = 2;
    }

    void IniJamminger()
    {
        puntosDeVida = 2;
        ataqueCol = 1;
        ataqueA = 0; 
        ataqueB = 0;
        velocidadMov = 3;
    }

    void IniRoadAt()
    {
        puntosDeVida = 12; // en 7 muere conductor,en 3 se queda quieto y explota
        ataqueCol = 2;
        ataqueA = 1; 
        ataqueB = 0;
        velocidadMov = 3;
    }



    void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.CompareTag("MegamanAtaqueA")) //basico
        {
            Damage(1); // si balas basicas chocan deberian desparecer, las fuerets si traspasan?
        }
        else if(other.gameObject.CompareTag("MegamanAtaqueB")) //medio
        {
             Damage(2);
        }
        else if(other.gameObject.CompareTag("MegamanAtaqueC"))//alto
        {
            Damage(4);
        }
    }



}
