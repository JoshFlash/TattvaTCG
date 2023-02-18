using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    [field: SerializeField] public Transform PlayerAnchor { get; private set; } = default;
    [field: SerializeField] public Transform OpponentAnchor { get; private set; } = default;
    
    [field: SerializeField] public Lane PlayerTopLane { get; private set; } = default;
    [field: SerializeField] public Lane PlayerBottomLane { get; private set; } = default;

    [field: SerializeField] public Lane OpponentTopLane { get; private set; } = default;
    [field: SerializeField] public Lane OpponentBottomLane { get; private set; } = default;
    
    #if UNITY_EDITOR
    [SerializeField] private Vector3 PlayFieldBounds = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        // play field
        var col = GetComponent<BoxCollider>();
        var size = col ? col.size: PlayFieldBounds;
        Gizmos.DrawWireCube(Vector3.up * transform.position.y, size);
        
        Gizmos.color = Color.cyan;
        // player side
        col = PlayerAnchor.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(PlayerAnchor.position, size * PlayerAnchor.lossyScale.x);
        
        col = PlayerTopLane.GetComponent<BoxCollider>();
        size = col ? col.size : Vector3.one;
        Gizmos.DrawWireCube(PlayerTopLane.Anchor.position, size * PlayerTopLane.Anchor.lossyScale.x);
        
        col = PlayerBottomLane.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(PlayerBottomLane.Anchor.position, size * PlayerBottomLane.Anchor.lossyScale.x);
        
        Gizmos.color = Color.magenta;
        // opponent side
        col = OpponentAnchor.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(OpponentAnchor.position, size * OpponentAnchor.lossyScale.x);
        
        col = OpponentTopLane.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(OpponentTopLane.Anchor.position, size * OpponentTopLane.Anchor.lossyScale.x);
        
        col = OpponentBottomLane.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(OpponentBottomLane.Anchor.position, size * OpponentBottomLane.Anchor.lossyScale.x);
    }
    #endif
}

