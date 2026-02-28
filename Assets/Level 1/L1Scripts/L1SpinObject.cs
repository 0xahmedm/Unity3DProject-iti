using UnityEngine;

public class L1SpinObject : MonoBehaviour
{
    public float rototaionspeed=1;


   
    void Update()
    {
        transform.Rotate(0,rototaionspeed,0,Space.World);
    }
}
