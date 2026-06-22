using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator Enemyanimator;

    void Start()
    {
        Enemyanimator = GetComponent<Animator>();
    }

    public IEnumerator atk()
    {
        Enemyanimator.SetTrigger("Atk");
        yield return null;
    }

    public IEnumerator def()
    {
        Enemyanimator.SetTrigger("Hit");
        yield return null;
    }

    public IEnumerator dead()
    {
        Enemyanimator.SetTrigger("Dead");
        yield return new WaitForSeconds(3f);
    }
}
