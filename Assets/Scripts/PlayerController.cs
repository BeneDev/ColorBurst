using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Arrow))]
public class PlayerController : MonoBehaviour {

    Rigidbody rb;
    [SerializeField] float speed = 5;
    [SerializeField] float rotateSpeed = 2;
    [SerializeField] float dashSpeed = 20;
    [SerializeField] float dashDuration = 10;
    Vector3 fwd;
    public enum state 
    {
        normal,
        dashing,
        inside
    }
    public state playerstate = state.normal;
    state prevState = state.normal;
    MeshRenderer rend;
    SphereCollider coll;
    public GameObject currentlyInside;
    [SerializeField] GameObject arrow;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<MeshRenderer>();
        coll = GetComponentInChildren<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MeshColliderToggling()
    {
        if (playerstate != state.inside)
        {
            currentlyInside = null;
            if (coll.enabled == false)
            {
                coll.enabled = true;
            }
            if (rend.enabled == false)
            {
                rend.enabled = true;
            }
        }
        if (playerstate == state.inside)
        {
            coll.enabled = false;
            rend.enabled = false;
            rb.velocity = Vector3.zero;
            ShowDirectionCursor();
        }
        else
        {
            arrow.GetComponentInChildren<Renderer>().enabled = false;
        }
    }

    void ShowDirectionCursor()
    {
        arrow.GetComponentInChildren<Renderer>().enabled = true;
        float offsetY = currentlyInside.GetComponent<Renderer>().bounds.size.y;
        Vector3 arrowPos = arrow.transform.position;
        arrowPos.y = offsetY;
        arrow.transform.position = arrowPos;
        //Instantiate(arrow, transform.position + new Vector3(0, offsetY, 0), transform.rotation);
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;
        fwd = transform.TransformDirection(Vector3.forward);
        ProcessInput();
        coll.enabled = false;
        coll.enabled = true;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(playerstate == state.inside)
            {
                transform.position += fwd * 2;
                rb.useGravity = true;
            }
            ChangeState(state.dashing);
            StartCoroutine(Dash());
        }
        MeshColliderToggling();
        rb.angularVelocity = Vector3.zero;
    }

    void ChangeState(state newState)
    {
        prevState = playerstate;
        playerstate = newState;
    }

    private void ProcessInput()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Input.GetAxis("Horizontal"));
        if (playerstate == state.normal)
        {
            Vector3 goFront = fwd.normalized * speed * Input.GetAxis("Vertical");
            rb.velocity = new Vector3(goFront.x, rb.velocity.y, goFront.z);
        }
    }

    IEnumerator Dash()
    {
        //coll.enabled = false;
        //coll.enabled = true;
        for (int i = 0; i < dashDuration; i++)
        {
            Vector3 goDash = fwd.normalized * dashSpeed;
            //goDash = new Vector3(goDash.x, 0, goDash.z);
            rb.velocity = new Vector3(goDash.x, rb.velocity.y, goDash.z);
            if(rb.velocity.y > 1)
            {
                rb.velocity = new Vector3(rb.velocity.x, -1.7f, rb.velocity.z);
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        if (playerstate == state.dashing)
        {
            ChangeState(state.normal);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "HideObj")
        {
            if (playerstate == state.dashing && prevState == state.normal)
            {
                ChangeState(state.inside);
                rb.useGravity = false;
                transform.position = collision.transform.position;
                currentlyInside = collision.gameObject;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("DEAD!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + fwd*3);
    }
}
