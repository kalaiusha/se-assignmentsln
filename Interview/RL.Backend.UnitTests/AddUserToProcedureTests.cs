using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;

namespace RL.Backend.UnitTests;

[TestClass]
public class AddUserToProcedureTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task AddUserToProcedureTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AddUserToProcedureCommandHandler(context.Object);
        var request = new AddUserToProcedureCommand()
        {
            ProcedureId = procedureId,
            UserId = new List<int> { 1 }
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }


    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToProcedureTests_PlanIdNotFound_ReturnsNotFound(int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);
        var request = new AddUserToProcedureCommand()
        {
            ProcedureId = procedureId,
            UserId = new List<int> { 1 }
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = 1
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId + 1,
            ProcedureTitle = "Test Procedure"
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToProcedureTests_UserIdNotFound_ReturnsNotFound(int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);
        var request = new AddUserToProcedureCommand()
        {
            UserId = new List<int> { 1 },
            ProcedureId = procedureId
        };

        context.Users.Add(new Data.DataModels.User { UserId = procedureId + 1 });
;
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId + 1,
            ProcedureTitle = "Test Procedure"
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AddUserToProcedureTests_AlreadyContainsUser_ReturnsSuccess(int userId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);
        var request = new AddUserToProcedureCommand()
        {
            UserId = new List<int> { userId },
            ProcedureId = procedureId
        };

        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.ProcedureUsers.Add(new Data.DataModels.ProcedureUser
        {
            ProcedureId = procedureId,
            UserId = userId
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AddUserToProcedureTests_DoesntContainsUser_ReturnsSuccess(int userId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);
        var request = new AddUserToProcedureCommand()
        {
            UserId = new List<int> { userId },
            ProcedureId = procedureId
        };

        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbPlanProcedure = await context.ProcedureUsers.FirstOrDefaultAsync(pp => pp.UserId == userId && pp.ProcedureId == procedureId);

        dbPlanProcedure.Should().NotBeNull();

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AddUserToProcedureTests_RemoveUser_ReturnsSuccess(int userId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);
        var request = new AddUserToProcedureCommand()
        {
            UserId = new List<int> { userId },
            ProcedureId = procedureId
        };

        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId
        });
        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId + 1
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.ProcedureUsers.Add(new Data.DataModels.ProcedureUser
        {
            ProcedureId = procedureId,
            UserId = userId
        });
        context.ProcedureUsers.Add(new Data.DataModels.ProcedureUser
        {
            ProcedureId = procedureId,
            UserId = userId + 1
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbPlanProcedure = await context.ProcedureUsers.FirstOrDefaultAsync(pp => pp.UserId == userId  && pp.ProcedureId == procedureId);
        dbPlanProcedure.Should().NotBeNull();

        var dbPlanProcedure1 = await context.ProcedureUsers.FirstOrDefaultAsync(pp => pp.UserId == userId + 1 && pp.ProcedureId == procedureId);
        dbPlanProcedure1.Should().BeNull();

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AddUserToProcedureTests_RemoveAllUser_ReturnsSuccess(int userId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);
        var request = new AddUserToProcedureCommand()
        {
            UserId = new List<int>(),
            ProcedureId = procedureId
        };

        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId
        });
        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId + 1
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.ProcedureUsers.Add(new Data.DataModels.ProcedureUser
        {
            ProcedureId = procedureId,
            UserId = userId
        });
        context.ProcedureUsers.Add(new Data.DataModels.ProcedureUser
        {
            ProcedureId = procedureId,
            UserId = userId + 1
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbPlanProcedure = await context.ProcedureUsers.FirstOrDefaultAsync(pp => pp.UserId == userId && pp.ProcedureId == procedureId);
        dbPlanProcedure.Should().BeNull();

        var dbPlanProcedure1 = await context.ProcedureUsers.FirstOrDefaultAsync(pp => pp.UserId == userId + 1 && pp.ProcedureId == procedureId);
        dbPlanProcedure1.Should().BeNull();

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

}