﻿using Application.Dtos;
using Application.ViewModels;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Api.Tests;

[TestFixture]
public class TaskControllerTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    private string _token;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();

        _token = GenerateToken();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetAll_ShouldReturnAllTasks()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<GetTaskVm[]>();
        Assert.That(tasks?.Length, Is.AtLeast(1));
    }

    [Test]
    public async Task GetById_ShouldReturnTaskById()
    {
        // Arrange
        var getAllResponse = await _client.GetAsync("/api/tasks");
        var tasks = await getAllResponse.Content.ReadFromJsonAsync<GetTaskVm[]>();
        Assert.That(tasks?.Length, Is.AtLeast(1));

        // Act
        var response = await _client.GetAsync($"/api/tasks/{tasks[0].TaskId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<GetTaskVm>();
        Assert.That(task?.TaskId, Is.EqualTo(tasks[0].TaskId));
    }

    [Test]
    public async Task Create_ShouldReturnTaskId()
    {
        // Arrange
        var stringDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        var taskName = $"Task {stringDate}";
        var taskDescription = $"Description {stringDate}";
        var taskStatus = TaskEntityStatus.ToDo;
        var newTask = new CreateTaskVm { TaskName = taskName, TaskDescription = taskDescription, TaskStatus = taskStatus };
        var jsonContent = JsonSerializer.Serialize(newTask);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/tasks", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var taskId = await response.Content.ReadFromJsonAsync<int>();
        Assert.That(taskId, Is.AtLeast(1));
    }

    [Test]
    public async Task Update_ShouldUpdatedTask()
    {
        // Arrange
        var stringDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        var taskName = $"Task {stringDate}";
        var taskDescription = $"Description {stringDate}";
        var taskStatus = TaskEntityStatus.ToDo;
        var newTask = new TaskEntityDto { TaskName = taskName, TaskDescription = taskDescription, TaskStatus = taskStatus };
        var newJsonContent = JsonSerializer.Serialize(newTask);
        var newContent = new StringContent(newJsonContent, System.Text.Encoding.UTF8, "application/json");

        var postResponse = await _client.PostAsync($"/api/tasks", newContent);

        postResponse.EnsureSuccessStatusCode();
        var taskId = await postResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var updateTask = new UpdateTaskVm { TaskId = taskId, TaskStatus = TaskEntityStatus.Completed };
        var updateJsonContent = JsonSerializer.Serialize(updateTask);
        var updateContent = new StringContent(updateJsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/tasks", updateContent);

        // Assert
        response.EnsureSuccessStatusCode();
        var getResponse = await _client.GetAsync($"/api/tasks/{taskId}");
        var task = await getResponse.Content.ReadFromJsonAsync<GetTaskVm>();
        Assert.That(task?.TaskStatus, Is.EqualTo(updateTask.TaskStatus));

    }

    [Test]
    public async Task Delete_ShouldUpdatedTask()
    {
        // Arrange
        var stringDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        var taskName = $"Task {stringDate}";
        var taskDescription = $"Description {stringDate}";
        var taskStatus = TaskEntityStatus.ToDo;
        var newTask = new TaskEntityDto { TaskName = taskName, TaskDescription = taskDescription, TaskStatus = taskStatus };
        var newJsonContent = JsonSerializer.Serialize(newTask);
        var newContent = new StringContent(newJsonContent, System.Text.Encoding.UTF8, "application/json");

        var postResponse = await _client.PostAsync($"/api/tasks", newContent);

        postResponse.EnsureSuccessStatusCode();
        var taskId = await postResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{taskId}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    private static string GenerateToken()
    {
        string userName = "TestUser";

        var key = Environment.GetEnvironmentVariable("JwtSettings_Key") ?? string.Empty;
        var issuer = Environment.GetEnvironmentVariable("JwtSettings_Issuer") ?? string.Empty;
        var audience = Environment.GetEnvironmentVariable("JwtSettings_Audience") ?? string.Empty;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, userName),
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(1),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

}