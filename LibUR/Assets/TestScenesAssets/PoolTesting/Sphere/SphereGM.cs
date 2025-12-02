using LibUR.Pooling.Auxiliary;
using UnityEngine;

public class SphereGM : MonoBehaviour
{
    string InjectedOnCreation;
    string IsSetWhenEnabled;
    string ID;

    public SphereGM Script => this;
    public GameObject GameObject => gameObject;
    public Transform Transform => transform;

    void Awake()
    {
        ID = Random.Range(10000, 99999).ToString();
    }

    public void InitOnCreate(string something)
    {
        InjectedOnCreation = something;
        Debug.Log($"InitOnCreate Triggered in {ID}");
    }

    public void ReInit(string something)
    {
        IsSetWhenEnabled = something;
        Debug.Log($"Enable Triggered {ID}");
    }
}
