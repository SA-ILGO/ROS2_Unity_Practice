using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using System;
using System.Collections.Generic;

public class SimulatedPointCloud : MonoBehaviour
{
    public string topicName = "/pointcloud";
    public float range = 10f;
    public int horizontalResolution = 360;
    public int verticalResolution = 16;

    private ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PointCloud2Msg>(topicName);
    }

    void Update()
    {
        List<byte> data = new List<byte>();
        int pointCount = 0;

        for (int v = 0; v < verticalResolution; v++)
        {
            float vAngle = Mathf.Lerp(-15, 15, v / (float)(verticalResolution - 1));

            for (int h = 0; h < horizontalResolution; h++)
            {
                float hAngle = h * 2 * Mathf.PI / horizontalResolution;
                Vector3 dir = Quaternion.Euler(vAngle, Mathf.Rad2Deg * hAngle, 0) * Vector3.forward;
                //Debug.DrawRay(transform.position, dir * range, Color.red, 0.1f);

                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, range))
                {
                    Vector3 local = transform.InverseTransformPoint(hit.point);
                    float rosX = local.z;
                    float rosY = -local.x;
                    float rosZ = local.y;

                    byte[] x = BitConverter.GetBytes(rosX);
                    byte[] y = BitConverter.GetBytes(rosY);
                    byte[] z = BitConverter.GetBytes(rosZ);
                    data.AddRange(x); data.AddRange(y); data.AddRange(z);
                    pointCount++;
                }
            }
        }

        PointCloud2Msg msg = new PointCloud2Msg
        {
            header = new RosMessageTypes.Std.HeaderMsg
            {
                frame_id = "map"
            },
            height = 1,
            width = (uint)pointCount,
            point_step = 12,
            row_step = (uint)(pointCount * 12),
            is_bigendian = false,
            is_dense = true,
            fields = new[]
            {
                new PointFieldMsg("x", 0, 7, 1),
                new PointFieldMsg("y", 4, 7, 1),
                new PointFieldMsg("z", 8, 7, 1)
            },
            data = data.ToArray()
        };

        ros.Publish(topicName, msg);
    }
}
