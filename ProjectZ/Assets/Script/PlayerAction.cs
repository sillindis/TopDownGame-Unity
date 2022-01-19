using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    static public PlayerAction instance;

    public string currentMapName; //현재 플레이어가 있는 맵의 이름(transferMapName)
    public float speed;

    Rigidbody2D rigid;
    Animator anim;
    GameObject scanObject;

    Vector3 dirVec;

    private float h;
    private float v;
    private bool isHorizonMove;
    
    

    void Awake()
    {
        if(instance==null) 
        {
            DontDestroyOnLoad(this.gameObject); //when scean move, Player not destory 
            instance = this;
            rigid = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Move Value
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        //Check Button Down & Up
        bool hDown = Input.GetButtonDown("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vUp = Input.GetButtonUp("Vertical");

        //Check Horizontal Move
        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if (hUp || vUp)
            isHorizonMove = h != 0;

        //Animation
        /*if (anim.GetInteger("hAxisRaw") != h)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        if (anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else
            anim.SetBool("isChange", false);
        */
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

        //Direction
        if (vDown && v == 1)
            dirVec = Vector3.up;
        else if (vDown && v == -1)
            dirVec = Vector3.down;
        else if(hDown && h == -1)
            dirVec = Vector3.left;
        else if (hDown && h == 1)
            dirVec = Vector3.right;

        //Scan object
        if (Input.GetButtonDown("Jump") && scanObject != null)
            Debug.Log("This is: " + scanObject.name);
    }

    private void FixedUpdate()
    {
        //Move
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVec * speed;

        //Ray
        Debug.DrawRay(rigid.position, dirVec*0.7f,new Color(0,1,1));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Object"));

        if (rayHit.collider != null)
        {
            scanObject = rayHit.collider.gameObject;
        }
        else
            scanObject = null;
    }
}
