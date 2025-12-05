using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpPoseReceiver : MonoBehaviour
{
    [Header("UDP Settings")]
    public int listenPort = 5006;
    public bool debugLogJson = false;

    UdpClient udpClient;
    Thread receiveThread;
    bool isRunning = false;

    readonly object lockObj = new object();
    string latestJson = null;
    bool hasNewJson = false;

    public Dictionary<string, float> LatestAngles { get; private set; }
        = new Dictionary<string, float>();

    public bool HasValidPose { get; private set; } = false;

    [Serializable]
    private class AnglesPacket
    {
        public float left_elbow;
        public float right_elbow;
        public float left_shoulder;
        public float right_shoulder;
        public float left_knee;
        public float right_knee;
        public float left_leg_spread;
        public float right_leg_spread;
    }

    void Start()
    {
        try
        {
            udpClient = new UdpClient(listenPort);
            isRunning = true;

            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log($"[UdpPoseReceiver] UDP 수신 시작 (port {listenPort})");
        }
        catch (Exception e)
        {
            Debug.LogError("[UdpPoseReceiver] UDP 시작 실패: " + e.Message);
        }
    }

    void ReceiveLoop()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            while (isRunning)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string json = Encoding.UTF8.GetString(data);

                lock (lockObj)
                {
                    latestJson = json;
                    hasNewJson = true;
                }
            }
        }
        catch
        {
            // 종료 과정에서 나는 예외는 무시
        }
    }

    void Update()
    {
        string jsonToProcess = null;

        lock (lockObj)
        {
            if (hasNewJson)
            {
                jsonToProcess = latestJson;
                hasNewJson = false;
            }
        }

        if (!string.IsNullOrEmpty(jsonToProcess))
            ProcessJson(jsonToProcess);
    }

    void ProcessJson(string json)
    {
        if (debugLogJson)
            Debug.Log("[UdpPoseReceiver] 받은 JSON : " + json);

        try
        {
            AnglesPacket packet = JsonUtility.FromJson<AnglesPacket>(json);

            if (packet != null)
            {
                if (LatestAngles == null)
                    LatestAngles = new Dictionary<string, float>();

                LatestAngles["left_elbow"]       = packet.left_elbow;
                LatestAngles["right_elbow"]      = packet.right_elbow;
                LatestAngles["left_shoulder"]    = packet.left_shoulder;
                LatestAngles["right_shoulder"]   = packet.right_shoulder;
                LatestAngles["left_knee"]        = packet.left_knee;
                LatestAngles["right_knee"]       = packet.right_knee;
                LatestAngles["left_leg_spread"]  = packet.left_leg_spread;
                LatestAngles["right_leg_spread"] = packet.right_leg_spread;

                HasValidPose = true;
            }
            else
            {
                HasValidPose = false;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UdpPoseReceiver] JSON 파싱 실패: " + e.Message);
            HasValidPose = false;
        }
    }

    // 🔥 공통 정리 함수
    private void StopUdp()
    {
        isRunning = false;

        try
        {
            if (udpClient != null)
            {
                udpClient.Close();   // 포트 반환
                udpClient = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UdpPoseReceiver] udpClient.Close 예외: " + e.Message);
        }

        try
        {
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Abort();
                receiveThread = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UdpPoseReceiver] receiveThread.Abort 예외: " + e.Message);
        }
    }

    void OnDestroy()
    {
        // 씬이 바뀌거나 오브젝트가 파괴될 때도 반드시 정리
        StopUdp();
    }

    void OnApplicationQuit()
    {
        // 앱 완전 종료 시에도 안전하게 한 번 더 정리
        StopUdp();
    }
}
