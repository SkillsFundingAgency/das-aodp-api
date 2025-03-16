using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQanCommandHandler : IRequestHandler<SaveQanCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationReviewRepository _reviewRepository;
        private readonly IMediator _mediator;

        public SaveQanCommandHandler(IApplicationRepository repository, IMediator mediator, IApplicationReviewRepository reviewRepository)
        {
            _applicationRepository = repository;
            _mediator = mediator;
            _reviewRepository = reviewRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQanCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var review = await _reviewRepository.GetByIdAsync(request.ApplicationReviewId);
                var application = await _applicationRepository.GetByIdAsync(review.ApplicationId);

                string previousQan = application.QualificationNumber ?? "N/A";
                application.QualificationNumber = request.Qan;

                await _applicationRepository.UpdateAsync(application);

                var msgCommand = new CreateApplicationMessageCommand()
                {
                    ApplicationId = review.ApplicationId,
                    MessageText = $"Previous QAN: {previousQan}\nNew QAN: {request.Qan}",
                    SentByEmail = request.SentByEmail,
                    SentByName = request.SentByName,
                    UserType = UserType.Qfau.ToString(),
                    MessageType = MessageType.QanUpdated.ToString()
                };

                var msgResult = await _mediator.Send(msgCommand);
                if (!msgResult.Success) throw new Exception(msgResult.ErrorMessage, msgResult.InnerException);

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
                response.Success = false;
            }
            return response;
        }
    }

}