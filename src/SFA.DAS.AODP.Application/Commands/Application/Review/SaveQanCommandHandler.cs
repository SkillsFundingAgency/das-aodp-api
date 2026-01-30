using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Application;
using System;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQanCommandHandler : IRequestHandler<SaveQanCommand, BaseMediatrResponse<SaveQanCommandResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationReviewRepository _reviewRepository;
        private readonly IMediator _mediator;
        private readonly IQanValidationService _qanValidationService;

        public SaveQanCommandHandler(IApplicationRepository repository, IMediator mediator, IApplicationReviewRepository reviewRepository, IQanValidationService qanValidationService)
        {
            _applicationRepository = repository;
            _mediator = mediator;
            _reviewRepository = reviewRepository;
            _qanValidationService = qanValidationService;
        }

        public async Task<BaseMediatrResponse<SaveQanCommandResponse>> Handle(SaveQanCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<SaveQanCommandResponse>
            { 
                Success = false,
                Value = new SaveQanCommandResponse()
            };

            try
            {
                var review = await _reviewRepository.GetByIdAsync(request.ApplicationReviewId);
                var application = await _applicationRepository.GetByIdAsync(review.ApplicationId);

                string? currentQan = string.IsNullOrWhiteSpace(application.QualificationNumber)
                    ? null
                    : application.QualificationNumber.Trim();

                string? newQan = string.IsNullOrWhiteSpace(request.Qan)
                    ? null
                    : request.Qan.Trim();

                var qanChanged = !string.Equals(currentQan, newQan, StringComparison.OrdinalIgnoreCase);

                if (qanChanged && newQan is not null)
                {
                    var validation = await _qanValidationService.ValidateAsync(
                        request.Qan,
                        application.Name,
                        application.AwardingOrganisationName,
                        cancellationToken);

                    if (!validation.IsValid)
                    {
                        response.Value = new()
                        {
                            IsQanValid = false,
                            QanValidationMessage = validation.ValidationMessage ?? "Invalid QAN"
                        };
                        response.Success = true;
                        return response;
                    }
                }

                string previousQan = currentQan ?? "N/A";
                application.QualificationNumber = newQan;

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