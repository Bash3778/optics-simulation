using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointParts : MonoBehaviour
{
    public bool inOperation = true;
    public Vector3 position;
    public float[] positionArray;
    public int index;
    public Vector3 combine = Vector3.zero;
    public Vector3 backCombine = Vector3.zero;
    public Vector3 forwardCombine = Vector3.zero;
    public List<int> tempStuff = new List<int>();
    public float tempDist;
    public float extremeOffset = 3;
    // Start is called before the first frame update
    void Start()
    {
        tempStuff = new List<int>();
        tempDist = 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    private List<Vector3> m_points = new List<Vector3>();
 
    public PointParts (Vector3[] points) {
        m_points = new List<Vector3>(points);
    }
 
    public int[] Triangulate() {
        List<int> indices = new List<int>();
 
        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();
 
        int[] V = new int[n];
        if (Area() > 0) {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }
 
        int nv = n;
        int count = 2 * nv;
        for (int v = nv - 1; nv > 2; ) {
            if ((count--) <= 0)
                return indices.ToArray();
 
            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;
 
            if (Snip(u, v, w, nv, V)) {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                indices.Add(c);
                indices.Add(b);
                indices.Add(a);
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }
 
        indices.Reverse();
        return indices.ToArray();
    }
 
    private float Area () {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++) {
            Vector3 pval = m_points[p];
            Vector3 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }
 
    private bool Snip (int u, int v, int w, int n, int[] V) {
        int p;
        Vector3 A = m_points[V[u]];
        Vector3 B = m_points[V[v]];
        Vector3 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++) {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector3 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }
 
    private bool InsideTriangle (Vector3 A, Vector3 B, Vector3 C, Vector3 P) {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;
 
        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;
 
        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;
 
        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
    public List<int> narrowDown (List<int> possible, List<PointParts> parts) {
        if (possible.Count > 0) {
            int firstIndex = 0;
            int secondIndex = 0;
            float firstDist = 1000;
            float secondDist = 1000;
            for (int i = 0; i < possible.Count; i++) {
                if (Vector3.Distance(parts[possible[i]].position, position) < secondDist && possible[i] != index) {
                    if (Vector3.Distance(parts[possible[i]].position, position) < firstDist) {
                        firstIndex = possible[i];
                        firstDist = Vector3.Distance(parts[possible[i]].position, position);
                    } else {
                        secondIndex = possible[i];
                        secondDist = Vector3.Distance(parts[possible[i]].position, position);
                    }
                }
            }
            List<int> returning = new List<int>();
            returning.Add(firstIndex);
            if (secondDist < firstDist + extremeOffset) {
                returning.Add(secondIndex);
            }
            Debug.Log(returning.Count);
            return returning;
        } else {
            Debug.Log(0);
            return new List<int>();
        }
    }
    public float smallDistance (List<int> possible, List<PointParts> parts) {
        float dist = 1000;
        if (possible.Count > 0) {
            for (int i = 0; i < possible.Count; i++) {
                if (Vector3.Distance(parts[possible[i]].position, position) < dist) {
                    dist = Vector3.Distance(parts[possible[i]].position, position);
                }
            }
        }
        return dist;
    }
    public List<int> oneTriAdd(List<int> parts, List<int> start) {
        List<int> tempOne = start;
        for (int i = 0; i < parts.Count; i++) {
            tempOne.Add(parts[i]);
        }
        for (int i = parts.Count - 1; i >= 0; i--) {
            tempOne.Add(parts[i]);
        }
        return tempOne;
    }
    public bool sameDirection (List<PointParts> points, int index1, int index2) {
        bool direction = true;
        for (int i = 0; i < points[index1].positionArray.Length; i++) {
            if (points[index1].positionArray[i] > 0 && points[index2].positionArray[i] < 0) {
                direction = false;
            }
            if (points[index1].positionArray[i] < 0 && points[index2].positionArray[i] > 0) {
                direction = false;
            }
        }
        return direction;
    }
    public List<int> triAdd (List<PointParts> parts, List<int> triangles) {
        List<int> temp = triangles;
        if (inOperation) {
            List<PointParts> possibles = new List<PointParts>();
            //possibles.Add(new PointParts());
            //possibles.Add(new PointParts());
            //possibles.Add(new PointParts());
            for (int i = 0; i < parts.Count; i++) {
                if (i != index) {
                    if (parts[i].position.x == position.x) {
                        possibles[0].tempStuff.Add(i);
                    } 
                    if (parts[i].position.y == position.y) {
                        possibles[1].tempStuff.Add(i);
                    }
                    if (parts[i].position.z == position.z) {
                        possibles[2].tempStuff.Add(i);
                    }
                }
            }
            int firstIndex = 0;
            int secondIndex = 0;
            float firstDist = 900;
            float secondDist = 900;
            for (int i = 0 ; i < possibles.Count; i++) {
                possibles[i].tempStuff = narrowDown(possibles[i].tempStuff, parts);
                possibles[i].tempDist = smallDistance(possibles[i].tempStuff, parts);
            }
            for (int i = 0; i < possibles.Count; i++) {
                if (possibles[i].tempDist < secondDist) {
                    if (possibles[i].tempDist < firstDist) {
                        firstIndex = i;
                        firstDist = possibles[i].tempDist;
                    } else {
                        secondIndex = i;
                        secondDist = possibles[i].tempDist;
                    }
                }
            }
            List<int> adjacent = new List<int>();
            for (int i = 0; i < possibles[firstIndex].tempStuff.Count; i++) {
                //Vector3 average = (position + parts[possibles[firstIndex].tempStuff[i]].position) / 2;
                int bestIndex = 0;
                float bestDist = 1000;
                for (int j = 0; j < parts.Count; j++) {
                    if (parts[j].positionArray[firstIndex] != positionArray[firstIndex] && Vector3.Distance(position, parts[j].position) < bestDist) {
                        bool inThere = false;
                        if (i > 0) {
                            if (j != adjacent[adjacent.Count -1] !sameDirection(parts, j, possibles[firstIndex].tempStuff[i])) {
                                inThere = true;
                            }
                        } else {
                            inThere = true;
                        }
                        if (inThere) {
                            bestDist = Vector3.Distance(position, parts[j].position);
                            bestIndex = j;
                        }
                    }
                }
                adjacent.Add(bestIndex);
            }
            int tempIndex = -1;
            for (int i = 0; i < possibles.Count; i++) {
                if (firstIndex != i || secondIndex != i) {
                    tempIndex = i;
                }
            }
            if (tempIndex != -1) {
                secondIndex = tempIndex;
            }
            for (int i = 0; i < 1ossibles[firstIndex].tempStuff.Count; i++) { //possibles[firstIndex].tempStuff.Count
                for (int j = 0; j < 1; j++) { // possibles[secondIndex].tempStuff.Count
                    temp = oneTriAdd(new List<int> {index, possibles[firstIndex].tempStuff[i], adjacent[j]}, temp);
                    //parts[possibles[firstIndex].tempStuff[i]].inOperation = false;
                    //parts[adjacent[j]].inOperation = false;
                }
            }
            //int[] currentClose = new int[4];
            List<float> currentCloseDist = new List<float>();
            List<int> currentClose = new List<int>();
            for (int i = 0; i < 4; i++) {
                currentCloseDist.Add(1000);
                currentClose.Add(0);
            }
            for (int i = 0; i < parts.Count; i++) {
                    if (i != index) {
                    int addIndex = 4;
                    float addDist = 1000;
                    for (int j = currentClose.Count - 1; j >= 0; j--) {
                        if (Vector3.Distance(position, parts[i].position) < currentCloseDist[j]) {
                            addIndex = j;
                            addDist = Vector3.Distance(position, parts[i].position);
                        }
                    }
                    currentClose.Insert(addIndex, i);
                    currentCloseDist.Insert(addIndex, addDist);
                    currentClose.RemoveAt(4);
                    currentCloseDist.RemoveAt(4);
                }
            }
            List<int> first = new List<int>();
            first.Add(currentClose[0]);
            first.Add(currentClose[2]);
            List<int> second = new List<int>();
            second.Add(currentClose[1]);
            second.Add(currentClose[3]);
            for (int i = 0; i < first.Count; i++) {
                for (int j = 0; j < second.Count; j++) {
                    temp = oneTriAdd(new List<int> {index, first[i], second[j]}, temp);
                    //parts[first[i]].inOperation = false;
                    //parts[second[j]].inOperation = false;
                }
            }
        } 
        return temp;
    } */
                    /*
                for (int i = 0; i < verts.Count; i++) {
                    float smallDist = 1000;
                    int smallIndex = 0;
                    for (int j = 0; j < verts.Count; j++) {
                        if (Vector3.Distance(verts[i], verts[j]) < smallDist && i != j) {
                            smallDist = Vector3.Distance(verts[i], verts[j]);
                            smallIndex = j;
                        }
                    }
                    Vector3 average = (verts[i] + verts[smallIndex]) / 2;
                    int firstIndex = 0;
                    int secondIndex = 0;
                    float firstDist = 1000;
                    float secondDist = 1000;
                    for (int j = 0; j < verts.Count; j++) {
                        if (Vector3.Distance(average, verts[j]) <= secondDist && j != i && j != smallIndex) {
                            if (Vector3.Distance(average, verts[j]) <= firstDist) {
                                secondIndex = firstIndex;
                                secondDist = firstDist;
                                firstDist = Vector3.Distance(average, verts[j]);
                                firstIndex = j;
                            } else {
                                secondIndex = j;
                                secondDist = Vector3.Distance(average, verts[j]);
                            }
                        }
                    }
                    Debug.Log(i + " " + smallIndex + " " + firstIndex);
                    triangle.Add(i);
                    triangle.Add(smallIndex);
                    triangle.Add(firstIndex);
                    triangle.Add(firstIndex);
                    triangle.Add(smallIndex);
                    triangle.Add(i);
                    triangle.Add(i);
                    triangle.Add(smallIndex);
                    triangle.Add(secondIndex);
                    triangle.Add(secondIndex);
                    triangle.Add(smallIndex);
                    triangle.Add(i);
                }*/
}
