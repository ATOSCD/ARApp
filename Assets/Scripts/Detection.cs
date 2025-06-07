using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using TMPro;

public class Detection : MonoBehaviour
{
    public NNModel model;
    private Model runtimeModel;
    private IWorker worker;

    public int cameraResolutionWidth = 640;
    public int cameraResolutionHeight = 480;
    public int inferenceImgSize = 320;
    public int numClasses = 14;
    public float confidenceThreshold = 0.5f;

    private List<DetectionResult> boxes;
    private List<Tuple<GameObject, Renderer>> labels;
    private COCONames names;
    private COCOColors colors;
    private Texture2D croppedTexture;

    private WebCamTexture webCamTexture;
    private TextMeshPro textMesh;

    public Material boundingBoxMaterial;

    public List<GameObject> buttonPrefabs;
    private Dictionary<string, GameObject> buttonPrefabsDict;
    private Dictionary<string, GameObject> activeButtonInstances = new Dictionary<string, GameObject>();

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            webCamTexture = new WebCamTexture(devices[0].name, cameraResolutionWidth, cameraResolutionHeight);
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("No camera devices found!");
            return;
        }

        runtimeModel = ModelLoader.Load(model);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, runtimeModel);

        names = new COCONames();
        colors = new COCOColors();
        croppedTexture = new Texture2D(inferenceImgSize, inferenceImgSize, TextureFormat.RGB24, false);

        var predictionObject = GameObject.Find("Prediction");
        if (predictionObject != null)
        {
            textMesh = predictionObject.GetComponent<TextMeshPro>();
        }

        boxes = new List<DetectionResult>();
        labels = new List<Tuple<GameObject, Renderer>>();

        InitializeClassButtons();

        StartCoroutine(DetectWebcam());
    }

    private void InitializeClassButtons()
    {
        buttonPrefabsDict = new Dictionary<string, GameObject>();

        for (int i = 0; i < numClasses; i++)
        {
            if (i < buttonPrefabs.Count)
            {
                buttonPrefabsDict[names.map[i]] = buttonPrefabs[i];
            }
        }
    }

    private void OnDestroy()
    {
        webCamTexture?.Stop();
        worker?.Dispose();
    }

    public IEnumerator DetectWebcam()
    {
        while (true)
        {
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                // 원본 카메라 이미지 저장
                Texture2D originalTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
                originalTexture.SetPixels(webCamTexture.GetPixels());
                originalTexture.Apply();

                byte[] rawImageBytes = originalTexture.EncodeToJPG();
                string rawPath = System.IO.Path.Combine(Application.persistentDataPath, "raw_input.jpg");
                System.IO.File.WriteAllBytes(rawPath, rawImageBytes);
                Debug.Log("Saved raw WebCamTexture image to: " + rawPath);

                CropTexture(webCamTexture, inferenceImgSize, inferenceImgSize);
                var tensor = PreprocessTexture(croppedTexture);

                // 모델 입력으로 들어가는 크롭된 이미지를 저장해보기
                byte[] bytes = croppedTexture.EncodeToJPG();
                string savePath = System.IO.Path.Combine(Application.persistentDataPath, "input.jpg");
                System.IO.File.WriteAllBytes(savePath, bytes);
                Debug.Log("Saved cropped input to: " + savePath);

                worker.Execute(tensor).FlushSchedule(true);
                Tensor result = worker.PeekOutput("output0");

                Debug.Log($"Output shape: {result.shape}");
                Debug.Log($"Tensor dims: batch={result.shape.batch}, height={result.shape.height}, width={result.shape.width}, channels={result.shape.channels}");

                var boxes_tmp = new List<DetectionResult>();
                ParseYoloOutput(result, confidenceThreshold, boxes_tmp);
                boxes = NonMaxSuppression(0.5f, boxes_tmp);

                if (textMesh != null)
                {
                    textMesh.text = $"Boxes: {boxes.Count}";
                }

                foreach (var (go, r) in labels)
                {
                    Destroy(r);
                    Destroy(go);
                }
                labels.Clear();

                foreach (var box in boxes)
                {
                    // 원본 카메라 이미지 저장
                    Texture2D originalDetectedTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
                    originalDetectedTexture.SetPixels(webCamTexture.GetPixels());
                    originalDetectedTexture.Apply();

                    byte[] rawDetectedImageBytes = originalDetectedTexture.EncodeToJPG();
                    string name = $"{box.Label}_detected";
                    string rawDetectedPath = System.IO.Path.Combine(Application.persistentDataPath, name + ".jpg");
                    System.IO.File.WriteAllBytes(rawDetectedPath, rawDetectedImageBytes);
                    Debug.Log("Saved detected raw WebCamTexture image to: " + rawDetectedPath);

                    // 모델 입력으로 들어가는 크롭된 이미지를 저장해보기
                    byte[] detectedBytes = croppedTexture.EncodeToJPG();
                    string saveDetectedPath = System.IO.Path.Combine(Application.persistentDataPath, name + "_cropped.jpg");
                    System.IO.File.WriteAllBytes(saveDetectedPath, detectedBytes);
                    Debug.Log("Saved detected cropped input to: " + saveDetectedPath);


                    GenerateBoundingBox(box);
                }

                tensor.Dispose();
                result.Dispose();
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private void CropTexture(WebCamTexture sourceTexture, int cropWidth, int cropHeight)
    {
        int texWidth = sourceTexture.width;
        int texHeight = sourceTexture.height;

        if (texWidth < cropWidth || texHeight < cropHeight)
        {
            Debug.LogWarning($"Camera resolution ({texWidth}x{texHeight}) is smaller than inference size ({cropWidth}x{cropHeight}). Skipping frame.");
            return;
        }

        int centerX = Mathf.Clamp(texWidth / 2 - cropWidth / 2, 0, texWidth - cropWidth);
        int centerY = Mathf.Clamp(texHeight / 2 - cropHeight / 2, 0, texHeight - cropHeight);

        Color[] pixels = sourceTexture.GetPixels(centerX, centerY, cropWidth, cropHeight);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
    }

    private void ParseYoloOutput(Tensor tensor, float confidenceThreshold, List<DetectionResult> boxes)
    {
        for (int i = 0; i < tensor.shape.width; i++)
        {
            var (label, confidence) = GetClassIdx(tensor, i, 0);
            if (confidence < confidenceThreshold)
                continue;

            BoundingBox box = ExtractBoundingBox(tensor, i);
            var labelName = names.map[label];
            boxes.Add(new DetectionResult { Bbox = box, Confidence = confidence, Label = labelName, LabelIdx = label });
        }
    }

    private BoundingBox ExtractBoundingBox(Tensor tensor, int row)
    {
        float rawX = tensor[0, 0, row, 0];
        float rawY = tensor[0, 0, row, 1];
        float rawW = tensor[0, 0, row, 2];
        float rawH = tensor[0, 0, row, 3];

        Debug.Log($"Raw BBox: X={rawX}, Y={rawY}, W={rawW}, H={rawH}");
        return new BoundingBox
        {
            X = tensor[0, 0, row, 0] / inferenceImgSize,
            Y = tensor[0, 0, row, 1] / inferenceImgSize,
            Width = tensor[0, 0, row, 2] / inferenceImgSize,
            Height = tensor[0, 0, row, 3] / inferenceImgSize
        };
    }

    private ValueTuple<int, float> GetClassIdx(Tensor tensor, int row, int batch)
    {
        int classIdx = 0;
        float maxConf = tensor[0, 0, row, 4];
        for (int i = 0; i < numClasses; i++)
        {
            if (tensor[batch, 0, row, 4 + i] > maxConf)
            {
                maxConf = tensor[batch, 0, row, 4 + i];
                classIdx = i;
            }
        }
        return (classIdx, maxConf);
    }

    private Tensor PreprocessTexture(Texture2D tex)
    {
        // Barracuda는 자동 정규화/채널변환을 안 해주므로 직접 처리
        Color32[] pixels = tex.GetPixels32();
        float[] input = new float[tex.width * tex.height * 3];
        for (int i = 0; i < pixels.Length; i++)
        {
            // RGB 순서, 0~1 정규화
            input[i * 3 + 0] = pixels[i].r / 255.0f;
            input[i * 3 + 1] = pixels[i].g / 255.0f;
            input[i * 3 + 2] = pixels[i].b / 255.0f;
        }
        // ONNX 입력 shape이 (1, height, width, 3)라면 아래처럼 생성
        return new Tensor(1, tex.height, tex.width, 3, input);
    }

    private void GenerateBoundingBox(DetectionResult det)
    {
        float zDistance = 2.0f;

        Debug.Log("Bbox.X: " + det.Bbox.X);
        Debug.Log("Bbox.Y: " + det.Bbox.Y);
        Debug.Log("Bbox.Width: " + det.Bbox.Width);
        Debug.Log("Bbox.Height: " + det.Bbox.Height);

        float xMin = det.Bbox.X - det.Bbox.Width / 2f;
        float yMin = det.Bbox.Y - det.Bbox.Height / 2f;
        float xMax = det.Bbox.X + det.Bbox.Width / 2f;
        float yMax = det.Bbox.Y + det.Bbox.Height / 2f;

        Vector3 worldBL = ImageToWorldPosition(xMin, yMin, zDistance);
        Vector3 worldTL = ImageToWorldPosition(xMin, yMax, zDistance);
        Vector3 worldTR = ImageToWorldPosition(xMax, yMax, zDistance);
        Vector3 worldBR = ImageToWorldPosition(xMax, yMin, zDistance);

        GameObject boxGO = new GameObject("BoundingBox");
        LineRenderer lr = boxGO.AddComponent<LineRenderer>();
        lr.widthMultiplier = 0.005f;
        lr.loop = true;
        lr.positionCount = 4;

        lr.material = boundingBoxMaterial != null ? boundingBoxMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.material.color = colors.map[det.LabelIdx];

        lr.SetPosition(0, worldBL);
        lr.SetPosition(1, worldTL);
        lr.SetPosition(2, worldTR);
        lr.SetPosition(3, worldBR);

        labels.Add(Tuple.Create(boxGO, (Renderer)lr));

        GameObject textGO = new GameObject("LabelText");
        textGO.transform.position = worldTL + new Vector3(0, 0.05f, 0);
        textGO.transform.LookAt(Camera.main.transform);
        textGO.transform.Rotate(0, 180f, 0);

        TextMeshPro tm = textGO.AddComponent<TextMeshPro>();
        tm.text = $"{det.Label} ({det.Confidence:F2})";
        tm.fontSize = 0.3f;
        tm.alignment = TextAlignmentOptions.Center;
        tm.color = colors.map[det.LabelIdx];

        labels.Add(Tuple.Create(textGO, textGO.GetComponent<Renderer>()));

        // 버튼은 라벨마다 한 번만 생성
        if (!activeButtonInstances.ContainsKey(det.Label))
        {
            if (buttonPrefabsDict.ContainsKey(det.Label))
            {
                GameObject prefab = buttonPrefabsDict[det.Label];
                GameObject newButton = Instantiate(prefab);

                Vector3 directionToCamera = (Camera.main.transform.position - worldBR).normalized;
                Vector3 spawnPos = worldBR + directionToCamera * 0.1f;

                newButton.transform.position = spawnPos;
                newButton.transform.LookAt(Camera.main.transform);
                newButton.transform.Rotate(0, 180f, 0);
                newButton.transform.localScale = Vector3.one * 1.5f;

                TextMeshPro text = newButton.GetComponentInChildren<TextMeshPro>();

                activeButtonInstances[det.Label] = newButton;
                Debug.Log($"Button created for label: {det.Label}");
            }
            else
            {
                Debug.LogWarning($"No prefab assigned for label: {det.Label}");
            }
        }
    }

    private List<DetectionResult> NonMaxSuppression(float threshold, List<DetectionResult> boxes)
    {
        return boxes.OrderByDescending(b => b.Confidence).GroupBy(b => b.Label).Select(g => g.First()).ToList();
    }

    private Vector3 ImageToWorldPosition(float x, float y, float zDistance)
    {
        float normalizedX = Mathf.Clamp01(x);
        float normalizedY = Mathf.Clamp01(y);

        // 스크린 좌표로 변환 (카메라 비율 고려)
        float screenX = normalizedX * Screen.width;
        float screenY = normalizedY * Screen.height;

        Vector3 screenPos = new Vector3(screenX, screenY, zDistance);

        // Raycast 기반 동적 거리 계산 (선택)
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            screenPos.z = hitInfo.distance;
        }

        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
