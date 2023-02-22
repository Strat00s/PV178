//create a sphere above targets (mainly for my own sanity)

using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private float _size = 1.0f;
    [SerializeField] private float _offset = 4.0f;
    [SerializeField] private Color _color = Color.yellow;

    private GameObject highlightSphere;

    void Awake()
    {
        //Create new sphere and set its color, size and position above
        highlightSphere                                         = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        highlightSphere.transform.parent                        = transform;
        highlightSphere.transform.localScale                    = new Vector3(_size, _size, _size);
        highlightSphere.transform.localPosition                 = new Vector3(0, _offset, 0);
        highlightSphere.GetComponent<Renderer>().material.color = _color;

        highlightSphere.SetActive(false);   //disable the sphere by default
    }

    public void SelectObject()
    {
        highlightSphere.SetActive(true);    //enable the sphere
    }

    public void DeselectObject()
    {
        highlightSphere.SetActive(false);   //disable the sphere
    }
}
