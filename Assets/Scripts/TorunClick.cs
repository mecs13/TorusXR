using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Torun))]
public class TorunClick : MonoBehaviour {

    Cylinder parentCylinder;

    Torun torun;

    VRTK.VRTK_InteractableObject io;

    public UnityEvent onClick;

    // Use this for initialization
    void Start () {
        if (!HasParentCylinder())
        {
            Debug.LogError("No parent cylinder in " + this.name);
            return;
        }

        torun = GetComponent<Torun>();

        io = GetComponent<VRTK.VRTK_InteractableObject>();

        if (!io)
        {
            io = gameObject.AddComponent<VRTK.VRTK_InteractableObject>();
        }
        io.isUsable = true;
        io.enabled = true;

        io.InteractableObjectUsed += new VRTK.InteractableObjectEventHandler(Click);
    }

    void OnMouseDown()
    {
        //Debug.Log("OnMouseDown() called on " + this.name );
        Click();
    }

    public void Click(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Click();
    }

    public void Click()
    {
        Debug.Log("Click() on " + this.name);
        torun.SetPressedColor(0.5f);
        onClick.Invoke();
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

}
