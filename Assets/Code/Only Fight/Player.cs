using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    public int life;
    public int score;
    public float maxShortDelay;
    public float curShortDelay;
    public int maxPower;
    public int power;
    public int boom;
    public int maxBoom;
    public bool isBoomTime;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject BoomEffect;

    public GameManager manager;
    public ObjectManager objManager;
    public bool isDamaged;
    Animator anim;

    public GameObject[] followers;


    void Awake()
    {
        anim = GetComponent<Animator>();
        manager.UpdateboomIcon(boom);

    }

    void Update()
    {
        if (manager.isEnd)
            return;

        Move();
        Fire();
        Boom();
        Reload();
    }

    void Move() //이동
    {
        
        //맵 나가는 거 방지
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;
        //플레이어 이동
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire() //총알 발사
    {
        if (!Input.GetButton("Fire1"))
            return;
        if (curShortDelay < maxShortDelay) //발사 속도 조절
            return;
        AudioManager.instance.PLayersfx(AudioManager.sfx.Shot);
        switch (power){
            case 1:
                //Power One
                GameObject bullet = objManager.MakeObj("BulletPlayerA");
                bullet.transform.position =transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = objManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position +Vector3.right*0.1f;
                
                GameObject bulletL = objManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
               
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
                GameObject bulletRR = objManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.4f;
               
                GameObject bulletLL = objManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.4f; 

                GameObject bulletCC = objManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

        }

        curShortDelay = 0;
    }

    void Reload() // 총알 딜레이 계산
    {
        curShortDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision) // 여러 충돌 관리(아이템 먹기, 적이나 벽과의 충돌)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name) {
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
        else if(collision.gameObject.tag == "EnemyBullet") //라이프 감소
        {
            if (isDamaged)//중복 방지
                return;
            isDamaged = true;
            if (life == 0)
            {
                manager.GameOver();
            }
            else
            { 
                life--;
                AudioManager.instance.PLayersfx(AudioManager.sfx.pDead);
                manager.UpdateLifeIcon(life);
                manager.RespawnPlayer();
                gameObject.SetActive(false);
            }
        }
        else if(collision.gameObject.tag == "Item") 
        {
            Item item =collision.gameObject.GetComponent<Item>();
            AudioManager.instance.PLayersfx(AudioManager.sfx.Item);
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        manager.UpdateboomIcon(boom);
                    }
                    break;
            }
            //먹은 아이템 삭제
            collision.gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision) //벽 충돌 초기화
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

    void OffBoomEffect() //필사기 효과 끄기
    {
        BoomEffect.SetActive(false);
        isBoomTime = false;

        
    }

    void Boom() //필사기
    {
        if (!Input.GetButton("Fire2")) //마우스 오른쪽 클릭
            return;
        if (isBoomTime)
            return;
        if (boom == 0)
            return;
        AudioManager.instance.PLayersfx(AudioManager.sfx.Boom);
        isBoomTime =true;
        boom--;
        manager.UpdateboomIcon(boom);

        //1.Effect Visible
        BoomEffect.SetActive(true);
        Invoke("OffBoomEffect", 3f);

        //2.Remove Enemy
        GameObject[] enemiesL = objManager.GetPool("EnemyL");
        GameObject[] enemiesS = objManager.GetPool("EnemyS");
        GameObject[] enemiesM = objManager.GetPool("EnemyM");

        for (int index = 0; index < enemiesL.Length; index++)
        {
            if (enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }

        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }

        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }

        }

        //3.Remove Enemy Bullet
        GameObject[] bulletsA = objManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objManager.GetPool("BulletEnemyB");
        GameObject[] bossBulletsA = objManager.GetPool("BulletBossA");
        GameObject[] bossBulletsB = objManager.GetPool("BulletBossB");

        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsA[index].activeSelf)
            {
                bulletsA[index].SetActive(false);
            }

        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                bulletsB[index].SetActive(false);
            }

        }
        for (int index = 0; index < bossBulletsA.Length; index++)
        {
            if (bossBulletsA[index].activeSelf)
            {
                bossBulletsA[index].SetActive(false);
            }

        }
        for (int index = 0; index < bossBulletsB.Length; index++)
        {
            if (bossBulletsB[index].activeSelf)
            {
                bossBulletsB[index].SetActive(false);
            }

        }
    }

    void AddFollower() //보조 머신 추가
    {
       if (power == 4)
        followers[0].SetActive(true);

        if (power == 5)
            followers[1].SetActive(true);

        if (power == 6)
            followers[2].SetActive(true);

    }
}
