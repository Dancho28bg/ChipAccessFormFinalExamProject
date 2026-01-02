using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ChipAccess.Api.Services;
using ChipAccess.Api.Repositories;
using ChipAccess.Api.DTOs.Access;
using ChipAccess.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChipAccess.Tests.Services
{
    public class AccessServiceTests
    {
        private readonly Mock<IAccessRepository> _repo;
        private readonly Mock<IEmployeeRepository> _employeeRepo;
        private readonly Mock<IAuditLogService> _audit;
        private readonly Mock<IAccessArchiveRepository> _archiveRepo;
        private readonly AccessService _service;

        public AccessServiceTests()
        {
            _repo = new Mock<IAccessRepository>();
            _employeeRepo = new Mock<IEmployeeRepository>();
            _audit = new Mock<IAuditLogService>();
            _archiveRepo = new Mock<IAccessArchiveRepository>();

            _service = new AccessService(
                _repo.Object,
                _employeeRepo.Object,
                _audit.Object,
                _archiveRepo.Object,
                NullLogger<AccessService>.Instance
            );
        }

        [Fact]
        public async Task CreateFromDtoAsync_ShouldCreatePendingManagerRequest()
        {
            var dto = new CreateAccessRequestDto
            {
                Approver = "manager1",
                AccessNeeded = "VPN Access",
                Reason = "Work from home",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            await _service.CreateFromDtoAsync(dto, "user1");

            _repo.Verify(r => r.CreateAsync(
                It.Is<AccessApprovalForm>(a =>
                    a.BamId == "user1" &&
                    a.Approver == "manager1" &&
                    a.AccessNeeded == "VPN Access" &&
                    a.Reason == "Work from home" &&
                    a.Status == AccessStatus.PendingManager
                )),
                Times.Once);

            _audit.Verify(a => a.LogAsync(
                "user1",
                "Created Access Request",
                "AccessApprovalForm",
                It.IsAny<int>(),
                It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateFromDtoAsync_ShouldSetCreatedDate()
        {
            var dto = new CreateAccessRequestDto
            {
                Approver = "manager1",
                AccessNeeded = "File Share",
                Reason = "Project access",
                ExpirationDate = DateTime.UtcNow.AddDays(10)
            };

            await _service.CreateFromDtoAsync(dto, "user2");

            _repo.Verify(r => r.CreateAsync(
                It.Is<AccessApprovalForm>(a =>
                    a.CreatedDate >= DateTime.UtcNow.AddMinutes(-1)
                )),
                Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_Manager_ShouldMoveToPendingAdmin()
        {
            var form = new AccessApprovalForm
            {
                Id = 1,
                Status = AccessStatus.PendingManager
            };

            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(form);
            _repo.Setup(r => r.UpdateAsync(form)).ReturnsAsync(true);

            var result = await _service.ApproveAsync(1, "Manager", "manager1");

            Assert.True(result);
            Assert.Equal(AccessStatus.PendingAdmin, form.Status);

            _audit.Verify(a => a.LogAsync(
                "manager1",
                "Manager Approved Request",
                "AccessApprovalForm",
                1,
                "Moved to PendingAdmin"),
                Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_Admin_ShouldMoveToActive()
        {
            var form = new AccessApprovalForm
            {
                Id = 2,
                Status = AccessStatus.PendingAdmin
            };

            _repo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(form);
            _repo.Setup(r => r.UpdateAsync(form)).ReturnsAsync(true);

            var result = await _service.ApproveAsync(2, "Admin", "admin1");

            Assert.True(result);
            Assert.Equal(AccessStatus.Active, form.Status);

            _audit.Verify(a => a.LogAsync(
                "admin1",
                "Admin Approved Request",
                "AccessApprovalForm",
                2,
                "Request is now Active"),
                Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_AdminCannotApprovePendingManager()
        {
            var form = new AccessApprovalForm
            {
                Id = 3,
                Status = AccessStatus.PendingManager
            };

            _repo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(form);

            var result = await _service.ApproveAsync(3, "Admin", "admin1");

            Assert.False(result);
        }

        [Fact]
        public async Task RejectAsync_Manager_ShouldReject()
        {
            var form = new AccessApprovalForm
            {
                Id = 4,
                Status = AccessStatus.PendingManager
            };

            _repo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(form);
            _repo.Setup(r => r.UpdateAsync(form)).ReturnsAsync(true);

            var result = await _service.RejectAsync(4, "manager1", "Manager");

            Assert.True(result);
            Assert.Equal(AccessStatus.RejectedByManager, form.Status);
            Assert.Equal("manager1", form.RejectedBy);

            _audit.Verify(a => a.LogAsync(
                "manager1",
                "Rejected Access Request",
                "AccessApprovalForm",
                4,
                It.IsAny<string>()),
                Times.Once);
        }


       [Fact]
        public async Task RejectAsync_Admin_ShouldReject()
        {
            var form = new AccessApprovalForm
            {
                Id = 5,
                Status = AccessStatus.PendingAdmin
            };

            _repo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(form);
            _repo.Setup(r => r.UpdateAsync(form)).ReturnsAsync(true);

            var result = await _service.RejectAsync(5, "admin1", "Admin");

            Assert.True(result);
            Assert.Equal(AccessStatus.RejectedByAdmin, form.Status);
            Assert.Equal("admin1", form.RejectedBy);

            _audit.Verify(a => a.LogAsync(
                "admin1",
                "Rejected Access Request",
                "AccessApprovalForm",
                5,
                It.IsAny<string>()),
                Times.Once);
        }


        [Fact]
        public async Task RevokeAsync_ShouldRevokeAccess()
        {
            var form = new AccessApprovalForm
            {
                Id = 6,
                Status = AccessStatus.Active
            };

            _repo.Setup(r => r.GetByIdAsync(6)).ReturnsAsync(form);
            _repo.Setup(r => r.UpdateAsync(form)).ReturnsAsync(true);

            var result = await _service.RevokeAsync(6);

            Assert.True(result);
            Assert.Equal(AccessStatus.Revoked, form.Status);
            Assert.NotNull(form.RevokedDate);

            _audit.Verify(a => a.LogAsync(
                "SYSTEM",
                "Revoked Access",
                "AccessApprovalForm",
                6,
                "Archived after revocation"),
                Times.Once);
        }
    }
}
