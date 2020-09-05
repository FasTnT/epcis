using FasTnT.Model.Utils;

namespace FasTnT.Model.Enums
{
    public class EpcisField : Enumeration
    {
        public static EpcisField RecordTime = new EpcisField(0, "recordTime");
        public static EpcisField CaptureTime = new EpcisField(1, "captureTime");
        public static EpcisField BusinessLocation = new EpcisField(2, "bizLocation");
        public static EpcisField BusinessStep = new EpcisField(3, "bizStep");
        public static EpcisField Disposition = new EpcisField(4, "disposition");
        public static EpcisField ReadPoint = new EpcisField(5, "readPoint");
        public static EpcisField Action = new EpcisField(6, "action");
        public static EpcisField EventType = new EpcisField(7, "eventType");
        public static EpcisField EventId = new EpcisField(8, "eventID");
        public static EpcisField RequestId = new EpcisField(9, "requestID");
        public static EpcisField TransformationId = new EpcisField(10, "transformationID");

        public EpcisField() { }
        public EpcisField(short id, string displayName) : base(id, displayName) { }
    }
}
