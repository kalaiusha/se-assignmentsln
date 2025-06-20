using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

/// <summary>
///  Remove all users or selected user from procedure
/// </summary>
public class RemoveUserFromProcedureCommandHandler : IRequestHandler<RemoveUserFromProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;
    private readonly ILogger<RemoveUserFromProcedureCommandHandler> _logger;

    public RemoveUserFromProcedureCommandHandler(RLContext context, ILogger<RemoveUserFromProcedureCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<Unit>> Handle(RemoveUserFromProcedureCommand request, CancellationToken cancellationToken)
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

            // Remove all users
            if(request.UserId == -1)
            {
                procedure.ProcedureUsers.Clear();
                _logger.LogInformation($"Remove All for procedure: {request.ProcedureId}");
            }
            else
            {
                var removeItem = procedure.ProcedureUsers.FirstOrDefault(procedure => procedure.UserId == request.UserId);
                if (removeItem != null)
                {
                    procedure.ProcedureUsers.Remove(removeItem);
                    _logger.LogInformation($"Remove user { request.UserId } for procedure: {request.ProcedureId}");
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (OperationCanceledException e)
        {
            _logger.LogError(e, $"Error While removing user: {request.UserId} to procedure: {request.ProcedureId}");
            return ApiResponse<Unit>.Fail(e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error While removing user: {request.UserId} to procedure: {request.ProcedureId}");
            return ApiResponse<Unit>.Fail(e);
        }
    }
}