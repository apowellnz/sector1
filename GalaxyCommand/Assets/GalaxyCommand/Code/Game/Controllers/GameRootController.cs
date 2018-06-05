using UnityEngine;
using UnityEngine.Networking;

public class GameRootController : NetworkBehaviour
{
    [SerializeField] public GameObject Grid;

    // Use this for initialization
    private void Start()
    {
        Grid.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}