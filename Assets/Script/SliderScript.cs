using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SliderScript : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public Slider mainVideoTimeSlider;

    PopUpHandlerUnion pop;

    bool isSlide = false;

    private void Start()
    {
        pop = FindObjectOfType<PopUpHandlerUnion>();
        mainVideoTimeSlider = GetComponent<Slider>();

    }
   
    private void Update()
    {
        if(pop.nowVideoPlayer == null)
        {
            return;
        }

        if (!isSlide)
        {
            mainVideoTimeSlider.value = (float)pop.nowVideoPlayer.frame / pop.nowVideoPlayer.frameCount;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isSlide = true;
        pop.nowVideoPlayer.Pause();

        Debug.Log(pop.nowVideoPlayer.frameCount);
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float frame = (float)mainVideoTimeSlider.value * pop.nowVideoPlayer.frameCount;

        pop.nowVideoPlayer.frame = (long)frame;

        pop.nowVideoPlayer.Play();
        StartCoroutine(Test((long)frame));
    }

    IEnumerator Test(long num)
    {
        yield return null;

        if(num + 5 >= (long)pop.nowVideoPlayer.frameCount)
        {
            num -= (long)pop.nowVideoPlayer.frameCount - 5;
        }

        if (pop.nowVideoPlayer.frame != num + 5)
        {
            StartCoroutine(Test(num));
        }
        else
        {
            isSlide = false;
        }

    }
}
