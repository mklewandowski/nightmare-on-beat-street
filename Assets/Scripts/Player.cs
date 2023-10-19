using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    Sprite[] IdleFrames;

    [SerializeField]
    Sprite[] DanceFrames;

    [SerializeField]
    float frameRate = .3f;

    int currFrame = 0;
    float currTime = 0f;

    bool isIdle = true;

    Image image;

    void Start()
    {
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > frameRate)
        {
            currTime = 0;
            if (isIdle)
            {
                currFrame++;
                if (currFrame >= IdleFrames.Length)
                    currFrame = 0;
                image.sprite = IdleFrames[currFrame];
            }
            else
            {
                currFrame = Random.Range(0, DanceFrames.Length);
                image.sprite = DanceFrames[currFrame];
            }
        }
    }

    public void SetIdle ()
    {
        isIdle = true;
    }

    public void SetDance()
    {
        isIdle = false;
    }
}
