using UnityEngine;

[RequireComponent(typeof(PlayField))]
public class PlayFieldGizmos : MonoBehaviour
{
    private PlayField playField = default;
#if UNITY_EDITOR
    [SerializeField] private Vector3 PlayFieldBounds = Vector3.zero;

    private void OnDrawGizmos()
    {
        playField ??= GetComponent<PlayField>();
        
        Gizmos.color = Color.blue;
        // play field
        var col = GetComponent<BoxCollider>();
        var size = col ? col.size: PlayFieldBounds;
        Gizmos.DrawWireCube(Vector3.up * transform.position.y, size);
        
        Gizmos.color = Color.cyan;
        // player side
        col = playField.PlayerAnchor.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(playField.PlayerAnchor.position, size * playField.PlayerAnchor.lossyScale.x);
        
        col = playField.PlayerTopLane.GetComponent<BoxCollider>();
        size = col ? col.size : Vector3.one;
        Gizmos.DrawWireCube(playField.PlayerTopLane.Anchor.position, size * playField.PlayerTopLane.Anchor.lossyScale.x);
        
        col = playField.PlayerBottomLane.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(playField.PlayerBottomLane.Anchor.position, size * playField.PlayerBottomLane.Anchor.lossyScale.x);
        
        Gizmos.color = Color.magenta;
        // opponent side
        col = playField.OpponentAnchor.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(playField.OpponentAnchor.position, size * playField.OpponentAnchor.lossyScale.x);
        
        col = playField.OpponentTopLane.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(playField.OpponentTopLane.Anchor.position, size * playField.OpponentTopLane.Anchor.lossyScale.x);
        
        col = playField.OpponentBottomLane.GetComponent<BoxCollider>();
        size = col ? col.size: Vector3.one;
        Gizmos.DrawWireCube(playField.OpponentBottomLane.Anchor.position, size * playField.OpponentBottomLane.Anchor.lossyScale.x);
    }
#endif
}