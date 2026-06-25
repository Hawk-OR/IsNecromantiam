using UnityEngine;
using UnityEngine.Rendering;

public class DepthOutline : MonoBehaviour
{
    [SerializeField]
    private Shader _shader;
    [SerializeField]
    private float _outlineThreshold = 0.01f;
    [SerializeField]
    private Color _outlineColor = Color.white;
    [SerializeField]
    private float _outlineThick = 1.0f;

    private Material _material;

    private void Awake()
    {
        Initialize();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        SetMaterialProperties();
#endif
    }

    private void Initialize()
    {
        var camera = GetComponent<Camera>();
        camera.depthTextureMode |= DepthTextureMode.Depth;

        if (camera.allowMSAA || camera.allowHDR)
        {
            return;
        }

        _material = new Material(_shader);
        SetMaterialProperties();

        var commandBuffer = new CommandBuffer();
        int tempTextureIdentifier = Shader.PropertyToID("_PostEffectTempTexture");
        commandBuffer.GetTemporaryRT(tempTextureIdentifier, -1, -1);
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, tempTextureIdentifier);
        commandBuffer.Blit(tempTextureIdentifier, BuiltinRenderTextureType.CurrentActive, _material);
        commandBuffer.ReleaseTemporaryRT(tempTextureIdentifier);
        camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
    }

    private void SetMaterialProperties()
    {
        if (_material != null)
        {
            _material.SetFloat("_OutlineThreshold", _outlineThreshold);
            _material.SetColor("_OutlineColor", _outlineColor);
            _material.SetFloat("_OutlineThick", _outlineThick);
        }
    }
}
