using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float moveAmount = 2f;

    [SerializeField]
    private Transform positionA;
    [SerializeField]
    private Transform positionB;

    [SerializeField]
    private Transform[] points;

    private Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;

        #region challenge 3
        StartCoroutine(MoveCube());
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region challenge 1
        //var newPosition = transform.position;
        //newPosition.x = startingPosition.x + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        //transform.position =  newPosition;
        #endregion

        #region challenge 2
        //float time = Mathf.PingPong(Time.time * moveSpeed, 1);
        //transform.position = Vector3.Lerp(positionA.position, positionB.position, time);

        #endregion

        
    }
    #region challenge 3 solution
    private IEnumerator MoveCube()
    {
        transform.position = this.points[0].position;

        int targetIndex = 1;

        Vector3 targetPoint = this.points[targetIndex].position;

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

            if(transform.position == targetPoint)
            {
                targetIndex = (targetIndex +1) % this.points.Length;
                targetPoint = this.points[targetIndex].position;
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
        
    }
    #endregion
}
