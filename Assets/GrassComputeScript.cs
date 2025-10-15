// @Minionsart version (fixed)
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class GrassComputeScript : MonoBehaviour
{
    // very slow, but will update always
    public bool autoUpdate;

    // main camera
    private Camera m_MainCamera;

    // grass settings to send to the compute shader
    public SO_GrassSettings currentPresets;

    // interactors
    ShaderInteractor[] interactors = new ShaderInteractor[0];

    // base data lists
    [SerializeField, HideInInspector]
    List<GrassData> grassData = new List<GrassData>();

    // list of all visible grass ids, rest are culled
    List<int> grassVisibleIDList = new List<int>();

    // A state variable to help keep track of whether compute buffers have been set up
    private bool m_Initialized;
    // A compute buffer to hold vertex data of the source mesh
    private ComputeBuffer m_SourceVertBuffer;
    // A compute buffer to hold vertex data of the generated mesh
    private ComputeBuffer m_DrawBuffer;
    // A compute buffer to hold indirect draw arguments
    private ComputeBuffer m_ArgsBuffer;
    // Instantiate the shaders so data belong to their unique compute buffers
    private ComputeShader m_InstantiatedComputeShader;
    // buffer that contains the ids of all visible instances
    private ComputeBuffer m_VisibleIDBuffer;
    [SerializeField] Material m_InstantiatedMaterial;
    // The id of the kernel in the grass compute shader
    private int m_IdGrassKernel = -1;
    // The x dispatch size for the grass compute shader
    private int m_DispatchSize;
    // compute shader thread group size
    uint threadGroupSize = 1; // default 1 to avoid division by zero

    // The size of one entry in the various compute buffers, size comes from the float3/float2 entrees in the shader
    private const int SOURCE_VERT_STRIDE = sizeof(float) * (3 + 3 + 2 + 3);
    private const int DRAW_STRIDE = sizeof(float) * (3 + 3 + ((3 + 2) * 3));

    // bounds of the total grass 
    Bounds bounds;

    private uint[] argsBufferReset = new uint[5]
   {
        0,  // Number of vertices to render
        1,  // Number of instances to render
        0,
        0,
        0
   };

    // culling tree data ----------------------------------------------------------------------
    CullingTreeNode cullingTree;
    List<Bounds> BoundsListVis = new List<Bounds>();
    List<CullingTreeNode> leaves = new List<CullingTreeNode>();
    Plane[] cameraFrustumPlanes = new Plane[6];
    float cameraOriginalFarPlane;

    // list of -1 to overwrite the grassvisible buffer with
    List<int> empty = new List<int>();

    // speeding up the editor a bit
    Vector3 m_cachedCamPos;
    Quaternion m_cachedCamRot;
    bool m_fastMode;
    int shaderID;

    // max buffer size can depend on platform and your draw stride, you may have to change it
    int maxBufferSize = 2500000;

    public List<GrassData> SetGrassPaintedDataList
    {
        get { return grassData; }
        set { grassData = value; }
    }

#if UNITY_EDITOR
    SceneView view;

    void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnScene;
    }

    void OnScene(SceneView scene)
    {
        view = scene;
        if (!Application.isPlaying)
        {
            if (view.camera != null)
            {
                m_MainCamera = view.camera;
            }
        }
        else
        {
            m_MainCamera = Camera.main;
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (view != null)
            {
                m_MainCamera = view.camera;
            }
        }
        else
        {
            m_MainCamera = Camera.main;
        }
    }
#endif

    private void OnEnable()
    {
        if (m_Initialized)
        {
            OnDisable();
        }

        MainSetup(true);
    }

    void MainSetup(bool full)
    {
#if UNITY_EDITOR
        SceneView.duringSceneGui += this.OnScene;
        if (!Application.isPlaying)
        {
            if (view != null)
            {
                m_MainCamera = view.camera;
            }
        }
#endif
        if (Application.isPlaying)
        {
            m_MainCamera = Camera.main;
        }

        // Don't do anything if resources are not found,
        // or no vertex is put on the mesh.
        if (grassData == null || grassData.Count == 0)
        {
            return;
        }

        if (currentPresets == null || currentPresets.shaderToUse == null || currentPresets.materialToUse == null)
        {
            Debug.LogWarning("Missing Compute Shader/Material in grass Settings", this);
            return;
        }

        // empty array to replace the visible grass with
        PopulateEmptyList(grassData.Count);
        m_Initialized = true;

        // Instantiate the shaders so they can point to their own buffers
        m_InstantiatedComputeShader = Instantiate(currentPresets.shaderToUse);
        m_InstantiatedMaterial = Instantiate(currentPresets.materialToUse);

        int numSourceVertices = grassData.Count;

        int maxBladesPerVertex = Mathf.Max(1, currentPresets.allowedBladesPerVertex);
        int maxSegmentsPerBlade = Mathf.Max(1, currentPresets.allowedSegmentsPerBlade);
        int maxBladeTriangles = maxBladesPerVertex * ((maxSegmentsPerBlade - 1) * 2 + 1);

        // Create compute buffers
        m_SourceVertBuffer = new ComputeBuffer(numSourceVertices, SOURCE_VERT_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        m_SourceVertBuffer.SetData(grassData);

        m_DrawBuffer = new ComputeBuffer(maxBufferSize, DRAW_STRIDE, ComputeBufferType.Append);
        m_ArgsBuffer = new ComputeBuffer(1, argsBufferReset.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        m_VisibleIDBuffer = new ComputeBuffer(grassData.Count, sizeof(int), ComputeBufferType.Structured);

        // Cache the kernel IDs we will be dispatching
        if (m_InstantiatedComputeShader != null)
        {
            try
            {
                m_IdGrassKernel = m_InstantiatedComputeShader.FindKernel("Main");
                // get kernel thread group sizes
                m_InstantiatedComputeShader.GetKernelThreadGroupSizes(m_IdGrassKernel, out threadGroupSize, out _, out _);
                if (threadGroupSize == 0) threadGroupSize = 1; // safety
            }
            catch
            {
                Debug.LogError("Unable to find kernel 'Main' in the compute shader", this);
                m_IdGrassKernel = -1;
                threadGroupSize = 1;
            }
        }

        m_InstantiatedComputeShader.SetBuffer(m_IdGrassKernel, "_SourceVertices", m_SourceVertBuffer);
        m_InstantiatedComputeShader.SetBuffer(m_IdGrassKernel, "_DrawTriangles", m_DrawBuffer);
        m_InstantiatedComputeShader.SetBuffer(m_IdGrassKernel, "_IndirectArgsBuffer", m_ArgsBuffer);
        m_InstantiatedComputeShader.SetBuffer(m_IdGrassKernel, "_VisibleIDBuffer", m_VisibleIDBuffer);
        m_InstantiatedMaterial.SetBuffer("_DrawTriangles", m_DrawBuffer);
        m_InstantiatedComputeShader.SetInt("_NumSourceVertices", numSourceVertices);

        shaderID = Shader.PropertyToID("_PositionsMoving");

        //set once only - safe cast to avoid divide by zero
        m_DispatchSize = Mathf.CeilToInt((float)grassData.Count / (float)threadGroupSize);
        SetGrassDataBase(full);

        if (full)
        {
            UpdateBounds();
        }
        SetupQuadTree(full);
    }

    void UpdateBounds()
    {
        if (grassData == null || grassData.Count == 0) return;

        bounds = new Bounds(grassData[0].position, Vector3.one);
        for (int i = 0; i < grassData.Count; i++)
        {
            Vector3 target = grassData[i].position;
            bounds.Encapsulate(target);
        }

        // DEBUG: if bounds too large, warn developer (helps track pb_Mesh warnings)
        if (bounds.size.magnitude > 500f)
        {
            Debug.LogWarning($"Grass bounds very large ({bounds.size}). Consider re-centering or using local space positions to avoid large triangles.", this);
        }
    }

    void SetupQuadTree(bool full)
    {
        if (full)
        {
            cullingTree = new CullingTreeNode(bounds, currentPresets.cullingTreeDepth);
            cullingTree.RetrieveAllLeaves(leaves);
            for (int i = 0; i < grassData.Count; i++)
            {
                cullingTree.FindLeaf(grassData[i].position, i);
            }
            cullingTree.ClearEmpty();
        }
        else
        {
            GrassFastList(grassData.Count);
            m_VisibleIDBuffer.SetData(grassVisibleIDList);
        }
    }

    void GrassFastList(int count)
    {
        grassVisibleIDList = Enumerable.Range(0, count).ToArray().ToList();
    }

    void PopulateEmptyList(int count)
    {
        empty = new List<int>(count);
        empty.InsertRange(0, Enumerable.Repeat(-1, count));
    }

    void GetFrustumData()
    {
        if (m_MainCamera == null)
            return;

        if (m_cachedCamRot == m_MainCamera.transform.rotation && m_cachedCamPos == m_MainCamera.transform.position && Application.isPlaying)
            return;

        cameraOriginalFarPlane = m_MainCamera.farClipPlane;
        m_MainCamera.farClipPlane = currentPresets.maxDrawDistance;
        GeometryUtility.CalculateFrustumPlanes(m_MainCamera, cameraFrustumPlanes);
        m_MainCamera.farClipPlane = cameraOriginalFarPlane;

        if (!m_fastMode)
        {
            BoundsListVis.Clear();
            if (empty != null && m_VisibleIDBuffer != null)
            {
                m_VisibleIDBuffer.SetData(empty);
            }
            grassVisibleIDList.Clear();
            cullingTree.RetrieveLeaves(cameraFrustumPlanes, BoundsListVis, grassVisibleIDList);
            if (m_VisibleIDBuffer != null && grassVisibleIDList.Count > 0)
                m_VisibleIDBuffer.SetData(grassVisibleIDList);
        }

        m_cachedCamPos = m_MainCamera.transform.position;
        m_cachedCamRot = m_MainCamera.transform.rotation;
    }

    private void OnDisable()
    {
        if (m_Initialized)
        {
            if (Application.isPlaying)
            {
                if (m_InstantiatedComputeShader != null) Destroy(m_InstantiatedComputeShader);
                if (m_InstantiatedMaterial != null) Destroy(m_InstantiatedMaterial);
            }
            else
            {
#if UNITY_EDITOR
                if (m_InstantiatedComputeShader != null) DestroyImmediate(m_InstantiatedComputeShader);
                if (m_InstantiatedMaterial != null) DestroyImmediate(m_InstantiatedMaterial);
#endif
            }

            m_SourceVertBuffer?.Release();
            m_DrawBuffer?.Release();
            m_ArgsBuffer?.Release();
            m_VisibleIDBuffer?.Release();
        }
        m_Initialized = false;
    }

    private void Update()
    {
        if (!Application.isPlaying && autoUpdate && !m_fastMode)
        {
            OnDisable();
            OnEnable();
        }

        if (!m_Initialized) return;

        GetFrustumData();
        SetGrassDataUpdate();

        // reset indirect buffers safely
        m_DrawBuffer.SetCounterValue(0);
        m_ArgsBuffer.SetData(argsBufferReset);

        // safe dispatch size calculation (cast to avoid integer/uint division surprises)
        if (threadGroupSize == 0) threadGroupSize = 1;
        m_DispatchSize = Mathf.CeilToInt((float)grassVisibleIDList.Count / (float)threadGroupSize);
        if (grassVisibleIDList.Count > 0) m_DispatchSize += 1;

        // only dispatch if everything valid
        if (m_DispatchSize > 0 && m_InstantiatedComputeShader != null && m_IdGrassKernel >= 0)
        {
            m_InstantiatedComputeShader.Dispatch(m_IdGrassKernel, m_DispatchSize, 1, 1);
            Graphics.DrawProceduralIndirect(m_InstantiatedMaterial, bounds, MeshTopology.Triangles,
                m_ArgsBuffer, 0, null, null, currentPresets.castShadow, true, gameObject.layer);
        }
    }

    private void SetGrassDataBase(bool full)
    {
        m_InstantiatedComputeShader.SetFloat("_Time", Time.time);
        m_InstantiatedComputeShader.SetFloat("_GrassRandomHeightMin", currentPresets.grassRandomHeightMin);
        m_InstantiatedComputeShader.SetFloat("_GrassRandomHeightMax", currentPresets.grassRandomHeightMax);
        m_InstantiatedComputeShader.SetFloat("_WindSpeed", currentPresets.windSpeed);
        m_InstantiatedComputeShader.SetFloat("_WindStrength", currentPresets.windStrength);

        if (full)
        {
            m_InstantiatedComputeShader.SetFloat("_MinFadeDist", currentPresets.minFadeDistance);
            m_InstantiatedComputeShader.SetFloat("_MaxFadeDist", currentPresets.maxDrawDistance);

            // use new API (already correct) but safe-guard null
            try
            {
                interactors = FindObjectsByType<ShaderInteractor>(FindObjectsSortMode.None);
            }
            catch
            {
                interactors = new ShaderInteractor[0];
            }
        }
        else
        {
            if (grassData.Count > 200000)
            {
                m_InstantiatedComputeShader.SetFloat("_MinFadeDist", 40f);
                m_InstantiatedComputeShader.SetFloat("_MaxFadeDist", 50f);
            }
            else
            {
                m_InstantiatedComputeShader.SetFloat("_MinFadeDist", currentPresets.minFadeDistance);
                m_InstantiatedComputeShader.SetFloat("_MaxFadeDist", currentPresets.maxDrawDistance);
            }
        }

        m_InstantiatedComputeShader.SetFloat("_InteractorStrength", currentPresets.affectStrength);
        m_InstantiatedComputeShader.SetFloat("_BladeRadius", currentPresets.bladeRadius);
        m_InstantiatedComputeShader.SetFloat("_BladeForward", currentPresets.bladeForwardAmount);
        m_InstantiatedComputeShader.SetFloat("_BladeCurve", Mathf.Max(0, currentPresets.bladeCurveAmount));
        m_InstantiatedComputeShader.SetFloat("_BottomWidth", currentPresets.bottomWidth);

        m_InstantiatedComputeShader.SetInt("_MaxBladesPerVertex", currentPresets.allowedBladesPerVertex);
        m_InstantiatedComputeShader.SetInt("_MaxSegmentsPerBlade", currentPresets.allowedSegmentsPerBlade);

        m_InstantiatedComputeShader.SetFloat("_MinHeight", currentPresets.MinHeight);
        m_InstantiatedComputeShader.SetFloat("_MinWidth", currentPresets.MinWidth);
        m_InstantiatedComputeShader.SetFloat("_MaxHeight", currentPresets.MaxHeight);
        m_InstantiatedComputeShader.SetFloat("_MaxWidth", currentPresets.MaxWidth);
        m_InstantiatedMaterial.SetColor("_TopTint", currentPresets.topTint);
        m_InstantiatedMaterial.SetColor("_BottomTint", currentPresets.bottomTint);
    }

    public void Reset()
    {
        m_fastMode = false;
        OnDisable();
        MainSetup(true);
    }

    public void ResetFaster()
    {
        m_fastMode = true;
        OnDisable();
        MainSetup(false);
    }

    private void SetGrassDataUpdate()
    {
        m_InstantiatedComputeShader.SetFloat("_Time", Time.time);
        m_InstantiatedComputeShader.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);

        if (interactors != null && interactors.Length > 0)
        {
            Vector4[] positions = new Vector4[interactors.Length];
            for (int i = 0; i < interactors.Length; i++)
            {
                if (interactors[i] == null) continue;
                positions[i] = new Vector4(interactors[i].transform.position.x, interactors[i].transform.position.y, interactors[i].transform.position.z, interactors[i].radius);
            }
            m_InstantiatedComputeShader.SetVectorArray(shaderID, positions);
            m_InstantiatedComputeShader.SetFloat("_InteractorsLength", interactors.Length);
        }
        else
        {
            m_InstantiatedComputeShader.SetFloat("_InteractorsLength", 0);
        }

        if (m_MainCamera != null)
        {
            m_InstantiatedComputeShader.SetVector("_CameraPositionWS", m_MainCamera.transform.position);
        }
#if UNITY_EDITOR
        else if (view != null)
        {
            m_InstantiatedComputeShader.SetVector("_CameraPositionWS", view.camera.transform.position);
        }
#endif
    }

    void OnDrawGizmos()
    {
        if (currentPresets && currentPresets.drawBounds)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            for (int i = 0; i < BoundsListVis.Count; i++)
            {
                Gizmos.DrawWireCube(BoundsListVis[i].center, BoundsListVis[i].size);
            }
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}

[System.Serializable]
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct GrassData
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 length;
    public Vector3 color;
}
