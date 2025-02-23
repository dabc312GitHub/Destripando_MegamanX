using UnityEngine;

public class Explotar : MonoBehaviour
{
  
   [SerializeField] private Transform explosion;

   void OnTriggerEnter(Collider Col) 
   {
        //if col es megaman o muro

        Explota();
        //megaman.RecibeDaño
   }

  
   void Explota()
   {
        Transform exp =  Instantiate(explosion);
        exp.position = this.transform.position;

        
        // al cabo de un rato delete la instancia y el enemigo?
   }
}
