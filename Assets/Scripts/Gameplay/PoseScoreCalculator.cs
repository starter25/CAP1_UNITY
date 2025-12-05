using System;
using System.Collections.Generic;
using UnityEngine;

public static class PoseComparer
{
    // JSON 구조 정의
    [Serializable]
    private class JointJson
    {
        public string name;
        public float angle;
    }

    [Serializable]
    private class PoseJson
    {
        public string pose_name;
        public JointJson[] joints;
    }

    // 게임에서 사용할 구조
    [Serializable]
    public class RefPoseData
    {
        public string pose_name;
        public Dictionary<string, float> joints;
    }

    // JSON → RefPoseData 변환
    public static RefPoseData LoadRefPose(TextAsset jsonAsset)
    {
        if (jsonAsset == null || string.IsNullOrEmpty(jsonAsset.text))
            return null;

        PoseJson raw = JsonUtility.FromJson<PoseJson>(jsonAsset.text);

        if (raw == null || raw.joints == null || raw.joints.Length == 0)
            return null;

        var dict = new Dictionary<string, float>();

        foreach (var j in raw.joints)
        {
            if (!string.IsNullOrEmpty(j.name))
                dict[j.name] = j.angle;
        }

        return new RefPoseData
        {
            pose_name = raw.pose_name,
            joints = dict
        };
    }

    // 비교 함수
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
            string joint = kv.Key;
            float refAngle = kv.Value;

            if (!current.TryGetValue(joint, out float curAngle))
                continue;

            float diff = Mathf.Abs(curAngle - refAngle);
            float score01 = Mathf.Clamp01(1f - diff / tolerance);

            float weight = GetWeight(joint);

            sum += score01 * weight;
            totalWeight += weight;
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
