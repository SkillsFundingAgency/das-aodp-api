﻿using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, DeleteSectionCommandResponse>
{
    public DeleteSectionCommandHandler()
    {
    }

    public async Task<DeleteSectionCommandResponse> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
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
