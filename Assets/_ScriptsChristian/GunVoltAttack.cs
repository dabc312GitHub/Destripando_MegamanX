using UnityEngine;

public class GunVoltAttack : MonoBehaviour
{

    [SerializeField] private Transform GunVolt;

    
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            GunVolt.GetComponent<Animator>().SetTrigger("Attack");

    }
}
