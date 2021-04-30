#if POLARIS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Polaris;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Griffin.DataTool
{
    public class GPolarisV1DataImporter
    {
        public LPTData SrcData { get; set; }
        public GTerrainData DesData { get; set; }
        public bool ImportGeometry { get; set; }
        public bool UsePolarisV1TerrainSize { get; set; }
        public bool ImportColors { get; set; }
        public bool ImportSplats { get; set; }
        public bool CreateNewSplatPrototypesGroup { get; set; }

        public void Import()
        {
            try
            {
                if (ImportGeometry)
                {
                    GCommon.Editor_CancelableProgressBar("Working...", "Importing Geometry...", 1f);
                    DoImportGeometry();
                }
                if (ImportColors)
                {
                    GCommon.Editor_CancelableProgressBar("Working...", "Importing Colors...", 1f);
                    DoImportColors();
                }
                if (ImportSplats)
                {
                    GCommon.Editor_CancelableProgressBar("Working...", "Importing Splats...", 1f);
                    DoImportSplats();
                }
            }
            catch (GProgressCancelledException)
            {
                Debug.Log("Importing process canceled!");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
                GCommon.Editor_ClearProgressBar();
            }
        }

        private void DoImportGeometry()
        {
            if (UsePolarisV1TerrainSize)
            {
                Vector3 size = SrcData.OverallShapeSettings.ScaledSize;
                DesData.Geometry.Width = size.x;
                DesData.Geometry.Height = size.y;
                DesData.Geometry.Length = size.z;
            }

            SrcData.SurfaceSettings.InitHeightMapColors();
            if (SrcData.SurfaceSettings.UseMedianSmooth)
            {
                for (int i = 0; i < SrcData.SurfaceSettings.MedianSmoothPassCount; ++i)
                {
                    SrcData.SurfaceSettings.SmoothHeightMapColorAdditionalPass();
                }
            }

            int resolution = LPTCommon.PAINTER_MAP_RESOLUTION;
            DesData.Geometry.HeightMapResolution = resolution;
            Color[] heightMapColor = new Color[resolution * resolution];
            Vector2 uv = Vector2.zero;
            float h = 0;
            for (int z = 0; z < resolution; ++z)
            {
                for (int x = 0; x < resolution; ++x)
                {
                    uv.Set(
                        Mathf.InverseLerp(0, resolution - 1, x),
                        Mathf.InverseLerp(0, resolution - 1, z));
                    h = SrcData.SurfaceSettings.GetHeightSample(uv);
                    heightMapColor[GUtilities.To1DIndex(x, z, resolution)] = new Color(h, 0, 0, 0);
                }
            }
            
            DesData.Geometry.Internal_HeightMap.SetPixels(heightMapColor);
            DesData.Geometry.Internal_HeightMap.Apply();
            DesData.Geometry.SetRegionDirty(GCommon.UnitRect);
            DesData.SetDirty(GTerrainData.DirtyFlags.Geometry);
            GC.Collect();
        }

        private void DoImportColors()
        {
            int resolution = LPTCommon.PAINTER_MAP_RESOLUTION;
            DesData.Shading.AlbedoMapResolution = resolution;
            Color[] colors = new Color[resolution * resolution];
            Vector2 uv = Vector2.zero;
            Color c = Color.clear;
            for (int z = 0; z < resolution; ++z)
            {
                for (int x = 0; x < resolution; ++x)
                {
                    uv.Set(
                        Mathf.InverseLerp(0, resolution - 1, x),
                        Mathf.InverseLerp(0, resolution - 1, z));
                    c = SrcData.SurfaceSettings.GetColor(uv);
                    colors[GUtilities.To1DIndex(x, z, resolution)] = c;
                }
            }

            DesData.Shading.Internal_AlbedoMap.SetPixels(colors);
            DesData.Shading.Internal_AlbedoMap.Apply();
            DesData.SetDirty(GTerrainData.DirtyFlags.Shading);
        }

        private void DoImportSplats()
        {
            GSplatPrototypeGroup splatGroup = DesData.Shading.Splats;
            if (splatGroup == null ||
                splatGroup == GGriffinSettings.Instance.TerrainDataDefault.Shading.Splats)
            {
                CreateNewSplatPrototypesGroup = true;
            }

            if (CreateNewSplatPrototypesGroup)
            {
                splatGroup = ScriptableObject.CreateInstance<GSplatPrototypeGroup>();

#if UNITY_EDITOR
                string path = AssetDatabase.GetAssetPath(DesData);
                string directory = Path.GetDirectoryName(path);
                string filePath = Path.Combine(directory, string.Format("Splats_{0}_{1}.asset", DesData.Id, System.DateTime.Now.Ticks));
                AssetDatabase.CreateAsset(splatGroup, filePath);
#endif
                DesData.Shading.Splats = splatGroup;
            }

            List<LPTSplatInfo> layers = SrcData.SurfaceSettings.Splats;
            GSplatPrototype[] prototypes = new GSplatPrototype[layers.Count];
            for (int i = 0; i < layers.Count; ++i)
            {
                GSplatPrototype p = (GSplatPrototype)layers[i];
                prototypes[i] = p;
            }
            splatGroup.Prototypes = new List<GSplatPrototype>(prototypes);

            int controlResolution = SrcData.SurfaceSettings.ControlResolution;
            DesData.Shading.SplatControlResolution = controlResolution;

            Texture[] alphaMaps = SrcData.SurfaceSettings.GetSplatControlTextures();
            int maxControlCount = Mathf.Min(alphaMaps.Length, DesData.Shading.SplatControlMapCount);
            RenderTexture rt = new RenderTexture(controlResolution, controlResolution, 0, RenderTextureFormat.ARGB32);

            for (int i = 0; i < maxControlCount; ++i)
            {
                Texture2D controlMap = DesData.Shading.Internal_GetSplatControl(i);
                GCommon.CopyToRT(alphaMaps[i], rt);
                GCommon.CopyFromRT(controlMap, rt);
                controlMap.filterMode = alphaMaps[i].filterMode;
            }

            rt.Release();
            GUtilities.DestroyObject(rt);

            DesData.SetDirty(GTerrainData.DirtyFlags.Shading);
            GC.Collect();
        }
    }
}
#endif