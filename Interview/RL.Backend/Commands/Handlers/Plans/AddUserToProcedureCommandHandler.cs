using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

/// <summary>
///  Add user to procedure
/// </summary>
public class AddUserToProcedureCommandHandler : IRequestHandler<AddUserToProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;
    private readonly ILogger<AddProcedureToPlanCommandHandler> _logger;

    public AddUserToProcedureCommandHandler(RLContext context, ILogger<AddProcedureToPlanCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<Unit>> Handle(AddUserToProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Validate request
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

            var procedure = await _context.Procedures
                .Include(p => p.ProcedureUsers)
                .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);            

            if (procedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));

            var user = await _context.Users
                            .FirstOrDefaultAsync(p => p.UserId == request.UserId);

            if(user is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));

            if (user is not null && !procedure.ProcedureUsers.Any(p => p.UserId == user.UserId))
            {
                procedure.ProcedureUsers.Add(new ProcedureUser
                {
                    UserId = user.UserId
                });
                _logger.LogInformation($"Added user {request.UserId} for procedure: {request.ProcedureId}");
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch(OperationCanceledException e)
        {
            _logger.LogError(e, $"Error While adding user: {request.UserId} to procedure: {request.ProcedureId}");
            return ApiResponse<Unit>.Fail(e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error While adding user: {request.UserId} to procedure: {request.ProcedureId}");
            return ApiResponse<Unit>.Fail(e);
        }
    }
}