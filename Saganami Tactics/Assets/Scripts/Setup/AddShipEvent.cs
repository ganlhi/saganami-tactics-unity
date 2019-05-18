using UnityEngine.Events;

namespace ST
{
    public struct AddShipEventData
    {
        public string ShipName;
    }

    public class AddShipEvent : UnityEvent<AddShipEventData>
    {
    }
}