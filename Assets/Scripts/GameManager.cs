using Attributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Information

    [Foldout("Information")]
    [SerializeField] Transform players;
    public Transform _players
    {
        get => players;
        set => players = value;
    }
    [Foldout("Information")]
    [SerializeField] Transform spawnPoints;
    public Transform _spawnPoints
    {
        get => spawnPoints;
    }

    #endregion
}
