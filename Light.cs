using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    public float velocityX;
    public float velocityZ;
    public float velocityY;
    public float polarizationAngle;
    public float rads;
    public float bamp;
    public float amplitude;
    public float currentIndRef = 1;
    public float iing;
    public float jing;
    public float ling;
    public float king;
    public MainController mainner;
    public int counter;
    public bool split;
    public GameObject middle;
    public Vector3 focal = new Vector3(0, 0, 0);
    public bool lens = false;
    public bool testing = false;
    public Vector3 centerDistAmpl;
    public refractingSurface testSurface;

    [SerializeField] bool onset = false;

    float time = 3;
    bool set = false;
    public float angleUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    float zeroOut(float numb, float compare) {
        if (compare < 0)
        {
            return Mathf.Abs(numb) * -1;
        }
        else
        {
            return Mathf.Abs(numb);
        }
    } 
    // Update is called once per frame
    void Update()
    {
        if (!testing) {
            if (mainner.emit)
            {
                if (lens)
                {
                    if (!set)
                    {
                        float totalVelocity = Mathf.Sqrt(Mathf.Pow(velocityY, 2) + Mathf.Pow(velocityZ, 2) + Mathf.Pow(velocityX, 2));
                        if (Mathf.Abs(focal.x) != Mathf.Infinity)
                        {
                            angleUp = Mathf.Atan(centerDistAmpl.y / Mathf.Sqrt(Mathf.Pow(centerDistAmpl.x, 2) + Mathf.Pow(centerDistAmpl.z, 2)));
                            float angleSide = Mathf.Atan(centerDistAmpl.z / centerDistAmpl.x);
                            velocityY = Mathf.Sin(angleUp) * totalVelocity;
                            float velocityXZ = Mathf.Cos(angleUp) * totalVelocity;
                            velocityX = Mathf.Cos(angleSide) * velocityXZ;
                            velocityZ = Mathf.Sin(angleSide) * velocityXZ;
                        }
                        else {
                            velocityY = 0;
                            velocityX = Mathf.Cos(middle.transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * totalVelocity;
                            velocityZ = Mathf.Sin(middle.transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * totalVelocity;
                        }
                        velocityY = zeroOut(velocityY, centerDistAmpl.y);
                        velocityX = zeroOut(velocityX, centerDistAmpl.x);
                        velocityZ = zeroOut(velocityZ, centerDistAmpl.z);
                        set = true;
                        lens = false;
                    }
                }
                else
                {
                    set = false;
                }
                if (transform.position.y < 0)
                {
                    Destroy(this.gameObject);
                }
                if (Vector3.Distance(mainner.centralBase.transform.position, transform.position) > 10)
                {
                    Destroy(this.gameObject);
                }
                transform.Translate(new Vector3(velocityX * Time.deltaTime, velocityY * Time.deltaTime, velocityZ * Time.deltaTime));
                if (split)
                {
                    if (time <= 0)
                    {
                        split = false;
                        time = 4;
                    }
                    time -= Time.deltaTime;
                }
            }
        }
        else {
            transform.Translate(new Vector3(velocityX * Time.deltaTime, velocityY * Time.deltaTime, velocityZ * Time.deltaTime));
        }
    }
    private void OnCollisionExit(Collision other) {
        Debug.Log("In collider");
        if (testing) {
            if (other.gameObject.tag == "boundry") {
                testSurface.posX.Add(transform.position.x);
                testSurface.posY.Add(transform.position.y);
                testSurface.posZ.Add(transform.position.z);
                testSurface.velX.Add(velocityX);
                testSurface.velY.Add(velocityY);
                testSurface.velZ.Add(velocityZ);
                testSurface.angleI.Add(iing);
                testSurface.angleJ.Add(jing);
                testSurface.angleK.Add(king);
                testSurface.angleL.Add(ling);
                this.gameObject.SetActive(false);
            } else if (other.gameObject.tag == "lens") {
                Vector3 normal = other.contacts[0].normal;
                float angleUp = Mathf.Atan(normal.y / (Mathf.Sqrt(Mathf.Pow(normal.x, 2) + Mathf.Pow(normal.z, 2))));
                float angleSide = Mathf.Atan(normal.x / normal.z);
                float lightUp = Mathf.Atan(velocityY / (Mathf.Sqrt(Mathf.Pow(velocityX, 2) + Mathf.Pow(velocityZ, 2))));
                float lightSide = Mathf.Atan(velocityX / velocityZ);
                ItemManager otherManage = other.gameObject.GetComponent<ItemManager>();
                float otherAngle = 0f;
                float.TryParse(otherManage.sections[otherManage.indexRefIndex].text, out otherAngle);
                float newAngle = Mathf.Asin((otherAngle / currentIndRef) * Mathf.Sin(angleUp - lightUp));
                float newSide = Mathf.Asin((otherAngle / currentIndRef) * Mathf.Sin(angleSide - lightSide));
                float totalVel = Mathf.Sqrt(Mathf.Pow(velocityX, 2) + Mathf.Pow(velocityY, 2) + Mathf.Pow(velocityZ, 2));
                velocityY = Mathf.Sin(newAngle) * totalVel;
                float velocityXZ = Mathf.Cos(newAngle) * totalVel;
                velocityX = Mathf.Cos(newSide) * velocityXZ;
                velocityZ = Mathf.Sin(newSide) * velocityXZ;
            }
        }    
    }
}
