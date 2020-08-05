using UnityEngine;
using UnityEngine.UI;

public class Lens : MonoBehaviour
{
    [SerializeField] GameObject setting;
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
            float concavity = idSetting.values[3];
            if (concavity == 0) {
                concavity = 0.000001f;
            }
            GameObject center = this.transform.GetChild(0).gameObject;
            Light lightScript = other.gameObject.GetComponent<Light>();
            float focalDist = 0;
            if (lightScript.focal == Vector3.zero)
            {
                focalDist = concavity;
            }
            else
            {
                focalDist = Mathf.Pow(Mathf.Pow(concavity, -1) - Mathf.Pow(Vector3.Distance(lightScript.focal, center.transform.position), -1), -1);
            }
            float xoff = Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * focalDist;
            float zoff = Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * focalDist;
            if (focalDist == 0)
            {
                focalDist = Mathf.Infinity;
                lightScript.focal = new Vector3(Mathf.Infinity, 0, Mathf.Infinity);
            }
            else {
                lightScript.focal = new Vector3(center.transform.position.x - xoff, center.transform.position.y, center.transform.position.z + zoff);
            }
            lightScript.middle = this.gameObject;
            lightScript.centerDistAmpl = new Vector3(lightScript.focal.x - other.transform.position.x, lightScript.focal.y - other.transform.position.y, lightScript.focal.z - other.transform.position.z);
            lightScript.lens = true;
        }
    }
    // Update is called once per frame
}
