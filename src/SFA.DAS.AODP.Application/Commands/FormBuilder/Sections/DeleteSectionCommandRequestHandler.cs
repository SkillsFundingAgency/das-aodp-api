using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class DeleteSectionCommandRequestHandler : IRequestHandler<DeleteSectionCommandRequest, DeleteSectionCommandResponse>
{
    public DeleteSectionCommandRequestHandler()
    {
    }

    public async Task<DeleteSectionCommandResponse> Handle(DeleteSectionCommandRequest request, CancellationToken cancellationToken)
    {
        var response = new DeleteSectionCommandResponse();

        try
        {
            //var section = _sectionRepository.GetById(request.Id);
            //if (section == null)
            //{
            //    response.Success = false;
            //    response.ErrorMessage = $"Section with id '{section!.Id}' could not be found.";

            //    return response;
            //}

            //_sectionRepository.Delete(request.Id);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
