using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.ExceptionMiddleware.Invoke(Microsoft.AspNetCore.Http.HttpContext)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.Controllers.ChargesApiController.AddBatch(System.String,System.String,System.Collections.Generic.IEnumerable{ChargesApi.V1.Boundary.Request.AddChargeRequest})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.Controllers.ChargesApiController.Post(System.String,System.String,ChargesApi.V1.Boundary.Request.AddChargeRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.Controllers.EstimatesActualUploadController.Post(System.String,System.String,System.Guid,ChargesApi.V1.Boundary.Request.AddEstimatesRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.Controllers.ChargesApiController.Patch(System.String,System.String,System.Guid,System.Guid,Microsoft.AspNetCore.JsonPatch.JsonPatchDocument{ChargesApi.V1.Boundary.Request.UpdateChargeRequest})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage(category: "Naming", checkId: "CA1724:Type names should not match namespaces", Justification = "<Pending>", Scope = "type", Target = "~T:ChargesApi.V1.Domain.Results")]
[assembly: SuppressMessage("Reliability", "CA2008:Do not create tasks without passing a TaskScheduler", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.Gateways.DynamoDbGateway.GetChargesAsync(ChargesApi.V1.Boundary.Request.PropertyChargesQueryParameters)~System.Threading.Tasks.Task{System.Collections.Generic.IList{ChargesApi.V1.Domain.Charge}}")]
[assembly: SuppressMessage("Reliability", "CA2008:Do not create tasks without passing a TaskScheduler", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.Gateways.DynamoDbGateway.GetChargesAsync(ChargesApi.V1.Boundary.Request.PropertyChargesQueryParameters)~System.Collections.Generic.IList{ChargesApi.V1.Domain.Charge}")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.Gateways.DynamoDbGateway.RemoveRange(System.Collections.Generic.List{ChargesApi.V1.Domain.ChargeKeys})~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ChargesApi.V1.Gateways.DynamoDbGateway.RemoveRangeAsync(System.Collections.Generic.List{ChargesApi.V1.Domain.ChargeKeys})~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "<Pending>", Scope = "type", Target = "~T:ChargesApi.LambdaHandler")]
