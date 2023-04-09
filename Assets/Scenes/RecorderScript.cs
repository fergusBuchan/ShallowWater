using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using System.IO;
using UnityEngine;

public class RecorderScript : MonoBehaviour
{
    ProfilerRecorder[] passRecorders = new ProfilerRecorder[11];
    ProfilerRecorder[] PlayerLoop = new ProfilerRecorder[3];
    ProfilerRecorder[] TotalMemory = new ProfilerRecorder[3];
    ProfilerRecorder[] GfxMemory = new ProfilerRecorder[3];


    string filename;

    int sampleTotal = 3000;

    void Start()
    {
        filename = Application.dataPath + "/stats.csv";

        PlayerLoop[0] = new ProfilerRecorder(ProfilerCategory.Scripts, "PlayerLoop", sampleTotal);
        PlayerLoop[1] = new ProfilerRecorder(ProfilerCategory.Scripts, "PlayerLoop", sampleTotal);
        PlayerLoop[2] = new ProfilerRecorder(ProfilerCategory.Scripts, "PlayerLoop", sampleTotal);

        TotalMemory[0] = new ProfilerRecorder(ProfilerCategory.Memory, "Total Used Memory", sampleTotal);
        TotalMemory[1] = new ProfilerRecorder(ProfilerCategory.Memory, "Total Used Memory", sampleTotal);
        TotalMemory[2] = new ProfilerRecorder(ProfilerCategory.Memory, "Total Used Memory", sampleTotal);

        GfxMemory[0] = new ProfilerRecorder(ProfilerCategory.Memory, "Gfx Used Memory", sampleTotal);
        GfxMemory[1] = new ProfilerRecorder(ProfilerCategory.Memory, "Gfx Used Memory", sampleTotal);
        GfxMemory[2] = new ProfilerRecorder(ProfilerCategory.Memory, "Gfx Used Memory", sampleTotal);


        passRecorders[0] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM total pass LW", sampleTotal);
        passRecorders[1] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM pass 0 LW", sampleTotal);
        passRecorders[2] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM pass 1 LW", sampleTotal);
        passRecorders[3] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM pass 2 LW", sampleTotal);

        passRecorders[4] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM total pass LF", sampleTotal);
        passRecorders[5] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM pass 0 LF", sampleTotal);
        passRecorders[6] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM pass 1 LF", sampleTotal);

        passRecorders[7] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM total pass MA", sampleTotal);
        passRecorders[8] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM pass 0 MA", sampleTotal);
        passRecorders[9] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM pass 1 MA", sampleTotal);
        passRecorders[10] = new ProfilerRecorder(ProfilerCategory.Scripts, "CUSTOM pass 2 MA", sampleTotal);
    }

    public void ChangeRecording(int currentSim)
    {
        // we want to seperate stats betweem the different sims
        switch (currentSim) {
            case 0:

                passRecorders[0].Start();
                passRecorders[1].Start();
                passRecorders[2].Start();
                passRecorders[3].Start();

                passRecorders[4].Stop();
                passRecorders[5].Stop();
                passRecorders[6].Stop();

                passRecorders[7].Stop();
                passRecorders[8].Stop();
                passRecorders[9].Stop();
                passRecorders[10].Stop();

                break;
            case 1:

                passRecorders[0].Stop();
                passRecorders[1].Stop();
                passRecorders[2].Stop();
                passRecorders[3].Stop();

                passRecorders[4].Start();
                passRecorders[5].Start();
                passRecorders[6].Start();

                passRecorders[7].Stop();
                passRecorders[8].Stop();
                passRecorders[9].Stop();
                passRecorders[10].Stop();

                break;
            case 2:

                passRecorders[0].Stop();
                passRecorders[1].Stop();
                passRecorders[2].Stop();
                passRecorders[3].Stop();

                passRecorders[4].Stop();
                passRecorders[5].Stop();
                passRecorders[6].Stop();

                passRecorders[7].Start();
                passRecorders[8].Start();
                passRecorders[9].Start();
                passRecorders[10].Start();

                break;
        }

        for (int i = 0; i < 3; i ++)
        {
            if (i == currentSim)
            {
                PlayerLoop[i].Start();
                TotalMemory[i].Start();
                GfxMemory[i].Start();
                Debug.Log(TotalMemory[i].CurrentValue);
            }
            else
            {
                PlayerLoop[i].Stop();
                TotalMemory[i].Stop();
                GfxMemory[i].Stop();
            }
        }

    }

    // save stats before close
    void OnApplicationQuit()
    {
        TextWriter writer = new StreamWriter(filename, false);
        writer.WriteLine("GPU:" + "," + "CPU:" +  "," + "RAM:"
                        );
        writer.WriteLine(SystemInfo.graphicsDeviceName + "," + SystemInfo.processorType + "," + SystemInfo.systemMemorySize);


        writer.WriteLine("fps LW" + "," + "fps LW" + "," + "fps LW"
                        );

        for (int i = 0; i < sampleTotal - 1; i++)
        {
            writer.WriteLine(PlayerLoop[0].GetSample(i).Value.ToString() + "," + PlayerLoop[1].GetSample(i).Value.ToString() + "," + PlayerLoop[2].GetSample(i).Value.ToString()
                            );
        }


        writer.WriteLine("total memory LW" + "," + "total memory LW" + "," + "total Memory LW" +
                         "gfx memory LF" + "," + "gfx memory LF" + "," + "gfx Memory LF"
                        );

        for (int i = 0; i < sampleTotal - 1; i++)
        {
            writer.WriteLine(TotalMemory[0].GetSample(i).Value.ToString() + "," + TotalMemory[1].GetSample(i).Value.ToString() + "," + TotalMemory[2].GetSample(i).Value.ToString() + "," +
                             GfxMemory[0].GetSample(i).Value.ToString() + "," + GfxMemory[1].GetSample(i).Value.ToString() + "," + GfxMemory[2].GetSample(i).Value.ToString()
                            );
        }

        writer.WriteLine("total pass LW" + "," + "pass 0 LW" + "," + "pass 1 LW" + "," + "pass 2 LW" + "," +
                         "total pass LF" + "," + "pass 0 LF" + "," + "pass 1 LF" + "," +
                         "total pass MA" + "," + "pass 0 MA" + "," + "pass 1 MA" + "," + "pass 2 MA"
                        );

        for (int i = 0; i < sampleTotal - 1; i++)
        {
            writer.WriteLine(passRecorders[0].GetSample(i).Value.ToString() + "," + passRecorders[1].GetSample(i).Value.ToString() + "," + passRecorders[2].GetSample(i).Value.ToString() + "," + passRecorders[3].GetSample(i).Value.ToString() + "," +
                             passRecorders[4].GetSample(i).Value.ToString() + "," + passRecorders[5].GetSample(i).Value.ToString() + "," + passRecorders[6].GetSample(i).Value.ToString() + "," +
                             passRecorders[7].GetSample(i).Value.ToString() + "," + passRecorders[8].GetSample(i).Value.ToString() + "," + passRecorders[9].GetSample(i).Value.ToString() + "," + passRecorders[10].GetSample(i).Value.ToString()
                            );
        }
        writer.Close();
    }
}
