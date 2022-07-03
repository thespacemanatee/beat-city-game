﻿using System;
using UnityEngine;

namespace MoreMountains.Tools
{
	/// <summary>
	///     Time helpers
	/// </summary>
	public class MMTime : MonoBehaviour
    {
	    /// <summary>
	    ///     Turns a float (expressed in seconds) into a string displaying hours, minutes, seconds and hundredths optionnally
	    /// </summary>
	    /// <param name="t"></param>
	    /// <param name="displayHours"></param>
	    /// <param name="displayMinutes"></param>
	    /// <param name="displaySeconds"></param>
	    /// <param name="displayHundredths"></param>
	    /// <returns></returns>
	    public static string FloatToTimeString(float t, bool displayHours = false, bool displayMinutes = true,
            bool displaySeconds = true, bool displayMilliseconds = false)
        {
            var intTime = (int)t;
            var hours = intTime / 3600;
            var minutes = intTime / 60;
            var seconds = intTime % 60;
            var milliseconds = Mathf.FloorToInt(t * 1000 % 1000);

            if (displayHours && displayMinutes && displaySeconds && displayMilliseconds)
                return string.Format("{0:00}:{1:00}:{2:00}.{3:D3}", hours, minutes, seconds, milliseconds);
            if (!displayHours && displayMinutes && displaySeconds && displayMilliseconds)
                return string.Format("{0:00}:{1:00}.{2:D3}", minutes, seconds, milliseconds);
            if (!displayHours && !displayMinutes && displaySeconds && displayMilliseconds)
                return string.Format("{0:D2}.{1:D3}", seconds, milliseconds);
            if (!displayHours && !displayMinutes && displaySeconds && !displayMilliseconds)
                return string.Format("{0:00}", seconds);
            if (displayHours && displayMinutes && displaySeconds && !displayMilliseconds)
                return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            if (!displayHours && displayMinutes && displaySeconds && !displayMilliseconds)
                return string.Format("{0:00}:{1:00}", minutes, seconds);
            return null;
        }

	    /// <summary>
	    ///     Takes a hh:mm:ss:SSS string and turns it into a float value expressed in seconds
	    /// </summary>
	    /// <returns>a number of seconds.</returns>
	    /// <param name="timeInStringNotation">Time in string notation to decode.</param>
	    public static float TimeStringToFloat(string timeInStringNotation)
        {
            if (timeInStringNotation.Length != 12)
                throw new Exception(
                    "The time in the TimeStringToFloat method must be specified using a hh:mm:ss:SSS syntax");

            var timeStringArray = timeInStringNotation.Split(new[] { ":" }, StringSplitOptions.None);

            var startTime = 0f;
            float result;
            if (float.TryParse(timeStringArray[0], out result)) startTime += result * 3600f;
            if (float.TryParse(timeStringArray[1], out result)) startTime += result * 60f;
            if (float.TryParse(timeStringArray[2], out result)) startTime += result;
            if (float.TryParse(timeStringArray[3], out result)) startTime += result / 1000f;

            return startTime;
        }
    }
}