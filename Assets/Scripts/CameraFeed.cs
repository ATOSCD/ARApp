// 2. MRTK3 ��� ī�޶� �ǵ� ���� (��: Unity WebCamTexture ���)
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

// 3. MRTK3 ���� �ʿ� ���� (������ �����̹Ƿ� ��ũ��Ʈȭ �Ұ�)
// - MRTK3 Foundation ��ġ
// - XR Plug-in Management: OpenXR Ȱ��ȭ
// - MRTK Input Profile ����
// - MRTK UX Components�� �ʿ��� UI ��� ��ġ (��: ��ư, �α� ��� ��)