using System;
using System.Collections.Generic;
using AutoMapper;
using CommandAPI.Controllers;
using CommandAPI.Data;
using CommandAPI.Dtos;
using CommandAPI.Models;
using CommandAPI.Profiles;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CommandAPITests
{
    public class CommandsControllerTests : IDisposable
    {
        Mock<ICommandAPIRepo> mockRepo;
        IMapper mapper;
        CommandsProfile realProfile;
        MapperConfiguration configuration;
        public CommandsControllerTests()
        {
            mockRepo = new Mock<ICommandAPIRepo>();
            realProfile = new CommandsProfile();
            configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            mapper = new Mapper(configuration);
        }

        private IEnumerable<Command> GetCommands(int num)
        {
            var commands = new List<Command>();
            if (num > 0)
            {
                commands.Add(new Command
                {
                    Id = 0,
                    HowTo = "How to generate a migration",
                    CommandLine = "dotnet ef migrations add <Name of Migration>",
                    Platform = ".Net Core EF"
                });
            }
            return commands;
        }

        public void Dispose()
        {
            realProfile = null;
            mockRepo = null;
            mapper = null;
            configuration = null;
        }

        [Fact]
        public void DeleteCommand_Returns204NoContent_WhenValidResourceIDSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command { Id = 1 });

            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.DeleteCommand(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns404NotFound_WhenNonExistentResourceIDSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.DeleteCommand(0);

            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public void PartialCommandUpdate_Returns404NotFound_WhenNonExistentResourceIDSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.PartialCommandUpdate(0, new JsonPatchDocument<CommandUpdateDto> { });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateCommand_Returns204NoContent_WhenValidObjectSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command { Id = 1, HowTo = "mock", Platform = "Mock", CommandLine = "Mock" });
            mockRepo.Setup(repo => repo.SaveChanges()).Returns(true);

            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.UpdateCommand(1, new CommandUpdateDto { });

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateCommand_Returns404NotFound_WhenNonExistentResourceIDSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.UpdateCommand(0, new CommandUpdateDto { });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateCommand_ReturnsCorrectResourceType_WhenValidObjectSubmiited()
        {
            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.CreateCommand(new CommandCreateDto { });

            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void CreateCommand_Returns201Created_WhenValidObjectSubmiited()
        {
            mockRepo.Setup(repo => repo.SaveChanges()).Returns(true);

            var controller = new CommandsController(mockRepo.Object, mapper);
            var result = controller.CreateCommand(new CommandCreateDto { });

            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }

        [Fact]
        public void GetCommandById_Returns404NotFound_WhenNonExistentIDProvided()
        {
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);
            var commandsController = new CommandsController(mockRepo.Object, mapper);

            var result = commandsController.GetCommandById(0);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetCommandById_Returns200OK_WhenValidIDProvided()
        {
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(() => new Command { Id = 1, HowTo = "mock", Platform = "Mock", CommandLine = "Mock" });
            var commandsController = new CommandsController(mockRepo.Object, mapper);

            var result = commandsController.GetCommandById(1);

            Assert.IsType<OkObjectResult>(result.Result);
        }
        [Fact]
        public void GetCommandById_ReturnsCorrectResourceType_WhenValidIDProvided()
        {
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(() => new Command { Id = 1, HowTo = "mock", Platform = "Mock", CommandLine = "Mock" });
            var commandsController = new CommandsController(mockRepo.Object, mapper);

            var result = commandsController.GetCommandById(1);

            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void GetAllCommands_ReturnsZeroItems_WhenDBIsEmpty()
        {
            //arrange
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(0));
            var commandsController = new CommandsController(mockRepo.Object, mapper);
            //act
            var result = commandsController.GetAllCommands();

            //assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllCommands_ReturnsOneItem_WhenDBHasOneResource()
        {
            //A
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(1));
            var commandsController = new CommandsController(mockRepo.Object, mapper);

            //
            var result = commandsController.GetAllCommands();

            //
            var okResult = result.Result as OkObjectResult;
            var commands = okResult.Value as List<CommandReadDto>;
            Assert.Single(commands);
        }

        [Fact]
        public void GetAllCommands_Returns200Ok_WhenDBHasOneResource()
        {
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(0));
            var commandsController = new CommandsController(mockRepo.Object, mapper);

            var result = commandsController.GetAllCommands();

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllCommands_ReturnsCorrectType_WhenDBHasOneResource()
        {
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(0));
            var commandsController = new CommandsController(mockRepo.Object, mapper);

            var result = commandsController.GetAllCommands();

            Assert.IsType<ActionResult<IEnumerable<CommandReadDto>>>(result);
        }
    }
}
