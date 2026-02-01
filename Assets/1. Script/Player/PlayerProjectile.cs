using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerProjectile : MonoBehaviourPun
{
    private float _damage;
    [SerializeField] float _speed = 15f;
    [SerializeField] float _lifeTime = 3f;

    [SerializeField] GameObject _hitEffect;
    private int _layer = -1;

    private Rigidbody _rb;
    private Collider _col;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
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
            StartCoroutine(DestroyAfterTime(_lifeTime));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (other.TryGetComponent(out IMobDamaged target))
        {
            target.OnMobDamaged(_damage);
            photonView.RPC("CreateHitEffect", RpcTarget.All, transform.position);
            StartCoroutine(DelayedDestroy());
        }
        if (other.gameObject.layer == _layer) //장애물 (나무, 돌 같은거)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    [PunRPC]
    private void CreateHitEffect(Vector3 pos)
    {
        if (_hitEffect != null)
        {
            GameObject effect = Instantiate(_hitEffect, pos, Quaternion.identity);

            Destroy(effect, 1.5f);
        }
    }
    IEnumerator DelayedDestroy()
    {
        _col.enabled = false;

        if (TryGetComponent(out ParticleSystem ps))
        {
            ps.Stop();           
            ps.Clear(); 
        }
        yield return new WaitForSeconds(0.1f);

        if (photonView.IsMine)
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
