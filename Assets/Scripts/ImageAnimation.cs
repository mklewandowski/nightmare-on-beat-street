using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    [SerializeField]
    Sprite[] AnimationFrames;

    [SerializeField]
    float frameRate = .067f;
    [SerializeField]
    bool destroyOnLast = false;
    int currFrame = 0;
    float currTime = 0f;

    Image image;

    bool lastReached;

    void Start()
    {
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AnimationFrames.Length > 0)
        {
            currTime += Time.deltaTime;
            currFrame = (int)Mathf.Floor(currTime / frameRate) % AnimationFrames.Length;
            if (destroyOnLast && lastReached && currFrame == 0)
            {
                Destroy(this.gameObject);
                return;
            }
            image.sprite = AnimationFrames[currFrame];
            if (currFrame == AnimationFrames.Length - 1)
                lastReached = true;
        }
    }
}
