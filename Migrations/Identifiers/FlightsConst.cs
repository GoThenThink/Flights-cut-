namespace Migrations
{
    internal static class FlightsConst
    {
        internal const string Table = "flights";

        internal const string Id = "id";
        internal const string Name = "name";
        internal const string From = "from";
        internal const string To = "to";
        internal const string AirlineId = "airline_id";
        internal const string FlightTypeId = "flight_type_id";
        internal const string TouristInfoEditLock = "tourist_info_edit_lock";
        internal const string ReturnFlightSameAirlineRequired = "return_flight_same_airline_required";
        internal const string AlternativeSearch = "alternative_search";
        internal const string FlightDuration = "flight_duration";
        internal const string AirplaneModelId = "airplane_model_id";
        internal const string StatusId = "status_id";

        // Indexes
        internal const string IxPk = "ix_flights_pk";
        internal const string IxName = "ix_flights_name";
        internal const string IxFrom = "ix_flights_from";
        internal const string IxTo = "ix_flights_to";

        // Foreign Keys
        internal const string FkAirline = "fk_flights_airline_id";
        internal const string FkFlightType = "fk_flights_flight_type_id";
        internal const string FkAirplaneModel = "fk_flights_airplane_model_id";
        internal const string FkStatus = "fk_flights_status_id";
        internal const string FkFrom = "fk_flights_from";
        internal const string FkTo = "fk_flights_to";
    }
}
