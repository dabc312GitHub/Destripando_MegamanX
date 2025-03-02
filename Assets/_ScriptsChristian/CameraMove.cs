using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float offset = 1f;
    private Vector3 nosePaqueSirve = Vector3.zero;

   

     // Update is called once per frame
    void LateUpdate()
    {
         Vector3 nuevaPos = Vector3.SmoothDamp( transform.position, target.position + new Vector3(0,offset,0) , ref nosePaqueSirve , speed * Time.deltaTime);// + offset ;

        transform.position = new Vector3(nuevaPos.x,  nuevaPos.y,transform.position.z);


    }
}
