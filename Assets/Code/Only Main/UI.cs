using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text title;
    public Rigidbody2D top;
    public Color firColor;


    void Awake()
    {
        firColor.a = 255;
    }


    void OnTriggerEnter2D(Collider2D collision) //¿Ã≈ª πÊ¡ˆ «‘ºˆ
    {
        ChangeColor();
    }

    public void ChangeColor()
    {
        title.color = Color.blue;

        Invoke("ReturnColor", 0.1f); //µÙ∑π¿Ã
    }

    void ReturnColor()
    {
        title.color = firColor;
        firColor.a = 255;
 
    }
}
