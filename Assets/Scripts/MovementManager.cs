using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    //get the pacStudent GameObject from the MovementManager GameObject this script will be attached to
    [SerializeField]
    public GameObject pacStudent;
    
    //private variables for the Tweener and Animator components
    private Tweener tweener;
    private Animator animator;
    
    //fixed positions and animation states for pacStudent, each starting position will have a corresponding animation state
    private Vector3[] pacStudentPositions = {
        new Vector3(-20.5f, 6.0f, 0.0f),
        new Vector3(-15.5f, 6.0f, 0.0f),
        new Vector3(-15.5f, 2.0f, 0.0f),
        new Vector3(-20.5f, 2.0f, 0.0f)
    };
    private string[] pacStudentStates = {
        "PacStudentLeft",
        "Down",
        "Right",
        "Up"
    };
    
    //speed of pacStudent's LERP movement
    private int speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        //Get the Tweener component and animator with animation states of pacStudent from the GameObject
        tweener = GetComponent<Tweener>();
        animator = pacStudent.GetComponent<Animator>();

        //Reset the pacStudent to start position
        pacStudent.transform.position = pacStudentPositions[0];
    }


    // Update is called once per frame
    void Update()
    {
        //for each fixed cardinal position on first quadrant, if pacStudent's position , start move to the next position and play the corresponding animation state
        for (int i = 0; i < pacStudentPositions.Length; i++) {
            if(pacStudent.transform.position == pacStudentPositions[i]) {
                float duration = Vector3.Distance(pacStudentPositions[i], pacStudentPositions[(i+1)%4]) / speed;
                tweener.AddTween(pacStudent.transform, pacStudentPositions[i], pacStudentPositions[(i+1)%4], duration);
                animator.Play(pacStudentStates[i]);
            }
        }
    }

}