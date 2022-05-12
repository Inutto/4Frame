using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// An Extentions Class for string to make rich text and other stuff.
/// </summary>
public static class StringFExtensions
{


    /// <summary>
    /// Return a rich text string with desinated color.
    /// </summary>
    /// <param name="_string"></param>
    /// <param name="_color"></param>
    /// <returns></returns>
    public static string ToRichTextColor(this string _string, Color _color)
    {
        var colorCode = ColorUtility.ToHtmlStringRGB(_color);
        return _string.ToRichTextColor(colorCode);
        
    }


    /// <summary>
    /// Return a rich text string with desinated color. _color should be in Hex Color Code like #008000 (Green)
    /// </summary>
    /// <param name="_string"></param>
    /// <param name="_color"></param>
    /// <returns></returns>
    public static string ToRichTextColor(this string _string, string _color)
    {
        string result = string.Format("<color={0}>{1}</color>",
          _color,
          _string);
        return result;
    }


    /// <summary>
    /// Return a rich text string in Bold.
    /// </summary>
    /// <param name="_string"></param>
    /// <returns></returns>
    public static string ToRichTextBold(this string _string)
    {
        return string.Format("<b>{0}</b>", _string);
    }


    /// <summary>
    /// Return a rich text string in Italic.
    /// </summary>
    /// <param name="_string"></param>
    /// <returns></returns>
    public static string ToRichTextItalic(this string _string)
    {
        return string.Format("<i>{0}</i>", _string);
    }
}
