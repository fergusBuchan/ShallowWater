using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    Camera MainCamera;

    [SerializeField]
    Camera VelocityCamera;
    [SerializeField]
    Shader VelocityShader;
    [SerializeField]
    RenderTexture vectocityRender;

    [SerializeField]
    WaterSurface[] surfaces;
    [SerializeField]
    Texture2D[] startingConditionsList;

    [SerializeField]
    GameObject interactObject;

    [SerializeField]
    RecorderScript Recorder;

    GameObject selectedObject;
    Vector3 offset;
    public int iterator;
    int currentSim;
    int currentConditions;

    // Start is called before the first frame update
    void Start()
    {
        vectocityRender = new RenderTexture(250, 250, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        vectocityRender.enableRandomWrite = true;
        vectocityRender.Create();

        if (VelocityCamera && VelocityShader)
        {
            VelocityCamera.targetTexture = vectocityRender;
            string tag = "";
            VelocityCamera.SetReplacementShader(VelocityShader, tag);
        }
        
        iterator = -1;
        currentSim = 0;
        currentConditions = 0;

        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("right"))
        {
            if (iterator < 11)
            {
                iterator++;
                currentSim = iterator % 3;
                currentConditions = iterator / 3;
                updateSurfaces();
                if (Recorder) { Recorder.ChangeRecording(currentSim); }
            }
        }
        else if (Input.GetKeyDown("left"))
        {
            if (iterator > 0) {
                iterator--;
                currentSim = iterator % 3;
                currentConditions = iterator / 3;
                updateSurfaces();
                if (Recorder) { Recorder.ChangeRecording(currentSim); }
            }
        }
        // this is how we chnage the simualtion on screen


        if (Input.GetKeyDown("r"))
        {
            updateSurfaces();
        }
        // this restarts the current sim

        if (interactObject)
        {
            surfaces[currentSim].SetSplashMap(vectocityRender);
        }
        // this is us sending the information from the velocity shader to the current sim
    }

    void updateSurfaces()
    {
        for (int i = 0; i < surfaces.Length; i ++)
        {
            surfaces[i].gameObject.SetActive(false);
        }
        surfaces[currentSim].gameObject.SetActive(true);
        surfaces[currentSim].SetConditions(startingConditionsList[currentConditions]);
    }

}
