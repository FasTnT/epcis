using FasTnT.Model.Utils;

namespace FasTnT.Model.Enums
{
    public class EpcisField : Enumeration
    {
        public static readonly EpcisField RecordTime = new EpcisField(0, "recordTime");
        public static readonly EpcisField CaptureTime = new EpcisField(1, "captureTime");
        public static readonly EpcisField BusinessLocation = new EpcisField(2, "bizLocation");
        public static readonly EpcisField BusinessStep = new EpcisField(3, "bizStep");
        public static readonly EpcisField Disposition = new EpcisField(4, "disposition");
        public static readonly EpcisField ReadPoint = new EpcisField(5, "readPoint");
        public static readonly EpcisField Action = new EpcisField(6, "action");
        public static readonly EpcisField EventType = new EpcisField(7, "eventType");
        public static readonly EpcisField EventId = new EpcisField(8, "eventID");
        public static readonly EpcisField RequestId = new EpcisField(9, "requestID");
        public static readonly EpcisField TransformationId = new EpcisField(10, "transformationID");

        public EpcisField() { }
        public EpcisField(short id, string displayName) : base(id, displayName) { }
    }
}
