﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    ///     Gets the X component of a <see cref="Vector2" /> cast to an int.
    /// </summary>
    /// <param name="vector2">
    ///     The 2D vector.
    /// </param>
    /// <returns>
    ///     The X component of a <see cref="Vector2" /> cast to an int.
    /// </returns>
    public static int XInt(this Vector2 vector2)
    {
        return (int) vector2.x;
    }

    /// <summary>
    ///     Gets the Y component of a <see cref="Vector2" /> cast to an int.
    /// </summary>
    /// <param name="vector2">
    ///     The 2D vector.
    /// </param>
    /// <returns>
    ///     The Y component of a <see cref="Vector2" /> cast to an int.
    /// </returns>
    public static int YInt(this Vector2 vector2)
    {
        return (int)vector2.y;
    }

    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }

    /// <summary>
    ///     Clears the contents of the string builder.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="StringBuilder"/> to clear.
    /// </param>
    public static void Clear(this StringBuilder value)
    {
        value.Length = 0;
        value.Capacity = 0;
    }

    public static void SetDefaultScale(this RectTransform trans)
    {
        trans.localScale = new Vector3(1, 1, 1);
    }
    public static void SetAnchorTopLeft(this RectTransform trans)
    {
        Vector2 vec = new Vector2(0, 1);
        trans.anchorMin = vec;
        trans.anchorMax = vec;
    }
    public static void SetAnchorBotLeft(this RectTransform trans)
    {
        Vector2 vec = new Vector2(0, 0);
        trans.anchorMin = vec;
        trans.anchorMax = vec;
    }
    public static void SetAnchorTopRight(this RectTransform trans)
    {
        Vector2 vec = new Vector2(1, 1);
        trans.anchorMin = vec;
        trans.anchorMax = vec;
    }
    public static void SetAnchorMidTop(this RectTransform trans)
    {
        Vector2 vec = new Vector2(.5f, 1);
        trans.anchorMin = vec;
        trans.anchorMax = vec;
    }

    public static Vector2 GetSize(this RectTransform trans)
    {
        return trans.rect.size;
    }
    public static float GetWidth(this RectTransform trans)
    {
        return trans.rect.width;
    }
    public static float GetHeight(this RectTransform trans)
    {
        return trans.rect.height;
    }

    public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
    }

    public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }
    public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }
    public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }
    public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }

    public static void SetSize(this RectTransform trans, Vector2 newSize)
    {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }
    public static void SetWidth(this RectTransform trans, float newSize)
    {
        SetSize(trans, new Vector2(newSize, trans.rect.size.y));
    }
    public static void SetHeight(this RectTransform trans, float newSize)
    {
        SetSize(trans, new Vector2(trans.rect.size.x, newSize));
    }
}
