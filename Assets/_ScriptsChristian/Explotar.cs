using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explotar : MonoBehaviour
{
  
   [SerializeField] private Transform explosion;
   [SerializeField] private float segundosInvencible = 4.0f;
   private static bool invulnerable = false;
   private Material matA, matB;

   private Impacto ImpactoScript;

   void Start()
   {
        ImpactoScript = GameObject.Find("megamanxCompleto").GetComponent<Impacto>();
        matA = ImpactoScript.getMaterialA();
        matB = ImpactoScript.getMaterialB();
   }

   void OnTriggerEnter(Collider Col) 
   {
       if(Col.tag == "Player")
        {
            if(!invulnerable)
            {
                Explota();
                Col.transform.parent.GetComponent<Animator>().SetTrigger("Damage");
                invulnerable = true;
                matA.SetFloat("_Invulnerable",1.0f);
                matB.SetFloat("_Invulnerable",1.0f);
                StartCoroutine("Invulnerable");
                //Col.transform.parent.GetComponent<Animator>().SetBool("Damage", false); 
            }
            
            
        }

        else
            Explota();
                
   }

   public IEnumerator Invulnerable()
   {   
        yield return new WaitForSeconds(segundosInvencible);
        invulnerable = false;
        matA.SetFloat("_Invulnerable",0.0f);
        matB.SetFloat("_Invulnerable",0.0f);         
        yield return null;
   }

  
   void Explota()
   {
        Transform exp =  Instantiate(explosion);
        exp.position = this.transform.position;

        
        // al cabo de un rato delete la instancia y el enemigo?
   }
}
