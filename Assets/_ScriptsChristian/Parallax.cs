using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform backgroundObject;
    [SerializeField] private Transform Camera;
   [SerializeField]  private int cantidad =10;

    private Material mat;
    private float posX; 

    void Start()
    {
         mat = backgroundObject.GetComponent<Renderer>().sharedMaterial;
    }
    
    void Update()
    {
        posX = Camera.position.x / cantidad;       
        mat.SetVector("_Offset",new Vector4(posX, 0,0,0));        
    }
}
