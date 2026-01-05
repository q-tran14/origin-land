// using UnityEngine;

// public class MineableRock : MonoBehaviour, IInteractable
// {
//     [Header("Rock Stats")]
//     public int maxHp = 5;
//     private int currentHp;

//     [Header("Drop")]
//     public GameObject stonePrefab;

//     private void Awake()
//     {
//         currentHp = maxHp;
//     }

//     /// <summary>
//     /// Player nhấn Interact → chỉ chuyển sang Attack
//     /// </summary>
//     public void Interact(Player player)
//     {
//         if (player == null) return;

//         player.Movement.SwitchState(player.Movement.attackState);
//     }

//     /// <summary>
//     /// GỌI TỪ Animation Event (OnAttackHit)
//     /// </summary>
//     public void TakeDamage(int damage = 1)
//     {
//         currentHp -= damage;

//         if (currentHp <= 0)
//         {
//             DropStone();
//             Destroy(gameObject);
//         }
//     }

//     private void DropStone()
//     {
//         Instantiate(stonePrefab, transform.position, Quaternion.identity);
//     }

//     public string GetInteractText()
//     {
//         return "Mine rock";
//     }
// }
