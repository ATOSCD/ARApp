using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MixedReality.Toolkit.Input;

public class GazeButtonMRTK3 : MonoBehaviour
{
    public Image gaugeBar;
    private float gazeThreshold = 2f; // 2초 응시 시 활성화
    private GazeInteractor gazeInteractor;
    private bool isGazing = false;

    void Start()
    {
        gazeInteractor = FindObjectOfType<GazeInteractor>(); // GazeInteractor 찾기
    }

    void Update()
    {
        if (IsUserLookingAtButton())
        {
            if (!isGazing)
            {
                isGazing = true;
                gaugeBarFillAnimation();
            }
        }
        else
        {
            isGazing = false;
            gaugeBar.DOKill();
            gaugeBar.fillAmount = 0f;
        }
    }

    bool IsUserLookingAtButton()
    {
        return gazeInteractor != null && gazeInteractor.hasHover; // GazeInteractor가 버튼을 감지하고 있는지 확인
    }

    void gaugeBarFillAnimation()
    {
        gaugeBar.fillAmount = 0f;
        gaugeBar.DOFillAmount(1f, gazeThreshold).OnComplete(() =>
        {
            ActivateButtonAction();
        });
    }

    void ActivateButtonAction()
    {
        Debug.Log("on");
    }
}