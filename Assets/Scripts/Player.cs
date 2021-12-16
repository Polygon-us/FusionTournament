using Attributes;
using Cinemachine;
using Fusion;
using System.Text;
using Tools.ServicesManager;
using UnityEngine;

public class Player : NetworkBehaviour
{
    #region Information

    [Foldout("Information/Components")]
    [SerializeField] TextMesh nameText;
    public string _nameTxt
    {
        get => nameText.text;
    }

    Vector3 initialPosition;

    #endregion

    public override void Spawned() 
    {
        GameManager gameManager = ServicesManager.instance.Get<GameManager>();

        if (gameManager._players == null)
            gameManager._players = new GameObject("Players [Generated]").transform;

        transform.SetParent(gameManager._players, true);

        nameText.text = transform.parent.childCount.ToString();

        if (Object.HasInputAuthority)
        {
            ServicesManager.instance.Get<CinemachineVirtualCamera>().Follow = transform.GetChild(0);

            initialPosition = transform.GetChild(1).position;

            ServicesManager.instance.Register(this);
        }
    }

    public void SendReliableMessageToServer(string message)
    {
        if(!Runner.IsServer)
            Runner.SendReliableDataFromClientToServer(Encoding.ASCII.GetBytes(message));
    }

    public void RestartPlayer()
    {
        transform.GetChild(1).position = initialPosition;

        GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
    }
}
