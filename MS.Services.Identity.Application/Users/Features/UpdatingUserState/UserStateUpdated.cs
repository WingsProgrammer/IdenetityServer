using MS.Services.Identity.Shared.Models;
using MsftFramework.Core.Domain.Events.External;

namespace MS.Services.Identity.Users.Features.UpdatingUserState;

public record UserStateUpdated(Guid UserId, UserState OldUserState, UserState NewUserState) : IntegrationEvent;
