using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public int maxHealth;
    public Sprite[] sprites;

    SpriteRenderer spriteRenderer;

    public float bulletSpeed;

    public float maxShortDelay;
    public float curShortDelay;
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject coin;
    public GameObject powerItem;
    public GameObject boomItem;
    
    public GameObject player;
    public ObjectManager objManager;
    public GameManager gameManager;
    


    [Header("Boss")]
    Animator anim;
    public Vector3 firstposition;

    public int patternIndex;
    public int memoryPatternIndex;
    public bool isRandomPattern;
    public int curPatternCount;
    public int[] maxPatternCount;

    float timer;
    public float followPos;
    public Queue<float> parentPos;
    public bool isMove;

    public bool isLive = false;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyName == "B")
        {
            isRandomPattern = false;
            anim = GetComponent<Animator>();
            parentPos = new Queue<float>();
            isMove = true;

        }
        maxHealth = health;
    }

    void OnEnable()
    {
        if (enemyName == "B")
        {
            Invoke("Stop", 2f);
        }
        else
        {
            health = maxHealth;
            health += gameManager.stageLevel * 10;
        }

        /*switch (enemyName)
        {
            
            case "B":
                health = 2000;
                Invoke("Stop", 2);
                break;
            case "L":
                health = 40;
                break;
            case "M":
                health = 20;
                break;
            case "S":
                health = 10;
                break;
        } */
    }

    void Update()
    {

        if (enemyName == "B")
        {
            if (patternIndex == 4)
            {
                Watch();
                Follow();
            }
            if (!isMove)
                RePosition();
            return;
        }
            
       
        
        Fire();
        Reload();
    }

    public void OnHit(int dmg)//피격
    {
        if (health <= 0)
            return;

        health -= dmg;
        if (enemyName == "B")
        {
            if(isLive)
                anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1]; //피격 모션
            Invoke("ReturnSprite", 0.1f); //딜레이
        }
       
        //#아이템 드롭
        if(health <= 0)
        {
            gameManager.kill++;
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;
            AudioManager.instance.PLayersfx(AudioManager.sfx.eDead);
            

            //1.Item is Dropped
            int ran = enemyName == "B"? 0 : Random.Range(0, 10);

            if (ran < 3) //nothing 30%
            {
                Debug.Log("Not Item");
            }
            else if(ran < 6) //Coin 30%
            {
                GameObject itemCoin = objManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;

               
            }
            else if(ran < 9) //Power 30% 
            {
                GameObject itemPower = objManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;

                
            }
            else if(ran <11)// Boom 20%
            {
                GameObject itemBoom = objManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;

              
            }
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity; //회전값 :0
            if (enemyName == "B")
            {
                isLive = false;
                Invoke("GameClear", 4f);
            }
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision) //이탈 방지 함수
    {
        if (collision.gameObject.tag == "BorderEnemy" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }

    void Fire()
    {
        if (curShortDelay < maxShortDelay) //발사 속도 조절
            return;

        if(enemyName == "S")
        {
            GameObject bullet = objManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
                
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.velocity = Vector2.zero;
            Vector3 dirVc = player.transform.position - transform.position;
            rigid.AddForce(dirVc.normalized * bulletSpeed, ForceMode2D.Impulse);

        }
        else if(enemyName == "L")
        {
            GameObject bulletR = objManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
                
            GameObject bulletL = objManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;
           

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            rigidL.velocity = Vector2.zero;
            rigidR.velocity = Vector2.zero;

            Vector3 dirVecR = player.transform.position - (transform.position+ Vector3.right*0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidR.AddForce(dirVecR.normalized * bulletSpeed, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * bulletSpeed, ForceMode2D.Impulse);
        }

        curShortDelay = 0;
    }

    void Reload()
    {
        curShortDelay += Time.deltaTime;
    }

    //#### Boss ####
    void Think()
    {
        if (isRandomPattern)
        {
            patternIndex = Random.Range(0, 4);
            isRandomPattern = false;
        }
        else
        {
            memoryPatternIndex = memoryPatternIndex == (maxPatternCount.Length-1) ? 0 : memoryPatternIndex + 1;
            patternIndex = memoryPatternIndex;
            isRandomPattern = true;
        }

        curPatternCount = 0;

        if (!isLive)
            return;

        switch (patternIndex)
        {
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
            case 4:
                FireLine();
                break;
        }
    }

    void FireForward()
    {
        if (!isLive)
            return;

        //#Fire
        GameObject bulletR = objManager.MakeObj("BulletBossB");
        bulletR.transform.position = transform.position + Vector3.right * 0.45f;
        bulletR.transform.rotation = Quaternion.identity;

        GameObject bulletRR = objManager.MakeObj("BulletBossB");
        bulletRR.transform.position = transform.position + Vector3.right * 0.75f;
        bulletRR.transform.rotation = Quaternion.identity;

        GameObject bulletL = objManager.MakeObj("BulletBossB");
        bulletL.transform.position = transform.position + Vector3.left * 0.45f;
        bulletL.transform.rotation = Quaternion.identity;

        GameObject bulletLL = objManager.MakeObj("BulletBossB");
        bulletLL.transform.position = transform.position + Vector3.left * 0.75f;
        bulletLL.transform.rotation = Quaternion.identity;


        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        rigidR.AddForce(Vector2.down * 15, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 15, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 15, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 15, ForceMode2D.Impulse);


        //#Pattern Count
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireForward", 3f);
        else
            Invoke("Think", 3);
    }

    void FireShot()
    {
        if (!isLive)
            return;

        //#FIre
        for (int index = 0; index < 5; index++) {
            GameObject bullet = objManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVc = player.transform.position - transform.position;
            Vector3 ranVec = new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(0f, 2f));
            dirVc += ranVec;
            rigid.AddForce(dirVc.normalized * 4, ForceMode2D.Impulse);
        }

        //#Pattern Count
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 1f);
        else
            Invoke("Think", 3);
    }

    void FireAround()
    {
        if (!isLive)
            return;

        //#Fire
        int roundNumA = 50;
        int roundNumB = 40;

        int roundNum = curPatternCount%2 ==0 ? roundNumA : roundNumB;

        for (int index = 0; index < roundNum; index++)
        {
            GameObject bullet = objManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVc = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum)
                                      ,Mathf.Sin(Mathf.PI * 2 * index / roundNum));
            rigid.AddForce(dirVc.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward *90;
            bullet.transform.Rotate(rotVec);
        }
        //#Pattern Count
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 1f);
        else
            Invoke("Think", 4.5f);
    }

    void FireArc()
    {
        if (!isLive)
            return;

        //# FIre
        GameObject bullet = objManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector3 dirVc = new Vector2(Mathf.Sin(Mathf.PI*10* curPatternCount / maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVc.normalized * 7, ForceMode2D.Impulse);

        //#Pattern Count
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.1f);
        else
            Invoke("Think", 3);
    }

    void Watch()
    {
        //#Input Pos
        if (!parentPos.Contains(player.transform.position.x))  //Contains() :그 값을 가지고 있는지 확인하는 함수
            parentPos.Enqueue(player.transform.position.x); //부모 위차값을 넣기

        //#Output Pos
        if (parentPos.Count > 9)
            followPos = parentPos.Dequeue();//위치 값 빼기
        else if (parentPos.Count < 9)
            followPos = player.transform.position.x;
    }
    void Follow()
    {
        
        Vector2 nextPosition = new Vector2(followPos, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * 1.5f * Time.deltaTime);

        //transform.position = new Vector2(followPos,transform.position.y);
    }
    void FireLine()
    {
        if (!isLive)
            return;

        //#Check MoveStart
        isMove = true;

        //#Fire
        GameObject bulletR = objManager.MakeObj("BulletEnemyB");
        bulletR.transform.position = transform.position + Vector3.right * 0.1f;
        bulletR.transform.rotation = Quaternion.identity;

        GameObject bulletL = objManager.MakeObj("BulletEnemyB");
        bulletL.transform.position = transform.position + Vector3.left * 0.1f;
        bulletL.transform.rotation = Quaternion.identity;

        GameObject bulletRR = objManager.MakeObj("BulletEnemyB");
        bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
        bulletRR.transform.rotation = Quaternion.identity;

        GameObject bulletLL = objManager.MakeObj("BulletEnemyB");
        bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
        bulletLL.transform.rotation = Quaternion.identity;

        GameObject bulletRRR = objManager.MakeObj("BulletEnemyB");
        bulletRRR.transform.position = transform.position + Vector3.right * 0.55f;
        bulletRRR.transform.rotation = Quaternion.identity;

        GameObject bulletLLL = objManager.MakeObj("BulletEnemyB");
        bulletLLL.transform.position = transform.position + Vector3.left * 0.55f;
        bulletLLL.transform.rotation = Quaternion.identity;

      


        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRRR = bulletRRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLLL = bulletLLL.GetComponent<Rigidbody2D>();
     

       

        rigidR.AddForce(Vector2.down * 3.5f, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 3.5f, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 3.5f, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 3.5f, ForceMode2D.Impulse);
        rigidRRR.AddForce(Vector2.down * 3.5f, ForceMode2D.Impulse);
        rigidLLL.AddForce(Vector2.down * 3.5f, ForceMode2D.Impulse);
    
        

        //#Pattern Count
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireLine", 0.8f);
        else
        {
            isMove = false;
            Invoke("Think", 3);
        }
        
    }

    void Stop()
    {
        if (gameObject.activeSelf)
        {
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.velocity = Vector2.zero;
            firstposition = transform.position;
            health += gameManager.stageLevel * 1000;
            AudioManager.instance.PLayerBgm(AudioManager.bgm.Boss);
            isLive = true;
            Invoke("Think", 2);
        }
    }

    void RePosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, firstposition, speed * 2f * Time.deltaTime);
    }
  
    void GameClear()
    {
        gameManager.GameWin();
    }
}
