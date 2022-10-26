/*
This script should go on anything that moves.
It handles gravity and friction with the ground.
The ground can be other game objects, including ones that are moving.

To make an object move, manipulate the public variables: rv, v, gravity

*/

using UnityEngine;

public class Kinetic : MonoBehaviour 
{

    // public variables / interface
    public Vector2 rv = Vector2.zero;   // rotational velocity (orientation)
    public Vector3 v = Vector3.zero;    // linear velocity     (position)
    public bool gravity = true;

    private Vector2 rv0 = Vector2.zero; // rotational velocity of ground
    private Vector3 v0 = Vector3.zero;  // linear velocity of ground
    private Vector3 gv = Vector3.zero;  // velocity due to gravity

    // helper variables and functions
    private bool grounded;
    private float dt, t, g = 9.8f, friction = 2.7f; // dt = time between frames. t = time since friction start
    private Vector3 v1; // v1 = velocity when friction starts

    private Rigidbody rb;
    private Collider col;

    private float square(float x) { return x*x; }
    private float min(float x, float y) { return x < y ? x : y; }
    private float max(float x, float y) { return x > y ? x : y; }
    private Vector2 v3tov2(Vector3 v3) { return new Vector2(v3.x, v3.z); }
    private float distance(Vector3 p1, Vector3 p2) { return Mathf.Sqrt(square(p1.x-p2.x)+square(p1.y-p2.y)+square(p1.z-p2.z)); }
    private RigidbodyConstraints constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;


    // getters and setters
    public bool Grounded() { return grounded; }
    public Vector3 Position() { return transform.position; }
    public Vector3 Rotation() { return transform.eulerAngles; }
    public Vector3 Velocity() { return v + gv; }
    public float Friction() { return friction; }
    public void Teleport(float x, float y, float z) { transform.position = new Vector3(x, y, z); }
    public void Rotate(float rho, float theta)
    { 
        transform.eulerAngles = new Vector3(rho, theta, transform.eulerAngles.z);
    }
    public RigidbodyConstraints Constraints() { return constraints; }
    public void setConstraints(RigidbodyConstraints constraints)
    {
        this.constraints = constraints;
        rb.constraints = constraints;
    }

    void Start()
    {
        
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        if(rb == null) rb = gameObject.AddComponent<Rigidbody>() as Rigidbody;
        rb.useGravity = false;
        rb.constraints = constraints;
        
    }

    void FixedUpdate()
    {
        // time since last frame
        dt = Time.deltaTime;

        // is the object touching the ground?
        RaycastHit hit;
        grounded = Physics.Raycast(transform.position, Vector3.down, out hit, col.bounds.extents.y + 0.1f);

        if(!grounded)
        {
            if(gravity)
                gv += g * Vector3.down * dt;
        }
        else
        {
            GameObject ground = hit.transform.gameObject;
            while(ground.transform.parent != null) ground = ground.transform.parent.gameObject;
            Kinetic gk = ground.GetComponent<Kinetic>();
            float f;
            if(gk == null)
            {
                v0 = Vector3.zero;
                rv0 = Vector2.zero;
                f = friction * 2.7f;
            }
            else
            {
                v0 = gk.Velocity();
                rv0 = gk.rv;
                f = friction * gk.Friction();
                // TO DO: adjust v0 for when rv0 is nonzero
                if(rv0.y != 0) {
                    float r = Mathf.Sqrt(square(transform.position.x - ground.transform.position.x)
                                         + square(transform.position.z - ground.transform.position.z));
                    v0 += r * (new Vector3(Mathf.Cos(rv0.y) - 1, 0, Mathf.Sin(rv0.y)));
                }
            }

            Vector3 N = hit.normal;
            Vector3 gdir = Vector3.zero;
            if(N == Vector3.up)
            {
                gv = new Vector3(gv.x, 0, gv.z);
            }
            else
            {
                // gdir points down slope, equal to unit vector of (j x N) x N
                gdir = Vector3.Normalize(new Vector3(N.x*N.y, -N.x*N.x - N.z*N.z, N.y*N.z));
                // magnitude of new gv is magnitude of gv times cosine of angle between gv and gdir, i.e. gv dot gdir
                float gv_mag = Vector3.Dot(gv, gdir);
                gv = gdir * gv_mag;
                // apply gravity to gv if gravity is applied to object
                if(gravity) {
                    float g_mag = Vector3.Dot(g * Vector3.down, gdir) * dt;
                    gv += gdir * g_mag;
                }
            }

            float mu = f * dt / (v - v0 + gv).magnitude;
            Vector3 gf = mu * gv, vf = mu * v;
            if((v - v0 + gv).magnitude == 0 || (gf+vf).magnitude > (v - v0 + gv).magnitude) {
                gv = Vector3.zero;
                v = v0;
            }
            else {
                gv -= gf;
                v -= vf - v0 * mu;
            }
        }

        if(gv.magnitude > 50) gv *= 50/gv.magnitude;
        // add velocities to position and orientation
        transform.position += (gv + v) * dt;
        transform.eulerAngles += new Vector3(rv0.x + rv.x, rv0.y + rv.y, 0) * dt;
    }

}