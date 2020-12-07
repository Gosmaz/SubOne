using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;
using UnityEngine.UI;

public class PopUpHandller : MonoBehaviour
{
    public GameObject mainPlayGround;
    public GameObject subPlayGround;
    public RectTransform sideBar;
    public GameObject bgAlpha;
    public GameObject videoControlBottons;

    public List<VideoPlayer> case1;
    public List<VideoPlayer> case2;
    public List<VideoPlayer> case3;

    public Image sideButton;
    public Image bigButton;

    public VideoPlayer subVideo;
    public VideoPlayer mainVideo;

    public RawImage monitor;

    public Texture main;
    public Texture sub;
    public Texture wait;
    public Texture baseMonitor;


    bool isMoving;
    bool isMain = false;

    Vector3 ogPos;
    Vector3 sidePos;

    Sprite[] sideImages;
    Sprite[] upSideImages;

    VideoPlayer nowMain;
    VideoPlayer nowSub;

    private void Start()
    {
        ogPos = subPlayGround.transform.localPosition;
        sidePos = sideBar.transform.localPosition;

        mainPlayGround.transform.localPosition = ogPos;
        subPlayGround.transform.localPosition = ogPos;

        monitor.texture = baseMonitor;

        sideImages = Resources.LoadAll<Sprite>("SideButton");
        upSideImages = Resources.LoadAll<Sprite>("UPSideButton");
    }
    public void Click(int clip)
    {
        if (isMoving)
        {
            return;
        }

        isMoving = true;

        if (!isMain)
        {
            isMain = !isMain;

            Video_Change(clip, false);

            monitor.texture = sub;

            if (mainPlayGround.transform.localPosition != ogPos)
                mainPlayGround.transform.DOLocalMoveY(-1200, 0.9f, false).SetEase(Ease.OutBack).OnComplete(() => mainPlayGround.transform.localPosition = ogPos);

            subPlayGround.transform.DOLocalMoveY(0, 1, false).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    
                    isMoving = false;
                    
                }).OnStart(()=>
                {
                    if (sideBar.transform.localPosition.x != 0)
                    {
                        sideBar.transform.localPosition = sidePos;
                        sideBar.DOLocalMoveX(0, 1, false).SetEase(Ease.Unset).OnComplete(() => isMoving = false);
                    }
                });
        }
        else
        {
            isMain = !isMain;

            Video_Change(clip, true);

            monitor.texture = main;

            if (subPlayGround.transform.localPosition != ogPos)
                subPlayGround.transform.DOLocalMoveY(-1200, 0.9f, false).SetEase(Ease.OutBack).OnComplete(() => subPlayGround.transform.localPosition = ogPos);

            mainPlayGround.transform.DOLocalMoveY(0, 0.7f, false).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    isMoving = false;
                }).OnStart(() =>
                {
                    if (sideBar.transform.localPosition.x != 0)
                    {
                        sideBar.transform.localPosition = sidePos;
                        sideBar.DOLocalMoveX(0, 0.7f, false).SetEase(Ease.Unset).OnComplete(() => isMoving = false);
                    }
                });
        }

        bgAlpha.SetActive(isMoving);
        videoControlBottons.SetActive(isMoving);

    }

    public void Home()
    {
        if (!isMain)
        {
            mainPlayGround.transform.DOLocalMoveY(-1000, 0.5f, false)
                .OnComplete(() =>
                {
                    mainPlayGround.transform.localPosition = ogPos;
                });
        }
        else
        {
            subPlayGround.transform.DOLocalMoveY(-1000, 0.5f, false)
               .OnComplete(() =>
               {
                   subPlayGround.transform.localPosition = ogPos;
               });
        }
        monitor.texture = baseMonitor;
        bgAlpha.SetActive(false);
        sideBar.DOLocalMoveX(305, 0.5f, false).SetEase(Ease.Unset).OnComplete(() => {
            isMoving = false;
            sideBar.transform.localPosition = sidePos;
        }) ;
        
    }

    private void Video_Change(int code, bool videoMain)
    {

        if (videoMain)
        {
            
            switch (code / 10)
            {
                case 1:
                    case1[(code % 10) - 1].targetTexture = (RenderTexture)main;
                    nowMain = case1[(code % 10) - 1];
                    break;
                case 2:
                    case2[(code % 10) - 1].targetTexture = (RenderTexture)main;
                    nowMain = case2[(code % 10) - 1];
                    break;
                case 3:
                    case3[(code % 10) - 1].targetTexture = (RenderTexture)main;
                    nowMain = case3[(code % 10) - 1];
                    break;
                default:
                    Debug.Log(code + " 해당하는 클립이 없습니다.");
                    break;
            }
        }
        else 
        {
            switch (code / 10)
            {
                case 1:
                    case1[(code % 10) - 1].targetTexture = (RenderTexture)sub;
                    nowSub = case1[(code % 10) - 1];
                    break;
                case 2:
                    case2[(code % 10) - 1].targetTexture = (RenderTexture)sub;
                    nowSub = case2[(code % 10) - 1];
                    break;
                case 3:
                    case3[(code % 10) - 1].targetTexture = (RenderTexture)sub;
                    nowSub = case3[(code % 10) - 1];
                    break;
                default:
                    Debug.Log(code + " 해당하는 클립이 없습니다.");
                    break;
            }
        }

        switch(code/10)
        {
            case 1:
                sideButton.sprite = sideImages[(code % 10) - 1];
                bigButton.sprite = upSideImages[(code % 10) - 1];
                break;
            case 2:
                break;
            case 3:
                break;
        }

        for (int i = 0; i < case1.Count; i++)
        {
            if (case1[i] == nowMain || case1[i] == nowSub)
                continue;
            case1[i].targetTexture = (RenderTexture)wait;
        }
        for (int i = 0; i < case2.Count; i++)
        {
            if (case2[i].targetTexture == nowMain || case2[i].targetTexture == nowSub)
                continue;
            case2[i].targetTexture = (RenderTexture)wait;
        }
        for (int i = 0; i < case3.Count; i++)
        {
            if (case3[i].targetTexture == nowMain || case3[i].targetTexture == nowSub)
                continue;
            case3[i].targetTexture = (RenderTexture)wait;
        }
    }

    public void VideoStart()
    {
        if(isMain)
        {
            nowMain.Play();
        }
        else
        {
            nowSub.Play();
        }
    }

    public void VideoStop()
    {
        if (isMain)
        {
            nowMain.Stop();
        }
        else
        {
            nowSub.Stop();
        }
    }

    public void VideoReset()
    {
        if (isMain)
        {
            nowMain.time = 0;
        }
        else
        {
            nowSub.time = 0;
        }
    }
}
