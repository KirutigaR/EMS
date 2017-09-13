using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;
using LinqKit;

namespace EMS.Repository
{
    public class TaskRepo
    {
        public static List<TaskModel> GetTaskList(int project_id)
        {
            var predicate = LinqKit.PredicateBuilder.True<Task>();
            if(project_id != 0)
            {
                predicate = predicate.And(i => i.project_id == project_id);
            }
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from task in datacontext.Tasks.AsExpandable().Where(predicate)
                            select new TaskModel
                            {
                                id= task.id,
                                task_name = task.task_name,
                                task_description = task.task_description,
                                project_id = task.project_id
                            };
                return query.ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static void AssignProjectTask(Task task)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Tasks.Add(task);
                datacontext.SaveChanges();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static Task GetTaskById(int task_id)//t_id task_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from task in datacontext.Tasks
                            where task.id == task_id
                            select task;
                return query.FirstOrDefault();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static TaskModel GetTaskDetailsById(int task_id)//t_id task_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from task in datacontext.Tasks
                            where task.id == task_id
                            select new TaskModel
                            {
                                id = task.id,
                                task_name = task.task_name,
                                task_description = task.task_description,
                                project_id = task.project_id
                            };
                return query.FirstOrDefault();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static void EditTask(Task task)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(task).State = EntityState.Modified;
                datacontext.SaveChanges();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static void DeleteTaskById(Task instance)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(instance).State = EntityState.Deleted;
                datacontext.SaveChanges();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

    }
}