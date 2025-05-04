using System.Collections.Generic;
using UnityEngine;
using System;


public class BoundingBox
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}

public class DetectionResult
{
    public BoundingBox Bbox { get; set; }
    public string Label { get; set; }
    public int LabelIdx { get; set; }
    public float Confidence { get; set; }

    public Rect Rect
    {
        get { return new Rect(Bbox.X, Bbox.Y, Bbox.Width, Bbox.Height); }
    }

    public override string ToString()
    {
        return $"{Label}:{Confidence}";
    }
}

public class COCONames
{
    public List<String> map;

    public COCONames()
    {
        map = new List<string>(){
    "air conditioner",
    "bed",
    "book",
    "chair",
    "clock",
    "door",
    "fan",
    "laptop",
    "mug",
    "thermometer",
    "tv",
    "window",
    "lamp",
    "tissue"
};

    }

}

public class COCOColors
{
    public List<Color> map;

    public COCOColors()
    {
        map = new List<Color>();
        for (var i = 0; i < 14; ++i)
        {
            map.Add(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }
        map[0] = new Color(255.0f, 0.0f, 127.0f);
    }
}

