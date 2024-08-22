using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Resources에 있는 csv 파일을 읽어서 List<Dictionary<string, object>> 타입으로 준다
/// </summary>
public class CSVReader
{
    static string _splitRe = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string _lineSplitRe = @"\r\n|\n\r|\n|\r";
    static char[] _trimChars = { '\"' };

    public static List<Dictionary<string, object>> Read(string csvFileName)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = ResourceManager.Instance.Load<TextAsset>(csvFileName);

        var lines = Regex.Split(data.text, _lineSplitRe);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], _splitRe);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], _splitRe);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(_trimChars).TrimEnd(_trimChars).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}
