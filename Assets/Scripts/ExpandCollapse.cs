using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Torun))]
public class ExpandCollapse : MonoBehaviour {

    Cylinder parentCylinder;

    Camera mainCamera;

    private Torun[] children;    

    float stackingOffset = 0.05f; // The spacing between collapsed toruns when they are stacked behind one another

    VRTK.VRTK_InteractableObject io;

    // TODO: Left, LeftThenUp, LeftThenDown
    public enum ExpandDirection
    {
        Up, Down, Right, RightThenUp, RightThenDown 
    }


    [SerializeField]
    ExpandDirection expandDirection;

    public enum Ordering
    {
        Alphabetical, ReverseAlphabetical
    }

    [SerializeField]
    Ordering ordering;

    Torun torun;

    bool isCollapsed = false;

    // Use this for initialization
    void Start () {
        if (!HasParentCylinder())
        {
            Debug.LogError("No parent cylinder in " + this.name);
            return;
        }

        torun = GetComponent<Torun>();

        InitializeChildren();

        LocateCamera();

        Collapse(0f);

        io = GetComponent<VRTK.VRTK_InteractableObject>();

        if (!io)
        {
            io = gameObject.AddComponent<VRTK.VRTK_InteractableObject>();
        }

        io.isUsable = true;

        io.InteractableObjectUsed += new VRTK.InteractableObjectEventHandler(Toggle);
    }

    public Torun[] getChildren()
    {
        return children;
    }

    void OnMouseDown()
    {
        //Debug.Log("OnMouseDown() called on " + this.name );
        Toggle(1.0f);
    }

    public void Toggle(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Toggle(1.0f);
    }

    public void Toggle(float duration) {
        torun.SetPressedColor(duration);
        if (isCollapsed)
            Expand(1.0f);
        else
            Collapse(1.0f);
    }

    public void Collapse(float duration)
    {
        float stackingDepth = 0f;
        float yPosition = 0f;
        foreach (Torun t in children)
        {
            if (!t.Equals(torun))    // Ignore my own torun
            {
                stackingDepth += stackingOffset;
                yPosition = torun.GetHeight() - t.GetHeight(); // Align tops
                t.SetTargetPosition(yPosition, 0, stackingDepth, duration);
                ExpandCollapse e = t.GetComponent<ExpandCollapse>();
                if(e)
                {
                    e.Collapse(duration);
                }
            }
        }

        isCollapsed = true;
    }


    public void CollapseSiblings(float duration)
    {
        ExpandCollapse parent = gameObject.transform.parent.GetComponent<ExpandCollapse>();

        if (!parent)
            return;

        foreach (Torun t in parent.getChildren())
        {
            //if (!t.Equals(me))    // Ignore the torun that asked for the collapse of its siblings
            //{
                ExpandCollapse c = t.GetComponent<ExpandCollapse>();
                if (c)
                {
                    c.Collapse(duration);
                }
            //}
        }
    }

    public void Expand(float duration)
    {
        float yPosOffset=0f, yDirection=0f, yAngleOffset=0f, yAngleDirection=0f;

        if(children.Length > 0) {
            switch (expandDirection)
            {
                case ExpandDirection.Up:
                    yPosOffset = torun.GetHeight();
                    yDirection = 1;
                    break;
                case ExpandDirection.Down:
                    yPosOffset = -children[0].GetHeight();
                    yDirection = -1;
                    break;
                case ExpandDirection.Right:
                    yPosOffset = torun.GetHeight() - children[0].GetHeight();   // Align tops
                    yAngleOffset = torun.GetAngleWidth();
                    yAngleDirection = 1f;
                    break;
                case ExpandDirection.RightThenUp:
                    yPosOffset = torun.GetHeight() - children[0].GetHeight();   // Align tops
                    yAngleOffset = torun.GetAngleWidth();
                    yDirection = 1f;
                    break;
                case ExpandDirection.RightThenDown:
                    yPosOffset = torun.GetHeight() - children[0].GetHeight();   // Align tops
                    yAngleOffset = torun.GetAngleWidth();
                    yDirection = -1f;
                    break;
            }

            // Collapse all my siblings (in case another menu is open at the same level of the hierarchy)
            CollapseSiblings(duration);

            // Expand all of my children   
            foreach (Torun t in children) 
            {
                t.SetTargetPosition(yPosOffset, yAngleOffset, 0f, duration);

                yPosOffset += yDirection * t.GetHeight();
                yAngleOffset += yAngleDirection * t.GetAngleWidth();
            }


        }
        isCollapsed = false;
    }



    // Returns true if parent cylinder is set
    bool HasParentCylinder()
    {
        if (transform.parent)
        {
            parentCylinder = GetComponentInParent<Cylinder>();
        }
        return parentCylinder != null;
    }

    public void SetParentCylinder(Cylinder newParent)
    {
        parentCylinder = newParent;
        transform.SetParent(newParent.transform);
    }

    void LocateCamera()
    {
        mainCamera = Camera.main;
        if (!mainCamera)
            mainCamera = FindObjectOfType<Camera>();
        if (!mainCamera)
            Debug.LogError("No camera found in ExpandCollapse.LocateCamera()");

    }

    public class GameObjectSorterAlphabetical : IComparer
    {
        // Calls CaseInsensitiveComparer.Compare on the game object's name
        int IComparer.Compare(System.Object x, System.Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((Torun)x).transform.name, ((Torun)y).transform.name));
        }

    }

    public class GameObjectSorterReverseAlphabetical : IComparer
    {
        // Calls CaseInsensitiveComparer.Compare on the game object's name
        int IComparer.Compare(System.Object x, System.Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((Torun)y).transform.name, ((Torun)x).transform.name));
        }
    }


    // Sorts the child toruns alphabetically into the "children" array.
    void InitializeChildren()
    {
        Torun[] childCandidates = GetComponentsInChildren<Torun>(false);

        int childCount = 0;
        for (int i = 0; i < childCandidates.Length; i++)
        {
            if (childCandidates[i].transform.parent == transform) // This is a direct child of my own transform
            {
                if (childCandidates[i] != torun)    // Ignore my own torun
                {
                    childCount++;
                }
            }

        }

        children = new Torun[childCount];

        childCount = 0;
        for (int i = 0; i < childCandidates.Length; i++)
        {
            if (childCandidates[i].transform.parent == transform) // This is a direct child of my own transform
            {
                if (childCandidates[i] != torun)    // Ignore my own torun
                {
                    children[childCount] = childCandidates[i];
                    childCount++;
                }
            }
        }

        // Finally, sort the child array
        IComparer myComparer = new GameObjectSorterAlphabetical(); ;

        //if(ordering == Ordering.Alphabetical)
        //    myComparer 
        if (ordering == Ordering.ReverseAlphabetical)
            myComparer = new GameObjectSorterReverseAlphabetical();

        System.Array.Sort(children, myComparer);

    }


}
