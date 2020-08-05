using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainController : MonoBehaviour
{
    [SerializeField] GameObject[] pieces;
    [SerializeField] Button[] piecePlacer;
    [SerializeField] float[] yPieceDisplacemnent;
    public GameObject[] canvases;
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject onPhysical;
    [SerializeField] Camera camera;
    [SerializeField] GameObject baseLayer;
    [SerializeField] GameObject axisLayer;
    public Transform centralBase;
    [SerializeField] int sideBaseLength;
    [SerializeField] float addLength;
    [SerializeField] float specialAdd;
    [SerializeField] float baseW;
    [SerializeField] float baseA;
    [SerializeField] float timerIntergral;
    [SerializeField] float waveSpeed;
    [SerializeField] GameObject wheelButton;
    [SerializeField] float cameraMult;
    [SerializeField] float cameraMax = 85;
    [SerializeField] float cameraMin = 5;
    [SerializeField] GameObject pivitCenter;
    [SerializeField] Button up;
    [SerializeField] Button down;
    [SerializeField] Button right;
    [SerializeField] Button left;
    [SerializeField] Button zoomIn;
    [SerializeField] Button zoomOut;
    [SerializeField] Button reset;
    [SerializeField] Button pauseButton;
    [SerializeField] float moveFactorCamera;
    [SerializeField] float minYMovement;
    [SerializeField] float maxYMovement;
    [SerializeField] float outerMost;
    [SerializeField] float zoomFactor;
    [SerializeField] float maxZoom;
    [SerializeField] float minZoom;
    [SerializeField] Button switchModeBut;
    [SerializeField] Button constructBut;
    [SerializeField] Text switchModeText; 
    [SerializeField] GameObject physicalObjects;
    [SerializeField] GameObject geometricObjects;
    HUDController hud;
    public bool placer = false;
    public bool emit = true;
    public int mode = 0;
    int smallTimer = 0;
    int placeIndex = 0;
    int currentIndex = 0;
    float dist;
    float savedDist;
    Vector3 savedStart;
    Quaternion savedRotation;
    GameObject[] optics = new GameObject[100];
    // Start is called before the first frame update
    void Start()
    {
        dist = Vector3.Distance(pivitCenter.transform.position, camera.transform.position);
        savedDist = dist;
        savedStart = camera.transform.position;
        savedRotation = camera.transform.rotation;
        hud = wheelButton.GetComponent<HUDController>();
        piecePlacer[0].onClick.AddListener(delegate { placement(0); });
        piecePlacer[1].onClick.AddListener(delegate { placement(1); });
        piecePlacer[2].onClick.AddListener(delegate { placement(2); });
        piecePlacer[3].onClick.AddListener(delegate { placement(3); });
        piecePlacer[4].onClick.AddListener(delegate { placement(4); });
        piecePlacer[5].onClick.AddListener(delegate { placement(5); });
        piecePlacer[6].onClick.AddListener(delegate { placement(6); });
        piecePlacer[7].onClick.AddListener(delegate { placement(7); });
        piecePlacer[8].onClick.AddListener(delegate { placement(8); });
        pauseButton.onClick.AddListener(pauseFunc);
        up.onClick.AddListener(upFunc);
        down.onClick.AddListener(downFunc);
        right.onClick.AddListener(rightFunc);
        left.onClick.AddListener(leftFunc);
        reset.onClick.AddListener(resetFunc);
        zoomIn.onClick.AddListener(zoomInFunc);
        zoomOut.onClick.AddListener(zoomOutFunc);
        switchModeBut.onClick.AddListener(switchMode);
        constructBut.onClick.AddListener(constructing);
        for (float width = -1 * sideBaseLength; width <= sideBaseLength; width+= addLength)
        {
            for (float length = -1 * sideBaseLength; length <= sideBaseLength; length+= addLength) {
                GameObject obj = Instantiate(baseLayer);
                obj.transform.position = centralBase.position + new Vector3(centralBase.position.x + width, centralBase.position.y, centralBase.position.z + length);
                obj.transform.SetParent(physicalObjects.transform);
                obj.SetActive(true);
            }
        }
        for (float width = -1 * sideBaseLength; width <= sideBaseLength; width += specialAdd)
        {
            if (width != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    float[] offsets = { 0f, 0f, 0f };
                    offsets[i] = width;
                    GameObject obj = Instantiate(axisLayer);
                    obj.transform.position = centralBase.position + new Vector3(centralBase.position.x + offsets[0], centralBase.position.y + offsets[1], centralBase.position.z + offsets[2]);
                    obj.transform.SetParent(geometricObjects.transform);
                    obj.SetActive(true);
                }
            }
        }
    }
    void placement(int index)
    {
        placer = true;
        placeIndex = index;
        smallTimer = 0;
    }
    void place()
    {
        if (placer)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonUp(0) && smallTimer >= 1)
            {
                GameObject obj = Instantiate(pieces[placeIndex]);
                PieceController idsetting = obj.GetComponent<PieceController>();
                idsetting.id = currentIndex;
                idsetting.baseWavelength = baseW;
                idsetting.baseAmplitude = baseA;
                idsetting.timerInt = timerIntergral;
                idsetting.waveSpeed = waveSpeed;
                optics[currentIndex] = obj;
                currentIndex++;
                obj.transform.position = hit.transform.position + new Vector3(0, yPieceDisplacemnent[placeIndex], 0);
                obj.SetActive(true);
                placer = false;
                placeIndex = 0;
            }
            smallTimer++;
        }
    }
    void cameraControl() {
        float deltaX = hud.direction.x - hud.revised.x;
        float deltaY = hud.direction.y - hud.revised.y;
        float angle = deltaX * cameraMult;
        if (deltaX != 0)
        {
            camera.transform.Rotate(new Vector3(0f, angle, 0f));
        }
        if (deltaY != 0 && camera.transform.rotation.eulerAngles.x + (deltaY * cameraMult) < cameraMax && camera.transform.rotation.eulerAngles.x + (deltaY * cameraMult) > cameraMin) {
            camera.transform.Rotate(new Vector3(deltaY * cameraMult, 0f, 0f));
        }
        float xpos;
        float zpos;
        float angleCam = (camera.transform.rotation.eulerAngles.y % 90) * Mathf.Deg2Rad;
        float camY = (camera.transform.rotation.eulerAngles.x % 90) * Mathf.Deg2Rad;
        float xzdist = Mathf.Cos(camY) * dist;
        float ypos = Mathf.Sin(camY) * dist;
        if (camera.transform.rotation.eulerAngles.y >= 0 && camera.transform.rotation.eulerAngles.y <= 90)
        {
            xpos = Mathf.Sin(angleCam) * xzdist * -1;
            zpos = Mathf.Cos(angleCam) * xzdist * -1;
        }
        else if (camera.transform.rotation.eulerAngles.y > 90 && camera.transform.rotation.eulerAngles.y <= 180)
        {
            xpos = Mathf.Cos(angleCam) * xzdist * -1;
            zpos = Mathf.Sin(angleCam) * xzdist;
        }
        else if (camera.transform.rotation.eulerAngles.y > 180 && camera.transform.rotation.eulerAngles.y <= 270)
        {
            xpos = Mathf.Sin(angleCam) * xzdist;
            zpos = Mathf.Cos(angleCam) * xzdist;
        }
        else
        {
            xpos = Mathf.Cos(angleCam) * xzdist;
            zpos = Mathf.Sin(angleCam) * xzdist * -1;
        }
        camera.transform.position = new Vector3(xpos + pivitCenter.transform.position.x, ypos + pivitCenter.transform.position.y, zpos + pivitCenter.transform.position.z);
        camera.transform.rotation = Quaternion.Euler(camera.transform.rotation.eulerAngles.x, camera.transform.rotation.eulerAngles.y, 0f);
    }
    void pauseFunc() {
        if (emit)
        {
            emit = false;
        }
        else {
            emit = true;
        }
    }
    void upFunc() {
        if (pivitCenter.transform.position.y + moveFactorCamera < maxYMovement)
        {
            pivitCenter.transform.position = new Vector3(pivitCenter.transform.position.x, pivitCenter.transform.position.y + moveFactorCamera, pivitCenter.transform.position.z);
        }
    }
    void downFunc() {
        if (pivitCenter.transform.position.y - moveFactorCamera > minYMovement)
        {
            pivitCenter.transform.position = new Vector3(pivitCenter.transform.position.x, pivitCenter.transform.position.y - moveFactorCamera, pivitCenter.transform.position.z);
        }
    }
    void rightFunc() {
        float xpos = Mathf.Sin((camera.transform.rotation.eulerAngles.y + 90) * Mathf.Deg2Rad) * moveFactorCamera;
        float zpos = Mathf.Cos((camera.transform.rotation.eulerAngles.y + 90) * Mathf.Deg2Rad) * moveFactorCamera;
        if (pivitCenter.transform.position.x + xpos < outerMost && pivitCenter.transform.position.x + xpos > -1 * outerMost && pivitCenter.transform.position.z + zpos < outerMost && pivitCenter.transform.position.z + zpos > -1 * outerMost)
        {
            pivitCenter.transform.position = new Vector3(pivitCenter.transform.position.x + xpos, pivitCenter.transform.position.y, pivitCenter.transform.position.z + zpos);
        }
    }
    void leftFunc()
    {
        float xpos = Mathf.Sin((camera.transform.rotation.eulerAngles.y + 90) * Mathf.Deg2Rad) * moveFactorCamera;
        float zpos = Mathf.Cos((camera.transform.rotation.eulerAngles.y + 90)* Mathf.Deg2Rad) * moveFactorCamera;
        if (pivitCenter.transform.position.x - xpos < outerMost && pivitCenter.transform.position.x - xpos > -1 * outerMost && pivitCenter.transform.position.z - zpos < outerMost && pivitCenter.transform.position.z - zpos > -1 * outerMost)
        {
            pivitCenter.transform.position = new Vector3(pivitCenter.transform.position.x - xpos, pivitCenter.transform.position.y, pivitCenter.transform.position.z - zpos);
        }
    }
    void zoomInFunc() {
        if (dist - zoomFactor > minZoom)
        {
            dist -= zoomFactor;
        }
    }
    void zoomOutFunc() {
        if (dist + zoomFactor < maxZoom)
        {
            dist += zoomFactor;
        }
    }
    void resetFunc() {
        camera.transform.position = savedStart;
        camera.transform.rotation = savedRotation;
        dist = savedDist;
        pivitCenter.transform.position = new Vector3(0, 0, 0);
    }
    void switchMode() {
        if (mode == 0)
        {
            mode = 1;
            switchModeText.text = "Current Mode: Geometric";
        }
        else {
            mode = 0;
            switchModeText.text = "Current Mode: Physical";
        }
    }
    void constructing() {
        for (int i = 0; i < canvases.Length; i++) {
            canvases[i].SetActive(false);
        } mainCanvas.SetActive(false);
        physicalObjects.SetActive(false);
        geometricObjects.SetActive(true);
        onPhysical.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        cameraControl();
        place();
    }
}