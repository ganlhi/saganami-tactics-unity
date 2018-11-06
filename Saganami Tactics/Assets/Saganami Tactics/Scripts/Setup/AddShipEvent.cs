using UnityEngine.Events;

namespace ST
{
    public struct AddShipEventData
    {
        public ShipSSD SSD;
        public string ShipName;
    }

    public class AddShipEvent : UnityEvent<AddShipEventData>
    {
    }
}