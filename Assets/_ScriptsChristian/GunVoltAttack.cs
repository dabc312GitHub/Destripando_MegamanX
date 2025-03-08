using UnityEngine;

public class GunVoltAttack : MonoBehaviour
{

    [SerializeField] private Transform GunVolt;

    
    void OnTriggerEnter(Collider other) // trigger es como si "manteniera presionada" una tecla y se queda ahi, por eso cambio a bool
    {
        if(other.tag == "Player")
        {
           GunVolt.GetComponent<Animator>().SetBool("Attack",true);
        }

    }
}
