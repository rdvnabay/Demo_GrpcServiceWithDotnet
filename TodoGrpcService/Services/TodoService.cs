using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TodoGrpcService.Data;
using TodoGrpcService.Models;
using TodoGrpcService.Protos;

namespace TodoGrpcService.Services;

public class TodoService : TodoIt.TodoItBase
{
    private readonly AppDbContext _dbContext;

    public TodoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<AddTodoResponse> AddTodo(AddTodoRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        Todo todo = new()
        {
            Title = request.Title,
            Description = request.Description,
        };

        await _dbContext.Todos.AddAsync(todo);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new AddTodoResponse()
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            Status = todo.Status,
        });
    }

    public override async Task<UpdateTodoResponse> UpdateTodo(UpdateTodoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var todo = await _dbContext.Todos.FindAsync(request.Id);

        if (todo is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Todo item not found"));

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.Status = request.Status;

        _dbContext.Todos.Update(todo);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new UpdateTodoResponse()
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            Status = todo.Status,
        });
    }

    public override async Task<DeleteTodoResponse> DeleteTodo(DeleteTodoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var todo = await _dbContext.Todos.FindAsync(request.Id);

        if (todo is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Todo item not found"));

        _dbContext.Todos.Remove(todo);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new DeleteTodoResponse()
        {
            Id = todo.Id,
        });
    }

    public override async Task<ReadTodoResponse> ReadTodo(ReadTodoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var todo = await _dbContext.Todos.FindAsync(request.Id);

        if (todo is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Todo item not found"));

        return await Task.FromResult(new ReadTodoResponse()
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            Status = todo.Status
        });
    }

    public override async Task<ReadTodoListResponse> ReadTodoList(ReadTodoListRequest request, ServerCallContext context)
    {
        ReadTodoListResponse response = new();
        var todoList = await _dbContext.Todos.ToListAsync();

        foreach (var todo in todoList)
        {
            response.ToDo.Add(new ReadTodoResponse ()
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Status = todo.Status
            });
        }

        return await Task.FromResult(response);
    }
}
