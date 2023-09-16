using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    float viewHeight;

    void Awake()
    {
       //#Measure CameraSize
        viewHeight = Camera.main.orthographicSize * 2; //orthographicSize :  orthographic 카메라 크기
    }
    private void Update()
    {
        //#Move BackGround
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed* Time.deltaTime;
        transform.position = curPos+nextPos;

        if (sprites[endIndex].position.y < viewHeight*(-1))
        {
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].transform.localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * 10;

            // Change End/Start Point
            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave-1 ==-1)? sprites.Length-1 : startIndexSave -1;
        }
    }

}
