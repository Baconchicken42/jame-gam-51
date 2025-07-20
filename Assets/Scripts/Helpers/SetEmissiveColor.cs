using UnityEngine;

public class SetEmissiveColor : MonoBehaviour
{
    public Color colorDefault = Color.black;
    public Color colorFail = Color.red;
    public Color colorSuccess = Color.limeGreen;
    public float emissiveIntensity;
    [Space(10)] public Renderer rend;

    MaterialPropertyBlock colorPropertyBlock;
    MaterialPropertyBlock emissivePropertyBlock;


    private void Awake()
    {
        colorPropertyBlock = new MaterialPropertyBlock();
        emissivePropertyBlock = new MaterialPropertyBlock();

        SetDefault();
    }

    [ContextMenu("SetSuccess")]
    public void SetSuccess()
    {
        SetColor(colorSuccess);
    }

    [ContextMenu("SetFail")]
    public void SetFail()
    {
        SetColor(colorFail);
    }

    [ContextMenu("SetDefault")]
    public void SetDefault()
    {
        SetColor(colorDefault);
    }

    void SetColor(Color color)
    {

        rend.GetPropertyBlock(colorPropertyBlock);
        colorPropertyBlock.SetColor("_FirstColor", color);
        rend.SetPropertyBlock(colorPropertyBlock);


        rend.GetPropertyBlock(emissivePropertyBlock);
        emissivePropertyBlock.SetColor("_EmissiveTint", color * emissiveIntensity);
        rend.SetPropertyBlock (emissivePropertyBlock);
    }

}
