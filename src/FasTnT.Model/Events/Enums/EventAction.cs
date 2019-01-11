using FasTnT.Model.Utils;

namespace FasTnT.Model.Events.Enums
{
    public class EventAction : Enumeration
    {
        public static EventAction Add = new EventAction(0, "ADD");
        public static EventAction Observe = new EventAction(1, "OBSERVE");
        public static EventAction Delete = new EventAction(2, "DELETE");

        public EventAction()
        {
        }

        private EventAction(short id, string displayName) : base(id, displayName)
        {
        }
    }
}
