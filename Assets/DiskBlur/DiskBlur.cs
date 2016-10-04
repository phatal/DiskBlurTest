using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DiskBlur : MonoBehaviour
{
    public enum SampleCount { Low, Medium, High, VeryHigh }

    [SerializeField] SampleCount _sampleCount = SampleCount.Medium;
    [SerializeField] float _scale = 1;

    [SerializeField, HideInInspector] Shader _shader;

    Material _material;

    void OnEnable()
    {
        if (_material == null)
        {
            _material = new Material(Shader.Find("Hidden/DiskBlur"));
            _material.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void OnDestroy()
    {
        if (_material != null)
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        const int downscale = 4;

        var width = source.width;
        var height = source.height;
        var format = source.format;

        var rt1 = RenderTexture.GetTemporary(width / downscale, height / downscale, 0, format);
        var rt2 = RenderTexture.GetTemporary(width / downscale, height / downscale, 0, format);

        _material.SetFloat("_DownsampleRatio", downscale);
        Graphics.Blit(source, rt1, _material, 0);

        _material.SetFloat("_Scale", _scale);
        Graphics.Blit(rt1, rt2, _material, 1 + (int)_sampleCount);

        Graphics.Blit(rt2, destination);

        RenderTexture.ReleaseTemporary(rt1);
        RenderTexture.ReleaseTemporary(rt2);
    }
}