using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Polarizer : MonoBehaviour
{
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
            float polarization = idSetting.values[3];
            GameObject center = this.transform.GetChild(0).gameObject;
            Light lightScript = other.gameObject.GetComponent<Light>();
            float angle = Mathf.Abs(-1 * lightScript.polarizationAngle + polarization);
            float dist = Vector3.Distance(center.transform.position, other.transform.position) * Mathf.Cos(angle * Mathf.Deg2Rad);
            float xzoff = Vector2.Distance(new Vector2(center.transform.position.x, center.transform.position.z), new Vector2(other.transform.position.x, other.transform.position.z));
            float xoff = center.transform.position.x - other.transform.position.x;
            float zoff = center.transform.position.z - other.transform.position.z;
            float yoff = center.transform.position.y - other.transform.position.y;
            float ydist = Mathf.Cos(polarization * Mathf.Deg2Rad) * dist;
            float xzdist = Mathf.Sin(polarization * Mathf.Deg2Rad) * dist;
            if (Mathf.Abs(yoff) > xzoff)
            {
                if (yoff > 0)
                {
                    ydist *= -1;
                    xzdist *= -1;
                }
            }
            else
            {
                if ((xoff < 0 && zoff < 0) || (xoff > 0 && zoff > 0))
                {
                    xzdist *= -1;
                    ydist *= -1;
                }
            }
            float xdist = Mathf.Cos(rotation * Mathf.Deg2Rad) * xzdist;
            float zdist = Mathf.Sin(rotation * Mathf.Deg2Rad) * xzdist;
            if (Mathf.Abs(Mathf.Abs(lightScript.polarizationAngle - polarization) - 90) < 0.0001) {
                other.gameObject.SetActive(false);
            }
            if (Mathf.Abs(lightScript.polarizationAngle) != Mathf.Abs(polarization))
            {
                other.transform.position = new Vector3(center.transform.position.x + xdist, ydist + center.transform.position.y, center.transform.position.z + zdist);
            }
            lightScript.polarizationAngle = polarization;
        }
    }
}