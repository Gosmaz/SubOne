using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct Videos
{
    public VideoPlayer[] video1;
    public VideoPlayer[] video2;
    public VideoPlayer[] video3;
    public VideoPlayer[] video4;
    public VideoPlayer[] video5;
    public VideoPlayer[] video6;
}

public struct Screen
{
    public GameObject[] screen1;
    public GameObject[] screen2;
    public GameObject[] screen3;
    public GameObject[] screen4;
    public GameObject[] screen5;
    public GameObject[] screen6;
}

public class PopUpHandller1 : MonoBehaviour
{
    public List<GameObject> popUpScreen = new List<GameObject>();
    public RectTransform sideBar;
    public GameObject VideoControlButton;
    public GameObject bgAlpha;

    public GameObject[] bigScreen;

    public List<Screen> screens = new List<Screen>();
    public List<Videos> videoPlayer = new List<Videos>();

    public Image sideButton;

    Vector3 waitPos;

    GameObject nowPopUp = null;
    GameObject beforePopUp = null;

    Sprite[] sideImages;
    Sprite[] upSideImages;

    Image clickingButton;

    bool moving = false;

    int caseNum = 0;

    private void Start()
    {
        waitPos = popUpScreen[0].transform.localPosition;

        for(int i = 0; i<popUpScreen.Count; i++)
        {
            popUpScreen[i].transform.localPosition = waitPos;
        }

        sideImages = Resources.LoadAll<Sprite>("SideButton");
        upSideImages = Resources.LoadAll<Sprite>("UPSideButton");

        for(int i = 0; i < videoPlayer.Count; i++)
        {
            for(int j = 0; i < videoPlayer[i].video1.Length; i++)
            {
                videoPlayer[i].video1[j].Stop();
                videoPlayer[i].video2[j].Stop();
                videoPlayer[i].video3[j].Stop();
                videoPlayer[i].video4[j].Stop();
                videoPlayer[i].video5[j].Stop();
                videoPlayer[i].video6[j].Stop();
            }
        }

        videoPlayer[0].video1[0].Play();
    }

    public void Click(int popNum)
    {
        if (moving)
        {
            return;
        }

        clickingButton = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();

        clickingButton.DOFade(0, 0.25f).OnComplete(()=> PopUp(popNum));
    }

    public void PopUp(int popNum)
    {
        if (moving)
        {
            return;
        }

        moving = true;

        VideoControlButton.SetActive(true);
        bgAlpha.SetActive(true);

        nowPopUp = popUpScreen[caseNum * 6 + popNum - 1];

        if (beforePopUp != null && beforePopUp.transform.localPosition != waitPos)
        {
            beforePopUp.transform.DOLocalMoveY(-1200, 0.3f, false).OnComplete(()=> ReloadPopUp());
        }

        beforePopUp = nowPopUp;

        popUpScreen[caseNum * 6 + popNum -1].transform.DOLocalMoveY(0, 0.7f, false).SetEase(Ease.OutBack).OnComplete(() => moving = false);

        sideButton.sprite = sideImages[caseNum * 6 + popNum - 1];
        // bigButton.sprite = upSideImages[caseNum * 6 + popNum -1];

        sideBar.DOLocalMoveX(0, 0.7f, false).SetEase(Ease.Unset);

    }


    private void ReloadPopUp()
    {
        for(int i = 0; i < popUpScreen.Count; i++)
        {
            if (popUpScreen[i] == nowPopUp)
            {
                continue;
            }
            popUpScreen[i].transform.localPosition = waitPos;
        }
    }

    public void VideoStart(VideoPlayer video)
    {
        video.Play();
    }

    public void VideoPause(VideoPlayer video)
    {
        video.Pause();
    }

    public void VideoReset(VideoPlayer video)
    {
        video.time = 0;
    }

    public void Home()
    {
        moving = true;
        nowPopUp.transform.DOLocalMoveY(-1200, 0.9f, false).OnComplete(() => { nowPopUp = null; ReloadPopUp(); moving = false; });
        VideoControlButton.SetActive(false);
        sideBar.DOLocalMoveX(305, 0.7f, false).SetEase(Ease.Unset);
        clickingButton.DOFade(170/255f, 0);
        bgAlpha.SetActive(false);
    }
}
