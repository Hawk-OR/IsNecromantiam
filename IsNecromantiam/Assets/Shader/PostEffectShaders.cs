using System.Collections.Generic;
using UnityEngine;

public class PostEffectShaders : MonoBehaviour
{
    [SerializeField] Material m_Materials = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_Materials);
    }
}
