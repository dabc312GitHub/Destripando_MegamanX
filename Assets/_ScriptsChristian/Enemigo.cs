using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

public class Enemigo : MonoBehaviour
{
    [SerializeField] private PlayableDirector abejaDirector;
    [SerializeField] private Camera camara;
    [SerializeField] private Transform sueloAbeja;
    [SerializeField] private int id = 0;
    [SerializeField] private Transform salidaMisiles;
    [SerializeField] private Transform misil;

    private float puntosDeVida;
   // private float ataqueA;
   // private float ataqueB; no es necesario a estas alturas
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
    private bool derecha = false;

    private Transform crusherCrusher = null;
    private Transform crusherTarget = null; // simpre con un decimal en inspector o se malogra
    private Vector3 crusherOri;
    private bool crusherSubiendo = false;
    private bool cicloCrusher = false;
    private bool CrusherMirar = false;

    [SerializeField] private Transform bombbeenMina;
    private bool bombbeenRetirada = false;    
    private  List<Transform> minas = new List<Transform>() ;
    private int i = 1;
    private float bombbeenTiempo = 0;
    

    private float contadorBee = 0;
    private bool activarBee = false;

    [SerializeField] private Transform balaRoadAt;    
    private Transform roadAtFiringPos;   
    private float contadorRoad = 0;

    private bool ataqueInicio = false;

    [SerializeField] private Transform enemigoInstancia;  
    private Transform naveOrigenAttackers;


    private Transform megaman;

    private enum TipoEnemigo
    {
        spiky,
        crusher,
        ball,
        gunVolt,
        bee,
        bombbeen,
        jamminger,
        roadAt,
        nave,
        otro,  //masa del crusher y proyectiles tal vez deberian estar aqui
        boss
    }

    private TipoEnemigo tipoEnemigo;

    private  AnimEvents animEventsScript;

    void Start() // me parece que Enemigo.cs e Impacto.cs deberian fusionarse e incluir proyectil como tipo de enemigo
    {
        
        switch(id) //redundante?, deberia usar herencia? o interfaz? ya me olvide que es una interfaz
        {
            case 0: 
                tipoEnemigo = TipoEnemigo.spiky;
                IniSpiky();
                break;
            case 1:
                tipoEnemigo = TipoEnemigo.crusher;
                IniCrusher();
                break;
            case 2:
                tipoEnemigo = TipoEnemigo.ball;
                IniBall();
                break;
            case 3:
                tipoEnemigo = TipoEnemigo.gunVolt;
                IniGunVolt();
                break;
            case 4:
                tipoEnemigo = TipoEnemigo.bee;
                IniBee();
                break;
            case 5:
                 tipoEnemigo = TipoEnemigo.bombbeen;
                 IniBombBeen();
                 break;
            case 6:
                 tipoEnemigo = TipoEnemigo.jamminger;
                 IniJamminger();
                 break;
            case 7:
                tipoEnemigo = TipoEnemigo.roadAt;
                IniRoadAt();
                break;
            case 8:
                tipoEnemigo = TipoEnemigo.nave;
                IniNave();
                break;
            case 99:
                tipoEnemigo = TipoEnemigo.boss;
                IniBoss();
                break;
            default:
                tipoEnemigo = TipoEnemigo.otro; //estoy usando este para masa de crusher? // 9
                IniOtro();
                break;
        }
        animator = GetComponent<Animator>();

        if (tipoEnemigo != TipoEnemigo.otro)
            mat = transform.GetChild(0).GetComponent<Renderer>().material;
        else
            mat =  null;

        if(tipoEnemigo == TipoEnemigo.boss)
        {
            var matArray = transform.GetChild(1).GetComponent<Renderer>().materials;
            matArray[1] = mat;
            transform.GetChild(1).GetComponent<Renderer>().materials = matArray;
        }

        col = GetComponent<Collider>();
        explotaScript = GetComponent<Explotar>();
        animEventsScript = GetComponent<AnimEvents>();

        megaman = GameObject.Find("megamanxCompleto").transform;


    }

    void Update()
    {
        if(efectoDamage)
        {
            contadorEfectoDamage -=Time.deltaTime;
            if(contadorEfectoDamage <= 0)
            {
                if(mat)
                    mat.SetFloat("_Damage",0);
                efectoDamage = false;
            }
        }

        float distancia =  Vector3.Distance(transform.position,megaman.position);

        
        if(tipoEnemigo == TipoEnemigo.spiky)
        {
             if( distancia < 8)
                ataqueInicio = true;

            if(ataqueInicio)
            spiky_comportamiento();
        }
        else if(tipoEnemigo == TipoEnemigo.ball)
        {
            ball_comportamiento();
        }
        else if (tipoEnemigo == TipoEnemigo.crusher)
        {
            if(!CrusherMirar)
             CrusherMirar = crusher_preComportamiento();
             //CrusherMirar = true ; //Debug
             if(CrusherMirar) // si aparece megaman a la vista;
                crusher_comportamiento(); 
        }
        else if( tipoEnemigo == TipoEnemigo.gunVolt)
        {
            gunVolt_comportamiento();

        }
        //movimiento de bee se maneja con timeline
        else if ( tipoEnemigo == TipoEnemigo.bee)
        {
            if(activarBee)
                bee_comportamiento();
            contadorBee += Time.deltaTime;
            if(contadorBee > 3)
                activarBee= true;
        }

        else if ( tipoEnemigo == TipoEnemigo.bombbeen)
        {
           
            bombeen_comportamiento();
            
           
            if(bombbeenRetirada && i <4)
            {
                 animator.SetLayerWeight(1, 1);                
                if(bombbeenTiempo <=2)
                {
                    float tiempoSimple = Mathf.Round(bombbeenTiempo * 10) *0.1f;
                    
                    if (Mathf.Approximately(tiempoSimple, 0.6f*i))
                    {

                        minas.Add ( Instantiate(bombbeenMina));
                        minas[i-1].position = transform.GetChild(1).transform.position; 
                        minas[i-1].GetComponent<Rigidbody>().AddForce(-1.0f-i, 0, 0, ForceMode.Impulse);                    
                        i++; 
                    }
                   
                }
                bombbeenTiempo += Time.deltaTime;                
            }
            else
            {
                vectorMov.x = - velocidadMov;
                 animator.SetLayerWeight(1, 0); 
            } 
                
        }

        else if ( tipoEnemigo == TipoEnemigo.jamminger)
        {
            if(puntosDeVida > 0) //deberia hacer este check con todos?
                jamminger_comportamiento(); //comportamiento original es un poco mas complejo
        }

        else if ( tipoEnemigo == TipoEnemigo.roadAt)
        {
             
            if(ataqueInicio == false && distancia < 8)
                ataqueInicio = true;

            if(ataqueInicio)
            {
                roadAt_comportamiento();

                if(contadorRoad > 2)
                {
                    roadAtAtaque();
                    contadorRoad = 0;
                }
                contadorRoad += Time.deltaTime;
            }
           
        }

        /* TIMELINE tiene la prioridad,no son simultaneas, lo ignora. Constraint funciona pero dificil empalmar posiciones del timeline
        else if( tipoEnemigo == TipoEnemigo.nave)
        {
            Vector3 megamanPos = megaman.position;
            float x = Mathf.Lerp( transform.position.x, megaman.position.x, Time.deltaTime * 10f );
            transform.position = new Vector3( x,transform.position.y,transform.position.z);
            //seguir a megaman ligeramente?

        }*/

        else if(tipoEnemigo == TipoEnemigo.boss)
        {
            boss_comportamiento();
        }
    }

    public void InstanceAttacker() // se llama como evento en animacion misma
    {
       Transform attacker = Instantiate(enemigoInstancia);
       attacker.position = naveOrigenAttackers.position;
  
    }


    void FixedUpdate()
    {   
            // Condicional para los que usan esto?
         GroundedCheck();
        if(!isWallCollided) // solo una vez?
            WallCheck();

    } 


    void spiky_comportamiento() 
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
            Movimiento(vectorMov); //Spiky tambien se mueve en una rampa a ver como la hago sin RigidBody :P, bueno lo logré maso :P*/
            
    }

    void ball_comportamiento()
    {
        bool atacar = false;
        float sentido = -1;
        if (derecha)
            sentido = 1;
     
        Quaternion rot; 
        if (derecha)
           rot = Quaternion.Euler(0, 90, 0);
        else 
           rot = Quaternion.Euler(0, -90, 0);

         vectorMov.x = sentido * velocidadMov;

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 2.0f);

        animator.SetTrigger("Pararse"); //solo una vez pero hmm luego veo como evitar que se llame seguido
        Movimiento(vectorMov);
    }

    void gunVolt_comportamiento()
    {
        List<Vector3> salidas = animEventsScript.getSalidas();
       
        float alto = salidas[0].y;
        float bajo = salidas[2].y;

        

        Vector3 origenA = new Vector3(salidas[4].x,alto,salidas[4].z );  //salidas[4];
        Vector3 origenB = new Vector3(salidas[4].x,bajo,salidas[4].z );  //salidas[4];

        RaycastHit hit;
         LayerMask layerMask = LayerMask.GetMask("Default"); // ignora paredes invisiblse
        float distancia = 8;
        bool atacableA = Physics.Raycast(
            origenA, 
            new Vector3(-1,0,0),
            out hit,
            distancia,
            layerMask
        );
        /*
        bool atacableB = Physics.Raycast(  // no necesito ambos para la version sencilla
            origenB, 
            new Vector3(-1,0,0),
            out hit,
            distancia
        );*/

        //Debug.DrawRay(origenA , new Vector3(-1,0,0) * distancia, Color.yellow); 
        //Debug.DrawRay(origenB , new Vector3(-1,0,0) * distancia , Color.yellow);

        if (hit.collider!=null)
        {
        
             if( hit.transform.gameObject.CompareTag("Player"))
            {
                animator.SetBool("Attack", true);
                //ataque se dispara en la animacion 
                StartCoroutine("EsperarGunVolt",2);              
                
            }
            else
                 animator.SetBool("Attack", false);
        }
           


    }

    IEnumerator EsperarGunVolt(float sec) //solo para GunVolt?
    {
        yield return new WaitForSeconds(sec);
        animator.SetBool("Attack", false); 
        yield return null;
    }




    bool crusher_preComportamiento() //megaman a la vista
    {
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Default"); // megaman en default, para evitar que choque con sus limites de movimiento
        float distancia = 6;
        Vector3 origen = transform.position + new Vector3(0,-1.1f,0);
        bool megaman = Physics.Raycast(
            origen,
            new Vector3(-1,0,0),
            out hit,
            distancia,
            layerMask
        );
       
        //Debug.DrawRay(origen , new Vector3(-1,0,0) * distancia, Color.yellow);
        if (megaman)
        {

             if( hit.transform.gameObject.CompareTag("Player"))
             {
                //Debug.Log(hit.transform.gameObject);
                return true;
             }
             return false;
               
               
        }
        return false;
    }

    void crusher_comportamiento() // tags para evitar que se bugee cuando hay muchos y coinciden casi al mismo tiempo en atacar mismo suelo, aparentemente funca
    {
        bool atacar = false;
        float sentido = -1;
        if (derecha)
            sentido = 1;
     
        Quaternion rot; 
        if (derecha)
           rot = Quaternion.Euler(0, 160, 0);
        else 
           rot = Quaternion.Euler(0, 200, 0);

         vectorMov.x = sentido * velocidadMov;
     
             
       if(crusher_detector())
       {
            atacar = ( (int) Random.Range(0, 2) == 1 ); 
                     
            float valor =  Mathf.Round(transform.position.x * 10.0f) * 0.1f;
              
            if( Mathf.Approximately( valor, crusherTarget.position.x) && crusherTarget.tag!="PisoOcupado")
            {

                atacar = true; //&& probabilidad luego

                                
                if(atacar) 
                {
                    vectorMov.x = 0;
                    rot =  Quaternion.Euler(0, 180, 0);                        
                   
                    if ( cicloCrusher ) //se cumple ciclo
                    {
                        //inhabilitar ataque un tiempito
                       vectorMov.x =  sentido * velocidadMov;
                        if (derecha)
                           rot = Quaternion.Euler(0, 160, 0);
                        else 
                           rot = Quaternion.Euler(0, 200, 0);
                       atacar = false;
                       StartCoroutine("EsperarCrusher", atacar);

                    }
                    else
                     Crush();
                
                }
            }
       }
      

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 20f);       
        Movimiento(vectorMov);
    }

    bool crusher_detector()
    {

        LayerMask layerMask = LayerMask.GetMask("PisoDestruible");
        RaycastHit hit;
        float distancia = 1.5f;
        Vector3 origen = transform.position + new Vector3(0,-1,0);
        bool ray = Physics.Raycast(
            origen, 
            Vector3.down,
            out hit,
            distancia,
            layerMask
        );

        if(hit.collider)
        {
            //crusherTarget =  hit.collider.transform.GetChild(0);
            crusherTarget =  hit.collider.transform.parent.GetChild(0);
        }
       
        //Debug.DrawRay(origen , Vector3.down * distancia, Color.yellow); 
        return ray;
        
    }

    void Crush ()
    {
        
        Vector3 direccion = Vector3.down;
        float valor = 0; 

       ParticleSystem humo = crusherTarget.parent.GetChild(5).GetChild(0).GetComponent<ParticleSystem>();
       ParticleSystem escombros = crusherTarget.parent.GetChild(5).GetChild(1).GetComponent<ParticleSystem>();
       float bajon1 =0;
       float bajon2 = 0;
       float bajon3 =0;
       int x =0;
       if (crusherTarget.parent.GetChild(2).gameObject.activeSelf)
            x = 1; 
       else if (crusherTarget.parent.GetChild(3).gameObject.activeSelf)
            x= 2;

       if(crusherTarget)
       {
             bajon1 =  crusherTarget.parent.position.y;// +0.4  -0.4f;
             bajon2 =  crusherTarget.parent.position.y - 0.4f;
             bajon3 = crusherTarget.parent.position.y -0.8f;
       }
      

       if(!crusherSubiendo) //bajando
       {
            if(!crusherTarget.gameObject.activeSelf ) // si ya no hay tierrita abajo
            {
                 cicloCrusher = true;
            }
            else
            {
                crusherTarget.parent.GetChild(1+x).tag="PisoOcupado";
                crusherCrusher.position += (direccion * 2f * Time.deltaTime);
                valor =  Mathf.Round(crusherCrusher.position.y * 10.0f) * 0.1f;

                                 
                if( Mathf.Approximately(valor,crusherTarget.position.y) ) // si coincide en altura
                {                
                    //destruir poco el piso y subir // a veces rompe hasta dos bloques? o 3?
                    
                    float posY = crusherTarget.position.y - 0.4f;
                    posY = Mathf.Round( posY * 10.0f  ) * 0.1f;                   
                    crusherTarget.position = new Vector3(crusherTarget.position.x,posY,crusherTarget.position.z);                     
                    escombros.transform.parent.position +=  new Vector3(0, -0.4f ,0);
                  
                    crusherTarget.parent.GetChild(5).gameObject.SetActive(true);
                    crusherTarget.parent.GetChild(1).gameObject.SetActive(false);   

                    //Debug.Log(crusherTarget.position);                                         
                     
                   
                     if( Mathf.Approximately(crusherTarget.position.y, bajon1) )  // 0  //crusherTarget.position.y
                     {
                        crusherTarget.parent.GetChild(2).gameObject.SetActive(true);
                        crusherTarget.parent.GetChild(1).gameObject.SetActive(false);                       

                     }

                     if(  Mathf.Approximately(crusherTarget.position.y,bajon2 ))  //.5
                     {                      
                        
                        crusherTarget.parent.GetChild(2).gameObject.SetActive(false);
                        crusherTarget.parent.GetChild(3).gameObject.SetActive(true);
                        humo.Stop();
                        humo.Play();
                        escombros.Stop();
                        escombros.Play();

                     }

                     else if (  Mathf.Approximately(crusherTarget.position.y, bajon3 ) ) //.1
                     {                        
                        crusherTarget.parent.GetChild(3).gameObject.SetActive(false);
                        crusherTarget.parent.GetChild(4).gameObject.SetActive(true);
                        
                        humo.Stop();
                        humo.Play();
                        escombros.Stop();
                        escombros.Play();
                        crusherTarget.gameObject.SetActive(false);
                     }

                     crusherSubiendo = true;
                }
            }
                       
       }

       else //subiendo
       {
            crusherTarget.parent.GetChild(1+x).tag="Untagged";
            direccion.y = 1;            
            crusherCrusher.position += (direccion * 0.7f * Time.deltaTime);
            valor =  Mathf.Round(crusherCrusher.position.y * 10.0f) * 0.1f;
             //Debug.Log(valor + " " + crusherOri.y);
            if( Mathf.Approximately(valor,crusherOri.y ))
            {                
                crusherSubiendo = false;
                cicloCrusher = true;                
            }
       }
        
       
    }

    IEnumerator EsperarCrusher(bool atacar) 
    {
        yield return new WaitForSeconds(2);
        atacar = true;
        cicloCrusher = false;
        yield return null;
    }


    void bee_comportamiento()
    {
        Vector3 origenA = salidaMisiles.position;       

        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Default"); // ignora paredes invisiblse
        float distancia = 5;
        bool atacableA = Physics.Raycast(
            origenA, 
            new Vector3(-1,0,0),
            out hit,
            distancia,
            layerMask
        );

        
        if (atacableA)
        {    
            if( hit.transform.gameObject.CompareTag("Player"))
            {               
               ataqueMisil();
               contadorBee = 0;
               activarBee = false;
            }
        }

    }

    public void ataqueMisil() 
   {

        Transform proyectil;
        proyectil =  Instantiate(misil);         

        proyectil.position = salidaMisiles.position; 
        StartCoroutine("moverAtaque",proyectil);
   }


     IEnumerator EsperarBee(float sec) // para bee 
    {
        yield return new WaitForSeconds(sec);   
        ataqueMisil();
        yield return null;
    }

   IEnumerator moverAtaque(Transform attack) 
   {
      float tiempo = 5.0f;
      while (tiempo > 0)
      {
        if(attack)
            attack.position += Vector3.left * Time.deltaTime * 3.0f; 
         tiempo -=Time.deltaTime;
         
         yield return null;
      }
      //attack.position = new Vector3(0,-20,0); no es necesario, el antibalas lo maneja?, si lo descomentas null exception
      yield return null; 
   }

   
   void bombeen_comportamiento()
   {
        col.enabled = true;
        
        if( !bombbeenRetirada)
         vectorMov.x = -velocidadMov;
       
        float distanciaX = transform.position.x - megaman.position.x;
        
        if( megaman.position.x > transform.position.x + 2f) //comparar posiciones es mejor que usar rayCast para detectar megaman, seria bueno usarlo en otros enemigos
        {
            bombbeenRetirada  = true;
            vectorMov.x = 0;
            vectorMov.y = velocidadMov;
            if( transform.position.y >= 8)
                Damage(100);

        }

        
        if (!bombbeenRetirada  && distanciaX > 7.0f ) //detectar megaman
        {
            col.enabled = false;
            vectorMov.x = 0;
        }


        if(!bombbeenRetirada  && distanciaX <= 2f)
        {
           bombbeenRetirada  = true;
           vectorMov.x = 0;
           //StartCoroutine("EsperarBombbeen");
            

        }

        
        Movimiento(vectorMov);
   }

   IEnumerator EsperarBombbeen()
   {
         vectorMov.x = 0;
        
         yield return new WaitForSeconds(3);
         vectorMov.x = -velocidadMov;
         yield return null;
   }


   void jamminger_comportamiento()
   {  
        float distancia =  Vector3.Distance(transform.position,megaman.position);
                   
        if( distancia < 7)     
            transform.position =  Vector3.SmoothDamp(transform.position, megaman.position + new Vector3(1,1.2f,0) ,ref velocity, 0.5f);

       //Movimiento(vectorMov); // falta un ataque, se acerca y se retira rapido un poco hacia arriba, si megaman se mueve retoma comportamiento norma?
      
   }

 

   void roadAt_comportamiento()
   {
        LayerMask layerMask = LayerMask.GetMask("Piso", "PisoDestruible"); //mismos layer para el caso megaman (impedir saltar sobre balas y enemigos a veces)
        RaycastHit hit;
        bool suelo = Physics.Raycast(
            transform.position, 
            Vector3.down,
            out hit,
            0.1f,
            layerMask
        );


        int orientacion = -1;        
        vectorMov.y = 0;

        Quaternion rot = transform.rotation;
        

        int overdrive = 5;
        
        if(!suelo)
        {
                
           vectorMov.y = -velocidadMov;
           Movimiento(vectorMov); 
        }

        else
        {
            
             if( hit.collider.tag == "ZonaMuerte") //se destruye ya no necesario?
            {
                vectorMov.y = -velocidadMov;
                Movimiento(vectorMov);
            }

          
            float x = transform.position.x;

            if(Quaternion.Dot(transform.rotation,Quaternion.Euler(0, -90, 0) ) > 0.1f  ) //transform.rotation == Quaternion.Euler(0, -90, 0))
                 x = Mathf.MoveTowards( x, megaman.position.x - overdrive , Time.deltaTime * velocidadMov);

           
            if( Quaternion.Dot(transform.rotation,Quaternion.Euler(0, 90, 0)) > 0.1f )//transform.rotation ==  Quaternion.Euler(0, 90, 0) ) 
                x = Mathf.MoveTowards(x, megaman.position.x + overdrive , Time.deltaTime * velocidadMov);
                         
            transform.position = new Vector3 (x,transform.position.y,transform.position.z) ;

        }

       
      float simpleIzq = Mathf.Round((megaman.position.x - overdrive) *10 ) * 0.1f;
      float simpleDer = Mathf.Round((megaman.position.x + overdrive) *10 ) * 0.1f;
      float simplePos = Mathf.Round(transform.position.x  *10 ) * 0.1f;
  
       
        if( Mathf.Approximately( simpleIzq , simplePos ) )
        {            
            rot = Quaternion.Euler(0, 90, 0);
        }

        else if( Mathf.Approximately( simpleDer , simplePos ) )
        {
             rot = Quaternion.Euler(0, -90, 0);
        } 
       
        //transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 20f);
        transform.rotation = rot; // si aparencen errores que sea giro brusco, menos vistoso pero menos errores ( si hubiera)

   }



   void roadAtAtaque() 
   {
        Vector3 pos = roadAtFiringPos.position;
     
        Transform bala = Instantiate(balaRoadAt);
        bala.position = pos;       
        bala.GetComponent<Rigidbody>().linearVelocity = new Vector3(-transform.right.z * 6,0,0);
   }


    void boss_comportamiento()
    {
        float x = transform.position.x;
        x  = Mathf.MoveTowards( x, megaman.position.x, Time.deltaTime * velocidadMov); 
       
        
        transform.position = new Vector3 (x,transform.position.y,transform.position.z) ;

    }


    void Movimiento(Vector3 vectorMov) 
    {
        transform.position += vectorMov * Time.deltaTime;                
    }

    void Rotar()
    {
        derecha = !derecha;       
    }

    private float groundTolerance =0.1f;
    private bool isGrounded = false;
    private bool isWallCollided = false;


    void GroundedCheck()
    {
       // LayerMask layerMask = LayerMask.GetMask("Piso", "PisoDestruible"); //no es necesario, solo lo usa Spiky?
        isGrounded = Physics.Raycast(
            transform.position, 
            Vector3.down,
            groundTolerance
            //layerMask
        );
        
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

        efectoDamage = false; //si muere deja de brillar?
        if(mat)
            mat.SetFloat("_Damage",0); 

        if( tipoEnemigo == TipoEnemigo.spiky)
        {
            col.enabled = true; // daña hasta que explota
            animator.SetTrigger("Muerte");           
        }

        else if(tipoEnemigo == TipoEnemigo.gunVolt)
        {
            animator.SetBool("Attack",false);
            explotaScript.Explota();
        }

        else if (tipoEnemigo == TipoEnemigo.ball)
        {
            explotaScript.Explota();
        }

        else if(tipoEnemigo == TipoEnemigo.crusher)
        {
            explotaScript.Explota(); //la maza no se destruye, el cuerpo explota y la maza debería caer al infinito o explotar al contacto del suelo
        }

        else if(tipoEnemigo == TipoEnemigo.bee)
        {
            bool segundoRound = false;

            if( transform.parent.parent.name == "EnemigosPlat05" )
            {
                segundoRound = true;
                sueloAbeja = GameObject.Find("PisoAbejaDerrumbeB").transform;
            }
                       
            abejaDirector.Stop();
            transform.parent.SetParent(GameObject.Find("EnemigosPlat05").transform);
            transform.parent.transform.localPosition = Vector3.zero;
            transform.parent.GetChild(0).gameObject.SetActive(false);
            transform.position += new Vector3( 0,20,0);
            col.enabled = true;            

            if(!segundoRound)
                IniBee();


            //Piso caerse
            sueloAbeja.GetComponent<Rigidbody>().isKinematic = false;
            sueloAbeja.GetComponent<Rigidbody>().useGravity = true;
            //camara resucita
            camara.GetComponent<CameraMove>().setCamaraQuieta(false); 
            //Activar Abeja Destruida
        } 

        else if (tipoEnemigo == TipoEnemigo.bombbeen)
        {
            explotaScript.Explota();
            vectorMov = Vector3.zero; // ayuda?
        }   

         else if (tipoEnemigo == TipoEnemigo.jamminger)
        {
            explotaScript.Explota();
            vectorMov = Vector3.zero; // ayuda?
        }  

        else if (tipoEnemigo == TipoEnemigo.roadAt)
        {
            explotaScript.Explota();
            vectorMov = Vector3.zero; // este si se mueve muertito
            //efectitos y que salga el destruido?
        }              
           
        
    }

    void Damage(float d)
    {
        if(!efectoDamage)
            contadorEfectoDamage = 0.5f;

        efectoDamage = true;

        puntosDeVida -=d;
        if(mat)        
            mat.SetFloat("_Damage",1);

        if(puntosDeVida <=0)
            Muerte();
    }

     void IniSpiky()
    {
        puntosDeVida = 2;
        ataqueCol = 2;
  
        velocidadMov = 2.5f; //digamos

        //velocidadMov = 0; //debug
        //puntosDeVida = 10;
    }

    void IniCrusher()
    {
        puntosDeVida = 4;
        ataqueCol = 4;
 
        velocidadMov = .8f;

        crusherCrusher = transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1); // no hay mejora manera?
        crusherOri = crusherCrusher.position;
        crusherOri.y =  Mathf.Round(crusherOri.y * 10.0f) * 0.1f; //redondeando a un decimal para comparar sin problemas

    }

    void IniBall()
    {
        puntosDeVida = 4;
        ataqueCol = 1;
 
        velocidadMov =0.5f;
    }

    void IniGunVolt()
    {
        puntosDeVida = 16;
        ataqueCol = 3;

        velocidadMov = 0;
    }

    void IniBee()
    {
        puntosDeVida = 32;
        ataqueCol = 4;
    
        velocidadMov = 2;
    }

    void IniBombBeen()
    {
        puntosDeVida = 2;
        ataqueCol = 2;
   
        velocidadMov = 2;
    }

    void IniJamminger()
    {
        puntosDeVida = 2;
        ataqueCol = 1;
    
        velocidadMov = 3;
    }

    void IniRoadAt()
    {
        puntosDeVida = 12; // en 7 muere conductor,en 3 se queda quieto y explota
        ataqueCol = 2;
  
        velocidadMov = 3;

        roadAtFiringPos = transform.GetChild(4);
    }

    void IniOtro()
    {
        puntosDeVida = 9999; // no recibe daño(otro modo que no sea 9999?), si muere con maza abajo masa cae 
        ataqueCol = 4;

        velocidadMov = 0;
    }

    void IniNave() 
    {
        puntosDeVida = 0;
        ataqueCol = 0;
 
        velocidadMov = 0;

        naveOrigenAttackers= transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1); //no hay mejora manera?

    }

    void IniBoss()
    {
        puntosDeVida = 9999;
        ataqueCol = 1;
        velocidadMov = 2;
    }



    void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.CompareTag("MegamanAtaqueA")) //basico
        {
            Damage(1); // si balas basicas chocan deberian desparecer, las fuerets si traspasan?
            Destroy(other.gameObject); // ?? mejor pooling pero de momento no
        }
        else if(other.gameObject.CompareTag("MegamanAtaqueB")) //medio
        {
             Damage(2); //mueren en Antibalas
        }
        else if(other.gameObject.CompareTag("MegamanAtaqueC"))//alto
        {
            Damage(4); //mueren en Antibalas
        }

        if(other.gameObject.CompareTag("Limite"))
        {
            Rotar();
        }

         if(other.gameObject.CompareTag("ZonaMuerte"))
        {
            Destroy(this.gameObject);
        }
    }



}
