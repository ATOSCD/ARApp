using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MixedReality.Toolkit.Input;

public class GazeButtonMRTK3 : MonoBehaviour
{
    public Image gaugeBar;
    private float gazeThreshold = 2f; // 2�� ���� �� Ȱ��ȭ
    private GazeInteractor gazeInteractor;
    private bool isGazing = false;

    void Start()
    {
        gazeInteractor = FindObjectOfType<GazeInteractor>(); // GazeInteractor ã��
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
        return gazeInteractor != null && gazeInteractor.hasHover; // GazeInteractor�� ��ư�� �����ϰ� �ִ��� Ȯ��
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