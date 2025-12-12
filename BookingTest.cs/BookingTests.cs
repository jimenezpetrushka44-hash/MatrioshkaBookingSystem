using MatrioshkaBookingSystem.Models;

namespace BookingTest.cs
{
    public class BookingTests
    {
        [Fact]
        public void Booking_WithValidDates_ShouldHavePositiveNights()
        {
            var booking = new Booking
            {
                DateofBooking = new DateOnly(2025, 5, 10),
                EndofBooking = new DateOnly(2025, 5, 15)
            };

            var nights = (booking.EndofBooking.ToDateTime(TimeOnly.MinValue)
                        - booking.DateofBooking.ToDateTime(TimeOnly.MinValue)).TotalDays;

            Assert.True(nights > 0);
        }
    }
    }