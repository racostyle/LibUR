using UnityEngine;

public class Timeout_Kill : MonoBehaviour
{
    private int _counter;
    [Range(100, 500)]
    [SerializeField] int TIMEOUT = 400;

    void OnEnable()
    {
        _counter = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_counter == TIMEOUT)
            Destroy(gameObject);
        else
            _counter++;
    }
}
