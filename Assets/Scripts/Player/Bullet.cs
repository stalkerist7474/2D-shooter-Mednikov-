using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _speed;


    private void Update()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != Player.LocalPlayer)
        {

            if (collision.gameObject.TryGetComponent(out Player player))
            {
                player.TakeDamage(_damage);

                Destroy(gameObject);
            }
        }
    }
}
