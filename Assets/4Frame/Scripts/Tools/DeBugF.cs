using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class DebugF : MonoBehaviour
{
    /// <summary>
    /// Used by GetFrame -> outtrace # frame and get Reflections info. 
    /// In this case, magic number 3 will go just out of class DebugSTD
    /// </summary>
    private static int stackTraceNum = 3;

    private static string classNameColor = "#09F7F7";
    private static string methodNameColor = "yellow";
    private static string gameObjectNameColor = "#38F709";
    private static string signalColor = "#FFC0CB";
    private static string stateColor = "#FF8C00";

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

    #region STATE CHANGE


    public static void Log(string before, string after, GameObject _gameObject = null)
    {
        string message = GenerateStateChangeMessage(before, after, _gameObject);
        Debug.Log(message);
    }




    #endregion


    private static string GenerateLogMessage(object _message, GameObject _gameObject = null)
    {
        string message = "Default Message";
        if (_gameObject == null)
        {
            message = string.Format("{0} -> {1}",
                GenerateBaseMethodFullInfo(),
                _message.ToString());
        }

        else
        {
            message = string.Format("{0} -> {1}: {2}",
                GenerateBaseMethodFullInfo(),
                GenerateGameObjectInfo(_gameObject),
                _message.ToString());
        }

        return message;
    }


    private static string GenerateStateChangeMessage(string _before, string _after, GameObject _gameObject = null)
    {
        string stateChangeText = GetColoredString("STATE CHANGE", signalColor);
        string before = GetColoredString(_before, stateColor);
        string after = GetColoredString(_after, stateColor);

        string message = "";

        if (_gameObject == null)
        {
            message = string.Format("{0} -> {1}: {2} -> {3}",
                GenerateBaseMethodFullInfo(),
                stateChangeText,
                before,
                after);
        }

        else
        {
            message = string.Format("{0} -> {1}: {2}: {3} -> {4}",
                GenerateBaseMethodFullInfo(),
                stateChangeText,
                GenerateGameObjectInfo(_gameObject),
                before,
                after);
        }


        return message;
    }

    private static string GenerateBaseMethodFullInfo()
    {
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
        MethodBase methodBase = stackTrace.GetFrame(stackTraceNum).GetMethod();

        string methodName = methodBase.Name;
        string className = methodBase.DeclaringType.Name;

        return string.Format("[<b><color={0}>{1}</color></b>].<i><color={2}>{3}</color></i>",
            classNameColor, className,
            methodNameColor, methodName);
    }

    private static string GenerateGameObjectInfo(GameObject _gameObject)
    {
        return string.Format("[<color={0}>{1}</color>]",
            gameObjectNameColor, _gameObject.name);
    }


    private static string GetColoredString(string _message, string _color)
    {
        string message = string.Format("<color={0}>{1}</color>",
            _color,
            _message);
        return message;
    }
}
