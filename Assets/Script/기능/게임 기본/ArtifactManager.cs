using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

[System.Serializable]
public class ArtifactList
{
    public List<ArtifactData> artifacts; 

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static ArtifactList FromJson(string json)
    {
        return JsonUtility.FromJson<ArtifactList>(json);
    }
}

[System.Serializable]
public class ArtifactData 
{
    public Grade grade;
    public Type type;
    public int index;
    public int Quantity;
}

public class ArtifactManager : MonoBehaviour
{
    public Artifact[] artifacts;
    private string path;
    private CultureInfo currentCulture;

    private void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "artifact.json");
        currentCulture = CultureInfo.CurrentCulture;
    }

    public void SaveArtifact()
    {
        ArtifactList artifactList = new ArtifactList { artifacts = new List<ArtifactData>() };

        foreach (var artifact in artifacts)
        {
            ArtifactData data = new ArtifactData
            {
                grade = artifact.grade,
                type = artifact.type,
                index = artifact.index,
                Quantity = artifact.Quantity
            };
            artifactList.artifacts.Add(data);
        }

        string json = artifactList.ToJson();
        File.WriteAllText(path, json, Encoding.UTF8);
    }

    public void LoadArtifact()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path, Encoding.UTF8);
            ArtifactList artifactList = ArtifactList.FromJson(json);

            for (int i = 0; i < artifactList.artifacts.Count && i < artifacts.Length; i++)
            {
                artifacts[i].grade = artifactList.artifacts[i].grade;
                artifacts[i].type = artifactList.artifacts[i].type;
                artifacts[i].index = artifactList.artifacts[i].index;
                artifacts[i].Quantity = artifactList.artifacts[i].Quantity;

                if (i < artifacts.Length)
                {
                    char gradeChar = artifacts[i].artifactId[1];
                    string numberPart = artifacts[i].artifactId.Substring(2);

                    switch (gradeChar)
                    {
                        case 'n':
                            artifacts[i].grade = Grade.Normal;
                            break;
                        case 'e':
                            artifacts[i].grade = Grade.Epic;
                            break;
                        case 'u':
                            artifacts[i].grade = Grade.Unique;
                            break;
                        default:
                            artifacts[i].grade = Grade.Normal;
                            break;
                    }

                    if (int.TryParse(numberPart, out int index))
                    {
                        artifacts[i].index = index;
                    }
                }
            }
        }
        else
        {
            foreach (var artifact in artifacts)
            {
                artifact.Quantity = 0;
            }

            SaveArtifact();
        }
    }
}
