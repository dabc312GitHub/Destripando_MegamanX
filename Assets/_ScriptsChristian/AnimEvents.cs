using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimEvents : MonoBehaviour
{
   [SerializeField] private Transform explosion;
   [SerializeField] private Transform gunVoltProyectil;
   [SerializeField] private Transform gunVoltEnergia;
   [SerializeField] private List<Transform> salidas;
   [SerializeField] private int velocidadProyectiles;

   public List<Vector3>  getSalidas()
   {
      List<Vector3> salidasPos = new List<Vector3>();
     
      for(int i =0; i<salidas.Count; i++)
      {
         salidasPos.Add(salidas[i].position);         
      }

      return salidasPos;
   }

  
   void Explota()
   {
        Transform exp =  Instantiate(explosion);
        
        // al cabo de un rato delete la instancia y el enemigo?
   }

   public void GunVoltAttackA() // qe reciba parametro segun lo que los raycast digan atacar donde detecte?
   {

        Transform proyectil;

        int index = (int) Random.Range(0, 5); //salidas.Count -1);
       
        bool explota = false;

        if(index == 4 )
             proyectil =  Instantiate(gunVoltEnergia);
        else
        {
            explota = true;
            proyectil =  Instantiate(gunVoltProyectil);
        }
          

        proyectil.position = salidas[index].position;


        moverAtaque(proyectil,explota);
        StartCoroutine(moverAtaque(proyectil, explota));

   }

   IEnumerator moverAtaque(Transform attack, bool explota)
   {
      float tiempo = 5.0f;
      while (tiempo > 0)
      {
         attack.position += Vector3.left * Time.deltaTime * velocidadProyectiles; 
         tiempo -=Time.deltaTime;
         yield return null;
      }
      yield return null; 
   }

/*
   void moverAtaque(Transform attack, bool explota)
   {

   }*/
   
}


/*
   StartCoroutine(CoroutineWithMultipleParameters(1.0F, 2.0F, "foo"));
  }
  IEnumerator CoroutineWithMultipleParameters(float aNum, float bNum, string aWord){
     //stuff

     */