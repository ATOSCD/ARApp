using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
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

    public Material boundingBoxMaterial; // Material을 public으로 선언

    private Matrix4x4 camera2WorldMatrix;
    private Matrix4x4 projectionMatrix;

    void Start()
    {
        // Initialize WebCamTexture
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

        // Load the model and create a worker
        runtimeModel = ModelLoader.Load(model);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, runtimeModel);

        // Initialize other components
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

        StartCoroutine(DetectWebcam());
    }

    private void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }

        if (worker != null)
        {
            worker.Dispose();
        }
    }

    public IEnumerator DetectWebcam()
    {
        while (true)
        {
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                // Crop and resize the texture
                CropTexture(webCamTexture, inferenceImgSize, inferenceImgSize);

                // Create a tensor from the cropped texture
                var tensor = new Tensor(croppedTexture, false, Vector4.one, Vector4.zero);

                // Run inference
                worker.Execute(tensor).FlushSchedule(true);
                Tensor result = worker.PeekOutput("output0");

                // Parse the YOLO output
                var boxes_tmp = new List<DetectionResult>();
                ParseYoloOutput(result, confidenceThreshold, boxes_tmp);
                boxes = NonMaxSuppression(0.5f, boxes_tmp);

                // Update UI
                if (textMesh != null)
                {
                    textMesh.text = $"Boxes: {boxes.Count}";
                }

                // Clear previous labels
                foreach (var (go, r) in labels)
                {
                    Destroy(r);
                    Destroy(go);
                }
                labels.Clear();

                // Generate bounding boxes
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
            {
                continue;
            }

            BoundingBox box = ExtractBoundingBox(tensor, i);
            var labelName = names.map[label];
            boxes.Add(new DetectionResult
            {
                Bbox = box,
                Confidence = confidence,
                Label = labelName,
                LabelIdx = label
            });

            Debug.Log($"Detected: {labelName}, Confidence: {confidence:F2}, BBox: [X: {box.X}, Y: {box.Y}, Width: {box.Width}, Height: {box.Height}]");
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

    private List<DetectionResult> NonMaxSuppression(float threshold, List<DetectionResult> boxes)
    {
        var results = new List<DetectionResult>();
        if (boxes.Count == 0)
        {
            return results;
        }
        var detections = boxes.OrderByDescending(b => b.Confidence).ToList();
        results.Add(detections[0]);

        for (int i = 1; i < detections.Count; i++)
        {
            bool add = true;
            for (int j = 0; j < results.Count; j++)
            {
                float iou = IoU(detections[i].Rect, results[j].Rect);
                if (iou > threshold)
                {
                    add = false;
                    break;
                }
            }
            if (add)
                results.Add(detections[i]);
        }

        return results;
    }

    private float IoU(Rect boundingBoxA, Rect boundingBoxB)
    {
        float intersectionArea = Mathf.Max(0, Mathf.Min(boundingBoxA.xMax, boundingBoxB.xMax) - Mathf.Max(boundingBoxA.xMin, boundingBoxB.xMin)) *
                                 Mathf.Max(0, Mathf.Min(boundingBoxA.yMax, boundingBoxB.yMax) - Mathf.Max(boundingBoxA.yMin, boundingBoxB.yMin));

        float unionArea = boundingBoxA.width * boundingBoxA.height + boundingBoxB.width * boundingBoxB.height - intersectionArea;

        if (unionArea == 0)
        {
            return 0;
        }

        return intersectionArea / unionArea;
    }

    private void GenerateBoundingBox(DetectionResult det)
    {
        float zDistance = 2.0f; // 카메라로부터 적당히 떨어진 거리 (조정 가능)

        // YOLO 좌표에서 Unity 월드 좌표로 변환하기 위해 픽셀 좌표로 변경
        float xMin = (det.Bbox.X - det.Bbox.Width / 2);
        float yMin = (det.Bbox.Y - det.Bbox.Height / 2);
        float xMax = (det.Bbox.X + det.Bbox.Width / 2);
        float yMax = (det.Bbox.Y + det.Bbox.Height / 2);

        // 각 모서리를 Unity 월드 좌표로 변환
        Vector3 worldBL = ImageToWorldPosition(xMin, yMin, zDistance);
        Vector3 worldTL = ImageToWorldPosition(xMin, yMax, zDistance);
        Vector3 worldTR = ImageToWorldPosition(xMax, yMax, zDistance);
        Vector3 worldBR = ImageToWorldPosition(xMax, yMin, zDistance);

        // Bounding box 생성
        GameObject boxGO = new GameObject("BoundingBox");
        LineRenderer lr = boxGO.AddComponent<LineRenderer>();
        lr.widthMultiplier = 0.005f;
        lr.loop = true;
        lr.positionCount = 4;

        if (boundingBoxMaterial != null)
            lr.material = boundingBoxMaterial;
        else
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
            Debug.LogWarning("BoundingBox Material not assigned, using default material.");
        }

        lr.material.color = colors.map[det.LabelIdx];

        lr.SetPosition(0, worldBL);
        lr.SetPosition(1, worldTL);
        lr.SetPosition(2, worldTR);
        lr.SetPosition(3, worldBR);

        labels.Add(Tuple.Create(boxGO, (Renderer)lr));

        // Label 텍스트 생성
        GameObject textGO = new GameObject("LabelText");
        textGO.transform.position = worldTL + new Vector3(0, 0.05f, 0);
        textGO.transform.LookAt(Camera.main.transform);
        textGO.transform.Rotate(0, 180f, 0);

        TextMeshPro tm = textGO.AddComponent<TextMeshPro>();
        tm.text = $"{det.Label} ({det.Confidence:F2})";
        tm.fontSize = 0.1f;
        tm.alignment = TextAlignmentOptions.Center;
        tm.color = colors.map[det.LabelIdx];

        labels.Add(Tuple.Create(textGO, textGO.GetComponent<Renderer>()));
    }

    private Vector3 ImageToWorldPosition(float xNorm, float yNorm, float zDistance)
    {
        Vector3 screenPos = new Vector3(
            Mathf.Clamp01(xNorm) * cameraResolutionWidth,
            (1 - Mathf.Clamp01(yNorm)) * cameraResolutionHeight,
            zDistance
        );
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

}