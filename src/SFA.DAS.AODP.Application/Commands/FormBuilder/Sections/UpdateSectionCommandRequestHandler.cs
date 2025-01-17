using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class UpdateSectionCommandRequestHandler : IRequestHandler<UpdateSectionCommandRequest, UpdateSectionCommandResponse>
{
    public UpdateSectionCommandRequestHandler()
    {
    }

    public async Task<UpdateSectionCommandResponse> Handle(UpdateSectionCommandRequest request, CancellationToken cancellationToken)
    {
        var response = new UpdateSectionCommandResponse();
        response.Success = false;

        try
        {
            //var section = _sectionRepository.GetById(request.Id);

            //if (section == null)
            //{
            //    response.Success = false;
            //    response.ErrorMessage = $"Section with id '{section!.Id}' could not be found.";
            //    return response;
            //}

            //section.Order = request.Order;
            //section.Title = request.Title;
            //section.Description = request.Description;
            //section.NextSectionId = request.NextSectionId;

            //_sectionRepository.Update(section);
            //response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
