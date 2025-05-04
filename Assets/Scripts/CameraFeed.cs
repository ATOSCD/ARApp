// 2. MRTK3 기반 카메라 피드 연동 (예: Unity WebCamTexture 사용)
using UnityEngine;
using UnityEngine.UI;

public class CameraFeed : MonoBehaviour
{
    private RawImage previewUI;
    private RenderTexture outputTexture;
    private WebCamTexture camTex;

    void Start()
    {
        outputTexture = new RenderTexture(160, 160, 0, RenderTextureFormat.ARGB32);
        outputTexture.Create();

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            camTex = new WebCamTexture(devices[0].name);
            camTex.Play();
        }
    }

    void Update()
    {
        if (camTex != null && camTex.didUpdateThisFrame)
        {
            Graphics.Blit(camTex, outputTexture);
        }
    }

    public RenderTexture GetOutputTexture()
    {
        if (outputTexture == null)
        {
            outputTexture = new RenderTexture(camTex.width, camTex.height, 0);
        }
        return outputTexture;
    }
}

// 3. MRTK3 설정 필요 사항 (에디터 설정이므로 스크립트화 불가)
// - MRTK3 Foundation 설치
// - XR Plug-in Management: OpenXR 활성화
// - MRTK Input Profile 구성
// - MRTK UX Components로 필요한 UI 요소 배치 (예: 버튼, 로그 출력 등)