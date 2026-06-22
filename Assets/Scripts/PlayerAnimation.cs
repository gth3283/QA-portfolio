using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator PlayerAnimator;
    private Transform moveTarget;
    private float attackMoveDistance = 0.35f;
    private Vector3 originPosition;
    public GameObject PlayerAtkParticle;
    public GameObject EnemyAtkParticle;

    void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        moveTarget = transform;
        originPosition = moveTarget.localPosition;
    }

    public IEnumerator atk()
    {
        PlayerAnimator.SetTrigger("Atk");
        yield return StartCoroutine(MoveAttackForwardAndBack());
    }

    public IEnumerator def()
    {
        PlayerAnimator.SetTrigger("Hit");
        yield return new WaitForSeconds(0.5f);
        EnemyAtkParticle.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        EnemyAtkParticle.SetActive(false);
    }

    private IEnumerator MoveAttackForwardAndBack()
    {
        Vector3 start = originPosition;
        Vector3 forward = originPosition + Vector3.right * attackMoveDistance;

        float time = 0f;

        while (time < 0.4f)
        {
            time += Time.deltaTime;
            float a = Mathf.Clamp01(time / 0.4f);

            moveTarget.localPosition = Vector3.Lerp(start, forward, a);

            yield return null;
        }

        moveTarget.localPosition = forward;
        PlayerAtkParticle.SetActive(true);

        time = 0f;

        while (time < 0.4f)
        {
            time += Time.deltaTime;
            float a = Mathf.Clamp01(time / 0.4f);

            moveTarget.localPosition = Vector3.Lerp(forward, start, a);

            yield return null;
        }

        moveTarget.localPosition = start;
        PlayerAtkParticle.SetActive(false);
    }
}
