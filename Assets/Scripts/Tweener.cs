using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    //private Tween activeTween;
    private List<Tween> activeTweens = new List<Tween>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if there are active tweens in the List, loop through each.
        for (int i = activeTweens.Count - 1; i >= 0; i--) {
            //calculate the distance between the active tween's object's current position and the end position.
            float distance = Vector3.Distance(activeTweens[i].Target.position, activeTweens[i].EndPos);
            //if the distance is greater than 0.1f, we still need to Lerp to end position.
            if (distance > 0.1f) {
                //calculate the time fraction that has passed since the tween started.
                float t = (Time.time - activeTweens[i].StartTime) / activeTweens[i].Duration;
                //Lerp the object's position from the start position to the end position using the time fraction calculated.
                activeTweens[i].Target.position = Vector3.Lerp(activeTweens[i].StartPos, activeTweens[i].EndPos, t);
            }
            //if the distance is less than 0.1f, we have reached the end position, so set the object's position to the end position and remove the tween from the List.
            else 
            {
                activeTweens[i].Target.position = activeTweens[i].EndPos;
                //Due to this, cannot use foreach loop to iterate through the List.
                activeTweens.RemoveAt(i);
            }
        }
    }
    
    public bool AddTween (Transform targetObject, Vector3 startPos, Vector3 endPos, float duration) {
        //if the tween does not exist, add it to the List and return true.
        if (!TweenExists(targetObject)) {
            //Add a new Tween to the List with the target object, start position, end position, start time, and duration.
            activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration));
            return true;
        }
        return false;
    }
    
    //New: Remove all tweens of the target object from the List.
    public bool RemoveTween(Transform targetObject) {
        if (TweenExists(targetObject)) {
            activeTweens.RemoveAll(item => item.Target == targetObject);
            return true;
        }
        return false;
    }

    public bool TweenExists (Transform target) {
        //loop through each active tween in the List and check if the target object is the same as the tween's target object.
        foreach (Tween tween in activeTweens) {
            if (tween.Target.Equals(target))
                return true;
        }
        return false;
    }
    
    public bool isTweening(Transform target) {
        //return true of activeTweens list has any tween running.
        return activeTweens.Count > 0;
    }
}