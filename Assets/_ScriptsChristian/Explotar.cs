using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explotar : MonoBehaviour
{
  
   [SerializeField] private Transform explosion;
   [SerializeField] private int explosionesMax =5;
   //[SerializeField] private int MisilesMax =5; //luego pooling de misiles, y en otro lado balas de megaman

   private float segundosInvencible;
   private static bool invulnerable = false;
   private Material matA, matB;

   private  Impacto ImpactoScript;

   public static List<Transform> poolExplosion = new List<Transform>();
   public static List<Transform> poolProyectiles;

   private Vector3 posPool;
   private Vector3 posEnemigos;
   private int indice;

   private static Transform capsulaSalud;
   public static List<Transform> capsulas = new List<Transform>();
   private int capsulaIndice;

   void Start()
   {
        if( tag != "Capsula")
        {

            ImpactoScript = GameObject.Find("megamanxCompleto").GetComponent<Impacto>();
            matA = ImpactoScript.getMaterialA();
            matB = ImpactoScript.getMaterialB();
            //matA.SetFloat("_Invulnerable",1.0f); // si se utiliza aqui se malogra???
            //matB.SetFloat("_Invulnerable",1.0f);
            segundosInvencible = ImpactoScript.getSegInvencible();

            capsulaSalud = GameObject.Find("salud").transform; // no hay otra forma que no de flojera?
            capsulaSalud.GetComponent<Rigidbody>().useGravity = false;// pa que no se caiga

            posPool = new Vector3 (0,-20,0);
            posEnemigos = new Vector3 (4,-20,0);
            indice = 0;
            capsulaIndice = 0;
            Transform exp;
            
            if(poolExplosion.Count == 0)
            {
                 for( int i =0; i< explosionesMax ; i++)
                {
                    exp =  Instantiate(explosion);
                    exp.gameObject.SetActive(false);
                    exp.position = posPool;
                    poolExplosion.Add(exp);

                    exp = Instantiate(capsulaSalud);
                    exp.gameObject.SetActive(false);
                    exp.position = posPool;
                    capsulas.Add(exp);
                }
            }

        }
       
        

   }

   void ImpactoMega(Collider Col)
   {
        if(Col.transform.parent)
            Col.transform.parent.GetComponent<Animator>().SetTrigger("Damage"); //Impacto se llama en anim
        else
            Col.transform.GetComponent<Animator>().SetTrigger("Damage"); // si es el characterController???
        invulnerable = true;
        ImpactoScript.getExtension().enabled = false;   
        matA.SetFloat("_Invulnerable",1.0f);
        matB.SetFloat("_Invulnerable",1.0f);
        StartCoroutine("Invulnerable");
        //Col.transform.parent.GetComponent<Animator>().SetBool("Damage", false); 
   }


   void OnTriggerEnter(Collider Col) 
   {
     
       if(Col.tag == "Player") // si proyectil toca player
        {
            if(!invulnerable)
            {
               if(this.tag != "Mina")
               {
                    Enemigo enemigoScript = transform.GetComponent<Enemigo>();
                    if( enemigoScript)
                    {
                        if(Col.transform.parent) // para que no toa en cuenta el Charactercontroller (si es lo que causa problemas)
                            Col.transform.parent.GetComponent<Impacto>().Damage(enemigoScript.getAtaqueCol()); //daño por chocar enemigos //puede que a veces choque con characterController?
                        else
                            Col.transform.GetComponent<Impacto>().Damage(enemigoScript.getAtaqueCol()); // necesario?
                    }

                    else if( gameObject.CompareTag("Proyectil") ) 
                    {
                        Explota();// solo explota si es proyectil
                         Col.transform.parent.GetComponent<Impacto>().Damage(2f); // todos hacen daño 2?                         
                    }
                    
                    else if( gameObject.CompareTag("Plasma") ) //plasma se comporta como proyectil pero no explota
                    {
                         this.transform.position = posEnemigos; //solo moverlo, si muere en antibalas interrumpe script y causa bug 
                        Col.transform.parent.GetComponent<Impacto>().Damage(2f); // todos hacen daño 2? 
                    }

                    else
                        Col.transform.parent.GetComponent<Impacto>().Damage(1f); //  disparo de bee y otros?
                   
                   if(!gameObject.CompareTag("Capsula"))
                        ImpactoMega(Col);       
               }

                else 
                  StartCoroutine("ExplotaTiempo",Col); //minas de bombeen deberian explotar apenas tocan suelo y no apenas toquen megaan?   
                             
            } 


            if(gameObject.CompareTag("Capsula"))
            {
                transform.parent.position = posEnemigos;
                if(Col.transform.parent) // este es para la extension
                    Col.transform.parent.GetComponent<Impacto>().Damage(-1f); // sana 1
                else // este es para el character controller
                    Col.transform.GetComponent<Impacto>().Damage(-1f); // sana 1
            }
            
        }
              
                
   }

   public IEnumerator ExplotaTiempo(Collider col)
   {
         yield return new WaitForSeconds(3f);
         Explota();
         ImpactoMega(col);
         yield return null;
   }
   


   public IEnumerator Invulnerable()
   {   
    
        yield return new WaitForSeconds(segundosInvencible);
        invulnerable = false;
        matA.SetFloat("_Invulnerable",0.0f);
        matB.SetFloat("_Invulnerable",0.0f);   
        ImpactoScript.getExtension().enabled =true;   
        
        yield return null;
   }

   
   public void Explota()
   {
       if( tag == "Untagged") // impedir que proyectiles boten capsulas
       {

            int j = capsulaIndice % capsulas.Count;

            int posibilidad = Random.Range(0, 3); //25% prob?
            
            if(posibilidad == 2)
            {
                capsulas[j].position = transform.position + new Vector3(0,1,0);
                capsulas[j].gameObject.SetActive(true);
                capsulas[j].GetComponent<Rigidbody>().useGravity = true;

                StartCoroutine("ContadorCapsula",j);
            }
       }

       int i = indice%poolExplosion.Count;

        poolExplosion[i].position = this.transform.position;
        poolExplosion[i].gameObject.SetActive(true);
        
        StartCoroutine("MoverExplosion",i);        
        
        // al cabo de un rato pooling? y delete al enemigo, no pool pal enemigo?
   }

   IEnumerator MoverExplosion(int i)
   {
        this.transform.position = posEnemigos; //no lo puedo matar arruina el resto del codigo, guardarlo en una lista y matarlos luego?
        yield return new WaitForSeconds(2);

        if(transform.GetComponent<Enemigo>()) // si desactivo mientras megaman esta invulnerable queda infinito, cuidado con tiempos
            transform.GetComponent<Enemigo>().enabled = false; // pa que no se muevan, comprobar que no arruine abeja y otros?
       
        poolExplosion[i].transform.position = posPool;
        poolExplosion[i].gameObject.SetActive(false);
        indice ++;        
        yield return null;
   }

    IEnumerator ContadorCapsula(int j)
    {
        yield return new WaitForSeconds(5); //5       
        capsulas[j].transform.position = posPool;
        capsulas[j].gameObject.SetActive(false);
        capsulaIndice ++; 

    }
}
