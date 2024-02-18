using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Persistence.Tests;

public class Tests
{
    private ITaskRepository _taskRepository;
    private Mock<ITaskRepository> _mockTaskRepository;

    [SetUp]
    public void Setup()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _taskRepository = _mockTaskRepository.Object;
    }

    [Test]
    public void GetAll_ShouldReturnAllTasks()
    {
        // Arrange
        var expectedTasks = new List<TaskEntity> { };
        _mockTaskRepository.Setup(repository => repository.GetAll()).Returns(expectedTasks);

        // Act
        var tasks = _taskRepository.GetAll();

        // Assert
        Assert.That(tasks, Is.EqualTo(expectedTasks));
    }


    [Test]
    public void Get_ShouldReturnTaskById()
    {
        // Arrange
        var taskId = 1;
        var taskName = "Task 1";
        var taskDescription = "Description 1";
        var taskStatus = TaskEntityStatus.ToDo;
        var expectedTask = new TaskEntity { TaskId = taskId, TaskName = taskName, TaskDescription = taskDescription, TaskStatus = taskStatus };
        _mockTaskRepository.Setup(repo => repo.Get(taskId)).Returns(expectedTask);

        // Act
        var result = _taskRepository.Get(taskId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTask));
    }


    [Test]
    public void Create_ShouldReturnAddedTask()
    {
        // Arrange
        var taskId = 1;
        var taskName = "Task 1";
        var taskDescription = "Description 1";
        var taskStatus = TaskEntityStatus.ToDo;
        var newTask = new TaskEntity { TaskId = taskId, TaskName = taskName, TaskDescription = taskDescription, TaskStatus = taskStatus };

        // Act
        _taskRepository.Create(newTask);

        // Assert
        _mockTaskRepository.Verify(repo => repo.Create(newTask), Times.Once);
    }

    [Test]
    public void Update_ShouldReturnUpdatedTask()
    {
        // Arrange
        var taskId = 1;
        var taskName = "Task 1";
        var taskDescription = "Description 1";
        var taskStatus = TaskEntityStatus.ToDo;
        var updatedTask = new TaskEntity { TaskId = taskId, TaskName = taskName, TaskDescription = taskDescription, TaskStatus = taskStatus };

        // Act
        _taskRepository.Update(updatedTask);

        // Assert
        _mockTaskRepository.Verify(repo => repo.Update(updatedTask), Times.Once);
    }

    [Test]
    public void Remove_ShouldReturnUpdatedTask()
    {
        // Arrange
        var taskId = 1;

        // taskId
        _taskRepository.Remove(taskId);

        // Assert
        _mockTaskRepository.Verify(repo => repo.Remove(taskId), Times.Once);
    }

}