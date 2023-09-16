using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    public float speed;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    public float maxShortDelay;
    public float curShortDelay;



    public bool isBoomTime;

    public GameObject bulletObjA;
    public GameObject BoomEffect;

    public MainGameManager manager;
    public MainBulletManager bulletManager;
    Animator anim;

   


    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    void Move() //�̵�
    {

        //�� ������ �� ����
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;
        //�÷��̾� �̵�
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire() //�Ѿ� �߻�
    {
        if (!Input.GetButton("Fire1"))
            return;
        if (curShortDelay < maxShortDelay) //�߻� �ӵ� ����
            return;
        MainAudioManager.instance.PLayersfx(MainAudioManager.sfx.Shot);
   
        GameObject bullet = bulletManager.Makebullet();
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
           
        curShortDelay = 0;
    }

    void Reload() // �Ѿ� ������ ���
    {
        curShortDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision) // �浹 ����(������ �浹)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D collision) //�� �浹 �ʱ�ȭ
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }

    void OffBoomEffect() //�ʻ�� ȿ�� ����
    {
        BoomEffect.SetActive(false);
        isBoomTime = false;


    }

    void Boom() //�ʻ��
    {
        if (!Input.GetButton("Fire2")) //���콺 ������ Ŭ��
            return;
        if (isBoomTime)
            return;
        AudioManager.instance.PLayersfx(AudioManager.sfx.Boom);
        isBoomTime = true;

        //1.Effect Visible
        BoomEffect.SetActive(true);
        Invoke("OffBoomEffect", 3f);

    }


}
