# Custom validation of a JWT token

## `/src/CustomJwtApi`

Use a requirement based approach.

Add a `Requirement` by implementing the `IAuthorizationRequirement`.

This is invoked through a `AuthorizationHandler`. The `AuthorizationHandler` can accept some type of additional input. For example, configuration.

Finally, it can be added as a requirement when the middleware is configured in the start-up.

Then decorate the controller to ensure the requirement is fulfilled:

`[Authorize(Policy = "Bearer", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]`

## Supporting material

[Stackoverflow](https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core)
