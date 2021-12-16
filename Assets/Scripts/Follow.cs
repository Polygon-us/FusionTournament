using Attributes;
using UnityEngine;

public class Follow : MonoBehaviour
{
    #region Information

    [Foldout("Information")]
    [SerializeField] private Transform target;
    [Foldout("Information")]
    [SerializeField] private Vector3 offset;

    #region Components
    new Transform transform;
    #endregion

    #endregion

    void Awake()
    {
        transform = GetComponent<Transform>();

        if (target != null)
            offset = transform.position - target.position;
    }

    void Update()
    {
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position + offset) < 0.5f)
                transform.position = Vector3.MoveTowards(transform.position, target.position + offset, 1f);
            else
                transform.position = target.position + offset;
        }
    }

    public void SetTarget(Transform target, Vector3 offset)
    {
        this.offset = offset;

        this.target = target;
    }
}
