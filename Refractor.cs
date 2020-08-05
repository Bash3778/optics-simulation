using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Refractor : MonoBehaviour
{

    PieceController idSetting;


    void Start()
    { 
        idSetting = this.GetComponent<PieceController>();
    } 
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Light")
        {
            float indexR = idSetting.values[3];
            float rotation = idSetting.values[2];
            GameObject center = this.transform.GetChild(0).gameObject;
            Light lightScript = other.gameObject.GetComponent<Light>();
            float lightAngle = Mathf.Tan(lightScript.velocityZ / lightScript.velocityX) * Mathf.Rad2Deg;
            float angle = rotation + lightAngle;
            float angleOut = 0;
            if (indexR != 0)
            {
                angleOut = angle / indexR;
            }
            else {
                angleOut = lightAngle;
            }
            float totalVel = Mathf.Sqrt(Mathf.Pow(lightScript.velocityX, 2) + Mathf.Pow(lightScript.velocityZ, 2));
            lightScript.velocityX = Mathf.Cos(angleOut * Mathf.Deg2Rad) * totalVel;
            lightScript.velocityZ = Mathf.Sin(angleOut * Mathf.Deg2Rad) * totalVel;
        }
    }
}
