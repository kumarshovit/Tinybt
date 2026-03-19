using Mediator;
using TinyBtUrlApi.Core.DTOs;

public sealed record GetUsersOverTimeQuery(
    DateTime Start,
    DateTime End,
    string ViewType
) : IRequest<List<UsersOverTimeDto>>;
