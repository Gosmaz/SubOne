using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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

    [Header("모니터 디스플레이")]
    public RawImage monitorScreen;

    [Header("미니 팝")]
    public Image multiPopImage;
    public Image miniPopImage;
    public GameObject miniPopButtons;
    public List<Button> miniBackButton;
    public Image monitorMultiPopImage;
    public Image monitorMiniPopImage;

    GameObject movedPop;

    VideoPlayer nowVideoPlayer;

    RawImage mainVideo;
    RawImage subVideo;
    RawImage multiVideo;

    Sprite[] sideImage1; //사이드 버튼 이미지
    Sprite[] bigDisplay; //위의 화면 이미지
    Sprite[] littleDisplay;
    Sprite[] MiniPopImages;
    Sprite[] multiPopImages;


    Image clickedButton;

    Vector3 waitPos;

    const float popDownTime = 0.7f;
    readonly int[] multiButtonX = new int[] {0, 0, 510, 430 ,350};

    bool isMainTurn = true;
    bool isMove = false;

    int buttonNum;


    private void OnApplicationQuit()
    {
        System.GC.Collect();
    }
    private void Awake()
    {
        System.GC.Collect();

        for (int i = 0; i < VideoPlayers1.Count; i++)
        {
            if(i == 1)
            {
                continue;
            }
            VideoPlayers1[i].url = "C:/VideoSource/" + (i + 1).ToString() + ".mp4";
            VideoPlayers1[i].Prepare();
            VideoPlayers1[i].Pause();
            VideoPlayers1[i].frame = 25;
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


        for (int i = 0; i < miniBackButton.Count; i++)
        {
            miniBackButton[i].gameObject.SetActive(false);
        }
        monitorMultiPopImage.gameObject.SetActive(false);
        monitorMiniPopImage.gameObject.SetActive(false);
    }

    public void Click(int num)
    {
        if (isMove)
        {
            return;
        }

        isMove = true;
        bool same = false;
        clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Image>(); // 클릭된 버튼의 이미지 가져오기

        buttonNum = num;

        for (int i = 0; i < littleButtons.Length; i++)
        {
            if (clickedButton.name == littleButtons[i].name) // 만약 클릭한 버튼이 버튼 리스트에 있는 버튼이라면
            {
                same = true;
                bigButtons[i].DOColor(new Color(1, 1, 1, 0), 0.5f);
                littleButtons[i].DOColor(new Color(1, 1, 1, 0), 0.5f) // 0.5초동안 버튼을 밝게 만들기
                    .OnComplete(() =>
                    {
                        isMove = false;
                        PopUp(buttonNum);
                    });
            }
        }

        if (!same)
        {
            PopUp(buttonNum);
        }
    }

    public void PopUp(int num)
    {
        if (isMove)
        {
            return;
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

        nowVideoPlayer = VideoPlayers1[buttonNum - 1];

       
        if (buttonNum == 2) // 멀티동영상 차례일때(미완성)
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
                mainPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos); // 옮긴 창을 대기 위치로 이동
            }
            else if (subPop.transform.localPosition != waitPos)
            {
                subPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
            }

            multiPop.transform.DOLocalMoveY(0, 1).OnComplete(() => OnCompleteSetting()); //해당하는 창 옮기기

            movedPop = multiPop;
        }
        else if (isMainTurn) //메인창의 차례일때
        {
            mainPopDisplay1.sprite = littleDisplay[buttonNum - 1];

            if (multiPop.transform.localPosition != waitPos)
            {
                multiPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
            }
            else if (subPop.transform.localPosition != waitPos)
            {
                subPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
            }

            mainPop.transform.DOLocalMoveY(0, 1).OnComplete(() => OnCompleteSetting());

            mainVideo = mainPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
            mainVideo.texture = VideoPlayers1[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
            monitorScreen.texture = VideoPlayers1[buttonNum - 1].texture;

            movedPop = mainPop;
        }
        else // 서브창의 차례일때
        {
            subPopDisplay2.sprite = littleDisplay[buttonNum - 1];

            if (mainPop.transform.localPosition != waitPos)
            {
                mainPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos);
            }
            if (multiPop.transform.localPosition != waitPos)
            {
                multiPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
            }

            subPop.transform.DOLocalMoveY(0, 1).OnComplete(() => OnCompleteSetting());

            subVideo = subPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
            subVideo.texture = VideoPlayers1[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
            monitorScreen.texture = VideoPlayers1[buttonNum - 1].texture;

            movedPop = subPop;
        }

    }

    public void Home()
    {
        isMove = true;

        VideoReset();

        movedPop.transform.DOLocalMoveY(-1200, popDownTime)
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
        bigButtonBackGround.gameObject.SetActive(true); //위 화면 버튼 대기창 키기
        sideMenu.transform.DOLocalMoveX(305, 0.5f); // 사이드 버튼 끄기

        for (int i = 0; i < littleButtons.Length; i++)
        {
            littleButtons[i].color = new Color(1, 1, 1, 170 / 255f); // 모든 버튼 색 초기화
            bigButtons[i].color = new Color(1, 1, 1, 170 / 255f);
        }

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
        nowVideoPlayer.frame = 25;
    }

    public void MiniPopUp(int num)
    {
        switch(num)
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


        monitorMiniPopImage.gameObject.SetActive(true);
        monitorMiniPopImage.sprite = MiniPopImages[num - 1];
        monitorMultiPopImage.sprite = multiPopImages[num];
    }
    public void MiniBack()
    {
        for(int i = 0; i < miniBackButton.Count; i++)
        {
            miniBackButton[i].gameObject.SetActive(false);
        }
        multiPopImage.sprite = multiPopImages[0];
        miniPopImage.gameObject.SetActive(false);
        miniPopButtons.SetActive(true);

        monitorMultiPopImage.sprite = multiPopImages[0];
        monitorMiniPopImage.gameObject.SetActive(false);
    }
}
