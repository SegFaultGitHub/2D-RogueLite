using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MyBox.Internal {
    public class ConditionalData {
        public bool IsSet => this._fieldToCheck.NotNullOrEmpty() || this._fieldsToCheckMultiple.NotNullOrEmpty() || this._predicateMethod.NotNullOrEmpty();

        private readonly string _fieldToCheck;
        private readonly bool _inverse;
        private readonly string[] _compareValues;

        private readonly string[] _fieldsToCheckMultiple;
        private readonly bool[] _inverseMultiple;
        private readonly string[] _compareValuesMultiple;

        private readonly string _predicateMethod;

        public ConditionalData(string fieldToCheck, bool inverse = false, params object[] compareValues) =>
            (this._fieldToCheck, this._inverse, this._compareValues) =
            (fieldToCheck, inverse, compareValues.Select(c => c.ToString().ToUpper()).ToArray());

        public ConditionalData(string[] fieldToCheck, bool[] inverse = null, params object[] compare) =>
            (this._fieldsToCheckMultiple, this._inverseMultiple, this._compareValuesMultiple) =
            (fieldToCheck, inverse, compare.Select(c => c.ToString().ToUpper()).ToArray());

        public ConditionalData(params string[] fieldToCheck) => this._fieldsToCheckMultiple = fieldToCheck;

        // ReSharper disable once UnusedParameter.Local
        public ConditionalData(bool useMethod, string methodName, bool inverse = false) =>
            (this._predicateMethod, this._inverse) = (methodName, inverse);


#if UNITY_EDITOR
        /// <summary>
        /// Iterate over Field Conditions
        /// </summary>
        public IEnumerator<(string Field, bool Inverse, string[] CompareAgainst)> GetEnumerator() {
            if (this._fieldToCheck.NotNullOrEmpty()) yield return (this._fieldToCheck, this._inverse, this._compareValues);
            if (this._fieldsToCheckMultiple.NotNullOrEmpty()) {
                for (var i = 0; i < this._fieldsToCheckMultiple.Length; i++) {
                    var field = this._fieldsToCheckMultiple[i];
                    bool withInverseValue = this._inverseMultiple != null && this._inverseMultiple.Length - 1 >= i;
                    bool withCompareValue = this._compareValuesMultiple != null && this._compareValuesMultiple.Length - 1 >= i;
                    var inverse = withInverseValue && this._inverseMultiple[i];
                    var compare = withCompareValue
                        ? new[] { this._compareValuesMultiple[i] }
                        : null;

                    yield return (field, inverse, compare);
                }
            }
        }

        /// <summary>
        /// Call and check Method Condition, if any
        /// </summary>
        public bool IsMethodConditionMatch(object owner) {
            if (this._predicateMethod.IsNullOrEmpty()) return true;

            var predicateMethod = this.GetMethodCondition(owner);
            if (predicateMethod == null) return true;

            bool match = (bool)predicateMethod.Invoke(owner, null);
            if (this._inverse) match = !match;
            return match;
        }


        private MethodInfo GetMethodCondition(object owner) {
            if (this._predicateMethod.IsNullOrEmpty()) return null;
            if (this._initializedMethodInfo) return this._cachedMethodInfo;
            this._initializedMethodInfo = true;

            var ownerType = owner.GetType();
            var bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var method = ownerType.GetMethods(bindings).SingleOrDefault(m => m.Name == this._predicateMethod);

            if (method == null || method.ReturnType != typeof(bool)) {
                ConditionalUtility.LogMethodNotFound((Object)owner, this._predicateMethod);
                this._cachedMethodInfo = null;
            } else
                this._cachedMethodInfo = method;

            return this._cachedMethodInfo;
        }

        private MethodInfo _cachedMethodInfo;
        private bool _initializedMethodInfo;
#endif
    }
}
