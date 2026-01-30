using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private MonsterSpawner _spawner;
    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            _hasTriggered = true;

            if (_spawner != null)
            {
                _spawner.ActivateSpawner();
            }

             gameObject.SetActive(false);
        }
    }

}
