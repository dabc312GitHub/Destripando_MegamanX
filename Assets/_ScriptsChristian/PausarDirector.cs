using UnityEngine;
using UnityEngine.Playables;

public class PausarDirector : MonoBehaviour
{
    public void PausarDeVerdad()
    {
        GetComponent<PlayableDirector>().playableGraph.GetRootPlayable(0).SetSpeed(0);
    }
}
