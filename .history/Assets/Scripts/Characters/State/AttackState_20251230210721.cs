using UnityEngine;
using System.Collections;

public class AttackState : ICharacterState
{
    private int currentStep = 0;           // đòn hiện tại (1..maxCombo)
    private float comboResetTimer = 0f;    // thời gian từ lần click cuối
    private bool isAttacking = false;

    private readonly int maxCombo = 3;
    private readonly float comboResetTime = 1.2f;      // nếu không click thêm thì reset combo
    private readonly float idleDelayAfterAttack = 0.5f; // thời gian chờ trước khi về Idle

    public void EnterState(CharacterMovement c)
    {
         Debug.Log("Enter Attack State");

    currentStep = 0;
    comboResetTimer = 0f;
    isAttacking = false;

    c.animator.SetLayerWeight(1, 1f);
    }

    public void UpdateState(CharacterMovement c)
    {

        comboResetTimer += Time.deltaTime;

        // 🎯 Cho phép di chuyển trong khi đang tấn công
        HandleMovementWhileAttacking(c);

        // Nếu quá lâu không click → reset combo và về Idle
        if (comboResetTimer > comboResetTime)
        {
            currentStep = 0;
            comboResetTimer = 0f;
            EndAttack(c);
        }
    }

    public void ExitState(CharacterMovement c)
    {
        currentStep = 0;
        comboResetTimer = 0f;
        isAttacking = false;
        c.animator.ResetTrigger("Attack");

        // 🔹 Tắt layer attack (về lại chỉ Base Layer)
        c.animator.SetLayerWeight(1, 0f);
    }

    public void OnClick(CharacterMovement c)
    {
        comboResetTimer = 0f; // reset thời gian combo
        isAttacking = true;

        // Tăng step
        currentStep++;
        if (currentStep > maxCombo)
            currentStep = 1;

        // Gọi animation ngay lập tức
        c.animator.ResetTrigger("Attack");
        c.animator.SetInteger("AttackIndex", currentStep);
        c.animator.SetTrigger("Attack");

        // Hẹn về Idle nếu không có click thêm
        c.StopAllCoroutines();
        c.StartCoroutine(ReturnToIdleAfter(c, idleDelayAfterAttack));
    }

    private IEnumerator ReturnToIdleAfter(CharacterMovement c, float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            // Nếu click thêm → reset đếm lại
            if (comboResetTimer < 0.1f)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        EndAttack(c);
    }

    private void EndAttack(CharacterMovement c)
    {
        if (!isAttacking) return;

        isAttacking = false;
        currentStep = 0;
        c.animator.ResetTrigger("Attack");
        c.animator.SetFloat("Speed", 0f);
        c.SwitchState(c.idleState);
    }

    // 🕹️ Cho phép di chuyển trong lúc đang Attack
    private void HandleMovementWhileAttacking(CharacterMovement c)
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new(h, 0, v);
        if (move.magnitude > 0.1f)
        {
            // Xoay theo hướng camera
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + c.mainCamera.transform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
            c.transform.rotation = Quaternion.Lerp(c.transform.rotation, rotation, Time.deltaTime * c.rotationSpeed);

            // Di chuyển theo hướng nhìn
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            c.controller.Move(moveDir * c.walkSpeed * Time.deltaTime);

            // Blend với animation di chuyển
            c.animator.SetFloat("Speed", 1f);
        }
        else
        {
            c.animator.SetFloat("Speed", 0f);
        }
    }
}
