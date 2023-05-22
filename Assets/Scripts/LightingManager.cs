using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    private Light m_directionalLight;

    [SerializeField]
    public Gradient AmbientColor;

    [SerializeField]
    public Gradient DirectionalColor;

    [SerializeField]
    public Gradient FogColor;

    private GameController cc_gameController;

    // Start is called before the first frame update
    void Start()
    {
        cc_gameController = GameObject.Find("GameController").GetComponent<GameController>();

        m_directionalLight = gameObject.GetComponent<Light>();
        Debug.Assert(m_directionalLight != null, "Must be attached to a directional light");

    }

    // Update is called once per frame
    void Update()
    {
        float m_timeOfDay = this.cc_gameController.timeOfDay;
        UpdateLighting(m_timeOfDay / 24f);
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = FogColor.Evaluate(timePercent);

        if (m_directionalLight)
        {
            m_directionalLight.color = DirectionalColor.Evaluate(timePercent);
            m_directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }
}
