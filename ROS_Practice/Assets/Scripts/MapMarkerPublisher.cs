using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using RosMessageTypes.Visualization;
using System;
using RosMessageTypes.BuiltinInterfaces;



public class MapMarkerPublisher : MonoBehaviour
{
    private ROSConnection ros;
    public string topicName = "/visualization_marker";
    private int markerIdCounter = 0;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<MarkerMsg>(topicName);

        PublishMapMarkers();
    }

    void PublishMapMarkers()
    {
        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");
        Debug.Log($"[MapMarkerPublisher] Found {mapObjects.Length} objects with tag 'Map'");

        foreach (GameObject obj in mapObjects)
        {
            Debug.Log($"[MapMarkerPublisher] Publishing marker for: {obj.name}");
            PublishMarker(obj, markerIdCounter);
            markerIdCounter++;
        }
    }

    void PublishMarker(GameObject obj, int id)
    {
        TimeMsg timestamp = GetROSTimestamp();

        MarkerMsg marker = new MarkerMsg
        {
            header = new HeaderMsg
            {
                frame_id = "map",
                stamp = timestamp
            },
            ns = "map_objects",
            id = id,
            type = MarkerMsg.CUBE,
            action = MarkerMsg.ADD,
            pose = new PoseMsg
            {
                position = obj.transform.position.To<FLU>(),
                orientation = obj.transform.rotation.To<FLU>()
            },
            scale = new Vector3Msg(
                obj.transform.localScale.x,
                obj.transform.localScale.y,
                obj.transform.localScale.z),
            color = new ColorRGBAMsg
            {
                r = 1.0f,
                g = 0.0f,
                b = 0.0f,
                a = 1.0f
            },
            lifetime = new DurationMsg(0, 0) // forever
        };

        ros.Publish(topicName, marker);

        Debug.Log($"[MapMarkerPublisher] Published marker id={marker.id} at pos={obj.transform.position}, color=({marker.color.r}, {marker.color.g}, {marker.color.b}, {marker.color.a})");
    }

    TimeMsg GetROSTimestamp()
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