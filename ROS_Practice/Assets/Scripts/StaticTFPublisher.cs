using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.Tf2;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class StaticTFPublisher : MonoBehaviour
{
    private ROSConnection ros;
    public string topicName = "/tf";

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TFMessageMsg>(topicName);
    }

    void Update()
    {
        SendTF(); // 매 프레임 퍼블리시 (혹은 일정 주기로도 가능)
    }

    void SendTF()
    {
        // frame_id: "map" -> base_link
        TransformStampedMsg tf = new TransformStampedMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg
            {
                frame_id = "map",
                stamp = GetROSTimestamp()
            },
            child_frame_id = "base_link",
            transform = new TransformMsg
            {
                translation = new Vector3Msg(0, 0, 0),
                rotation = new QuaternionMsg(0, 0, 0, 1)
            }
        };


        TFMessageMsg tfMessage = new TFMessageMsg(new[] { tf });

        ros.Publish(topicName, tfMessage);
    }

    TimeMsg GetROSTimestamp()
    {
        var now = DateTimeOffset.UtcNow;
        return new TimeMsg
        {
            sec = (int)now.ToUnixTimeSeconds(),
            nanosec = (uint)(now.ToUnixTimeMilliseconds() % 1000 * 1e6)
        };
    }


}
