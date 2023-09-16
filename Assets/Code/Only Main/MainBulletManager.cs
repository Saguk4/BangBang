using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBulletManager : MonoBehaviour
{
    public GameObject bulletPlayerAPrefab;


    GameObject[] bulletPlayerA;
    GameObject[] targetPool;

    void Awake()
    {
        bulletPlayerA = new GameObject[100];
  

        Generatr();
    }

    void Generatr()
    {

        //#Bullet
        for (int index = 0; index < bulletPlayerA.Length; index++)
        {
            bulletPlayerA[index] = Instantiate(bulletPlayerAPrefab);
            bulletPlayerA[index].SetActive(false);
        }


    }

    public GameObject Makebullet()
    {
      
        targetPool = bulletPlayerA;
        for (int index = 0; index < targetPool.Length; index++)
        {
            if (!targetPool[index].activeSelf)
            {
                targetPool[index].SetActive(true);
                return targetPool[index];
            }
        }

        return null;
    }

    public GameObject[] GetPoolBullet()
    {
        return bulletPlayerA;
    }
}
