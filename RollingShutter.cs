using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RollingShutter : MonoBehaviour
{
    public Camera sceneCamera;
    public Camera compositorCamera;
    public Material maskMaterial;
    public int temporalResolution = 100;
    public RenderTextureFormat textureFormat = RenderTextureFormat.RGB565;

    private CommandBuffer commandBuffer;
    private List<RenderTexture> renderTextures = new List<RenderTexture>();

    private void Start()
    {
        QualitySettings.vSyncCount = 2;
        Application.targetFrameRate = 60;
        int w = Screen.width;
        int h = Screen.height;
        for (int i = 0; i < temporalResolution; i++)
        {
            renderTextures.Add(new RenderTexture(w, h, 0, textureFormat, 0));
        }
        commandBuffer = new CommandBuffer();
        commandBuffer.name = "Rolling shutter effect";
        compositorCamera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
    }

    void Update()
    {
        renderTextures.Insert(0, renderTextures[renderTextures.Count - 1]);
        renderTextures.RemoveAt(renderTextures.Count - 1);
        sceneCamera.targetTexture=renderTextures[0];

        commandBuffer.Clear();
        for (int i = 0; i < temporalResolution; i++)
        {
            commandBuffer.SetGlobalFloat("_MaskPercentage",(float)i/temporalResolution);
            commandBuffer.Blit(renderTextures[i],BuiltinRenderTextureType.CameraTarget,maskMaterial);//the target is the screen
        }
    }
}
