using System.Collections.Generic;
using UnityEngine;

public enum Pivot
{
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
}

public enum Roll { Left, Right }

public struct Plotting
{
    static int Increment = 30;

    public List<Pivot> Pivots;
    public List<Roll> Rolls;
    public int Thrust;

    public static Plotting Empty
    {
        get
        {
            return new Plotting
            {
                Pivots = new List<Pivot>(),
                Rolls = new List<Roll>(),
                Thrust = 0,
            };
        }
    }

    public int Displacement { get { return Rolls.Count > 0 ? 0 : Thrust / 2; } }

    public bool CanPivotOnDiagonals
    {
        get
        {
            var diagonals = new List<Pivot> {
                Pivot.UpLeft,
                Pivot.UpRight,
                Pivot.DownLeft,
                Pivot.DownRight
            };
            return !Pivots.Exists(p => diagonals.Contains(p));
        }
    }

    public List<Pivot> HalfPivots { get { return Pivots.GetRange(0, Pivots.Count / 2); } }
    public List<Roll> HalfRolls { get { return Rolls.GetRange(0, Rolls.Count / 2); } }

    public Attitude AttitudeChange
    {
        get
        {
            var att = new Attitude();

            foreach (Pivot p in Pivots)
            {
                switch (p)
                {
                    case Pivot.Up:
                        att.Pitch += Increment;
                        break;
                    case Pivot.Down:
                        att.Pitch -= Increment;
                        break;
                    case Pivot.Left:
                        att.Yaw -= Increment;
                        break;
                    case Pivot.Right:
                        att.Yaw += Increment;
                        break;
                    case Pivot.UpLeft:
                        att.Pitch += Increment;
                        att.Yaw -= Increment;
                        break;
                    case Pivot.UpRight:
                        att.Pitch += Increment;
                        att.Yaw += Increment;
                        break;
                    case Pivot.DownLeft:
                        att.Pitch -= Increment;
                        att.Yaw -= Increment;
                        break;
                    case Pivot.DownRight:
                        att.Pitch -= Increment;
                        att.Yaw += Increment;
                        break;
                }
            }

            foreach (Roll r in Rolls)
            {
                att.Roll += r == Roll.Right ? Increment : -Increment;
            }

            return att;
        }
    }

    public Attitude HalfAttitudeChange
    {
        get
        {
            var p = new Plotting
            {
                Pivots = HalfPivots,
                Rolls = HalfRolls,
            };

            return p.AttitudeChange;
        }
    }
}