using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Retarder : MonoBehaviour
{
    [SerializeField] float slightDialation = 1.5f;

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
            float Degrees = idSetting.values[3];
            float Radians = idSetting.values[4];
            Light otherScript = other.gameObject.GetComponent<Light>();
            GameObject center = this.transform.GetChild(0).gameObject;
            float yoff = other.transform.position.y - center.transform.position.y;
            float xoff = other.transform.position.x - center.transform.position.x;
            float zoff = other.transform.position.z - center.transform.position.z;
            float dist = Vector3.Distance(other.transform.position, center.transform.position);
            float ydist = Mathf.Sin(otherScript.rads) * (otherScript.amplitude / otherScript.bamp) * Mathf.Sin((otherScript.polarizationAngle + Degrees) * Mathf.Deg2Rad);
            float xzdist = Mathf.Sin(otherScript.rads + Radians) * (otherScript.amplitude / otherScript.bamp) * Mathf.Cos((otherScript.polarizationAngle + Degrees) * Mathf.Deg2Rad);
            float ypos = ydist * slightDialation;
            float xpos = Mathf.Cos(rotation * Mathf.Deg2Rad) * xzdist * slightDialation;
            float zpos = Mathf.Sin(rotation * Mathf.Deg2Rad) * xzdist * slightDialation;
            other.transform.position = new Vector3(center.transform.position.x + xpos, center.transform.position.y + ypos, center.transform.position.z + zpos);
            otherScript.polarizationAngle = Mathf.Asin(ypos / Mathf.Sqrt(Mathf.Pow(ypos, 2) + Mathf.Pow(xpos, 2) + Mathf.Pow(zpos, 2))) * Mathf.Rad2Deg;
        }
    }
}
