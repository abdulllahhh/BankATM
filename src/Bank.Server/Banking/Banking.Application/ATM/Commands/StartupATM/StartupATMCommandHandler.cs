using Banking.Application.Abstractions.Persistence;
using Banking.Domain.ATM.Errors;
using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Banking.Application.ATM.Commands.StartupATM;

internal sealed class StartupATMCommandHandler
    : IRequestHandler<StartupATMCommand, Result<StartupATMResponse>>
{
    private readonly IATMRepository _atmRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartupATMCommandHandler(IATMRepository atmRepository, IUnitOfWork unitOfWork)
    {
        _atmRepository = atmRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StartupATMResponse>> Handle(
        StartupATMCommand request,
        CancellationToken cancellationToken)
    {
        var atmId = ATMId.Create(request.ATMId);

        var atm = await _atmRepository.GetByIdAsync(atmId, cancellationToken);

        if (atm is null)
        {
            return Result<StartupATMResponse>.Failure(Errors.ATM.NotFound);
        }

        var domainResult = atm.Start();

        if (domainResult.IsFailure)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var error = MapError(domainResult.Error);
            return Result<StartupATMResponse>.Failure(error);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
        {
            return Result<StartupATMResponse>.Failure(saveResult.Error);
        }

        var response = new StartupATMResponse(
            atm.Id.Value,
            atm.Status);

        return Result<StartupATMResponse>.Success(response);
    }

    private static Error MapError(string domainError)
    {
        return domainError switch
        {
            ATMErrors.CannotStart => Errors.ATM.CannotStart,
            _ => Errors.General.Unexpected
        };
    }
}
