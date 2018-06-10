using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.Game.Controllers;
using Assets.GalaxyCommand.Code.Game.Services;
using com.t7t.utilities;
using UnityEngine;
using UnityEngine.AI;

namespace com.t7t.formation
{
    /**
     * This is the key class which implements the FormationGrid setup and movement (=following ths FormationAnchor).
     * It uses an anchor, not a leader.
     * 
     * The FormationStates show the 4 states a formation can have:
     *      Form    = move all assigned units to their grid points
     *      Move    = move the formation towards the (FormationAnchor) target
     *      Arrive  = the formation arrives at the target and stops moving
     *      Disband =
     * 
     * The FormationGridPoints are stored in a list and follow the FormationAnchor. The units assigned to each FormatiomGridPoint follow their FormationGridPoint.
     * The movement is based on moving the CharacterController so it takes colliders into account and always lags a little behind the grid points themselves.
     * 
     * The GridType is the form the formation takes. The options are:
     *      Predefined formations   = box, line, wedge, etc..
     *      Function                =
     *      Custom                  =
     * 
     * Key properties: state, anchor, gridtype, gridpoints, visualizegrid
     * 
     * Key Methods: ChangeState, CalculateGrid, Update, SetAnchor, SetupGrid, AssignObjectsToGrid
     * 
     * 
     */


    // TODO LIST:
    //              Does anchor need rigidbody? Probably not.

    public enum GridTypes
    {
        None,
        Box9,
        RightFlank5,
        LeftFlank5,
        Wedge9,
        Line8,
        StaggeredLine8,
        Column10,
        Custom,
        Function,
        DynamicLine
    }
    // Custom = load from file
    // Function is function based on id

    /* Formation states, see above */
    public enum FormationStates
    {
        Form,
        Move,
        Arrive,
        Disband
    }


    public class FormationGrid : MonoBehaviour
    {
        /* When a unit straggles too far behind it grid point use this acceleration multiplier*/
        public float accelerationStraggler = 1.2f;

        [Header("Anchor")]
        /* Link the FormationAnchor here */ [SerializeField] protected GameObject anchor;

        protected AudioSource audioSource;

        [Header("Disband")] [SerializeField] protected float disbandDuration = 3.0F;

        protected bool disbanded;
        [SerializeField] protected float disbandRadius = 3.0F;
        protected float disbandTimer;


        /* The GameObject to which this FormationGrid script is attached to is cached so we can parent the FormationGrid instances to it easily, see SetupGrid*/
        protected GameObject formation;

        /* The cached FormationAnchor script which we get from the anchor GameObject in the Awake() method */
        protected FormationAnchor formationAnchor;

        [SerializeField] protected float gravity = 9.8F;

        /* List of FormationGridPoints which make up the formation */
        [SerializeField] protected List<FormationGridPoint> gridPoints;

        /* Scale factor of the grid used in CalculatePositionsAllGridPoints to enlarged the formation uniformly*/
        [SerializeField] protected float gridScale = 2.0F;


        [Header("Grid")]
        /* Grid types detrmining what the formation looks like */ [SerializeField]
        protected GridTypes gridType = GridTypes.None;

        [Header("Sound")]
        /* If true then activate the SoundSource on the grid at certain states*/ [SerializeField]
        protected bool hasSound = true;

        public LayerMask mask;
        [SerializeField] protected float maximumAcceleration = 10.0f;

        [SerializeField]
        protected float maximumVelocity = 4.0F; // Make this a bit higher than as NavMesh speed (=maximum speed)

        public int MaxRowCount = 6;

        protected Vector3 oldPosition;
        protected float oldRotation;


        protected bool positionDirty;

        [SerializeField] protected float randomizeOffset = 0.2F
            ; // If this value >0 then the FormationGridPoint offsets are randomized a little with this max distance

        protected float reRandomizeNextTime;

        protected float reRandomizeOffsets;
        [SerializeField] protected float reRandomizeTimeMax = 4.0F; // Maximum time before reRandomizing the offsets
        [SerializeField] protected float reRandomizeTimeMin = 2.0F; // Minimum time before reRandomizing the offsets
        protected bool rotationDirty;

        /* Smoothen the rotation in Update() Quaternion.Slerp*/
        public float smoothRotation = 2.0F; // smooth rotation

        [SerializeField] protected FormationStates state = FormationStates.Form;
        [SerializeField] protected bool useGravity = true;

        /* If true then the grid is shown by means of colored spheres, see FormationGridPoint*/
        [SerializeField] protected bool visualizeGrid = true;


        private void Awake()
        {
            var toolbox = Toolbox.Instance;
            toolbox.allFormations.Add(this);

            if (anchor == null)
            {
                // TODO: alternatively create the anchor here (from prefab?)
                Debug.LogError("FormationGrid.Awake(): anchor not assigned");
                return;
            }
            // Move anchor align with FormationGrid (this) position
            anchor.transform.position = transform.position;

            // Cache the FormationAnchor component from the anchor
            formationAnchor = anchor.GetComponent<FormationAnchor>();
            if (formationAnchor == null)
            {
                Debug.LogError("FormationGrid.Awake(): FormationAnchor component missing on anchor");
                return;
            }
            // Assign a reference to this FormationGrid in the FormationAnchor
            formationAnchor.SetFormation(this);

            if (hasSound)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    Debug.LogError("FormationGrid.Awake(): AudioSource component missing on Formation Grid");
                    return;
                }
            }

            // Assign a value (minimum re randomization time difference) to the next re randomization time difference.
            // This time difference is changed everytime a re randomization of the grid points take place to prevent a pattern in the random behaviour. 
            reRandomizeNextTime = reRandomizeTimeMin;

            // Cache the object this script is attached to
            formation = transform.gameObject;

            // Setup the grid based on the selected property
            CalculateGrid(gridType);

            // Change the state to the initial form state
            // ChangeState(FormationStates.Form);
        }


        public GridTypes GetGridType()
        {
            return gridType;
        }

        // This method can be used to attach to the FormationAnchor's TargetReached event so we can change the state on parth arrival
        // Currently the Anchor triggers a change to FormationState (to Arrive) when the agent (Navmesh) or seeker (A*) arrives at the target.
        public void AnchorArrived()
        {
            ChangeState(FormationStates.Arrive);
        }

        // Retrieve the current state of the FormationGrid
        public FormationStates GetState()
        {
            return state;
        }


        // Change the state of the FormationGrid and perform associated actions
        public void ChangeState(FormationStates newstate)
        {
            switch (newstate)
            {
                case FormationStates.Form:
                    Debug.Log("Changing state to Form");

                    // Formation remains to be static in Form state

#if T7T_ASTAR
                    formationAnchor.canMove = false;
                    formationAnchor.canSearch = false;
#else
                    formationAnchor.StopMove();
#endif

                    // Allow all grid assigned units to search and move towards the grid points:
                    ChangeMoveStateOnGridObjects(true);
                    ChangeAnimationStateOnGridObjects(true);
                    state = newstate;

                    if (audioSource)
                        audioSource.mute = false;
                    break;
                case FormationStates.Move:
                    Debug.Log("Changing state to Move");

                    // Formation starts search and move in Move state

#if T7T_ASTAR
                    formationAnchor.canSearch = true;
                    formationAnchor.canMove = true;
#else
                    formationAnchor.StartMove();
#endif
                    // All grid assigned units stop search and move because formation move needs to take over:
                    ChangeMoveStateOnGridObjects(false);
                    ChangeAnimationStateOnGridObjects(true);
                    state = newstate;

                    if (audioSource)
                        audioSource.mute = false;
                    break;

                case FormationStates.Arrive:
                    Debug.Log("Changing state to Arrive");
                    // Formation stop search and move in Move state
#if T7T_ASTAR
                    formationAnchor.canMove = false;
                    formationAnchor.canSearch = false;
#else
                    formationAnchor.StopMove();
#endif
                    // All grid assigned units stop search and move because formation move needs to take over:
                    ChangeMoveStateOnGridObjects(false);
                    ChangeAnimationStateOnGridObjects(false);
                    state = newstate;

                    if (audioSource)
                        audioSource.mute = true;
                    break;
                case FormationStates.Disband:
                    Debug.Log("Changing state to Disband");
                    disbandTimer = 0.0F;

                    ChangeMoveStateOnGridObjects(false);
                    ChangeAnimationStateOnGridObjects(true);

                    state = newstate;
                    break;
                default:
                    if (audioSource)
                        audioSource.mute = true;
                    break;
            }
        }

        /* Change the grid to a new type:
         *      If the grid already has existing FormationGridPoints then
         *          Collect the assigned units
         *          Destroy the spheres in the FormationGridPoints to cleanup memory
         *      Clear the grid points list
         *      Setup the new grid points list
         *      Setup the new grid
         *          Assign the previously assigned units to the FormationGridPoints
         *      Calculate the new positions
         */

        public void CalculateGrid(GridTypes gridtype)
        {
            gridType = gridtype;

            // Create the list for collecting the already assigned units
            var units = new List<GameObject>();

            Debug.Log("FormationGrid.CalculateGrid(): change state to " + gridType);


            if (gridPoints != null)
            {
                if (gridPoints.Count > 0)
                {
                    // collect units assigned to gridpoint so we can reassign automatically after grid change

                    Debug.Log("FormationGrid.CalculateGrid(): grid exists so check if it has assigned units");

                    for (var i = 0; i < gridPoints.Count; i++)
                    {
                        var fgp = gridPoints[i];
                        var go = fgp.GetAssignedUnit();
                        if (go)
                        {
                            units.Add(go);
                            Debug.Log("FormationGrid.CalculateGrid(): found one unit " + go.name);
                        }
                    }

                    // destroy list items first: cleanup the spheres
                    for (var i = 0; i < gridPoints.Count; i++)
                    {
                        var fgp = gridPoints[i];
                        fgp.DestroySphere();
                    }
                }
                gridPoints.Clear();
            }

            // Create a new list to completely start a new grid from scratch.
            gridPoints = new List<FormationGridPoint>();
            if (gridPoints == null)
            {
                Debug.LogError("FormationGrid.CalculateGrid(): gridPoints not initialized");
                return;
            }

            // Setup the new grid for the new gridtype
            var result = SetupGrid(gridtype);

            // Now add the units we has assigned to the previous grid
            if (units.Count > 0)
                AssignObjectsToGrid(units);

            // Calculate the real positions of the grid based on the offsets in the grid definition
            CalculatePositionsAllGridPoints();

            Debug.Log("FormationGrid.CalculateGrid(): result SetupGrid()=" + result);
        }


        // Use this for initialization
        private void Start()
        {
            if (anchor == null) return;

            CalculatePositionsAllGridPoints();

            oldPosition = anchorPosition;
            oldRotation = anchorRotation.eulerAngles.y;
        }


        // Update is called once per frame
        private void Update()
        {
            if (anchor == null) return;

            if (state == FormationStates.Form)
                for (var i = 0; i < gridPoints.Count; i++)
                {
                    var fgp = gridPoints[i];
                    if (fgp != null)
                        if (fgp.IsUnitAssigned())
                        {
                            var formationUnitAnimation = fgp.GetFormationUnitAnimation();
                            if (formationUnitAnimation)
                            {
                                var go = fgp.GetAssignedUnit();
                                if (go)
                                {
                                    
                                    var nma = go.GetComponent<NavMeshAgent>();
                                    formationUnitAnimation.velocity = nma.velocity;

                                }
                            }
                        }
                }

            if (state == FormationStates.Move)
            {
                if ((oldPosition - anchorPosition).sqrMagnitude > 0.001f * 0.001f
                ) // TODO: potentially we can do this by checking if target has been reached
                    positionDirty = true;
                else
                    positionDirty = false;

                if (Mathf.Abs(anchorRotation.eulerAngles.y - oldRotation) > 0.01f)
                    rotationDirty = true;
                else
                    rotationDirty = false;

                var target = anchorRotation;
                if (rotationDirty)
                    transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smoothRotation);

                // Rotate the units at grid points to align with anchor rotation:
                // TODO: can we check when not to run this by means of "fully rotated" units?
                if (formationAnchor != null)
                    for (var i = 0; i < gridPoints.Count; i++)
                    {
                        var fgp = gridPoints[i];
                        if (fgp != null)
                            if (fgp.IsUnitAssigned())
                            {
                                var au = fgp.GetAssignedUnit();

                                au.transform.rotation = Quaternion.Slerp(au.transform.rotation, target,
                                    Time.deltaTime * smoothRotation);
                            }
                    }


                if (positionDirty)
                {
                    transform.position =
                        anchorPosition; // Move the Formation to Anchor position. TODO: If dampening needed, do Lerp here.

                    if (randomizeOffset > 0.0F)
                    {
                        // Randomize the positions slightly if enabled

                        reRandomizeOffsets += Time.deltaTime;

                        if (reRandomizeOffsets > reRandomizeNextTime) // ReRandomize the gridpoints every 3 seconds
                        {
                            reRandomizeOffsets = 0.0F;
                            reRandomizeNextTime = reRandomizeTimeMin +
                                                  (reRandomizeTimeMax - reRandomizeTimeMin) * Random.value;

                            for (var i = 0; i < gridPoints.Count; i++)
                            {
                                var fgp = gridPoints[i];
                                fgp.RandomizePosition();
                            }
                        }
                    }

                    CalculatePositionsAllGridPoints(); // Calculate all Grid Points relative to the Anchor which has moved by means of A*Pathfinfing.
                }


                // Now move the units (assigned to grid positions) towards their grid position:

                // TODO: Add a check here to stop if all units have arrived.

                if (formationAnchor != null)
                    for (var i = 0; i < gridPoints.Count; i++)
                    {
                        var fgp = gridPoints[i];
                        if (fgp != null)
                            if (fgp.IsUnitAssigned())
                            {
                                var formationUnitAnimation = fgp.GetFormationUnitAnimation();
                                if (formationUnitAnimation)
                                    formationUnitAnimation.velocity = fgp.GetAssignedVelocity();
                            }

                        oldPosition = anchorPosition;
                        oldRotation = transform.rotation.eulerAngles.y;
                    }
            }

            if (state == FormationStates.Disband)
            {
                if (disbandTimer == 0.0f)
                {
                    // set the directions for each assigned unit
                    for (var i = 0; i < gridPoints.Count; i++)
                    {
                        var fgp = gridPoints[i];
                        if (fgp != null)
                            if (fgp.IsUnitAssigned())
                            {
                                fgp.SetDisbandDesitination(disbandRadius, mask);
                                fgp.SetPositionToDisband(mask);
                            }
                    }
                    ChangeMoveStateOnGridObjects(true);
                    disbanded = false;
                }


                // start a timer, for x seconds have the assigned units move into a random direction
                disbandTimer += Time.deltaTime;
                if (disbandTimer < disbandDuration)
                {
                    // Move them

                    //DebugPanel.Log("disbandtimer", "disband", disbandTimer);
                }
                else
                {
                    if (!disbanded)
                    {
                        ChangeMoveStateOnGridObjects(false);
                        ChangeAnimationStateOnGridObjects(false);
                        disbanded = true;
                    }
                }
            }
        }

        // In this function we actually move each assigned unit of a grid point towards that moving (in most cases) grid point
        // We use the Move function of the CharacterController and the motion is calculated based on the distance from the unit to the grid point.
        public void MoveUnitsCharacterControllerMode(int gridPointIndex, FormationGridPoint fgp, Vector3 vlcity,
            float endReachedDistance)
        {
            var controller = fgp.GetCharacterController();
            if (!controller)
                Debug.LogError(
                    "FormationGrid.MoveUnitsCharacterControllerMode(): Character Controller missing on assigned unit.");

            var distanceToGridPoint = fgp.CalculateDistanceUnitToGridPoint();

            //if (gridPointIndex == 0) DebugPanel.Log("GridPoint [" + gridPointIndex + "] unittogrid", "Grid", distanceToGridPoint);

            // default acceleration multiplier
            var acceleration = 1.0F;
            if (distanceToGridPoint > endReachedDistance * 5)
            {
                // takeover
                acceleration = accelerationStraggler;
            }
            else if (distanceToGridPoint > endReachedDistance && distanceToGridPoint < endReachedDistance * 5)
            {
                // slowdown
                var slope = 1 / (4 * endReachedDistance); //      1 / ((5-1) * endReachedDistance) 
                var intercept = -1 * (slope * endReachedDistance); //  

                acceleration =
                    slope * distanceToGridPoint +
                    intercept; //      a = 0 at endReachedDistance and a = 1 at 5*endReachedDistance 

                //acceleration = distanceToGridPoint / 0.5F;
            }
            else if (distanceToGridPoint < endReachedDistance)
            {
                acceleration = 0.0f;
            }

            //if (gridPointIndex == 0) DebugPanel.Log("GridPoint [" + gridPointIndex + "] acceleration", "Grid", acceleration);

            var direction = fgp.GetPosition() - fgp.GetAssignedUnit().transform.position;
            var fgp_velocity = direction * acceleration;


            if (useGravity)
            {
                // Use gravity and calculate vertical velocity down
                var vSpeed = fgp.GetUnitVerticalSpeed();
                if (controller.isGrounded)
                    vSpeed = 0;
                vSpeed -= gravity * Time.deltaTime;
                fgp.SetUnitVerticalSpeed(vSpeed);
                fgp_velocity.y = vSpeed;
            }


            controller.Move(fgp_velocity * Time.deltaTime);

            fgp.SetAssignedVelocity(fgp_velocity);
        }

        // In this function we actually move each assigned unit of a grid point towards that moving (in most cases) grid point
        // We use the Rigidbody velocity and the velocity is calculated based on the distance from the unit to the grid point.
        public void MoveUnitsRigidBodyMode(int gridPointIndex, FormationGridPoint fgp, Vector3 vlcity,
            float endReachedDistance)
        {
            var rigidbody = fgp.GetRigidbody();
            if (!rigidbody)
                Debug.LogError("FormationGrid.MoveUnitsRigidBodyMode(): Rigidbody missing on assigned unit.");


            var acceleration = fgp.GetPosition() - fgp.GetAssignedUnit().transform.position;
            acceleration.y = 0;
            acceleration.Normalize();
            acceleration *= maximumAcceleration;

            //DebugPanel.Log("Acceleration "+gridPointIndex, "Rigidbody", acceleration.magnitude);

            rigidbody.velocity += acceleration * Time.deltaTime;

            if (rigidbody.velocity.magnitude > maximumVelocity)
                rigidbody.velocity = rigidbody.velocity.normalized * maximumVelocity;

            //DebugPanel.Log("Rgb velocity "+gridPointIndex, "Rigidbody", rigidbody.velocity.magnitude);

            fgp.SetAssignedVelocity(rigidbody.velocity);
        }

        public void SetAnchorTransform(Transform transform)
        {
            anchor.transform.position = transform.position;
            anchor.transform.rotation = transform.rotation;
            
        }
        public void SetAnchor(GameObject nchr)
        {
            anchor = nchr;
            anchor.transform.position = transform.position;
            

            CalculatePositionsAllGridPoints();

            oldPosition = anchorPosition;
            oldRotation = anchorRotation.eulerAngles.y;
        }

        public bool LoadCustomGrid(string filename)
        {
            if (gridType == GridTypes.Custom)
                return true;
            return false;
        }

        // Setup the grid based on static offsets contained in the function.
        // TODO: replace these static definitions by JSON files loaded from the project.
        public bool SetupGrid(GridTypes gridtype)
        {
            gridPoints.Clear();

            Vector2[] grid;
            switch (gridtype)
            {
                case GridTypes.DynamicLine:

                    var customerGrid = new List<Vector2>();
                    var selectedUnits = GameUnitService.GetSelectedUnits();
                    var rows = selectedUnits.Count / MaxRowCount;
                    var start_y = 0f;
                    var start_x = Mathf.Round(MaxRowCount / 2) * -1;
                    for (var r = 0; r < rows; r++)
                    {
                        for (var j = 0; j < MaxRowCount; j++)
                        {
                            customerGrid.Add(new Vector2(start_x, start_y));
                            start_x += 1f;
                        }
                        start_y += -1f;
                    }

                    var remainingUnits = selectedUnits.Count - rows * MaxRowCount;
                    start_x = Mathf.Round(remainingUnits / 2) * -1;
                    for (var j = 0; j < remainingUnits; j++)
                    {
                        customerGrid.Add(new Vector2(start_x, start_y));
                        start_x += 1f;
                    }
                    grid = customerGrid.ToArray();
                    break;
                case GridTypes.Box9:
                    grid = new Vector2[9];
                    grid[0] = new Vector2(1.0f, 1.5f);
                    grid[1] = new Vector2(0.0f, 1.5f);
                    grid[2] = new Vector2(-1.0f, 1.5f);

                    grid[3] = new Vector2(1.0f, 0.5f);
                    grid[4] = new Vector2(0.0f, 0.5f);
                    grid[5] = new Vector2(-1.0f, 0.5f);

                    grid[6] = new Vector2(1.0f, -0.5f);
                    grid[7] = new Vector2(0.0f, -0.5f);
                    grid[8] = new Vector2(-1.0f, -0.5f);
                    break;

                case GridTypes.Wedge9:
                    grid = new Vector2[9];
                    grid[0] = new Vector2(0.0f, 2.0f);
                    grid[1] = new Vector2(0.5f, 1.0f);
                    grid[2] = new Vector2(-0.5f, 1.0f);

                    grid[3] = new Vector2(1.0f, 0.0f);
                    grid[4] = new Vector2(-1.0f, 0.0f);

                    grid[5] = new Vector2(1.5f, -1.0f);
                    grid[6] = new Vector2(-1.5f, -1.0f);

                    grid[7] = new Vector2(2.0f, -2.0f);
                    grid[8] = new Vector2(-2.0f, -2.0f);
                    break;

                case GridTypes.Column10:
                    grid = new Vector2[10];
                    grid[0] = new Vector2(0.0f, -0.5f);
                    grid[1] = new Vector2(0.0f, -1.5f);
                    grid[2] = new Vector2(0.0f, -2.5f);
                    grid[3] = new Vector2(0.0f, -3.5f);
                    grid[4] = new Vector2(0.0f, -4.5f);
                    grid[5] = new Vector2(0.0f, -5.5f);
                    grid[6] = new Vector2(0.0f, -6.5f);
                    grid[7] = new Vector2(0.0f, -7.5f);
                    grid[8] = new Vector2(0.0f, -8.5f);
                    grid[9] = new Vector2(0.0f, -9.5f);
                    break;

                default:
                    grid = new Vector2[1];
                    grid[0] = new Vector2(0.0f, 0.0f);
                    Debug.LogError("FormationGrid.SetupGrid(): no grid type selected");
                    break;
            }

            if (!formation)
                formation = transform.gameObject;


            for (var i = 0; i < grid.GetLength(0); i++)
            {
                var fgp = new FormationGridPoint(i, formation, randomizeOffset); // DKE: fixed missing id number.
                fgp.offsetX = grid[i].x;
                fgp.offsetZ = grid[i].y;
                gridPoints.Add(fgp);
            }

            return true;
        }

        // Calculate grid positions in the real world taking the grid/anchor position and rotation into account
        public void CalculatePositionsAllGridPoints()
        {
            if (gridPoints == null) return;

            var rotationY = transform.rotation.eulerAngles.y;
            var position = transform.position;

            for (var i = 0; i < gridPoints.Count; i++)
            {
                var fgp = gridPoints[i];

                fgp.SetPosition(position, rotationY, gridScale, mask);

                fgp.VisualizeGridPoint(visualizeGrid);
            }
        }

        // Change the movement state of the units assigned to a grid point:
        // False = stop moving
        // True = start moving
        public void ChangeMoveStateOnGridObjects(bool state)
        {
            for (var i = 0; i < gridPoints.Count; i++)
            {
                var fgp = gridPoints[i];
                var go = fgp.GetAssignedUnit();

                if (go)
                {
#if T7T_ASTAR
                    AIPath aip = go.GetComponent<AIPath>();
                    if (aip)
                    {
                        aip.target = fgp.GetTransform();
                        aip.canSearch = state;
                        aip.canMove = state;
                    }
                    else
                    {
                        Debug.LogError("FormationGrid.EnableMoveOnGridObjects(): no assigned unit found for gridpoint.");
                    }
#else
                    var nma = go.GetComponent<NavMeshAgent>();
                    if (nma)
                        if (state)
                        {
                            nma.destination = fgp.GetPosition();
                            nma.Resume();
                        }
                        else
                        {
                            nma.Stop();
                            var rigidbody = fgp.GetRigidbody();
                            if (rigidbody)
                                rigidbody.velocity = Vector3.zero;
                        }
                    else
                        Debug.LogError(
                            "FormationGrid.EnableMoveOnGridObjects(): no nav mesh agent found for assigned unit.");
#endif
                }
            }
        }


        // Change the animation state of each unit assigned to a grid point:
        // False = go to Idle state
        // True = go to movement state
        public void ChangeAnimationStateOnGridObjects(bool state)
        {
            for (var i = 0; i < gridPoints.Count; i++)
            {
                var fgp = gridPoints[i];
                var go = fgp.GetAssignedUnit();

                if (go)
                {
                    var formationUnitAnimation = fgp.GetFormationUnitAnimation();
                    if (formationUnitAnimation)
                        if (state)
                            formationUnitAnimation.StartAnimations();
                        else
                            formationUnitAnimation.StopAnimations();
                }
            }
        }


        // Assign the objects in a list to the FormationGridPoint(s) in the gridPoints list
        public bool AssignObjectsToGrid(List<GameObject> list)
        {
            var result = true;
            gridPoints.Clear();
            transform.LookAt(list.First().transform);
            CalculateGrid(GridTypes.DynamicLine);
            for (var i = 0; i < list.Count; i++)
                if (i < gridPoints.Count)
                {
                    var go = list[i];
                    var gu = go.GetComponent<GameUnitController>();
                    var nma = go.GetComponent<NavMeshAgent>();
                    if (nma)
                    {
                        var fgp = gridPoints[i];
                        fgp.AssignUnit(go);

                        gu.MovePosition(fgp.GetPosition());

                        Debug.Log("FormationGrid.AssignObjectsToGrid(): Assigned new target to object " +
                                  go.transform.name);
                    }
                    else
                    {
                        Debug.LogWarning("FormationGrid.AssignObjectsToGrid(): Assigned Object [" + go.transform.name +
                                         "] has no Navmesh component.");
                        result = false;
                    }
                }

            return result;
        }

        /* A series of getters which return anchor properties */

        #region anchorgetters

        [SerializeField]
        protected Transform anchorTransform
        {
            get { return anchor.transform; }
        }

        [SerializeField]
        protected Vector3 anchorPosition
        {
            get { return anchor.transform.position; }
        }

        [SerializeField]
        protected Quaternion anchorRotation
        {
            get { return anchor.transform.rotation; }
        }

        #endregion
    }
}