using ChipAccess.Api.DTOs.Reassignment;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Mappers
{
    public static class ReassignmentMapper
    {
        public static ReassignmentResponseDto ToDto(this ReassignmentQueue entity)
        {
            return new ReassignmentResponseDto
            {
                Id = entity.Id,
                OldBamId = entity.OldBamId,
                NewBamId = entity.NewBamId,
                RequestedBy = entity.RequestedBy,
                RequestedAt = entity.RequestedAt,
                Processed = entity.Processed,
                ProcessedAt = entity.ProcessedAt
            };
        }
    }
}