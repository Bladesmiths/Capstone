using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Math
{
    public static class PoissonDiskSampling
    {
        public const int DefaultPointsPerIteration = 30;

        static readonly float SquareRootTwo = (float)Mathf.Sqrt(2);

        struct Settings
        {
            public Vector2 TopLeft, LowerRight, Center;
            public Vector2 Dimensions;
            public float? RejectionSqDistance;
            public float? RejectionDistance;
            public float MinimumDistance;
            public float CellSize;
            public int GridWidth, GridHeight;
        }

        struct State
        {
            public Vector2?[,] Grid;
            public List<Vector2> ActivePoints, Points;
        }

        public static List<Vector2> SampleCircle(Vector2 center, float radius, float minimumDistance)
        {
            return SampleCircle(center, radius, minimumDistance, DefaultPointsPerIteration);
        }

        public static List<Vector2> SampleCircle(Vector2 center, float radius, float minimumDistance,
            int pointsPerIteration)
        {
            return Sample(center - new Vector2(radius, radius), center + new Vector2(radius, radius), radius,
                minimumDistance, pointsPerIteration);
        }

        public static List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance)
        {
            return SampleRectangle(topLeft, lowerRight, minimumDistance, DefaultPointsPerIteration);
        }

        public static List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance,
            int pointsPerIteration)
        {
            return Sample(topLeft, lowerRight, null, minimumDistance, pointsPerIteration);
        }

        static List<Vector2> Sample(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance,
            float minimumDistance, int pointsPerIteration)
        {
            var settings = new Settings
            {
                TopLeft = topLeft, LowerRight = lowerRight,
                Dimensions = lowerRight - topLeft,
                Center = (topLeft + lowerRight) / 2,
                CellSize = minimumDistance / SquareRootTwo,
                MinimumDistance = minimumDistance,
                RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance,
                RejectionDistance = rejectionDistance == null ? null : rejectionDistance,
            };
            settings.GridWidth = (int)(settings.Dimensions.x / settings.CellSize) + 1;
            settings.GridHeight = (int)(settings.Dimensions.y / settings.CellSize) + 1;

            var state = new State
            {
                Grid = new Vector2?[settings.GridWidth, settings.GridHeight],
                ActivePoints = new List<Vector2>(),
                Points = new List<Vector2>()
            };

            AddFirstPoint(ref settings, ref state);

            while (state.ActivePoints.Count != 0)
            {
                var listIndex = Random.Range(0, state.ActivePoints.Count);

                var point = state.ActivePoints[listIndex];
                var found = false;

                for (var k = 0; k < pointsPerIteration; k++)
                    found |= AddNextPoint(point, ref settings, ref state);

                if (!found)
                    state.ActivePoints.RemoveAt(listIndex);
            }

            return state.Points;
        }

        static void AddFirstPoint(ref Settings settings, ref State state)
        {
            var added = false;
            while (!added)
            {
                var d = Random.value;
                var xr = settings.TopLeft.x + settings.Dimensions.x * d;

                d = Random.value;
                var yr = settings.TopLeft.y + settings.Dimensions.y * d;

                var p = new Vector2((float)xr, (float)yr);
                if (settings.RejectionDistance != null &&
                    Vector2.Distance(settings.Center, p) > settings.RejectionDistance)
                    continue;
                added = true;

                var index = Denormalize(p, settings.TopLeft, settings.CellSize);

                state.Grid[(int)index.x, (int)index.y] = p;

                state.ActivePoints.Add(p);
                state.Points.Add(p);
            }
        }

        static bool AddNextPoint(Vector2 point, ref Settings settings, ref State state)
        {
            var found = false;
            var q = GenerateRandomAround(point, settings.MinimumDistance);

            if (q.x >= settings.TopLeft.x && q.x < settings.LowerRight.x &&
                q.y > settings.TopLeft.y && q.y < settings.LowerRight.y &&
                (settings.RejectionDistance == null ||
                 Vector2.Distance(settings.Center, q) <= settings.RejectionDistance))
            {
                var qIndex = Denormalize(q, settings.TopLeft, settings.CellSize);
                var tooClose = false;

                for (var i = (int)Mathf.Max(0, qIndex.x - 2);
                     i < Mathf.Min(settings.GridWidth, qIndex.x + 3) && !tooClose;
                     i++)
                for (var j = (int)Mathf.Max(0, qIndex.y - 2);
                     j < Mathf.Min(settings.GridHeight, qIndex.y + 3) && !tooClose;
                     j++)
                    if (state.Grid[i, j].HasValue &&
                        Vector2.Distance(state.Grid[i, j].Value, q) < settings.MinimumDistance)
                        tooClose = true;

                if (!tooClose)
                {
                    found = true;
                    state.ActivePoints.Add(q);
                    state.Points.Add(q);
                    state.Grid[(int)qIndex.x, (int)qIndex.y] = q;
                }
            }

            return found;
        }

        static Vector2 GenerateRandomAround(Vector2 center, float minimumDistance)
        {
            var d = Random.value;
            var radius = minimumDistance + minimumDistance * d;

            d = Random.value;
            var angle = MathHelper.TwoPi * d;

            var newX = radius * Mathf.Sin(angle);
            var newY = radius * Mathf.Cos(angle);

            return new Vector2((float)(center.x + newX), (float)(center.y + newY));
        }

        static Vector2 Denormalize(Vector2 point, Vector2 origin, double cellSize)
        {
            return new Vector2((int)((point.x - origin.x) / cellSize), (int)((point.y - origin.y) / cellSize));
        }
    }

    public static class MathHelper
    {
        public const float Pi = (float)Mathf.PI;
        public const float HalfPi = (float)(Mathf.PI / 2);
        public const float TwoPi = (float)(Mathf.PI * 2);
    }
}