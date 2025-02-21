using UnityEngine;

public class Explotar : MonoBehaviour
{
   [SerializeField] private Transform explosion;

   void Explota()
   {
        Transform pos =  Instantiate(explosion);
        
        // al cabo de un rato delete la instancia y el enemigo?
   }
   
}
