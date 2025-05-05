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
    public int inferenceImgSize = 160;
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
                CropTexture(webCamTexture, inferenceImgSize, inferenceImgSize);
                var tensor = new Tensor(croppedTexture, false, Vector4.one, Vector4.zero);

                worker.Execute(tensor).FlushSchedule(true);
                Tensor result = worker.PeekOutput("output0");

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
                    GenerateBoundingBox(box);
                }

                tensor.Dispose();
                result.Dispose();
            }
            yield return null;
        }
    }

    private void CropTexture(WebCamTexture sourceTexture, int cropWidth, int cropHeight)
    {
        int centerX = sourceTexture.width / 2 - cropWidth / 2;
        int centerY = sourceTexture.height / 2 - cropHeight / 2;
        croppedTexture.SetPixels(sourceTexture.GetPixels(centerX, centerY, cropWidth, cropHeight));
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
        return new BoundingBox
        {
            X = tensor[0, 0, row, 0],
            Y = tensor[0, 0, row, 1],
            Width = tensor[0, 0, row, 2],
            Height = tensor[0, 0, row, 3]
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

    private void GenerateBoundingBox(DetectionResult det)
    {
        float zDistance = 2.0f;

        float xMin = (det.Bbox.X - det.Bbox.Width / 2) * cameraResolutionWidth;
        float yMin = (det.Bbox.Y - det.Bbox.Height / 2) * cameraResolutionHeight;
        float xMax = (det.Bbox.X + det.Bbox.Width / 2) * cameraResolutionWidth;
        float yMax = (det.Bbox.Y + det.Bbox.Height / 2) * cameraResolutionHeight;

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
                if (text != null)
                {
                    text.text = $"{det.Label}";
                }

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

    private Vector3 ImageToWorldPosition(float xNorm, float yNorm, float zDistance)
    {
        Vector3 screenPos = new Vector3(Mathf.Clamp01(xNorm) * cameraResolutionWidth, (1 - Mathf.Clamp01(yNorm)) * cameraResolutionHeight, zDistance);
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
