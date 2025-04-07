using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Tf2;
using RosMessageTypes.Geometry;
using System;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;

public class TFPublisher : MonoBehaviour
{
    private ROSConnection ros;
    public Transform baseLinkTransform;
    public Transform laserTransform;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TFMessageMsg>("/tf");
        ros.RegisterPublisher<TFMessageMsg>("/tf_static");

        PublishStaticTransform();
    }

    void Update()
    {
        TimeMsg currentTime = GetROSTimestamp();

        var mapToOdom = new TransformStampedMsg
        {
            header = new HeaderMsg { frame_id = "map", stamp = currentTime },
            child_frame_id = "odom",
            transform = new TransformMsg
            {
                translation = new Vector3Msg(0, 0, 0),
                rotation = new QuaternionMsg(0, 0, 0, 1)
            }
        };

        var odomToBase = new TransformStampedMsg
        {
            header = new HeaderMsg { frame_id = "odom", stamp = currentTime },
            child_frame_id = "base_link",
            transform = new TransformMsg
            {
                translation = new Vector3Msg(
                    baseLinkTransform.position.x,
                    baseLinkTransform.position.y,
                    baseLinkTransform.position.z
                ),
                rotation = new QuaternionMsg(
                    baseLinkTransform.rotation.x,
                    baseLinkTransform.rotation.y,
                    baseLinkTransform.rotation.z,
                    baseLinkTransform.rotation.w
                )
            }
        };

        Vector3 relativePos = baseLinkTransform.InverseTransformPoint(laserTransform.position);
        Quaternion relativeRot = Quaternion.Inverse(baseLinkTransform.rotation) * laserTransform.rotation;

        var baseToLaser = new TransformStampedMsg
        {
            header = new HeaderMsg { frame_id = "base_link", stamp = currentTime },
            child_frame_id = "laser",
            transform = new TransformMsg
            {
                translation = new Vector3Msg(relativePos.x, relativePos.y, relativePos.z),
                rotation = new QuaternionMsg(relativeRot.x, relativeRot.y, relativeRot.z, relativeRot.w)
            }
        };

        ros.Publish("/tf", new TFMessageMsg(new[] { mapToOdom, odomToBase, baseToLaser }));
    }

    void PublishStaticTransform()
    {
        TimeMsg currentTime = GetROSTimestamp();

        Vector3 relativePos = baseLinkTransform.InverseTransformPoint(laserTransform.position);
        Quaternion relativeRot = Quaternion.Inverse(baseLinkTransform.rotation) * laserTransform.rotation;

        var baseToLaser = new TransformStampedMsg
        {
            header = new HeaderMsg { frame_id = "base_link", stamp = currentTime },
            child_frame_id = "laser",
            transform = new TransformMsg
            {
                translation = new Vector3Msg(relativePos.x, relativePos.y, relativePos.z),
                rotation = new QuaternionMsg(relativeRot.x, relativeRot.y, relativeRot.z, relativeRot.w)
            }
        };

        ros.Publish("/tf_static", new TFMessageMsg(new[] { baseToLaser }));
    }


    public static TimeMsg GetROSTimestamp()
    {
        DateTime now = DateTime.UtcNow;
        TimeSpan epoch = now - new DateTime(1970, 1, 1);
        return new TimeMsg
        {
            sec = (int)epoch.TotalSeconds,
            nanosec = (uint)((epoch.TotalSeconds - (int)epoch.TotalSeconds) * 1e9)
        };
    }
}
