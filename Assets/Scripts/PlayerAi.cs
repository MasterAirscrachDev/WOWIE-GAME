using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAi : MonoBehaviour
{
    Vector3 goal, currentTarget, distraction, exploreTarget, startPos;
    [SerializeField] state aState;
    CharacterController CC;
    List<GameObject> Lines = new List<GameObject>();
    [SerializeField] float speed;
    [SerializeField] GameObject LinePrefab;
    [SerializeField] Material[] lights;
    [SerializeField] AudioClip[] sounds;
    [SerializeField] List<Transform> explorePoints = new List<Transform>();
    AudioSource AS;
    RaycastHit hit;
    InteractibleTile tile;
    int stuckTicks = 0, raveindex = 0, dir = 0;
    bool tryingWander = false, ravemode = false, onMovingPlatform = false;

    // Start is called before the first frame update
    void Start()
    {
        //add all the explore points to the list
        GameObject[] points = GameObject.FindGameObjectsWithTag("ExplorePoint");
        for(int i = 0; i < points.Length; i++){
            explorePoints.Add(points[i].transform);
        }
        AS = GetComponent<AudioSource>();
        try{ goal = GameObject.FindGameObjectWithTag("Finish").transform.position + (Vector3.up * 0.5f); }
        catch{ goal = new Vector3(100,10,100); }
        CC = GetComponent<CharacterController>();
        startPos = transform.position;
        SetLine(0, new Vector3[] {Vector3.zero, Vector3.zero}, Color.green);
        SetLine(1, new Vector3[] {Vector3.zero, Vector3.zero}, Color.green);
        SetLine(2, new Vector3[] {Vector3.zero, Vector3.zero}, Color.green);
        StartCoroutine(CheckGoal());
    }

    // TODO: FIX Player Not registering moving tiles
    void Update()
    {
        Vector3 move = transform.position;
        Vector3 eyePos = transform.position + (Vector3.up * 0.44f);
        //INDECATOR MATERIAL #############################################################################
        Material mat;

        //if 1 and 9 are pressed, ravemode is enabled
        if(Input.GetKey(KeyCode.Alpha1) && Input.GetKeyDown(KeyCode.Alpha9)){
            ravemode = !ravemode; Debug.Log("Ravemode: " + ravemode);
        }
        if(ravemode){
            raveindex++;
            if(raveindex > lights.Length - 1){ raveindex = 0; }
            mat = lights[raveindex];
        }
        else{
            if(aState == state.Goal){ mat = lights[0]; }
            else if (aState == state.Distraction){ mat = lights[1]; }
            else if(aState == state.Explore){ mat = lights[3]; }
            else{ mat = lights[2]; }
        }
        Material[] mats = transform.GetChild(0).GetComponent<Renderer>().materials;
        mats[3] = mat;
        transform.GetChild(0).GetComponent<Renderer>().materials = mats;

        if(onMovingPlatform){
            try{
                if(tile.isMoving){ return; }
                else{ onMovingPlatform = false; }
            }
            catch{
                onMovingPlatform = false;
                aState = state.Idle;
            }
        }
        //raycast down for 100 units, if nothing teleports to start position
        if (!Physics.SphereCast(transform.position, 0.2f, Vector3.down, out hit, 100))
        { transform.position = startPos; AS.PlayOneShot(sounds[1]); aState = state.Idle; onMovingPlatform = false; return; }
        transform.parent = hit.collider.gameObject.transform;
        // GOAL DETECTION ###################################################################################################
        //if the goal lower or on the same height as the player
        if(goal.y <= transform.position.y + 0.2f){
            if(CheckPath(Vector3.Lerp(transform.position, goal, 0.5f), goal, 0.1f)){
                aState = state.Goal;
                move = goal;
            }
            else{ if(!tryingWander && aState == state.Idle){ StartCoroutine(Explore()); } }
        }
        else{
            SetLine(0, new Vector3[] {Vector3.zero, Vector3.zero}, Color.red);
            if(!tryingWander && aState == state.Idle){ StartCoroutine(Explore()); }
        }
        
        // DISTRACTION DETECTION ################################################################################################
        if(aState == state.Distraction){
            move = distraction;
            //if we are close to the distraction, go to goal
            if(CC.velocity.magnitude < 0.1f){ stuckTicks++; }
            if(Vector3.Distance(transform.position, distraction) < 0.1f || stuckTicks > 10){
                aState = state.Idle; stuckTicks = 0;
            }
        }
        // EXPLORE DETECTION #####################################################################################################
        if(aState == state.Explore){
            move = exploreTarget;
            if(CC.velocity.magnitude < 0.1f){ stuckTicks++; }
            if(Vector3.Distance(transform.position, exploreTarget) < 0.1f || stuckTicks > 10){
                //Debug.Log("retry explore");
                aState = state.Idle;
                StartCoroutine(Explore(0,0));
                stuckTicks = 0;
            }
        }
        // MOVE TILE CHECK #########################################################################################################
        Debug.DrawRay(transform.position, Vector3.down * 0.6f, Color.red, 3f);
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 0.6f)){
            if(hit.transform.GetComponent<InteractibleTile>() != null){
                tile = hit.transform.GetComponent<InteractibleTile>();
                if(tile.isMoving){
                    aState = state.Idle;
                    Debug.Log("Tile is moving");
                    onMovingPlatform = true;
                    return;
                }
            }
        }
        //get the distance from ground
        Physics.Raycast(eyePos, Vector3.down, out hit, 100);
        float dist = Vector3.Distance(transform.position, hit.point);
        if(dist > 0.7f){
            aState = state.Idle;
            move = hit.point + (Vector3.up * 0.5f);
        }
        Movement(move);
    }
    void Movement(Vector3 target){
        Vector3 move = Vector3.zero;
        if(currentTarget != target){
            Vector3 midpoint, eye = Vector3.up * 0.44f;
            currentTarget = target;
            if(Mathf.Abs(currentTarget.x) < Mathf.Abs(currentTarget.z)){
                //raycast 
                midpoint = new Vector3(currentTarget.x, transform.position.y, transform.position.z);
                //raycast from the player to the midpoint
                if(CheckPath(midpoint, currentTarget)){ dir = 1; }
                else{
                    midpoint = new Vector3(transform.position.x, transform.position.y, currentTarget.z);
                    if(CheckPath(midpoint, currentTarget)){ dir = 2; }
                    else{
                        aState = state.Idle;
                        StartCoroutine(Explore(0,0));
                        return;
                        //bad route
                    }
                }
            }
            else{
                midpoint = new Vector3(transform.position.x, transform.position.y, currentTarget.z);
                //raycast
                if(CheckPath(midpoint, currentTarget)){ dir = 2; }
                else{
                    midpoint = new Vector3(currentTarget.x, transform.position.y, transform.position.z);
                    if(CheckPath(midpoint, currentTarget)){ dir = 1; }
                    else{
                        aState = state.Idle;
                        StartCoroutine(Explore(0,0));
                        return;
                        //bad route
                    }
                }
            }
            if(aState == state.Goal){ SetLine(0, new Vector3[] {transform.position + eye, midpoint + eye, target + eye}, Color.green); }
            else if(aState == state.Distraction){ SetLine(1, new Vector3[] {transform.position + eye, midpoint + eye, target + eye}, Color.blue); }
            else if(aState == state.Explore){ SetLine(2, new Vector3[] {transform.position + eye, midpoint + eye, target + eye}, new Color(1, 0.5181f, 0)); }
            
            //do any state stuff
            if(aState == state.Explore){
                int sound = Random.Range(4,10);
                AS.PlayOneShot(sounds[sound]);
            }
        }
        //Debug.Log(currentTarget);
        if(dir == 1){
            move.x = currentTarget.x - transform.position.x;
            if(Mathf.Abs(move.x) < 0.05f){ dir = 2; }
        }
        else if (dir == 2){
            move.z = currentTarget.z - transform.position.z;
            if(Mathf.Abs(move.z) < 0.05f){ dir = 1; }
        }
        
        move = Vector3.Normalize(move);
        Debug.DrawRay(transform.position, move, Color.green);
        if(ravemode){move *= 1.2f;}
        move.y += -1;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 0.55f)){ move.y = -1; }
        CC.Move(move * speed * Time.deltaTime);
    }
    bool CheckPath(Vector3 mid, Vector3 end, float rayTime = 5f){
        //raycast from the player to the midpoint
        Debug.DrawLine(transform.position, mid, Color.yellow, rayTime);
        if(Physics.Raycast(transform.position, mid - transform.position, out hit, (mid - transform.position).magnitude)){ return false; }
        else{
            for(float i = 0; i < 1; i += 0.1f){
                //raycast from the midpoint to the end
                Debug.DrawRay(Vector3.Lerp(transform.position, mid, i), Vector3.down * 0.6f, Color.yellow, rayTime);
                if(!Physics.Raycast(Vector3.Lerp(transform.position, mid, i), Vector3.down, out hit, 0.6f)){ return false; }
            }
        }
        //raycast from the midpoint to the end
        Debug.DrawLine(mid, end, Color.yellow, rayTime);
        if(Physics.Raycast(mid, end - mid, out hit, (end - mid).magnitude)){ return false; }
        else{
            for(float i = 0; i < 1; i += 0.1f){
                //raycast from the midpoint to the end
                Debug.DrawRay(Vector3.Lerp(mid, end, i), Vector3.down * 0.6f, Color.yellow, rayTime);
                if(!Physics.Raycast(Vector3.Lerp(mid, end, i), Vector3.down, out hit, 0.6f)){ return false; }
            }
        }
        return true;
    }
    IEnumerator Explore(int min = 3, int max = 5){
        tryingWander = true;
        yield return new WaitForSeconds(Random.Range(min,max));
        if(aState == state.Idle){
            bool found = false;
            int tries = 0, range = 10, pointIndex = -1;
            //find the nearest explorePoint within range
            float closest = range;
            for(int i = 0; i < explorePoints.Count; i++){
                if(Vector3.Distance(transform.position, explorePoints[i].transform.position) < closest){
                    closest = Vector3.Distance(transform.position, explorePoints[i].transform.position);
                    pointIndex = i;
                    Debug.DrawLine(transform.position, explorePoints[i].transform.position, Color.green, 5);
                }
            }
            if(pointIndex != -1){
                exploreTarget = explorePoints[pointIndex].transform.position;
                Debug.DrawRay(exploreTarget, Vector3.down, Color.cyan, 2);
                Debug.DrawRay(transform.position, exploreTarget - transform.position, Color.cyan, 2);
                if(!Physics.Raycast(transform.position, exploreTarget - transform.position, (exploreTarget - transform.position).magnitude) && Physics.Raycast(exploreTarget, Vector3.down, out hit)){
                    if(hit.collider != null){
                        found = true;
                        StartCoroutine(ConsumeTarget(explorePoints[pointIndex]));
                        Debug.DrawRay(exploreTarget, Vector3.up, Color.green, 2);
                        aState = state.Explore;
                        tryingWander = false;
                        yield break;
                    }
                }
            }
            while(!found){
                exploreTarget = new Vector3(Random.Range(-range,range), 0, Random.Range(-range,range));
                exploreTarget = transform.TransformPoint(exploreTarget);
                Debug.DrawRay(exploreTarget, Vector3.down, Color.red, 2);
                Debug.DrawRay(transform.position, exploreTarget - transform.position, Color.red, 2);
                if(!Physics.Raycast(transform.position, exploreTarget - transform.position, (exploreTarget - transform.position).magnitude) && Physics.Raycast(exploreTarget, Vector3.down, out hit)){
                    if(hit.collider != null){
                        found = true;
                        exploreTarget.y = hit.point.y + 0.5f;
                        Debug.DrawRay(exploreTarget, Vector3.up, Color.green, 2);
                    }
                }
                tries++; range--;
                if(range < 1){ range = 1; }
                if(tries > 20){
                    Debug.Log("couldnt find a place to explore");
                    aState = state.Idle;
                    break;
                }
            }
            aState = state.Explore;
        }
        tryingWander = false;
    }
    IEnumerator ConsumeTarget(Transform toConsume){
        Debug.Log($"consuming {toConsume.name}");
        explorePoints.Remove(toConsume);
        yield return new WaitForSeconds(15);
        explorePoints.Add(toConsume);
        Debug.Log($"{toConsume.name} Re added");
    }
    public void SetDistraction(Vector3 pos){
        if(aState != state.Goal){
            pos.y += 0.1f;
            //check if we can see the distraction
            Physics.Raycast(transform.position, pos - transform.position, out hit, (pos - transform.position).magnitude);
            Debug.DrawRay(transform.position, pos - transform.position, Color.blue, 200);
            pos.y += 0.4f;
            if(hit.collider == null){
                distraction = pos;
                aState = state.Distraction;
                int sound = Random.Range(1, 3);
                AS.PlayOneShot(sounds[sound]);
            }
            else{
                Debug.Log("Cant see distraction");
                Debug.DrawLine(transform.position + (Vector3.up * 0.1f), hit.point + (Vector3.up * 0.1f), Color.cyan, 200);
                Debug.Log("hit: " + hit.collider.name);
            }
        }
    }
    public void BlockCollide()
    { aState = state.Idle; onMovingPlatform = false; transform.parent = null; }
    public void ClearLines(){
        for(int i = 0; i < Lines.Count; i++){ Destroy(Lines[i]); }
        Lines.Clear();
    }
    void SetLine(int line, Vector3[] positions, Color color)
    {
        LineRenderer LR;
        try{ LR = Lines[line].GetComponent<LineRenderer>(); }
        catch{
            Lines.Add(Instantiate(LinePrefab, transform));
            LR = Lines[line].GetComponent<LineRenderer>();
        }
        
        LR.positionCount = positions.Length;
        LR.SetPositions(positions);
        LR.startColor = color;
        LR.endColor = color;
    }
    IEnumerator CheckGoal(){
        yield return new WaitForSeconds(3);
        try{ goal = GameObject.FindGameObjectWithTag("Finish").transform.position + (Vector3.up * 0.5f); }
        catch{ goal = new Vector3(100,10,100); }
        StartCoroutine(CheckGoal());
    }
}
enum state{ Goal, Distraction, Idle, Explore }