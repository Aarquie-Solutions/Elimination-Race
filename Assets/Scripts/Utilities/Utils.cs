using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

public static class Utils
{
    public static void SetActive(this Transform tr, bool value)
    {
        tr.gameObject.SetActive(value);
    }

    public static void Swap<T>(ref List<T> list, int indexA, int indexB)
    {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }

    public static int MoveTowards(int current, int target, int maxDelta = 1)
    {
        int delta = target - current;
        if (Mathf.Abs(delta) <= maxDelta)
        {
            return target;
        }
        return current + Mathf.Clamp(delta, -maxDelta, maxDelta);
    }

    public static T GetRandomElement<T>(this List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static void SwapByElement<T>(ref List<T> list, T elementA, T elementB)
    {
        int indexA = list.IndexOf(elementA);
        int indexB = list.IndexOf(elementB);

        if (indexA == -1 || indexB == -1)
            throw new ArgumentException("One or both elements not found in the list.");

        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }

    public static void SwapElementWithIndex<T>(ref List<T> list, T elementA, int indexB)
    {
        int indexA = list.IndexOf(elementA);

        if (indexA == -1)
            throw new ArgumentException("Element not found in the list.", nameof(elementA));

        if (indexB < 0 || indexB >= list.Count)
            throw new ArgumentOutOfRangeException(nameof(indexB), "Index is out of range.");

        // Swap the elements
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }

    public static float GenerateRandomNumber(float _min, float _max)
    {
        return UnityEngine.Random.Range(_min, _max);
    }

    public static int GenerateRandomNumber(int _min, int _max)
    {
        return UnityEngine.Random.Range(_min, _max);
    }

    public static string GenerateRandomIdentifier()
    {
        Guid randomGuid = Guid.NewGuid();
        return randomGuid.ToString();
    }

    public static string ToHex(Color color)
    {
        Color32 color32 = color;
        return $"{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
    }

    public static Color FromHex(string hex)
    {
        if (hex.Length != 8)
        {
            return Color.white;
        }

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        // Parse A component if present (hex string length 8)
        byte a = 255; // Default to full opacity
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }

        // Convert byte values (0-255) to float values (0-1) for Unity Color
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static bool TryParseFloatField(this TMPro.TMP_InputField field, ref float settingField)
    {
        if (float.TryParse(field.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
        {
            settingField = result;
            return true;
        }
        return false;
    }

    public static bool TryParseIntField(this TMPro.TMP_InputField field, ref int settingField)
    {
        if (int.TryParse(field.text, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
        {
            settingField = result;
            return true;
        }
        return false;
    }

    public static void LogCloud(this string msg, GameObject go = null)
    {
        Debug.Log($"<color=white>[Cloud]:{msg}</color>", go);
    }

    public static void LogWarningCloud(this string msg, GameObject go = null)
    {
        Debug.Log($"<color=yellow>[Cloud]:{msg}</color>", go);
    }

    public static void Log(this string msg, GameObject go = null)
    {
        Debug.Log($"<color=white>[V]:{msg}</color>", go);
    }


    public static void LogWarning(this string msg, GameObject go = null)
    {
        // Debug.LogWarning($"<color=yellow>[V]:{msg}</color>", go);
        Debug.Log($"<color=yellow>[V]:{msg}</color>", go);
    }

    public static void LogError(this string msg, GameObject go = null)
    {
        // Debug.LogError($"<color=red>[V]:{msg}</color>", go);
        Debug.Log($"<color=red>[V]:{msg}</color>", go);
    }

    public static bool NullCheck<T>(this T obj, string label = null)
    {
        // Special handling for UnityEngine.Object which has custom == operator
        if (obj is Object unityObj)
        {
            if (unityObj == null)
            {
                ($"{label ?? "Object"} is NULL (Unity Object)").LogWarning();
                return true;
            }

            ($"{label ?? "Object"} is NOT null. Name: {unityObj.name}").Log();
            return false;
        }

        // Handle regular reference or value types
        if (obj == null || obj.Equals(default(T)))
        {
            ($"{label ?? "Object"} is NULL or default").LogWarning();
            return true;
        }
        else
        {
            ($"{label ?? "Object"} is NOT null. Value: {obj.ToString()}").Log();
            return false;
        }
    }

    public static float DistanceXZ(this Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        return Vector3.Distance(a, b);
    }
}

public static class FastShuffler
{
    /// <summary>
    /// Generates a shuffled list of numbers from 1 to n.
    /// Optionally fixes a specific number at a specific index if both parameters are >= 0.
    /// </summary>
    public static int[] Generate(int n, int fixedNumber = -1, int fixedIndex = -1)
    {
        if (n < 1)
            throw new ArgumentException("n must be greater than 0.");

        bool hasFixed = fixedNumber > 0 && fixedIndex >= 0;

        if (hasFixed)
        {
            if (fixedNumber < 1 || fixedNumber > n)
                throw new ArgumentOutOfRangeException(nameof(fixedNumber), "Fixed number must be in range 1 to n.");
            if (fixedIndex >= n)
                throw new ArgumentOutOfRangeException(nameof(fixedIndex), "Fixed index must be within list bounds.");
        }

        // Create full list from 1 to n
        List<int> numbers = new List<int>();
        for (int i = 1; i <= n; i++)
        {
            numbers.Add(i);
        }

        // If fixedNumber is required, remove it temporarily before shuffling
        if (hasFixed)
            numbers.Remove(fixedNumber);

        // Shuffle the remaining numbers
        Random rng = new Random();
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }
        numbers.Shuffle();

        // Insert fixedNumber at the correct index
        if (hasFixed)
            numbers.Insert(fixedIndex, fixedNumber);

        return numbers.ToArray();
    }

    public static void Shuffle<T>(this List<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
