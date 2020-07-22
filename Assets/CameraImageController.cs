using UnityEngine;
using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraImageController : MonoBehaviour
{
    public ARCameraManager cameraManager;

    Texture2D mTexture;
    private MeshRenderer mRenderer;

    private void Start()
    {
        mRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        cameraManager.frameReceived += OnCameraFrameReceied;
    }

    private void OnDisable()
    {
        cameraManager.frameReceived -= OnCameraFrameReceied;
    }

    unsafe void OnCameraFrameReceied(ARCameraFrameEventArgs eventArgs){
        XRCpuImage image;
        if (!cameraManager.TryGetLatestImage(out image))
            return;

        var conversionParams = new XRCpuImage.ConversionParams
            (
             image,
             TextureFormat.RGBA32,
             XRCpuImage.Transformation.None
            );

        if(mTexture == null || mTexture.width != image.width || mTexture.height != image.height)
        {
            mTexture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                conversionParams.outputFormat,
                false);
        }

        var buffer = mTexture.GetRawTextureData<byte>();
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

        mTexture.Apply();
        mRenderer.material.mainTexture = mTexture;

        buffer.Dispose();
        image.Dispose();

    }
}
