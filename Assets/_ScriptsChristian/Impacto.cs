using UnityEngine;

public class Impacto : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private Transform quadImpacto;
    private Material controlImpacto;
    private Animator animator;

    void Start()
    {
        if(quadImpacto)
            controlImpacto = quadImpacto.GetComponent<Renderer>().sharedMaterial;
        Reset();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!animator.GetBool("Damage"))
            Reset();
    }

    void Impactar()  // quedaria mejor con corutina?, cambio menos brusco pero afectaria sincronizacion con las transiciones de anims?
    {
        if(mat)
        {
            mat.SetFloat("_Damage",1.0f);
            controlImpacto.SetFloat("_Impacto",1.0f);
        }
    }

    void Reset()
    {
        if(mat)
        {
            mat.SetFloat("_Damage",0.0f);
           controlImpacto.SetFloat("_Impacto",0.0f);
        }
    }
   
}
