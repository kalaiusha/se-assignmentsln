using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

public class AddUserToProcedureCommandHandler : IRequestHandler<AddUserToProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public AddUserToProcedureCommandHandler(RLContext context)
    {
        _context = context;
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

            foreach(var item in request.UserId)
            {
                var user = await _context.Users
                            .FirstOrDefaultAsync(p => p.UserId == item);
                if(user is not null && !procedure.ProcedureUsers.Any(p => p.UserId == user.UserId))
                { 
                    procedure.ProcedureUsers.Add(new ProcedureUser
                    {
                        UserId = user.UserId
                    });
                }
            }

            List<ProcedureUser> remove = new List<ProcedureUser>();
            
            foreach(var data in procedure.ProcedureUsers)
            {
                if(!request.UserId.Contains(data.UserId))
                {
                    remove.Add(data);
                }
            }

            if(remove.Count > 0)
            {
                foreach(var data in remove)
                {
                    procedure.ProcedureUsers.Remove(data);
                }
            }

            await _context.SaveChangesAsync();

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}