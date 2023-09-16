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
      //Queue<> :�迭�� �� ����,, ���� �� �����Ͱ� ���� ������ �ڷ� ���� (FIFO)


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
        if (!parentPos.Contains(parent.position))  //Contains() :�� ���� ������ �ִ��� Ȯ���ϴ� �Լ�
            parentPos.Enqueue(parent.position); //�θ� �������� �ֱ�

        //#Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();//��ġ �� ����
        else if (parentPos.Count < followDelay)
            followPos = parent.position;
    }
   
    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (curShortDelay < maxShortDelay) //�߻� �ӵ� ����
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
