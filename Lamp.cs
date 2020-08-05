using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Lamp : MonoBehaviour
{

    [SerializeField] GameObject main;
    [SerializeField] GameObject lightObject;
    [SerializeField] Transform physical;
    [SerializeField] Transform geometric;

    float timer;
    float aTimer;
    float bWave;
    float bAmp;
    float tIngeral;
    float waveSpeed;
    int counter = 0;

    MainController scripter;
    PieceController idSetting;
    // Start is called before the first frame update
    void Start()
    {
        scripter = main.GetComponent<MainController>();
        idSetting = this.GetComponent<PieceController>();
        tIngeral = idSetting.timerInt;
        bWave = idSetting.baseWavelength;
        bAmp = idSetting.baseWavelength;
        waveSpeed = idSetting.waveSpeed;
    }
    void lightEmision()
    {
        if (scripter.emit)
        {
            if (scripter.mode == 0)
            {
                float wavelength = idSetting.values[3];
                float rotation = idSetting.values[2];
                float degreeOpening = idSetting.values[5];
                float amplitude = idSetting.values[4];
                float density = idSetting.values[6];

                if (wavelength / timer == 1 || timer > wavelength)
                {
                    for (float i = rotation - (degreeOpening / 2); i < rotation + (degreeOpening / 2); i += density)
                    {
                        GameObject obj = Instantiate(lightObject);
                        float ypos = Mathf.Sin(aTimer) * (amplitude / bAmp);
                        aTimer += Mathf.PI / 16;
                        obj.transform.position = new Vector3(transform.position.x, ypos + transform.position.y, transform.position.z);
                        Light objScript = obj.GetComponent<Light>();
                        objScript.velocityX = Mathf.Sin(i * Mathf.Deg2Rad) * waveSpeed;
                        objScript.velocityZ = Mathf.Cos(i * Mathf.Deg2Rad) * waveSpeed;
                        objScript.mainner = scripter;
                        objScript.rads = aTimer;
                        objScript.bamp = bAmp;
                        objScript.amplitude = amplitude;
                        objScript.polarizationAngle = 0f;
                        objScript.counter = counter;
                        counter++;
                        obj.transform.SetParent(physical);
                        obj.SetActive(true);
                    }
                    timer = 0;
                }
                else if (wavelength != 0 && wavelength >= tIngeral)
                {
                    timer += tIngeral;
                }
                if (timer > wavelength)
                {
                    timer = 0;
                }
            }
            else {

            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        lightEmision();
    }
}
