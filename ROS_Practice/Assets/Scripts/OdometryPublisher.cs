using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Tf2;
using RosMessageTypes.Geometry;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;
using System;
using RosMessageTypes.Nav;


public class OdometryPublisher : MonoBehaviour
{
    public string topicName = "/odom";
    public string frameId = "odom";
    public string childFrameId = "base_link";

    private ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<OdometryMsg>(topicName);
    }

    void Update()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        TimeMsg currentTime = TFPublisher.GetROSTimestamp(); // Àç»ç¿ë

        OdometryMsg msg = new OdometryMsg
        {
            header = new HeaderMsg
            {
                frame_id = frameId,
                stamp = currentTime
            },
            child_frame_id = childFrameId,
            pose = new PoseWithCovarianceMsg
            {
                pose = new PoseMsg
                {
                    position = new PointMsg(pos.x, pos.y, pos.z),
                    orientation = new QuaternionMsg(rot.x, rot.y, rot.z, rot.w)
                }
            },
            twist = new TwistWithCovarianceMsg()
        };

        ros.Publish(topicName, msg);
    }
}