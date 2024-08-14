//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Sensor
{
    [Serializable]
    public class TimeReferenceMsg : Message
    {
        public const string k_RosMessageName = "sensor_msgs/TimeReference";
        public override string RosMessageName => k_RosMessageName;

        //  Measurement from an external time source not actively synchronized with the system clock.
        public Std.HeaderMsg header;
        //  stamp is system time for which measurement was valid
        //  frame_id is not used
        public BuiltinInterfaces.TimeMsg time_ref;
        //  corresponding time from this external source
        public string source;
        //  (optional) name of time source

        public TimeReferenceMsg()
        {
            this.header = new Std.HeaderMsg();
            this.time_ref = new BuiltinInterfaces.TimeMsg();
            this.source = "";
        }

        public TimeReferenceMsg(Std.HeaderMsg header, BuiltinInterfaces.TimeMsg time_ref, string source)
        {
            this.header = header;
            this.time_ref = time_ref;
            this.source = source;
        }

        public static TimeReferenceMsg Deserialize(MessageDeserializer deserializer) => new TimeReferenceMsg(deserializer);

        private TimeReferenceMsg(MessageDeserializer deserializer)
        {
            this.header = Std.HeaderMsg.Deserialize(deserializer);
            this.time_ref = BuiltinInterfaces.TimeMsg.Deserialize(deserializer);
            deserializer.Read(out this.source);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.time_ref);
            serializer.Write(this.source);
        }

        public override string ToString()
        {
            return "TimeReferenceMsg: " +
            "\nheader: " + header.ToString() +
            "\ntime_ref: " + time_ref.ToString() +
            "\nsource: " + source.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
