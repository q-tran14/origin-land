public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    private Player player;

    private int comboStep = 0;
    private float comboTimer;
    public float comboResetTime = 1f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        player = GetComponent<Player>();
    }

    void Update()
    {
        comboTimer += Time.deltaTime;
        if (comboTimer > comboResetTime)
            comboStep = 0;
    }

    public void Attack()
    {
        if (player.IsBusy()) return;

        comboStep++;
        if (comboStep > 3)
            comboStep = 1;

        comboTimer = 0f;

        Debug.Log("⚔️ Combo Step = " + comboStep);

        animator.SetInteger("ComboStep", comboStep);
        animator.SetTrigger("Attack");

        player.SetBusy();
    }

    public void OnAttackHit()
    {
        Debug.Log("🔥 OnAttackHit CALLED - Combo " + comboStep);
        player.ClearAction();
    }
}
