using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightShadowToggle : MonoBehaviour
{
    public Light2D playerLight;
    public float lampCheckRadius = 3f;
    public LayerMask lampLayer;

    void Update()
    {
        bool nearLamp = Physics2D.OverlapCircle(transform.position, lampCheckRadius, lampLayer);

        playerLight.shadowIntensity = nearLamp ? 0f : 1f;
    }
}
