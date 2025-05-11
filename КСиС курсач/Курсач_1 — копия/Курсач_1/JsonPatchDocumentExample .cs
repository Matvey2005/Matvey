using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Filters;
using Курсач_1.Models;

namespace Курсач_1
{
    public class JsonPatchDocumentExample : IExamplesProvider<JsonPatchDocument<Event>>
    {
        public JsonPatchDocument<Event> GetExamples()
        {
            var patchDoc = new JsonPatchDocument<Event>();
            patchDoc.Operations.Add(new Operation<Event>
            {
                op = "replace",
                path = "/Description",
                value = "Новое название"
            });
            patchDoc.Operations.Add(new Operation<Event>
            {
                op = "replace",
                path = "/Time",
                value = "2025-05-09T15:00:00"
            });
            return patchDoc;
        }
    }
}
