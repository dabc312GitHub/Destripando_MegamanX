using UnityEngine;

public class Impacto : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private Transform quadImpacto;
    [SerializeField] private Transform muerte;
    [SerializeField] private Material vidaUI;
    [SerializeField] private Material megamanMatA;
    [SerializeField] private Material megamanMatB;
    private Material controlImpacto;
    private Animator animator;
    private float vida = 10f;

    void Start()
    {
        if(quadImpacto)
            controlImpacto = quadImpacto.GetComponent<Renderer>().sharedMaterial;
        Reset();
        animator = GetComponent<Animator>();
        vidaUI.SetFloat("_Salud", 1.0f);
    }

    public Material getMaterialA()
    {
        return megamanMatA;
    }

    public Material getMaterialB()
    {
        return megamanMatB;
    }
/*
    void Update()
    {
        if (!animator.GetBool("Damage"))
            Reset();
    }*/

    void Impactar()  // quedaria mejor con corutina?, cambio menos brusco pero afectaria sincronizacion con las transiciones de anims?
    {
        if(mat)
        {
            mat.SetFloat("_Damage",1.0f);
            controlImpacto.SetFloat("_Impacto",1.0f);
            megamanMatA.SetFloat("_Invulnerable",1.0f);
            megamanMatB.SetFloat("_Invulnerable",1.0f);


            //temporal
            vida -= 3; 
        if( vida <= 0)
            Muerte();                  
            
        vidaUI.SetFloat("_Salud", vida/10f);
        // temporal

        }
    }

    void Damage (float d)
    {

        vida -= d; 
        if( vida <= 0)
            Muerte();                  
            
        vidaUI.SetFloat("_Salud", vida/10f);

    }


    void Muerte()
    {
        muerte.gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(5).gameObject.SetActive(false); // collider ancho...
        vidaUI.SetFloat("_Salud", 0.0f);
        // UI game Over, en realidad pantalla se pone en blanco gradualmente y si fueron 3 veces sale ventana de password
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
