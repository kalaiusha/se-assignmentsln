using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands
{
    public class AddUserToProcedureCommand : UserRequest,IRequest<ApiResponse<Unit>>
    {
    }
}