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
    bool isLooping = false;

    float frame;

    IEnumerator valueCheck;

    private void Start()
    {
        pop = FindObjectOfType<PopUpHandlerUnion>();
        mainVideoTimeSlider = GetComponent<Slider>();

        valueCheck = Test((long)frame);
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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        frame = Mathf.Round(mainVideoTimeSlider.value * 100) * 0.01f * pop.nowVideoPlayer.frameCount;

        pop.nowVideoPlayer.frame = (long)frame;

        pop.nowVideoPlayer.Play();

        if (isLooping)
        {
            StopCoroutine(valueCheck);
        }
        valueCheck = Test((long)frame);
        StartCoroutine(valueCheck);
    }

    IEnumerator Test(long num)
    {
        isLooping = true;

        yield return null;

        if(num + 3 >= (long)pop.nowVideoPlayer.frameCount)
        {
            num -= (long)pop.nowVideoPlayer.frameCount - 3;
        }

        while (pop.nowVideoPlayer.frame != num + 3)
        {
            yield return null;
        }

        isSlide = false;
        isLooping = false;

    }
}
