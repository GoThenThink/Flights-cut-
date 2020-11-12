namespace Migrations
{
    internal static class ReturnFlightsConst
    {
        internal const string Table = "return_flights";

        internal const string FlightToId = "flight_to_id";
        internal const string FlightFromId = "flight_from_id";
        internal const string Priority = "priority";
        internal const string IsAvailable = "is_available";
        internal const string IsNativeReturnFlight = "is_native_return_flight";

        // Indexes
        internal const string IxPk = "ix_return_flights_pk";

        // Foreign Keys
        internal const string FkFlightTo = "fk_return_flights_flight_to_id";
        internal const string FkFlightFrom = "fk_return_flights_flight_from_id";
    }
}
