using UnityEngine;

public class UnitAnim : MonoBehaviour
{
    const string AP_MOVE_SPEED = "_MoveSpeed";
    const string AP_TURN_DIR = "_TurnDir";
    const string AP_TURN_SPEED = "_TurnSpeed";
    const string AP_ATTACK_TRIGGER = "_Attack";
    const string AP_DIE = "_Die";
    const string AP_WIN = "_Win";

    [SerializeField]
    UnitController unit;
    [SerializeField]
    UnitModel model;

    [SerializeField]
    Animator anim;

    private void Awake()
    {
        unit.OnAttackBegin += Unit_OnAttack;
        model.OnWin += Model_OnWin;
        //unit.OnAttackFinished += Unit_OnAttackFinished;
        model.OnDeath += Model_OnDeath;
    }

    private void Model_OnWin()
    {
        anim.SetTrigger(AP_WIN);
    }

    private void Model_OnDeath()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger(AP_DIE);
    }

    private void Unit_OnAttackFinished()
    {
        anim.applyRootMotion = false;
        anim.rootPosition = Vector3.zero;
        anim.transform.localPosition = Vector3.zero;
        anim.transform.localRotation = Quaternion.identity;
    }

    private void Unit_OnAttack()
    {
        anim.SetTrigger(AP_ATTACK_TRIGGER);
        //anim.applyRootMotion = true;
    }

    private void Update()
    {
        anim.SetFloat(AP_MOVE_SPEED, unit.MovementDir.magnitude);

        anim.SetFloat(AP_TURN_DIR, Mathf.InverseLerp(-1, 1, unit.AngularMovementDir));
        anim.SetFloat(AP_TURN_SPEED, Mathf.Abs(unit.AngularMovementDir));
    }
}
