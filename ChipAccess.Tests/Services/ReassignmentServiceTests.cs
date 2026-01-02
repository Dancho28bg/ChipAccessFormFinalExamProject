using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ChipAccess.Api.Services;
using ChipAccess.Api.Repositories;
using ChipAccess.Api.DTOs.Reassignment;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Tests.Services
{
    public class ReassignmentServiceTests
    {
        private readonly Mock<IReassignmentRepository> _repo;
        private readonly Mock<IAuditLogService> _audit;
        private readonly ReassignmentService _service;

        public ReassignmentServiceTests()
        {
            _repo = new Mock<IReassignmentRepository>();
            _audit = new Mock<IAuditLogService>();

            _service = new ReassignmentService(
                _repo.Object,
                _audit.Object);
        }

        [Fact]
        public async Task CreateJobFromDtoAsync_ShouldCreateJobAndAudit()
        {
            var dto = new CreateReassignmentDto
            {
                OldBamId = "oldBam",
                NewBamId = "newBam"
            };

            var result = await _service.CreateJobFromDtoAsync(dto, "requester1");

            _repo.Verify(r => r.CreateJobAsync(
                It.Is<ReassignmentQueue>(j =>
                    j.OldBamId == "oldBam" &&
                    j.NewBamId == "newBam" &&
                    j.RequestedBy == "requester1"
                )),
                Times.Once);

            _audit.Verify(a => a.LogAsync(
                "requester1",
                "Created Reassignment",
                "ReassignmentQueue",
                It.IsAny<int>(),
                It.IsAny<string>()),
                Times.Once);
        }


        [Fact]
        public async Task AcceptAsync_WhenJobNotFound_ReturnsFalse()
        {
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync((ReassignmentQueue?)null);

            var result = await _service.AcceptAsync(1, "newBam");

            Assert.False(result);
        }

        [Fact]
        public async Task AcceptAsync_WhenUserIsNotNewBam_ThrowsUnauthorized()
        {
            var job = new ReassignmentQueue
            {
                Id = 1,
                OldBamId = "oldBam",
                NewBamId = "newBam"
            };

            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(job);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.AcceptAsync(1, "someoneElse"));
        }

        [Fact]
        public async Task AcceptAsync_WhenValid_ShouldAcceptProcessAndAudit()
        {
            var job = new ReassignmentQueue
            {
                Id = 1,
                OldBamId = "oldBam",
                NewBamId = "newBam"
            };

            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);
            _repo.Setup(r => r.AcceptAsync(1)).ReturnsAsync(true);
            _repo.Setup(r => r.ProcessJobAsync(1)).ReturnsAsync(2);

            var result = await _service.AcceptAsync(1, "newBam");

            Assert.True(result);

            _repo.Verify(r => r.AcceptAsync(1), Times.Once);
            _repo.Verify(r => r.ProcessJobAsync(1), Times.Once);

            _audit.Verify(a => a.LogAsync(
                "newBam",
                "Accepted Reassignment",
                "ReassignmentQueue",
                1,
                It.IsAny<string>()),
                Times.Once);

            _audit.Verify(a => a.LogAsync(
                "SYSTEM",
                "Auto-Processed Reassignment",
                "ReassignmentQueue",
                1,
                It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task RejectAsync_WhenJobNotFound_ReturnsFalse()
        {
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync((ReassignmentQueue?)null);

            var result = await _service.RejectAsync(1, "newBam", "reason");

            Assert.False(result);
        }

        [Fact]
        public async Task RejectAsync_WhenUserIsNotNewBam_ThrowsUnauthorized()
        {
            var job = new ReassignmentQueue
            {
                Id = 1,
                OldBamId = "oldBam",
                NewBamId = "newBam"
            };

            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(job);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.RejectAsync(1, "someoneElse", "reason"));
        }

        [Fact]
        public async Task RejectAsync_WhenValid_ShouldRejectAndAudit()
        {
            var job = new ReassignmentQueue
            {
                Id = 1,
                OldBamId = "oldBam",
                NewBamId = "newBam"
            };

            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);
            _repo.Setup(r => r.RejectAsync(1, "no longer needed"))
                 .ReturnsAsync(true);

            var result = await _service.RejectAsync(1, "newBam", "no longer needed");

            Assert.True(result);

            _audit.Verify(a => a.LogAsync(
                "newBam",
                "Rejected Reassignment",
                "ReassignmentQueue",
                1,
                It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessJobAsync_WhenResultGreaterThanZero_ShouldAudit()
        {
            _repo.Setup(r => r.ProcessJobAsync(1))
                 .ReturnsAsync(3);

            var result = await _service.ProcessJobAsync(1);

            Assert.Equal(3, result);

            _audit.Verify(a => a.LogAsync(
                "SYSTEM",
                "Processed Reassignment",
                "ReassignmentQueue",
                1,
                It.IsAny<string>()),
                Times.Once);
        }
    }
}
