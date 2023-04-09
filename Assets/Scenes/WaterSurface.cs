using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;



public class WaterSurface : MonoBehaviour
{
    private ProfilerMarker totalMarker;
    private ProfilerMarker[] MarkerArray;

    [SerializeField]
    Camera cam;

    [SerializeField]
    string computeShaderName;
    [SerializeField] 
    int passes;
    [SerializeField] 
    Vector2Int[] particleMapDims;
    // these values are set in the editor since they change based on what algorthm we're using to simulate the water

    const int kernalX = 8;
    const int kernalY = 8;

    public ComputeShader waterSim;

    public Texture2D startingConditions;
    // this is the texture we add images to from the editor
    public List<RenderTexture> waterSimParticles;
    // this texutre holds detailed infromation about the entire sim
    public RenderTexture heightMap;
    // this texture holds only the infromation on the current step (no half step values)
    public RenderTexture splashMap;
    // this texture holds the information of the objects' interactions with it's surface

    void Start()
    {
        totalMarker = new ProfilerMarker("CUSTOM total pass " + computeShaderName);
        MarkerArray = new ProfilerMarker[passes + 1];
        for (int i = 0; i < MarkerArray.Length; i ++)
        {
            string markerName = "CUSTOM pass " + i.ToString() + " " + computeShaderName;
            MarkerArray[i] = new ProfilerMarker(markerName);
        }

        CreateWaterSimTexture(startingConditions.width / 4, startingConditions.height / 4);
        // create an image to store suff in, easier than 2d arrays
        SetBuffers();
    }

    // Update is called once per frame
    void Update()
    {

        // if detatime is too high the sim gives bad results (especially when it's just finnished loading)
        float dt = Mathf.Clamp(Time.deltaTime, 0f, 0.03f);

        totalMarker.Begin();
        //this is a marker for to measure preformance

        waterSim.SetFloat("deltaTime", dt);
        //we need to update the GPUs delta time

        for (int i = 0; i < passes; i++)
        {
            MarkerArray[i].Begin();

            waterSim.Dispatch(i + 1, heightMap.width / kernalX, heightMap.height / kernalY, 1);
            
            MarkerArray[i].End();
        }
        // this is us running all the passes on the compute shader 
        if (splashMap)
        {
            MarkerArray[MarkerArray.Length - 1].Begin();

            waterSim.SetTexture(3, "splashTexture_0", splashMap);
            waterSim.Dispatch(3, heightMap.width / kernalX, heightMap.height / kernalY, 1);

            MarkerArray[MarkerArray.Length - 1].End();
        }
        // this is the special 'splash pass' where we add interations from physics objects

        totalMarker.End();

        SetTexture(heightMap);
        // this is us setting a new texture for the plane

    }

    void CreateWaterSimTexture(int xRes,int yRes)
    {
        heightMap = new RenderTexture(xRes, xRes, 24);//, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        heightMap.enableRandomWrite = true;
        heightMap.Create();

        waterSimParticles = new List<RenderTexture>();

        for (int i = 0; i < particleMapDims.Length; i ++) {
            int ScaledResX = xRes * particleMapDims[i].x;
            int ScaledResY = yRes * particleMapDims[i].y;
            RenderTexture thisTexture = new RenderTexture(ScaledResX, ScaledResY, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            thisTexture.enableRandomWrite = true;
            thisTexture.Create();
            waterSimParticles.Add(thisTexture);
        }
        // here we add the textures the shader uses, using the data that has been provided in the editor


        Graphics.Blit(startingConditions, waterSimParticles[0]);
        // this copies our starting texture to the input texture
    }

    public void SetConditions(Texture2D inTex)
    {
        if (waterSimParticles[0]) {
            Graphics.Blit(inTex, waterSimParticles[0]);
            waterSim.Dispatch(0, waterSimParticles[0].width / kernalX, waterSimParticles[0].height / kernalY, 1);
        }
    }//resets the sim to the 'inTex' conditions, it's public so we can change it in the manager

    public void SetSplashMap(RenderTexture inRenderTexture)
    {
        splashMap = inRenderTexture;
    }//sets the spash map

    void SetBuffers()
    {
        waterSim.SetFloat("dimX", heightMap.width);
        waterSim.SetFloat("dimY", heightMap.height);

        waterSim.SetTexture(0, "waterSimParticles_0", waterSimParticles[0]);
        waterSim.Dispatch(0, waterSimParticles[0].width / kernalX, waterSimParticles[0].height / kernalY, 1);
        //pass 0 is run once to set the inital values; it only needs one texture

        for (int i = 1; i < passes + 1; i ++)
        {
            for (int j = 0; j < waterSimParticles.Count; j++)
            {
                waterSim.SetTexture(i, "waterSimParticles_" + j.ToString(), waterSimParticles[j]);
            }
        }
        // here we set the textures in our sim, it's modular so that a sim with any number of passes or textues can be used
        waterSim.SetTexture(3, "waterSimParticles_0", waterSimParticles[0]);


        waterSim.SetTexture(passes, "hieghtMap", heightMap);
        // the hieghtmap is what we output to, so it should only be needed in the last pass
    }

    //this is to set the texture of the object the script is attached to, the shader attached to the material should use it as a heightmap
    void SetTexture(Texture inTexture)
    {
        Renderer thisRenderer = GetComponent<Renderer>();
        thisRenderer.material.SetTexture("_MainTex", inTexture);
        thisRenderer.material.SetTexture("_HeightMap", inTexture);
    }

}
