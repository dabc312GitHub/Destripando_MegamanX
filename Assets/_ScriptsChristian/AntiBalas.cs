using UnityEngine;

public class AntiBalas : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Proyectil"  || other.tag == "Plasma"  || other.tag == "MegamanAtaqueA" || other.tag == "MegamanAtaqueB" || other.tag == "MegamanAtaqueC")
        {
            Destroy(other.gameObject);
        }
        
    }
}
