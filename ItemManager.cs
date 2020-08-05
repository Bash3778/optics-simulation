using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public bool on = false;
    public bool butOn = true;
    public bool turn = false;
    public bool pressed = false;
    public string name = " ";
    public string[] extra;
    public InputField[] sections;
    public InputField[] fields;
    public Lenscrafter craft;
    public List<GameObject> lowers = new List<GameObject>();
    public GameObject current;
    public ItemManager higher;
    public int type;   
    public Transform generalCanvas;
    public int indexRefIndex = 4;
    InputField currentChange;
    int currentInt;
    // Start is called before the first frame update.  There seems to be two planes in a row
    void Start()
    {
        string[] temp = {"", "", "", "", ""};
        extra = temp;
        if (type <= 1) { 
            for (int i = 0; i < extra.Length; i++) {
                sections[i].text = extra[i];
            }
        } else {
            for (int i = 0; i < extra.Length - 1; i++) {
                sections[i].text = extra[i];
            }
        }
        butOn = true;
        this.gameObject.GetComponent<Button>().onClick.AddListener(activePress);
        if (type == 0)
        {
            if (craft != null)
            {
                name = "item " + craft.items.Count.ToString();
            }
        }
        else
        {
            if (higher != null)
            {
                if (type == 1)
                {
                    name = "layer " + higher.lowers.Count.ToString();
                }
                else
                {
                    name = "plane " + higher.lowers.Count.ToString();
                }
            }
        } 
        RectTransform recter = this.gameObject.GetComponent<Button>().GetComponent<RectTransform>();
        RectTransform genRect = generalCanvas.GetComponent<RectTransform>();
        recter.anchorMin = new Vector2((transform.position.x - recter.rect.width / 2) / genRect.rect.width, (transform.position.y - recter.rect.height / 2) / genRect.rect.height);
        recter.anchorMax = new Vector2((transform.position.x + recter.rect.width / 2) / genRect.rect.width, (transform.position.y + recter.rect.height / 2) / genRect.rect.height);
        recter.offsetMin = Vector2.zero;
        recter.offsetMax = Vector2.zero;
    }
    // Update is called once per frame
    void Update()
    {
        if (type > 0) {
            if (!higher.on) {
                on = false;
                butOn = false;
            }
        }
        if (turn) {
            butOn = onOffReturn(butOn);
            if (on) {
                on = false;
            }
            turn = false;
        }
        if (butOn)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        if (type != 2) {
            int countIndex = -1;
            for (int i = 0; i < lowers.Count; i++)
            {
                if (lowers[i].GetComponent<ItemManager>().pressed)
                {
                    countIndex = i;
                    currentChange = fields[lowers[i].GetComponent<ItemManager>().type];
                    currentInt = i;
                    lowers[i].GetComponent<ItemManager>().pressed = false;
                    current = lowers[i];
                }
            }
            if (countIndex != -1)
            {
                for (int i = 0; i < lowers.Count; i++)
                {
                    if (i != countIndex)
                    {
                        lowers[i].GetComponent<ItemManager>().on = false;
                    }
                }
            }
        }
        if (fields != null && on && fields[type].text != "") {
            name = fields[type].text;
        }
        if (sections != null && on) {
            for (int i = 0; i < sections.Length - 1; i++) {
                if (sections[i].text != "") {
                    extra[i] = sections[i].text;
                }
            }
            if (type == 1) {
                extra[extra.Length - 1] = sections[sections.Length - 1].text;
            }
        }
        Text texter = this.gameObject.GetComponentInChildren(typeof(Text)) as Text;
        texter.text = name;
    }
    private void LateUpdate()
    {
        if (currentChange != null)
        {
            if (type != 2) {
                currentChange.text = lowers[currentInt].GetComponent<ItemManager>().name;
                currentInt = 0;
                currentChange = null;
            }
        }
    }
    public bool onOffReturn(bool current) {
        return !current;
    }
    public void onChange (bool set) {
        for (int i = 0; i < lowers.Count; i++) {
            List<GameObject> plane = lowers[i].GetComponent<ItemManager>().lowers;
            for (int j = 0; j < plane.Count; j++) {
                List<GameObject> dot = plane[j].GetComponent<ItemManager>().lowers;
                for (int k = 0; k < dot.Count; k++) {
                    dot[k].SetActive(set);
                }
            }
        }
    }
    void activePress() {
        on = onOffReturn(on);
        if (on)
        {
            fields[type].text = name;
            if (type == 2) {
                for (int i = 0; i < sections.Length - 1; i++) {
                    if (sections[i] != null) {
                        sections[i].text = extra[i];
                    }
                }
            } else if (type == 0) {
                onChange(true);
            } else {
                sections[sections.Length - 1].text = extra[extra.Length - 1];
            }
        }
        else {
            fields[type].text = ""; 
            if (type <= 1) {
                for (int i = 0; i < sections.Length; i++) {
                    if (sections[i] != null) {
                        sections[i].text = "";
                    }
                }
            } else {
                for (int i = 0; i < sections.Length - 1; i++) {
                    if (sections[i] != null) {
                        sections[i].text = "";
                    }
                }
            }
            if (type == 0) {
                onChange(false);
            }
        }
        pressed = true;
        for (int i = 0; i < lowers.Count; i++)
        {
            if (!lowers[i].gameObject.activeInHierarchy) {
                lowers[i].gameObject.SetActive(true);
            }
            if (type != 2) {
                lowers[i].GetComponent<ItemManager>().turn = true;
            }
        }
    }
}