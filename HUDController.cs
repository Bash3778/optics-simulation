using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HUDController : EventTrigger
{
    [SerializeField] Vector2 starter;
    public Vector2 revised;
    public Vector2 direction;
    [SerializeField] Image staticer;
    [SerializeField] Image wheel;
    [SerializeField] Image fake;
    public float distance;
    bool down = false;
    bool reviseController = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        starter = fake.transform.position;
        if (down == false)
        {
            staticer.transform.position = Vector2.Lerp(staticer.transform.position, starter, 6f);
            wheel.transform.position = Vector2.Lerp(wheel.transform.position, starter, 6f);
            direction = starter;
            revised = starter;
        }
        else
        {
            staticer.transform.position = revised;
            wheel.transform.position = direction;
        }
    }
    public override void OnDrag(PointerEventData data)
    {
        down = true;
        if (!reviseController)
        {
            revised = data.position;
            direction = data.position;
            reviseController = true;
        }
        if (Vector2.Distance(revised, data.position) < distance)
        {
            direction = data.position;
        }
        else
        {
            float xer;
            float yer;
            float distx = Vector2.Distance(new Vector2(data.position.x, 0), new Vector2(revised.x, 0));
            float disty = Vector2.Distance(new Vector2(0, data.position.y), new Vector2(0, revised.y));
            float dist = Vector2.Distance(data.position, revised);
            xer = ((distx / dist) * distance) + revised.x;
            yer = ((disty / dist) * distance) + revised.y;
            if (data.position.y < revised.y)
            {
                yer = ((yer - revised.y) * -1) + revised.y;
            }
            if (data.position.x < revised.x)
            {
                xer = ((xer - revised.x) * -1) + revised.x;
            }
            direction = new Vector2(xer, yer);
        }
    }

    public override void OnPointerUp(PointerEventData data)
    {
        down = false;
        reviseController = false;
    }
}