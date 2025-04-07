using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using UnityEngine;

public class CmdVelSubscriber : MonoBehaviour
{
    public string topicName = "/cmd_vel";
    public float linearScale = 1.0f;
    public float angularScale = 1.0f;

    private Vector3 linear;
    private float angular;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>(topicName, CmdVelCallback);
    }

    void CmdVelCallback(TwistMsg msg)
    {
        linear = new Vector3((float)msg.linear.x, 0, (float)msg.linear.y);
        angular = (float)msg.angular.z;
    }

    void Update()
    {
        transform.Translate(linear * linearScale * Time.deltaTime);
        transform.Rotate(Vector3.up * angular * angularScale * Time.deltaTime);
    }
}
