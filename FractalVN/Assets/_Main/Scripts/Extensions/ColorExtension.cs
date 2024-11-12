using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
    public static Color GetColorByName(this Color original, string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red":
                return Color.red;
            case "green":
                return Color.green;
            case "blue":
                return Color.blue;
            case "white":
                return Color.white;
            case "black":
                return Color.black;
            case "gray":
                return Color.gray;
            case "yellow":
                return Color.yellow;
            case "cyan":
                return Color.cyan;
            case "magenta": 
                return Color.magenta;
            default:
                Debug.LogWarning($"Invalid color name '{colorName}'.");
                return Color.clear;
        }
    }
}
