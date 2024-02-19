﻿using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.Enums;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class TaskRepository : Repository, ITaskRepository
{
    public TaskRepository(SqlConnection context, SqlTransaction transaction)
    {
        Context = context;
        Transaction = transaction;
    }

    public IEnumerable<TaskEntity> GetAll()
    {
        var result = new List<TaskEntity>();

        var command = CreateCommand("SELECT * FROM task");

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(new TaskEntity
                {
                    TaskId = Convert.ToInt32(reader["TaskId"]),
                    TaskName = Convert.ToString(reader["TaskName"]) ?? "",
                    TaskDescription = Convert.ToString(reader["TaskDescription"]) ?? "",
                    TaskStatus = (TaskEntityStatus)Enum.Parse(typeof(TaskEntityStatus), Convert.ToString(reader["TaskStatus"]) ?? nameof(TaskEntityStatus.ToDo))
                });
            }
        }

        return result;
    }

    public TaskEntity Get(int id)
    {
        var command = CreateCommand("SELECT * FROM Task WHERE TaskId = @TaskId");
        command.Parameters.AddWithValue("@TaskId", id);

        using (var reader = command.ExecuteReader())
        {
            reader.Read();

            return new TaskEntity
            {
                TaskId = Convert.ToInt32(reader["TaskId"]),
                TaskName = Convert.ToString(reader["TaskName"]) ?? "",
                TaskDescription = Convert.ToString(reader["TaskDescription"]) ?? "",
                TaskStatus = (TaskEntityStatus)Enum.Parse(typeof(TaskEntityStatus), Convert.ToString(reader["TaskStatus"]) ?? nameof(TaskEntityStatus.ToDo))
            };
        };
    }

    public int Create(TaskEntity model)
    {
        var query = "INSERT INTO Task (TaskName, TaskDescription, TaskStatus) VALUES (@TaskName, @TaskDescription, @TaskStatus); SELECT SCOPE_IDENTITY();";
        var command = CreateCommand(query);

        command.Parameters.AddWithValue("@TaskName", model.TaskName);
        command.Parameters.AddWithValue("@TaskDescription", model.TaskDescription);
        command.Parameters.AddWithValue("@TaskStatus", model.TaskStatus);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(TaskEntity model)
    {
        var query = "UPDATE Task set TaskName = @TaskName, TaskDescription = @TaskDescription, TaskStatus = @TaskStatus WHERE TaskId = @TaskId";
        var command = CreateCommand(query);

        command.Parameters.AddWithValue("@TaskId", model.TaskId);
        command.Parameters.AddWithValue("@TaskName", model.TaskName);
        command.Parameters.AddWithValue("@TaskDescription", model.TaskDescription);
        command.Parameters.AddWithValue("@TaskStatus", model.TaskStatus);

        command.ExecuteNonQuery();
    }

    public void Remove(int taskId)
    {
        var command = CreateCommand("DELETE FROM Task WHERE TaskId = @TaskId");
        command.Parameters.AddWithValue("@TaskId", taskId);

        command.ExecuteNonQuery();
    }

}