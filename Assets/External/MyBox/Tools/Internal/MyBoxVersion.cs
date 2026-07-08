#if UNITY_EDITOR
using System;

namespace MyBox.Internal {
    [Serializable]
    public class MyBoxVersion {
        public string Major;
        public string Minor;
        public string Patch;

        public string AsSting;

        /// <param name="version">NUM.NUM.NUM format</param>
        public MyBoxVersion(string version) {
            this.AsSting = version;
            var v = version.Split('.');
            this.Major = v[0];
            this.Minor = v[1];
            this.Patch = v[2];
        }

        /// <summary>
        /// Major & Minor versions match, skip patch releases
        /// </summary>
        public bool BaseVersionMatch(MyBoxVersion version) {
            return this.Major == version.Major && this.Minor == version.Minor;
        }

        public bool VersionsMatch(MyBoxVersion version) {
            return this.BaseVersionMatch(version) && this.Patch == version.Patch;
        }
    }
}
#endif
