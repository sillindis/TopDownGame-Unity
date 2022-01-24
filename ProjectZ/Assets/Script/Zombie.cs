using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    ///////////////////////////////////
    [SerializeField] private bool m_bDebugMode = false;

    [Header("View Config")]
    [Range(0f, 360f)]
    [SerializeField] private float m_horizontalViewAngle; // �þ߰�
    [SerializeField] private float m_viewRadius; // �þ� ����
    [Range(-180f, 180f)]
    [SerializeField] private float m_viewRotateZ; // �þ߰��� ȸ����
    [SerializeField] private LayerMask m_viewTargetMask;       // �ν� ������ Ÿ���� ����ũ
    [SerializeField] private LayerMask m_viewObstacleMask;     // �ν� ���ع��� ����ũ 
    private List<Collider2D> hitedTargetContainer = new List<Collider2D>(); // �ν��� ��ü���� ������ �����̳�
    private float m_horizontalViewHalfAngle = 0f; // �þ߰��� ���� ��
                                                  ///////////////////////////////////

    Rigidbody2D rigid;
    RaycastHit2D rayHit;
    Vector2 dirVec;
    Animator anim;

    public GameObject player;

    public float idleSpeed;
    public float trackSpeed;


    private int h;
    private int v;
    private bool onIdle; //Activate Idle()
    private bool isIdle = false; //When Idle() is in progress,

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //player = GetComponent<Player>();
        dirVec = Vector2.down;
        onIdle = true;

        ///////////////////////////////////
        m_horizontalViewAngle = 120f;
        m_viewRadius = 3f;
        m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;
        /////////
        /////////////////////////////
    }

    private void OnDrawGizmos()
    {
        if (m_bDebugMode)
        {
            m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position;

            Gizmos.DrawWireSphere(originPos, m_viewRadius);

            Vector3 horizontalRightDir = AngleToDirZ(m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 horizontalLeftDir = AngleToDirZ(-m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 lookDir = AngleToDirZ(m_viewRotateZ);

            Debug.DrawRay(originPos, horizontalLeftDir * m_viewRadius, Color.cyan);
            Debug.DrawRay(originPos, lookDir * m_viewRadius, Color.green);
            Debug.DrawRay(originPos, horizontalRightDir * m_viewRadius, Color.cyan);

            FindViewTargets();
        }
    }

    public Collider2D[] FindViewTargets()
    {
        hitedTargetContainer.Clear();

        /////////
        onIdle = true;
        /////////////

        Vector2 originPos = transform.position;
        Collider2D[] hitedTargets = Physics2D.OverlapCircleAll(originPos, m_viewRadius, m_viewTargetMask);

        foreach (Collider2D hitedTarget in hitedTargets)
        {
            Vector2 targetPos = hitedTarget.transform.position;
            Vector2 dir = (targetPos - originPos).normalized;
            Vector2 lookDir = AngleToDirZ(m_viewRotateZ);

            // float angle = Vector3.Angle(lookDir, dir)
            // �Ʒ� �� ���� ���� �ڵ�� �����ϰ� ������. ���� ������ ����
            float dot = Vector2.Dot(lookDir, dir);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angle <= m_horizontalViewHalfAngle)
            {
                RaycastHit2D rayHitedTarget = Physics2D.Raycast(originPos, dir, m_viewRadius, m_viewObstacleMask);
                if (rayHitedTarget)
                {
                    if (m_bDebugMode)
                        Debug.DrawLine(originPos, rayHitedTarget.point, Color.yellow);
                }
                else //�� �߰߱�
                {
                    hitedTargetContainer.Add(hitedTarget);

                    if (m_bDebugMode)
                        Debug.DrawLine(originPos, targetPos, Color.red);

                    //�þ� �о���
                    m_horizontalViewAngle = 180f;
                    m_viewRadius = 5f;
                    onIdle = false;

                    ///////////////////////////////////
                    Debug.Log("�� �߰�");

                    float dirx = player.transform.position.x - transform.position.x;
                    float diry = player.transform.position.y - transform.position.y;

                    if (Mathf.Abs(dirx) < 0.5f || Mathf.Abs(diry) < 0.5f)
                        rigid.velocity = dirVec.normalized * 0f;

                    if (Mathf.Abs(dirx) >= Mathf.Abs(diry))
                    {
                        if (dirx >= 0)
                        {
                            m_viewRotateZ = 90;
                            dirVec = Vector2.right;
                            rigid.velocity = dirVec.normalized * trackSpeed;
                        }
                        else
                        {
                            m_viewRotateZ = -90;
                            dirVec = Vector2.left;
                            rigid.velocity = dirVec.normalized * trackSpeed;
                        }
                    }
                    else
                    {
                        if (diry >= 0)
                        {
                            m_viewRotateZ = 0;
                            dirVec = Vector2.up;
                            rigid.velocity = dirVec.normalized * trackSpeed;
                        }
                        else
                        {
                            m_viewRotateZ = 180;
                            dirVec = Vector2.down;
                            rigid.velocity = dirVec.normalized * trackSpeed;
                        }
                    }
                    ///////////////////////////////////
                }
            }

        }

        if (hitedTargetContainer.Count > 0)
            return hitedTargetContainer.ToArray();
        else
            return null;
    }

    // -180~180�� ���� Up Vector ���� Local Direction���� ��ȯ������.
    private Vector2 AngleToDirZ(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian));
    }


    // Update is called once per frame
    void Update()
    {
        //Animation Direction
        if (dirVec == Vector2.up)
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
            m_viewRotateZ = -m_viewRotateZ;
            rigid.velocity = dirVec * idleSpeed;
        }

        if (onIdle == true && isIdle == false)
            Idle();
    }

    private void Stop()
    {

        Debug.Log("Stop ��");


        rigid.velocity = dirVec * 0f; //Zombie stop
        dirVec = new Vector2(0, 0);

        Invoke("Idle", 3);
    }

    private void Idle()
    {
        if (onIdle == false)
            return;

        Debug.Log("Idle ��");

        m_horizontalViewAngle = 120f;
        m_viewRadius = 3f;

        isIdle = true;

        int ranDir = Random.Range(0, 4);

        switch (ranDir)
        {
            case 0: //Up
                m_viewRotateZ = 0;
                dirVec = Vector2.up;
                break;
            case 1: //down
                m_viewRotateZ = 180;
                dirVec = Vector2.down;
                break;
            case 2: //left
                m_viewRotateZ = -90;
                dirVec = Vector2.left;
                break;
            case 3: //right
                m_viewRotateZ = 90;
                dirVec = Vector2.right;
                break;
        }

        rigid.velocity = dirVec * idleSpeed;
        Invoke("Stop", 3);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("��Ҵ�");
            dirVec = new Vector2(0, 0);
            rigid.velocity = dirVec * 0;
            rigid.mass = 100;
            rigid.drag = 1000;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("����ִ���");
            dirVec = new Vector2(0, 0);
            rigid.velocity = dirVec * 0;
            rigid.mass = 100;
            rigid.drag = 10000;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("������");
            rigid.mass = 1;
            rigid.drag = 0;
        }
    }

}
