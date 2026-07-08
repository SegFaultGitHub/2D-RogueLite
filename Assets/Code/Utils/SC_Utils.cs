using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Utils {
    public static class SC_Utils {
        public static float Round(float value, float roundTo) {
            return Mathf.Round(value / roundTo) * roundTo;
        }

        public static Vector2 Round(Vector2 value, float roundTo) {
            float x = Round(value.x, roundTo);
            float y = Round(value.y, roundTo);
            return new Vector2(x, y);
        }

        public static int Round(float value, int roundTo) {
            return (int)Mathf.Round(value / roundTo) * roundTo;
        }

        public static bool Rate(float rate) {
            return Random.Range(0, 1f) < rate;
        }

        public static (int, bool) RatePlus(float rate) {
            int result = Mathf.FloorToInt(rate);
            bool b = Rate(rate - result);
            return (result
                    + (b
                        ? 1
                        : 0), b);
        }

        public static float RatioFrom(float from, float to, float current) {
            if (Mathf.Approximately(from, to)) return from;

            float min = Mathf.Min(from, to);
            float max = Mathf.Max(from, to);
            current = Mathf.Clamp(current, min, max);

            float ratio = (current - min) / (max - min);
            if (from > to) ratio = 1 - ratio;

            return ratio;
        }

        public static float MapFrom(float fromMin, float fromMax, float toMin, float toMax, float current) {
            float ratio = RatioFrom(fromMin, fromMax, current);
            if (toMin > toMax) ratio = 1 - ratio;

            float _toMin = Mathf.Min(toMin, toMax);
            float _toMax = Mathf.Max(toMin, toMax);

            if (_toMin > _toMax) ratio = 1 - ratio;

            return _toMin + ratio * (_toMax - _toMin);
        }

        // Return a sin version of the ratio from 0 to 1
        public static float SinRatio(float x) {
            x = Mathf.Clamp(x, 0, 1);
            return .5f * (Mathf.Cos(x * Mathf.PI + Mathf.PI) + 1);
        }

        #region Simple sample
        public static T Sample<T>(List<T> list) {
            if (list.Count != 0) return list[Random.Range(0, list.Count)];
            throw new Exception("Trying to sample an empty list");
        }

        public static T Sample<T>(ICollection<T> list) {
            if (list.Count == 0) {
                throw new Exception("Trying to sample an empty list");
            }

            List<T> clone = new();
            clone.AddRange(list);
            return clone[Random.Range(0, list.Count)];
        }

        public static List<T> Shuffle<T>(ICollection<T> list) {
            return Sample(list, list.Count);
        }

        public static List<T> Sample<T>(IEnumerable<T> list, int n) {
            List<T> clone = new();
            clone.AddRange(list);

            List<T> result = new();
            while (result.Count < n && clone.Count > 0) {
                T element = Sample(clone);
                clone.Remove(element);
                result.Add(element);
            }

            return result;
        }
        #endregion

        #region Weighted sample
        private static C_WeightedObject<T> GetRandomItemInDistribution<T>(List<C_WeightedObject<T>> list) {
            float total = list.Sum(weightDistribution => weightDistribution.Weight);
            float choice = Random.Range(0, total);
            float index = 0;
            foreach (C_WeightedObject<T> weightDistribution in list) {
                index += weightDistribution.Weight;
                if (choice <= index) {
                    return weightDistribution;
                }
            }

            return list[^1];
        }

        public static C_WeightedObject<T> Sample<T>(List<C_WeightedObject<T>> list) {
            return GetRandomItemInDistribution(list);
        }

        public static List<C_WeightedObject<T>> Sample<T>(List<C_WeightedObject<T>> list, int n) {
            if (list.Count == 0) {
                throw new Exception("Trying to sample an empty list");
            }

            List<C_WeightedObject<T>> clone = new();
            clone.AddRange(list);

            List<C_WeightedObject<T>> result = new();
            while (result.Count < n && clone.Count > 0) {
                C_WeightedObject<T> element = GetRandomItemInDistribution(clone);
                clone.Remove(element);
                result.Add(element);
            }

            return result;
        }

        public static C_WeightedObject<T> Sample<T>(List<C_WeightedObject<T>> list, List<T> allowedValues) {
            if (list.Count == 0 || allowedValues.Count == 0) {
                throw new Exception("Trying to sample an empty list");
            }

            List<C_WeightedObject<T>> clone = new();
            clone.AddRange(list.Where(item => allowedValues.Contains(item.Obj)));

            return Sample(clone);
        }

        public static List<C_WeightedObject<T>> Sample<T>(List<C_WeightedObject<T>> list, List<T> allowedValues, int n) {
            if (list.Count == 0 || allowedValues.Count == 0) {
                throw new Exception("Trying to sample an empty list");
            }

            List<C_WeightedObject<T>> clone = new();
            clone.AddRange(list.Where(item => allowedValues.Contains(item.Obj)));

            return Sample(clone, n);
        }
        #endregion

        public static string FormatNumber(float value, int decimals = 0, bool prefix = false) {
            return
                $"{(prefix && value >= 0 ? "+" : "")}{Math.Round(value, decimals).ToString($"N{decimals}", CultureInfo.InvariantCulture)}";
        }

        public static Vector3 LerpAngle(Vector3 a, Vector3 b, float t) {
            float x = Mathf.LerpAngle(a.x, b.x, t);
            float y = Mathf.LerpAngle(a.y, b.y, t);
            float z = Mathf.LerpAngle(a.z, b.z, t);
            return new Vector3(x, y, z);
        }

        public static float EvaluateOnCurve(AnimationCurve curve, float currentValue, RangedInt valueRange, RangedFloat resultRange) =>
            EvaluateOnCurve(curve, currentValue, new RangedFloat(valueRange.Min, valueRange.Max), resultRange);

        public static int EvaluateOnCurve(AnimationCurve curve, float currentValue, RangedFloat valueRange, RangedInt resultRange) =>
            Mathf.FloorToInt(EvaluateOnCurve(curve, currentValue, valueRange, new RangedFloat(resultRange.Min, resultRange.Max)));

        public static int EvaluateOnCurve(AnimationCurve curve, float currentValue, RangedInt valueRange, RangedInt resultRange) =>
            Mathf.FloorToInt(
                EvaluateOnCurve(
                    curve,
                    currentValue,
                    new RangedFloat(valueRange.Min, valueRange.Max),
                    new RangedFloat(resultRange.Min, resultRange.Max)
                )
            );

        public static float EvaluateOnCurve(AnimationCurve curve, float currentValue, RangedFloat valueRange, RangedFloat resultRange) {
            float evaluated = curve.Evaluate((currentValue - valueRange.Min) / (valueRange.Max - valueRange.Min));
            return evaluated * (resultRange.Max - resultRange.Min) + resultRange.Min;
        }

        public static Vector2 Rotate(Vector2 vector, float delta) {
            delta = Mathf.Deg2Rad * delta;
            return new Vector2(
                vector.x * Mathf.Cos(delta) - vector.y * Mathf.Sin(delta),
                vector.x * Mathf.Sin(delta) + vector.y * Mathf.Cos(delta)
            );
        }
    }
}
