using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Utils {
    [SelectionBase]
    public class MB_RandomGameObject : MonoBehaviour {
        #region Members
        [Foldout("MB_RandomGameObject", true)]
        [SerializeField] private List<C_WeightedObject<GameObject>> m_Objects;
        [SerializeField] private bool m_RandomRotateX;
        [SerializeField][ConditionalField(nameof(m_RandomRotateX))]
        private int m_AngleStepX = 1;
        [SerializeField] private bool m_RandomRotateY;
        [SerializeField][ConditionalField(nameof(m_RandomRotateY))]
        private int m_AngleStepY = 1;
        [SerializeField] private bool m_RandomRotateZ;
        [SerializeField][ConditionalField(nameof(m_RandomRotateZ))]
        private int m_AngleStepZ = 1;
        [SerializeField] private bool m_RandomFlipX;
        [SerializeField] private bool m_RandomFlipY;
        #endregion

        #region Getters / Setters
        private List<C_WeightedObject<GameObject>> Objects { get => this.m_Objects; }
        private bool RandomRotateX { get => this.m_RandomRotateX; }
        private int AngleStepX { get => this.m_AngleStepX; }
        private bool RandomRotateY { get => this.m_RandomRotateY; }
        private int AngleStepY { get => this.m_AngleStepY; }
        private bool RandomRotateZ { get => this.m_RandomRotateZ; }
        private int AngleStepZ { get => this.m_AngleStepZ; }
        private bool RandomFlipX { get => this.m_RandomFlipX; }
        private bool RandomFlipY { get => this.m_RandomFlipY; }
        #endregion

        private void Awake() {
            if (this.Objects.Count == 0)
                throw new Exception($"[RandomGameObject:Awake] Objects is empty! {this.name}");

            C_WeightedObject<GameObject> choice = SC_Utils.Sample(this.Objects);
            foreach (C_WeightedObject<GameObject> randomGameObject in this.Objects.Where(randomGameObject => randomGameObject != choice)) {
                Destroy(randomGameObject.Obj);
            }

            if (choice.Obj == null)
                throw new Exception($"[RandomGameObject:Awake] Object is null! {this.name}");
            choice.Obj.SetActive(true);

            Vector3 angles = choice.Obj.transform.localEulerAngles;
            float x = angles.x, y = angles.y, z = angles.z;
            if (this.RandomRotateX) {
                x += Random.Range(0, 360 / this.AngleStepX) * this.AngleStepX;
            }

            if (this.RandomRotateY) {
                y += Random.Range(0, 360 / this.AngleStepY) * this.AngleStepY;
            }

            if (this.RandomRotateZ) {
                z += Random.Range(0, 360 / this.AngleStepZ) * this.AngleStepZ;
            }


            if (this.RandomFlipX) {
                choice.Obj.transform.localScale = new Vector3(
                    choice.Obj.transform.localScale.x
                    * (SC_Utils.Rate(.5f)
                        ? -1
                        : 1),
                    choice.Obj.transform.localScale.y,
                    choice.Obj.transform.localScale.z
                );
            }

            if (this.RandomFlipY) {
                choice.Obj.transform.localScale = new Vector3(
                    choice.Obj.transform.localScale.x,
                    choice.Obj.transform.localScale.y
                    * (SC_Utils.Rate(.5f)
                        ? -1
                        : 1),
                    choice.Obj.transform.localScale.z
                );
            }

            choice.Obj.transform.localEulerAngles = new Vector3(x, y, z);

            Destroy(this);
        }
    }
}
