using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Combats
{
    public class DealDamageOnContact : MonoBehaviour
    {
        [SerializeField] int _damage = 5;

        ulong _ownerID; 

        public void SetOwner(ulong ownerId)
        {
            _ownerID = ownerId;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            //if(!other.attachedRigidbody) return;

            if (other.TryGetComponent(out NetworkObject networkObject))
            {
                if (networkObject.OwnerClientId == _ownerID) return;
            }
            
            if (other.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
            }
        }
    }
}