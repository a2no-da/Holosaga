using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public static class CsvData
{
    public static List<Dictionary<string, string>> Read(string fileName, char delimiter = ',')
    {
        var result = new List<Dictionary<string, string>>();

        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("File not found: " + fileName);
            return result;
        }

        var lines = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        if (lines.Length > 0)
        {
            var headers = ParseCsvLine(lines[0], delimiter);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var values = ParseCsvLine(lines[i], delimiter);

                if (values.Length != headers.Length)
                {
                    Array.Resize(ref values, headers.Length);
                }

                var entry = new Dictionary<string, string>();

                for (int j = 0; j < headers.Length; j++)
                {
                    entry[headers[j]] = j < values.Length ? values[j] : string.Empty;
                }

                result.Add(entry);
            }
        }

        return result;
    }

    private static string[] ParseCsvLine(string line, char delimiter)
    {
        var values = new List<string>();
        var value = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
            }
            else if (c == delimiter && !inQuotes)
            {
                values.Add(value.ToString());
                value.Clear();
            }
            else
            {
                value.Append(c);
            }
        }

        values.Add(value.ToString());

        return values.ToArray();
    }
}