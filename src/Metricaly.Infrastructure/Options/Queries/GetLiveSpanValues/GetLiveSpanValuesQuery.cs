using MediatR;
using Metricaly.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Options.Queries.GetLiveSpanValues
{
    public class LiveSpanValueDto
    {
        public string Label { get; set; }
        public int TotalSeconds { get; set; }
    }

    public class GetLiveSpanValuesQuery : IRequest<List<LiveSpanValueDto>>
    {
    }

    public class GetLiveSpanValuesQueryHandler : IRequestHandler<GetLiveSpanValuesQuery, List<LiveSpanValueDto>>
    {
        public async Task<List<LiveSpanValueDto>> Handle(GetLiveSpanValuesQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(LiveTimespan.Values.Select(x => new LiveSpanValueDto() { Label = x.Key, TotalSeconds = (int)x.Value.TotalSeconds }).ToList());
        }
    }
}
