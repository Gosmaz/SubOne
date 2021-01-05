using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class PopUpHandlerUnion : MonoBehaviour
{
    [Header("팝업창들")]
    public GameObject mainPop;
    public GameObject subPop;
    public GameObject multiPop; // 장비 성능창(2번)

    [Header("팝업창 이미지")]
    public Image monitorDisplay;
    public Image mainPopDisplay1;
    public Image subPopDisplay2;
    public Image multiPopDisplay3;

    [Header("비디오 컨트롤 버튼")]
    public List<GameObject> VideoControlButtons; //비디오 컨드롤 버튼들
    public Image mainVideoChangeImage;
    public Image subVideoChangeImage;
    public List<GameObject> mainVideoChangeMask;
    public List<GameObject> subVideoChangeMask;
    public Slider mainVideoTimeSlider;

    [Header("사이드 버튼")]
    public RectTransform sideMenu; // 사이드 버튼

    [Header("배경화면")]
    public Image buttonBackGround; //버튼 이미지
    public Image bigButtonBackGround;

    [Header("버튼 클릭시 알파값")]
    public GameObject bgAlpha;

    [Header("배경화면 버튼들")]
    public Image[] littleButtons; //작은 화면의 버튼들
    public Image[] bigButtons; // 큰 화면의 버튼들

    [Header("비디오 플레이어")]
    public List<VideoPlayer> VideoPlayers1;
    public List<VideoPlayer> tempVideoPlayer;

    [Header("모니터 디스플레이")]
    public RawImage monitorScreen;

    [Header("미니 팝")]
    public Image multiPopImage;
    public Image miniPopImage;
    public GameObject miniPopButtons;
    public GameObject miniExitButton;
    public List<Button> miniBackButton;
    public Image monitorMultiPopImage;
    public Image monitorMiniPopImage;

    [Header("다른 스크립트에서 쓰는 것들")]
    public VideoPlayer nowVideoPlayer;

    
    GameObject movedPop;

    RawImage mainVideo;
    RawImage subVideo;
    RawImage nowVideo;

    Sprite[] sideImage1; //사이드 버튼 이미지
    Sprite[] bigDisplay; //위의 화면 이미지
    Sprite[] littleDisplay;
    Sprite[] MiniPopImages;
    Sprite[] multiPopImages;
    Sprite[] videoChangeButtonImages;

    int[] videoMaxCount = { 0, 0, 0, 0, 0, 0, 0, 0 };

    readonly string[] folders = { "01_CoustomerEnvironmentAnalysis", "02_StatusOfEquipment", "03_ConceptualDesign", "04_VirtualSimulation", "05_DetailModeling", "06_Test&Verification", "07_PaperCorrugatedCardboard", "08_EPSPacking" };

    Vector3 waitPos;

    IEnumerator test;

    const float popDownTime = 0.7f;

    bool isMainTurn = true;
    bool isMove = false;

    bool isSpeedup = false;

    int buttonNum;

    private void OnApplicationQuit()
    {
        System.GC.Collect();
    }
    private void Awake()
    {
        for (int i = 0; i < VideoPlayers1.Count; i++)
        {
            if (i == 1)
            {
                continue;
            }


            VideoPlayers1[i].url = "C:/VideoSource/" + folders[i] + "/0" + (i + 1).ToString() + "_01.mp4";
            VideoPlayers1[i].Prepare();
            VideoPlayers1[i].Pause();
            VideoPlayers1[i].frame = 1;
        }

        for (int i = 0; i < VideoPlayers1.Count; i++)
        {
            for (int j = 1; j <= 4; j++)
            {
                if (!File.Exists("C:/VideoSource/" + folders[i] + "/0" + (i + 1).ToString() + "_0" + j.ToString() + ".mp4"))
                {
                    videoMaxCount[i] = j - 1;
                    break;
                }

            }
        }
    }


    void Start()
    {
        waitPos = mainPop.transform.localPosition; // 위치 초기화
        subPop.transform.localPosition = waitPos;
        multiPop.transform.localPosition = waitPos;

        sideImage1 = Resources.LoadAll<Sprite>("SideButtonImages");
        bigDisplay = Resources.LoadAll<Sprite>("BigDisplay");
        littleDisplay = Resources.LoadAll<Sprite>("PopDisplay");
        MiniPopImages = Resources.LoadAll<Sprite>("MultiMiniPopImages");
        multiPopImages = Resources.LoadAll<Sprite>("MultiPopImages");
        videoChangeButtonImages = Resources.LoadAll<Sprite>("VideoChangeButton");


        for (int i = 0; i < miniBackButton.Count; i++)
        {
            miniBackButton[i].gameObject.SetActive(false);
        }
        monitorMultiPopImage.gameObject.SetActive(false);
        monitorMiniPopImage.gameObject.SetActive(false);

        test = PrepareSetting();
    }

    public void ColorChange(int num)
    {
        if (Input.GetMouseButton(0))
        {
            littleButtons[num - 1].DOColor(new Color(1, 1, 1, 0), 0.5f);
            bigButtons[num - 1].DOColor(new Color(1, 1, 1, 0), 0.5f);
        }
    }

    public void FocusOutColor(int num)
    {
        littleButtons[num - 1].DOColor(new Color(1, 1, 1, 140 / 255f), 0.5f);
        bigButtons[num - 1].DOColor(new Color(1, 1, 1, 140 / 255f), 0.5f);
    }

    public void Click(int num)
    {
        if (isMove)
        {
            return;
        }

        isMove = true;

        buttonNum = num;

        if (buttonNum % 2 == 0)
        {
            if (buttonNum == 2)
            {
                SetPopUp(multiPop);
            }
            else if (isMainTurn)
            {
                SetPopUp(subPop);
            }
            else
            {
                SetPopUp(mainPop);
            }
        }

        bigButtons[num - 1].DOColor(new Color(1, 1, 1, 140 / 255f), 0.5f);
        littleButtons[num - 1].DOColor(new Color(1, 1, 1, 140 / 255f), 0.5f) // 0.5초동안 버튼을 밝게 만들기
            .OnComplete(() =>
            {
                isMove = false;
                PopUp(buttonNum);
            });
    }

    public void PopUp(int num)
    {
        if (isMove)
        {
            return;
        }

        mainVideoChangeImage.sprite = videoChangeButtonImages[0];
        subVideoChangeImage.sprite = videoChangeButtonImages[0];

        switch (videoMaxCount[num - 1])
        {
            case 1:
            case 2:
            case 3:
                for (int i = 0; i < mainVideoChangeMask.Count; i++)
                {
                    mainVideoChangeMask[i].SetActive(false);
                }
                mainVideoChangeMask[videoMaxCount[num - 1] - 1].SetActive(true);

                for (int i = 0; i < subVideoChangeMask.Count; i++)
                {
                    subVideoChangeMask[i].SetActive(false);
                }
                subVideoChangeMask[videoMaxCount[num - 1] - 1].SetActive(true);
                break;
            default:
                for (int i = 0; i < mainVideoChangeMask.Count; i++)
                {
                    mainVideoChangeMask[i].SetActive(false);
                }
                for (int i = 0; i < subVideoChangeMask.Count; i++)
                {
                    subVideoChangeMask[i].SetActive(false);
                }
                break;
        }

        buttonNum = num;

        isMove = true;
        isMainTurn = !isMainTurn;

        bgAlpha.SetActive(true);
        bigButtonBackGround.gameObject.SetActive(false);
        monitorMultiPopImage.gameObject.SetActive(false);

        sideMenu.transform.DOLocalMoveX(0, 1);

        sideMenu.GetComponent<Image>().sprite = sideImage1[buttonNum - 1];

        monitorDisplay.sprite = bigDisplay[buttonNum - 1];

        if (nowVideoPlayer != null)
        {
            VideoReset();
        }

        if (VideoPlayers1[buttonNum - 1].url != "C:/VideoSource/" + folders[buttonNum - 1] + "/0" + buttonNum.ToString() + "_01.mp4" && buttonNum != 2)
        {
            VideoPlayers1[buttonNum - 1].url = "C:/VideoSource/" + folders[buttonNum - 1] + "/0" + buttonNum.ToString() + "_01.mp4";
            VideoPlayers1[buttonNum - 1].Prepare();
            VideoPlayers1[buttonNum - 1].frame = 1;
        }

        nowVideoPlayer = VideoPlayers1[buttonNum - 1];

        StopCoroutine(test);
        test = PrepareSetting();
        StartCoroutine(test);

        Debug.Log(nowVideoPlayer.frame);

        if (buttonNum == 2) // 멀티동영상 차례일때
        {
            multiPopDisplay3.sprite = littleDisplay[buttonNum - 1];

            miniPopButtons.SetActive(true);
            miniPopImage.gameObject.SetActive(false);
            monitorMultiPopImage.gameObject.SetActive(true);
            monitorMiniPopImage.gameObject.SetActive(false);

            multiPopImage.sprite = multiPopImages[0];
            monitorMultiPopImage.sprite = multiPopImages[0];

            if (mainPop.transform.localPosition != waitPos) //이전에 클릭된 창을 밑으로 옮기기
            {
                mainPop.transform.DOLocalMoveY(-2200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos); // 옮긴 창을 대기 위치로 이동
            }
            else if (subPop.transform.localPosition != waitPos)
            {
                subPop.transform.DOLocalMoveY(-2200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
            }

            multiPop.transform.DOLocalMoveY(0, 1).OnComplete(() => OnCompleteSetting()); //해당하는 창 옮기기

            movedPop = multiPop;
        }
        else if (isMainTurn) //메인창의 차례일때
        {
            mainPopDisplay1.sprite = littleDisplay[buttonNum - 1];


            if (multiPop.transform.localPosition != waitPos)
            {
                multiPop.transform.DOLocalMoveY(-2200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
            }
            else if (subPop.transform.localPosition != waitPos)
            {
                subPop.transform.DOLocalMoveY(-2200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
            }

            mainPop.transform.DOLocalMoveY(0, 1).OnComplete(() => OnCompleteSetting());

            mainVideo = mainPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
            mainVideo.texture = VideoPlayers1[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
            monitorScreen.texture = VideoPlayers1[buttonNum - 1].texture;

            nowVideo = mainVideo;
            movedPop = mainPop;
        }
        else // 서브창의 차례일때
        {
            subPopDisplay2.sprite = littleDisplay[buttonNum - 1];

            if (mainPop.transform.localPosition != waitPos)
            {
                mainPop.transform.DOLocalMoveY(-2200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos);
            }
            if (multiPop.transform.localPosition != waitPos)
            {
                multiPop.transform.DOLocalMoveY(-2200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
            }

            subPop.transform.DOLocalMoveY(0, 1).OnComplete(() => OnCompleteSetting());

            subVideo = subPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
            subVideo.texture = VideoPlayers1[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
            monitorScreen.texture = VideoPlayers1[buttonNum - 1].texture;

            nowVideo = subVideo;
            movedPop = subPop;
        }

    }

    private void SetPopUp(GameObject pop)
    {
        if(buttonNum % 2 == 0)
        {
            pop.transform.localPosition = new Vector3(waitPos.x, -waitPos.y, waitPos.z);
        }

    }

    public void Home()
    {
        isMove = true;

        VideoReset();

        mainVideoChangeImage.sprite = videoChangeButtonImages[0];
        subVideoChangeImage.sprite = videoChangeButtonImages[0];

        movedPop.transform.DOLocalMoveY(-2200, popDownTime)
            .OnComplete(() =>
            {
                movedPop.transform.localPosition = waitPos;
                isMove = false;
                bgAlpha.SetActive(false);
            });

        for (int i = 0; i < VideoControlButtons.Count; i++)
        {
            VideoControlButtons[i].SetActive(false); // 비디오 컨트롤창 끄기
        }
        miniExitButton.SetActive(false);
        bigButtonBackGround.gameObject.SetActive(true); //위 화면 버튼 대기창 키기
        sideMenu.transform.DOLocalMoveX(707, 0.5f); // 사이드 버튼 끄기


    }

    private void OnCompleteSetting()
    {
        isMove = false;
        for (int i = 0; i < VideoControlButtons.Count; i++)
        {
            VideoControlButtons[i].SetActive(true); // 비디오 컨트롤창 켜기
        }

    }

    public void Play()
    {
        nowVideoPlayer.Play();
    }

    public void Pause()
    {
        nowVideoPlayer.Pause();
    }

    public void VideoReset()
    {
        nowVideoPlayer.Pause();
        nowVideoPlayer.frame = 1;
    }

    public void MiniPopUp(int num)
    {
        switch (num)
        {
            case 2:  //1개
            case 3:
            case 4:
                miniBackButton[0].gameObject.SetActive(true);
                break;
            case 1:  //2개
            case 6:
                miniBackButton[1].gameObject.SetActive(true);
                break;
            case 7:  //3개
                miniBackButton[2].gameObject.SetActive(true);
                break;
            case 5:  //4개
                miniBackButton[3].gameObject.SetActive(true);
                break;
        }

        miniPopButtons.SetActive(false);
        miniPopImage.gameObject.SetActive(true);
        miniPopImage.sprite = MiniPopImages[num - 1];
        multiPopImage.sprite = multiPopImages[num];
        miniExitButton.SetActive(true);


        monitorMiniPopImage.gameObject.SetActive(true);
        monitorMiniPopImage.sprite = MiniPopImages[num - 1];
        monitorMultiPopImage.sprite = multiPopImages[num];
    }
    public void MiniBack()
    {
        for (int i = 0; i < miniBackButton.Count; i++)
        {
            miniBackButton[i].gameObject.SetActive(false);
        }
        multiPopImage.sprite = multiPopImages[0];
        miniPopImage.gameObject.SetActive(false);
        miniPopButtons.SetActive(true);
        miniExitButton.SetActive(false);

        monitorMultiPopImage.sprite = multiPopImages[0];
        monitorMiniPopImage.gameObject.SetActive(false);
    }

    public void VideoChange(int num)
    {
        VideoReset();

        mainVideoChangeImage.sprite = videoChangeButtonImages[num - 1];
        subVideoChangeImage.sprite = videoChangeButtonImages[num - 1];

        if (num != 1)
        {
            nowVideoPlayer = tempVideoPlayer[num - 2];
            nowVideo.texture = tempVideoPlayer[num - 2].texture;
            monitorScreen.texture = nowVideo.texture;
        }
        else
        {
            nowVideoPlayer = VideoPlayers1[buttonNum - 1];
            nowVideo.texture = VideoPlayers1[buttonNum - 1].texture;
            monitorScreen.texture = VideoPlayers1[buttonNum - 1].texture;
        }

        nowVideoPlayer.Play();
    }

    IEnumerator PrepareSetting()
    {
        yield return new WaitForSeconds(1);
        if (File.Exists("C:/VideoSource/" + folders[buttonNum - 1] + "/0" + buttonNum.ToString() + "_02.mp4"))
        {
            tempVideoPlayer[0].url = "C:/VideoSource/" + folders[buttonNum - 1] + "/0" + buttonNum.ToString() + "_02.mp4";
            tempVideoPlayer[0].Prepare();
            tempVideoPlayer[0].Pause();
            tempVideoPlayer[0].frame = 1;
        }

        StartCoroutine(PrepareSetting2());
    }
    IEnumerator PrepareSetting2()
    {
        yield return new WaitForSeconds(0.1f);
        if (File.Exists("C:/VideoSource/" + folders[buttonNum - 1] + "/0" + buttonNum.ToString() + "_03.mp4"))
        {
            tempVideoPlayer[1].url = "C:/VideoSource/" + folders[buttonNum - 1] + "/0" + buttonNum.ToString() + "_03.mp4";
            tempVideoPlayer[1].Prepare();
            tempVideoPlayer[1].Pause();
            tempVideoPlayer[1].frame = 1;
        }
        StartCoroutine(PrepareSetting3());
    }
    IEnumerator PrepareSetting3()
    {
        nowVideoPlayer.Play();
        yield return new WaitForSeconds(0.1f);
        if (File.Exists("C:/VideoSource/" + folders[buttonNum - 1] + "/0" + buttonNum.ToString() + "_04.mp4"))
        {
            tempVideoPlayer[2].url = "C:/VideoSource/" + folders[buttonNum - 1] + "/0" + buttonNum.ToString() + "_04.mp4";
            tempVideoPlayer[2].Prepare();
            tempVideoPlayer[2].Pause();
            tempVideoPlayer[2].frame = 1;
        }

    }

    public void VideoSpeed()
    {
        if (isSpeedup)
        {
            nowVideoPlayer.playbackSpeed = -1;
        }
        else
        {
            nowVideoPlayer.playbackSpeed = 1;
        }

        isSpeedup = !isSpeedup;
    }


}
