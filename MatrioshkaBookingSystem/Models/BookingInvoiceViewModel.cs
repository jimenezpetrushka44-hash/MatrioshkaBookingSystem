namespace MatrioshkaBookingSystem.Models
{
    public class BookingInvoiceViewModel
    {
        public Booking Booking { get; set; } = null!;
        public Billinginfo BillingInfo { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public Hotel Hotel { get; set; } = null!;

        public decimal RoomPrice { get; set; }
        public int NumberOfNights { get; set; }
        public decimal TotalExtraAssetsPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDue { get; set; }
    }
}
