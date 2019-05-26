using UnityEngine;

[System.Serializable]
public struct Attitude
{
    public float Yaw;
    public float Pitch;
    public float Roll;
    public Quaternion ToQuaternion()
    {
        // positive yaw and roll are on the right, positive pitch is up
        return Quaternion.AngleAxis(Yaw, Vector3.up) * Quaternion.AngleAxis(Pitch, Vector3.left) * Quaternion.AngleAxis(Roll, Vector3.back);
    }

    public static Attitude operator +(Attitude a, Attitude b)
    {
        return new Attitude
        {
            Yaw = a.Yaw + b.Yaw,
            Pitch = a.Pitch + b.Pitch,
            Roll = a.Roll + b.Roll
        };
    }
}