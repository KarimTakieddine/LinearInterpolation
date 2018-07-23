using UnityEngine;

public enum MovementState
{
    FORWARD     = 0,
    BACKWARD    = 1
};

public class LineInterpolator : MonoBehaviour
{
    /// <summary>
    /// The list of LineSegments assigned to the LinearInterpolator.
    /// </summary>
    
    public LineSegment[] Lines;

    /// <summary>
    /// The current LineSegment used for interpolation.
    /// </summary>

    public LineSegment CurrentLine  { get; private set; }

    /// <summary>
    /// The current starting point of the LineInterpolator.
    /// </summary>
    
    public Vector2 A { get; private set; }

    /// <summary>
    /// The current target point of the LineInterpolator.
    /// </summary>
    
    public Vector2 B { get; private set; }

    /// <summary>
    /// The time-dependent distance interpolation unknown or variable.
    /// </summary>
    
    public float Timer { get; private set; }

    /// <summary>
    /// Inspector-exposed iterpolation variable step-factor,
    /// or speed of temporal evolution.
    /// </summary>

    [Range(0.0f, Mathf.Infinity)]
    public float StepFactor;

    /// <summary>
    /// Index of the current LineSegment used for interpolation.
    /// </summary>

    public int LineIndex { get; private set; }

    /// <summary>
    /// The LineInterpolator's current movement state (forwards or backwards). 
    /// </summary>
    
    public MovementState State { get; private set; }

    /// <summary>
    /// The interpolation coefficient i.e. the time-dependent
    /// distance traversed, as a fraction of the total length
    /// between start and target.
    /// </summary>

    public float InterpolationCoefficient { get; private set; }

    /// <summary>
    /// Cached value indicating whether line segments are assigned
    /// to this GameObject.
    /// </summary>
    
    public bool HasLines { get; private set; }

    /// <summary>
    /// Inspector-exposed line drawing toggle.
    /// </summary>
    
    public bool DrawLineSegments;

    void Awake()
    {
        HasLines = false;

        /*
         * Prevent any execution of logic beyond this point
         * if no lines are found.
         */
        
        if (Lines.Length == 0)
        {
            return;
        }

        /*
         * Set the current LineSegment to the first element
         * in the list.
         */
        
        CurrentLine = Lines[0];
        
        // Starting point is the LineSegment's first point.
        
        A = CurrentLine.A;

        // Target point is the LineSegment's second point.

        B = CurrentLine.B;

        // Interpolation coefficient is set to 0.

        InterpolationCoefficient = 0.0f;

        // Current index is that of the first LineSegment element.

        LineIndex = 0;

        // Initial movement state is set to FORWARD.

        State = MovementState.FORWARD;

        // LineSegment count successfully validated.

        HasLines = true;
    }

	void Update ()
    {
        /*
         * Prevent any execution of logic beyond this point
         * if no lines are found.
         */
        
        if (!HasLines)
        {
            return;
        }

        int lineSegmentCount = Lines.Length;

        // Optionally draw assigned LineSegment instances.

        if (DrawLineSegments)
        {
            for (int i = 0; i < lineSegmentCount; ++i)
            {
                LineSegment lineSegment = Lines[i];

                Debug.DrawLine(lineSegment.A, lineSegment.B, Color.white);
            }
        }

        // Step interpolation variable.

        Timer += Time.deltaTime;

        if (InterpolationCoefficient >= 1.0f)
        {
            /*
             * The linear interpolation traversal from
             * start to target is complete.
             */
            
            if (State == MovementState.FORWARD)
            {
                if (LineIndex < lineSegmentCount - 1)
                {
                    /*
                     * Select the next LineSegment and re-assign starting
                     * and target points in a forward-facing manner.
                     */
                    
                    CurrentLine = Lines[++LineIndex];
                    A           = CurrentLine.A;
                    B           = CurrentLine.B;
                }
                else
                {
                    /*
                     * The direction must now change. The last LineSegment
                     * in the list is selected. Sarting and target points
                     * are re-assigned in a backward-facing manner.
                     */
                     
                    LineIndex   = lineSegmentCount - 1;
                    CurrentLine = Lines[LineIndex];
                    A           = CurrentLine.B;
                    B           = CurrentLine.A;

                    // The LineInterpolator will be moving backwards on the next Update() iteration.

                    State = MovementState.BACKWARD;
                }
            }
            else if (State == MovementState.BACKWARD)
            {
                if (LineIndex > 0)
                {
                    /*
                     * Select the previous LineSegment and re-assign starting
                     * and target points in a backward-facing manner.
                     */
                    
                    CurrentLine = Lines[--LineIndex];
                    A           = CurrentLine.B;
                    B           = CurrentLine.A;
                }
                else
                {
                    /*
                     * The direction must now change. The first LineSegment
                     * in the list is selected. Sarting and target points
                     * are re-assigned in a forward-facing manner.
                     */

                    LineIndex   = 0;
                    CurrentLine = Lines[LineIndex];
                    A           = CurrentLine.A;
                    B           = CurrentLine.B;

                    // The LineInterpolator will be moving forward on the next Update() iteration.

                    State = MovementState.FORWARD;
                }
            }

            Timer = 0.0f;
        }

        // Step the interpolation variable or unknown.

        float timeSteppedVariable = Timer * StepFactor;
        
        /*
         * The horizontal position of the interpolation variable
         * or unknown.
         */

        float x = A.x + (State == MovementState.BACKWARD ? -timeSteppedVariable : timeSteppedVariable);

        /*
         * The horizontal distance between A and B,
         * taking direction into account.
         */

        float hDistanceAtoB = (B.x - A.x);

        /* 
         * Calculates the fraction of the total horizontal length
         * traversed by the LinearInterpolator.
         * 
         * Divide-by-zero protection is enforced.
         */

        InterpolationCoefficient = hDistanceAtoB == 0 ? 0 : (x - A.x) / hDistanceAtoB;

        /*
         * The position of the LineInterpolator Transform is obtained
         * from equation of the slope of the LineSegment.
         * 
         * https://en.wikipedia.org/wiki/Linear_interpolation
         */

        transform.position = new Vector3(
            x,
            A.y + ( InterpolationCoefficient * (B.y - A.y) )
        );
    }
};