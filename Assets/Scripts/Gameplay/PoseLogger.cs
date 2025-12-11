using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PoseLogger : MonoBehaviour
{
    public bool enableLogging = true;
    public string fileName = "pose_log.csv";

    string filePath;
    bool headerWritten = false;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log("[PoseLogger] 저장 경로: " + filePath);
    }

    public void LogPose(string poseName, string grade, Dictionary<string, float> angles)
    {
        if (!enableLogging) return;

        // 첫 줄 헤더 작성
        if (!headerWritten)
        {
            string header = "Time,PoseName,Grade";
            foreach (var key in angles.Keys)
                header += "," + key;

            File.WriteAllText(filePath, header + "\n");
            headerWritten = true;
        }

        // 데이터 라인 기록
        string line = Time.time.ToString("F3") + "," + poseName + "," + grade;
        foreach (var val in angles.Values)
            line += "," + val.ToString("F2");

        File.AppendAllText(filePath, line + "\n");
    }
}
