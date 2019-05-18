using System.Collections.Generic;
using UnityEngine;

namespace ST
{
    public struct Bearing
    {
        public Side Side;
        public Side? Wedge;
        public Side? SideWall;

        public static Bearing Compute(Transform attitude, Vector3 targetPosition)
        {
            var targetDirection = attitude.position.DirectionTo(targetPosition);
            var angles = GetTargetToShipVectorsAngles(targetDirection, attitude);

            if (angles[Side.Forward] <= 45)
            {
                if (angles[Side.Top] <= 45)
                {
                    return new Bearing
                    {
                        Side = Side.Forward,
                        Wedge = Side.Top,
                        SideWall = null,
                    };
                }

                if (angles[Side.Bottom] <= 45)
                {
                    return new Bearing
                    {
                        Side = Side.Forward,
                        Wedge = Side.Bottom,
                        SideWall = null,
                    };
                }

                if (angles[Side.Starboard] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Forward,
                        Wedge = null,
                        SideWall = Side.Starboard,
                    };
                }

                if (angles[Side.Port] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Forward,
                        Wedge = null,
                        SideWall = Side.Port,
                    };
                }

                return new Bearing
                {
                    Side = Side.Forward,
                    Wedge = null,
                    SideWall = null,
                };
            }

            if (angles[Side.Aft] <= 45)
            {
                if (angles[Side.Top] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Aft,
                        Wedge = Side.Top,
                        SideWall = null,
                    };
                }

                if (angles[Side.Bottom] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Aft,
                        Wedge = Side.Bottom,
                        SideWall = null,
                    };
                }

                if (angles[Side.Starboard] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Aft,
                        Wedge = null,
                        SideWall = Side.Starboard,
                    };
                }

                if (angles[Side.Port] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Aft,
                        Wedge = null,
                        SideWall = Side.Port,
                    };
                }

                return new Bearing
                {
                    Side = Side.Aft,
                    Wedge = null,
                    SideWall = null,
                };
            }

            if (angles[Side.Port] <= 45)
            {
                if (angles[Side.Top] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Port,
                        Wedge = Side.Top,
                        SideWall = null,
                    };
                }

                if (angles[Side.Bottom] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Port,
                        Wedge = Side.Bottom,
                        SideWall = null,
                    };
                }

                return new Bearing
                {
                    Side = Side.Port,
                    Wedge = null,
                    SideWall = Side.Port,
                };
            }

            if (angles[Side.Starboard] <= 45)
            {
                if (angles[Side.Top] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Starboard,
                        Wedge = Side.Top,
                        SideWall = null,
                    };
                }

                if (angles[Side.Bottom] <= 75)
                {
                    return new Bearing
                    {
                        Side = Side.Starboard,
                        Wedge = Side.Bottom,
                        SideWall = null,
                    };
                }

                return new Bearing
                {
                    Side = Side.Starboard,
                    Wedge = null,
                    SideWall = Side.Starboard,
                };
            }

            if (angles[Side.Top] <= 45)
            {
                return new Bearing
                {
                    Side = angles[Side.Port] < angles[Side.Starboard] ? Side.Port : Side.Starboard,
                    Wedge = Side.Top,
                    SideWall = null,
                };
            }

            if (angles[Side.Bottom] <= 45)
            {
                return new Bearing
                {
                    Side = angles[Side.Port] < angles[Side.Starboard] ? Side.Port : Side.Starboard,
                    Wedge = Side.Bottom,
                    SideWall = null,
                };
            }

            throw new System.Exception("Laws of mathematics have been violated!");
        }

        private static Dictionary<Side, float> GetTargetToShipVectorsAngles(Vector3 direction, Transform attitude)
        {
            return new Dictionary<Side, float>()
            {
                { Side.Forward, Vector3.Angle(attitude.forward, direction) },
                { Side.Aft, Vector3.Angle(-attitude.forward, direction) },
                { Side.Port, Vector3.Angle(-attitude.right, direction) },
                { Side.Starboard, Vector3.Angle(attitude.right, direction) },
                { Side.Top, Vector3.Angle(attitude.up, direction) },
                { Side.Bottom, Vector3.Angle(-attitude.up, direction) },
            };
        }
    }
}