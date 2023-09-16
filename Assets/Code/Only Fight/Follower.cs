using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShortDelay;
    public float curShortDelay;
    public ObjectManager objManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;
      //Queue<> :배열의 한 종류,, 먼저 들어간 데이터가 먼저 나오는 자료 구조 (FIFO)


    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }

    void Watch()
    {
        //#Input Pos
        if (!parentPos.Contains(parent.position))  //Contains() :그 값을 가지고 있는지 확인하는 함수
            parentPos.Enqueue(parent.position); //부모 위차값을 넣기

        //#Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();//위치 값 빼기
        else if (parentPos.Count < followDelay)
            followPos = parent.position;
    }
   
    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (curShortDelay < maxShortDelay) //발사 속도 조절
            return;

        GameObject bullet = objManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
       

        curShortDelay = 0;
    }

    void Reload()
    {
        curShortDelay += Time.deltaTime;
    }
}
