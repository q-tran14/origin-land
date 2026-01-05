using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack")]
    public float attackRange = 1.5f;
    public LayerMask interactLayer;

    [Header("Combo")]
    public float comboResetTime = 0.8f;

    private int comboStep = 0;
    private float comboTimer;

    private Animator animator;
    private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        // reset combo nếu quá thời gian
        if (comboStep > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboResetTime)
            {
                ResetCombo();
            }
        }
    }

    // Gọi khi người chơi click / attack
    public void Attack()
    {
        if (player.IsBusy()) return;

        comboStep++;
        if (comboStep > 3)
            comboStep = 1;

        comboTimer = 0f;

        animator.SetInteger("ComboStep", comboStep);
        animator.SetTrigger("Attack");

        player.SetBusy();
    }

    // Animation Event (GIỮ NGUYÊN CỦA BẠN)
    public void OnAttackHit()
    {
        Debug.Log("🔥 OnAttackHit CALLED - Combo " + comboStep);

        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, interactLayer))
        {
            if (hit.collider.TryGetComponent(out ChopableTree tree))
                tree.OnHit();
        }
    }

    // Animation Event ở CUỐI animation
    public void OnAttackEnd()
    {
        player.ClearAction();
    }

    private void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0f;
        animator.SetInteger("ComboStep", 0);
    }
}
