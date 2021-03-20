using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushableButton : MonoBehaviour
{
    [HideInInspector]
    public bool isPushed;

    [Tooltip("List of objects that can push this button")]
    public List<GameObject> stuffThatCanPushThisButton = new List<GameObject>();
    [Tooltip("Direction that the button can be pushed into")]
    public ButtonPushDirection direction;
    [Tooltip("Doesn't need setting unless direction is special")]
    public Vector3 directionOfDown;
    [Tooltip("how far button can be pushed from")]
    public float distanceToTouch = 5;
    [Tooltip("how far the button can be pushed in (.9 of directionally-relevant scale recommended)")]
    public float maxMovement = 3;

    [Tooltip("what happens when you press the button")]
    public UnityEvent ButtonPress = new UnityEvent();

    Vector3 initialPos;
    Vector3 lastPosOfPusher;
    Vector3 move;
    Transform objPushing, objHolder;
    bool beingPushed;
    float closestObj;
    float distance, distanceHold;
    float distanceFromStart;
    float distanceToMove;
    bool activated = false;

    void Awake()
    {
        if (ButtonPress == null)
            ButtonPress = new UnityEvent();
    }

    void Start()
    {
        initialPos = transform.position;

        if (direction == ButtonPushDirection.Down)
            directionOfDown = Vector3.down;
        else if (direction == ButtonPushDirection.Up)
            directionOfDown = Vector3.up;
        else if (direction == ButtonPushDirection.Backward)
            directionOfDown = Vector3.back;
        else if (direction == ButtonPushDirection.Forward)
            directionOfDown = Vector3.forward;
        else if (direction == ButtonPushDirection.Left)
            directionOfDown = Vector3.left;
        else if (direction == ButtonPushDirection.Right)
            directionOfDown = Vector3.right;
    }

    void FixedUpdate()
    {
        if (!beingPushed)
        {
            closestObj = float.MaxValue;
            for (int counter = 0; counter < stuffThatCanPushThisButton.Count; counter++)
            {
                objHolder = stuffThatCanPushThisButton[counter].transform;

                distanceHold = Vector3.Distance(objHolder.position, transform.position);

                if (distanceHold < closestObj)
                {
                    closestObj = distanceHold;
                    distance = distanceHold;
                    objPushing = objHolder;
                }
            }

            if (distance < distanceToTouch)
            {
                //Move button towards pushed in position
                beingPushed = true;
                lastPosOfPusher = objPushing.position;
            }
            else
                objPushing = null;
        }

        if(distance > (distanceFromStart + distanceToTouch))
        {
            beingPushed = false;
            objPushing = null;
            transform.position = initialPos;
        }

        if(beingPushed && LimitPressDirection())
        {
            float lastDistance = Vector3.Distance(lastPosOfPusher, transform.position);
            float currDistance = Vector3.Distance(objPushing.position, transform.position);

            distanceToMove = lastDistance - currDistance;
            move = transform.position + (directionOfDown.normalized * distanceToMove);

            if(Vector3.Distance(transform.position, initialPos) <= maxMovement)
                transform.position = move;
            else if (Vector3.Distance(transform.position, initialPos) > maxMovement)
                transform.position = initialPos + (directionOfDown.normalized * maxMovement);

            if (Vector3.Distance(transform.position, (initialPos + (directionOfDown.normalized * maxMovement))) > Vector3.Distance(initialPos, (initialPos + (directionOfDown.normalized * maxMovement))))
                transform.position = initialPos;
        }

        distanceFromStart = Vector3.Distance(initialPos, transform.position);

        if (beingPushed)
        {
            lastPosOfPusher = objPushing.position;
            distance = Vector3.Distance(objPushing.position, transform.position) - distanceToTouch;
            isPushed = (distanceFromStart > maxMovement / 2 && !activated);
        }

        if (isPushed && !activated)
            Press();

        if (activated && distanceFromStart == 0)
            activated = false;
    }

    /// <summary>
    /// what happens when you press the button
    /// </summary>
    public void Press()
    {
        isPushed = false;
        activated = true;
        ButtonPress.Invoke();
    }

    /// <summary>
    /// limits how far the button is pushed in
    /// </summary>
    /// <returns>bool whether you can continue to move the button in</returns>
    bool LimitPressDirection()
    {
        bool check = false;

        if (direction == ButtonPushDirection.Special)
            check = true;
        else if (direction == ButtonPushDirection.Down)
            if (objPushing.position.y > transform.position.y)
                check = true;
        else if (direction == ButtonPushDirection.Up)
            if (objPushing.position.y < transform.position.y)
                check = true;
        else if (direction == ButtonPushDirection.Backward)
            if (objPushing.position.z > transform.position.z)
                check = true;
        else if (direction == ButtonPushDirection.Forward)
            if (objPushing.position.z < transform.position.z)
                check = true;
        else if (direction == ButtonPushDirection.Left)
            if (objPushing.position.x > transform.position.x)
                check = true;
        else if (direction == ButtonPushDirection.Right)
            if (objPushing.position.x < transform.position.x)
                check = true;

        return check;
    }
}