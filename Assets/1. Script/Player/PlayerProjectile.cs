using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerProjectile : MonoBehaviourPun
{
    private float _damage;
    [SerializeField] float _speed = 15f;
    [SerializeField] float _lifeTime = 3f;

    private int _layer = -1;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _layer = LayerMask.NameToLayer("Obstacle");
    }
    public void Init(float damage)
    {
        _damage = damage;
    }
    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(DestroyAfterTime(3f));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.TryGetComponent(out IMobDamaged target))
        {
            target.OnMobDamaged(_damage);
            PhotonNetwork.Destroy(gameObject);
        }
        if (other.gameObject.layer == _layer) //장애물 (나무, 돌 같은거)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    IEnumerator DestroyAfterTime(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);

        if (gameObject != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Vector3 nextPos = transform.position + transform.forward * _speed * Time.fixedDeltaTime;

        _rb.MovePosition(nextPos);
    }
}
