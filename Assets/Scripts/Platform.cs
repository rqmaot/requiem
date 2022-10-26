using UnityEngine;

class Platform : MonoBehaviour {
    private Kinetic kinetic;
    void Start() {
        kinetic = GetComponent<Kinetic>();
        if(kinetic == null) kinetic = gameObject.AddComponent<Kinetic>() as Kinetic;
        kinetic.gravity = false;
        kinetic.rv = new Vector3(0, 100, 0);
    }

    void Update() {

    }
}