using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class SimulatedLidar : MonoBehaviour
{
    public int rays = 360;
    public float range = 10f;
    public string topicName = "/scan";

    private ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<LaserScanMsg>(topicName);
    }

    void Update()
    {
        TimeMsg currentTime = TFPublisher.GetROSTimestamp(); // Àç»ç¿ë

        LaserScanMsg scan = new LaserScanMsg
        {
            header = new HeaderMsg { frame_id = "laser", stamp = currentTime },
            angle_min = -Mathf.PI,
            angle_max = Mathf.PI,
            angle_increment = (2 * Mathf.PI) / rays,
            time_increment = Time.deltaTime / rays,
            scan_time = Time.deltaTime,
            range_min = 0.1f,
            range_max = range,
            ranges = new float[rays]
        };

        for (int i = 0; i < rays; i++)
        {
            float angle = scan.angle_min + i * scan.angle_increment;
            Vector3 localDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 worldDirection = transform.rotation * localDirection;

            if (Physics.Raycast(transform.position, worldDirection, out RaycastHit hit, range))
                scan.ranges[i] = hit.distance;
            else
                scan.ranges[i] = range;
        }

        ros.Publish(topicName, scan);
    }
}