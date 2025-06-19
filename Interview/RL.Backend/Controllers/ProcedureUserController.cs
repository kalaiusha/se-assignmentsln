using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ProcedureUserController : ControllerBase
{
    private readonly ILogger<ProcedureUserController> _logger;
    private readonly RLContext _context;

    public ProcedureUserController(ILogger<ProcedureUserController> logger, RLContext context)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [EnableQuery]
    public IEnumerable<ProcedureUser> Get()
    {
        return _context.ProcedureUsers;
    }
}
