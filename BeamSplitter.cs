using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BeamSplitter: MonoBehaviour
{
    [SerializeField] GameObject lightObject;
    int last;

    MainController scripter;
    PieceController idSetting;
    // Start is called before the first frame update
    void Start()
    {
        idSetting = this.GetComponent<PieceController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Light")
        {
            float rotation = idSetting.values[2];
            Light otherScript = other.gameObject.GetComponent<Light>();
            if (!otherScript.split)
            {
                last = otherScript.counter;
                otherScript.split = true;
                GameObject center = this.transform.GetChild(0).gameObject;
                float totalVelocity = Mathf.Sqrt(Mathf.Pow(otherScript.velocityX, 2) + Mathf.Pow(otherScript.velocityZ, 2));
                GameObject newLight = Instantiate(lightObject);
                float zvel = Mathf.Sin(rotation * Mathf.Deg2Rad) * totalVelocity;
                float xvel = Mathf.Cos(rotation * Mathf.Deg2Rad) * totalVelocity;
                if (otherScript.velocityX < 0)
                {
                    xvel = xvel * -1;
                }
                if (otherScript.velocityZ < 0)
                {
                    zvel = zvel * -1;
                }
                float otherAngle = Mathf.Atan2(otherScript.velocityZ, otherScript.velocityX) * Mathf.Rad2Deg;
                Light newScript = newLight.GetComponent<Light>();
                newScript.velocityZ = zvel;
                newScript.velocityX = xvel;
                otherScript.velocityX = zvel * -1;
                otherScript.velocityZ = xvel;
                float netAngle = rotation - otherAngle;
                float xdist = other.transform.position.x - center.transform.position.x;
                float zdist = other.transform.position.z - center.transform.position.z;
                float xzdist = Mathf.Sqrt(Mathf.Pow(xdist, 2) + Mathf.Pow(zdist, 2));
                float xpos = Mathf.Cos((rotation + netAngle) * Mathf.Deg2Rad) * xzdist;
                float zpos = Mathf.Sin((rotation + netAngle) * Mathf.Deg2Rad) * xzdist;
                float xprime = Mathf.Cos(rotation * Mathf.Deg2Rad) * xzdist;
                float zprime = Mathf.Sin(rotation * Mathf.Deg2Rad) * xzdist;
                if (zdist < 0)
                {
                    xpos = xpos * -1;
                    zprime = zprime * -1;
                }
                if (xdist < 0)
                {
                    zpos = zpos * -1;
                    xprime *= -1;
                }
                other.gameObject.transform.position = new Vector3(center.transform.position.x + xprime, other.transform.position.y, center.transform.position.z + zprime);
                newLight.transform.position = new Vector3(center.transform.position.x + xpos, other.transform.position.y, center.transform.position.z + zpos);
                newScript.split = true;
                newScript.polarizationAngle = otherScript.polarizationAngle;
                newScript.rads = otherScript.rads;
                newScript.bamp = otherScript.bamp;
                newScript.amplitude = otherScript.amplitude;
                newScript.mainner = otherScript.mainner;
                newScript.counter = otherScript.counter;
                newLight.SetActive(true);
            }
        }
    }
}
