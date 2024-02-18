﻿using Domain.Enums;

namespace Application.Dtos;

public class TaskDto
{
    public required int TaskId {  get; set; }
    public required string TaskName { get; set; }
    public required string TaskDescription { get; set; }
    public required TaskEntityStatus TaskStatus { get; set; }
}
