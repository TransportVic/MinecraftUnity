using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAITasks {

  private List<EntityAITask> tasks = new List<EntityAITask>();
  private List<EntityAITask> executingTasks = new List<EntityAITask>();
  private int tickCount = 0;
  private int executeFrequency = 2;

  public EntityAITasks() {

  }

  public void AddTask(EntityAITask task) {
    task.ResetTask();
    tasks.Add(task);
  }

  public void Update() {
    if (tickCount++ % executeFrequency == 0) {
      List<EntityAITask> toExecute = new List<EntityAITask>();
      foreach (EntityAITask task in tasks) {
        if (executingTasks.IndexOf(task) != -1) { // task is already executing
          if (task.ShouldContinueExecuting() && CanExecute(task)) continue;

          task.ResetTask();
          executingTasks.Remove(task);
        } else {
          if (task.CanExecute() && CanExecute(task))
            toExecute.Add(task);
        }
      }

      foreach (EntityAITask task in toExecute) {
        task.StartExecution();
        executingTasks.Add(task);
      }

      foreach(EntityAITask task in executingTasks) {
        task.UpdateExecution();
      }
    }
  }

  private bool CanExecute(EntityAITask task) {
    int taskPriority = 0;
    int givenTaskPriority = tasks.IndexOf(task);
    foreach (EntityAITask taskToCheck in tasks.GetRange(0, tasks.IndexOf(task))) {
      if (givenTaskPriority > taskPriority++) {
        if (executingTasks.IndexOf(taskToCheck) != -1 && !this.AreTasksCompatible(task, taskToCheck)) {
          return false;
        }
      } else if (executingTasks.IndexOf(taskToCheck) != -1 && !taskToCheck.IsInterruptible()) {
        return false;
      }
    }

    return true;
  }

  private bool AreTasksCompatible(EntityAITask task1, EntityAITask task2) {
    return (task1.GetMutexBits() & task2.GetMutexBits()) == 0;
  }

}
