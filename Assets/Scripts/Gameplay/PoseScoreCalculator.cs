using System;
using System.Collections.Generic;
using UnityEngine;

public static class PoseComparer
{
    [Serializable]
    public class RefPoseData
    {
        public string pose_name;
        public Dictionary<string, float> joints;
    }

    // JSON 파일에서 reference PoseData 읽기
    public static RefPoseData LoadRefPose(TextAsset json)
    {
        if (json == null) return null;
        return JsonUtility.FromJson<RefPoseData>(json.text);
    }

    // 현재 Pose(Dictionary) vs RefPoseData 비교
    public static float Compare(Dictionary<string, float> current,
                                RefPoseData reference,
                                float tolerance = 60f)
    {
        if (current == null || reference == null || reference.joints == null)
            return 0f;

        float totalWeight = 0f;
        float sum = 0f;

        foreach (var kv in reference.joints)
        {
            string key = kv.Key;
            float refVal = kv.Value;

            if (!current.ContainsKey(key))
                continue;

            float curVal = current[key];
            float diff = Mathf.Abs(curVal - refVal);

            float score01 = 1f - (diff / tolerance);
            score01 = Mathf.Clamp01(score01);

            float w = GetWeight(key);

            sum += score01 * w;
            totalWeight += w;
        }

        if (totalWeight == 0f) return 0f;

        return (sum / totalWeight) * 100f;
    }

    private static float GetWeight(string joint)
    {
        if (joint.Contains("shoulder")) return 1.5f;
        if (joint.Contains("elbow")) return 1.5f;
        if (joint.Contains("leg_spread")) return 1.2f;
        if (joint.Contains("knee")) return 1.0f;
        return 1.0f;
    }
}
