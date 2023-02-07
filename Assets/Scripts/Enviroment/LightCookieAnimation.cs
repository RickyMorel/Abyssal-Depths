using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class LightCookieAnimation : MonoBehaviour
{
    public Vector2 speed = new();
    private UniversalAdditionalLightData lightData;

    // Start is called before the first frame update
    void Start()
    {
        lightData = GetComponent<UniversalAdditionalLightData>();

        if(lightData == null)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        lightData.lightCookieOffset += speed * Time.deltaTime;
    }
}
