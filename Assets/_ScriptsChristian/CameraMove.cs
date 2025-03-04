using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float factorPos;
    [SerializeField] private float offsetY = 1f;
    [SerializeField] private float offsetX = 1f;

    private Vector3 nosePaqueSirve = Vector3.zero;
    private CharacterInputPlayer_Chris characterScript;

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

        transform.position = new Vector3(nuevaPos.x,  nuevaPos.y,transform.position.z);


    }
}
