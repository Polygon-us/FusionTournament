using Attributes;
using Fusion;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    #pragma warning disable CS0109

    #region Information

    [Title("Information")]
    [SerializeField] float speed = 5000f;

    #region Components
    [Title("Information/Components")]
    [SerializeField] new Transform transform;
    [SerializeField] new Rigidbody rigidbody;
    #endregion

    #endregion

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out GameInput gameInput))
        {
            float speed = this.speed * Runner.DeltaTime;

            if (gameInput.IsDown(GameInput.FORWARD))
                rigidbody.AddForce(Vector3.forward * speed);
            else if (gameInput.IsDown(GameInput.BACKWARD))
                rigidbody.AddForce(Vector3.back * speed);

            if (gameInput.IsDown(GameInput.LEFT))
                rigidbody.AddForce(Vector3.left * speed);
            else if (gameInput.IsDown(GameInput.RIGHT))
                rigidbody.AddForce(Vector3.right * speed);
        }

        if (transform.position.y <= -5f)
            GetComponentInParent<Player>().RestartPlayer();
    }
}
