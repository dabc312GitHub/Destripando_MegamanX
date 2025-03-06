using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Impacto : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private Transform quadImpacto;
    [SerializeField] private Transform muerte;
    [SerializeField] private Material vidaUI;
    [SerializeField] private Material megamanMatA;
    [SerializeField] private Material megamanMatB;
    [SerializeField] private float segInvencible = 2.0f;
    [SerializeField] private Material FullScreenMat;
    private Material controlImpacto;
    private Animator animator;
    private Collider extension;
    private float vida = 20f;
    private float vidaMaxima;

    void Start()
    {
        FullScreenMat.SetFloat("_EfectoBlanco",0);
        if(quadImpacto)
            controlImpacto = quadImpacto.GetComponent<Renderer>().sharedMaterial;
        Reset();
        animator = GetComponent<Animator>();
        vidaUI.SetFloat("_Salud", 1.0f);
        extension = transform.GetChild(4).GetComponent<Collider>();

        megamanMatA.SetFloat("_Invulnerable",0.0f); // y aqui creo qe no se malogra?
        megamanMatB.SetFloat("_Invulnerable",0.0f);
        vidaMaxima = vida;

    }

    public Collider getExtension()
    {
        return extension;
    }

    public float getSegInvencible()
    {
        return segInvencible;
    }

    public Material getMaterialA()
    {
        return megamanMatA;
    }

    public Material getMaterialB()
    {
        return megamanMatB;
    }

     public bool getVivo()
     {
        return vida<=0;
     }

   
    void Impactar()  // quedaria mejor con corutina?, cambio menos brusco pero afectaria sincronizacion con las transiciones de anims?
    {
        if(mat)
        {
            mat.SetFloat("_Damage",1.0f);
            controlImpacto.SetFloat("_Impacto",1.0f);
            //megamanMatA.SetFloat("_Invulnerable",1.0f);
            //megamanMatB.SetFloat("_Invulnerable",1.0f);


         //temporal
            vida -= 3; 
            vidaUI.SetFloat("_Salud", vida/vidaMaxima);
        if( vida <= 0)
            Muerte(); 
        // temporal

        }
    }

    void Damage (float d)
    {
        Debug.Log(vida);

        vida -= d; 
        if( vida <= 0)
            Muerte();                  
        else
            vidaUI.SetFloat("_Salud", vida/vidaMaxima);
         Debug.Log(vida);

    }


    void Muerte()
    {
        vida =0;
        vidaUI.SetFloat("_Salud", 0.0f);
        muerte.gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false); // collider ancho...
        
        // UI game Over, en realidad pantalla se pone en blanco gradualmente y si fueron 3 veces sale ventana de password
        StartCoroutine("Pantalla");
        
    }

    void Reset()
    {
        if(mat)
        {
           mat.SetFloat("_Damage",0.0f);
           controlImpacto.SetFloat("_Impacto",0.0f);
           megamanMatB.SetFloat("_Carga",0.0f);            
        }
    }

    public IEnumerator Pantalla()
    {
        float tiempo = 0;
        while(tiempo <= 3 )
        {
            tiempo += Time.deltaTime;
            FullScreenMat.SetFloat("_EfectoBlanco",tiempo/3);
             yield return null;
        }
        yield return null;
        
    }



    void OnTriggerEnter(Collider other) // a veces no entra por el character controller que es raro?
    {
        
        if(other.gameObject.CompareTag("ZonaMuerte"))
        {
            Muerte();
        }

        if(other.gameObject.CompareTag("En_Spiky"))
        {
            float damage = other.transform.GetComponent<Enemigo>().getAtaqueCol();
            Damage(damage); // daño en megaman debe interrumpir su movimiento brevemente
        }
               
    }
   
}
