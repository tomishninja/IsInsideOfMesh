using UnityEngine;

public class RandomPlacementWithinThisObject : MonoBehaviour
{
    /// <summary>
    /// If this is enabled every 60 frames the objects will update
    /// </summary>
    public bool testMode = true;

    /// <summary>
    /// Tells the system if it is currently processing or not
    /// </summary>
    public bool IsProcessing { get => isProcessing; }
    private bool isProcessing = false;

    /// <summary>
    /// The objects to place in the object
    /// </summary>
    public BoxCollider[] objectsToAppear;

    /// <summary>
    /// The amount of object to produce
    /// </summary>
    public int AmountOfObjectsToAppear = 0;

    /// <summary>
    /// 
    /// </summary>
    private int testCounter = 0;

    // this tells the system what frame it should be caring about
    private int index = 0;

    /// <summary>
    /// Optional condtion to check if a object is inside of this object
    /// </summary>
    public MeshCollider meshCollider;

    public LayerMask layers;

    public IsInsideOfObjectCheck isInsideOfObjectChecker;

    public Vector3 Min;
    public Vector3 Max;

    // unity triggers

    private void Update()
    {
        if (testMode)
        {
            if (testCounter++ == 60)
            {
                isProcessing = true;
                testCounter = 0;


                // this is just to test the output
                //OutputData data = new OutputData();
                //this.GetData(ref data);
                //Debug.Log(data);
            }


            if (isProcessing)
            {
                RandomPlacementOfObjects(Min, Max);
            }

            // increment the index while it is useful to
            index++;

            // once the processing is over
            if (index >= AmountOfObjectsToAppear)
            {
                isProcessing = false;
                index = 0;
            }
        }
        else
        {
            if (isProcessing)
            {
                RandomPlacementOfObjects(Min, Max);
            }

            // increment the index while it is useful to
            index++;
            
            // once the processing is over
            if (index >= AmountOfObjectsToAppear)
            {
                isProcessing = false;
            }
        }
    }
    // publicly accessable functions

    public void StartToFindNewPositions()
    {
        isProcessing = true;
        index = 0;
    }

    /*public void GetData(ref OutputData dataStore)
    {
        //dataStore.randomlyPlacedObjects = new GeometricShapeData[objectsToAppear.Length];

        for(int index = 0; index < objectsToAppear.Length; index++)
        {
            //dataStore.randomlyPlacedObjects[index].name = objectsToAppear[index].name;
            //dataStore.randomlyPlacedObjects[index].position = objectsToAppear[index].transform.localPosition;
            // An assumption was made that this object is the parent of the other objects
            //dataStore.randomlyPlacedObjects[index].distanceFromCenterOfParent = Vector3.Distance(objectsToAppear[index].transform.localPosition, this.gameObject.transform.localPosition); 
        }
    }

    public bool GetData(ref inputTask.GoalObject data)
    {
        // make sure the data is ready to grab
        if (!isProcessing)
        {
            // set the array to be the right length

            // inptut all of the data for all the nessarcary objects
            for (int index = 0; index < objectsToAppear.Length; index++)
            {
                // create a new guide object to put the data into
                
                // detemine the type of mesh needed

                // save the local pos

                // work out the distance they are from the center of the object

            }
            // this side worked so show it here
            return true;
        }

        // the operation failed notifiy the other side
        return false;
    }*/

    // Private Functions //

    /// <summary>
    /// Change the placement of objects from other classes and events
    /// </summary>
    /// <returns>
    /// True if the function activated and worked as expected
    /// False if it was already running this frame
    /// </returns>
    private bool ChangeThePlacementOfObjects()
    {
        if (isProcessing)
        {
            return false;
        }
        else
        {
            RandomPlacementOfObjects(Min, Max);
            return true;
        }
    }

    /// <summary>
    /// This will randomly assign a new position of the new objects 
    /// </summary>
    private void RandomPlacementOfObjects(Vector3 min, Vector3 max)
    {
        // used to hold the new position verible
        UnityEngine.Vector3 newPos;

        // this is my exception
        int exceptionCounter = 0;

        // just making sure you can't go over the amount of objects that appear
        if (objectsToAppear.Length > AmountOfObjectsToAppear) AmountOfObjectsToAppear = objectsToAppear.Length;


        // There is some repeting maths problems that to work stuff out so some of the answers for these are stored in this verible
        bool isNotAnValidPlacment = false;
        do
        {

            // Throw an exception if things there dosn't seem to be enough space
            if (exceptionCounter >= 1024)
            {
                isProcessing = false;
                throw new System.Exception("There is not enough space left to place the object");
            }

            float x = Random.Range(min.x, max.x * 100) / 100f - max.x / 2;
            float y = Random.Range(min.y, max.y * 100) / 100f - max.y / 2;
            float z = Random.Range(min.z, max.z * 100) / 100f - max.z / 2;

            // update their transform based on were the parent of this script is located
            newPos = this.gameObject.transform.position + new UnityEngine.Vector3(x, y, z);

            // determine if the overlap happened
            if (Physics.CheckBox(newPos, Multi(objectsToAppear[index].size, objectsToAppear[index].transform.lossyScale) / 2, Quaternion.identity, this.layers))
            {
                // if there is a collision restart the loop and update the counter.
                isNotAnValidPlacment = true;
                exceptionCounter++;
            }else if (this.meshCollider != null && !this.meshCollider.bounds.Contains(newPos))
            {
                isNotAnValidPlacment = true;
                exceptionCounter++;
            }
            else if (isInsideOfObjectChecker != null && !isInsideOfObjectChecker.IsInCollider(this.meshCollider, newPos))
            {
                isNotAnValidPlacment = true;
                exceptionCounter++;
            }
            else
            {
                isNotAnValidPlacment = false;
            }

            if (this.meshCollider != null && !this.meshCollider.bounds.Contains(newPos))
            {
                isNotAnValidPlacment = true;
                exceptionCounter++;
            }
            else if (isInsideOfObjectChecker != null && !isInsideOfObjectChecker.IsInCollider(this.meshCollider, newPos))
            {
                isNotAnValidPlacment = true;
                exceptionCounter++;
            }

        } while (isNotAnValidPlacment);

        // set the new position object
        Debug.Log(newPos);
        objectsToAppear[index].gameObject.transform.localPosition = newPos;
    }

    private UnityEngine.Vector3 Multi(UnityEngine.Vector3 left, UnityEngine.Vector3 right)
    {
        return new UnityEngine.Vector3(left.x * right.x, left.y * right.y, left.z * right.z);
    }

    private UnityEngine.Vector3 Abs(UnityEngine.Vector3 vec)
    {
        if (vec.x < 0) vec.x *= -1;
        if (vec.y < 0) vec.y *= -1;
        if (vec.z < 0) vec.z *= -1;
        return vec;
    }
}
