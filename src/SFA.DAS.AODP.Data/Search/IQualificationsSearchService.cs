using SFA.DAS.AODP.Data.Entities.Qualification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Data.Search
{
    public interface IQualificationsSearchService
    {
        Task<IEnumerable<QualificationSearchResult>> SearchQualificationsByKeywordAsync(string input, int take = 25, CancellationToken ct = default);

        //IEnumerable<QualificationSearchResultDto> SearchQualificationsByKeyword(string keyword);
    }
}
