using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected StateMachine stateMachine;
    protected Entity monster;

    // Constructor to set the state machine and monster references
    public State(StateMachine stateMachine, Entity monster)
    {
        this.stateMachine = stateMachine;
        this.monster = monster;
    }

    // Called when the state is entered
    public virtual void EnterState() { }

    // Called on each frame while the state is active
    public abstract void Update();

    // Called when the state is exited
    public virtual void OnExit() { }
}

public class IdleState : State
{
    public IdleState(StateMachine stateMachine, Entity monster) : base(stateMachine, monster) { }

    public override void EnterState()
    {
        Debug.Log("Entering Idle State");
        stateMachine.RunCoroutine(WaitAndStalk());
    }

    public override void Update()
    {
        monster.gameObject.transform.position = new Vector3(-1000f, -1000f, -1000f);
    }

    // Coroutine to wait before transitioning to the Stalk state
    private IEnumerator WaitAndStalk()
    {
        while(monster.aggro < 20f){
            yield return null;
        }
        yield return new WaitForSeconds(5);  // Wait 5 seconds
        stateMachine.SetState(new StalkState(stateMachine, monster));  // Transition to StalkState
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Idle State");
    }
}

//Stalking
public class StalkState : State{

    public StalkState(StateMachine stateMachine, Entity monster) : base(stateMachine, monster) { }

    public override void EnterState()
    {
        Debug.Log("Entering Idle State");
        stateMachine.RunCoroutine(BeginStalking());
    }

    public override void Update()
    {

    }

    // Coroutine to wait before transitioning to the Stalk state
    private IEnumerator BeginStalking()
    {
        while(monster.aggro < 80f){
            yield return null;
        }
        yield return new WaitForSeconds(5);  // Wait 5 seconds
        stateMachine.SetState(new ChaseState(stateMachine, monster));  // Transition to StalkState
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Idle State");
    }    
}

//Chasing
public class ChaseState : State{

    public ChaseState(StateMachine stateMachine, Entity monster) : base(stateMachine, monster) { }

    public override void EnterState()
    {
        Debug.Log("Entering Idle State");
        stateMachine.RunCoroutine(BeginStalking());
    }

    public override void Update()
    {

    }

    // Coroutine to wait before transitioning to the Stalk state
    private IEnumerator BeginStalking()
    {
        while(monster.aggro >= 80f){
            yield return null;
        }
        monster.aggro = 0;
        stateMachine.SetState(new IdleState(stateMachine, monster));  // Transition to StalkState
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Idle State");
    }    
}

