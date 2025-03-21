using UnityEngine;
using UnityEngine.Playables;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float factorPos;
    [SerializeField] private float offsetY = 1f;
    [SerializeField] private float offsetX = 1f;

    [SerializeField] private PlayableDirector directorAbeja;
    [SerializeField] private PlayableDirector directorNave;

    private Transform limiteA;
    private Transform limiteB;


    private Vector3 nosePaqueSirve = Vector3.zero;
    private CharacterInputPlayer_Chris characterScript;
    private bool camaraQuieta = false;

    

    void Start()
    {
        characterScript = GameObject.Find("megamanxCompleto").GetComponent<CharacterInputPlayer_Chris>();
        
    }
   

     // Update is called once per frame
    void LateUpdate()
    {

        if ( characterScript.getOrientation() )
            offsetX = -3f;
        else
            offsetX = 2f;
         Vector3 nuevaPos = Vector3.SmoothDamp( transform.position, target.position + new Vector3(offsetX,offsetY,0) , ref nosePaqueSirve , factorPos * Time.deltaTime);// + offset ;

        if(!camaraQuieta)
            transform.position = new Vector3(nuevaPos.x,  nuevaPos.y,transform.position.z);


    }

    public void setCamaraQuieta ( bool quieta) //cuando muere abeja llamar
    {
        camaraQuieta = quieta;
        //no deberia estar en un metodo Set 
        limiteA.parent.gameObject.SetActive(false);

    }

    public void Reanudar()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if(col.name =="CamaraNave")
        {
            directorNave.Play();
            GetComponent<Collider>().enabled = false; //desactivar collider de camara (o deberia ser col de la zona?) para no iniciarlo varias veces,           
        }
        else
        {
            camaraQuieta = true;
            limiteA = col.transform.GetChild(0);
            limiteB = col.transform.GetChild(1);
            limiteA.GetComponent<Collider>().enabled = true; 
            limiteB.GetComponent<Collider>().enabled = true;
            col.transform.GetComponent<Collider>().enabled = false;
            directorAbeja.Play();
        }
        
    }
}
