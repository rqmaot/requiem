using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    
    private Kinetic kinetic;

    void Start()
    {
        kinetic = GetComponent<Kinetic>();
        if(kinetic == null) kinetic = gameObject.AddComponent<Kinetic>() as Kinetic;
        //kinetic.gravity = false;
        //kinetic.v = new Vector3(0,0,-1);
        kinetic.setConstraints(RigidbodyConstraints.FreezeRotation);
    }
    
    void OnCollisionEnter()
    {
        Debug.Log("hit!");
        //kinetic.gravity = false;
        //kinetic.v = new Vector3(kinetic.v.x, 0, kinetic.v.z);
    }

    
    void Update()
    {
        //Debug.Log(kinetic.Grounded().ToString());
    }
}
