using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Laser : MonoBehaviour
{
    [SerializeField] GameObject lightObject;
    [SerializeField] MainController scripter;
    [SerializeField] Transform physical;
    [SerializeField] Transform geometric;

    PieceController idSetting;

    float timer;
    float aTimer;
    float bWave;
    float bAmp;
    float tIngeral;
    float waveSpeed;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        idSetting = this.GetComponent<PieceController>();
        tIngeral = idSetting.timerInt;
        bWave = idSetting.baseWavelength;
        bAmp = idSetting.baseWavelength;
        waveSpeed = idSetting.waveSpeed;
    }
    void lightEmision() {
        if (scripter.emit)
        {
            if (scripter.mode == 0)
            {
                float rotation = idSetting.values[2];
                float wavelength = idSetting.values[4];
                float polarization = idSetting.values[3];
                float amplitude = idSetting.values[5];
                if (wavelength / timer == 1 || timer > wavelength)
                {
                    GameObject center = this.transform.GetChild(0).gameObject;
                    GameObject obj = Instantiate(lightObject);
                    float ypos = Mathf.Sin(aTimer) * (amplitude / bAmp);
                    float xzpos = Mathf.Sin(polarization * Mathf.Deg2Rad) * ypos;
                    if (ypos < 0)
                    {
                        ypos = Mathf.Sqrt(Mathf.Pow(ypos, 2) - Mathf.Pow(xzpos, 2)) * -1;
                    }
                    else
                    {
                        ypos = Mathf.Sqrt(Mathf.Pow(ypos, 2) - Mathf.Pow(xzpos, 2));
                    }
                    float xpos = xzpos * Mathf.Cos(rotation * Mathf.Deg2Rad);
                    float zpos = xzpos * Mathf.Sin(rotation * Mathf.Deg2Rad);
                    aTimer += Mathf.PI / 16;
                    obj.transform.position = new Vector3(center.transform.position.x + xpos, ypos + center.transform.position.y, zpos + center.transform.position.z);
                    Light objScript = obj.GetComponent<Light>();
                    objScript.velocityX = Mathf.Sin(rotation * Mathf.Deg2Rad) * waveSpeed;
                    objScript.velocityZ = Mathf.Cos(rotation * Mathf.Deg2Rad) * waveSpeed;
                    objScript.mainner = scripter;
                    objScript.rads = aTimer;
                    objScript.bamp = bAmp;
                    objScript.amplitude = amplitude;
                    objScript.polarizationAngle = polarization;
                    objScript.counter = counter;
                    counter++;
                    obj.transform.SetParent(physical);
                    obj.SetActive(true);
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