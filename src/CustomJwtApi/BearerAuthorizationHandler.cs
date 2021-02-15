using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CustomJwtApi
{
    public class BearerAuthorizationHandler : AuthorizationHandler<BearerRequirement>
    {
        public readonly SomeValidationContext _thatYouCanInject;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerAuthorizationHandler(IHttpContextAccessor httpContextAccessor, SomeValidationContext thatYouCanInject)
        {
            _thatYouCanInject = thatYouCanInject;

            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BearerRequirement requirement)
        {
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.Contains("Bearer"))
            {
                var token = authHeader.Replace("Bearer ", string.Empty);

                if (await requirement.IsTokenValid(_thatYouCanInject, token))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}