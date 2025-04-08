using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SueloCaida : MonoBehaviour
{
    private Transform bloque = null;

    void OnTriggerEnter(Collider obj)
    {   
      
        if( obj.gameObject.tag =="Player" )
        {           

            bloque = transform.parent;
            StartCoroutine("caida");
        }
    }

    public IEnumerator caida()
    {
        bloque.GetChild(1).GetComponent<Collider>().enabled = false;
        bloque.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        bloque.GetChild(0).gameObject.SetActive(false) ;
        bloque.GetComponent<Rigidbody>().useGravity = true;
        bloque.GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(4);
        bloque.GetComponent<Rigidbody>().useGravity = false;

    } 
}
