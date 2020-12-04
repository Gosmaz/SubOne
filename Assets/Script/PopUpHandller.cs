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

    public List<VideoClip> case1;
    public List<VideoClip> case2;
    public List<VideoClip> case3;

    public VideoPlayer subVideo;
    public VideoPlayer mainVideo;

    public RawImage monitor;

    public Texture main;
    public Texture sub;
    public Texture baseMonitor;


    bool isMoving;
    bool isMain = false;

    Vector3 ogPos;
    Vector3 sidePos;

    private void Start()
    {
        ogPos = subPlayGround.transform.localPosition;
        sidePos = sideBar.transform.localPosition;

        mainPlayGround.transform.localPosition = ogPos;
        subPlayGround.transform.localPosition = ogPos;

        monitor.texture = baseMonitor;
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
                    mainVideo.clip = case1[code % 10 - 1];
                    break;
                case 2:
                    mainVideo.clip = case2[code % 10 - 1];
                    break;
                case 3:
                    mainVideo.clip = case3[code % 10 - 1];
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
                    subVideo.clip = case1[code % 10 - 1];
                    break;
                case 2:
                    subVideo.clip = case2[code % 10 - 1];
                    break;
                case 3:
                    subVideo.clip = case3[code % 10 - 1];
                    break;
                default:
                    Debug.Log(code + " 해당하는 클립이 없습니다.");
                    break;
            }
        }
    }
}
