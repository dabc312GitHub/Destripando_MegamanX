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

   void Start()
   {
        ImpactoScript = GameObject.Find("megamanxCompleto").GetComponent<Impacto>();
        matA = ImpactoScript.getMaterialA();
        matB = ImpactoScript.getMaterialB();
        //matA.SetFloat("_Invulnerable",1.0f); // si se utiliza aqui se malogra???
        //matB.SetFloat("_Invulnerable",1.0f);
        segundosInvencible = ImpactoScript.getSegInvencible();


        posPool = new Vector3 (0,-20,0);
        posEnemigos = new Vector3 (4,-20,0);
        indice = 0;
        Transform exp;
        
        if(poolExplosion.Count == 0)
        {
             for( int i =0; i< explosionesMax ; i++)
            {
                exp =  Instantiate(explosion);
                exp.gameObject.SetActive(false);
                exp.position = posPool;
                poolExplosion.Add(exp);
            }
        }
       


   }


   void OnTriggerEnter(Collider Col) 
   {
     
       if(Col.tag == "Player") // si proyectil toca player
        {
            if(!invulnerable)
            {
                if( gameObject.CompareTag("Proyectil")) //plasma se comporta como proyectil pero no explota, ver eso luego
                {
                    Explota();// solo explota si es proyectil
                    // y desparece
                }
                Col.transform.parent.GetComponent<Animator>().SetTrigger("Damage");
                invulnerable = true;
                ImpactoScript.getExtension().enabled = false;   
                matA.SetFloat("_Invulnerable",1.0f);
                matB.SetFloat("_Invulnerable",1.0f);
                StartCoroutine("Invulnerable");
                //Col.transform.parent.GetComponent<Animator>().SetBool("Damage", false); 
            }        
            
        }
        else if(gameObject.tag == "Proyectil") // si choco pared sinedo proyectil 
        {
            Explota();
        }            
                
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
        //Transform exp =  Instantiate(explosion);
        //exp.position = this.transform.position;

        int i = indice%poolExplosion.Count;

        poolExplosion[i].position = this.transform.position;
        poolExplosion[i].gameObject.SetActive(true);
        
        StartCoroutine("MoverExplosion",i);        
        
        // al cabo de un rato pooling? y delete al enemigo, no pool pal enemigo?
   }

   IEnumerator MoverExplosion(int i)
   {
        this.transform. position = posEnemigos;
        yield return new WaitForSeconds(2);
        poolExplosion[i].transform.position = posPool;
        poolExplosion[i].gameObject.SetActive(false);
        indice ++;
        yield return null;
   }
}
