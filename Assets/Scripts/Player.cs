using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float velocity = 5f;
    [SerializeField] public Transform spriteTransform;


    [SerializeField] public Animator animator;

    private Transform breakables;


    //Movement variables
    private Vector2 touchBeginPosition;
    private Vector2 touchEndPosition;
    private Vector2 normalizedDirection;
    public LayerMask whatStopsMovement; //Layer that stops the player from moving in that direction
    public LayerMask whatAllowsMovement; //Layer that allows the player from moving in that direction
    private bool isCosmetic;


    public bool IsMovementLocked { set; get; } = true;

    public bool IsJumping { set; get; }
    
    public void SetIsCosmetic(bool value) {
        isCosmetic = value;
        Debug.Log("Cosmetic set to " + isCosmetic);
    }

    public bool IsCosmetic() {
        return isCosmetic;
    }

    public Transform MovePoint
    {
        set { spriteTransform = value; }
        get { return spriteTransform; }
    }
    // Start is called before the first frame update
    void Start()
    {
        breakables = transform.parent.GetChild(0).GetComponent<Transform>();
        spriteTransform.GetComponent<SphereScript>().OgPlayer = this.gameObject;
        spriteTransform.parent = transform.parent;

    }

    // Update is called once per frame
    void Update()
    {

        ChangeDirections();
        Move();

    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (IsCosmetic()) return;
        
        if (Physics2D.OverlapCircle(transform.position + new Vector3(1f, 0f, 0f), .2f, whatStopsMovement)
            && Physics2D.OverlapCircle(transform.position + new Vector3(-1f, 0f, 0f), .2f, whatStopsMovement)
            && Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1f, 0f), .2f, whatStopsMovement)
            && Physics2D.OverlapCircle(transform.position + new Vector3(0f, -1f, 0f), .2f, whatStopsMovement)
            && !IsJumping)
        {


            if (CheckWin() && collision.gameObject.name == "Win") {
                IsMovementLocked = true;
                GameManager.Instance.SetIsCurrentLevelCompleted(true);
            }
            else
            {
                //               StartCoroutine(stuck(1.5f));
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(spriteTransform.gameObject);
    }
    IEnumerator stuck(float seconds)
    {
        IsJumping = true;
        IsMovementLocked = true;
        yield return new WaitForSecondsRealtime(1);
        IsJumping = false;
        if (!CheckWin())
        {
            if ((Physics2D.OverlapCircle(transform.position + new Vector3(1f, 0f, 0f), .2f, whatStopsMovement)
            && Physics2D.OverlapCircle(transform.position + new Vector3(-1f, 0f, 0f), .2f, whatStopsMovement)
            && Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1f, 0f), .2f, whatStopsMovement)
            && Physics2D.OverlapCircle(transform.position + new Vector3(0f, -1f, 0f), .2f, whatStopsMovement))
            || (!IsJumping && Physics2D.OverlapCircle(transform.position, .2f, whatStopsMovement)))
            {
                yield return new WaitForSecondsRealtime(seconds - 1);
                GameManager.Instance.IsStuck = true;
            }
            else
            {
                IsMovementLocked = false;
            }

        }

    }

    private bool CheckWin() {
        if (IsCosmetic()) return false;
        int brokenNumber = 0;
        foreach (Transform child in breakables)
        {
            if (child.gameObject.GetComponent<Breakable>().Broken)
            {
                brokenNumber++;
            }
        }
        return brokenNumber == breakables.childCount && !IsJumping && !IsMovementLocked;
    }
    #region Private Functions
    /**
     *  Set moving directorion based on current finger movement
     */
    private void ChangeDirections() {
        if(IsCosmetic()) return;

        //Get starting and ending postions of screen touches
        if (Input.touchCount > 0
         && Input.GetTouch(0).phase == TouchPhase.Began) {
            touchBeginPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        if (Input.touchCount > 0) {
            touchEndPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }

        // Get movement of the finger since last frame
        normalizedDirection = new Vector2(touchEndPosition.x - touchBeginPosition.x, touchEndPosition.y - touchBeginPosition.y);

        // Round the movement of the finger to values of 1 and -1;
        //X
        const float distanceRequirement = .2f;
        if (normalizedDirection.x >= distanceRequirement && Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.y)) {
            normalizedDirection.x = 1;
        } else if (normalizedDirection.x <= -distanceRequirement && Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.y)) {
            normalizedDirection.x = -1;
        } else {
            normalizedDirection.x = 0;
        }
        //Y
        if (normalizedDirection.y >= distanceRequirement && Mathf.Abs(normalizedDirection.x) < Mathf.Abs(normalizedDirection.y)) {
            normalizedDirection.y = 1;
        } else if (normalizedDirection.y <= -distanceRequirement && Mathf.Abs(normalizedDirection.x) < Mathf.Abs(normalizedDirection.y)) {
            normalizedDirection.y = -1;
        } else {
            normalizedDirection.y = 0;
        }

        //Reset values if no touch is detected
        if (Input.touchCount == 0) {
            touchBeginPosition = touchEndPosition;
            animator.SetInteger("MoveAnimation", 0);
        }

    }
    private void Move() {
        if(IsCosmetic()) return;
        // Move towards targetPoint at all times.
        spriteTransform.position = Vector3.MoveTowards(spriteTransform.position, transform.position, velocity * Time.deltaTime);

        // Move targetPoint if it is on top of the Player.
        if (Vector3.Distance(transform.position, spriteTransform.position) <= .3f 
            && Input.touchCount == 0 
            && !IsMovementLocked) {
            IsJumping = false;
            //Move horizontaally...
            if (Mathf.Abs(normalizedDirection.x) == 1.0f) {
                //..only if no overlap with objects that do not allow movement
                if (!Physics2D.OverlapCircle(transform.position + new Vector3(normalizedDirection.x, 0f, 0f), .2f, whatStopsMovement)
                    && Physics2D.OverlapCircle(transform.position + new Vector3(normalizedDirection.x, 0f, 0f), .2f, whatAllowsMovement)) {
                    transform.position += new Vector3(normalizedDirection.x, 0f, 0f);

                    
                    //Change animations
                    if (normalizedDirection.x == 1) {
                        animator.SetInteger("MoveAnimation", 1);
                    } else if (normalizedDirection.x == -1) {
                        animator.SetInteger("MoveAnimation", 3);
                    }
                }

                //... or move vertically
            } else if (Mathf.Abs(normalizedDirection.y) == 1.0f) {
                //..only if no overlap with objects that do not allow movement
                if (!Physics2D.OverlapCircle(transform.position + new Vector3(0f, normalizedDirection.y, 0f), .2f, whatStopsMovement)
                    && Physics2D.OverlapCircle(transform.position + new Vector3(0f, normalizedDirection.y, 0f), .2f, whatAllowsMovement)) {
                    transform.position += new Vector3(0f, normalizedDirection.y, 0f);
                    
                }
            }
        }
    }

    #endregion
}
