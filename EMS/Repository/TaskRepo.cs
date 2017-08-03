using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;

namespace EMS.Repository
{
    public class TaskRepo
    {
        public static List<TaskModel> GetTaskList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from task in datacontext.Tasks
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

        public static Task GetTaskById(int t_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from task in datacontext.Tasks
                            where task.id == t_id
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


    }
}