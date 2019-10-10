using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityAITask {

  private int mutexBits;

  public EntityAITask() {

  }

  public abstract bool CanExecute();
  public abstract bool ShouldContinueExecuting();
  public abstract void StartExecution();
  public abstract void UpdateExecution();
  public abstract void ResetTask();
  public abstract bool IsInterruptible();

  public void SetMutexBits(int bits) {
    mutexBits = bits;
  }

  public int GetMutexBits() {
    return mutexBits;
  }

}
