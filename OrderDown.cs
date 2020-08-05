using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OrderDown : MonoBehaviour
{
    public string entered = "";
    public List<string> level = new List<string>();
    public List<OrderDown> below = new List<OrderDown>();
    public float[] varPowers = new float[] {0, 0, 0};
    public float value = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    } 
    public bool variableYes (string enter) {
        if (enter == "x" || enter == "y" || enter == "z") {
            return true;
        } else {
            return false;
        }
    }
    public int variableType (string enter) {
        string[] var = new string[] {"x", "y", "z"};
        int returning = 0;
        for (int i = 0; i < var.Length; i++) {
            if (enter == var[i]) {
                returning = i;
            }
        }
        return returning;
    }
    public bool operater (string enter) {
        if (enter == "+" || enter == "*" || enter == "/" || enter == "^") {
            return true;
        } else {
            return false;
        }
    }
    public float operate (float one, float two, string op) {
        if (op == "^") {
            return Mathf.Pow(one, two);
        } else if (op == "*") {
            return one * two;
        } else if (op == "/") {
            return one / two;
        } else if (op == "+") {
            return one + two; 
        } else {
            return one - two;
        }
    }
    public bool isNumb (string enter) {
        if (enter == "1" || enter == "2" || enter == "3" || enter == "4" || enter == "5" || enter == "6" || enter == "7" || enter == "8" || enter == "9" || enter == "0" || enter == ".") {
            return true;
        } else {
            return false;
        }
    }
    public int ending(char[] parts, int start, bool direction) {
        int  tracker = 0;
        bool done = false;
        while(!done) {
            if (direction) {
                tracker++;
            } else {
                tracker--;
            }
            if (start + tracker >= parts.Length || !isNumb(parts[start + tracker].ToString())) {
                done = true;
            }
        }
        return start + tracker - 1;
    }
    public string compressString (char[] parts, int start, int end) {
        string total = "";
        for (int i = start; i < end; i++) {
            total = total + parts[i];
        }
        return total;
    }
    public int compressEnding (char[] parts, int start) {
        int counter = 1;
        int numb = 1;
        while (numb > 0) {
            if (parts[counter + start].ToString() == "(") {
                numb++;
            } else if (parts[counter + start].ToString() == ")") {
                numb--;
            }
            counter++;
        }
        return start + counter + 1;
    }
    public bool specialOperator (string enter) {
        if (enter == "+" || enter == "*" || enter == "/") {
            return true;
        } else {
            return false;
        }
    }
    public float varPower(char[] parts, int varIndex) {
        float returning = 0f;
        if (varIndex + 1 < parts.Length) {
            int tempCount = varIndex + 1;
            bool problem = true;
            bool exponent = false;
            while (tempCount < parts.Length && problem) {
                if (specialOperator(parts[tempCount].ToString())) {
                    problem = false;
                } else if (parts[tempCount].ToString() == "^") {
                    exponent = true;
                } else if (isNumb(parts[tempCount].ToString()) || parts[tempCount].ToString() == "-") {
                    if (exponent) {
                        if (parts[tempCount].ToString() == "-") {
                            float.TryParse(compressString(parts, tempCount + 1, ending(parts, tempCount + 1, true) + 1), out returning);
                            returning *= -1;
                        } else {
                            float.TryParse(compressString(parts, tempCount , ending(parts, tempCount, true) + 1), out returning);
                        }
                    }
                    problem = false;
                }
                tempCount++;
            }
        }
        return returning;
    }
    public void sort () {
        for (int i = 0; i < below.Count; i++) {
            below.RemoveAt(0);
        } 
        for (int i = 0; i < level.Count; i++) {
            level.RemoveAt(0);
        }
        char[] parts = entered.ToCharArray();
        int counter = 0;
        while (counter < parts.Length) {
            if (parts[counter].ToString() == "(") {
                int temp = compressEnding(parts, counter);
                OrderDown tempDown = new OrderDown();
                tempDown.entered = compressString(parts, counter + 1, temp - 1);
                below.Add(tempDown);
                if (counter > 0 && isNumb(parts[counter - 1].ToString())) {
                    level.Add("*");
                }
                level.Add(" ");
                counter = temp - 1;
            } else {
                if (isNumb(parts[counter].ToString())) {
                    int end = ending(parts, counter, true); 
                    level.Add(compressString(parts, counter, end + 1));
                    counter = end + 1;
                } else {
                    if (operater(parts[counter].ToString()) || variableYes(parts[counter].ToString())) {
                        if (variableYes(parts[counter].ToString())) {
                            float potentialPower = varPower(parts, counter);
                            if (potentialPower != 0) {
                                varPowers[variableType(parts[counter].ToString())] = potentialPower;
                            } else {
                                varPowers[variableType(parts[counter].ToString())] = 1;
                            }
                            if (counter > 0) {
                                if (isNumb(parts[counter - 1].ToString())) {
                                    level.Add("*");
                                }
                            }

                        } 
                        level.Add(parts[counter].ToString());
                    } else if (parts[counter].ToString() == "-") {
                        if (level.Count > 0) {
                            level.Add("+");
                        }
                        level.Add("-1");
                        level.Add("*");
                    }
                    counter++;
                }
            }
        }
        for (int i = 0; i < below.Count; i++) {
            below[i].sort();
        }
        int belowcount = 0;
        float[] tempBelow  = new float[3];
        for (int i = 0; i < varPowers.Length; i++) {
            tempBelow[i] = varPowers[i];
        }
        for (int i = 0; i < level.Count; i++) {
            if (level[i] == " ") {
                if (i < level.Count - 2 && level[i+1] == "^") {
                    for (int j = 0; j < below[belowcount].varPowers.Length; j++) {
                        float multiplier= 1;
                        float.TryParse(level[i+2], out multiplier);
                        if (below[belowcount].varPowers[j] * multiplier > tempBelow[j]) {
                            tempBelow[j] = below[belowcount].varPowers[j] * multiplier;
                        }
                    }
                }
                belowcount++;
            }
        }
        for (int i = 0; i < varPowers.Length; i++) {
            varPowers[i] = tempBelow[i];
        }
    }
    public List<string> reformer (List<string> current, string[] ops) {
        List<string> temp = current;
        List<int> place = new List<int>();
        for (int i = 0; i < temp.Count; i++) {
            for (int j = 0; j < ops.Length; j++) {
                if (temp[i] == ops[j]) {
                    place.Add(i);
                }
            }
        }
        for (int i = place.Count - 1; i >= 0; i--) {
            float one;
            float two;
            float.TryParse(temp[place[i] - 1], out one);
            float.TryParse(temp[place[i] + 1], out two);
            temp[place[i]] = operate(one, two, temp[place[i]]).ToString();
            temp.RemoveAt(place[i] + 1);
            temp.RemoveAt(place[i] - 1);
        }
        return temp;
    }
    public void calcValue (float[] varValues) {
        string[] vars = {"x", "y", "z"};
        int belowCount = 0;
        List<string> temp = new List<string>();
        for (int i = 0; i < level.Count; i++) {
            temp.Add(level[i]);
        }
        for (int i = 0; i < temp.Count; i++) {
            if (temp[i] == " ") {
                below[belowCount].calcValue(varValues);
                temp[i] = below[belowCount].value.ToString();
                belowCount++;
            }
            for (int j = 0; j < vars.Length; j++) {
                if (vars[j] == temp[i]) {
                    temp[i] = varValues[j].ToString();
                }
            }
        }
        string[] first = {"^"};
        string[] second = {"*", "/"};
        string[] third = {"+", "-"};
        temp = reformer(temp, first);
        temp = reformer(temp, second);
        temp = reformer(temp, third);
        if (temp.Count == 1) {
            float.TryParse(temp[0], out value);
        } else {
            Debug.Log("Problems somewhere");
        }
    }
}