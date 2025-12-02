using LibUR.Instantiation;
using UnityEngine;

namespace TestScenesAssets.InstantiatorTesting
{
    public class TheCube : MonoBehaviour, IInitializable
    {
        InfoToPass _infoToPass;

        public void Initialize(params object[] args)
        {
            _infoToPass = (InfoToPass)args[0];
        }

        public void Terminate()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(_infoToPass.TextToDisplay);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
