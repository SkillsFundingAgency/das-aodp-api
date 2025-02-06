using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder;

public class MoveQuestionUpCommand : IRequest<MoveQuestionUpCommandResponse>
{
    public Guid QuestionId { get; set; }
    public Guid PageId { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
}
