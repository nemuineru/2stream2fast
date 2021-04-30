using UnityEngine;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Griffin.DataTool
{
    public class GUnityTerrainGroupConverter
    {
        public GameObject Root { get; set; }
        public bool ImportGeometry { get; set; }
        public bool ImportSplats { get; set; }
        public bool ImportSplatsAsAlbedo { get; set; }
        public bool ImportTrees { get; set; }
        public bool ImportGrasses { get; set; }
        public string DataDirectory { get; set; }

        private string conversionName;
        private string ConversionName
        {
            get
            {
                if (string.IsNullOrEmpty(conversionName))
                {
                    conversionName = System.DateTime.Now.Ticks.ToString();
                }
                return conversionName;
            }
            set
            {
                conversionName = value;
            }
        }

        private List<Terrain> terrains;
        private List<Terrain> Terrains
        {
            get
            {
                if (terrains == null)
                {
                    terrains = new List<Terrain>();
                }
                return terrains;
            }
            set
            {
                terrains = value;
            }
        }

        private List<GSplatPrototypeGroup> splatGroups;
        private List<GSplatPrototypeGroup> SplatGroups
        {
            get
            {
                if (splatGroups == null)
                {
                    splatGroups = new List<GSplatPrototypeGroup>();
                }
                return splatGroups;
            }
            set
            {
                splatGroups = value;
            }
        }

        private List<int> splatGroupIndices;
        private List<int> SplatGroupIndices
        {
            get
            {
                if (splatGroupIndices == null)
                {
                    splatGroupIndices = new List<int>();
                }
                return splatGroupIndices;
            }
            set
            {
                splatGroupIndices = value;
            }
        }

        private List<GTreePrototypeGroup> treeGroups;
        private List<GTreePrototypeGroup> TreeGroups
        {
            get
            {
                if (treeGroups == null)
                {
                    treeGroups = new List<GTreePrototypeGroup>();
                }
                return treeGroups;
            }
            set
            {
                treeGroups = value;
            }
        }

        private List<int> treeGroupIndices;
        private List<int> TreeGroupIndices
        {
            get
            {
                if (treeGroupIndices == null)
                {
                    treeGroupIndices = new List<int>();
                }
                return treeGroupIndices;
            }
            set
            {
                treeGroupIndices = value;
            }
        }

        private List<GGrassPrototypeGroup> grassGroups;
        private List<GGrassPrototypeGroup> GrassGroups
        {
            get
            {
                if (grassGroups == null)
                {
                    grassGroups = new List<GGrassPrototypeGroup>();
                }
                return grassGroups;
            }
            set
            {
                grassGroups = value;
            }
        }

        private List<int> grassGroupIndices;
        private List<int> GrassGroupIndices
        {
            get
            {
                if (grassGroupIndices == null)
                {
                    grassGroupIndices = new List<int>();
                }
                return grassGroupIndices;
            }
            set
            {
                grassGroupIndices = value;
            }
        }

        public void Convert()
        {
            try
            {
                Validate();
                SaveAssets();
                Initialize();
                CreateSharedData();
                CreateTerrainAndImportData();
                FinishingUp();
            }
            catch (GProgressCancelledException)
            {
                Debug.Log("Converting process canceled!");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
#if UNITY_EDITOR
                GCommonGUI.ClearProgressBar();
#endif
            }
        }

        private void Validate()
        {
            if (Root == null)
                throw new System.ArgumentNullException("Root");

            Terrain[] terrains = Root.GetComponentsInChildren<Terrain>();
            if (terrains.Length == 0)
            {
                throw new System.Exception("No Terrain found under Root");
            }

            bool hasData = false;
            for (int i = 0; i < terrains.Length; ++i)
            {
                if (terrains[i].terrainData != null)
                {
                    hasData = true;
                }
            }

            if (!hasData)
            {
                throw new System.Exception("No Terrain with Terrain Data found under Root");
            }

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(DataDirectory) || !DataDirectory.StartsWith("Assets/"))
            {
                throw new System.ArgumentException("Data Directory must be related to Assets folder");
            }
#endif
        }

        private void SaveAssets()
        {
#if UNITY_EDITOR
            GCommonGUI.ProgressBar("Saving", "Saving unsaved assets...", 1f);
            AssetDatabase.SaveAssets();
            GCommonGUI.ClearProgressBar();
#endif
        }

        private void Initialize()
        {
            Terrains = new List<Terrain>();
            Terrain[] t = Root.GetComponentsInChildren<Terrain>();
            for (int i = 0; i < t.Length; ++i)
            {
                if (t[i].terrainData != null)
                    Terrains.Add(t[i]);
            }

            GUtilities.EnsureDirectoryExists(DataDirectory);
        }

        private void CreateSharedData()
        {
            if (ImportSplats)
                CreateSharedSplats();

            if (ImportTrees)
                CreateSharedTrees();

            if (ImportGrasses)
                CreateSharedGrasses();

            SaveAssets();
        }

        private void CreateSharedSplats()
        {
            SplatGroups.Clear();
            SplatGroupIndices.Clear();

            for (int i = 0; i < Terrains.Count; ++i)
            {
                Terrain t = Terrains[i];
#if !UNITY_2018_1 && !UNITY_2018_2
                TerrainLayer[] layers = t.terrainData.terrainLayers;
#else
                SplatPrototype[] layers = t.terrainData.splatPrototypes;
#endif

                int splatGroupIndex = -1;
                for (int j = 0; j < SplatGroups.Count; ++j)
                {
                    if (SplatGroups[j].Equals(layers))
                    {
                        splatGroupIndex = j;
                        break;
                    }
                }

                if (splatGroupIndex >= 0)
                {
                    SplatGroupIndices.Add(splatGroupIndex);
                }
                else
                {
                    GSplatPrototypeGroup group = GSplatPrototypeGroup.Create(layers);
                    SplatGroups.Add(group);
                    SplatGroupIndices.Add(SplatGroups.Count - 1);
                }
            }

#if UNITY_EDITOR
            GUtilities.EnsureDirectoryExists(DataDirectory);
            for (int i = 0; i < SplatGroups.Count; ++i)
            {
                GSplatPrototypeGroup group = SplatGroups[i];
                group.name = string.Format("{0}{1}_{2}", "SharedSplats", i.ToString(), ConversionName);
                string assetPath = string.Format("{0}.asset", Path.Combine(DataDirectory, group.name));
                AssetDatabase.CreateAsset(group, assetPath);
            }
#endif
        }

        private void CreateSharedTrees()
        {
            TreeGroups.Clear();
            TreeGroupIndices.Clear();

            for (int i = 0; i < Terrains.Count; ++i)
            {
                Terrain t = Terrains[i];
                TreePrototype[] treePrototypes = t.terrainData.treePrototypes;

                int treeGroupIndex = -1;
                for (int j = 0; j < TreeGroups.Count; ++j)
                {
                    if (TreeGroups[j].Equals(treePrototypes))
                    {
                        treeGroupIndex = j;
                        break;
                    }
                }

                if (treeGroupIndex >= 0)
                {
                    TreeGroupIndices.Add(treeGroupIndex);
                }
                else
                {
                    GTreePrototypeGroup group = GTreePrototypeGroup.Create(treePrototypes);
                    TreeGroups.Add(group);
                    TreeGroupIndices.Add(TreeGroups.Count - 1);
                }
            }

#if UNITY_EDITOR
            GUtilities.EnsureDirectoryExists(DataDirectory);
            for (int i = 0; i < TreeGroups.Count; ++i)
            {
                GTreePrototypeGroup group = TreeGroups[i];
                group.name = string.Format("{0}{1}_{2}", "SharedTrees", i.ToString(), ConversionName);
                string assetPath = string.Format("{0}.asset", Path.Combine(DataDirectory, group.name));
                AssetDatabase.CreateAsset(group, assetPath);
            }
#endif
        }

        private void CreateSharedGrasses()
        {
            GrassGroups.Clear();
            GrassGroupIndices.Clear();

            for (int i = 0; i < Terrains.Count; ++i)
            {
                Terrain t = Terrains[i];
                DetailPrototype[] detailPrototypes = t.terrainData.detailPrototypes;

                int grassGroupIndex = -1;
                for (int j = 0; j < GrassGroups.Count; ++j)
                {
                    if (GrassGroups[j].Equals(detailPrototypes))
                    {
                        grassGroupIndex = j;
                        break;
                    }
                }

                if (grassGroupIndex >= 0)
                {
                    GrassGroupIndices.Add(grassGroupIndex);
                }
                else
                {
                    GGrassPrototypeGroup group = GGrassPrototypeGroup.Create(detailPrototypes);
                    GrassGroups.Add(group);
                    GrassGroupIndices.Add(GrassGroups.Count - 1);
                }
            }

#if UNITY_EDITOR
            GUtilities.EnsureDirectoryExists(DataDirectory);
            for (int i = 0; i < GrassGroups.Count; ++i)
            {
                GGrassPrototypeGroup group = GrassGroups[i];
                group.name = string.Format("{0}{1}_{2}", "SharedGrasses", i.ToString(), ConversionName);
                string assetPath = string.Format("{0}.asset", Path.Combine(DataDirectory, group.name));
                AssetDatabase.CreateAsset(group, assetPath);
            }
#endif
        }

        private void CreateTerrainAndImportData()
        {
            GameObject terrainRoot = new GameObject(string.Format("{0}-{1}", Root.name, ConversionName));
            terrainRoot.transform.parent = Root.transform.parent;
            terrainRoot.transform.position = Root.transform.position;

            for (int i = 0; i < Terrains.Count; ++i)
            {
#if UNITY_EDITOR
                GCommonGUI.CancelableProgressBar("Converting", "Converting " + Terrains[i].name, 1f);
#endif
                GStylizedTerrain t = CreateTerrain();
                t.transform.parent = terrainRoot.transform;
                t.transform.position = Terrains[i].transform.position;
                t.name = Terrains[i].name;

#if !UNITY_2018_1 && !UNITY_2018_2
                t.GroupId = Terrains[i].groupingID;
#endif
                if (ImportSplats)
                {
                    t.TerrainData.Shading.Splats = SplatGroups[SplatGroupIndices[i]];
                    GSplatsModel splatModel = t.TerrainData.Shading.Splats.Prototypes.Count <= 4 ?
                        GSplatsModel.Splats4Normals4 :
                        GSplatsModel.Splats8;
                    Material mat = GGriffinSettings.Instance.WizardSettings.GetClonedMaterial(GLightingModel.PBR, GTexturingModel.Splat, splatModel);
                    t.TerrainData.Shading.MaterialToRender.shader = mat.shader;
                    t.TerrainData.Shading.UpdateMaterials();
                    GUtilities.DestroyObject(mat);
                }
                if (ImportTrees)
                {
                    t.TerrainData.Foliage.Trees = TreeGroups[TreeGroupIndices[i]];
                }
                if (ImportGrasses)
                {
                    t.TerrainData.Foliage.Grasses = GrassGroups[GrassGroupIndices[i]];
                }

                GUnityTerrainDataImporter importer = new GUnityTerrainDataImporter();
                importer.SrcData = Terrains[i].terrainData;
                importer.SrcTerrain = Terrains[i];
                importer.DesData = t.TerrainData;
                importer.DesTerrain = t;
                importer.ImportGeometry = ImportGeometry;
                importer.UseUnityTerrainSize = true;
                importer.ImportSplats = ImportSplats;
                importer.ImportSplatsAsAlbedo = ImportSplatsAsAlbedo;
                importer.CreateNewSplatPrototypesGroup = false;
                importer.ImportTrees = ImportTrees;
                importer.CreateNewTreePrototypesGroup = false;
                importer.ImportGrasses = ImportGrasses;
                importer.CreateNewGrassPrototypesGroup = false;
                importer.GrassDensity = GUnityTerrainDataImporter.GetAdjustedGrassDensity(importer.SrcData, 1);
                importer.Import();

#if UNITY_EDITOR
                SaveAssets();
                GCommonGUI.ClearProgressBar();
#endif
            }
        }

        private GStylizedTerrain CreateTerrain()
        {
            GameObject g = new GameObject("Stylized Terrain");
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.localScale = Vector3.one;

            GTerrainData terrainData = GTerrainData.CreateInstance<GTerrainData>();
            terrainData.Reset();

#if UNITY_EDITOR
            string assetName = "TerrainData_" + terrainData.Id;
            AssetDatabase.CreateAsset(terrainData, string.Format("{0}.asset", Path.Combine(DataDirectory, assetName)));
#endif

            Material mat = GGriffinSettings.Instance.WizardSettings.GetClonedMaterial(GLightingModel.PBR, GTexturingModel.Splat);
            if (mat == null && GGriffinSettings.Instance.TerrainDataDefault.Shading.CustomMaterial != null)
            {
                mat = Object.Instantiate(GGriffinSettings.Instance.TerrainDataDefault.Shading.CustomMaterial);
            }
            terrainData.Shading.CustomMaterial = mat;

#if UNITY_EDITOR
            if (mat != null)
            {
                string matName = "TerrainMaterial_" + terrainData.Id;
                mat.name = matName;
                AssetDatabase.CreateAsset(mat, string.Format("{0}.mat", Path.Combine(DataDirectory, matName)));
            }
#endif

            GStylizedTerrain terrain = g.AddComponent<GStylizedTerrain>();
            terrain.GroupId = 0;
            terrain.TerrainData = terrainData;

            GameObject colliderGO = new GameObject("Tree Collider");
            colliderGO.transform.parent = terrain.transform;
            colliderGO.transform.localPosition = Vector3.zero;
            colliderGO.transform.localRotation = Quaternion.identity;
            colliderGO.transform.localScale = Vector3.one;

            GTreeCollider collider = colliderGO.AddComponent<GTreeCollider>();
            collider.Terrain = terrain;

            return terrain;
        }

        private void FinishingUp()
        {
#if UNITY_EDITOR
            GCommonGUI.ProgressBar("Finishing Up", "Matching geometry...", 1f);
#endif
            GStylizedTerrain.ConnectAdjacentTiles();
            GStylizedTerrain.MatchEdges(-1);
            Root.gameObject.SetActive(false);
#if UNITY_EDITOR
            GCommonGUI.ClearProgressBar();
#endif
        }
    }
}
