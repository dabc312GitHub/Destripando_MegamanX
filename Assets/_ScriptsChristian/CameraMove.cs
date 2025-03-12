using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float factorPos;
    [SerializeField] private float offsetY = 1f;
    [SerializeField] private float offsetX = 1f;

    private Vector3 nosePaqueSirve = Vector3.zero;
    private CharacterInputPlayer_Chris characterScript;
    private bool camaraQuieta = false;

    void Start()
    {
        characterScript=GameObject.Find("megamanxCompleto").GetComponent<CharacterInputPlayer_Chris>();
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
    }

    void OnTriggerEnter(Collider col)
    {
        camaraQuieta = true;
        col.transform.GetChild(0).GetComponent<Collider>().enabled = true; //deberia tenerlas en variables guardadas para desactivarlas luego?
        col.transform.GetChild(1).GetComponent<Collider>().enabled = true;
        col.transform.GetComponent<Collider>().enabled = false;
    }
}
