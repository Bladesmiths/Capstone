using System;
using System.Linq;
using Sirenix.Utilities.Editor;
using UnityEngine.UIElements;

namespace Bladesmiths.Capstone
{
    using Sirenix.OdinInspector;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Bladesmiths.Capstone.Math;

    [RequireComponent(typeof(BoxCollider))]
    public class CrystalWallGenerator : SerializedMonoBehaviour
    {
        [TitleGroup("Prefabs")]
        [TableList(ShowIndexLabels = true)]
        public List<Crystal> crystalCollection;

        [TitleGroup("Procedural Generation Properties")]
        [Range(0f, 50f)] [SerializeField] private float areaLength = 10f;
        [Range(0f, 50f)] [SerializeField] private float areaWidth = 10f;
        [Range(0f, 50f)] [SerializeField] private float areaHeight = 10f;

        [Range(0f, 5f)] [SerializeField] private float minimumDistance = 10f;

        private List<Vector2> samplePoints = new List<Vector2>();

        [TitleGroup("Crystal Wall Dissolve Properties")]
        [Range(-3f, 3f)] [SerializeField]
        private float objectHeightStart = -2.0f;
        [Range(-3f, 3f)] [SerializeField]
        private float objectHeightEnd = 2.0f;
        [Range(0f, 10f)] [SerializeField]
        private float timerSlowdownDivisor = 2.0f;


        [SerializeField]
        private Material amethystMaterial;
        [SerializeField] [ReadOnly]

        private float cutoffHeight;

        private float dissolveTimer = 0f;
        private bool dissolveToggle = false;
        
        // Start is called before the first frame update
        void Start()
        {
            ResetMaterialCutoff();

            GenerateWall();

            UpdateCollider();
        }

        // Update is called once per frame
        void Update()
        {

            if (dissolveToggle)
            {
                dissolveTimer += Time.deltaTime;
                float height = objectHeightStart + dissolveTimer / timerSlowdownDivisor;
                cutoffHeight = amethystMaterial.GetFloat("_CutoffHeight");
                amethystMaterial.SetFloat("_CutoffHeight", height);
            }

            if (amethystMaterial != null && dissolveToggle &&
                cutoffHeight > objectHeightEnd)
            {
                dissolveToggle = false;
            }
        }

        [TitleGroup("Editor and Play Mode Actions")]
        [Button("Generate Wall Once")]
        public void GenerateWall()
        {
            if (samplePoints.Count > 0 || transform.childCount > 0)
                ClearWall();

            Vector2 topLeft = new Vector2(0, 0);
            Vector2 lowerRight = new Vector2(areaLength, areaWidth);

            samplePoints = PoissonDiskSampling.SampleRectangle(topLeft, lowerRight, minimumDistance);

            for (int i = 0; i < samplePoints.Count; i++)
            {
                var newCrystal =
                    Instantiate(crystalCollection[Random.Range(0, crystalCollection.Count)].prefab,
                        transform);

                // Set their local position based on sample points
                newCrystal.transform.localPosition = new Vector3(samplePoints[i].x - areaLength / 2f,
                    newCrystal.transform.localPosition.y,
                    samplePoints[i].y - areaWidth / 2f);

                // Random rotations for crystals
                newCrystal.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0));
            }
        }

        [Button("Reset Material Cutoff Value")]
        public void ResetMaterialCutoff()
        {
            if (amethystMaterial != null)
                amethystMaterial.SetFloat("_CutoffHeight", objectHeightStart);
        }

        [TitleGroup("Play Mode Only Actions")]
        [Button("Remove Wall with Dissolve Effect")]
        [DisableInEditorMode]
        public void DissolveWall()
        {
            if (amethystMaterial != null)
            {
                dissolveToggle = true;
            }
        }

        [TitleGroup("Editor Mode Only Actions")]
        [Button("Clear Crystal Child Prefabs")]
        [DisableInPlayMode]
        private void ClearWall()
        {
            samplePoints.Clear();

            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private void UpdateCollider()
        {
            GetComponent<BoxCollider>().size = new Vector3(areaLength, areaHeight, areaWidth);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaLength, areaHeight, areaWidth));
        }

        [Serializable]
        public struct Crystal
        {
            [TableColumnWidth(80, Resizable = false)]
            [PreviewField(60, ObjectFieldAlignment.Center)]
            public GameObject prefab;

            [ShowInInspector, DisplayAsString]
            public string Description
            {
                get
                {
#if UNITY_EDITOR
                    GUIHelper.RequestRepaint();
#endif
                    return prefab ? prefab.name : "";
                }
            }
        }
    }
}