using System;
using System.Collections.Generic;

public enum Pivot
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    UpLeft = 4,
    UpRight = 5,
    DownLeft = 6,
    DownRight = 7,
}

public enum Roll { Left = 0, Right = 1 }

[Serializable]
public struct Plotting
{
    static int Increment = 30;

    public static bool IsDiagonal(Pivot p)
    {
        return (int)p >= 4;
    }

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

    public void UndoPivot()
    {
        if (Pivots.Count > 0)
        {
            Pivots.RemoveAt(Pivots.Count - 1);
        }
    }
    public void UndoRoll()
    {
        if (Rolls.Count > 0)
        {
            Rolls.RemoveAt(Rolls.Count - 1);
        }
    }
}