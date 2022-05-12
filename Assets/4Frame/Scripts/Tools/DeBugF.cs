using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;



/// <summary>
/// A Wrapper Static Class for UnityEngine.Debug
/// </summary>
public static class DebugF
{
    /// <summary>
    /// Used by GetFrame -> outtrace # frame and get Reflections info. 
    /// In this case, magic number 3 will go just out of class DebugSTD
    /// </summary>
    private static int stackTraceNum = 3;

    private static string classNameColor = "#09F7F7";
    private static string methodNameColor = "yellow";
    private static string gameObjectNameColor = "#38F709";

    #region BASIC

    /// <summary>
    /// Generate LogMessage With Classname and MethodName Automatically
    /// </summary>
    /// <param name="_message"></param>
    /// <param name="_gameObject"></param>
    public static void Log(object _message, GameObject _gameObject = null)
    {
        string message = GenerateLogMessage(_message, _gameObject);
        Debug.Log(message);
    }

    /// <summary>
    /// Quick Generate Empty Message For FLAGCHECK
    /// </summary>
    public static void Log()
    {
        string message = GenerateLogMessage("", null);
        Debug.Log(message);
    }

    public static void LogError(object _message, GameObject _gameObject = null)
    {
        string message = GenerateLogMessage(_message, _gameObject);
        Debug.LogError(message);
    }

    public static void LogWarning(object _message, GameObject _gameObject = null)
    {
        string message = GenerateLogMessage(_message, _gameObject);
        Debug.LogWarning(message);
    }


    #endregion

    private static string GenerateLogMessage(object _message, GameObject _gameObject = null)
    {
        if (_gameObject == null)
        {
            return string.Format("{0} -> {1}",
                GetMethodBaseInfo(),
                _message.ToString());
        }

        else
        {
            return string.Format("{0} -> {1}: {2}",
                GetMethodBaseInfo(),
                GetGameObjectInfo(_gameObject),
                _message.ToString());
        }
    }

    private static string GetMethodBaseInfo()
    {
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
        MethodBase methodBase = stackTrace.GetFrame(stackTraceNum).GetMethod();

        string methodName = methodBase.Name;
        string className = methodBase.DeclaringType.Name;
        
        return string.Format("[{0}].{1}",
            className.ToRichTextColor(classNameColor).ToRichTextBold(),
            methodName.ToRichTextColor(methodNameColor).ToRichTextItalic());

    }

    private static string GetGameObjectInfo(GameObject _gameObject)
    {
        return _gameObject.name.ToRichTextColor(gameObjectNameColor);        
    }


}
