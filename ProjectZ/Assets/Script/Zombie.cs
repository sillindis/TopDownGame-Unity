using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    Rigidbody2D rigid;
    RaycastHit2D rayHit;
    Vector2 dirVec;
    Animator anim;
    //GameObject scanObject;

    public GameObject player;

    public int speed;

    //private enum enimDir { Up, Down, Left, Right};
    private int h;
    private int v;
    private float idleSpeed = 0.5f;
    private float trackSpeed = 2f;
    private bool onIdle; //Activate Idle()
    private bool isIdle = false; //When Idle() is in progress,

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //player = GetComponent<Player>();
        dirVec = Vector2.down;
        onIdle = true;
    }
    void Start()
    {
        
       // Player playerLogic = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //Animation Direction
        if(dirVec == Vector2.up)
        {
            v = 1;
            h = 0;
        }
        else if (dirVec == Vector2.down)
        {
            v = -1;
            h = 0;
        }
        else if (dirVec == Vector2.left)
        {
            v = 0;
            h = -1;
        }
        else if (dirVec == Vector2.right)
        {
            v = 0;
            h = 1;
        }
        else
        {
            v = 0;
            h = 0;
        }

        //Animation
        if (anim.GetInteger("hAxisRaw") != h)
        {
            anim.SetInteger("hAxisRaw", (int)h);
            anim.SetTrigger("isChange");
        }
        else if (anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetInteger("vAxisRaw", (int)v);
            anim.SetTrigger("isChange");
        }

        //Check player
    }

    void FixedUpdate()
    {
        Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 1));
        rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Border"));
        if (rayHit.collider != null)
        {
            dirVec = -dirVec;
            rigid.velocity = dirVec * idleSpeed;
        }

        if (onIdle == true && isIdle == false)
            Idle();

        //Near Player (0.5f ~ 3f)
        if (Vector2.Distance(player.transform.position, transform.position) < 3f
            && Vector2.Distance(player.transform.position, transform.position) > 0.5f)
        {
            isIdle = false;
            onIdle = false;
            Debug.Log("º¸ÀÎ´Ù");
            TrackPlayer(); //Move to the player.
        }

    }

    private void Stop()
    {
        rigid.velocity = dirVec * 0f; //Zombie stop
        dirVec = new Vector2(0, 0);

        Invoke("Idle", 3);
    }

    private void Idle()
    {
        if (onIdle == false)
            return;

        isIdle = true;

        int ranDir = Random.Range(0, 4);
        
        switch (ranDir)
        {
            case 0: //Up
                dirVec = Vector2.up;
                break;
            case 1: //down
                dirVec = Vector2.down;
                break;
            case 2: //left
                dirVec = Vector2.left;
                break;
            case 3: //right
                dirVec = Vector2.right;
                break;
        }

        rigid.velocity = dirVec * idleSpeed;
        Invoke("Stop", 3);
    }

    private void TrackPlayer()
    {
        //Move to the player.
        float dirx = player.transform.position.x - transform.position.x;
        float diry = player.transform.position.y - transform.position.y;

        //get out of sight Player 
        if (Vector2.Distance(player.transform.position, transform.position) > 5f)
        {
            onIdle = true;
            rigid.velocity = dirVec * 0f;
            //dirVec = new Vector2(0, 0);

            return;
        }

        if (Mathf.Abs(dirx) >= Mathf.Abs(diry))
        {
            if (dirx >= 0)
            {
                dirVec = Vector2.right;
                rigid.velocity = dirVec.normalized * trackSpeed;
            }
            else
            {
                dirVec = Vector2.left;
                rigid.velocity = dirVec.normalized * trackSpeed;
            }
        }
        else
        {
            if (diry >= 0)
            {
                dirVec = Vector2.up;
                rigid.velocity = dirVec.normalized * trackSpeed;
            }
            else
            {
                dirVec = Vector2.down;
                rigid.velocity = dirVec.normalized * trackSpeed;
            }
        }
    }
            
    
}
