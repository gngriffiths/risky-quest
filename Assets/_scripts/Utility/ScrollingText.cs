using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ScrollingText : MonoBehaviour
{
    public Transform scrollingTextParent;
    public float timer, timeToClearMessage = 10.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ( timer > 0 )
        {
            timer -= Time.deltaTime;
            if ( timer <= 0 ) { NewLine( "" ); }
        }

    }

    public void NewLine(string newline)
    {
        int count = scrollingTextParent.childCount - 1;
        while ( count > 0 )
        {
            scrollingTextParent.GetChild( count ).GetComponent<TextMeshPro>().text = scrollingTextParent.GetChild( count - 1 ).GetComponent<TextMeshPro>().text;

            count--;
        }
        scrollingTextParent.GetChild( 0 ).GetComponent<TextMeshPro>().text = newline;


        timer = timeToClearMessage;
    }
}
