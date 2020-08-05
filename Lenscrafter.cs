using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lenscrafter : MonoBehaviour
{
    [SerializeField] GameObject buildObject;
    [SerializeField] GameObject scripterObject;
    [SerializeField] Button newItem;
    [SerializeField] Button newLayer;
    [SerializeField] Button newPlane;
    [SerializeField] Button calculate;
    [SerializeField] GameObject activeButton;
    [SerializeField] GameObject testObject;
    [SerializeField] GameObject outerBound;
    [SerializeField] Transform generalCanvas;
    [SerializeField] Text itemTag;
    [SerializeField] Text layerTag;
    [SerializeField] Text indexTag;
    [SerializeField] Text planeTag;
    [SerializeField] InputField[] inputs;
    [SerializeField] InputField[] limits;
    [SerializeField] Button[] saveAs;
    [SerializeField] InputField equation;
    [SerializeField] InputField refraction;
    [SerializeField] float countingOffset = 20;
    [SerializeField] int maxDist;
    [SerializeField] float interval;
    [SerializeField] float angleInterval = 20;
    [SerializeField] float angleInterval2 = 60;
    [SerializeField] float testRadius = 14;
    [SerializeField] float degreeOpening= 120;
    [SerializeField] float totalTestVelocity = 5;
    [SerializeField] float maxMin;
    [SerializeField] float minmax;
    public List<GameObject> items = new List<GameObject>();
    public GameObject currentItem;
    public ItemManager currentScript;
    InputField currentChange;
    int currentInt;
    bool newer  = false;
    void Start()
    {
        newItem.onClick.AddListener(delegate { newItemFunc(0); });
        newLayer.onClick.AddListener(delegate { newItemFunc(1); });
        newPlane.onClick.AddListener(delegate { newItemFunc(2); });
        calculate.onClick.AddListener(equationDisect);
        for (int i = 0; i < saveAs.Length; i++) {
            saveAs[i].onClick.AddListener(saveAsFunction);
        }
    }
    bool difference (string sign) {
        if (sign == "=" || sign == "<" || sign == ">") {
            return true;
        } else {
            return false;
        }
    }
    int opLength (char[] parts, int start) {
        if (start + 1 < parts.Length) {
            if (difference(parts[start + 1].ToString())) {
                return 2;
            } else {
                return 1;
            }
        } else {
            return 1;
        }
    }
    bool direct (string sign) {
        if (sign == "<") {
            return false;
        } else {
            return true;
        }
    }
    PointParts pointer (int index, Vector3 coordinates) {
        PointParts tempPoint = new PointParts();
        tempPoint.index = index;
        tempPoint.position = coordinates;
        tempPoint.positionArray = new float[] {coordinates.x, coordinates.y, coordinates.z};
        return tempPoint;
    }
    List<int> triangleAdd (List<int> existing, List<int> previous, List<int> current, List<int> future, List<PointParts> partPoint, List<Vector3> verts) {
        List<int> triangle = existing;
        if (current.Count > 0) {
            List<int> temp = new List<int>();
            for(int j = 0; j < current.Count; j++) {
                temp.Add(current[j]);
            }
            int currentIndex = 0;
            int fun = 0;
            while (temp.Count > 0 && fun < current.Count + 1) {
                int closeIndex = 0;
                int removeIndex = 0;
                float smallerDist = 1000;
                for (int k = 0; k < temp.Count; k++) {
                    if (temp[k] != current[currentIndex]) {
                        if (Vector3.Distance(verts[current[currentIndex]], verts[temp[k]]) < smallerDist) {
                            closeIndex = k;
                            smallerDist = Vector3.Distance(verts[current[currentIndex]], verts[temp[k]]);
                        } 
                    } else {
                        removeIndex = k;
                    }
                }
                if (smallerDist < 900) {
                    if (previous.Count > 0) {
                        triangle.Add(current[currentIndex]);
                        triangle.Add(temp[closeIndex]);
                        float smallDist = 1000;
                        int smallIndex = 0;
                        for (int s = 0; s < previous.Count; s++) {
                            Vector3 average = (verts[current[currentIndex]] + verts[temp[closeIndex]]) / 2;
                            if (Vector3.Distance(average, verts[previous[s]]) < smallDist && partPoint[previous[s]].backCombine != average) {
                                smallDist = Vector3.Distance(average, verts[previous[s]]);
                                smallIndex = s;
                                partPoint[previous[s]].backCombine = average;
                            }
                        }
                        triangle.Add(previous[smallIndex]);
                        triangle.Add(previous[smallIndex]);
                        triangle.Add(temp [closeIndex]);
                        triangle.Add(current[currentIndex]);
                    }
                    if (future.Count > 0) {
                        triangle.Add(current[currentIndex]);
                        triangle.Add(temp[closeIndex]);
                        float smallDist2 = 1000;
                        int smallIndex2 = 0;
                        for (int s = future.Count - 1; s >= 0; s--) {
                            Vector3 average = (verts[current[currentIndex]] + verts[temp[closeIndex]]) / 2;
                            if (Vector3.Distance(average, verts[future[s]]) < smallDist2 && partPoint[future[s]].forwardCombine != average) {
                                smallDist2 = Vector3.Distance(average, verts[future[s]]);
                                smallIndex2 = s;
                                partPoint[future[s]].forwardCombine = average;
                            }
                        }
                        triangle.Add(future[smallIndex2]);
                        triangle.Add(future[smallIndex2]);
                        triangle.Add(temp [closeIndex]);
                        triangle.Add(current[currentIndex]);
                    }
                }
                int midde = temp[closeIndex];
                temp.RemoveAt(removeIndex);
                if (fun == 1) {
                    temp.Add(current[0]);
                }
                for (int j = 0; j < current.Count; j++) {
                    if (current[j] == midde) {
                        currentIndex = j;
                    }
                }
                fun++;
            }
        }
        return triangle;
    }
    Vector3 averageList (List<int> range, List<Vector3> verts) {
        Vector3 sum = new Vector3(0, 0, 0);
        for (int i = 0; i < range.Count; i++) {
            sum += verts[range[i]];
        }
        sum /= range.Count;
        return sum;
    }
    void equationDisect () {
        float [] lowers = new float[3];
        float [] higher = new float[3];
        string[] vars = {"x", "y", "z"};
        if (currentItem != null) {
            if (currentScript.current.GetComponent<ItemManager>().current != null && currentScript.current.GetComponent<ItemManager>().current.GetComponent<ItemManager>().on) {
                ItemManager tempScript = currentScript.current.GetComponent<ItemManager>().current.GetComponent<ItemManager>();
                if (tempScript.lowers.Count > 0) {
                    for (int i = 0; i < tempScript.lowers.Count; i++) {
                        Destroy(tempScript.lowers[i]);
                    }
                }
                int middle = 0;
                OrderDown operation = new OrderDown();
                for (int i = 0; i < limits.Length; i++) {
                    if (limits[i].text != "") {
                        char[] parts = limits[i].text.ToCharArray();
                        for (int k = 0; k < parts.Length; k++) {
                            if (parts[k].ToString() == vars[i]) {
                                middle = k;
                            }
                        }
                        float tempLow = -1 * maxDist;
                        float tempHigh = maxDist;
                        bool direction = false;
                        bool equal = false;
                        bool equal2 = false;
                        bool equalSign = false;
                        bool present = false;
                        int pressed = 0;
                        int j = 0;
                        while (j < middle) {
                            if (parts[j].ToString() == "-" || operation.isNumb(parts[j].ToString())){
                                int end = operation.ending(parts, j, true);
                                float.TryParse(operation.compressString(parts, j, end + 1), out tempLow);
                                j = end + 1;
                            } else if (difference(parts[j].ToString())) {
                                int length = opLength(parts, j);
                                if (length == 1) {
                                    if (parts[j].ToString() == "=") {
                                        equalSign = true;
                                    } else {
                                        direction = direct(parts[j].ToString());
                                    }
                                } else {
                                    direction = direct(parts[j].ToString());
                                    equal = true;
                                }
                                pressed++;
                                present = true;
                                j = j + length;
                            } else {
                                j++;
                            }
                        }
                        j = middle + 1;
                        while (j < parts.Length) {
                            if (parts[j].ToString() == "-" || operation.isNumb(parts[j].ToString())){
                                int end = operation.ending(parts, j, true);
                                float.TryParse(operation.compressString(parts, j, end + 1), out tempHigh);
                                j = end + 1;
                            } else if (difference(parts[j].ToString())) {
                                int length = opLength(parts, j);
                                if (length == 1) {
                                    if (parts[j].ToString() == "=") {
                                        equalSign = true;
                                    } else {
                                        direction = direct(parts[j].ToString());
                                    }
                                } else {
                                    equal2 = true;
                                    direction = direct(parts[j].ToString());
                                }
                                pressed++;
                                j = j + length;
                            } else {
                                j++;
                            }
                        }
                        if (equalSign) {
                            if (present) {
                                higher[i] = tempLow + interval;
                                lowers[i] = tempLow;
                            } else {
                                higher[i] = tempHigh + interval;
                                lowers[i] = tempHigh;
                            }
                        } else {
                            if (pressed == 1) {
                                if (present) {
                                    if (direction) {
                                        tempHigh = -1 * maxDist;
                                    }
                                } else {
                                    if (direction) {
                                        tempLow = maxDist;
                                    }
                                }
                            }
                            if (!direction) {
                                lowers[i] = tempLow;
                                higher[i] = tempHigh;
                                if (!equal) {
                                    lowers[i] += interval;
                                }
                                if (equal2) {
                                    higher[i] += interval;
                                }
                            } else {
                                higher[i] = tempLow;
                                lowers[i] = tempHigh;
                                if (equal) {
                                    higher[i] += interval;
                                }
                                if (!equal2) {
                                    lowers[i] += interval;
                                }
                            }
                        } 
                    } else {
                        lowers[i] = -1 * maxDist;
                        higher[i] = maxDist;
                    }
                }
                tempScript.lowers.Clear();
                char[] equals = equation.text.ToCharArray();
                OrderDown exp1 = new OrderDown();
                OrderDown exp2 = new OrderDown();
                for (int i = 0; i < equals.Length; i++) {
                if (equals[i].ToString() == "=") {
                    string first = "";
                    for (int j = 0; j < i; j++) {
                        first = first + equals[j];
                    }
                    string second = "";
                    for (int j = i + 1; j < equals.Length; j++){
                        second = second + equals[j];
                    }
                    exp1.entered = first;
                    exp2.entered = second;
                    }
                } 
                exp1.sort();
                exp2.sort();
                float[] totalPower = new float[3];
                for (int i = 0; i < exp1.varPowers.Length; i++) {
                    if (exp1.varPowers[i] > exp2.varPowers[i]){
                        totalPower[i] = exp1.varPowers[i];
                    } else {
                        totalPower[i] = exp2.varPowers[i];
                    }
                }
                int[] orderPower = new int[3]; 
                orderPower[0] = 0;
                orderPower[1] = 1;
                orderPower[2] = 2;
                for (int i = 0 ; i < orderPower.Length; i++) {
                    int maxInt = i;
                    float maxDist = Mathf.Abs(totalPower[orderPower[i]]);
                    for (int j = i + 1; j < orderPower.Length; j++) {
                        if (Mathf.Abs(totalPower[orderPower[j]]) > maxDist) {
                            maxInt = j;
                            maxDist = Mathf.Abs(totalPower[orderPower[j]]);
                        }
                    }
                    int temp = orderPower[i];
                    orderPower[i] = orderPower[maxInt];
                    orderPower[maxInt] = temp;
                }
                Mesh mesh = new Mesh();
                List<Vector3> verts = new List<Vector3>();
                List<int> triangle = new List<int>();
                List<PointParts> partPoint = new List<PointParts>();
                bool inPlace = false;
                int counter = 0;
                List<int> previous = new List<int>(); 
                List<int> current = new List<int>();
                List<int> future = new List<int>();
                for (float i = lowers[orderPower[0]]; i < higher[orderPower[0]]; i+=interval) {
                    List<int> queue= new List<int>();
                    for (float j = lowers[orderPower[1]]; j < higher[orderPower[1]]; j+=interval) {
                        for (float k = lowers[orderPower[2]]; k < higher[orderPower[2]]; k+=interval) {
                            float[] coordinates = {i, j, k};
                            float[] calculation = {coordinates[orderPower[0]], coordinates[orderPower[1]], coordinates[orderPower[2]]};
                            exp1.calcValue(calculation);
                            exp2.calcValue(calculation);
                            if (exp1.value == exp2.value) {
                                /*
                                GameObject obj1 = Instantiate(buildObject);
                                obj1.transform.position = new Vector3(i, j, k);
                                tempScript.lowers.Add(obj1);
                                obj1.SetActive(true);
                                */ 
                                verts.Add(new Vector3(calculation[0], calculation[1], calculation[2])); 
                                if (!inPlace) {
                                    GameObject obj = Instantiate(buildObject);
                                    obj.tag = "lens";
                                    obj.transform.position = Vector3.zero;
                                    tempScript.lowers.Add(obj);
                                    inPlace = true;
                                }
                                partPoint.Add(pointer(counter, new Vector3(calculation[0], calculation[1], calculation[2])));
                                queue.Add(counter);
                                counter++;
                            }
                        }
                    }
                    triangle = triangleAdd(triangle, previous, current, future, partPoint, verts);
                    if (queue.Count != 0) {
                        if (current.Count == 0 && previous.Count == 0 && queue.Count > 1) {
                            previous = current;
                            verts.Add(averageList(queue, verts));
                            partPoint.Add(pointer(counter, averageList(queue, verts)));
                            counter++;
                            List<int> existing = new List<int>();
                            existing.Add(counter - 1);
                            current = existing;
                            future = queue;
                        } else {
                            previous = current;
                            current = future;
                            future = queue;
                        }
                    }
                }
                int weirdCount = 0;
                while (current.Count > 0 && weirdCount <= 3) {
                    triangle = triangleAdd(triangle, previous, current, future, partPoint, verts);
                    previous = current;
                    current = future;
                    if (future.Count > 1) {
                        verts.Add(averageList(future, verts));
                        partPoint.Add(pointer(counter, averageList(future, verts)));
                        counter++;
                        List<int> existing = new List<int>();
                        existing.Add(counter - 1);
                        future = existing;
                    } else {
                        future.Clear();
                    }
                    weirdCount++;
                }
                Vector3[] ver = new Vector3[verts.Count];
                int[] tri = new int[triangle.Count];
                for (int i = 0; i < verts.Count; i++) {
                    ver[i] = verts[i];
                } 
                for (int i = 0; i < triangle.Count; i++) {
                    tri[i] = triangle[i];
                }
                if (tempScript.lowers.Count > 0) {
                    mesh.vertices = ver; 
                    tempScript.lowers[0].GetComponent<MeshFilter>().mesh = mesh;
                    mesh.triangles = tri;
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mesh.RecalculateTangents();
                    tempScript.lowers[0].AddComponent<MeshCollider>();
                    tempScript.lowers[0].AddComponent<Rigidbody>();
                    tempScript.lowers[0].GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
    }
    void Update()
    {
        if (currentItem != null)
        {
            int countIndex = -1;
            currentScript = currentItem.GetComponent<ItemManager>();
            for (int i = 0; i < items.Count; i++) {
                if (items[i].GetComponent<ItemManager>().pressed) {
                    countIndex = i;
                    currentChange = inputs[items[i].GetComponent<ItemManager>().type];
                    currentInt = i;
                    items[i].GetComponent<ItemManager>().pressed = false;
                    currentScript = items[i].GetComponent<ItemManager>();
                    currentItem = items[i];
                }
            }
            if (countIndex != -1) {
                for (int i = 0; i < items.Count; i++) {
                    if (i != countIndex) {
                        items[i].GetComponent<ItemManager>().on = false;
                    }
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (currentChange != null) {
            currentChange.text = items[currentInt].GetComponent<ItemManager>().name;
            currentInt = 0;
            currentChange = null;
        }
    }
    GameObject updateHigh(int count, Transform pos) {
        GameObject obj = Instantiate(activeButton);
        obj.transform.SetParent(generalCanvas);
        obj.transform.position = new Vector3(pos.position.x, -1 * (count + 0.1f) * countingOffset + pos.position.y);
        obj.SetActive(true);
        return obj;
    }
    void startScript(ItemManager script, int type) {
        script.type = type;
        script.pressed = true;
        script.on = true;
        script.generalCanvas = generalCanvas;
        script.fields = inputs;
        InputField[] temp = {equation, limits[0], limits[1], limits[2], refraction};
        script.sections = temp;
    }
    void newItemFunc(int type) {
        if (type == 0)
        {
            items.Add(updateHigh(items.Count + 1, itemTag.transform));
            currentItem = items[items.Count - 1];
            startScript(currentItem.GetComponent<ItemManager>(), type);
            currentItem.GetComponent<ItemManager>().on = true;
            currentItem.GetComponent<ItemManager>().craft = this;
        }
        else
        {
            if (currentItem != null && currentItem.GetComponent<ItemManager>().on)
            {
                if (type == 1)
                {
                    currentScript.lowers.Add(updateHigh(currentScript.lowers.Count + 1, layerTag.transform));
                    currentScript.current = currentScript.lowers[currentScript.lowers.Count - 1];
                    currentScript.lowers[currentScript.lowers.Count - 1].GetComponent<ItemManager>().higher = currentScript;
                    startScript(currentScript.current.GetComponent<ItemManager>(), type);
                }
                else {
                    if (currentScript.current != null && currentScript.current.GetComponent<ItemManager>().on)
                    {
                        currentScript.current.GetComponent<ItemManager>().lowers.Add(updateHigh(currentScript.current.GetComponent<ItemManager>().lowers.Count + 1, planeTag.transform));
                        currentScript.current.GetComponent<ItemManager>().current = currentScript.current.GetComponent<ItemManager>().lowers[currentScript.current.GetComponent<ItemManager>().lowers.Count - 1];
                        currentScript.current.GetComponent<ItemManager>().lowers[currentScript.current.GetComponent<ItemManager>().lowers.Count - 1].GetComponent<ItemManager>().higher = currentScript.current.GetComponent<ItemManager>();
                        startScript(currentScript.current.GetComponent<ItemManager>().current.GetComponent<ItemManager>(), type);
                    }
                }
            }
        }
    }
    void saveAsFunction () {
        outerBound.SetActive(true);
        //circular coordinate y
        for (float i = 0; i < 360; i+= angleInterval) {
            //circular coordinate xz
            for (float j = 0; j < 360; j+= angleInterval) {
                //rotational coordinate y
                for (float k = -1 * degreeOpening / 2; k <= degreeOpening / 2; k += angleInterval2) {
                    //rotational coordinate xz
                    for (float l = -1 * degreeOpening / 2; l <=  degreeOpening / 2; l+= angleInterval2) {
                        float angleX = (j % 90) * Mathf.Deg2Rad;
                        float xpos = 100;
                        float zpos = 100;
                        float xzdist = Mathf.Cos(i * Mathf.Deg2Rad) * testRadius * -1;
                        float ypos = Mathf.Sin(i * Mathf.Deg2Rad) * testRadius;
                        if (j >= 0 && j < 90)
                        {
                            xpos = Mathf.Sin(angleX) * xzdist * -1;
                            zpos = Mathf.Cos(angleX) * xzdist * -1;
                        }
                        else if (j >= 90 && j < 180)
                        {
                            xpos = Mathf.Cos(angleX) * xzdist * -1;
                            zpos = Mathf.Sin(angleX) * xzdist;
                        }
                        else if (j >= 180 && j < 270)
                        {
                            xpos = Mathf.Sin(angleX) * xzdist;
                            zpos = Mathf.Cos(angleX) * xzdist;
                        }
                        else
                        {
                            xpos = Mathf.Cos(angleX) * xzdist;
                            zpos = Mathf.Sin(angleX) * xzdist * -1;
                        }
                        GameObject obj = Instantiate(testObject);
                        obj.transform.position = new Vector3(xpos, ypos, zpos);
                        Light scripter = obj.GetComponent<Light>();
                        float anglerY = (i + 180 + k) * Mathf.Deg2Rad;
                        scripter.velocityY = Mathf.Sin(anglerY) * totalTestVelocity;
                        scripter.velocityX = 0;
                        scripter.velocityZ = 0;
                        scripter.testing = true;
                        float inputAngle = (j + 180 + l) * Mathf.Deg2Rad;
                        float velocityXZ = Mathf.Cos(anglerY) * totalTestVelocity;
                        if (inputAngle >= 0 && inputAngle <= 90)
                        {
                            scripter.velocityX = Mathf.Sin(inputAngle) * velocityXZ * -1;
                            scripter.velocityZ = Mathf.Cos(inputAngle) * velocityXZ * -1;
                        }
                        else if (inputAngle > 90 && inputAngle <= 180)
                        {
                            scripter.velocityX = Mathf.Cos(inputAngle) * velocityXZ;
                            scripter.velocityZ = Mathf.Sin(inputAngle) * velocityXZ * -1;
                        }
                        else if (inputAngle > 180 && inputAngle <= 270)
                        {
                            scripter.velocityX = Mathf.Sin(inputAngle) * velocityXZ;
                            scripter.velocityZ = Mathf.Cos(inputAngle) * velocityXZ;
                        }
                        else if (inputAngle > 270 && inputAngle <= 360)
                        {
                            scripter.velocityX = Mathf.Cos(inputAngle) * velocityXZ * -1;
                            scripter.velocityZ = Mathf.Sin(inputAngle) * velocityXZ;
                        }
                        refractingSurface surface = new refractingSurface();
                        scripter.testSurface = surface;
                        scripter.iing = i;
                        scripter.jing = j;
                        scripter.king = k;
                        scripter.ling = l;
                    }
                }
            } 
        }
        outerBound.SetActive(false);
    }
}