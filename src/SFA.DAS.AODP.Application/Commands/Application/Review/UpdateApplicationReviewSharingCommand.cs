using MediatR;
using SFA.DAS.AODP.Models.Application;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class UpdateApplicationReviewSharingCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public UserType ApplicationReviewUserType { get; set; }
        public bool ShareApplication { get; set; }
        public Guid ApplicationReviewId { get; set; }
    }
}