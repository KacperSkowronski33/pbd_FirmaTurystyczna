using BookingApp.Shared.DTOs.RezerwacjaDto;
using FluentValidation;

namespace BookingApp.Api.Validators
{
    public class CreateRezerwacjaValidator : AbstractValidator<CreateRezerwacjaDto>
    {
        public CreateRezerwacjaValidator() {
            RuleFor(x => x.KlientId)
                .GreaterThan(0).WithMessage("Wybierz poprawnego klienta z listy.");

            RuleFor(x => x.TerminCenaId)
                .GreaterThan(0).WithMessage("Wybierz poprawną ofertę/termin.");

            RuleFor(x => x.LiczbaOsob)
                .GreaterThan(0).WithMessage("Rezerwacja musi obejmować minimum 1 osobę.");
        }
    }
}
