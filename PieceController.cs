using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceController : MonoBehaviour
{
    public InputField[] fields;
    public float[] values;
    public int id;
    public float baseWavelength;
    public float baseAmplitude;
    public float timerInt;
    public float waveSpeed;
    public MainController scripterObject;

    // xpos 0, zpos 1, rotation 2, everything else follows

    [SerializeField] Camera camera;
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject canvas;
    [SerializeField] Button canvasQuit;
    [SerializeField] Button deleteItem;
    [SerializeField] Text idText;

    bool ready = false;
    bool deleteOn = false;
    // Start is called before the first frame update
    void Start()
    {
        canvasQuit.onClick.AddListener(canvasReset);
        deleteItem.onClick.AddListener(deleteFunc);
        values[0] = transform.position.x;
        values[1] = transform.position.z;
        fields[0].text = values[0].ToString();
        fields[1].text = values[1].ToString();
    }
    void properties()
    {
        int currentCanvas;
        int.TryParse(idText.text, out currentCanvas);
        if (canvas.active == true)
        {
            if (currentCanvas == id)
            {
                if (deleteOn)
                {
                    this.gameObject.SetActive(false);
                    deleteOn = false;
                }
                if (!ready)
                {
                    for (int i = 0; i < fields.Length; i++) {
                        fields[i].text = values[i].ToString();
                    }
                    ready = true;
                }
                else
                {
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (fields[i].text != "") {
                            float.TryParse(fields[i].text, out values[i]);
                        }
                    } 
                }
            }
            else
            {
                ready = false;
            }
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, values[2], transform.rotation.eulerAngles.z);
        if (values[0] != -1000 && values[1] != -1000)
        {
            transform.position = new Vector3(values[0], transform.position.y, values[1]);
        }
    }
    void canvasReset()
    {
        mainCanvas.SetActive(true);
        canvas.SetActive(false);
        ready = false;
    }
    void deleteFunc()
    {
        deleteOn = true;
    }
    // Update is called once per frame
    void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform.position == transform.position && Input.GetMouseButtonDown(0))
        {
            idText.text = id.ToString();
            mainCanvas.SetActive(false);
            for (int i = 0; i < scripterObject.canvases.Length; i++)
            {
                scripterObject.canvases[i].SetActive(false);
            }
            canvas.SetActive(true);
        }
        properties();
    }
}
