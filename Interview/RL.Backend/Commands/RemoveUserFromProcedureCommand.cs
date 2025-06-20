using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands
{
    public class RemoveUserFromProcedureCommand : UserRequest, IRequest<ApiResponse<Unit>>
    {
    }
}