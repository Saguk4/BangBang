using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.IO;
using static AudioManager;

public class GameManager : MonoBehaviour
{
    [Header("Enemy")]
    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public int stageLevel = 0;

    [Header("Player")]
    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Player playerLogic;

    public int kill;

    [Header("GameOver")]
    public GameObject gameOverSet;
    public Text finalScoreL;

    [Header("GameWin")]
    public GameObject gameWinSet;
    public Text finalScoreW;

    public Text Grade;

    [Header("Manager")]
    public ObjectManager objectManager;

    [Header("Boom")]
    public Image[] boomImage;

    [Header("SpawnResource")]
    public List<Spon> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    public bool isEnd = false;



    void Awake()
    {
        isEnd = false;

        Grade.transform.localScale = Vector2.zero;

        spawnList = new List<Spon>();
        enemyObjs = new string[] { "EnemyL", "EnemyM", "EnemyS", "EnemyB" };
        ReadSpawnFile();
       
    }

    void ReadSpawnFile()
    {
        //#1.���� �ʱ�ȭ
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2.������ ���� �б�
        TextAsset textFile = Resources.Load("stage0") as TextAsset; //using system.IO �ʼ�
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            //Debug.Log(line);

            if (line == null)
                break;
            //#3.������ ������ ����
            Spon spawnData = new Spon();
            spawnData.delay = float.Parse(line.Split(",")[0]); //Spilt:������ ���� ���ڷ� ���ڿ��� ������ �Լ�
            spawnData.type = line.Split(",")[1];
            spawnData.point = int.Parse(line.Split(",")[2]);
            spawnList.Add(spawnData);
        }

        //#4.�ؽ�Ʈ ���� �ݱ�
        stringReader.Close();
        //#5.������ ����
        nextSpawnDelay = spawnList[0].delay;

    }

    //############
    void Update()
    {

        //UI update
        scoreText.text = string.Format("{0:n0}", playerLogic.score); // Format( '���� ' , ����) : �ٸ� ���� ������ ��Ʈ������,,,,,"{0 :n0}"  :���ڸ� ������ ' ,'

        //Spawn Enemy
        curSpawnDelay += Time.deltaTime;

        if (kill > 10 + stageLevel * 5)
            stageLevel++;

        if (curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }
    }
    //############

    public void GoMain()
    {
        AudioManager.instance.StoperBgm();
        SceneManager.LoadScene(0);
    }
 
    void SpawnEnemy() //�� ��ȯ
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "L":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "S":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }

        int enemyPoint = spawnList[spawnIndex].point;

        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.objManager = objectManager;
        enemyLogic.gameManager =this;
        


        if (enemyPoint == 5|| enemyPoint == 6) //���� ����
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed,-1);

        }
        else if(enemyPoint == 7|| enemyPoint == 8)//���� �� ����
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed*(-1), -1);
        }
        else //������ ����
        {
            rigid.velocity = new Vector2(0,enemyLogic.speed *(-1));
        }

        //#������ �ε��� ����
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }
        //#���� ������ ������ ����
        nextSpawnDelay = spawnList[spawnIndex].delay;


    }
    
    public void RespawnPlayer() //�÷��̾� ���� �� ��Ȱ
    {
        
        Invoke("RespawnPlayerExe", 2f);
    }
    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);
        Invoke("IsDamagedOn", 0.5f);
        /*Player playerLogic =player.GetComponent<Player>();
        playerLogic.isDamaged = false;*/
    }
    void IsDamagedOn()
    {
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isDamaged = false;
    }

    public void UpdateLifeIcon(int life) //��� �̹��� ���� �Լ�
    {
        //Image => SetActive(false)
        for(int index=0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        // Needed Image => SetActive(true)
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }
    
    public void GameOver()
    {
        Time.timeScale = 0;
        isEnd = true;
        AudioManager.instance.StoperBgm();
        AudioManager.instance.PLayersfx(AudioManager.sfx.Lose);
        player.SetActive(false);

        //#Score
        if (playerLogic.boom != 0)
            playerLogic.score += playerLogic.boom * 500;
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        finalScoreL.text = scoreText.text;

        ShowGrade();
        gameOverSet.SetActive(true);
    }

    public void GameWin()
    {
        Time.timeScale = 0;
        isEnd = true;
        AudioManager.instance.StoperBgm();
        AudioManager.instance.PLayersfx(AudioManager.sfx.Win);
        //#Score
        if (playerLogic.boom != 0)
            playerLogic.score += playerLogic.boom * 500;
        if (playerLogic.life != 0)
            playerLogic.score += playerLogic.life * 1000;
        ShowGrade();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        finalScoreW.text = scoreText.text;
        gameWinSet.SetActive(true);


    }
    public void GameRetry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void UpdateboomIcon(int boom) //�ʻ�� �̹��� ���� �Լ�
    {
        //Image => SetActive(false)
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);
        }

        // Needed Image => SetActive(true)
        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    /*void ReturnSprite()
    {
        
    }
    */
    public void ShowGrade()
    {
        if (playerLogic.score >= 10000)
        {
            Grade.color = Color.red;
            Grade.text = "A";
        }
        else if(playerLogic.score >= 6000)
        {
            Grade.color = Color.blue;
            Grade.text = "B";
        }
        else
        {
            Grade.color = Color.yellow;
            Grade.text = "C";
        }

        Grade.transform.localScale = new Vector2(2,2);
    }
    
}
