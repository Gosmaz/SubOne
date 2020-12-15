using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class PopUpHandlerUnion : MonoBehaviour
{
    [Header("팝업창들")]
    public GameObject mainPop;
    public GameObject subPop;
    public GameObject multiPop; // 장비 성능창

    [Header("팝업창 이미지")]
    public Image monitorDisplay;
    public Image mainPopDisplay1;
    public Image subPopDisplay2;
    public Image multiPopDisplay3;

    [Header("비디오 컨트롤 버튼")]
    public GameObject VideoControlButtons; //비디오 컨드롤 버튼들

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
    public List<VideoPlayer> VideoPlayers2;
    public List<VideoPlayer> VideoPlayers3;

    [Header("모니터 디스플레이")]
    public RawImage monitorScreen;

    GameObject movedPop;

    VideoPlayer nowVideoPlayer;

    RawImage mainVideo;
    RawImage subVideo;

    Sprite[] sideImage1; //사이드 버튼 이미지
    Sprite[] bigDisplay; //위의 화면 이미지
    Sprite[] littleDisplay;


    Image clickedButton;

    Vector3 waitPos;

    string caseState = "배경화면";
    int caseLevel = 0;

    float popDownTime = 0.7f;

    bool isMainTurn = true;
    bool isMove = false;

    int buttonNum;

    void Start()
    {
        waitPos = mainPop.transform.localPosition; // 위치 초기화
        subPop.transform.localPosition = waitPos;
        multiPop.transform.localPosition = waitPos;

        for(int i = 0; i < VideoPlayers1.Count; i++)
        {
            VideoPlayers1[i].Prepare();
            VideoPlayers1[i].Pause();
        }
        for (int i = 0; i < VideoPlayers2.Count; i++)
        {
            VideoPlayers2[i].Prepare();
            VideoPlayers2[i].Pause();
        }
        for (int i = 0; i < VideoPlayers3.Count; i++)
        {
            VideoPlayers3[i].Prepare();
            VideoPlayers3[i].Pause();
        }

        sideImage1 = Resources.LoadAll<Sprite>("SideButtonImages");
        bigDisplay = Resources.LoadAll<Sprite>("BigDisplay");
        littleDisplay = Resources.LoadAll<Sprite>("PopDisplay");
    }

    public void Click(int num)
    {
        if(isMove)
        {
            return;
        }

        if(num == 7)
        {
            ChangeCase1();
            num = 1;
        }
        else if(num == 8)
        {
            ChangeCase2();
            num = 1;
        }

        isMove = true;
        bool same = false;
        clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Image>(); // 클릭된 버튼의 이미지 가져오기

        buttonNum = num;

        for(int i = 0; i < littleButtons.Length; i++)
        {
            if(clickedButton.name == littleButtons[i].name) // 만약 클릭한 버튼이 버튼 리스트에 있는 버튼이라면
            {
                same = true;
                littleButtons[i].DOColor(new Color(1, 1, 1, 0), 0.5f) // 0.5초동안 버튼을 밝게 만들기
                    .OnStart(() =>
                    {
                        bigButtons[i].DOColor(new Color(1, 1, 1, 0), 0.5f);
                    })
                    .OnComplete(() =>
                    {
                        isMove = false;
                        PopUp(buttonNum);
                    });
            }
        }

        if(!same)
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
        VideoControlButtons.SetActive(true);
        bigButtonBackGround.gameObject.SetActive(false);

        sideMenu.transform.DOLocalMoveX(0, 1);

        sideMenu.GetComponent<Image>().sprite = sideImage1[buttonNum - 1];

        monitorDisplay.sprite = bigDisplay[buttonNum - 1 + (6 * caseLevel)];

        if(nowVideoPlayer != null)
        {
            VideoReset();
        }

        //switch (buttonBackGround.sprite.name)
        switch (caseState)
        {
            case "배경화면":

                nowVideoPlayer = VideoPlayers1[buttonNum - 1];

                if (buttonNum == 2) // 멀티동영상 차례일때
                {
                    multiPopDisplay3.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (mainPop.transform.localPosition != waitPos) //이전에 클릭된 창을 밑으로 옮기기
                    {
                        mainPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos); // 옮긴 창을 대기 위치로 이동
                    }
                    if (subPop.transform.localPosition != waitPos)
                    {
                        subPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
                    }

                    multiPop.transform.DOLocalMoveY(0, 1).OnComplete(()=>isMove=false); //해당하는 창 옮기기

                    movedPop = multiPop;
                }
                else if (isMainTurn) //메인창의 차례일때
                {
                    mainPopDisplay1.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (multiPop.transform.localPosition != waitPos)
                    {
                        multiPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
                    }
                    if (subPop.transform.localPosition != waitPos)
                    {
                        subPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
                    }

                    mainPop.transform.DOLocalMoveY(0, 1).OnComplete(() => isMove = false);

                    mainVideo = mainPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
                    mainVideo.texture = VideoPlayers1[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
                    monitorScreen.texture = VideoPlayers1[buttonNum - 1].texture;

                    movedPop = mainPop;
                }
                else // 서브창의 차례일때
                {
                    subPopDisplay2.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (mainPop.transform.localPosition != waitPos)
                    {
                        mainPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos);
                    }
                    if (multiPop.transform.localPosition != waitPos)
                    {
                        multiPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
                    }

                    subPop.transform.DOLocalMoveY(0, 1).OnComplete(() => isMove = false);

                    subVideo = subPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
                    subVideo.texture = VideoPlayers1[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
                    monitorScreen.texture = VideoPlayers1[buttonNum - 1].texture;

                    movedPop = subPop;
                }
                break;

            case "배경화면1": // Case1

                nowVideoPlayer = VideoPlayers2[buttonNum - 1];

                if (buttonNum == 2) // 멀티동영상 차례일때
                {
                    multiPopDisplay3.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (mainPop.transform.localPosition != waitPos) //이전에 클릭된 창을 밑으로 옮기기
                    {
                        mainPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos); // 옮긴 창을 대기 위치로 이동
                    }
                    if (subPop.transform.localPosition != waitPos)
                    {
                        subPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
                    }

                    multiPop.transform.DOLocalMoveY(0, 1).OnComplete(() => isMove = false); //해당하는 창 옮기기

                    movedPop = multiPop;
                }
                else if (isMainTurn) //메인창의 차례일때
                {
                    mainPopDisplay1.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (multiPop.transform.localPosition != waitPos)
                    {
                        multiPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
                    }
                    if (subPop.transform.localPosition != waitPos)
                    {
                        subPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
                    }

                    mainPop.transform.DOLocalMoveY(0, 1).OnComplete(() => isMove = false);

                    mainVideo = mainPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
                    mainVideo.texture = VideoPlayers2[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
                    monitorScreen.texture = VideoPlayers2[buttonNum - 1].texture;

                    movedPop = mainPop;
                }
                else // 서브창의 차례일때
                {
                    subPopDisplay2.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (mainPop.transform.localPosition != waitPos)
                    {
                        mainPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos);
                    }
                    if (multiPop.transform.localPosition != waitPos)
                    {
                        multiPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
                    }

                    subPop.transform.DOLocalMoveY(0, 1).OnComplete(() => isMove = false);

                    subVideo = subPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
                    subVideo.texture = VideoPlayers2[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
                    monitorScreen.texture = VideoPlayers2[buttonNum - 1].texture;

                    movedPop = subPop;
                }
                break;

            case "배경화면2": //Case2

                nowVideoPlayer = VideoPlayers3[buttonNum - 1];

                if (buttonNum == 2) // 멀티동영상 차례일때
                {
                    multiPopDisplay3.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (mainPop.transform.localPosition != waitPos) //이전에 클릭된 창을 밑으로 옮기기
                    {
                        mainPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos); // 옮긴 창을 대기 위치로 이동
                    }
                    if (subPop.transform.localPosition != waitPos)
                    {
                        subPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
                    }

                    multiPop.transform.DOLocalMoveY(0, 1).OnComplete(() => isMove = false); //해당하는 창 옮기기

                    movedPop = multiPop;
                }
                else if (isMainTurn) //메인창의 차례일때
                {
                    mainPopDisplay1.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (multiPop.transform.localPosition != waitPos)
                    {
                        multiPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
                    }
                    if (subPop.transform.localPosition != waitPos)
                    {
                        subPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => subPop.transform.localPosition = waitPos);
                    }

                    mainPop.transform.DOLocalMoveY(0, 1).OnComplete(() => isMove = false);

                    mainVideo = mainPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
                    mainVideo.texture = VideoPlayers3[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
                    monitorScreen.texture = VideoPlayers3[buttonNum - 1].texture;

                    movedPop = mainPop;
                }
                else // 서브창의 차례일때
                {
                    subPopDisplay2.sprite = littleDisplay[buttonNum - 1 + (6 * caseLevel)];

                    if (mainPop.transform.localPosition != waitPos)
                    {
                        mainPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => mainPop.transform.localPosition = waitPos);
                    }
                    if (multiPop.transform.localPosition != waitPos)
                    {
                        multiPop.transform.DOLocalMoveY(-1200, popDownTime).OnComplete(() => multiPop.transform.localPosition = waitPos);
                    }

                    subPop.transform.DOLocalMoveY(0, 1).OnComplete(() => isMove = false);

                    subVideo = subPop.transform.Find("Video").GetComponent<RawImage>(); // Video 자식 오브젝트 가져오기
                    subVideo.texture = VideoPlayers3[buttonNum - 1].texture; //Video의 자식 오브젝트 텍스쳐 바꾸기
                    monitorScreen.texture = VideoPlayers3[buttonNum - 1].texture;

                    movedPop = subPop;
                }
                break;
        }
    }

    public void Home()
    {
        isMove = true;

        caseState = "배경화면";
        caseLevel = 0;

        movedPop.transform.DOLocalMoveY(-1200, popDownTime)
            .OnComplete(() => 
            {
                movedPop.transform.localPosition = waitPos;
                isMove = false; 
                bgAlpha.SetActive(false);
            });

        VideoControlButtons.SetActive(false); // 비디오 컨트롤창 끄기
        bigButtonBackGround.gameObject.SetActive(true); //위 화면 버튼창 키기
        sideMenu.transform.DOLocalMoveX(305, 0.5f); // 사이드 버튼 끄기

        for (int i = 0; i < littleButtons.Length; i++)
        {
            littleButtons[i].color = new Color(1, 1, 1, 170 / 255f); // 모든 버튼 색 초기화
            bigButtons[i].color = new Color(1, 1, 1, 170 / 255f);
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
        nowVideoPlayer.time = 0;
    }

    public void ChangeCase1()
    {
        caseState = "배경화면1";
        caseLevel = 1;

    }

    public void ChangeCase2()
    {
        caseState = "배경화면2";
        caseLevel = 2;
    }
}
