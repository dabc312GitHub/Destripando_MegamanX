using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimEvents : MonoBehaviour
{
   [SerializeField] private Transform explosion;
   [SerializeField] private Transform gunVoltProyectil;
   [SerializeField] private List<Transform> salidas;

  
   void Explota()
   {
        Transform exp =  Instantiate(explosion);
        
        // al cabo de un rato delete la instancia y el enemigo?
   }

   void GunVoltAttackA()
   {

        Transform proyectil =  Instantiate(gunVoltProyectil);
        proyectil.position = salidas[0].position;

   }

   void moverAtaque(Transform attack, bool explota)
   {

   }
   
}
