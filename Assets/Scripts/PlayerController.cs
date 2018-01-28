using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        inside,
        insideEnemy
    }
    public state playerstate = state.normal;
    state prevState = state.normal;
    SkinnedMeshRenderer rend;
    CapsuleCollider coll;
    public GameObject currentlyInside;
    private GameObject wasInside;
    [SerializeField] GameObject arrow;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rend = transform.Find("Anim").GetComponentInChildren<SkinnedMeshRenderer>();
        coll = transform.Find("Anim").GetComponentInChildren<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MeshColliderToggling()
    {
        if (playerstate != state.inside || playerstate != state.insideEnemy)
        {
            if (coll.enabled == false)
            {
                coll.enabled = true;
            }
            if (rend.enabled == false)
            {
                rend.enabled = true;
            }
        }
        if (playerstate == state.inside || playerstate == state.insideEnemy)
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
        float offsetY;
        if (playerstate == state.inside)
        {
            offsetY = currentlyInside.GetComponentInChildren<Renderer>().bounds.size.y + currentlyInside.transform.position.y;
        }
        else
        {
            offsetY = 3.5f;
        }
        Vector3 arrowPos = arrow.transform.position;
        arrowPos.y = offsetY;
        arrow.transform.position = arrowPos;
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;
        fwd = transform.TransformDirection(Vector3.forward);
        ProcessInput();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeDash();
        }
        DieIfShoneOn();
        MeshColliderToggling();
        rb.angularVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (playerstate == state.inside)
        {
            transform.position = new Vector3(currentlyInside.transform.position.x, transform.position.y, currentlyInside.transform.position.z);
        }
    }

    private void MakeDash()
    {
        GetComponent<Animator>().enabled = false;
        if (playerstate == state.inside)
        {
            transform.position += fwd * 2;
            rb.useGravity = true;
        }
        else if (playerstate == state.insideEnemy)
        {
            if (currentlyInside.GetComponent<EnemyController>().hijacked == true)
            {
                currentlyInside.GetComponent<EnemyController>().hijacked = false;
                wasInside.GetComponent<EnemyController>().attackField.enabled = false;
            }
            transform.position += fwd * 4;
            rb.useGravity = true;
        }
        ChangeState(state.dashing);
        StartCoroutine(Dash());
    }

    void DieIfShoneOn()
    {
        if (playerstate == state.inside)
        {
            if (currentlyInside.GetComponent<HideObjController>().shineOn == true)
            {
                print("DEAD!");
                LevelReset();
                return;
            }
        }
    }

    private void LevelReset()
    {
        transform.position = Vector3.zero;
        playerstate = state.normal;
        SceneManager.LoadScene(2);
    }

    void ChangeState(state newState)
    {
        prevState = playerstate;
        playerstate = newState;
    }

    private void ProcessInput()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Input.GetAxis("Horizontal"));
        if (playerstate == state.normal || playerstate == state.insideEnemy)
        {
            Vector3 goFront = fwd.normalized * speed * Input.GetAxis("Vertical");
            rb.velocity = new Vector3(goFront.x, rb.velocity.y, goFront.z);
        }
    }

    IEnumerator Dash()
    {
        coll.enabled = false;
        coll.enabled = true;
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
            wasInside = null;
            GetComponent<Animator>().enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "HideObj")
        {
            if (playerstate == state.dashing && collision.gameObject != wasInside)
            {
                ChangeState(state.inside);
                rb.useGravity = false;
                transform.position = collision.transform.position;
                currentlyInside = collision.gameObject;
                wasInside = currentlyInside;
                Vector3 newPos = new Vector3(transform.position.x, collision.gameObject.GetComponentInChildren<Collider>().bounds.size.y + collision.gameObject.transform.position.y, transform.position.z);
                transform.position = newPos;
            }
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            if (playerstate == state.dashing && collision.gameObject != wasInside)
            {
                if (wasInside.gameObject.layer == collision.gameObject.GetComponent<EnemyController>().accessColor)
                {
                    ChangeState(state.insideEnemy);
                    rb.useGravity = false;
                    transform.position = collision.transform.position;
                    currentlyInside = collision.gameObject;
                    wasInside = currentlyInside;
                    currentlyInside.GetComponent<EnemyController>().hijacked = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(playerstate != state.inside && playerstate != state.insideEnemy && other.gameObject.tag == "EnemyTarget")
        {
            print("DEAD!");
            LevelReset();
            return;
        }
    }
}
